using DataAccessLayer;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
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
    /// Interaction logic for AddNewDepositView.xaml
    /// </summary>
    public partial class AddNewDepositView : Window
    {
        public AddNewDepositView()
        {
            InitializeComponent();
            this.typeCombo.ItemsSource=  Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>();
        }
    }


    public class AddNewDepositViewModel:Deposit,IDataErrorInfo
    {
        private string error;
        public new string MyTitle { get; set; } = "TAMBAH DEPOSITE";
        public AddNewDepositViewModel(customer customer)
        {
            this.CreatedDate = DateTime.Now;
            Customer = customer;
            TanggalBayar = DateTime.Now;
            this.PaymentType = PaymentType.Transfer;
            this.CustomerId = customer.Id;
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidation, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
        }

        private void SaveAction(object obj)
        {
            var context = new DataAccessLayer.Bussines.DepositBussines();
            try
            {
                var dep=(Deposit)this;
                context.AddNewDeposit(dep);
                this.User = context.GetUser();
                Helpers.ShowMessage("Data Tersimpan");
                this.Saved = true;
                WindowClose();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }

        private bool SaveValidation(object obj)
        {
            if (string.IsNullOrEmpty(Error))
                return true;
            else
                return false;
        }

        public string Error => error;

        public customer Customer { get; set; }
        public bool Saved { get; private set; }
        public Action WindowClose { get; internal set; }

        public string this[string columnName]
        {
            get
            {
                error = string.Empty;
                if (columnName == "TanggalBayar")
                    error = TanggalBayar == new DateTime() ? "Tentukan Tanggal Bayar" : null;
                if (columnName == "Jumlah")
                    error = Jumlah <= 0 ? "Harus Lebih Besar Dari 0" : null;

            

                return error;
            }
        }
    }
}
