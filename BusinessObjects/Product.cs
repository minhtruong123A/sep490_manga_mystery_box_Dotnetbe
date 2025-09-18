using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public string RarityId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UrlImage { get; set; }
    public bool Is_Block { get; set; }
    public int Quantity { get; set; }
    public int QuantityCurrent { get; set; }
    public int Status { get; set; }
    public string CollectionId { get; set; }
    /*public Product(string title, string rarityId, string description = "")
    {
        Name = title;
        RarityId = rarityId;
        Description = string.IsNullOrEmpty(description) ? $"Design of {title}" : description;
    }*/
}