using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Models
{
    public class Subscription
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PodcastId { get; set; }
        public string CustomName { get; set; }
        public DateTime SubscribedAt { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
