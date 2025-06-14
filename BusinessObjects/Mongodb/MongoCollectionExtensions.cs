using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Mongodb
{
    public static class MongoCollectionExtensions
    {
        public static IMongoCollection<BsonDocument> WithBson<T>(this IMongoCollection<T> collection)
        {
            return collection.Database.GetCollection<BsonDocument>(collection.CollectionNamespace.CollectionName);
        }
    }
}
