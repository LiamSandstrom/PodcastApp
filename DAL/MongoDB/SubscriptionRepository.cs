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
                   .Ascending(s => s.Email)
                   .Ascending(s => s.RssUrl);

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
                return await _collection.Find(s => s.Email == entity.Email && s.RssUrl == entity.RssUrl)
                .FirstOrDefaultAsync();
            }
        }
        public async Task<Subscription?> GetSubscriptionAsync(string Email, string RssUrl)
        {
            return await _collection
                .Find(s => s.Email == Email && s.RssUrl == RssUrl)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Subscription>> GetByUserIdAsync(string Email)
        {
            return await _collection
                .Find(s => s.Email == Email)
                .ToListAsync();
        }

        public async Task<bool> SubscriptionExists(string Email, string rssUrl)
        {
            var res = await _collection
                .Find(s => s.Email == Email && s.RssUrl == rssUrl)
                .FirstOrDefaultAsync();

            return res != null;
        }

        public async Task<List<Subscription>> GetByCategoryAsync(string userEmail, string categoryId)
        {
            var filter = Builders<Subscription>.Filter.And(
                Builders<Subscription>.Filter.Eq(s => s.UserEmail, userEmail),
                Builders<Subscription>.Filter.AnyEq(s => s.CategoryId, categoryId)
            );

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> AddCategory(string userEmail, string rssUrl, string categoryId)
        {
            var sub = await GetSubscriptionAsync(userEmail, rssUrl);
            if (sub == null)
                return false;

            if (sub.CategoryId.Contains(categoryId)) return false;

            sub.CategoryId.Add(categoryId);

            await UpdateAsync(sub);

            return true;
        }

        public async Task<bool> RemoveCategory(string userEmail, string rssUrl, string CategoryId)
        {
            var sub = await GetSubscriptionAsync(userEmail, rssUrl);
            if (sub == null)
                return false;

            sub.CategoryId.Remove(CategoryId);
            return await UpdateAsync(sub);
        }





    }
}

