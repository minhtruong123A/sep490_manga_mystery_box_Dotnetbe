using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class ProductOrder
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string SellerId { get; set; }
    public string BuyerId { get; set; }
    public string SellProductId { get; set; }
    public int Amount { get; set; }
}