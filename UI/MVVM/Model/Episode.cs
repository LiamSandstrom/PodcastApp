using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.MVVM.Model
{
    public class Episode
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishTime { get; set; }
        public int EpisodeNumber { get; set; }
        public string Duration { get; set; }

    }
}
