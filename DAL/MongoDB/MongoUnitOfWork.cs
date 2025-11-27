using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using DAL.MongoDB.Interfaces;
namespace DAL.MongoDB
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly MongoClient _client;
        public IClientSessionHandle Session { get; private set; }
        public MongoUnitOfWork(MongoClient client)
        {
            _client = client;
        }
        public async Task StartTransactionAsync()
        {
            Session = await _client.StartSessionAsync();
            Session.StartTransaction();
            
        }
        public async Task CommitAsync()
        {
           await Session.CommitTransactionAsync();
        }
        public async Task RollbackAsync()
        {
            await Session.AbortTransactionAsync();
        }
        public void Dispose()
        {
            Session?.Dispose();
        }
        
            


    }
}
