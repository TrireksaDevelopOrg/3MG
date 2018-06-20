using DataAccessLayer.DataModels;
using Ocph.DAL;
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
    /// Interaction logic for AddNewCustomerDeposit.xaml
    /// </summary>
    public partial class AddNewCustomerDeposit : Window
    {
        public AddNewCustomerDeposit()
        {
            InitializeComponent();
          
        }
    }

    public class AddNewCustomerDepositViewModel:customer,IDataErrorInfo
    {
        private string title;
        private string error;

        public string TitleCaption
        {
            get { return title; }
            set { SetProperty(ref title ,value); }
        }

      

        public CommandHandler CancelCommand { get; }
        public CommandHandler SaveCommand { get; }
        public Action WindowClose { get; internal set; }

        public AddNewCustomerDepositViewModel()
        {
            this.CustomerType = DataAccessLayer.CustomerType.Deposit;
            this.CreatedDate = DateTime.Now;
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = CancelCommandaction };
            SaveCommand = new CommandHandler { CanExecuteAction = SaveCommandValidate, ExecuteAction = SaveCommandAction };
        }

        public AddNewCustomerDepositViewModel(customer cust)
        {

            this.Id = cust.Id;
            this.Name = cust.Name;
            this.Address = cust.Address;
            this.ContactName = cust.ContactName;
            this.Email = cust.Email;
            this.Handphone = cust.Handphone;
            this.Phone1 = cust.Phone1;
            this.NoIdentitas = cust.NoIdentitas;
            this.TitleCaption = "Edit Customer";

            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = CancelCommandaction };
            SaveCommand = new CommandHandler { CanExecuteAction = SaveCommandValidate, ExecuteAction = SaveCommandAction };
        }

        private void SaveCommandAction(object obj)
        {
            DataAccessLayer.Bussines.CustomerBussiness context = new DataAccessLayer.Bussines.CustomerBussiness();
            try
            {
                var cust = (customer)this;
                context.AddNewCustomerDeposit(cust);
               Helpers.ShowMessage("Data Tersimpan");
                this.Saved = true;
                WindowClose();
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private bool SaveCommandValidate(object obj)
        {
            if (string.IsNullOrEmpty(Error))
                return true;
            return false;
        }

        private void CancelCommandaction(object obj)
        {
            WindowClose();
        }


        public string Error => error;

        public bool Saved { get; private set; }

        public string this[string columnName]
        {
            get
            {
                error = string.Empty;
                if (columnName == "Name")
                    error = string.IsNullOrEmpty(this.Name) ? "Nama Tidak Boleh Kosong" : null;
                if (columnName == "Address")
                    error = string.IsNullOrEmpty(this.Address) ? "Alamat Tidak Boleh Kosong" : null;

                return error;
            }
        }

    }
}
