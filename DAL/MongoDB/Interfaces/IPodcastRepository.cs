using Models;
using MongoDB.Driver;

namespace DAL.MongoDB.Interfaces
{
    public interface IPodcastRepository : IRepository<Podcast>
    {
        Task<bool> ExistsByRssAsync(string RssUrl);
        public Task<Podcast> GetByRssAsync(string rssUrl);
        Task AddNewEpisodesAsync(string podcastId, List<Episode> newEpisodes, IClientSessionHandle Session);
        public Task<List<Podcast>> GetAllByRssAsync(List<string> rssUrls);
        public Task<List<Podcast>> GetByCategory(string categoryId);
        public Task<List<Podcast>> GetByRssUrlsAndCategoryAsync(List<string> rssUrls, string categoryId);
    }

}
