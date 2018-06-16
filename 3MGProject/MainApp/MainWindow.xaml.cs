using DataAccessLayer.Bussines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewmodel;
        public MainWindow()
        {
            var loginSuccess = false;
            var closeApp = false;
            while(!loginSuccess)
            {
                UserManagement userManagement = new UserManagement();
                if (userManagement.IsFirstUser())
                {
                    var regForm = new Views.Registration();
                    regForm.ShowDialog();
                    var regVM = (Views.RegistrationViewModel)regForm.DataContext;
                    if (regVM.UserCreated!=null)
                    {
                        Helpers.UserLogin = regVM.UserCreated;
                       if(!userManagement.IsRoleExist("Administrator").Result)
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

                        loginSuccess = true;
                    }
                    else
                    {
                        var result =MessageBox.Show("Yakin Menutup Aplikasi ?", "Menutup Aplikasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(result== MessageBoxResult.Yes)
                        {
                            closeApp = true;
                            loginSuccess = true;
                        }

                    }
                }
                else
                {
                    var loginForm = new Views.LoginView();
                    loginForm.ShowDialog();
                    var vm = (Views.LoginViewModel)loginForm.DataContext;
                    if(vm.Success)
                    {
                        Helpers.UserLogin = vm.UserLogin;
                        loginSuccess = true;
                    }else
                    {

                        var result = MessageBox.Show("Yakin Menutup Aplikasi ?", "Menutup Aplikasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            closeApp = true;
                            loginSuccess = true;
                        }
                    }


                }
            }

            if (closeApp)
                this.Close();

            InitializeComponent();
            viewmodel = new MainWindowViewModel();
            this.DataContext = viewmodel;
        }

        private void pti_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadPTI();
        }

        private void customer_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadCustomerDeposit();
        }

        private void smu_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadSMU();
        }


       
        private void userManage_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadUserManagement();
        }

        private void laporan_Click(object sender, RoutedEventArgs e)
        {

        }

        private void manifest_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadManifest();
        }

        private void schedule_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadShcedule();
        }
    }


    public class MainWindowViewModel:Authorization
    {
        public MainWindowViewModel() : base(typeof(MainWindowViewModel)) { }

        [Authorize("Administrator,Manager,Accounting")]
        internal void LoadCustomerDeposit()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var form = new Views.CustomerDepositView();
                form.ShowDialog();
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);
         
        }


        [Authorize("Administrator,Operational")]
        internal void LoadManifest()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var form = new Views.ManivestView();
                form.ShowDialog();
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);
        }

        [Authorize("Administrator,Operational,Admin")]
        internal void LoadPTI()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {

                var form = new Views.PTIView();
                var viewmodel = new Views.PTIViewModel() { WindowClose = form.Close };
                form.DataContext = viewmodel;
                form.ShowDialog();
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);

        }

        [Authorize("Administrator,Admin,Manager")]
        internal void LoadShcedule()
        {
            var form = new Views.FlightView();
            form.ShowDialog();
        }

        [Authorize("Administrator,Admin,Manager")]
        internal void LoadSMU()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var form = new Views.SMUView();
                var viewmodel = new Views.SMUViewModel() { WindowClose = form.Close };
                form.DataContext = viewmodel;
                form.ShowDialog();
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);
        }

        [Authorize("Administrator,Manager")]
        internal void LoadUserManagement()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var form = new Views.UserManagementView();
                form.ShowDialog();
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);
        }
    }
}
