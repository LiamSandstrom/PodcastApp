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
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(ISubscriptionRepository subRepo, IPodcastRepository podRepo, ICategoryRepository categoryRepo, IUnitOfWork unitOfWork)
        {
            subscriptionRepo = subRepo;
            podcastRepo = podRepo;
            this.categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> SubscribeAsync(string Email, string RssUrl, string customName)
        {
            await _unitOfWork.StartTransactionAsync();
            try
            {
                var podcast = await podcastRepo.GetByRssAsync(RssUrl);
                if (podcast == null)
                    return false;

                var exists = await subscriptionRepo.SubscriptionExists(Email, RssUrl);
                if (exists)
                    return false;

                var sub = new Subscription
                {
                    Email = Email,
                    RssUrl = RssUrl,
                    CustomName = string.IsNullOrWhiteSpace(customName) ? podcast.Title : customName,
                    SubscribedAt = DateTime.UtcNow
                };

                await subscriptionRepo.AddAsync(sub, _unitOfWork.Session);

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }
        public async Task<bool> UnsubscribeAsync(string Email, string RssUrl)
        {
            await _unitOfWork.StartTransactionAsync();
            try
            {
                var existing = await subscriptionRepo.GetSubscriptionAsync(Email, RssUrl);
                if (existing == null)
                    return false;

                var result = await subscriptionRepo.DeleteAsync(existing.Id, _unitOfWork.Session);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
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

        public async Task<bool> RenameSubscriptionAsync(string Email, string RssUrl, string newName)
        {
            await _unitOfWork.StartTransactionAsync();
            try
            {
                var sub = await subscriptionRepo.GetSubscriptionAsync(Email, RssUrl);
                if (sub == null)
                    return false;

                sub.CustomName = newName;

                var result = await subscriptionRepo.UpdateAsync(sub, _unitOfWork.Session);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
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
                await _unitOfWork.StartTransactionAsync();
                var session = _unitOfWork.Session;

                var sub = await subscriptionRepo.GetSubscriptionAsync(email, rssUrl);
                if (sub == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return false;
                }

                if (sub.CategoryId.Contains(categoryId))
                {
                    await _unitOfWork.RollbackAsync();
                    return false;
                }

                sub.CategoryId.Add(categoryId);

                await subscriptionRepo.UpdateAsync(sub, session);

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
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
                await _unitOfWork.StartTransactionAsync();
                var session = _unitOfWork.Session;

                var sub = await subscriptionRepo.GetSubscriptionAsync(userEmail, rssUrl);
                if (sub == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return false;
                }

                sub.CategoryId.Remove(CategoryId);

                await subscriptionRepo.UpdateAsync(sub, session);

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }

        public async Task<List<DTOsubscription>> GetMostPopular()
        {
            try
            {
                var rssUrls = await subscriptionRepo.GetTopLikedPodcasts(10);
                var podcasts = await podcastRepo.GetAllByRssAsync(rssUrls);

                var res = new List<DTOsubscription>();

                foreach (var podcast in podcasts)
                {
                    var sub = new DTOsubscription
                    {
                        RssUrl = podcast.RssUrl,
                        PodcastTitle = podcast.Title,
                        CustomName = podcast.Title,
                        PodcastImgUrl = podcast.ImageUrl,
                    };

                    res.Add(sub);
                }

                return res;
            }
            catch (Exception ex)
            {
                return new List<DTOsubscription>();
            }

        }


    }
}
