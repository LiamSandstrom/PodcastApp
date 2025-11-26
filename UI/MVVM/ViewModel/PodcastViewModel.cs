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
using UI.MVVM.View;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UI.MVVM.ViewModel
{
    public class PodcastViewModel : ObservableObject
    {
        private Window? _popupRef;

        private string _rssUrl = "";

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
        private int _index = 0;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

        private string _imageUrl;

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }

        private bool _isLiked;

        public bool IsLiked
        {
            get { return _isLiked; }
            set
            {
                _isLiked = value;
                OnPropertyChanged();
            }
        }
        private bool _notEditing = true;
        public bool NotEditing
        {
            get => _notEditing;
            set
            {
                _notEditing = value;
                if (_notEditing == true)
                {
                    UpdateCustomName();
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DTOepisode> Episodes { get; set; } = new();
        public ObservableCollection<string> Categories { get; set; } = new();
        private List<string> realCategories = new();

        public readonly MainViewModel MVM;

        public RelayCommand AddCategoryCommand { get; }
        public RelayCommand GetNextEpisodesCommand { get; }
        public RelayCommand LikeButtonCommand { get; }
        public ICommand EditingCommand { get; }

        public PodcastViewModel(MainViewModel MVM)
        {
            this.MVM = MVM;

            GetNextEpisodesCommand = new RelayCommand(async o =>
           {
               if (string.IsNullOrWhiteSpace(_rssUrl)) return;
               var res = await Services.PodcastService.GetNextEpisodesAsync(_rssUrl, Index, MVM._episodesPerRender);
               if (res == null || res.Count == 0) return;
               AddEpisodes(res);
           });

            LikeButtonCommand = new RelayCommand(async o =>
            {
                if (!IsLiked) Like();
                else RemoveLike();

            });

            EditingCommand = new RelayCommand(_ => NotEditing = !NotEditing);

            AddCategoryCommand = new RelayCommand(async o =>
            {
                await OpenPopup();
            });

        }

        private async void Like()
        {
            IsLiked = !IsLiked;
            var res = await Services.SubscriptionService.SubscribeAsync(Storage.Email, _rssUrl, Title);
            if (res == false) IsLiked = !IsLiked;
        }

        private async void RemoveLike()
        {
            IsLiked = !IsLiked;
            var res = await Services.SubscriptionService.UnsubscribeAsync(Storage.Email, _rssUrl);
            if (res == false) IsLiked = !IsLiked;
            else
            {
                Categories.Clear();
                foreach (var category in realCategories)
                {
                    Categories.Add(category);
                }
            }


        }

        public async Task SetPodcast(DTOpodcast podcast)
        {
            Title = podcast.Title;
            ImageUrl = podcast.ImageUrl;
            _rssUrl = podcast.RssUrl;
            IsLiked = podcast.IsLiked;
            _popupRef = null;

            Episodes.Clear();

            //Adds episodes and increases Index
            AddEpisodes(podcast.Episodes);

            Categories.Clear();
            realCategories.Clear();
            foreach (var catg in podcast.Categories)
            {
                Categories.Add(catg);
                realCategories.Add(catg);
            }
            var subCategories = await Services.SubscriptionService.GetCategoriesOnSubscription(Storage.Email, _rssUrl);
            foreach (var category in subCategories)
            {
                Categories.Add(category);
            }
        }

        private void AddEpisodes(List<DTOepisode> episodes)
        {
            foreach (var ep in episodes)
            {
                if (ep.Description == "") ep.Description = "Episode has no description...";
                Episodes.Add(ep);
                Index++;
            }
        }

        private void UpdateCustomName()
        {
            Services.SubscriptionService.RenameSubscriptionAsync(Storage.Email, _rssUrl, Title);
        }
        public async Task OpenPopup()
        {
            if (!IsLiked)
            {
                MessageBox.Show("You have to subscribe first to be able to add custom categories!");
                return;
            }
            if (_popupRef != null) return;


            var popup = new AddCategoryWindow();
            var VM = new AddCategoryViewModel("hello world", ClosePopup, RemoveCategory);
            popup.DataContext = VM;
            await VM.SetCategories();

            popup.Show();
            _popupRef = popup;
        }

        public async Task ClosePopup(string id, string name)
        {
            if (string.IsNullOrEmpty(id))
            {
                _popupRef = null;
                return;
            }
            if (_popupRef == null) return;


            await Services.SubscriptionService.AddCategory(Storage.Email, _rssUrl, id);
            if (Categories.Contains(name))
            {
                MessageBox.Show("Podcast already has this category :)");
            }
            else
            {
                Categories.Add(name);
                _popupRef.Close();
                _popupRef = null;
            }


        }

        public async Task RemoveCategory(string id, string name)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            if (_popupRef == null) return;


            await Services.SubscriptionService.RemoveCategory(Storage.Email, _rssUrl, id);
            if (Categories.Contains(name))
            {
                Categories.Remove(name);
                _popupRef.Close();
                _popupRef = null;
            }
            else
            {
                MessageBox.Show("Podcast does not have this Category :(");
            }


        }

    }
}
