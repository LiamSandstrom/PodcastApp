using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal interface IRssRepository
    {
        public List<RssItem> GetFeed(string url);
    }
}
