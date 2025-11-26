using BL.DTOmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Core;

namespace UI.MVVM.ViewModel
{

    public class HomeViewModel
    {
        private MainViewModel MVM;
        public ObservableCollection<DTOsubscription> Podcasts { get; set; } = new();
        public RelayCommand PodcastClickCommand { get; }

        public HomeViewModel(MainViewModel mvm)
        {
            MVM = mvm;
            PodcastClickCommand = new RelayCommand(OnOpen);
        }


        public async Task SetPodcasts()
        {
            Podcasts.Clear();
            var res = await Services.SubscriptionService.GetMostPopular();
            foreach (var pod in res)
            {
                Podcasts.Add(pod);
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
