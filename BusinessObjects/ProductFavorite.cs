using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class ProductFavorite
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string User_Id { get; set; }
    public string User_productId { get; set; }
}