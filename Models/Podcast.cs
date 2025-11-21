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
        public List<string> Authors { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public required string RssUrl { get; set; }
        public required List<Episode> Episodes { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
