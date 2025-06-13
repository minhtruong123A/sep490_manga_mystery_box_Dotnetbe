using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class UserProduct
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CollectionId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CollectedAt { get; set; }
    }
}
