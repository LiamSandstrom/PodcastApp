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
        Task<bool> SubscribeAsync(string userId, string podcastId);
        Task<bool> UnsubscribeAsync(string userId, string podcastId);
        Task<List<DTOsubscription>> GetUserSubscriptionsAsync(string userId);
    }
}
