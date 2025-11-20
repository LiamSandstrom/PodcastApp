using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels.Interface;

namespace BL.DTOmodels
{
    public class DTOepisode : IDTOmodels
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public int EpisodeNumber { get; set; }
        public string DateAndDuration { get; set; }
        public DateTime Date { get; set; }
    }
}
