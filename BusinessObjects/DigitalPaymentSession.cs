using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class DigitalPaymentSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string WalletId { get; set; }
    public string OrderId { get; set; }
    public string Type { get; set; }
    public int Amount { get; set; }
    public bool IsWithdraw { get; set; }
}