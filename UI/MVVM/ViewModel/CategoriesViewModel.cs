using BL.DTOmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Core;

namespace UI.MVVM.ViewModel
{
    public class CategoriesViewModel : ObservableObject
    {
        public RelayCommand AddCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand RenameCommand { get; }

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

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        private DTOcategory _selectedCategoryRename = null;
        public DTOcategory SelectedCategoryRename
        {
            get => _selectedCategoryRename;
            set
            {
                _selectedCategoryRename = value;
                OnPropertyChanged();
            }
        }
        private string _newName = "";
        public string NewName
        {
            get => _newName;
            set
            {
                _newName = value;
                OnPropertyChanged();
            }
        }

        public CategoriesViewModel()
        {
            AddCommand = new RelayCommand(async o =>
            {
                if (String.IsNullOrWhiteSpace(Name)) return;
                var res = await Services.CategoryService.CreateCategory(Name, Storage.Email);
                if (res == null)
                {
                    MessageBox.Show("Failed to add category...");
                }
                else
                {
                    MessageBox.Show("Successfully Added category!");
                    await SetCategories();
                    Name = "";
                }

            });

            RemoveCommand = new RelayCommand(async o =>
            {
                if (SelectedCategory == null) return;
                var res = await Services.CategoryService.DeleteCategory(SelectedCategory.Id, Storage.Email);
                if (res != null)
                {
                    await SetCategories();
                    SelectedCategory = null;
                    MessageBox.Show("Successfully Deleted category!");
                }
                else
                {
                    MessageBox.Show("Failed to Delete category...");
                }
            });

            RenameCommand = new RelayCommand(async o =>
            {
                if (String.IsNullOrWhiteSpace(NewName)) return;
                if (SelectedCategoryRename == null) return;
                var res = await Services.CategoryService.RenameCategory(SelectedCategoryRename.Id, NewName, Storage.Email);
                if (res == true)
                {
                    await SetCategories();
                    NewName = "";
                    MessageBox.Show("Successfully Renamed category!");
                }
                else
                {
                    MessageBox.Show("Failed to Rename category...");
                }
            });
        }

        public async Task SetCategories()
        {
            Categories.Clear();
            var categories = await Services.CategoryService.GetUserOnly(Storage.Email);

            foreach (var category in categories)
            {
                Categories.Add(category);

            }
        }
    }
}
