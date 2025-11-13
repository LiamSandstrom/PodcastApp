using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MongoDB.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> Add(T entity);

        Task<T?> GetById(string id);
        Task<IEnumerable<T>> GetAll();

        Task<bool> Update(T entity);

        Task<bool> Delete(string id);
    }
}
