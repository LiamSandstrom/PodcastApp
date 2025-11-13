using DAL.MongoDB.Interfaces;
using Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MongoDB
{
    public class PodcastRepository : MongoRepository<Podcast>, IPodcastRepository
    {
        public PodcastRepository(IMongoDatabase db)
            : base(db, "Podcast") { }

        public async override Task<Podcast> Add(Podcast entity)
        {
            var result = await _collection.Find(e => e.RssUrl == entity.RssUrl).FirstOrDefaultAsync();
            if (result != null) return result;
            await _collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
