using BL.Interfaces;
using DAL.MongoDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UI.Core;
using BL;
using DAL.Rss;
using System.Windows.Controls;

namespace UI.MVVM.ViewModel
{

    class MainViewModel : ObservableObject
    {
        public HomeViewModel HomeVM { get; set; }
        public CategoriesViewModel CategoriesVM { get; set; }
        public SubscriptionViewModel SubscriptionVM { get; set; }
        public PodcastViewModel PodcastVM { get; set; }

        private object _currentView;

        public RelayCommand HomeViewCommand { get; }
        public RelayCommand CategoriesViewCommand { get; }
        public RelayCommand SubscriptionViewCommand { get; }
        public RelayCommand PodcastViewCommand { get; }
        public RelayCommand SearchCommand { get; }

        public event Action RequestScrollToTop;

        private string _searchText;
        private string _lastSearched = "";
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        private readonly IPodcastService _podcastService;

        public MainViewModel()
        {

            _podcastService = new PodcastService(new RssRepository());
            HomeVM = new HomeViewModel();
            CategoriesVM = new CategoriesViewModel();
            SubscriptionVM = new SubscriptionViewModel();
            PodcastVM = new PodcastViewModel(this);
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });

            CategoriesViewCommand = new RelayCommand(o =>
            {
                CurrentView = CategoriesVM;
            });

            SubscriptionViewCommand = new RelayCommand(o =>
            {
                CurrentView = SubscriptionVM;
            });

            PodcastViewCommand = new RelayCommand(o =>
            {
                CurrentView = PodcastVM;
            });


            SearchCommand = new RelayCommand(async o =>
            {
                if (SearchText.Equals(_lastSearched))
                {
                    RequestScrollToTop?.Invoke();
                    return;

                }
                var res = await _podcastService.GetPodcastFromRssAsync(SearchText);
                if (res == null) return;

                _lastSearched = SearchText;
                PodcastVM.SetPodcast(res);
                PodcastViewCommand.Execute(this);
                RequestScrollToTop?.Invoke();
            });
        }

    }
}
