using BusinessObjects;
using BusinessObjects.Dtos.Exchange;
using BusinessObjects.Enum;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
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

        public ExchangeRepository(MongoDbContext context) : base(context.GetCollection<ExchangeInfo>("ExchangeInfo"))
        {
            _exchangeInfo = context.GetCollection<ExchangeInfo>("Exchangeinfo");
            _exchangeProduct = context.GetCollection<ExchangeProduct>("ExchangeProduct");
            _exchangeSession = context.GetCollection<ExchangeSession>("ExchangeSession");
            _sellProduct = context.GetCollection<SellProduct>("SellProduct");
            _userProduct = context.GetCollection<UserProduct>("User_Product");
        }
        public async Task<List<ExchangeGetAllWithProductDto>> GetExchangesWithProductsByItemReciveIdAsync(string sellProductId)
        {
            var infos = await _exchangeInfo.Find(x => x.ItemReciveId == sellProductId).ToListAsync();
            if (!infos.Any()) return [];

            // Lấy toàn bộ session theo ItemGiveId (chính là session id)
            var sessionIds = infos.Select(x => x.ItemGiveId).Distinct().ToList();
            var products = await _exchangeProduct.Find(p => sessionIds.Contains(p.ExchangeId)).ToListAsync();

            return infos.Select(info =>
            {
                var productList = products
                    .Where(p => p.ExchangeId == info.ItemGiveId)
                    .Select(p => new ExchangeProductDetailDto
                    {
                        ProductExchangeId = p.ProductExchangeId,
                        QuantityProductExchange = p.QuantityProductExchange,
                        Status = p.Status
                    }).ToList();

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

            // Lấy toàn bộ session theo ItemGiveId (chính là session id)
            var sessionIds = infos.Select(x => x.ItemGiveId).Distinct().ToList();
            var products = await _exchangeProduct.Find(p => sessionIds.Contains(p.ExchangeId)).ToListAsync();

            return infos.Select(info =>
            {
                var productList = products
                    .Where(p => p.ExchangeId == info.ItemGiveId)
                    .Select(p => new ExchangeProductDetailDto
                    {
                        ProductExchangeId = p.ProductExchangeId,
                        QuantityProductExchange = p.QuantityProductExchange,
                        Status = p.Status
                    }).ToList();

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
                var info = await _exchangeInfo.Find(x => x.Id == exchangeId).FirstOrDefaultAsync();

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


                var sell = await _sellProduct.Find(x => x.ProductId == info.ItemReciveId).FirstOrDefaultAsync();
                if (sell == null) throw new Exception("SellProduct not found");

                var updateSell = await _sellProduct.UpdateOneAsync(
                    x => x.ProductId == sell.ProductId,
                    Builders<SellProduct>.Update.Set(x => x.IsSell, true)
                );
                if (updateSell.ModifiedCount == 0) throw new Exception("Failed to update SellProduct");


                var filter = Builders<UserProduct>.Filter.Where(x => x.CollectorId == info.BuyerId && x.ProductId == sell.ProductId);
                var existing = await _userProduct.Find(filter).FirstOrDefaultAsync();
                if (existing.Quantity == 0) throw new Exception("This product is been selled or insufficient quantity");

                var quantity = existing.Quantity - sell.Quantity;
                if (existing != null)
                {
                    await _userProduct.UpdateOneAsync(filter,
                        Builders<UserProduct>.Update.Inc(x => x.Quantity, quantity));
                }
                var receiverId = sell.SellerId;

                var exchangeProducts = await _exchangeProduct.Find(x => x.ExchangeId == session.Id).ToListAsync();
                foreach (var ep in exchangeProducts)
                {
                    filter = Builders<UserProduct>.Filter.Where(x =>
                        x.CollectorId == info.BuyerId && x.ProductId == ep.ProductExchangeId);
                    existing = await _userProduct.Find(filter).FirstOrDefaultAsync();

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

                await UpdateUserProduct(info.BuyerId, info.ItemReciveId);
                await UpdateUserProduct(receiverId, info.ItemGiveId);

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[AcceptExchangeAsync] Error: {ex.Message}");
                return false;
            }
            
        }



        private async Task UpdateUserProduct(string userId, string productId)
        {
            var filter = Builders<UserProduct>.Filter.Where(x => x.CollectorId == userId && x.ProductId == productId);
            var existing = await _userProduct.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                await _userProduct.UpdateOneAsync(filter,
                    Builders<UserProduct>.Update.Inc(x => x.Quantity, 1));
            }
            else
            {
                var newUserProduct = new UserProduct
                {
                    CollectorId = userId,
                    ProductId = productId,
                    Quantity = 1,
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


        public async Task<bool> RejectExchangeAsync(string exchangeId, string userId)
        {
            var info = await _exchangeInfo.Find(x => x.Id == exchangeId).FirstOrDefaultAsync();
            if (info == null) return false;


            var sell = await _sellProduct.Find(x => x.ProductId == info.ItemReciveId).FirstOrDefaultAsync();
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
