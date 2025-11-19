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
    public class SubscriptionRepository : MongoRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(IMongoDatabase db)
            : base(db, "Subscriptions") 
        {
            var keys = Builders<Subscription>.IndexKeys
                   .Ascending(s => s.UserId)
                   .Ascending(s => s.PodcastId);

            var options = new CreateIndexOptions
            {
                Unique = true
            };

            _collection.Indexes.CreateOne(
                new CreateIndexModel<Subscription>(keys, options)
            );

        }


        public async override Task<Subscription> AddAsync(Subscription entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);
                return entity;
            }
            catch (MongoWriteException ex)
               when (ex.WriteError != null &&
                    (ex.WriteError.Category == ServerErrorCategory.DuplicateKey ||
                     ex.WriteError.Code == 11000))
            {
                return await _collection.Find(s => s.UserId == entity.UserId && s.PodcastId == entity.PodcastId)
                .FirstOrDefaultAsync();
            }
        }
    }
}

