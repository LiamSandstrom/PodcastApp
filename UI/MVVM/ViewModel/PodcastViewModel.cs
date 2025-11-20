using BL.DTOmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UI.MVVM.ViewModel
{
    public class PodcastViewModel : ObservableObject
    {
        private string _rssUrl = "";

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private int _index = 0;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

        private string _imageUrl;

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<DTOepisode> Episodes { get; set; } = new();
        public ObservableCollection<string> Categories { get; set; } = new();

        public readonly MainViewModel MVM;

        public RelayCommand GetNextEpisodesCommand { get; }

        public PodcastViewModel(MainViewModel MVM)
        {
            this.MVM = MVM;

            GetNextEpisodesCommand = new RelayCommand(async o =>
           {
               if (string.IsNullOrWhiteSpace(_rssUrl)) return;
               var res = await MVM.podcastService.GetNextEpisodesAsync(_rssUrl, Index, MVM._episodesPerRender);
               if (res == null || res.Count == 0) return;
               AddEpisodes(res);
           });

        }

        public void SetPodcast(DTOpodcast podcast)
        {
            Title = podcast.Title;
            ImageUrl = podcast.ImageUrl;
            _rssUrl = podcast.RssUrl;

            Episodes.Clear();

            //Adds episodes and increases Index
            AddEpisodes(podcast.Episodes);

            Categories.Clear();
            foreach (var catg in podcast.Categories)
            {
                Categories.Add(catg);
            }
        }

        private void AddEpisodes(List<DTOepisode> episodes)
        {
            foreach (var ep in episodes)
            {
                if (ep.Description == "") ep.Description = "Episode has no description...";
                Episodes.Add(ep);
                Index++;
            }
        }

    }
}
