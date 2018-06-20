using DataAccessLayer.DataModels;
using Ocph.DAL;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DataAccessLayer.Models;
using Microsoft.Reporting.WinForms;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for CustomerDepositView.xaml
    /// </summary>
    public partial class CustomerDepositView : Window
    {
        CustomerDepositViewModel viewmodel;
        public CustomerDepositView()
        {
            InitializeComponent();
            viewmodel = new CustomerDepositViewModel { WindowClose = this.Close };
            this.DataContext = viewmodel;
        }

    }


    public class CustomerDepositViewModel : BaseNotify
    {
        DataAccessLayer.Bussines.CustomerBussiness context = new DataAccessLayer.Bussines.CustomerBussiness();

        public Action WindowClose { get; internal set; }
        public ObservableCollection<customer> Source { get; }
        public CollectionView SourceView { get; }

        public CommandHandler PrintRekening { get; }
        public CommandHandler AddNewCustomerCommand { get; }
        public CommandHandler CancelCommand { get; }
        public CommandHandler AddNewDepositCommand { get; }
        public CommandHandler RefreshCommand { get; }

        private customer _customer;

        public customer SelectedCustomer
        {
            get { return _customer; }
            set {
                SetProperty(ref _customer ,value);
                if(value!=null)
                {
                    LoadDataSelectedCustomer();
                }
         
            }
        }

        private async void LoadDataSelectedCustomer()
        {
            try
            {
                await Task.Delay(200);
                var datas = await context.GetDepositsOfCustomer(SelectedCustomer);
                DepositSource.Clear();
                foreach (var item in datas)
                {
                    DepositSource.Add(item);
                }
                DepositViewSource.Refresh();
                DebetDepositGetData(SelectedCustomer.Id);
                SelectedCustomer.SisaSaldo = await context.GetSisaSaldo(SelectedCustomer.Id);
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private async void DebetDepositGetData(int id)
        {
           var result= await context.GetDebetDeposit(id);
            DebetDepositSource.Clear();
            foreach(var item in result)
            {
                DebetDepositSource.Add(item);
            }

            DebetDepositViewSource.Refresh();
        }

       


        public ObservableCollection<Deposit> DepositSource { get; }
        public CollectionView DepositViewSource { get; }
        public ObservableCollection<DebetDeposit> DebetDepositSource { get; }
        public CollectionView DebetDepositViewSource { get; }

        public CustomerDepositViewModel()
        {
            PrintRekening = new CommandHandler { CanExecuteAction = x => SelectedCustomer != null && SelectedDate<=DateTime.Now, ExecuteAction = PrintRekeningKoran };
            AddNewDepositCommand = new CommandHandler { CanExecuteAction = x => SelectedCustomer != null, ExecuteAction = AddNewDepositCommandAction };
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshCommandaction };
            AddNewCustomerCommand = new CommandHandler { CanExecuteAction = AddNewCustomerCommandValidate, ExecuteAction = AddNewCustomerCommandAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = CancelCommandaction };

            Source = new ObservableCollection<customer>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Refresh();

            DepositSource = new ObservableCollection<Deposit>();
            DepositViewSource = (CollectionView)CollectionViewSource.GetDefaultView(DepositSource);

            DebetDepositSource = new ObservableCollection<DebetDeposit>();
            DebetDepositViewSource = (CollectionView)CollectionViewSource.GetDefaultView(DebetDepositSource);

            DepositViewSource.Refresh();
            RefreshCommand.Execute(null);
            SelectedCustomer = null;
            SelectedDate = DateTime.Now;
        }

       

        private void AddNewDepositCommandAction(object obj)
        {
            var form = new AddNewDepositView();
            var viemodel = new AddNewDepositViewModel(SelectedCustomer) { WindowClose=form.Close};
            form.DataContext = viemodel;
            form.ShowDialog();
            if(viemodel.Saved)
            {
                DepositSource.Add((Deposit)viemodel);
            }
            DepositViewSource.Refresh();
        }

        private async void RefreshCommandaction(object obj)
        {
            try
            {
                var data = context.GetCustomersDeposites();
                Source.Clear();
                foreach (var item in data)
                {
                    Source.Add(item);
                }
                SourceView.Refresh();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
                await Task.Delay(200);
                WindowClose();
            }
        }

        private void CancelCommandaction(object obj)
        {
            WindowClose();
        }

        private bool AddNewCustomerCommandValidate(object obj)
        {
            return true;
        }

        private void AddNewCustomerCommandAction(object obj)
        {
            var form = new Views.AddNewCustomerDeposit();
            var viewmodel = new AddNewCustomerDepositViewModel() { WindowClose=form.Close};
            form.DataContext = viewmodel;
            form.ShowDialog();

            if(viewmodel.Saved)
            {
                customer cust = (customer)viewmodel;
                Source.Add(cust);
                SourceView.Refresh();
            }

        }

       
        public async void PrintRekeningKoran(object obj)
        {
           var result = await  context.GetRekeningKorang(SelectedDate,SelectedCustomer.Id);
            ReportParameter[] parameters =
                 {
                       new ReportParameter("User",context.GetUser()),
                    new ReportParameter("Nomor",string.Format("{0:D6}",SelectedCustomer.Id)),
                    new ReportParameter("Shiper",SelectedCustomer.Name.ToString()),
                    new ReportParameter("Address",string.Format("{0}\r\n Hanphone :{1}",SelectedCustomer.Address,SelectedCustomer.Handphone)),
                    new ReportParameter("Tanggal",DateTime.Now.ToShortDateString())


                };
           double saldo = 0;

            foreach(var item in result)
            {
                saldo = saldo + item.Kredit - item.Total+ item.SaldoAkhir;
                item.SaldoAkhir = saldo;

            }

            result.Add(new Saldo { Tanggal = DateTime.Now, SaldoAkhir = saldo, Description = "Saldo Akhir" });
            var source = new ReportDataSource { Value = result };
            Helpers.PrintPreviewWithFormAction(source, "MainApp.Reports.Layouts.Rekening1.rdlc", parameters);
        }
        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { SetProperty(ref selectedDate ,value); }
        }

    }
}
