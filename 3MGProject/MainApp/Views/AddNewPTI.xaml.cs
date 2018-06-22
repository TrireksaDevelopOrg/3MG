using DataAccessLayer;
using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AddNewPTI.xaml
    /// </summary>
    public partial class AddNewPTI : Window
    {
        AddNewPTIViewModel viewmodel;
        public AddNewPTI()
        {
            InitializeComponent();
            viewmodel = new AddNewPTIViewModel() { WindowClose=Close};
            this.DataContext = viewmodel;
            comboPayType.ItemsSource = Enum.GetValues(typeof(PayType)).Cast<PayType>();

        }



        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            var model = new collies { PtiId = viewmodel.Id, Kemasan="Pcs", Pcs=1 };
            e.NewItem = model;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void shiper_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter)
            {
                //var shiper = ((TextBox)sender).Text;
                var forms = new Views.BrowserCustomerDeposit();
                forms.ShowDialog();
                var vm = (BrowseCustomerDepositViewModel)forms.DataContext;
                if(vm.SelectedCustomer!=null)
                {
                    viewmodel.Shiper = vm.SelectedCustomer;
                    if (vm.SelectedCustomer.CustomerType == CustomerType.Deposit)
                    {
                        viewmodel.ShiperID = vm.SelectedCustomer.Id;
                        viewmodel.PayTypeSelected= PayType.Deposit;
                        reciver.Focus();
                        shiper.IsEnabled = false;
                    }
                }
            }
            else if(viewmodel.ShiperID>0)
            {
                viewmodel.Shiper = new customer();
                viewmodel.ShiperID = 0;
                viewmodel.PayTypeSelected = PayType.Chash;
            }
     
        }

        private void reciver_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewmodel.Shiper = new customer();
            viewmodel.ShiperID = 0;
            viewmodel.PayTypeSelected = PayType.Chash;
            shiper.IsEnabled = true;
            shiper.Focus();
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
          
        }
    }

    public class Kemasan:List<string>
    {
        public Kemasan()
        {
            this.Add("Pcs");
            this.Add("Karung");
            this.Add("Box");
            this.Add("Drum");
        }
    }

    public class AddNewPTIViewModel:pti,IDataErrorInfo,IBusyBase
    {
        PtiBussines context = new PtiBussines();
        private string error;
        public new string MyTitle { get; set; } = "PTI BARU";
        public AddNewPTIViewModel()
        {

            this.CreatedDate = DateTime.Now;
            this.Collies = new ObservableCollection<collies>();
            this.SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Collies);
            this.SourceView.Refresh();
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
            PrintPreviewCommand = new CommandHandler { CanExecuteAction = PrintPreviewValidate, ExecuteAction = PrintPreviewAction };
            AddNewCityCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewCityAction };
            Cities = new ObservableCollection<city>();
            CitiesSourceView = (CollectionView)CollectionViewSource.GetDefaultView(Cities);
            LoadDataCities();
            SetNew();
        }

        private async void LoadDataCities()
        {
            var scheduleBussines = new ScheduleBussines();
            var result =await scheduleBussines.GetCities();
            var fromid = Helpers.GetIntValue("CityId");
            if (result!=null)
            {
                foreach(var item in result.Where(O=>O.Id!=fromid).ToList())
                {
                    Cities.Add(item);
                }
            }

            if(Cities.Count==1)
            {
                var city = Cities.FirstOrDefault();
                this.ToId = city.Id;
            }
        }

        private void AddNewCityAction(object obj)
        {
            var form = new Views.AddNewCity();
            form.ShowDialog();
            var vm = (AddNewCityViewModel)form.DataContext;
            if(vm.SaveSuccess)
            {
                Cities.Add(vm.SavedResult);
            }
            CitiesSourceView.Refresh();
        }

        private bool PrintPreviewValidate(object obj)
        {
            if (string.IsNullOrEmpty(Shiper.Name) || string.IsNullOrEmpty(Reciever.Name))
                return false;
            return true;
        }

        private void PrintPreviewAction(object obj)
        {

            Helpers.PrintPreviewWithFormAction("Print Preview",new ReportDataSource { Value = Collies.ToList() }, "MainApp.Reports.Layouts.PTI.rdlc", null);

        }

        private async void SetNew()
        {
            PTINumber = await CodeGenerate.GetNewPTINumber();
            Code = CodeGenerate.PTI(PTINumber);
            Shiper = new customer();
            Reciever = new customer();
            this.ActiveStatus = ActivedStatus.OK;
            this.CreatedDate = DateTime.Now;
            this.Etc = 0;
            this.Note = string.Empty;
            this.PayType = PayType.Chash;
            this.RecieverId = 0;
            this.ShiperID = 0;
            this.FromId = Helpers.GetIntValue("CityId");
        }


        private PayType payTypeSelected;

        public PayType PayTypeSelected
        {
            get { return payTypeSelected; }
            set {
                if (ShiperID <= 0 && value == PayType.Deposit)
                {
                    Helpers.ShowErrorMessage("Pengirim Bukan Customer Deposit");
                    SetBackToCash();
                }
                else
                {
                    SetProperty(ref payTypeSelected, value);
                    PayType = value;
                }
      




            }
        }
        public async void SetBackToCash()
        {
            await Task.Delay(100);
            PayTypeSelected = PayType.Chash;
        }


        private bool SaveValidate(object obj)
        {
            if (this.ToId<=0 || string.IsNullOrEmpty(Shiper.Name) || string.IsNullOrEmpty(Reciever.Name) || Collies.Count<=0 || (ShiperID<=0 && PayTypeSelected== PayType.Deposit))
            {
                return false;
            }
            else
                return true;
        }

        private void SaveAction(object obj)
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                pti model = (pti)this;
                model.FromId = Helpers.GetIntValue("CityId");
                if (IsListValid(model.Collies))
                {
                    context.SaveChange(model);
                    Saved = true;

                    var resultDialog = MessageBox.Show("Akan Mencetak ?", "Print", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resultDialog == MessageBoxResult.Yes)
                    {
                        using (var print = new HelperPrint())
                        {
                            ReportParameter[] parameters =
                            {
                        new ReportParameter("Petugas",context.GetUser()),
                        new ReportParameter("Nomor",model.Code),
                        new ReportParameter("Pengirim",Shiper.Name),
                        new ReportParameter("AlamatPengirim",string.Format("{0}\r Hanphone :{1}",Shiper.Address,Shiper.Handphone)),
                        new ReportParameter("Penerima",Reciever.Name),
                        new ReportParameter("AlamatPenerima",string.Format("{0}\r Hanphone :{1}",Reciever.Address,Reciever.Handphone)),
                        new ReportParameter("Tanggal",model.CreatedDate.ToShortDateString())
                        };

                            print.PrintDocument(model.Collies.ToList(), "MainApp.Reports.Layouts.PTI.rdlc", parameters);
                        }
                    }


                    WindowClose();
                }
                else
                    Helpers.ShowErrorMessage("Item Data Tidak Lengkap, Periksa Kembali");
       
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }finally
            {
                IsBusy = false;
            }
        }

        private bool IsListValid(ObservableCollection<collies> collies)
        {
            if (collies.Where(O => string.IsNullOrEmpty(O.Kemasan) || string.IsNullOrEmpty(O.Content)).Count() > 0)
                return false;
            if (collies.Where(O => O.Pcs == 0 || O.Weight == 0 || O.Price == 0).Count() > 0)
                return false;
            return true;
        }

        public CollectionView SourceView { get; }
        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }
        public CommandHandler PrintPreviewCommand { get; }
        public CommandHandler AddNewCityCommand { get; }
        public ObservableCollection<city> Cities { get; }
        public CollectionView CitiesSourceView { get; }
        public bool IsNew { get; private set; }

        public string Error => error;

        public bool Saved { get; internal set; }
        public Action WindowClose { get; internal set; }

        public string this[string columnName]
        {
            get
            {

                error = string.Empty;
                if (columnName == "ShiperId")
                    error = ShiperID <= 0 ? "" : null;

                return error;
            }
        }
    }

}
