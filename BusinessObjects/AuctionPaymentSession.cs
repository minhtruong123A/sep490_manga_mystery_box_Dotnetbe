using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class AuctionPaymentSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AuctionResultId { get; set; }
        public string WalletId { get; set; }
        public string UserId { get; set; }
        public int Amount { get; set; }
        public bool IsWithdraw { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
