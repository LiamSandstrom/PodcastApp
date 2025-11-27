using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DAL.MongoDB.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> AddAsync(T entity, IClientSessionHandle Session);

        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();

        Task<bool> UpdateAsync(T entity, IClientSessionHandle Session);

        Task<bool> DeleteAsync(string id, IClientSessionHandle Session);
    }
}
