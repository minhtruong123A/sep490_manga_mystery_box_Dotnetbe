using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class UserReward
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId { get; set; }
    public string RewardId { get; set; }
    public bool isReceive { get; set; }
    public bool isPublic { get; set; }
}