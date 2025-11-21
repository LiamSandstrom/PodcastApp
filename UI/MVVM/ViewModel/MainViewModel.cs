using BL;
using BL.Interfaces;
using DAL.MongoDB;
using DAL.MongoDB.Interfaces;
using DAL.Rss;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.Core;

namespace UI.MVVM.ViewModel
{

    public class MainViewModel : ObservableObject
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
        public readonly int _episodesPerRender = 20;
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



        public MainViewModel()
        {
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
                if (String.IsNullOrEmpty(SearchText)) return;
                if (SearchText.Equals(_lastSearched) && CurrentView == PodcastVM)
                {
                    RequestScrollToTop?.Invoke();
                    return;

                }
                var res = await Services.PodcastService.GetPodcastFromRssAsync(SearchText, _episodesPerRender);
                if (res == null) return;

                PodcastVM.Index = res.Episodes.Count;
                _lastSearched = SearchText;
                PodcastVM.SetPodcast(res);
                PodcastViewCommand.Execute(this);
                RequestScrollToTop?.Invoke();
            });
        }
    }
}
