using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.MVVM.ViewModel;

namespace UI.MVVM.View
{
    /// <summary>
    /// Interaction logic for PodcastView.xaml
    /// </summary>
    public partial class PodcastView : UserControl
    {
        public PodcastView()
        {
            InitializeComponent();

            Loaded += PodcastView_Loaded;
        }

        private void PodcastView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is PodcastViewModel vm)
            {
                vm.MVM.RequestScrollToTop += OnScrollRequested;
            }
        }
        private void OnScrollRequested()
        {
            MyScrollViewer.ScrollToTop();
        }

        //For infinite Scroll
        private void MyScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;

            if (sv.VerticalOffset >= sv.ScrollableHeight)
            {
                // Bottom reached
                if (DataContext is PodcastViewModel vm)
                {
                    vm.GetNextEpisodesCommand.Execute(this);
                }
            }

        }
        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length;
        }
    }
}
