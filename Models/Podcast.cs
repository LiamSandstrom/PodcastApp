using Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Podcast : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Categories { get; set; } = new();
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string RssUrl { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
