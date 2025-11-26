using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MongoDB.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<List<string>> GetNamesByIds(List<string> ids);
        public Task<List<Category>> GetAllUserCategories(string email);
        public Task<string?> GetIdByNameAndEmail(string name, string email);
    }
}
