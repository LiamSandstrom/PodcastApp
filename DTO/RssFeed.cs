using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RssFeed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string RssUrl { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Categories { get; set; }
        public List<RssItem> Items { get; set; }
    }
}
