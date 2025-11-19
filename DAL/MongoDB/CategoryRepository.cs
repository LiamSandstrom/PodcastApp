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
            : base(db, "Categories") { }

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
                return await _collection.Find(e => e.Name == entity.Name).FirstOrDefaultAsync();
            }
        }
    }
}
