using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class TransactionHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string WalletId { get; set; }
    public DateTime DataTime { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public int Amount { get; set; }
    public string TransactionCode { get; set; }
}