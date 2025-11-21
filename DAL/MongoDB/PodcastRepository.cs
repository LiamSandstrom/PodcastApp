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

        public async override Task<Podcast> AddAsync(Podcast entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);
                return entity;
            }
            catch (MongoWriteException ex)
                when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return await _collection.Find(e => e.RssUrl == entity.RssUrl).FirstOrDefaultAsync();
            }
        }

        public async Task<bool> ExistsByRssAsync(string rssUrl)
        {
            return await _collection.Find(e => e.RssUrl == rssUrl).AnyAsync();
        }

        public async Task<Podcast> GetByRssAsync(string rssUrl)
        {
            return await _collection.Find(e => e.RssUrl == rssUrl).FirstOrDefaultAsync();
        }


    }
}
