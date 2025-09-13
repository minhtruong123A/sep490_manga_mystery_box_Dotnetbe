using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects;

public class AuctionSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("title")] public string Title { get; set; }

    [BsonElement("descripition")] public string Descripition { get; set; }

    [BsonElement("start_time")] public DateTime Start_time { get; set; }

    [BsonElement("end_time")] public DateTime End_time { get; set; }

    [BsonElement("seller_id")] public string Seller_id { get; set; }

    [BsonElement("status")] public int Status { get; set; }
}