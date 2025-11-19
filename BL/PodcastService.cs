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
    public class PodscastService : IPodcastService
    {
        private readonly IRssRepository rssRepo;
        
        public PodscastService(IRssRepository rssRepository)
        {
            rssRepo = rssRepository;
        }
        
        public async Task<DTOpodcast> GetPodcastFromRssAsync(string rssUrl)
        {
            var feed = await rssRepo.GetFeed(rssUrl);

            var dtoPodd = new DTOpodcast
            {
                Title = feed.Title,
                Description = feed.Description,
                Authors = feed.Authors,
                Categories = feed.Categories,
                ImageUrl = feed.ImageUrl,
                RssUrl = feed.RssUrl,
            };

            var episodes = feed.Items.Select(item => new DTOepisode
            {
                Title = item.Title,
                Description = item.Description,
               EpisodeNumber = item.EpisodeNumber,



            }).ToList();
           
            dtoPodd.Episodes = episodes;

            return dtoPodd;
                
                
            
        }
    }
}
