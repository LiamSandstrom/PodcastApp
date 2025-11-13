using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RssItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int EpisodeNumber { get; set; }
        public string Duration { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
