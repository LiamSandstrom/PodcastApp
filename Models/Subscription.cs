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

        public required string Email { get; set; }

        public required string RssUrl { get; set; }
        public string CustomName { get; set; }
        public DateTime SubscribedAt { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
