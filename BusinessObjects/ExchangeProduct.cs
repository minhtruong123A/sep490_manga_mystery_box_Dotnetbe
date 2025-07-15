using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class ExchangeProduct
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // ⬅️ Đây là _id thực sự
        public string ExchangeId { get; set; } // ⬅️ Trường bạn đang truy vấn
        public string ProductExchangeId { get; set; }
        public int QuantityProductExchange { get; set; }
        public int Status { get; set; } // Optional
    }


}
