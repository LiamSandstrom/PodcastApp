using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UI.Core;

namespace UI.MVVM.ViewModel
{

    class MainViewModel : ObservableObject
    {
        public HomeViewModel HomeVM { get; set; }
        public CategoriesViewModel CategoriesVM { get; set; }
        public SubscriptionViewModel SubscriptionVM { get; set; }

        private object _currentView;

        public RelayCommand HomeViewCommand { get; }
        public RelayCommand CategoriesViewCommand { get; }
        public RelayCommand SubscriptionViewCommand { get; }

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
        }

    }
}
