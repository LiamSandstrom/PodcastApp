using Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Episode : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PodcastId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishTime { get; set; }
        public int EpisodeNumber { get; set; }
        public string Duration { get; set; }
    }
}
