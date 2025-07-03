using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class OrderHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string BoxOrderId { get; set; }
        public string ProductOrderId { get; set; }
        public DateTime Datetime { get; set; }
        public int Status { get; set; }
    }

}
