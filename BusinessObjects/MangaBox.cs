using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class MangaBox 
{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string MysteryBoxId { get; set; }
        public string CollectionTopicId { get; set; }
        public int Quantity { get; set; }
        public DateTime Start_time { get; set; }
        public DateTime End_time { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
}

