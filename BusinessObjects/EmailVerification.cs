using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class EmailVerification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("email")] public string Email { get; set; }

    [BsonElement("code")] public string Code { get; set; }

    [BsonElement("expire_time")] public DateTime ExpireTime { get; set; }
}