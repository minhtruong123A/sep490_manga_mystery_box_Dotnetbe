using BusinessObjects;
using BusinessObjects.Dtos.Exchange;
using BusinessObjects.Enum;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class ExchangeRepository : GenericRepository<ExchangeInfo>, IExchangeRepository
    {
        private readonly IMongoCollection<ExchangeInfo> _exchangeInfo;
        private readonly IMongoCollection<ExchangeProduct> _exchangeProduct;
        private readonly IMongoCollection<ExchangeSession> _exchangeSession;
        private readonly IMongoCollection<SellProduct> _sellProduct;
        private readonly IMongoCollection<UserProduct> _userProduct;
        private readonly IMongoCollection<Product> _product;
        private readonly IMongoCollection<ProductInMangaBox> _productInMangaBox;
        private readonly IMongoCollection<MangaBox> _mangaBox;
        private readonly IMongoCollection<UserCollection> _userCollection;
        private readonly IUserAchievementRepository _userAchievementRepository;
        private readonly IMongoClient _mongoClient;

        public ExchangeRepository(MongoDbContext context, IMongoClient mongoClient, IUserAchievementRepository userAchievementRepository) : base(context.GetCollection<ExchangeInfo>("ExchangeInfo"))
        {
            _exchangeInfo = context.GetCollection<ExchangeInfo>("Exchangeinfo");
            _exchangeProduct = context.GetCollection<ExchangeProduct>("ExchangeProduct");
            _exchangeSession = context.GetCollection<ExchangeSession>("ExchangeSession");
            _sellProduct = context.GetCollection<SellProduct>("SellProduct");
            _userProduct = context.GetCollection<UserProduct>("User_Product");
            _product = context.GetCollection<Product>("Product");
            _productInMangaBox = context.GetCollection<ProductInMangaBox>("ProductInMangaBox");
            _mangaBox = context.GetCollection<MangaBox>("MangaBox");
            _userCollection = context.GetCollection<UserCollection>("UserCollection");
            _mongoClient = mongoClient;
            _userAchievementRepository = userAchievementRepository;
        }

        public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string userId)
        {
            var sellproducts = await _sellProduct.Find(x => x.SellerId.Equals(userId)).ToListAsync();
            var sellIds = sellproducts.Select(x => x.Id).Distinct().ToList();
            var sellps = sellproducts.Select(x => x.ProductId).Distinct().ToList();

            var images = await _product.Find(p =>
                sellps.Contains(p.Id)).ToListAsync(); //Imagete

            var infos = await _exchangeInfo.Find(p =>
                sellIds.Contains(p.ItemReciveId) &&
                p.Status == (int)ExchangeStatus.Pending || p.Status == (int)ExchangeStatus.Finish 
                ).ToListAsync();

            if (!infos.Any()) return [];

            await RejectIfExpiredAsync(infos);
            var sessionIds = infos.Select(x => x.ItemGiveId).Distinct().ToList();
            var exchangeSessions = await _exchangeSession.Find(x => sessionIds.Contains(x.Id)).ToListAsync();
            var eproducts = await _exchangeProduct.Find(p => sessionIds.Contains(p.ExchangeId)).ToListAsync();
            var eproductIds = eproducts.Select(x => x.ProductExchangeId).Distinct().ToList();
            var uproducts = await _userProduct.Find(p => eproductIds.Contains(p.Id)).ToListAsync();
            var uproductIds = uproducts.Select(x => x.ProductId).Distinct().ToList();
            var products = await _product.Find(p => uproductIds.Contains(p.Id)).ToListAsync();

            return infos.Select(info =>
            {
                var productList = eproducts
                .Where(p => p.ExchangeId == info.ItemGiveId)
                .Select(p =>
                {
                    var up = uproducts.FirstOrDefault(x => x.Id == p.ProductExchangeId);
                    var productId = up?.ProductId;
                    var product = products.FirstOrDefault(pr => pr.Id == productId);
                    return new ExchangeProductDetailDto
                    {
                        ProductExchangeId = product?.Id ?? "unknown",
                        QuantityProductExchange = p.QuantityProductExchange,
                        Status = p.Status,
                        Image = product?.UrlImage
                    };
                })
                .ToList();

                var itemReciveProductId = sellproducts.FirstOrDefault(sp => sp.Id == info.ItemReciveId)?.ProductId;
                var imageUrl = images.FirstOrDefault(p => p.Id == itemReciveProductId)?.UrlImage;
                var exchangeSession = exchangeSessions.FirstOrDefault(x => x.Id.Equals(info.ItemGiveId));
                var isFeedback = exchangeSession?.FeedbackId != null && exchangeSession.FeedbackId.Any();


                return new ExchangeGetAllWithProductDto
                {
                    Id = info.Id,
                    BuyerId = info.BuyerId,
                    ItemReciveId = info.ItemReciveId,
                    IamgeItemRecive = imageUrl,
                    ItemGiveId = info.ItemGiveId,
                    Status = info.Status,
                    Datetime = info.Datetime,
                    IsFeedback = isFeedback,
                    Products = productList
                };
            }).ToList();
        }
        public async Task<ExchangeInfo> GetExchangeInfoById(string id)
        {
            var exchangeInfo = await _exchangeInfo.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            return exchangeInfo;
        }
        public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsOfBuyerAsync(string userId)
        {
            var infos = await _exchangeInfo.Find(x => x.BuyerId == userId).ToListAsync();
            if (!infos.Any()) return [];
            await RejectIfExpiredAsync(infos);

            var itemReciveIds = infos.Select(x => x.ItemReciveId).Distinct().ToList();
            var sellProducts = await _sellProduct.Find(p => itemReciveIds.Contains(p.Id)).ToListAsync();
            var reciveProductIds = sellProducts.Select(x => x.ProductId).Distinct().ToList();
            var reciveProducts = await _product.Find(p => reciveProductIds.Contains(p.Id)).ToListAsync();


            var sessionIds = infos.Select(x => x.ItemGiveId).Distinct().ToList();
            var exchangeSessions = await _exchangeSession.Find(x => sessionIds.Contains(x.Id)).ToListAsync();
            var eproducts = await _exchangeProduct.Find(p => sessionIds.Contains(p.ExchangeId)).ToListAsync();
            var eproductIds = eproducts.Select(x => x.ProductExchangeId).Distinct().ToList();

            var uproducts = await _userProduct.Find(p => eproductIds.Contains(p.Id)).ToListAsync();
            var uproductIds = uproducts.Select(x => x.ProductId).Distinct().ToList();

            var products = await _product.Find(p => uproductIds.Contains(p.Id)).ToListAsync();

            return infos.Select(info =>
            {
                var productList = eproducts
                .Where(p => p.ExchangeId == info.ItemGiveId)
                .Select(p =>
                {
                    var up = uproducts.FirstOrDefault(x => x.Id == p.ProductExchangeId);
                    var productId = up?.ProductId;
                    var product = products.FirstOrDefault(pr => pr.Id == productId);
                    return new ExchangeProductDetailDto
                    {
                        ProductExchangeId = up.Id,
                        QuantityProductExchange = p.QuantityProductExchange,
                        Status = p.Status,
                        Image = product?.UrlImage
                    };
                })
                .ToList();
                var sellProduct = sellProducts.FirstOrDefault(sp => sp.Id == info.ItemReciveId);
                var reciveProduct = reciveProducts.FirstOrDefault(p => p.Id == sellProduct?.ProductId);
                var exchangeSession = exchangeSessions.FirstOrDefault(x => x.Id.Equals(info.ItemGiveId));
                var isFeedback = exchangeSession?.FeedbackId != null && exchangeSession.FeedbackId.Any();
                return new ExchangeGetAllWithProductDto
                {
                    Id = info.Id,
                    BuyerId = info.BuyerId,
                    ItemReciveId = info.ItemReciveId,
                    IamgeItemRecive = reciveProduct?.UrlImage,
                    ItemGiveId = info.ItemGiveId,
                    Status = info.Status,
                    Datetime = info.Datetime,
                    IsFeedback = isFeedback,
                    Products = productList
                };
            }).ToList();
        }

        public async Task<ExchangeInfo> CreateExchangeAsync(ExchangeInfo info, List<ExchangeProduct> products, ExchangeSession session)
        {
            session.Id = ObjectId.GenerateNewId().ToString();
            info.ItemGiveId = session.Id;
            await _exchangeSession.InsertOneAsync(session);

            await _exchangeInfo.InsertOneAsync(info);


            foreach (var p in products)
                p.ExchangeId = session.Id;
            await _exchangeProduct.InsertManyAsync(products);


            return info;
        }

        public async Task<bool> AcceptExchangeAsync(string exchangeId, string currentUserId)
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    var info = await _exchangeInfo.Find(session, x => x.Id.Equals(exchangeId)).FirstOrDefaultAsync();
                    if (info == null) throw new Exception("Exchange request not found.");

                    if (info.Status != (int)ExchangeStatus.Pending)
                    {
                        throw new Exception("This exchange request cannot be accepted because it is not in a pending state.");
                    }

                    var sessionInfo = await _exchangeSession.Find(session, x => x.Id == info.ItemGiveId).FirstOrDefaultAsync();
                    if (sessionInfo == null) throw new Exception("Exchange session data not found.");

                    var sell = await _sellProduct.Find(session, x => x.Id.Equals(info.ItemReciveId)).FirstOrDefaultAsync();
                    if (sell == null) throw new Exception("The product to be received no longer exists.");

                    if (sell.SellerId != currentUserId)
                    {
                        throw new Exception("You are not authorized to accept this exchange request.");
                    }

                    var sellProductFilter = Builders<SellProduct>.Filter.And(
                        Builders<SellProduct>.Filter.Eq(x => x.Id, info.ItemReciveId),
                        Builders<SellProduct>.Filter.Gte(x => x.Quantity, 1)
                    );
                    var sellProductUpdate = Builders<SellProduct>.Update.Inc(x => x.Quantity, -1);
                    var updateSellResult = await _sellProduct.UpdateOneAsync(session, sellProductFilter, sellProductUpdate);

                    if (updateSellResult.ModifiedCount == 0)
                    {
                        throw new Exception("Unable to process the exchange. The requested product may be out of stock or has been exchanged by another user.");
                    }

                    var exchangeProducts = await _exchangeProduct.Find(session, x => x.ExchangeId == sessionInfo.Id).ToListAsync();
                    foreach (var ep in exchangeProducts)
                    {
                        var userProductFilter = Builders<UserProduct>.Filter.And(
                            Builders<UserProduct>.Filter.Eq(x => x.CollectorId, info.BuyerId),
                            Builders<UserProduct>.Filter.Eq(x => x.Id, ep.ProductExchangeId),
                            Builders<UserProduct>.Filter.Gte(x => x.Quantity, ep.QuantityProductExchange)
                        );
                        var userProductUpdate = Builders<UserProduct>.Update.Inc(x => x.Quantity, -ep.QuantityProductExchange);
                        var updateUserProductResult = await _userProduct.UpdateOneAsync(session, userProductFilter, userProductUpdate);

                        if (updateUserProductResult.ModifiedCount == 0)
                        {
                            throw new Exception($"Not enough quantity for the product being offered: {ep.ProductExchangeId}");
                        }
                    }

                    await UpdateUserProduct(session, info.BuyerId, sell.ProductId, 1);
                    foreach (var ep in exchangeProducts)
                    {
                        var userProductToGive = await _userProduct.Find(session, x => x.Id == ep.ProductExchangeId).FirstOrDefaultAsync();
                        if (userProductToGive == null)
                        {
                            throw new Exception($"Could not find the user product with ID {ep.ProductExchangeId} that was offered.");
                        }

                        await UpdateUserProduct(session, sell.SellerId, userProductToGive.ProductId, ep.QuantityProductExchange);
                    }

                    await _exchangeSession.UpdateOneAsync(session, x => x.Id == sessionInfo.Id, Builders<ExchangeSession>.Update.Set(x => x.Status, 1));
                    await _exchangeInfo.UpdateOneAsync(session, x => x.Id == exchangeId, Builders<ExchangeInfo>.Update
                        .Set(x => x.Status, (int)ExchangeStatus.Finish)
                        .Set(x => x.Datetime, DateTime.UtcNow)
                    );

                    await session.CommitTransactionAsync();
                    await _userAchievementRepository.CheckAchievement(info.BuyerId);
                    await _userAchievementRepository.CheckAchievement(sell.SellerId);
                    return true;
                }
                catch (Exception ex)
                {
                    await session.AbortTransactionAsync();
                    throw new Exception($"Exchange failed and all changes were rolled back. Reason: {ex.Message}");
                }
            }
        }

        private async Task UpdateUserProduct(IClientSessionHandle session, string userId, string productId, int quantity)
        {
            var productInBox = await _productInMangaBox.Find(session, p => p.ProductId.Equals(productId)).FirstOrDefaultAsync();
            if (productInBox == null) throw new Exception($"ProductInMangaBox not found for ProductId: {productId}");

            var mangaBox = await _mangaBox.Find(session, m => m.Id.Equals(productInBox.MangaBoxId)).FirstOrDefaultAsync();
            if (mangaBox == null) throw new Exception($"MangaBox not found for ID: {productInBox.MangaBoxId}");

            var collectionId = mangaBox.CollectionTopicId;

            var userCollection = await _userCollection
                .Find(session, uc => uc.UserId.Equals(userId) && uc.CollectionId.Equals(collectionId))
                .FirstOrDefaultAsync();

            if (userCollection == null)
            {
                userCollection = new UserCollection
                {
                    CollectionId = collectionId,
                    UserId = userId
                };
                await _userCollection.InsertOneAsync(session, userCollection);
            }

            var filter = Builders<UserProduct>.Filter.Where(x => x.CollectorId.Equals(userId) && x.ProductId.Equals(productId));
            var existing = await _userProduct.Find(session, filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                await _userProduct.UpdateOneAsync(
                    session,
                    filter,
                    Builders<UserProduct>.Update.Inc(x => x.Quantity, quantity)
                );
            }
            else
            {
                var newUserProduct = new UserProduct
                {
                    CollectorId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    CollectionId = userCollection.Id,
                    CollectedAt = DateTime.UtcNow
                };
                await _userProduct.InsertOneAsync(session, newUserProduct);
            }
        }

        public async Task<bool> CancelExchangeAsync(string exchangeId, string userId)
        {
            var info = await _exchangeInfo.Find(x => x.Id == exchangeId).FirstOrDefaultAsync();
            if (info == null || info.BuyerId != userId) throw new Exception("You do not have the right to cancel");

            if (info.Status != (int)ExchangeStatus.Pending) throw new Exception("Cancel failed");


            await _exchangeInfo.UpdateOneAsync(
                x => x.Id == exchangeId,
                Builders<ExchangeInfo>.Update.Set(x => x.Status, (int)ExchangeStatus.Cancel)
            );


            await _exchangeSession.UpdateOneAsync(
                x => x.Id == info.ItemGiveId,
                Builders<ExchangeSession>.Update.Set(x => x.Status, 0)
            );

            return true;
        }

        private async Task RejectIfExpiredAsync(List<ExchangeInfo> infos)
        {
            // Lọc các exchange hết hạn và vẫn đang Pending
            var expiredInfos = infos
                .Where(x => x.Datetime.AddDays(7) < DateTime.UtcNow && x.Status == (int)ExchangeStatus.Pending)
                .ToList();

            if (!expiredInfos.Any()) return;

            var expiredIds = expiredInfos.Select(x => x.Id).ToList();
            var expiredSessionIds = expiredInfos.Select(x => x.ItemGiveId).ToList();

            // ✅ Update ExchangeInfo -> Reject
            var exchangeFilter = Builders<ExchangeInfo>.Filter.In(x => x.Id, expiredIds);
            var exchangeUpdate = Builders<ExchangeInfo>.Update.Set(x => x.Status, (int)ExchangeStatus.Reject);
            await _exchangeInfo.UpdateManyAsync(exchangeFilter, exchangeUpdate);

            // ✅ Update ExchangeSession -> Status = 0 (mở lại session)
            var sessionFilter = Builders<ExchangeSession>.Filter.In(x => x.Id, expiredSessionIds);
            var sessionUpdate = Builders<ExchangeSession>.Update.Set(x => x.Status, 0);
            await _exchangeSession.UpdateManyAsync(sessionFilter, sessionUpdate);

            // ✅ Cập nhật lại trạng thái trong `infos` truyền vào (nếu cần xử lý tiếp)
            foreach (var info in infos)
            {
                if (expiredIds.Contains(info.Id))
                {
                    info.Status = (int)ExchangeStatus.Reject;
                }
            }
        }



        public async Task<bool> RejectExchangeAsync(string exchangeId, string userId)
        {
            var info = await _exchangeInfo.Find(x => x.Id == exchangeId).FirstOrDefaultAsync();
            if (info == null) return false;


            var sell = await _sellProduct.Find(x => x.Id.Equals(info.ItemReciveId)).FirstOrDefaultAsync();
            if (sell == null || sell.SellerId != userId) throw new Exception("You do not have the right to reject");

            if (info.Status != (int)ExchangeStatus.Pending) throw new Exception("Reject failed");


            await _exchangeInfo.UpdateOneAsync(
                x => x.Id == exchangeId,
                Builders<ExchangeInfo>.Update.Set(x => x.Status, (int)ExchangeStatus.Reject)
            );

            await _exchangeSession.UpdateOneAsync(
                x => x.Id == info.ItemGiveId,
                Builders<ExchangeSession>.Update.Set(x => x.Status, 0)
            );

            return true;
        }

    }
}
