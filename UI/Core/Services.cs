using BL;
using BL.Interfaces;
using DAL.MongoDB;
using DAL.Rss;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Core
{
    public class Services
    {
        private static IPodcastService _podcastService;
        public static IPodcastService PodcastService
        {
            get => _podcastService;
            set
            {
                if (_podcastService != null) return;
                _podcastService = value;
            }
        }
        private static ISubscriptionService _subscriptionService;
        public static ISubscriptionService SubscriptionService
        {
            get => _subscriptionService;
            set
            {
                if (_subscriptionService != null) return;
                _subscriptionService = value;
            }
        }
        public static void SetUp()
        {
            //Setup services and db/repo injections
            var mongoUri = Environment.GetEnvironmentVariable("MONGO_URI");
            if (string.IsNullOrWhiteSpace(mongoUri))
                throw new Exception("Missing MONGO_URI environment variable!");

            var client = new MongoClient(mongoUri);

            string databaseName = "PodcastDB";
            var db = client.GetDatabase(databaseName);

            var rssRepo = new RssRepository();
            PodcastService = new PodcastService(rssRepo);

            var podRepo = new PodcastRepository(db);
            var subRepo = new SubscriptionRepository(db);

            SubscriptionService = new SubscriptionService(subRepo, podRepo);
        }
    }
}
