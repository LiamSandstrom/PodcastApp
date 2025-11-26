using BL.DTOmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Core;
using ZstdSharp.Unsafe;

namespace UI.MVVM.ViewModel
{
    class AddCategoryViewModel : ObservableObject
    {
        private string _url;
        private Func<string, string, Task> _cb;
        public RelayCommand AddCategoryCommand { get; }
        public RelayCommand RemoveCategoryCommand { get; }
        public RelayCommand CloseCommand { get; }
        public ObservableCollection<DTOcategory> Categories { get; } = new();

        private DTOcategory _selectedCategory = null;
        public DTOcategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }


        public AddCategoryViewModel(string url, Func<string, string, Task> addCb, Func<string, string, Task> removeCb)
        {
            _url = url;
            _cb = addCb;

            AddCategoryCommand = new RelayCommand(async o =>
            {
                if (SelectedCategory == null) return;
                await _cb(SelectedCategory.Id, SelectedCategory.Name);
            });

            CloseCommand = new RelayCommand(async o =>
            {
                await _cb("", "");
            });

            RemoveCategoryCommand = new RelayCommand(async o =>
            {
                if (SelectedCategory == null) return;
                await removeCb(SelectedCategory.Id, SelectedCategory.Name);
            });

        }
        public async Task SetCategories()
        {
            Categories.Clear();
            var categories = await Services.CategoryService.GetUserOnly(Storage.Email);

            foreach (var category in categories)
            {

                {
                    Categories.Add(category);

                }
            }
        }
    }
}
