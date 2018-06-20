using DataAccessLayer.Bussines;
using DataAccessLayer.Models;
using Microsoft.Reporting.WinForms;
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
    /// Interaction logic for SMUView.xaml
    /// </summary>
    public partial class SMUView : Window
    {
        public SMUView()
        {
            InitializeComponent();
        }
    }

    public class SMUViewModel : BaseNotify
    {
        public Action WindowClose { get; internal set; }
        public CommandHandler CancelCommand { get; }
        public CommandHandler PrintSMUCommand { get; set; }
        public CommandHandler PrintPreviewSMUCommand { get; }
        public CommandHandler CancelSMU { get; }
        public ObservableCollection<SMU> Source { get; }
        public CollectionView SourceView { get; }

        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                SetProperty(ref startDate, value);
                if (StartDate < EndDate)
                    LoadData(StartDate, EndDate);
            }
        }

        private async void LoadData(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                context = new DataAccessLayer.Bussines.SMUBussines();
                List<SMU> datas = await context.GetSMU(startDate, endDate);
                Source.Clear();
                foreach (var item in datas)
                {
                    Source.Add(item);
                }
                SourceView.Refresh();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private DateTime endDate;

        public CommandHandler RefreshCommand { get; }

        private SMUBussines context;
        private SMU selected;
        private bool _showCancel;
        private bool _showAktif;

        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                SetProperty(ref endDate, value);
                if (StartDate < EndDate)
                    LoadData(StartDate, EndDate);
            }
        }


        public SMUViewModel()
        {
            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshAction };
            PrintSMUCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = PrintCommandAction };
            PrintPreviewSMUCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = PrintPreviewActionAsync };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
            CancelSMU = new CommandHandler { CanExecuteAction = CancelSMUValidate, ExecuteAction = CancelSMUAction };
            Source = new ObservableCollection<SMU>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Filter = DataFilter;
            SourceView.Refresh();
            LoadData(StartDate, EndDate);
            this.Aktif = true; this.Batal = false;
        }

        private void PrintCommandAction(object obj)
        {
            throw new NotImplementedException();
        }

        private async void PrintPreviewActionAsync(object obj)
        {
           List<SuratMuatanUdara> header = await context.GetSMUHeader(SelectedItem.Id);
            var dataHeader = new ReportDataSource { Name = "Header", Value = header };

            List<SMUDetail> datas = await context.GetSMUDetail(SelectedItem.Id);

            var items = new ReportDataSource { Name = "Details", Value = datas};
            Helpers.PrintWithFormActionTwoSource(dataHeader,items,"MainApp.Reports.Layouts.SuratMuatanUdara.rdlc",null );
        }

        private void RefreshAction(object obj)
        {
            LoadData(StartDate, EndDate);
        }

        private bool DataFilter(object obj)
        {
            var data = (SMU)obj;
            if (data != null && data.ActiveStatus == DataAccessLayer.ActivedStatus.OK && this.Aktif)
                return true;
            if (data != null && data.ActiveStatus == DataAccessLayer.ActivedStatus.Cancel && this.Batal)
                return true;

            return false;
        }

        private bool CancelSMUValidate(object obj)
        {
            if (context.IsInRole("Manager").Result && SelectedItem!=null)
                return true;
            else
                return false;
        }

        private void CancelSMUAction(object obj)
        {
            try
            {
                if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                    Helpers.ShowErrorMessage("Tambahkan Alasan Pembatalan");
                context.SetCancelSMU(SelectedItem, obj.ToString());
                MessageBox.Show(string.Format("{0} Dibatalkan", SelectedItem.Code));
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }


        public SMU SelectedItem {
            get { return selected;}
            set { SetProperty(ref selected, value); }
        }

        public bool Aktif {
            get { return _showAktif; }
            set { SetProperty(ref _showAktif, value);
                SourceView.Refresh();
            }
        }

        public bool Batal
        {
            get { return _showCancel; }
            set { SetProperty(ref _showCancel, value);
                SourceView.Refresh();
            }
        }

        
    }
}
