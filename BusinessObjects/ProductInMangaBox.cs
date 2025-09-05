using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class ProductInMangaBox
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string MangaBoxId { get; set; }
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Chance { get; set; }
}