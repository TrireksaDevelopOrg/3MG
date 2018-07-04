using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for SMUView.xaml
    /// </summary>
    public partial class SMUView : Window
    {
        private SMUViewModel vm;

        public SMUView()
        {
            InitializeComponent();
            dg.PreviewKeyDown += Dg_PreviewKeyDown;
          
        }
        private void Dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                vm = (SMUViewModel)this.DataContext;
                DataGrid dg = (DataGrid)sender;
                var item = (SMU)dg.SelectedItem;
                if (item != null)
                    vm.ShowDetails(item);
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            detailsDg.IsReadOnly = true;
            SMUDetail selectedRow = e.Row.DataContext as SMUDetail;
            if (e.EditAction == DataGridEditAction.Commit)
            {
               
            }

          
            // vm.SelectedPTI.DetailView.Refresh();

            //  detailsDg.Items.Refresh();

        }

        private void detailsDg_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == DataGrid.DeleteCommand)
            {
                if (vm.UserCanaccess())
                {
                    DataGrid grid = (DataGrid)sender;
                    if (MyMessage.Show("Yakin Menghapus Data ?", "Confirm Delete", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        e.Handled = false;
                    else
                    {
                        vm.RemoveItemCollies(grid.SelectedItem as SMUDetail);
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => Helpers.ShowErrorMessage("Anda Tidak Memiliki Akses")),
                        System.Windows.Threading.DispatcherPriority.Normal);
                }
              
            }
        }

        private void dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            vm = (SMUViewModel)this.DataContext;
            DataGrid dg = (DataGrid)sender;
            var item = (SMU)dg.SelectedItem;
            if (item != null)
                vm.ShowDetails(item);
        }
    }

    public class SMUViewModel : Authorization
    {
        public Action WindowClose { get; internal set; }
        public CommandHandler CancelCommand { get; }
        public CommandHandler PrintSMUCommand { get; set; }
        public CommandHandler OutManifestCommand { get; }
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
        private GridLength myHorizontalInputRegionSize = new GridLength(0, GridUnitType.Pixel);

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


        public SMUViewModel():base(typeof(SMUViewModel))
        {
            MyTitle = "SURAT MUATAN UDARA";
            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshAction };
            PrintSMUCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = PrintCommandAction };
            OutManifestCommand = new CommandHandler { CanExecuteAction =x=>true, ExecuteAction = OutManifestAction };

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



      [Authorize("Manager")]
        private void OutManifestAction(object obj)
        {
            if(User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                try
                {

                    if (SelectedItem.IsSended)
                    {
                        var isTakeOff =context.ManifestIsTakeOf(SelectedItem);
                        if (isTakeOff)
                            throw new SystemException("Pesawat Telah Berangkat, Data Tidak Dapat Diubah");
                    }

                    var form = new Views.BrowseColliesView();
                    var vm = new Views.BrowseColliesViewModel(SelectedItem) { WindowClose=form.Close};
                    form.DataContext = vm;
                    form.ShowDialog();

                    if (vm.Success)
                    {
                        foreach (var item in vm.Source.Where(O => O.IsSended).ToList())
                            SelectedItem.Details.Add(new SMUDetail
                            {
                                ColliesId = item.Id,
                                Content = item.Content,
                                Kemasan = item.Kemasan,
                                Pcs = item.Pcs,
                                Weight = item.Weight,
                                Price = item.Price,
                                PTIId = item.PtiId
                            });
                    }
                }
                catch (Exception ex)
                {

                    Helpers.ShowErrorMessage(ex.Message);
                }
            }


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
            Helpers.PrintWithFormActionTwoSource("Print Preview",dataHeader,items,"MainApp.Reports.Layouts.SuratMuatanUdara.rdlc",null );
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
                Helpers.ShowMessage(string.Format("T{0:D9} Dibatalkan", SelectedItem.Id));
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        internal async void ShowDetails(SMU selectedSMU)
        {
            var res = await context.GetSMUDetail(selectedSMU.Id);
            selected.Details.Clear();

            foreach (var item in res)
            {
                selected.Details.Add(item);
            }
            GridWidth = new GridLength(30, GridUnitType.Star);
        }

        [Authorize("Manager")]
        internal bool UserCanaccess()
        {
            return User.CanAccess(MethodBase.GetCurrentMethod());
        }

        internal void RemoveItemCollies(SMUDetail sMUDetail)
        {
            try
            {

                if(SelectedItem!=null && SelectedItem.IsSended)
                {
                    Helpers.ShowMessage("SMU Telah Terdaftar di Manifes, Hapus SMU dari Manifest Sebelum di Edit");
                }else
                {
                    if (context.RemoveSMUItem(sMUDetail))
                    {
                        SelectedItem.Details.Remove(sMUDetail);
                        Helpers.ShowMessage("Item SMU Terhapus");
                    }
                }

          
                    
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        public SMU SelectedItem {
            get { return selected;}
            set { SetProperty(ref selected, value);
                GridWidth = new GridLength(0, GridUnitType.Pixel);
            }
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

      

        public GridLength GridWidth
        {
            get
            {
                // If not yet set, get the starting value from the DataModel
                return myHorizontalInputRegionSize;
            }
            set
            {
                SetProperty(ref myHorizontalInputRegionSize, value);
            }
        }

    }
}
