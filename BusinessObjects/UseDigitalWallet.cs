using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class UseDigitalWallet
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("ammount")] public decimal Ammount { get; set; }

    [BsonElement("is_active")] public bool IsActive { get; set; }
}