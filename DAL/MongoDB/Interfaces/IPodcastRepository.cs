using Models;

namespace DAL.MongoDB.Interfaces
{
    public interface IPodcastRepository : IRepository<Podcast>
    {
        Task<bool> ExistsByRssAsync(string RssUrl);
        public Task<Podcast> GetByRssAsync(string rssUrl);
        Task AddNewEpisodesAsync(string podcastId, List<Episode> newEpisodes);
    }

}
