using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class Report
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId { get; set; }
    public string SellProductId { get; set; }
    public string SellerId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
}