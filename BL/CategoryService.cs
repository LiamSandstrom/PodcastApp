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
public class CategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }


    public async Task<Category> CreateCategory(string name, string? userEmail)
    {
        var category = new Category
        {
            Name = name,
            UserEmail = userEmail
        };

        return await _repo.AddAsync(category);
    }


    public async Task<IEnumerable<Category>> GetForUser(string userEmail)
    {
        var all = await _repo.GetAllAsync();

        return all.Where(c =>
            c.UserEmail == null ||
            c.UserEmail == userEmail
        );
    }


    public async Task<bool> RenameCategory(string id, string newName, string currentUserEmail)
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


    public async Task<bool> DeleteCategory(string id, string currentUserEmail)
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
  }
}
