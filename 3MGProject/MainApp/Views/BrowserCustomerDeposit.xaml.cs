using DataAccessLayer.Bussines;
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
    /// Interaction logic for BrowserCustomerDeposit.xaml
    /// </summary>
    public partial class BrowserCustomerDeposit : Window
    {
        BrowseCustomerDepositViewModel viewmodel;

        public BrowserCustomerDeposit()
        {
            InitializeComponent();
            viewmodel = new BrowseCustomerDepositViewModel() { WindowClose = this.Close };
            this.DataContext = viewmodel;
        }

        private void DataGrid_TouchEnter(object sender, TouchEventArgs e)
        {

        }
    }


    public class BrowseCustomerDepositViewModel:BaseNotify
    {
        private CustomerBussiness context = new CustomerBussiness();

        public ObservableCollection<customer> Source { get; }
        public CollectionView SourceView { get; }
        public CommandHandler SelectedCommand { get; }
        public CommandHandler CancelCommand { get; }
        private customer _customer;

        public customer SelectedCustomer
        {
            get { return _customer; }
            set { SetProperty(ref _customer ,value); }
        }

        public Action WindowClose { get; internal set; }

        public BrowseCustomerDepositViewModel()
        {
            var data = context.GetCustomersDeposites();
            this.Source = new ObservableCollection<customer>(data);
            this.SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Refresh();
            SelectedCommand = new CommandHandler { CanExecuteAction = SelectedCommandValidate, ExecuteAction = SelectedCommandAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = CancelCommandaction };
        }

        private void CancelCommandaction(object obj)
        {
            SelectedCustomer = null;
            WindowClose();
        }

        private bool SelectedCommandValidate(object obj)
        {
            if (SelectedCustomer != null)
                return true;
            return false;
        }

        private void SelectedCommandAction(object obj)
        {
            WindowClose();
        }
    }
}
