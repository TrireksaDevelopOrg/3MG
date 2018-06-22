using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using FoxLearn.License;
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
            Start();
            InitializeComponent();

            viewmodel = new LoginViewModel { WindowClose = this.Close };
            this.DataContext = viewmodel;
        }

        private void Start()
        {
            if (IsRegistration())
            {
                UserManagement userManagement = new UserManagement();
                if (userManagement.IsFirstUser())
                {
                    var regForm = new Views.Registration();
                    regForm.ShowDialog();
                    var regVM = (Views.RegistrationViewModel)regForm.DataContext;
                    if (regVM.UserCreated != null)
                    {
                        Helpers.UserLogin = regVM.UserCreated;
                        if (!userManagement.IsRoleExist("Administrator").Result)
                        {
                            userManagement.AddNewRole("Administrator");
                        }

                        if (!userManagement.IsRoleExist("Manager").Result)
                        {
                            userManagement.AddNewRole("Manager");
                        }

                        if (!userManagement.IsRoleExist("Admin").Result)
                        {
                            userManagement.AddNewRole("Admin");
                        }

                        if (!userManagement.IsRoleExist("Operational").Result)
                        {
                            userManagement.AddNewRole("Operational");
                        }

                        if (!userManagement.IsRoleExist("Accounting").Result)
                        {
                            userManagement.AddNewRole("Accounting");
                        }

                        userManagement.AddUserInRole(Helpers.UserLogin.Id, "Administrator");

                        var setting = new Views.Setting();
                        setting.ShowDialog();

                    }
                    
                }
            }
            else
            {
                this.Close();
            }
           
        }

        private bool IsRegistration()
        {

            string ProductId = ComputerInfo.GetComputerId();
            KeyManager key = new KeyManager(ProductId);

            LicenseInfo lic = new LicenseInfo();
            try
            {
                key.LoadSuretyFile(string.Format("Key.lic"), ref lic);
                if (lic.ProductKey == null)
                {
                    var form = new Views.RegistrationProduct();
                    form.ShowDialog();
                    if (form.Success)
                    {
                        return true;
                    }

                }
                else
                {
                    string productKey = lic.ProductKey;
                    if (key.ValidKey(ref productKey))
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            return false;

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
            MyTitle = "Login";
            LoginCommand = new CommandHandler{CanExecuteAction =LoginValidate,  ExecuteAction = LoginAction};
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction =CloseApp };
        }

        private void CloseApp(object obj)
        {
            var result = MessageBox.Show("Yakin Menutup Aplikasi ?", "Menutup Aplikasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.WindowClose();
            }
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
                var main = new MainWindow();
                main.Show();
                WindowClose();
            }else
            {
                Helpers.ShowErrorMessage("User atau Password Anda Salah");
            }
        }
    }
}
