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
        public PodcastViewModel(MainViewModel MVM)
        {
            this.MVM = MVM;
        }

        public void SetPodcast(DTOpodcast podcast)
        {
            Title = podcast.Title;

            Episodes.Clear();
            int i = 0;
            int max = 20;
            foreach (var ep in podcast.Episodes)
            {
                if (i > max) break;
                if (ep.Description == "") ep.Description = "Episode has no description...";
                Episodes.Add(ep);
                i++;
            }

            i = 0;
            Categories.Clear();
            foreach (var catg in podcast.Categories)
            {
                if (i > max) break;
                Categories.Add(catg);
                i++;
            }
        }

    }
}
