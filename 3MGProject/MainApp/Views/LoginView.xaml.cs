using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using Ocph.DAL;
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
using System.Windows.Shapes;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private LoginViewModel viewmodel;
        public LoginView()
        {
            InitializeComponent();
            viewmodel = new LoginViewModel { WindowClose = this.Close };
            this.DataContext = viewmodel;
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewmodel.Password = ((PasswordBox)sender).Password;
        }
    }

    public class LoginViewModel:BaseNotify
    {
        private UserManagement userManager = new UserManagement();
        public Action WindowClose { get; internal set; }

        private string userName;
        private string password;

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName ,value); }
        }

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        public CommandHandler LoginCommand { get; }
        public CommandHandler CancelCommand { get; }
        public bool Success { get; private set; }
        public user UserLogin { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new CommandHandler{CanExecuteAction =LoginValidate,  ExecuteAction = LoginAction};
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
        }

        private bool LoginValidate(object obj)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                return true;
            else
                return false;
        }

        private async void LoginAction(object obj)
        {
            var userLogin = await userManager.Login(UserName, Password);
            if (userLogin != null)
            {
                this.Success = true;
                this.UserLogin = userLogin;
                WindowClose();
            }else
            {
                Helpers.ShowErrorMessage("User atau Password Anda Salah");
            }
        }
    }
}
