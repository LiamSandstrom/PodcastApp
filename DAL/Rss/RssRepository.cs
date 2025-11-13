using DAL.Rss.Interfaces;
using DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DAL.Rss
{
    public class RssRepository : IRssRepository
    {
        private static readonly HttpClient _httpClient = new();
        public async Task<RssFeed> GetFeed(string url)
        {
            using var stream = await _httpClient.GetStreamAsync(url);

            using var reader = XmlReader.Create(stream);
            var feed = SyndicationFeed.Load(reader);

            var feedDto = new RssFeed
            {
                Title = feed.Title?.Text ?? "",
                Description = feed.Description?.Text ?? "",
                Authors = SyndicationHelper.GetAuthors(feed),
                Categories = SyndicationHelper.GetCategories(feed),
                ImageUrl = SyndicationHelper.GetImageUrl(feed),
                RssUrl = url,
                Items = feed.Items.Select(item => new RssItem
                {
                    Title = item.Title?.Text ?? "",
                    Description = item.Summary?.Text ?? "",
                    PublishDate = item.PublishDate.DateTime,
                }).ToList()
            };

            return feedDto;
        }
    }
}
