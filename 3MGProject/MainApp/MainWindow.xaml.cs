using DataAccessLayer.Bussines;
using FoxLearn.License;
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

            if(IsRegistration())
            {
                while (!loginSuccess)
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



                            loginSuccess = true;
                        }
                        else
                        {
                            var result = MessageBox.Show("Yakin Menutup Aplikasi ?", "Menutup Aplikasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
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
                        if (vm.Success)
                        {
                            Helpers.UserLogin = vm.UserLogin;
                            loginSuccess = true;
                        }
                        else
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
            }else
                closeApp = true;

            if (closeApp)
                this.Close();

            InitializeComponent();
            viewmodel = new MainWindowViewModel() { WindowParent = this };
            this.DataContext = viewmodel;
        }

        private bool IsRegistration()
        {

            string ProductId = ComputerInfo.GetComputerId();
            KeyManager key = new KeyManager(ProductId);

            LicenseInfo lic = new LicenseInfo();
            try
            {
                key.LoadSuretyFile(string.Format("Key.lic"), ref lic);
                if(lic.ProductKey==null)
                {
                    var form = new Views.RegistrationProduct();
                    form.ShowDialog();
                    if (form.Success)
                    {
                        return true;
                    }

                }else
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

        public Window WindowParent { get; set; }

        [Authorize("Administrator,Manager,Accounting")]
        internal void LoadCustomerDeposit()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var form = new Views.CustomerDepositView();
                Helpers.ShowChild(WindowParent, form);
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
                Helpers.ShowChild(WindowParent, form);
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
                var viewmodel = new Views.PTIViewModel() { WindowParent=form, WindowClose = form.Close };
                form.DataContext = viewmodel;
                Helpers.ShowChild(WindowParent, form);
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);

        }

        [Authorize("Administrator,Admin,Manager")]
        internal void LoadShcedule()
        {
            var form = new Views.FlightView();
            Helpers.ShowChild(WindowParent, form);
        }

        [Authorize("Administrator,Admin,Manager")]
        internal void LoadSMU()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var form = new Views.SMUView();
                var viewmodel = new Views.SMUViewModel() { WindowClose = form.Close };
                form.DataContext = viewmodel;
                Helpers.ShowChild(WindowParent, form);
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
                Helpers.ShowChild(WindowParent, form);
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);
        }
    }
}
