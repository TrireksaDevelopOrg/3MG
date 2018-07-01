using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DataAccessLayer;
using DataAccessLayer.Bussines;
using Microsoft.Reporting.WinForms;
using DataAccessLayer.DataModels;
using System.Reflection;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for PTIView.xaml
    /// </summary>
    public partial class PTIView : Window
    {
        private PTIViewModel vm;

        public PTIView()
        {
            InitializeComponent();
            dg.PreviewKeyDown += Dg_PreviewKeyDown;
        }

        private void Dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           if(e.Key== Key.Enter || e.Key== Key.Return)
            {
               vm = (PTIViewModel)DataContext;
                DataGrid dg = (DataGrid)sender;
                var item = (PTI)dg.SelectedItem;
                if (item != null)
                    vm.ShowDetails(item);
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            detailsDg.IsReadOnly = true;
            collies selectedRow = e.Row.DataContext as collies;
            if (e.EditAction== DataGridEditAction.Commit)
            {
               // detailsDg.CommitEdit( DataGridEditingUnit.Row, false);
               if(selectedRow!=null)
                vm.UpdateItemCollies(selectedRow);
            }

            if(e.Row.IsNewItem)
            {
                vm.AddNewCollies(selectedRow);
            }
   
           // vm.SelectedPTI.DetailView.Refresh();
     
            //  detailsDg.Items.Refresh();
           
        }

        private void detailsDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var colly = e.Row.DataContext as collies;
            if(colly!=null)
            {
                var a = e.Column.Header.ToString();
                if (a == "Pcs")
                {
                    var control = (TextBox)e.EditingElement;
                    var item = Convert.ToInt32(control.Text);
                    colly.Pcs = item;
                }

                if (a == "Kemasan")
                {
                    var control = (TextBox)e.EditingElement;
                    colly.Kemasan = control.Text;
                }
                if (a == "Content")
                {
                    var control = (TextBox)e.EditingElement;
                    colly.Content = control.Text;
                }

                if (a == "Berat")
                {
                    var control = (TextBox)e.EditingElement;
                    var item = Convert.ToDouble(control.Text);

                    colly.Weight = item;
                }

                if (a == "Harga")
                {
                    var control = (TextBox)e.EditingElement;
                    var item = Convert.ToDouble(control.Text);
                    colly.Price = item;
                }
            }
           
        }

        private  void detailsDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (vm != null)
                if (vm.UserCanaccess())
                {
                    if (detailsDg.IsReadOnly)
                    {
                        detailsDg.IsReadOnly = false;
                        detailsDg.CanUserDeleteRows = true;
                        detailsDg.CanUserAddRows = true;
                    }
                }else
                {
                    Helpers.ShowErrorMessage("Anda Tidak Memiliki Akses");
                }
        }

        private  void detailsDg_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (e.Command == DataGrid.DeleteCommand)
            {
                if (vm.UserCanaccess())
                {
                    if (MyMessage.Show("Yakin Menghapus Data ?", "Confirm Delete", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        e.Handled = false;
                    else
                    {
                        vm.RemoveItemCollies(grid.SelectedItem as collies);
                    }
                }else
                {
                    
                    Dispatcher.BeginInvoke(new Action(() => Helpers.ShowErrorMessage("Anda Tidak Memiliki Akses")),
                        System.Windows.Threading.DispatcherPriority.Normal);

                }
            }
         }
    }

    public class PTIViewModel : Authorization,IBusyBase
    {
        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate ,value);
                if (StartDate < EndDate)
                    LoadData(StartDate, EndDate);
            }
        }

        private DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { SetProperty(ref  endDate ,value);
                if (StartDate < EndDate)
                    LoadData(StartDate, EndDate);
            }
        }


        private PTI selectedPTI;

        public PTI SelectedPTI
        {
            get { return selectedPTI; }
            set { SetProperty(ref selectedPTI ,value);
                GridWidth = new GridLength(0, GridUnitType.Pixel);
            }
        }


        public ObservableCollection<PTI> Source { get; }
        public CollectionView SourceView { get; }
        public Action WindowClose { get; internal set; }
        public CommandHandler AddNewPTICommand { get; }
        public CommandHandler CancelCommand { get; }
        public CommandHandler CreateSMU { get; }
        public CommandHandler RefreshCommand { get; }
        public CommandHandler PrintPreviewhCommand { get; }
        public CommandHandler CancelPTI { get; }
        public CommandHandler PrintCommand { get; }

        DataAccessLayer.Bussines.PtiBussines ptiContext = new DataAccessLayer.Bussines.PtiBussines();

        public PTIViewModel():base(typeof(PTIViewModel))
        {
            MyTitle = "PEMBERITAHUAN TENTANG ISI ( P T I )";
            CancelPTI = new CommandHandler { CanExecuteAction = CancelPTIValidate, ExecuteAction = CancelPTIAction };
            PrintCommand = new CommandHandler { CanExecuteAction = x => SelectedPTI != null, ExecuteAction = PrintCommandAction };
            PrintPreviewhCommand = new CommandHandler { CanExecuteAction = x => SelectedPTI != null, ExecuteAction = PrintPreviewAction };
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshAction };
            AddNewPTICommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewPTIAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction =x=>WindowClose()};
            CreateSMU = new CommandHandler { CanExecuteAction = CreateSMUValidate, ExecuteAction = CreateSMUAction };
            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1,date.Hour,date.Minute,date.Second);
            endDate = startDate.AddMonths(1).AddDays(-1);
            Source = new ObservableCollection<PTI>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Filter = DataFilter;
            Aktif = true;
            SourceView.Refresh();
            RefreshCommand.Execute(null);
        }

        private bool DataFilter(object obj)
        {
            var data = (PTI)obj;
            if (data != null && data.ActiveStatus == DataAccessLayer.ActivedStatus.OK && this.Aktif)
                return true;
            if (data != null && data.ActiveStatus == DataAccessLayer.ActivedStatus.Cancel && this.Batal)
                return true;

            return false;
        }


        private bool CancelPTIValidate(object obj)
        {
            if (ptiContext.IsInRole("Manager").Result)
                return true;
            else
                return false;
        }

        private void CancelPTIAction(object obj)
        {
            try
            {
                if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                    Helpers.ShowErrorMessage("Tambahkan Alasan Pembatalan");
                else if(SelectedPTI.ActiveStatus== ActivedStatus.Cancel)
                    Helpers.ShowErrorMessage(string.Format("PTI NO {0:D6} Telah Berstatus Batal", SelectedPTI.Id));
                else
                {
                    ptiContext.SetCancelPTI(SelectedPTI, obj.ToString());
                    Helpers.ShowMessage(string.Format("PTI NO {0:d6} Berhasil Dibatalkan", SelectedPTI.Id));
                }
  
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private async void PrintPreviewAction(object obj)
        {
           if(SelectedPTI.Details.Count<=0)
            {
                var res = await ptiContext.GetDetailOfPTI(SelectedPTI);
                foreach (var item in res)
                {
                    SelectedPTI.Details.Add(item);
                }

                var header = new ReportDataSource() {
                    Value = new List<PTI> { SelectedPTI },
                    Name="Header"
                };
                var source = new ReportDataSource() { Name="Detail", Value = SelectedPTI.Details };

                Helpers.PrintWithFormActionTwoSource("Print Preview",header,source, "MainApp.Reports.Layouts.PTI.rdlc", null);
            }
        }

        private async void PrintCommandAction(object obj)
        {
            using (var print = new HelperPrint())
            {
                if (SelectedPTI.Details.Count <= 0)
                {
                    var res = await ptiContext.GetDetailOfPTI(SelectedPTI);
                    foreach (var item in res)
                    {
                        SelectedPTI.Details.Add(item);
                    }

                    ReportParameter[] parameters =
                    {
                        new ReportParameter("Petugas",SelectedPTI.User),
                        new ReportParameter("Nomor",SelectedPTI.Id.ToString()),
                        new ReportParameter("Pengirim",SelectedPTI.ShiperName),
                        new ReportParameter("AlamatPengirim",string.Format("{0}\r Hanphone :{1}",SelectedPTI.RecieverAddress,SelectedPTI.RecieverHandphone)),
                        new ReportParameter("Penerima",SelectedPTI.RecieverName),
                        new ReportParameter("AlamatPenerima",string.Format("{0}\r Hanphone :{1}",SelectedPTI.RecieverAddress,SelectedPTI.RecieverHandphone)),
                        new ReportParameter("Tanggal",SelectedPTI.CreatedDate.ToShortDateString())};

                    print.PrintDocument(SelectedPTI.Details.ToList(), "MainApp.Reports.Layouts.PTI.rdlc", parameters);
                }
            }
        }

        public async void ShowDetails(PTI selected)
        {
            var res = await ptiContext.GetDetailOfPTI(selected);
            selected.Details.Clear();
           
            foreach (var item in res)
            {
                selected.Details.Add(item);
            }

            if (selected.DetailView == null)
                selected.DetailView = (CollectionView)CollectionViewSource.GetDefaultView(selected.Details);

            GridWidth = new GridLength( 30, GridUnitType.Star);
        }

        private void RefreshAction(object obj)
        {
            LoadData(StartDate, EndDate);
        }

        private bool CreateSMUValidate(object obj)
        {
            if (SelectedPTI != null)
                return true;
            return false;
        }

        private void CreateSMUAction(object obj)
        {
            var form = new Views.AddNewSMU();
            var vm = new Views.AddNewSMUViewModel(SelectedPTI) { WindowClose = form.Close };
            form.DataContext = vm;
            form.ShowDialog();

        }

        private void AddNewPTIAction(object obj)
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                var form = new Views.AddNewPTI();
                Helpers.ShowChild(WindowParent, form);
        
                var vm = (AddNewPTIViewModel)form.DataContext;
                if (vm.Saved)
                {
                    PTI p = new PTI
                    {
                         ShiperID=vm.ShiperID, RecieverId=vm.RecieverId,
                        CreatedDate = vm.CreatedDate,
                        Id = vm.Id,
                        PayType = vm.PayType,
                        Pcs = vm.Collies.Sum(O => O.Pcs),
                        Weight = vm.Collies.Sum(O => O.Weight),
                        RecieverName = vm.Reciever.Name,
                        ShiperName = vm.Shiper.Name,
                        Biaya = vm.Collies.Sum(O => O.Biaya),
                        User = Authorization.User.Name
                    };
                    Source.Add(p);
                }
                SourceView.Refresh();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }finally
            {
                IsBusy = false;
            }
            
        }

        private async void LoadData(DateTime startDate, DateTime endDate)
        {

            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                var result = await ptiContext.GetPTIFromTo(StartDate, EndDate);
                await Task.Delay(500);
                Source.Clear();
                foreach (var item in result)
                {
                    Source.Add(item);
                }
                SourceView.Refresh();
            }
            catch (Exception ex)
            {

                Helpers.ShowMessage(ex.Message);
            }finally
            {
                IsBusy = false;
            }
        }
        
      
        internal collies UpdateItemCollies(collies colly)
        {
            try
            {
                return ptiContext.UpdateCollies(colly);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Authorize("Manager")]
        internal bool UserCanaccess()
        {
            if(User.CanAccess(MethodBase.GetCurrentMethod()))
            {
                if (MyMessage.Show("Yakin Mengubah Data ?", "Confirm Edit", MessageBoxButton.OKCancel)== MessageBoxResult.OK)
                {
                    return true;
                }
            }
            return false;
        }

        internal void AddNewCollies(collies selectedRow)
        {
            try
            {
                if (selectedRow.Pcs > 0 && !string.IsNullOrEmpty(selectedRow.Content) &&
                selectedRow.Weight > 0 && !string.IsNullOrEmpty(selectedRow.Kemasan) &&
                selectedRow.Price > 0)
                {
                    selectedRow.PtiId = SelectedPTI.Id;
                    ptiContext.AddNewCollyItem(selectedRow);
                    SelectedPTI.Details.Add(selectedRow);
                }
                else
                    throw new SystemException("Lengkapi Data dengan Benar");
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
               

        }

        internal void RemoveItemCollies(collies collies)
        {
            if(collies!=null)
            {
                try
                {
                    if(ptiContext.RemoveItemCollies(collies))
                    {
                        SelectedPTI.Details.Remove(collies);
                    }
                }
                catch (Exception ex)
                {

                    Helpers.ShowErrorMessage(ex.Message);
                }
            }
        }

        private GridLength myHorizontalInputRegionSize = new GridLength(0, GridUnitType.Pixel);
        private bool _showAktif;
        private bool _showCancel;

        public GridLength GridWidth
        {
            get
            {
                // If not yet set, get the starting value from the DataModel
                return myHorizontalInputRegionSize;
            }
            set
            {
                SetProperty(ref myHorizontalInputRegionSize,value);
            }
        }

        public Window WindowParent { get; internal set; }

        public bool Aktif
        {
            get { return _showAktif; }
            set
            {
                SetProperty(ref _showAktif, value);
                SourceView.Refresh();
            }
        }

        public bool Batal
        {
            get { return _showCancel; }
            set
            {
                SetProperty(ref _showCancel, value);
                SourceView.Refresh();
            }
        }
    }
}
