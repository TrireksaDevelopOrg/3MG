using DataAccessLayer;
using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        RegistrationViewModel vm;

        public Registration()
        {
            InitializeComponent();
            vm = new RegistrationViewModel { WindowClose = Close };
            cmbGender.ItemsSource = Enum.GetValues(typeof(Gender)).Cast<Gender>();
            this.DataContext = vm;
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            vm.Password = ((PasswordBox)sender).Password;
        }
    }

    public class RegistrationViewModel : user, IDataErrorInfo
    {
        private string error;
        private UserManagement userManager= new UserManagement();

        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }

        public RegistrationViewModel()
        {
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
        }

        private async void SaveAction(object obj)
        {
            try
            {

                var model = (user)this;
                var success = await userManager.CreateNewUser(model);
                if(success)
                {
                    this.UserCreated = model;
                    Helpers.ShowMessage("User Berhasil Di Buat");
                    WindowClose();
                }
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private bool SaveValidate(object obj)
        {
            if (string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(UserName)
                && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Password))
                return true;
            return false;
        }




        public string this[string columnName] {
            get
            {
                error = string.Empty;
        
                if (columnName == "Name")
                    error = string.IsNullOrEmpty(this.Name) ? "Nama Kelamin Tidak Boleh Kosong" : null;

                if (columnName == "UserName")
                    error = string.IsNullOrEmpty(this.UserName) ? "Jenis Kelamin Tidak Boleh Kosong" : null;

                if (columnName == "Password")
                    error = string.IsNullOrEmpty(this.Password) ? "Jenis Kelamin Tidak Boleh Kosong" : null;
                 if (columnName == "Address")
                    error = string.IsNullOrEmpty(this.Address) ? "Alamat Tidak Boleh Kosong" : null;

                return error;
            }
        }

        public string Error => error;

        public user UserCreated { get; private set; }
        public Action WindowClose { get; internal set; }
    }
}
