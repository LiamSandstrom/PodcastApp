
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace DAL.Rss
{
    public static class SyndicationHelper
    {
        private const string ITUNES_NS = "http://www.itunes.com/dtds/podcast-1.0.dtd";

        /// Extracts authors from SyndicationFeed, preferring iTunes author if present.
        public static List<string> GetAuthors(SyndicationFeed feed)
        {
            // Check iTunes author element
            var itunesAuthor = feed.ElementExtensions
                .Where(e => e.OuterName == "author" && e.OuterNamespace == ITUNES_NS)
                .Select(e => e.GetObject<string>())
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(itunesAuthor))
                return new List<string> { itunesAuthor };

            // Fallback to normal authors
            return feed.Authors.Select(a => a.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
        }

        /// Extracts categories from SyndicationFeed, including iTunes categories.
        public static List<string> GetCategories(SyndicationFeed feed)
        {
            // iTunes categories can be nested, we just take the top-level text attribute
            var itunesCategories = feed.ElementExtensions
                .Where(e => e.OuterName == "category" && e.OuterNamespace == ITUNES_NS)
                .Select(e =>
                {
                    try
                    {
                        var x = e.GetObject<XElement>();
                        return x.Attribute("text")?.Value;
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList();

            if (itunesCategories.Any())
                return itunesCategories;

            // Fallback to normal RSS categories
            return feed.Categories.Select(c => c.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
        }

        /// Extracts iTunes image URL if present
        public static string GetImageUrl(SyndicationFeed feed)
        {
            var itunesImage = feed.ElementExtensions
                .Where(e => e.OuterName == "image" && e.OuterNamespace == ITUNES_NS)
                .Select(e =>
                {
                    try
                    {
                        var x = e.GetObject<XElement>();
                        return x.Attribute("href")?.Value;
                    }
                    catch
                    {
                        return null;
                    }
                })
                .FirstOrDefault();

            return itunesImage ?? feed.ImageUrl?.ToString() ?? "";
        }
    }
}
