using BL.DTOmodels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface ICategoryService
    {
        public Task<Category> CreateCategory(string name, string? userEmail);
        public Task<bool> RenameCategory(string id, string newName, string currentUserEmail);
        public Task<bool> DeleteCategory(string id, string currentUserEmail);
        public Task<List<DTOcategory>> GetForUser(string userEmail);
        public Task<List<DTOcategory>> GetUserOnly(string userEmail);
        public Task<string> GetIdByNameAndEmail(string name, string email);


    }
}
