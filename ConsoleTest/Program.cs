
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
}


