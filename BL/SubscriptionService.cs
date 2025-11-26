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
        private readonly ICategoryRepository categoryRepo;
        public SubscriptionService(ISubscriptionRepository subRepo, IPodcastRepository podRepo, ICategoryRepository categoryRepo)
        {
            subscriptionRepo = subRepo;
            podcastRepo = podRepo;
            this.categoryRepo = categoryRepo;
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

        public async Task<List<DTOsubscription>> GetSubscribedPodcastsByCategory(string userEmail, string categoryId)
        {
            try
            {
                var subscriptions = await subscriptionRepo.GetByUserIdAsync(userEmail);
                if (!subscriptions.Any()) return new List<DTOsubscription>();

                var rssUrls = subscriptions.Select(s => s.RssUrl).ToList();
                var podcasts = await podcastRepo.GetAllByRssAsync(rssUrls);

                var filtered = subscriptions
                    .Select(sub =>
                    {
                        var pod = podcasts.FirstOrDefault(p => p.RssUrl == sub.RssUrl);
                        if (pod != null && pod.Categories.Contains(categoryId) || sub.CategoryId.Contains(categoryId))
                        {
                            return new DTOsubscription
                            {
                                Email = sub.Email,
                                RssUrl = sub.RssUrl,
                                CustomName = sub.CustomName,
                                PodcastTitle = pod.Title,
                                PodcastImgUrl = pod.ImageUrl,
                                SubscribedAt = sub.SubscribedAt,
                                CategoryId = sub.CategoryId
                            };
                        }
                        return null;
                    })
                    .Where(dto => dto != null)
                    .ToList()!;

                return filtered;
            }
            catch (Exception)
            {
                return new List<DTOsubscription>();
            }
        }


        public async Task<bool> AddCategory(string email, string rssUrl, string categoryId)
        {
            try
            {
                await subscriptionRepo.AddCategory(email, rssUrl, categoryId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<List<string>> GetCategoriesOnSubscription(string Email, string rssUrl)
        {
            try
            {
                var sub = await subscriptionRepo.GetSubscriptionAsync(Email, rssUrl);
                var res = await categoryRepo.GetNamesByIds(sub.CategoryId);
                return res;

            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
        public async Task<bool> RemoveCategory(string userEmail, string rssUrl, string CategoryId)
        {
            try
            {
                return await subscriptionRepo.RemoveCategory(userEmail, rssUrl, CategoryId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
