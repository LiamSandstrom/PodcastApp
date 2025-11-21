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

        private static IUserService _userService;
        public static IUserService UserService
        {
            get => _userService;
            set
            {
                if (_userService != null) return;
                _userService = value;
            }
        }
        public static void SetUp()
        {
            //Connection to Mongo
            var mongoUri = Environment.GetEnvironmentVariable("MONGO_URI");
            if (string.IsNullOrWhiteSpace(mongoUri))
                throw new Exception("Missing MONGO_URI environment variable!");

            var client = new MongoClient(mongoUri);
            string databaseName = "PodcastDB";
            var db = client.GetDatabase(databaseName);

            //Repos
            var rssRepo = new RssRepository();

            var podRepo = new PodcastRepository(db);
            var subRepo = new SubscriptionRepository(db);
            var userReo = new UserRepository(db);

            //Services
            PodcastService = new PodcastService(rssRepo);
            SubscriptionService = new SubscriptionService(subRepo, podRepo);
            UserService = new UserService(userReo);
        }
    }
}
