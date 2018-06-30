using DataAccessLayer.Bussines;
using FoxLearn.License;
using MainApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            viewmodel = new MainWindowViewModel() {MyTitle="PT. Tri M.G Airlines", WindowClose=Close, WindowParent = this };
            this.DataContext = viewmodel;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewmodel.LoadDashboard();
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
            viewmodel.LoadLaporan();
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
            viewmodel.Logout();
        }

        private void password_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.ChangePassword();
            form.ShowDialog();

        }

        private void backup_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.Backup();
            form.ShowDialog();
        }

        private void penjualan_Click(object sender, RoutedEventArgs e)
        {

        }
    }


    public class MainWindowViewModel : Authorization
    {
        public MainWindowViewModel() : base(typeof(MainWindowViewModel)) {
            Dashboards = new ObservableCollection<DashboarTile>();
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => LoadDashboard() };

        }

        public Window WindowParent { get; set; }
        public Action WindowClose { get; internal set; }
        public ObservableCollection<DashboarTile> Dashboards { get; }
        public CommandHandler RefreshCommand { get; }

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


        DashboardBussines dashboardContext = new DataAccessLayer.Bussines.DashboardBussines();
        LaporanBussines laporan = new LaporanBussines();
        internal async void LoadDashboard()
        {
            Dashboards.Clear();
            var data2 = await laporan.GetDataBufferStock();
            Dashboards.Add(new DashboarTile
            {
                TitleTile = "BUFFER STOK",
                Nilai = string.Format("{0:N2} Ton", data2.Sum(O => O.TotalWeight) / 1000),
                BackgroundColor = GenerateColorGradient("#EA38D8")
            });

            ScheduleBussines schedule = new ScheduleBussines();
            var schedules = await schedule.GetSchedules(DateTime.Now);
            var data5 = schedules.Where(O => O.Start >= DateTime.Now.TimeOfDay).FirstOrDefault();
            if (data5 != null)
            {
                Dashboards.Add(new DashboarTile
                {
                    TitleTile = "NEXT SCHEDULE",
                    Nilai = string.Format("{0} - {1}", data5.FlightNumber, data5.Start.ToString()),
                    BackgroundColor = GenerateColorGradient("#D4F339")
                });

            } else
            {
                Dashboards.Add(new DashboarTile
                {
                    TitleTile = "NEXT SCHEDULE",
                    Nilai = "",
                    BackgroundColor = GenerateColorGradient("#D4F339")
                });
            }


            AddPenjualan();
            AddSisaSaldo();

            AddBorderelCargo();
        }



        [Authorize("Manager,Accounting")]
        public  void AddSisaSaldo()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {

                CustomerBussiness custs = new CustomerBussiness();
                var result = custs.GetCustomersDeposites();

                double saldo = 0;

                foreach (var item in result)
                {
                    saldo += custs.GetSisaSaldo(item.Id).Result;
                }


                Dashboards.Add(new DashboarTile
                {
                    TitleTile = "Sisa Saldo Deposite Customer",
                    Nilai = string.Format("Rp.{0:N2}", saldo),
                    BackgroundColor = GenerateColorGradient("#BF018A")
                });
            }
        }



        [Authorize("Manager, Accounting")]
        public void AddPenjualan()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var date = DateTime.Now;
                var newDate = new DateTime(date.Year, date.Month, 1);
                var newLast = newDate.AddMonths(1).AddDays(-1);

                var data = laporan.GetDataLaporan(newDate, newLast).Result;
                Dashboards.Add(new DashboarTile
                {
                    TitleTile = "Penjualan Bulan Ini",
                    Nilai = string.Format("Rp.{0:N2}", data.Sum(O => O.Total)),
                    BackgroundColor = GenerateColorGradient("#7FF90B0B")
                });

                newDate = newDate.AddMonths(-1);
                newLast = newLast.AddMonths(-1);
                var data1 = laporan.GetDataLaporan(newDate, newLast).Result;
                Dashboards.Add(new DashboarTile
                {
                    TitleTile = "Penjualan Bulan Lalu",
                    Nilai = string.Format("Rp.{0:N2}", data1.Sum(O => O.Total)),
                    BackgroundColor = GenerateColorGradient("#6AEA38")
                });
            }

        }

        [Authorize("Administrator")]
        public void AddBorderelCargo()
        {
          if(User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                var data3 = laporan.GetDataBorderelCargo(DateTime.Now).Result;
                Dashboards.Add(new DashboarTile
                {
                    TitleTile = "BORDEREL CARGO HARI INI",
                    Nilai = string.Format("Rp. {0:N2}", data3.Sum(O => (O.TotalWeight * O.Price) + (O.TotalWeight * O.Price * 0.1))),
                    BackgroundColor = GenerateColorGradient("#F3A239")
                });
            }

        }

        private LinearGradientBrush GenerateColorGradient(string co)
        {


            Color color1 = (Color)ColorConverter.ConvertFromString(co);
            color1.A = 100;
            Color color2 = (Color)ColorConverter.ConvertFromString("#190F75BB");
            var gra  = new GradientStopCollection { new GradientStop(color1, 0), new GradientStop(color2, 1) };
            var col = new LinearGradientBrush(gra,45);


              //< LinearGradientBrush EndPoint = "0.5,1" StartPoint = "0.5,0" >


              //         < GradientStop Color = "#7FF90B0B" Offset = "0" />


              //            < GradientStop Color = "#190F75BB" Offset = "1" />


              //           </ LinearGradientBrush >
            return col;
        }

        [Authorize("Administrator,Accounting,Manager")]
        internal void LoadLaporan()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                var form = new Reports.Forms.Laporan();
                Helpers.ShowChild(WindowParent, form);
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
            finally { IsBusy = false; }
        }

        [Authorize("Administrator,Operational,Manager")]
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

        [Authorize("Administrator,Admin")]
        internal void LoadPTI()
        {
            if (User.CanAccess(MethodBase.GetCurrentMethod()))
            {

                var form = new Views.PTIView();
                var viewmodel = new Views.PTIViewModel() { WindowParent = form, WindowClose = form.Close };
                form.DataContext = viewmodel;
                Helpers.ShowChild(WindowParent, form);
            }
            else
                Helpers.ShowErrorMessage(NotHaveAccess);

        }

        [Authorize("Administrator,Operational,Manager")]
        internal void LoadShcedule()
        {
            var form = new Views.FlightView();
            Helpers.ShowChild(WindowParent, form);
        }

        [Authorize("Administrator,Accounting,Manager")]
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

        internal void Logout()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                var login = new Views.LoginView();
                login.Show();
                this.WindowClose();
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
            finally { IsBusy = false; }

        }
    }
}
