using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels;
using DAL.MongoDB;
using DAL.MongoDB.Interfaces;
using DAL.Rss;
using DAL.Rss.Interfaces;
using BL.Interfaces;
using Models;
using DTO;


namespace BL
{
    public class PodcastService : IPodcastService
    {
        private readonly IRssRepository rssRepo;
        private readonly IPodcastRepository podcastRepo;

        public PodcastService(IPodcastRepository podcastRepository, IRssRepository rssRepository)
        {
            rssRepo = rssRepository;
            podcastRepo = podcastRepository;
        }

        public async Task<DTOpodcast> GetPodcastAsync(string rssUrl, int amountOfEpisodes)
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
                return res;
            }

            return await GetPodcastFromRssAsync(rssUrl, amountOfEpisodes);
        }

        public async Task<DTOpodcast> GetPodcastFromRssAsync(string rssUrl, int amountOfEpisodes)
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
                    Date = item.PublishDate


                }).ToList();

                var limitedEpisodes = allEpisodes.Take(amountOfEpisodes).ToList();

                var dtoPodd = new DTOpodcast
                {
                    Title = feed.Title,
                    Description = feed.Description,
                    Authors = feed.Authors,
                    Categories = feed.Categories,
                    ImageUrl = feed.ImageUrl,
                    RssUrl = feed.RssUrl,
                    Episodes = limitedEpisodes,
                };



                return dtoPodd;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DTOepisode>> GetNextEpisodesAsync(string rssUrl, int index, int amountOfEpisodes)
        {
            try
            {
                var feed = await rssRepo.GetFeed(rssUrl);
                await AddPodcast(feed);

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

        private async Task AddPodcast(RssFeed feed)
        {
            var amountOfEpisodes = 20;

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
                    Categories = feed.Categories,
                    Description = feed.Description,
                    ImageUrl = feed.ImageUrl,
                    RssUrl = feed.RssUrl,
                    Episodes = limitedEpisodes,
                    CreatedAt = DateTime.Now,
                    LastUpdated = DateTime.Now,
                };

                await podcastRepo.AddAsync(pod);

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
    }
}
