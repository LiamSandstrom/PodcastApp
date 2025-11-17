using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RssItem
    {
        public required string Title { get; set; }
        public string Description { get; set; }
        public int EpisodeNumber { get; set; }
        public string Duration { get; set; }
        public required DateTime PublishDate { get; set; }
    }
}
