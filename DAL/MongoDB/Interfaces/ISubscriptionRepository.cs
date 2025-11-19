using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MongoDB.Interfaces
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<Subscription?> GetSubscriptionAsync(string userId, string podcastId);
        Task<IEnumerable<Subscription>> GetByUserIdAsync(string userId);
    }
}
