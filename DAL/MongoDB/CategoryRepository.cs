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
    public class CategoryRepository : MongoRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(IMongoDatabase db)
            : base(db, "Categories")

        {
            var keys = Builders<Category>.IndexKeys
                .Ascending(c => c.UserEmail)
                .Ascending(c => c.Name);

            var options = new CreateIndexOptions
            {
                Unique = true
            };

            _collection.Indexes.CreateOne(
                new CreateIndexModel<Category>(keys, options)
            );
        }

        public async override Task<Category> AddAsync(Category entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);
                return entity;
            }
            catch (MongoWriteException ex)
                when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return await _collection.Find(c => c.UserEmail == entity.UserEmail && c.Name == entity.Name
                ).FirstOrDefaultAsync();
            }
        }
    }
}