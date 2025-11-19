using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels;
using BL.Interfaces;
using DAL.MongoDB.Interfaces;
using Models;


namespace BL
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository subscriptionRepo;
        private readonly IPodcastRepository podcastRepo;
        public SubscriptionService(ISubscriptionRepository subRepo, IPodcastRepository podRepo)
        {
            subscriptionRepo = subRepo;
            podcastRepo = podRepo;
        }
       public async Task<bool> SubscribeAsync(string userId, string podcastId)
        {
            
            var podcast = await podcastRepo.GetByIdAsync(podcastId);
            if (podcast == null)
                return false;

            
            var existing = await subscriptionRepo.GetSubscriptionAsync(userId, podcastId);
            if (existing != null)
                return false;

            
            var sub = new Subscription
            {
                UserId = userId,
                PodcastId = podcastId,
                CustomName = podcast.Title,
                SubscribedAt = DateTime.Now
            };

            await subscriptionRepo.AddAsync(sub);
            return true;
        }
       public async Task<bool> UnsubscribeAsync(string userId, string podcastId)
        {
            var existing = await subscriptionRepo.GetSubscriptionAsync(userId, podcastId);
            if (existing == null)
                return false; 

            return await subscriptionRepo.DeleteAsync(existing.Id);
        }
        public async Task<List<DTOsubscription>> GetUserSubscriptionsAsync(string userId)
        {
            var subs = await subscriptionRepo.GetByUserIdAsync(userId);
            var result = new List<DTOsubscription>();

            foreach (var sub in subs)
            {
                var podcast = await podcastRepo.GetByIdAsync(sub.PodcastId);

                result.Add(new DTOsubscription
                {
                    SubscriptionId = sub.Id,
                    UserId = sub.UserId,
                    PodcastId = sub.PodcastId,
                    CustomName = sub.CustomName,
                    PodcastTitle = podcast?.Title ?? "(deleted)",
                    SubscribedAt = sub.SubscribedAt
                });
            }

            return result;

        }
    }
}
