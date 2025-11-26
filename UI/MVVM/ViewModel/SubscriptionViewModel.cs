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
    public class SubscriptionViewModel : ObservableObject
    {
        private MainViewModel MVM;
        public ObservableCollection<DTOsubscription> Podcasts { get; set; } = new();
        public RelayCommand PodcastClickCommand { get; }
        public RelayCommand CategoryCommand { get; }

        public ObservableCollection<DTOcategory> Categories { get; } = new();

        private DTOcategory _selectedCategory = null;
        public DTOcategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                CategoryCommand.Execute(value);
                OnPropertyChanged();
            }
        }


        public SubscriptionViewModel(MainViewModel MainVM)
        {
            MVM = MainVM;
            PodcastClickCommand = new RelayCommand(OnOpen);
            CategoryCommand = new RelayCommand(async o =>
            {
                await SortPodcasts();
            });
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

        public async Task SetCategories()
        {
            Categories.Clear();
            var categories = await Services.CategoryService.GetForUser(Storage.Email);

            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private async Task SortPodcasts()
        {
            var res = await Services.SubscriptionService.GetSubscribedPodcastsByCategory(Storage.Email, SelectedCategory.Id);
            SetPodcasts(res);
        }
    }
}
