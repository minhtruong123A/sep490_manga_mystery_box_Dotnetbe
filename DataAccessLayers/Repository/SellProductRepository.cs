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
        private readonly MongoDbContext _mongoDbContext;
        public SellProductRepository(MongoDbContext context) : base(context.GetCollection<SellProduct>("SellProduct"))
        {
            _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _productCollection = context.GetCollection<Product>("Product");
            _mongoDbContext = context;

        }

        public async Task<bool> CreateSellProductAsync(SellProductCreateDto dto, string userId)
        {
            var userProduct = await _userProductCollection
                .Find(x => x.CollectorId == userId.Trim() && x.ProductId == dto.ProductId.Trim())
                .FirstOrDefaultAsync();

            if (userProduct == null || userProduct.Quantity <= 0)
                throw new Exception("User does not own this product or quantity is zero.");

            if (userProduct.Quantity == 1)
                await _userProductCollection.DeleteOneAsync(x => x.Id == userProduct.Id);
            else
                await _userProductCollection.UpdateOneAsync(
                    x => x.Id == userProduct.Id,
                    Builders<UserProduct>.Update.Inc(up => up.Quantity, -1));

            var sellProduct = new SellProduct
            {
                ProductId = dto.ProductId,
                SellerId = userId,
                Quantity = dto.Quantity,
                Description = dto.Description,
                Price = dto.Price,
                ExchangeCode = dto.ExchangeCode,
                IsSell = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _sellProductCollection.InsertOneAsync(sellProduct);
            return true;
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
            var sellProductList = await _mongoDbContext.SellProducts.AsQueryable().Where(c => c.IsSell).ToListAsync();
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
            var sellProduct = await _mongoDbContext.SellProducts.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == id && c.IsSell);
            if (sellProduct is null) return null;
            var productTask = _mongoDbContext.Products.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == sellProduct.ProductId);
            var userTask = _mongoDbContext.Users.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == sellProduct.SellerId);
            var userProductTask = _mongoDbContext.UserProducts.AsQueryable().FirstOrDefaultAsync(c => c.ProductId == sellProduct.ProductId);
            await Task.WhenAll(productTask, userTask, userProductTask);

            var productResult = productTask.Result;
            var userResult = userTask.Result;
            var userProductResult = userProductTask.Result;
            var collectionTask = userProductResult?.CollectionId is string colId
                ? _mongoDbContext.Collections.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == colId) : Task.FromResult<Collection?>(null);
            var rarityTask = productResult?.RarityId is string rId
                ? _mongoDbContext.Rarities.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == rId) : Task.FromResult<Rarity?>(null);
            await Task.WhenAll(collectionTask, rarityTask);

            var collectionResult = collectionTask.Result;
            var rarityResult = rarityTask.Result;

            return new SellProductDetailDto
            {
                Id = sellProduct.Id.ToString(),
                Name = productResult?.Name,
                Price = sellProduct.Price,
                UrlImage = productResult?.UrlImage,
                Description = sellProduct.Description ?? "",
                Username = userResult?.Username ?? "Unknown",
                Topic = collectionResult?.Topic ?? "Unknown",
                RateName = rarityResult?.Name ?? "Unknown"
            };
        }
    }
}