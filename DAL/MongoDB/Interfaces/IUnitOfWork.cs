using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DAL.MongoDB.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IClientSessionHandle Session { get; }
        Task StartTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
