using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Pipelines
{
    public static class SellProductPipelineBuilder
    {
        public static IAggregateFluent<BsonDocument> BuildBasePipeline(IAggregateFluent<BsonDocument> pipeline)
        {
            //Cau truc cua builder pipeline no nhu la viec m co cau to chuc chung,
            //dao duong tao thanh he thong giao thong vay, m dao 1 lan thi m chay chung do,
            // dao duoc 12 bang thi chi can repository no vong vo 12 bang thi dung base duoc het
            return pipeline
                .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                    new BsonDocument("ProductObjectId", new BsonDocument("$toObjectId", "$ProductId"))))
                .Lookup("Product", "ProductObjectId", "_id", "Product")
                .Unwind("Product")
                .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                    new BsonDocument("ProductStringId", new BsonDocument("$toString", "$Product._id"))))
                .Lookup("User_Product", "ProductStringId", "ProductId", "UserProduct")
                .Unwind("UserProduct", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
                .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                    new BsonDocument("CollectionObjectId", new BsonDocument("$toObjectId", "$UserProduct.CollectionId"))))
                .Lookup("Collection", "CollectionObjectId", "_id", "Collection")
                .Unwind("Collection", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
                .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                    new BsonDocument("SellerObjectId", new BsonDocument("$toObjectId", "$SellerId"))))
                .Lookup("User", "SellerObjectId", "_id", "User")
                .Unwind("User");
        }

        public static IAggregateFluent<BsonDocument> BuildProductOnSalePipeline(IAggregateFluent<BsonDocument> pipeline)
        {
            return BuildBasePipeline(pipeline.Match(x => x["IsSell"] == true))
                .Project(new BsonDocument
                {
                { "Id", new BsonDocument("$toString", "$_id") },
                { "Name", "$Product.Name" },
                { "Price", "$Price" },
                { "Username", "$User.username" },
                { "UrlImage", "$Product.UrlImage" },
                { "Topic", "$Collection.Topic" }
                });
        }

        public static IAggregateFluent<BsonDocument> BuildProductDetailPipeline(IAggregateFluent<BsonDocument> pipeline, ObjectId id)
        {
            return BuildBasePipeline(pipeline.Match(x => x["_id"] == id && x["IsSell"] == true))
                .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                    new BsonDocument("RarityObjectId", new BsonDocument("$toObjectId", "$Product.RarityId"))))
                .Lookup("Rarity", "RarityObjectId", "_id", "Rarity")
                .Unwind("Rarity", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
                .Project(new BsonDocument
                {
                { "Id", new BsonDocument("$toString", "$_id") },
                { "Name", "$Product.Name" },
                { "Price", "$Price" },
                { "Username", "$User.username" },
                { "UrlImage", "$Product.UrlImage" },
                { "Topic", "$Collection.Topic" },
                { "RateName", "$Rarity.Name" },
                { "Description", "$Description" }
                });
        }
    }
}
