using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using BL.DTOmodels.Interface;

namespace BL.DTOmodels
{
    public class DTOsubscription : IDTOmodels
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string PodcastId { get; set; }
        public string CustomName { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}
