using BL.DTOmodels;
using BL.Interfaces;
using DAL.MongoDB;
using DAL.MongoDB.Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }


        public async Task<Category> CreateCategory(string name, string? userEmail)
        {
            try
            {
                var category = new Category
                {
                    Name = name,
                    UserEmail = userEmail
                };

                return await _repo.AddAsync(category);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<DTOcategory>> GetForUser(string userEmail)
        {
            try
            {
                var all = await _repo.GetAllAsync();

                return all.Where(c =>
                    c.UserEmail == null ||
                    c.UserEmail == userEmail
                ).Select(c => new DTOcategory
                {
                    Id = c.Id,
                    Name = c.Name,
                }).ToList();

            }
            catch (Exception ex)
            {
                return new List<DTOcategory>();
            }
        }
        public async Task<List<DTOcategory>> GetUserOnly(string userEmail)
        {
            try
            {
                var all = await _repo.GetAllUserCategories(userEmail);
                List<DTOcategory> res = new();

                foreach (var category in all)
                {
                    DTOcategory item = new DTOcategory
                    {
                        Id = category.Id,
                        Name = category.Name,
                    };
                    res.Add(item);
                }
                return res;
            }
            catch (Exception ex)
            {
                return new List<DTOcategory>();
            }

        }


        public async Task<bool> RenameCategory(string id, string newName, string currentUserEmail)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(id);

                if (existing == null)
                    return false;

                if (existing.UserEmail == null)
                    throw new InvalidOperationException("Standard categories cannot be renamed.");


                if (existing.UserEmail != currentUserEmail)
                    throw new UnauthorizedAccessException("You can only rename your own categories.");

                existing.Name = newName;

                return await _repo.UpdateAsync(existing);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> DeleteCategory(string id, string currentUserEmail)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(id);

                if (existing == null)
                    return false;


                if (existing.UserEmail == null)
                    throw new InvalidOperationException("Standard categories cannot be deleted.");


                if (existing.UserEmail != currentUserEmail)
                    throw new UnauthorizedAccessException("You can only delete your own categories.");

                return await _repo.DeleteAsync(id);
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GetIdByNameAndEmail(string name, string email)
        {
            try
            {
                return await _repo.GetIdByNameAndEmail(name, email);
            }
            catch
            {
                return null;
            }
        }


    }
}
