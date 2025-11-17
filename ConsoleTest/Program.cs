
using DAL.MongoDB;
using DAL.MongoDB.Interfaces;
using DAL.Rss;
using DAL.Rss.Interfaces;
using Models;
using MongoDB.Driver;
using System.Diagnostics;

MainAsync().GetAwaiter().GetResult();

static async Task MainAsync()
{

    string connectionString = Environment.GetEnvironmentVariable("MONGO_URI")
        ?? throw new Exception("MONGO_URI not set");

    var sw = Stopwatch.StartNew();

    var client = new MongoClient(connectionString);
    var db = client.GetDatabase("PodcastDB");

    await db.RunCommandAsync((Command<dynamic>)"{ping:1}");

    sw.Stop();
    Console.WriteLine($"Handshake + connection: {sw.ElapsedMilliseconds}");
    sw = Stopwatch.StartNew();

    var podcastRepo = new PodcastRepository(db);

    await TestPodcast(db, podcastRepo);

    sw.Stop();
    Console.WriteLine($"Total: {sw.ElapsedMilliseconds}");

    var UserRepository = new UserRepository(db);

    var user = new User();
    user.Name = "test";
    user.Email = "test@gg.com";
    var usr = await UserRepository.AddAsync(user);
    Console.WriteLine(usr.Id);

    // To customize application configuration such as set high DPI settings or default font,
    // see https://aka.ms/applicationconfiguration.

}

static async Task TestPodcast(IMongoDatabase db, IPodcastRepository podcastRepo)
{
    string rssUrl = "https://api.sr.se/api/rss/pod/itunes/3966";

    if (await podcastRepo.ExistsByRssAsync(rssUrl)) return;

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


    var res = await podcastRepo.AddAsync(podcast);
}
