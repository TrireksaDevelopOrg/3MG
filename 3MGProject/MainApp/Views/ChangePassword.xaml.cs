using DataAccessLayer.Bussines;
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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private ChangePasswordViewModel vm;

        public ChangePassword()
        {
            InitializeComponent();
            vm = new ChangePasswordViewModel() { WindowClose=Close};
            this.DataContext = vm;
        }

        private void oldpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            vm.OldPassword = passwordBox.Password;
        }

        private void newpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            vm.NewPassword = passwordBox.Password;

        }

        private void confirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            vm.ConfirmPassword = passwordBox.Password;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vm.Save();
        }
    }

    public class ChangePasswordViewModel:BaseNotify
    {
        public ChangePasswordViewModel()
        {
            this.Loaded();
            MyTitle = "UBAH PASSWORD";
        }

        private void Loaded()
        {
            UserName = Authorization.User.UserName;
        }

        public void Save()
        {

            try
            {
                if (string.IsNullOrEmpty(OldPassword))
                {
                    Helpers.ShowErrorMessage("Masukkan Password Sebelumnya");
                    return;
                }

                if (string.IsNullOrEmpty(NewPassword))
                {
                    Helpers.ShowErrorMessage("Masukkan Password Baru");
                    return;
                }
                if (string.IsNullOrEmpty(NewPassword))
                {
                    Helpers.ShowErrorMessage("Confirm Password Baru");
                    return;
                }


                if (OldPassword != Authorization.User.Password)
                {
                    Helpers.ShowErrorMessage("Password Sebelumnya Salah");
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    Helpers.ShowErrorMessage("Password Baru dan Confirm Password Tidak sama ");
                    return;
                }

                UserManagement userManagement = new UserManagement();
                if (userManagement.ChangePassword(Authorization.User, NewPassword))
                {
                    Helpers.ShowMessage("Password Anda Berhasil Diubah");
                    WindowClose();
                 }
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);

            }

        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName ,value); }
        }

        private string oldPassword;

        public string OldPassword
        {
            get { return oldPassword; }
            set { oldPassword = value; }
        }


        private string newPassword;

        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value; }
        }


        private string confirmPassword;

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { confirmPassword = value; }
        }

        public Action WindowClose { get; internal set; }
    }
}
