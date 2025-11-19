using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Core;
using UI.MVVM.Model;
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
        public ObservableCollection<Episode> Episodes { get; set; } = new();
        public ObservableCollection<string> Categories { get; set; } = new();

        public PodcastViewModel()
        {
            Title = "Test Podcast";
            Categories.Add("History");
            Categories.Add("News");
            Categories.Add("Gaming");
            AsyncStuff();
        }
        public async void AsyncStuff()
        {
            await AddEpisodes(10, 2000);

        }
        public async Task AddEpisodes(int amount, int delay)
        {
            await Task.Delay(delay);
            for (int i = amount; i > 0; i--)
            {
                Episodes.Add(new Episode
                {
                    Title = "#" + i + " - " + "Episode",
                    Description = "Test description. Here is text that text is text which can be text.",
                    EpisodeNumber = i,
                    DateAndDuration = "19 nov - 1h 48min"
                });
            }
        }
    }

}
