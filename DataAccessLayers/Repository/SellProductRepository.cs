using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using DataAccessLayers.Pipelines;
using MongoDB.Bson;
using MongoDB.Driver;
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

        public SellProductRepository(MongoDbContext context) : base(context.GetCollection<SellProduct>("SellProduct"))
        {
            _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _productCollection = context.GetCollection<Product>("Product");
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
        //    var result = await _sellProductCollection.Aggregate()
        //        .Match(x => x.IsSell)
        //        .AppendStage<BsonDocument>(new BsonDocument("$addFields",
        //            new BsonDocument("ProductObjectId", new BsonDocument("$toObjectId", "$ProductId"))))
        //        .Lookup("Product", "ProductObjectId", "_id", "Product")
        //        .Unwind("Product")
        //        .AppendStage<BsonDocument>(new BsonDocument("$addFields",
        //            new BsonDocument("ProductStringId", new BsonDocument("$toString", "$Product._id"))))
        //        .Lookup("User_Product", "ProductStringId", "ProductId", "UserProduct")
        //        .Unwind("UserProduct", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
        //        .AppendStage<BsonDocument>(new BsonDocument("$addFields",
        //            new BsonDocument("CollectionObjectId", new BsonDocument("$toObjectId", "$UserProduct.CollectionId"))))
        //        .Lookup("Collection", "CollectionObjectId", "_id", "Collection")
        //        .Unwind("Collection", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
        //        .AppendStage<BsonDocument>(new BsonDocument("$addFields",
        //            new BsonDocument("SellerObjectId", new BsonDocument("$toObjectId", "$SellerId"))))
        //        .Lookup("User", "SellerObjectId", "_id", "User")
        //        .Unwind("User")
        //        .Project(new BsonDocument
        //        {
        //            { "Id", new BsonDocument("$toString", "$_id") },
        //            { "Name", "$Product.Name" },
        //            { "Price", "$Price" },
        //            { "Username", "$User.username" },
        //            { "UrlImage", "$Product.UrlImage" },
        //            { "Topic", "$Collection.Topic" }
        //        })
        //        .ToListAsync();

        //    return result.Select(x => new SellProductGetAllDto
        //    {
        //        Id = x.GetValue("Id", "").AsString,
        //        Name = x.GetValue("Name", "").AsString,
        //        Price = x.GetValue("Price", 0).ToInt32(),
        //        Username = x.GetValue("Username", "").AsString,
        //        Topic = x.Contains("Topic") && !x["Topic"].IsBsonNull ? x["Topic"].AsString : "Unknown",
        //        UrlImage = x.Contains("UrlImage") && !x["UrlImage"].IsBsonNull ? x["UrlImage"].AsString : null
        //    }).ToList();
        //}

        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync()
        {
            return await _sellProductCollection
                .WithBson()
                .RunAggregateWithLookups(
                    buildPipeline: SellProductPipelineBuilder.BuildProductOnSalePipeline,
                    selector: x => new SellProductGetAllDto
                    {
                        Id = x.GetValue("Id", "").AsString,
                        Name = x.GetValue("Name", "").AsString,
                        Price = x.GetValue("Price", 0).ToInt32(),
                        Username = x.GetValue("Username", "").AsString,
                        Topic = x.TryGetString("Topic") ?? "Unknown",
                        UrlImage = x.TryGetString("UrlImage")
                    });
        }

        //getproductonsalebyid
        public async Task<SellProductDetailDto> GetProductDetailByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);

            var result = await _sellProductCollection
                .WithBson()
                .RunAggregateWithLookups(
                    buildPipeline: p => SellProductPipelineBuilder.BuildProductDetailPipeline(p, objectId),
                    selector: x => new SellProductDetailDto
                    {
                        Id = x.GetValue("Id", "").AsString,
                        Name = x.GetValue("Name", "").AsString,
                        Price = x.GetValue("Price", 0).ToInt32(),
                        Username = x.GetValue("Username", "").AsString,
                        Topic = x.TryGetString("Topic") ?? "Unknown",
                        UrlImage = x.TryGetString("UrlImage"),
                        RateName = x.TryGetString("RateName") ?? "Unknown",
                        Description = x.TryGetString("Description") ?? ""
                    });

            return result.FirstOrDefault();
        }
    }
}