using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Mongodb
{
    public static class MongoBsonDocumentExtensions
    {
        public static string? TryGetString(this BsonDocument doc, string key)
        {
            return doc.Contains(key) && !doc[key].IsBsonNull ? doc[key].AsString : null;
        }
    }
}
