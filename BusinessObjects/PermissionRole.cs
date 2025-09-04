using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class PermissionRole
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("permission_id")] public string PermissionId { get; set; }

    [BsonElement("role_id")] public string RoleId { get; set; }
}