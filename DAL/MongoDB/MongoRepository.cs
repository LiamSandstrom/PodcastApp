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
    public abstract class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        protected readonly IMongoCollection<T> _collection;
        public MongoRepository(IMongoDatabase db, string collectionName)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentException("Have to provide a collection");

            _collection = db.GetCollection<T>(collectionName);
        }

        //let implementing repos define add. Remember to set the key you add on to unique 
        abstract public Task<T> AddAsync(T entity);

        public async Task<bool> DeleteAsync(string id)
        {
            ValidateId(id);

            var result = await _collection.DeleteOneAsync(e => e.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;

        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            ValidateId(id);

            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateId(entity.Id);

            var result = await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        protected void ValidateId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id cannot be null or empty", nameof(id));
        }
    }
}
