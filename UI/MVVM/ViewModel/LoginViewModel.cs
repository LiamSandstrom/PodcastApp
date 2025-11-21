using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Core;

namespace UI.MVVM.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        private string _email = "";
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand LoginCommand { get; }
        public Action<string> LogInEvent;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async o =>
            {
                var res = await Services.UserService.CreateUserAsync(Email);
                if (res == false)
                {
                    MessageBox.Show("Invalid Email...");
                    return;
                }
                LogInEvent?.Invoke(Email);
            });
        }
    }
}
