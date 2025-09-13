using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class UserProduct
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string CollectionId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime CollectedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public string CollectorId { get; set; }
    public bool isQuantityUpdateInc { get; set; }
}