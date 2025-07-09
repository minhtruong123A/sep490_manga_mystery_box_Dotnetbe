using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class ExchangeInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string BuyerId { get; set; }
        public string ItemReciveId { get; set; }
        public string ItemGiveId { get; set; }
        public DateTime Datetime { get; set; }
        public int Status { get; set; }
    }

}
