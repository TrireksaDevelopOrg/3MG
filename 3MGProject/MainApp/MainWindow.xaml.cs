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
            

            InitializeComponent();
            viewmodel = new MainWindowViewModel() { WindowParent = this };
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
            var form = new Reports.Forms.PenjualanForm();
            form.ShowDialog();
        }

        private void manifest_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadManifest();
        }

        private void schedule_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadShcedule();
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Views.LoginView();
            login.Show();
            this.Close();
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
