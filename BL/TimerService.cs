using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels;
using DAL.MongoDB.Interfaces;
using Models;

namespace BL
{
    public class TimerService : IDisposable
    {
        private readonly PodcastService poddService;
        private readonly IPodcastRepository poddRepo;

        private Timer _timer;
        private int _intervalMinutes;

        public TimerService(PodcastService podcastService, IPodcastRepository podcastRepo, int intervalMinutes)
        {
            poddService = podcastService;
            poddRepo = podcastRepo;
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
            var allPodcasts = await poddRepo.GetAllAsync();

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

                var newEpisodes = await poddService.CheckForNewEpisodesAsync(dtoPod);

                if (newEpisodes.Any())
                {
                    Console.WriteLine($"Ny episod hittad i {pod.Title}:");

                    foreach (var ep in newEpisodes)
                    {
                        Console.WriteLine($"  • {ep.Title}");

                    }
                    var mappedEpisode = newEpisodes.Select(ep => new Episode
                    {
                        Title = ep.Title,
                        Description = ep.Description,
                        EpisodeNumber = ep.EpisodeNumber,
                        PublishTime = ep.Date,
                        Duration = ep.DateAndDuration
                    }).ToList();

                    await poddRepo.AddNewEpisodesAsync(pod.Id, mappedEpisode);

                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
