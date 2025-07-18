using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class UserBank
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }
        [BsonElement("account_bank_name")]
        public string AccountBankName { get; set; }

        [BsonElement("bank_number")]
        public string BankNumber { get; set; }

        [BsonElement("bankId")]
        public string BankId { get; set; }
    }
}
