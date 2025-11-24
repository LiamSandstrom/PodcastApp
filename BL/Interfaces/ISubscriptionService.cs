using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels;

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
    }
}
