using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class SellProduct
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string SellerId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int ExchangeCode { get; set; }
    public bool IsSell { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}