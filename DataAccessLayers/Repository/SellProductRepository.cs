using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using DataAccessLayers.Pipelines;
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
    public class SellProductRepository : GenericRepository<SellProduct>, ISellProductRepository
    {
        private readonly IMongoCollection<SellProduct> _sellProductCollection;
        private readonly IMongoCollection<UserProduct> _userProductCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Collection> _collections;
        private readonly IMongoCollection<Rarity> _rarityCollection;
        public SellProductRepository(MongoDbContext context) : base(context.GetCollection<SellProduct>("SellProduct"))
        {
            _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _productCollection = context.GetCollection<Product>("Product");
            _userCollection = context.GetCollection<User>("User");
            _collections = context.GetCollection<Collection>("Collection");
            _rarityCollection = context.GetCollection<Rarity>("Rarity");
        }

        public async Task<int> CreateSellProductAsync(SellProductCreateDto dto, string userId)
        {
            var userProduct = await _userProductCollection.Find(x => x.Id == dto.UserProductId).FirstOrDefaultAsync();
            if (userProduct == null || userProduct.CollectorId != userId) throw new Exception("UserProduct not found or not owned by this user.");
            if (userProduct.Quantity == 0) throw new Exception("Cannot sell. This product is out of stock.");
            if (dto.Quantity <= 0) throw new Exception("Quantity must be greater than 0.");
            if (dto.Quantity > userProduct.Quantity) throw new Exception("Not enough quantity in inventory.");
            if (dto.Quantity == userProduct.Quantity)
            {
                var update = Builders<UserProduct>.Update.Set(x => x.Quantity, 0);
                await _userProductCollection.UpdateOneAsync(x => x.Id == userProduct.Id, update);
            }
            else
            {
                var update = Builders<UserProduct>.Update.Inc(x => x.Quantity, -dto.Quantity);
                await _userProductCollection.UpdateOneAsync(x => x.Id == userProduct.Id, update);
            }

            var ExchangeCode = await GenerateUniqueExchangeCodeAsync();
            var newSellProduct = new SellProduct
            {
                ProductId = userProduct.ProductId,
                SellerId = userId,
                Quantity = dto.Quantity,
                Description = dto.Description,
                Price = dto.Price,
                ExchangeCode = ExchangeCode,
                IsSell = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _sellProductCollection.InsertOneAsync(newSellProduct);

            return ExchangeCode;
        }

        private async Task<int> GenerateUniqueExchangeCodeAsync()
        {
            var rng = new Random();
            int code;
            bool exists;

            do
            {
                code = rng.Next(100000, 1000000);
                exists = await _sellProductCollection.Find(sp => sp.ExchangeCode == code).AnyAsync();
            } while (exists);

            return code;
        }

        //getallproductonsale
        //public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync()
        //{
        //    return await _sellProductCollection
        //        .WithBson()
        //        .RunAggregateWithLookups(
        //            buildPipeline: SellProductPipelineBuilder.BuildProductOnSalePipeline,
        //            selector: x => new SellProductGetAllDto
        //            {
        //                Id = x.GetValue("Id", "").AsString,
        //                Name = x.GetValue("Name", "").AsString,
        //                Price = x.GetValue("Price", 0).ToInt32(),
        //                Username = x.GetValue("Username", "").AsString,
        //                Topic = x.TryGetString("Topic") ?? "Unknown",
        //                UrlImage = x.TryGetString("UrlImage")
        //            });
        //}
        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync()
        {
            var sellProductList = await _sellProductCollection.AsQueryable().Where(c => c.IsSell).ToListAsync();
            var productIds = sellProductList.Select(c => c.ProductId).ToHashSet();
            var sellerIds = sellProductList.Select(c => c.SellerId).ToHashSet();
            var productTask = _productCollection.AsQueryable().Where(c => productIds.Contains(c.Id.ToString())).ToListAsync();
            var userTask = _userCollection.AsQueryable().Where(c => sellerIds.Contains(c.Id.ToString())).ToListAsync();
            var userProductTask = _userProductCollection.AsQueryable().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            await Task.WhenAll(productTask, userTask, userProductTask);

            var productList = productTask.Result;
            var userList = userTask.Result;
            var userProductList = userProductTask.Result;

            var collectionIds = userProductList.Select(up => up.CollectionId).ToHashSet();
            var rarityIds = productList.Select(up => up.RarityId).ToHashSet();
            var collectionsTask = _collections.AsQueryable().Where(c => collectionIds.Contains(c.Id.ToString())) .ToListAsync();
            var raritiesTask = _rarityCollection.AsQueryable().Where(r => rarityIds.Contains(r.Id.ToString())).ToListAsync();
            await Task.WhenAll(collectionsTask, raritiesTask);

            var collections = collectionsTask.Result;
            var rarities = raritiesTask.Result;

            return sellProductList.Select(sellProduct =>
            {
                var product = productList.FirstOrDefault(c => c.Id.ToString() == sellProduct.ProductId);
                var user = userList.FirstOrDefault(c => c.Id.ToString() == sellProduct.SellerId);
                var userProduct = userProductList.FirstOrDefault(c => c.ProductId == sellProduct.ProductId);
                var collection = collections.FirstOrDefault(c => c.Id.ToString() == userProduct?.CollectionId);
                var rarity = rarities.FirstOrDefault(r => r.Id.ToString() == product?.RarityId);

                return new SellProductGetAllDto
                {
                    Id = sellProduct.Id.ToString(),
                    Name = product?.Name ?? "Unknown",
                    Price = sellProduct?.Price ?? null,
                    Username = user?.Username ?? "Unknown",
                    Topic = collection?.Topic ?? "Unknown",
                    UrlImage = product?.UrlImage ?? "Unknown",
                    RarityName = rarity?.Name ?? "Unknown",
                    CreatedAt = sellProduct?.CreatedAt ?? null,
                };
            }).ToList();
        }
        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleOfUserIdAsync(string id)
        {
            var sellProductList = await _mongoDbContext.SellProducts.AsQueryable().Where(c => c.IsSell && c.SellerId.Equals(id)).ToListAsync();
            var productIds = sellProductList.Select(c => c.ProductId).ToHashSet();
            var sellerIds = sellProductList.Select(c => c.SellerId).ToHashSet();
            var productTask = _mongoDbContext.Products.AsQueryable().Where(c => productIds.Contains(c.Id.ToString())).ToListAsync();
            var userTask = _mongoDbContext.Users.AsQueryable().Where(c => sellerIds.Contains(c.Id.ToString())).ToListAsync();
            var userProductTask = _mongoDbContext.UserProducts.AsQueryable().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            await Task.WhenAll(productTask, userTask, userProductTask);

            var productList = productTask.Result;
            var userList = userTask.Result;
            var userProductList = userProductTask.Result;
            var collections = await _mongoDbContext.Collections.AsQueryable().Where(c => userProductList.Any(up => up.CollectionId == c.Id.ToString())).ToListAsync();

            return sellProductList.Select(sellProduct =>
            {
                var product = productList.FirstOrDefault(c => c.Id.ToString() == sellProduct.ProductId);
                var user = userList.FirstOrDefault(c => c.Id.ToString() == sellProduct.SellerId);
                var userProduct = userProductList.FirstOrDefault(c => c.ProductId == sellProduct.ProductId);
                var collection = collections.FirstOrDefault(c => c.Id.ToString() == userProduct?.CollectionId);

                return new SellProductGetAllDto
                {
                    Id = sellProduct.Id.ToString(),
                    Name = product?.Name ?? "Unknown",
                    Price = sellProduct.Price,
                    Username = user?.Username ?? "Unknown",
                    Topic = collection?.Topic ?? "Unknown",
                    UrlImage = product?.UrlImage
                };
            }).ToList();
        }
        //getproductonsalebyid
        //public async Task<SellProductDetailDto> GetProductDetailByIdAsync(string id)
        //{
        //    var objectId = ObjectId.Parse(id);

        //    var result = await _sellProductCollection
        //        .WithBson()
        //        .RunAggregateWithLookups(
        //            buildPipeline: p => SellProductPipelineBuilder.BuildProductDetailPipeline(p, objectId),
        //            selector: x => new SellProductDetailDto
        //            {
        //                Id = x.GetValue("Id", "").AsString,
        //                Name = x.GetValue("Name", "").AsString,
        //                Price = x.GetValue("Price", 0).ToInt32(),
        //                Username = x.GetValue("Username", "").AsString,
        //                Topic = x.TryGetString("Topic") ?? "Unknown",
        //                UrlImage = x.TryGetString("UrlImage"),
        //                RateName = x.TryGetString("RateName") ?? "Unknown",
        //                Description = x.TryGetString("Description") ?? ""
        //            });

        //    return result.FirstOrDefault();
        public async Task<SellProductDetailDto?> GetProductDetailByIdAsync(string id)
        {
            var sellProduct = await _sellProductCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == id && c.IsSell);
            if (sellProduct is null) return null;
            var productTask = _productCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == sellProduct.ProductId);
            var userTask = _userCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == sellProduct.SellerId);
            var userProductTask = _userProductCollection.AsQueryable().FirstOrDefaultAsync(c => c.ProductId == sellProduct.ProductId);
            await Task.WhenAll(productTask, userTask, userProductTask);

            var productResult = productTask.Result;
            var userResult = userTask.Result;
            var userProductResult = userProductTask.Result;
            var collectionTask = userProductResult?.CollectionId is string colId
                ? _collections.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == colId) :  Task.FromResult<Collection?>(null);
            var rarityTask = productResult?.RarityId is string rId
                ? _rarityCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == rId) : Task.FromResult<Rarity?>(null);
            await Task.WhenAll(collectionTask, rarityTask);

            var collectionResult = collectionTask.Result;
            var rarityResult = rarityTask.Result;

            return new SellProductDetailDto
            {
                Id = sellProduct.Id.ToString(),
                Name = productResult?.Name ?? "Unknown",
                Price = sellProduct.Price,
                UrlImage = productResult?.UrlImage ?? "Unknown",
                Description = sellProduct.Description ?? "",
                UserId = userResult?.Id ?? "Unknown Id",
                Username = userResult?.Username ?? "Unknown",
                Topic = collectionResult?.Topic ?? "Unknown",
                RateName = rarityResult?.Name ?? "Unknown"
            };
        }
    }
}