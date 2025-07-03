using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class ProductOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SellId { get; set; }
        public string BuyerId { get; set; }
        public string ProductId { get; set; }
        public int Amount { get; set; }
    }

}
