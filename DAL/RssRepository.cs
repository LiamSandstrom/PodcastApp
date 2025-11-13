using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;

namespace DAL
{
    public class RssRepository : IRssRepository
    {
        public RssFeed GetFeed(string url)
        {
            using var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);

            var feedDto = new RssFeed
            {
                Title = feed.Title.Text,
                Description = feed.Description.Text ?? "",
                ImageUrl = feed.ImageUrl.ToString(),
                Categories = feed.Categories.Select(c => c.Name).ToList(),
                Items = feed.Items.Select(i => new RssItem
                {
                    Title = i.Title.Text,
                    Description = i.Summary.Text ?? "",
                    PublishDate = i.PublishDate.DateTime
                }).ToList()
            };

            return feedDto;
        }
    }
}
