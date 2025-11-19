using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels.Interface;

namespace BL.DTOmodels
{
    public class DTOpodcast : IDTOmodels
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public string RssUrl { get; set; }

        public List<DTOepisode> Episodes { get; set; } = new();

    }
}
