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
        Task<Subscription?> GetSubscriptionAsync(string Email, string RssUrl);
        Task<IEnumerable<Subscription>> GetByUserIdAsync(string Email);
        public Task<bool> SubscriptionExists(string Email, string rssUrl);

        public Task<List<Subscription>> GetByCategoryAsync(string userEmail, string categoryId);
        public Task<bool> AddCategory(string userEmail, string rssUrl, string categoryId);
        public Task<bool> RemoveCategory(string userEmail, string rssUrl, string CategoryId);
        public Task<List<string>> GetTopLikedPodcasts(int topCount);

    }
}
