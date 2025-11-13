using DAL.MongoDB.Interfaces;
using Models;
using Models.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        protected readonly IMongoCollection<T> _collection;
        public MongoRepository(IMongoDatabase db, string collectionName)
        {
            _collection = db.GetCollection<T>(collectionName);
        }

        virtual public async Task<T> Add(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> Delete(string id)
        {
            var result = await _collection.DeleteOneAsync(e => e.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;

        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T?> GetById(string id)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> Update(T entity)
        {
            var result = await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
