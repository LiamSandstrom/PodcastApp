using Models;

namespace DAL.MongoDB.Interfaces
{
    public interface IPodcastRepository : IRepository<Podcast>
    {
        Task<bool> GetByRss(string RssUrl);
    }

}
