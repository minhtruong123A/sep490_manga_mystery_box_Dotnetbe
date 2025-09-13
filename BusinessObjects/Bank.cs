using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class Bank
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("abbreviation")] public string Abbreviation { get; set; }

    [BsonElement("Name")] public string Name { get; set; }
}