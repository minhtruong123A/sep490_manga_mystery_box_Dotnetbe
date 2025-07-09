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
        [BsonElement("exchange_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ExchangeId { get; set; }
        public string ProductExchangeId { get; set; }
        public int QuantityProductExchange { get; set; }
        public int Status { get; set; } // Optional
    }

}
