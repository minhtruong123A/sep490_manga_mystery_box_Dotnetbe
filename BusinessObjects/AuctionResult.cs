using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class AuctionResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("auction_id")]
        public string AuctionId { get; set; }

        [BsonElement("product_id")]
        public string ProductId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("bidder_id")]
        public string BidderId { get; set; }

        [BsonElement("hoster_id")]
        public string HosterId { get; set; }

        [BsonElement("bidder_amount")]
        public decimal BidderAmount { get; set; }

        [BsonElement("host_claim_amount")]
        public decimal HostClaimAmount { get; set; }

        [BsonElement("is_solved")]
        public bool IsSolved { get; set; } = false;
    }
}
