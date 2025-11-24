using BL.DTOmodels;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UI.Core;

namespace UI.MVVM.ViewModel
{
    public class SubscriptionViewModel
    {
        private MainViewModel MVM;
        public ObservableCollection<DTOsubscription> Podcasts { get; set; } = new();
        public RelayCommand PodcastClickCommand { get; }

        public SubscriptionViewModel(MainViewModel MainVM)
        {
            MVM = MainVM;
            PodcastClickCommand = new RelayCommand(OnOpen);
        }

        public void SetPodcasts(List<DTOsubscription> podcasts)
        {
            Podcasts.Clear();
            foreach (var podcast in podcasts)
            {
                Podcasts.Add(podcast);
            }
        }

        private async void OnOpen(object parameter)
        {
            var url = parameter as string;
            if (url == null) return;

            await MVM.Search(url);
        }
    }
}
