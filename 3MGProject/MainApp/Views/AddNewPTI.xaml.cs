using DataAccessLayer;
using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
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
            var model = new collies {  PtiId=viewmodel.Id };
            e.NewItem = model;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void shiper_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter)
            {
                var shiper = ((TextBox)sender).Text;
                var forms = new Views.BrowserCustomerDeposit();
                forms.ShowDialog();
                var vm = (BrowseCustomerDepositViewModel)forms.DataContext;
                if(vm.SelectedCustomer!=null)
                {
                    viewmodel.Shiper = vm.SelectedCustomer;
                    if (vm.SelectedCustomer.CustomerType == CustomerType.Deposit)
                        viewmodel.PayType = PayType.Deposit;
                }
            }
     
        }
    }

    public class AddNewPTIViewModel:pti,IDataErrorInfo
    {
        PtiBussines context = new PtiBussines();
        private string error;

        public AddNewPTIViewModel()
        {
          
            this.CreatedDate = DateTime.Now;
            this.Collies= new ObservableCollection<collies>();
            this.SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Collies);
            this.SourceView.Refresh();
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction =x=>true, ExecuteAction =x=>WindowClose()};

            SetNew();
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
            
        }

        private bool SaveValidate(object obj)
        {
            if (string.IsNullOrEmpty(Error)&& Collies.Count>0)
            {
                return true;
            }
            else
                return false;
        }

        private void SaveAction(object obj)
        {
            try
            {
                pti model = (pti)this;
                context.SaveChange(model);
                Saved = true;
                WindowClose();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        public CollectionView SourceView { get; }
        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }
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
