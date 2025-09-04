using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class MysteryBox
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int TotalProduct { get; set; }
    public string UrlImage { get; set; }
    public string Title { get; set; }
}