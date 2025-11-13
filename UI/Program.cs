
using DAL;
using DAL.MongoDB;
using DAL.MongoDB.Interfaces;
using DAL.Rss;
using DAL.Rss.Interfaces;
using Models;
using MongoDB.Driver;
using System.Diagnostics;

namespace UI;

static class Program
{
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        MainAsync().GetAwaiter().GetResult();
        MessageBox.Show("done");
    }
    static async Task MainAsync()
    {

        var sw = Stopwatch.StartNew();
        string connectionString = Environment.GetEnvironmentVariable("MONGO_URI")
            ?? throw new Exception("MONGO_URI not set");

        sw.Stop();
        MessageBox.Show($"Startup: {sw.ElapsedMilliseconds}");
        sw = Stopwatch.StartNew();

        var client = new MongoClient(connectionString);
        var db = client.GetDatabase("PodcastDB");

        await db.RunCommandAsync((Command<dynamic>)"{ping:1}");

        sw.Stop();
        MessageBox.Show($"Handshake + connection: {sw.ElapsedMilliseconds}");
        sw = Stopwatch.StartNew();

        var podcastRepo = new PodcastRepository(db);

        await TestPodcast(db, podcastRepo);

        sw.Stop();
        MessageBox.Show($"total: {sw.ElapsedMilliseconds}");

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.

    }

    static async Task TestPodcast(IMongoDatabase db, IPodcastRepository podcastRepo)
    {
        string rssUrl = "https://api.sr.se/api/rss/pod/itunes/3966";

        if (await podcastRepo.GetByRss(rssUrl)) return;

        IRssRepository repo = new RssRepository();

        var feed = await repo.GetFeed(rssUrl);
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
    }
}

