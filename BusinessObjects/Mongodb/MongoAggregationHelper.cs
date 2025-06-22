using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Mongodb
{
    public static class MongoAggregationHelper
    {
        public static async Task<List<TDto>> RunAggregateWithLookups<TDto>(
            this IMongoCollection<BsonDocument> collection,
            Func<IAggregateFluent<BsonDocument>, IAggregateFluent<BsonDocument>> buildPipeline,
            Func<BsonDocument, TDto> selector)
        {
            var pipeline = collection.Aggregate();
            pipeline = buildPipeline(pipeline);
            var documents = await pipeline.ToListAsync();
            return documents.Select(selector).ToList();
        }

        public static async Task<TDto?> RunAggregateWithLookupsSingle<TDto>(
            this IMongoCollection<BsonDocument> collection,
            Func<IAggregateFluent<BsonDocument>, IAggregateFluent<BsonDocument>> buildPipeline,
            Func<BsonDocument, TDto> selector)
        {
            var pipeline = collection.Aggregate();
            pipeline = buildPipeline(pipeline);
            var document = await pipeline.FirstOrDefaultAsync();
            return document == null ? default : selector(document);
        }

        public static string? TryGetString(this BsonDocument doc, string key)
        {
            return doc.Contains(key) && !doc[key].IsBsonNull ? doc[key].AsString : null;
        }

        public static IMongoCollection<BsonDocument> WithBson<T>(this IMongoCollection<T> collection)
        {
            return collection.Database.GetCollection<BsonDocument>(collection.CollectionNamespace.CollectionName);
        }
    }
}
