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
    class PodcastViewModel : ObservableObject
    {
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
        public ObservableCollection<DTOepisode> Episodes { get; set; } = new();
        public ObservableCollection<string> Categories { get; set; } = new();

        public readonly MainViewModel MVM;

        public RelayCommand GetNextEpisodes { get; }

        public PodcastViewModel(MainViewModel MVM)
        {
            this.MVM = MVM;

            GetNextEpisodes = new RelayCommand(o =>
           {

           });

        }

        public void SetPodcast(DTOpodcast podcast)
        {
            Title = podcast.Title;

            Episodes.Clear();
            foreach (var ep in podcast.Episodes)
            {
                if (ep.Description == "") ep.Description = "Episode has no description...";
                Episodes.Add(ep);
            }

            Categories.Clear();
            foreach (var catg in podcast.Categories)
            {
                Categories.Add(catg);
            }
        }

        public void SetNextEpisodes(List<DTOepisode> nextEpisodes)
        {
            foreach (var ep in nextEpisodes)
            {
                if (ep.Description == "") ep.Description = "Episode has no description...";
                Episodes.Add(ep);
            }
        }
    }
}
