using BL.DTOmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface ISubscriptionService : IService
    {
        Task<bool> SubscribeAsync(string Email, string RssUrl, string customName);
        Task<bool> UnsubscribeAsync(string Email, string RssUrl);
        Task<List<DTOsubscription>> GetUserSubscriptionsAsync(string Email);
        Task<bool> RenameSubscriptionAsync(string Email, string RssUrl, string newName);
        Task<bool> SubscriptionExists(string Email, string RssUrl);
        Task<DTOsubscription> GetSubscriptionAsync(string Email, string RssUrl);
        public Task<List<DTOsubscription>> GetSubscribedPodcastsByCategory(string userEmail, string categoryId);
        public Task<bool> AddCategory(string email, string rssUrl, string categoryId);
        public Task<List<string>> GetCategoriesOnSubscription(string Email, string rssUrl);
        public Task<bool> RemoveCategory(string userEmail, string rssUrl, string CategoryId);
        public Task<List<DTOsubscription>> GetMostPopular();
    }
}
