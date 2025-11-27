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
        private readonly IUnitOfWork _unitOfWork;

        private Timer _timer;
        private int _intervalMinutes;

        public TimerService(PodcastService podcastService, IPodcastRepository podcastRepo, int intervalMinutes, IUnitOfWork unitOfWork)
        {
            poddService = podcastService;
            poddRepo = podcastRepo;
            _intervalMinutes = intervalMinutes;
            _unitOfWork = unitOfWork;
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

                if (!newEpisodes.Any())
                    continue;

                Console.WriteLine($"Ny episod hittad i {pod.Title}:");
                foreach (var ep in newEpisodes)
                    Console.WriteLine($"  • {ep.Title}");

                var mappedEpisodes = newEpisodes.Select(ep => new Episode
                {
                    Title = ep.Title,
                    Description = ep.Description,
                    EpisodeNumber = ep.EpisodeNumber,
                    PublishTime = ep.Date,
                    Duration = ep.DateAndDuration
                }).ToList();

                
                await _unitOfWork.StartTransactionAsync();
                var session = _unitOfWork.Session;

                try
                {
                    await poddRepo.AddNewEpisodesAsync(pod.Id, mappedEpisodes, session);

                    
                    await _unitOfWork.CommitAsync();
                }
                catch
                {
                    
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
