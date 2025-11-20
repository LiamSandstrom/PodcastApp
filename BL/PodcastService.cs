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


namespace BL
{
    public class PodcastService : IPodcastService
    {
        private readonly IRssRepository rssRepo;

        public PodcastService(IRssRepository rssRepository)
        {
            rssRepo = rssRepository;
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
                    AllEpisodes = allEpisodes,
                    Episodes = limitedEpisodes,
                    CurrentIndex = limitedEpisodes.Count
                };



                return dtoPodd;
            }
            catch (Exception ex)
            {
                return null;
            }




        }

        public async Task<List<DTOepisode>> GetNextEpisodes(string rssUrl, int index, int amountOfEpisodes)
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


                }).ToList();

                if (allEpisodes == null) return new List<DTOepisode>();

                List<DTOepisode> limitedEpisodes = allEpisodes
                    .Skip(index)
                    .Take(amountOfEpisodes)
                    .ToList();

                return limitedEpisodes;
            }
            catch (Exception ex)
            {
                return null;
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
