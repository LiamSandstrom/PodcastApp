using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RssFeed
    {
        public required string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public required string RssUrl { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Categories { get; set; }
        public required List<RssItem> Items { get; set; }
    }
}
