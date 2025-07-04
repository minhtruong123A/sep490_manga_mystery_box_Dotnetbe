using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class TransactionFee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ReferenceId { get; set; }
        public string ReferenceType { get; set; }
        public string FromUserId { get; set; }
        public string ProductId { get; set; }
        public int GrossAmount { get; set; }
        public int FeeAmount { get; set; }
        public double FeeRate { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
