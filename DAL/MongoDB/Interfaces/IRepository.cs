using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MongoDB.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> AddAsync(T entity);

        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(string id);
    }
}
