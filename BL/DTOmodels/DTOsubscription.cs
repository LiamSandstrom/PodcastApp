using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels.Interface;

namespace BL.DTOmodels
{
    public class DTOsubscription : IDTOmodels
    {
       
        
        public string SubscriptionId { get; set; }
        public required string UserId { get; set; }
        public required string PodcastId { get; set; }
        public string PodcastTitle { get; set; }
        public string CustomName { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}
