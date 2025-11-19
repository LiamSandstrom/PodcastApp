using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.MVVM.Model
{
    public class Podcast
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public required string RssUrl { get; set; }
        public List<Episode> Episodes { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
