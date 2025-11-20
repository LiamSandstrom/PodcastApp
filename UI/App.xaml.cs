using System.Configuration;
using System.Data;
using System.Windows;
using UI.MVVM.View;
using UI.MVVM.ViewModel;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        LoginWindow loginWindow;
        LoginViewModel loginVM;
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            loginWindow = new LoginWindow();
            loginVM = new LoginViewModel();
            loginVM.LogInEvent += OnLogIn;

            loginWindow.DataContext = loginVM;
            loginWindow.Show();
        }

        private void OnLogIn(string email)
        {
            //need to send to backend and validate
            email = email.Trim();
            if (String.IsNullOrEmpty(email)) return;
            var mainVM = new MainViewModel(email);
            var mainWindow = new MainWindow(mainVM);

            mainWindow.Show();

            loginWindow.Close();
        }
    }

}
