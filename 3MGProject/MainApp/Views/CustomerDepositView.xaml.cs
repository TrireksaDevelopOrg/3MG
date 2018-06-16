using DataAccessLayer.DataModels;
using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                try
                {
                    var datas = context.GetDepositsOfCustomer(value);
                    DepositSource.Clear();
                    foreach (var item in datas)
                    {
                        DepositSource.Add(item);
                    }
                    DepositViewSource.Refresh();
                }
                catch (Exception ex)
                {
                    Helpers.ShowErrorMessage(ex.Message);
                }
            }
        }

        public ObservableCollection<deposit> DepositSource { get; }
        public CollectionView DepositViewSource { get; }

        public CustomerDepositViewModel()
        {
            AddNewDepositCommand = new CommandHandler { CanExecuteAction = x => SelectedCustomer != null, ExecuteAction = AddNewDepositCommandAction };
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshCommandaction };
            AddNewCustomerCommand = new CommandHandler { CanExecuteAction = AddNewCustomerCommandValidate, ExecuteAction = AddNewCustomerCommandAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = CancelCommandaction };

            Source = new ObservableCollection<customer>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Refresh();

            DepositSource = new ObservableCollection<deposit>();
            DepositViewSource = (CollectionView)CollectionViewSource.GetDefaultView(DepositSource);
            DepositViewSource.Refresh();
            RefreshCommand.Execute(null);
        }

        private void AddNewDepositCommandAction(object obj)
        {
            var form = new AddNewDepositView();
            var viemodel = new AddNewDepositViewModel(SelectedCustomer) { WindowClose=form.Close};
            form.DataContext = viemodel;
            form.ShowDialog();
            if(viemodel.Saved)
            {
                DepositSource.Add((deposit)viemodel);
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
    }
}
