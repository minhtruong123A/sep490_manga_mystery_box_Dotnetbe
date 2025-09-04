using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class UserBox
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId { get; set; }
    public string BoxId { get; set; }
    public int Quantity { get; set; }
    public DateTime UpdatedAt { get; set; }
}