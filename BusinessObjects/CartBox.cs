using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class CartBox
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string CartId { get; set; }
    public string MangaBoxId { get; set; }
    public int Quantity { get; set; }
}