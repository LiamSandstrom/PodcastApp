using Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Models
{
    public class Subscription : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string PodcastId { get; set; }
        public string CustomName { get; set; }
        public DateTime SubscribedAt { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
