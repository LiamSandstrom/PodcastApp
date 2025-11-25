using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
        public async Task<bool> SubscribeAsync(string Email, string RssUrl, string customName)
        {
            try
            {
                var podcast = await podcastRepo.GetByRssAsync(RssUrl);
                if (podcast == null)
                    return false;


                var existing = await subscriptionRepo.SubscriptionExists(Email, RssUrl);
                if (existing == true)
                    return false;


                var sub = new Subscription
                {
                    Email = Email,
                    RssUrl = RssUrl,
                    CustomName = string.IsNullOrWhiteSpace(customName) ? podcast.Title : customName,
                    SubscribedAt = DateTime.Now
                };

                await subscriptionRepo.AddAsync(sub);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> UnsubscribeAsync(string Email, string RssUrl)
        {
            try
            {
                var existing = await subscriptionRepo.GetSubscriptionAsync(Email, RssUrl);
                if (existing == null)
                    return false;

                return await subscriptionRepo.DeleteAsync(existing.Id);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<DTOsubscription>> GetUserSubscriptionsAsync(string Email)
        {
            try
            {
                var subs = await subscriptionRepo.GetByUserIdAsync(Email);
                var result = new List<DTOsubscription>();

                foreach (var sub in subs)
                {
                    var podcast = await podcastRepo.GetByRssAsync(sub.RssUrl);

                    result.Add(new DTOsubscription
                    {

                        Email = sub.Email,
                        RssUrl = sub.RssUrl,
                        CustomName = sub.CustomName,
                        PodcastImgUrl = podcast.ImageUrl,
                        PodcastTitle = podcast?.Title ?? "(deleted)",
                        SubscribedAt = sub.SubscribedAt,
                        CategoryId = sub.CategoryId
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                return new List<DTOsubscription>();

            }
        }

        public async Task<bool> RenameSubscriptionAsync(string userId, string podcastId, string newName)
        {
            try
            {
                var sub = await subscriptionRepo.GetSubscriptionAsync(userId, podcastId);
                if (sub == null)
                    return false;

                sub.CustomName = newName;
                return await subscriptionRepo.UpdateAsync(sub);

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SubscriptionExists(string Email, string RssUrl)
        {
            return await subscriptionRepo.SubscriptionExists(Email, RssUrl);
        }

        public async Task<DTOsubscription?> GetSubscriptionAsync(string Email, string RssUrl)
        {
            try
            {
                var res = await subscriptionRepo.GetSubscriptionAsync(Email, RssUrl);
                return new DTOsubscription
                {
                    Email = res.Email,
                    RssUrl = res.RssUrl,
                    PodcastTitle = res.CustomName,
                    CustomName = res.CustomName,
                    SubscribedAt = res.SubscribedAt,
                    CategoryId = res.CategoryId
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DTOsubscription>> GetUserSubscriptionsByCategory(string UserEmail, string CategoryId)
        {
            var subs = await subscriptionRepo.GetByCategoryAsync(UserEmail, CategoryId);
            var result = new List<DTOsubscription>();

            foreach (var sub in subs)
            {
                var podcast = await podcastRepo.GetByRssAsync(sub.RssUrl);

                result.Add(new DTOsubscription
                {
                    Email = sub.Email,
                    RssUrl = sub.RssUrl,
                    CustomName = sub.CustomName,
                    PodcastImgUrl = podcast?.ImageUrl,
                    PodcastTitle = podcast?.Title ?? "(deleted)",
                    SubscribedAt = sub.SubscribedAt,
                    CategoryId = sub.CategoryId
                });
            }

            return result;
        }
    }
}
