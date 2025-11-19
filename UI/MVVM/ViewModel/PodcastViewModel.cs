using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Core;
using UI.MVVM.Model;

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
        public ObservableCollection<Episode> Episodes { get; set; } = new();

        public PodcastViewModel()
        {
            Title = "Test Podcast";
            for (int i = 10; i > 0; i--)
            {
                Episodes.Add(new Episode
                {
                    Title = "#" + i + "-" + "Episode",
                    Description = "Test description. Here is text that text is text which can be text.",
                    EpisodeNumber = i,
                });
            }
        }
    }
}
