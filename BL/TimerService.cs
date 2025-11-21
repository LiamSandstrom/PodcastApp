using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels;
using DAL.MongoDB.Interfaces;

namespace BL
{
    public class UpdateTimerService : IDisposable
    {
        private readonly PodcastService _podcastService;
        private readonly IPodcastRepository _podcastRepo;

        private Timer _timer;
        private int _intervalMinutes;

        public UpdateTimerService(PodcastService podcastService, IPodcastRepository podcastRepo, int intervalMinutes)
        {
            _podcastService = podcastService;
            _podcastRepo = podcastRepo;
            _intervalMinutes = intervalMinutes;
        }

        public void Start()
        {
            _timer = new Timer(async _ =>
            {
                await RunUpdateCheck();
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(_intervalMinutes));
        }

        private async Task RunUpdateCheck()
        {
            var allPodcasts = await _podcastRepo.GetAllAsync();

            foreach (var pod in allPodcasts)
            {
                var dtoPod = new DTOpodcast
                {
                    Title = pod.Title,
                    Description = pod.Description,
                    ImageUrl = pod.ImageUrl,
                    Authors = pod.Authors,
                    Categories = pod.Categories,
                    RssUrl = pod.RssUrl,
                    Episodes = pod.Episodes.Select(e => new DTOepisode
                    {
                        Title = e.Title,
                        Description = e.Description,
                        EpisodeNumber = e.EpisodeNumber,
                        Date = e.PublishTime
                    }).ToList()
                };

                var newEpisodes = await _podcastService.CheckForNewEpisodesAsync(dtoPod);

                if (newEpisodes.Any())
                {
                    Console.WriteLine($"Ny episod hittad i {pod.Title}:");

                    foreach (var ep in newEpisodes)
                    {
                        Console.WriteLine($"  • {ep.Title}");

                        // lägg till i DB (du behöver en metod för detta)
                        await _podcastRepo.AddEpisodeAsync(pod.Id, ep);
                    }
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
