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
        public string Author { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string RssUrl { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
