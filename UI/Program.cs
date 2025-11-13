using DAL;
using DAL.MongoDB;
using DAL.MongoDB.Interfaces;
using DAL.Rss;
using DAL.Rss.Interfaces;
using Models;
using MongoDB.Driver;

namespace UI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        string connectionString = Environment.GetEnvironmentVariable("MONGO_URI")
            ?? throw new Exception("MONGO_URI not set");

        var client = new MongoClient(connectionString);
        var db = client.GetDatabase("PodcastDB");

        var podcastRepo = new PodcastRepository(db);

        TestPodcast(db, podcastRepo).GetAwaiter().GetResult();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.

        MessageBox.Show("test");
    }

    static async Task TestPodcast(IMongoDatabase db, IPodcastRepository podcastRepo)
    {
        IRssRepository repo = new RssRepository();

        var feed = await repo.GetFeed("https://api.sr.se/api/rss/pod/itunes/3966");
        var podcast = new Podcast
        {
            Title = feed.Title,
            Description = feed.Description,
            Categories = feed.Categories,
            ImageUrl = feed.ImageUrl,
            RssUrl = feed.RssUrl,
            CreatedAt = DateTime.UtcNow,
            Authors = feed.Authors,
        };

        var res = await podcastRepo.Add(podcast);
        MessageBox.Show(res.Title);
    }
}