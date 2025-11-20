using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels;

namespace BL.Interfaces
{
    public interface IPodcastService : IService
    {
        Task<DTOpodcast> GetPodcastFromRssAsync(string rssUrl, int amountOfEpisodes);

         Task<List<DTOepisode>> GetNextEpisodesAsync(string rssUrl, int index, int amountOfEpisodes);

        DTOepisode GetEpisodeByNumber(DTOpodcast podcast, int episodeNumber);
    }
}
