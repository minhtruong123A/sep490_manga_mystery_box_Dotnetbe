using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Reward
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AchievementId { get; set; }
        public int Conditions { get; set; }
        public string? Url_image { get; set; }
        public string MangaBoxId { get; set; }
        public int Quantity_box { get; set; }
    }
}
