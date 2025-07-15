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

        public ExchangeRepository(MongoDbContext context) : base(context.GetCollection<ExchangeInfo>("ExchangeInfo"))
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
        }

        public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string userId)
        {
            var sellproducts = await _sellProduct.Find(x => x.SellerId.Equals(userId)).ToListAsync();
            var sellIds = sellproducts.Select(x => x.Id).Distinct().ToList();

            var infos = await _exchangeInfo.Find(p =>
                sellIds.Contains(p.ItemReciveId) &&
                p.Status == (int)ExchangeStatus.Pending
                ).ToListAsync();

            if (!infos.Any()) return [];
            
            await RejectIfExpiredAsync(infos);
            var sessionIds = infos.Select(x => x.ItemGiveId).Distinct().ToList();
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

                return new ExchangeGetAllWithProductDto
                {
                    Id = info.Id,
                    BuyerId = info.BuyerId,
                    ItemReciveId = info.ItemReciveId,
                    ItemGiveId = info.ItemGiveId,
                    Status = info.Status,
                    Datetime = info.Datetime,
                    Products = productList
                };
            }).ToList();
        }

        public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsOfBuyerAsync(string userId)
        {
            var infos = await _exchangeInfo.Find(x => x.BuyerId == userId).ToListAsync();
            if (!infos.Any()) return [];
            await RejectIfExpiredAsync(infos);

            var sessionIds = infos.Select(x => x.ItemGiveId).Distinct().ToList();
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

                return new ExchangeGetAllWithProductDto
                {
                    Id = info.Id,
                    BuyerId = info.BuyerId,
                    ItemReciveId = info.ItemReciveId,
                    ItemGiveId = info.ItemGiveId,
                    Status = info.Status,
                    Datetime = info.Datetime,
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

        public async Task<bool> AcceptExchangeAsync(string exchangeId)
        {
            try
            {
                var info = await _exchangeInfo.Find(x => x.Id.Equals(exchangeId)).FirstOrDefaultAsync();

                if (info == null) throw new Exception("ExchangeInfo not found"); 


                var i = info.ItemGiveId;

                var updateInfoResult = await _exchangeInfo.UpdateOneAsync(
                    x => x.Id == exchangeId,
                    Builders<ExchangeInfo>.Update
                        .Set(x => x.Status, (int)ExchangeStatus.Finish)
                        .Set(x => x.Datetime, DateTime.UtcNow)
                );
                if (updateInfoResult.ModifiedCount == 0) throw new Exception("Failed to update ExchangeInfo");

                var session = await _exchangeSession.Find(x => x.Id == i).FirstOrDefaultAsync();
                if (session == null) throw new Exception("ExchangeSession not found");

                var updateSessionResult = await _exchangeSession.UpdateOneAsync(
                    x => x.Id == session.Id,
                    Builders<ExchangeSession>.Update.Set(x => x.Status, 1)
                );
                if (updateSessionResult.ModifiedCount == 0) throw new Exception("Failed to update ExchangeSession");


                var sell = await _sellProduct.Find(x => x.Id.Equals(info.ItemReciveId)).FirstOrDefaultAsync();
                if(sell == null) throw new Exception("SellProduct not found");
                if(sell.Quantity == 1)
                {
                    var updateSell = await _sellProduct.UpdateOneAsync(
                    x => x.Id == sell.Id,
                    Builders<SellProduct>.Update.Set(x => x.IsSell, false)
                                                .Set(x => x.Quantity, 0 )
                    );
                    if (updateSell.ModifiedCount == 0) throw new Exception("Failed to update SellProduct");
                }
                else if(sell.Quantity > 1)
                {
                    var updateSell = await _sellProduct.UpdateOneAsync(
                    x => x.ProductId == sell.ProductId,
                    Builders<SellProduct>.Update.Inc(x => x.Quantity, -1)
                    );
                    if (updateSell.ModifiedCount == 0) throw new Exception("Failed to update SellProduct");
                }
                else
                {
                    throw new Exception("The product is sold out.");
                }

                var receiverId = sell.SellerId;
                var receiverProductId = sell.ProductId;

                var exchangeProducts = await _exchangeProduct.Find(x => x.ExchangeId == session.Id).ToListAsync();
                foreach (var ep in exchangeProducts)
                {
                   var filter = Builders<UserProduct>.Filter.Where(x =>
                        x.CollectorId == info.BuyerId && x.Id == ep.ProductExchangeId);
                   var existing = await _userProduct.Find(filter).FirstOrDefaultAsync();

                    if (existing == null)
                        throw new Exception($"UserProduct not found for productId: {ep.ProductExchangeId}");

                    if (existing.Quantity < ep.QuantityProductExchange)
                        throw new Exception($"Not enough quantity for productId: {ep.ProductExchangeId}");

                    var update = await _userProduct.UpdateOneAsync(
                        filter,
                        Builders<UserProduct>.Update.Inc(x => x.Quantity, -ep.QuantityProductExchange)
                    );
                    if (update.ModifiedCount == 0)
                        throw new Exception($"Failed to update quantity for productId: {ep.ProductExchangeId}");
                }

                await UpdateUserProduct(info.BuyerId, receiverProductId, 1);
                foreach (var ep in exchangeProducts)
                {
                    var pro = await _userProduct.Find(x => x.Id.Equals(ep.ProductExchangeId)).FirstOrDefaultAsync();
                    await UpdateUserProduct(receiverId, pro.ProductId, ep.QuantityProductExchange);
                }

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }


        private async Task UpdateUserProduct(string userId, string productId, int quantity)
        {
            
            var productInBox = await _productInMangaBox.Find(p => p.ProductId.Equals(productId)).FirstOrDefaultAsync();
            if (productInBox == null) throw new Exception("Not found productInBox");

            
            var mangaBox = await _mangaBox.Find(m => m.Id.Equals(productInBox.MangaBoxId)).FirstOrDefaultAsync();
            if (mangaBox == null) throw new Exception("Not found mangaBox");

            
            var collectionId = mangaBox.CollectionTopicId;

            
            var userCollection = await _userCollection
                .Find(uc => uc.UserId.Equals(userId) && uc.CollectionId.Equals(collectionId))
                .FirstOrDefaultAsync();

            if (userCollection == null)
            {
                userCollection = new UserCollection
                {
                    CollectionId = collectionId,
                    UserId = userId
                };
                await _userCollection.InsertOneAsync(userCollection);
            }

            userCollection = await _userCollection
                .Find(uc => uc.UserId.Equals(userId) && uc.CollectionId.Equals(collectionId))
                .FirstOrDefaultAsync();

            // Bước 5: Tiếp tục thêm hoặc cập nhật UserProduct
            var filter = Builders<UserProduct>.Filter.Where(x => x.CollectorId.Equals(userId) && x.ProductId.Equals(productId));
            var existing = await _userProduct.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                await _userProduct.UpdateOneAsync(
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
                await _userProduct.InsertOneAsync(newUserProduct);
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
            if (sell == null || sell.SellerId != userId)  throw new Exception("You do not have the right to reject");

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
