using BL.DTOmodels;
using BL.Interfaces;
using DAL.MongoDB;
using DAL.MongoDB.Interfaces;
using DAL.Rss;
using DAL.Rss.Interfaces;
using DTO;
using Microsoft.Extensions.Logging;
using Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BL
{
    public class PodcastService : IPodcastService
    {
        private readonly IRssRepository rssRepo;
        private readonly IPodcastRepository podcastRepo;
        private readonly ICategoryRepository categoryRepo;
        private int amountOfEpisodes = 10;
        private readonly IUnitOfWork _unitOfWork;

        public PodcastService(IPodcastRepository podcastRepository, IRssRepository rssRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            rssRepo = rssRepository;
            podcastRepo = podcastRepository;
            categoryRepo = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DTOpodcast> GetPodcastAsync(string rssUrl, int amountOfEpisodes)
        {
            try
            {
                var pod = await podcastRepo.GetByRssAsync(rssUrl);
                if (pod != null)
                {
                    var episodes = pod.Episodes.Select(item => new DTOepisode
                    {
                        Title = item.Title,
                        Description = item.Description,
                        EpisodeNumber = item.EpisodeNumber,
                        DateAndDuration = FormatDateAndDuration(item.PublishTime, item.Duration),
                        Date = item.PublishTime
                    }).ToList();

                    var categories = await categoryRepo.GetNamesByIds(pod.Categories);

                    DTOpodcast res = new DTOpodcast
                    {
                        Title = pod.Title,
                        Description = pod.Description,
                        Authors = pod.Authors,
                        Categories = categories,
                        ImageUrl = pod.ImageUrl,
                        RssUrl = pod.RssUrl,
                        Episodes = episodes,
                    };
                    return res;

                }

                return await GetPodcastFromRssAsync(rssUrl, amountOfEpisodes);
            }
            catch
            {
                return null;
            }
        }

        
        public async Task<DTOpodcast> GetPodcastFromRssAsync(string rssUrl, int amountOfEpisodes)
        {
            try
            {
                var feed = await rssRepo.GetFeed(rssUrl);

                await _unitOfWork.StartTransactionAsync();

                try
                {
                   
                    List<string> categories = new();

                    foreach (var category in feed.Categories)
                    {
                        Category newCat = new Category
                        {
                            Name = category,
                            UserEmail = null
                        };

                        var savedCat = await categoryRepo.AddAsync(newCat, _unitOfWork.Session);
                        categories.Add(savedCat.Id);
                    }

                    
                    await AddPodcast(feed, categories);

                   
                    await _unitOfWork.CommitAsync();
                }
                catch
                {
                    
                    await _unitOfWork.RollbackAsync();
                    throw;
                }

               
                var allEpisodes = feed.Items.Select(item => new DTOepisode
                {
                    Title = item.Title,
                    Description = item.Description,
                    EpisodeNumber = item.EpisodeNumber,
                    DateAndDuration = FormatDateAndDuration(item.PublishDate, item.Duration),
                    Date = item.PublishDate
                }).ToList();

                var limitedEpisodes = allEpisodes.Take(amountOfEpisodes).ToList();

                return new DTOpodcast
                {
                    Title = feed.Title,
                    Description = feed.Description,
                    Authors = feed.Authors,
                    Categories = feed.Categories,
                    ImageUrl = feed.ImageUrl,
                    RssUrl = feed.RssUrl,
                    Episodes = limitedEpisodes,
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<DTOepisode>> GetNextEpisodesAsync(string rssUrl, int index, int amountOfEpisodes)
        {
            try
            {
                var feed = await rssRepo.GetFeed(rssUrl);

                var allEpisodes = feed.Items.Select(item => new DTOepisode
                {
                    Title = item.Title,
                    Description = item.Description,
                    EpisodeNumber = item.EpisodeNumber,
                    DateAndDuration = FormatDateAndDuration(item.PublishDate, item.Duration),
                    Date = item.PublishDate,

                }).ToList();

                if (allEpisodes == null) return new List<DTOepisode>();

                List<DTOepisode> limitedEpisodes = allEpisodes
                    .Skip(index)
                    .OrderByDescending(i => i.Date)
                    .Take(amountOfEpisodes)
                    .ToList();

                return limitedEpisodes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DTOpodcast>> GetAllPodcastsFromRssListAsync(List<string> RssList)
        {
            try
            {
                var podcasts = await podcastRepo.GetAllByRssAsync(RssList);

                List<DTOpodcast> resList = new();

                foreach (var pod in podcasts)
                {
                    var episodes = pod.Episodes.Select(item => new DTOepisode
                    {
                        Title = item.Title,
                        Description = item.Description,
                        EpisodeNumber = item.EpisodeNumber,
                        DateAndDuration = FormatDateAndDuration(item.PublishTime, item.Duration),
                        Date = item.PublishTime
                    }).ToList();

                    DTOpodcast res = new DTOpodcast
                    {
                        Title = pod.Title,
                        Description = pod.Description,
                        Authors = pod.Authors,
                        Categories = pod.Categories,
                        ImageUrl = pod.ImageUrl,
                        RssUrl = pod.RssUrl,
                        Episodes = episodes,
                    };

                    resList.Add(res);
                }
                return resList;
            }
            catch (Exception ex)
            {
                return new List<DTOpodcast>();
            }

        }

        private async Task AddPodcast(RssFeed feed, List<string> categories)
        {


            try
            {
                var allEpisodes = feed.Items.Select(item => new Episode
                {
                    Title = item.Title,
                    Description = item.Description,
                    EpisodeNumber = item.EpisodeNumber,
                    PublishTime = DateTime.Now,
                    Duration = item.Duration,
                }).ToList();

                var limitedEpisodes = allEpisodes.Take(amountOfEpisodes).ToList();

                var pod = new Podcast
                {
                    Title = feed.Title,
                    Authors = feed.Authors,
                    Categories = categories,
                    Description = feed.Description,
                    ImageUrl = feed.ImageUrl,
                    RssUrl = feed.RssUrl,
                    Episodes = limitedEpisodes,
                    CreatedAt = DateTime.Now,
                    LastUpdated = DateTime.Now,
                };

                await _unitOfWork.StartTransactionAsync();

                try
                {
                    
                    await podcastRepo.AddAsync( pod, _unitOfWork.Session);

                    
                    await _unitOfWork.CommitAsync();
                }
                catch
                {
                   
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
            
            catch (Exception ex)
            {

            }
        }



        public static string FormatDateAndDuration(DateTime publishDate, string durationString)
        {
            string date = publishDate.ToString("yyyy-MM-dd");

            if (string.IsNullOrWhiteSpace(durationString))
                return $"{date}";

            string hours = "";
            string minutes = "";

            try
            {

                var arr = durationString.Split(":");
                hours = arr[0] + "h";
                minutes = arr[1] + "min";
            }
            catch (Exception ex)
            {
            }

            return date + " - " + hours + " " + minutes;
        }
        
        public async Task<List<DTOepisode>> CheckForNewEpisodesAsync(DTOpodcast podcast)
        {
            try
            {
                if (podcast == null)
                    throw new ArgumentNullException(nameof(podcast));

                if (string.IsNullOrWhiteSpace(podcast.RssUrl))
                    throw new ArgumentException("Podcast is missing RSS url");

                
                RssFeed feed = await rssRepo.GetFeed(podcast.RssUrl);

                
                var rssEpisodes = feed.Items
                    .Select(item => new DTOepisode
                    {
                        Title = item.Title ?? "",
                        Description = item.Description ?? "",
                        EpisodeNumber = item.EpisodeNumber,
                        Date = item.PublishDate.ToUniversalTime(),
                        DateAndDuration = $"{item.PublishDate.ToShortDateString()} | {item.Duration}"
                    })
                    .OrderByDescending(ep => ep.Date)
                    .Take(amountOfEpisodes)
                    .OrderBy(ep => ep.Date)
                    .ToList();

                
                var latestSaved = podcast.Episodes
                    .OrderByDescending(e => e.Date)
                    .FirstOrDefault();

               
                var newEpisodes = latestSaved == null
                    ? rssEpisodes
                    : rssEpisodes.Where(ep => ep.Date > latestSaved.Date).ToList();

                if (!newEpisodes.Any())
                    return newEpisodes; // Inga nya → klart

               
                var dbPodcast = await podcastRepo.GetByRssAsync(podcast.RssUrl);
                if (dbPodcast == null)
                    return newEpisodes;

              
                await _unitOfWork.StartTransactionAsync();

                try
                {
                   
                    var dbEpisodes = newEpisodes.Select(ep => new Episode
                    {
                        Title = ep.Title,
                        Description = ep.Description,
                        EpisodeNumber = ep.EpisodeNumber,
                        PublishTime = ep.Date,
                        Duration = ep.DateAndDuration
                    }).ToList();

                    
                    await podcastRepo.AddNewEpisodesAsync(
                        dbPodcast.Id,
                        dbEpisodes,
                        _unitOfWork.Session
                    );

                   
                    await _unitOfWork.CommitAsync();
                }
                catch
                {
                    
                    await _unitOfWork.RollbackAsync();
                    throw;
                }

                return newEpisodes;
            }
            catch
            {
                return new List<DTOepisode>();
            }
        }
    }
}
