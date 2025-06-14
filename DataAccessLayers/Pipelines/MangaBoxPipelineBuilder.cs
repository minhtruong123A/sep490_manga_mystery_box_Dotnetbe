using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Pipelines
{
    public static class MangaBoxPipelineBuilder
    {
        public static IAggregateFluent<BsonDocument> BuildBasePipeline(IAggregateFluent<BsonDocument> pipeline)
        {
            return pipeline
                .AppendStage<BsonDocument>(new BsonDocument("$addFields", new BsonDocument
                {
                { "MysteryBoxId", new BsonDocument("$toObjectId", new BsonDocument("$trim", new BsonDocument("input", "$MysteryBoxId"))) },
                { "CollectionTopicId", new BsonDocument("$toObjectId", new BsonDocument("$trim", new BsonDocument("input", "$CollectionTopicId"))) }
                }))
                .Lookup("MysteryBox", "MysteryBoxId", "_id", "MysteryBox")
                .Lookup("Collection", "CollectionTopicId", "_id", "Collection")
                .Unwind("MysteryBox")
                .Unwind("Collection");
        }

        public static IAggregateFluent<BsonDocument> BuildAllPipeline(IAggregateFluent<BsonDocument> pipeline)
        {
            return BuildBasePipeline(pipeline)
                .Project(new BsonDocument
                {
                { "MysteryBox.Name", 1 },
                { "MysteryBox.Price", 1 },
                { "MysteryBox.UrlImage", 1 },
                { "Collection.Topic", 1 },
                { "_id", 1 }
                });
        }

        public static IAggregateFluent<BsonDocument> BuildDetailPipeline(IAggregateFluent<BsonDocument> pipeline, ObjectId id)
        {
            return BuildBasePipeline(pipeline.Match(x => x["_id"] == id))
                .Project(new BsonDocument
                {
                { "Status", 1 },
                { "MysteryBox.Name", 1 },
                { "MysteryBox.Description", 1 },
                { "MysteryBox.Price", 1 },
                { "MysteryBox.UrlImage", 1 },
                { "Collection.Topic", 1 },
                { "_id", 1 }
                });
        }
    }
}
