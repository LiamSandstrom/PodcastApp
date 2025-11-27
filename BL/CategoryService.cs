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
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
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

                
                await _unitOfWork.StartTransactionAsync();

                try
                {
                    
                    var created = await _repo.AddAsync(category, _unitOfWork.Session);

                    await _unitOfWork.CommitAsync();
                    return created;
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
            catch
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
            await _unitOfWork.StartTransactionAsync();

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

                var result = await _repo.UpdateAsync(existing, _unitOfWork.Session);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }


        public async Task<bool> DeleteCategory(string id, string currentUserEmail)
        {
            try
            {
                await _unitOfWork.StartTransactionAsync();

                
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null)
                    return false;

                if (existing.UserEmail == null)
                    throw new InvalidOperationException("Standard categories cannot be deleted.");

                if (existing.UserEmail != currentUserEmail)
                    throw new UnauthorizedAccessException("You can only delete your own categories.");

                
                var success = await _repo.DeleteAsync(id, _unitOfWork.Session);

                await _unitOfWork.CommitAsync();
                return success;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
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
