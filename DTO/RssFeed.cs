using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    internal class RssFeed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public List<RssItem> Items { get; set; }
    }
}
