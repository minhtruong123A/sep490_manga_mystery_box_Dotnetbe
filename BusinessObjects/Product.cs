using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
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
        public Product(string title, string rarityId, string description = "")
        {
            Name = title;
            RarityId = rarityId;
            Description = string.IsNullOrEmpty(description) ? $"Design of {title}" : description;
        }

    }
}
