using DataAccessLayer.Models;
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
using DataAccessLayer;
using DataAccessLayer.Bussines;
using Microsoft.Reporting.WinForms;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for PTIView.xaml
    /// </summary>
    public partial class PTIView : Window
    {
        public PTIView()
        {
            InitializeComponent();
            dg.PreviewKeyDown += Dg_PreviewKeyDown;
        }

        private void Dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           if(e.Key== Key.Enter || e.Key== Key.Return)
            {
                PTIViewModel vm = (PTIViewModel)DataContext;
                DataGrid dg = (DataGrid)sender;
                var item = (PTI)dg.SelectedItem;
                if (item != null)
                    vm.ShowDetails(item);
            }
        }

      
    }

    public class PTIViewModel : BaseNotify,IBusyBase
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

        public PTIViewModel()
        {
            CancelPTI = new CommandHandler { CanExecuteAction = CancelPTIValidate, ExecuteAction = CancelPTIAction };
            PrintCommand = new CommandHandler { CanExecuteAction = x => SelectedPTI != null, ExecuteAction = PrintCommandAction };
            PrintPreviewhCommand = new CommandHandler { CanExecuteAction = x => SelectedPTI != null, ExecuteAction = PrintPreviewAction };
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshAction };
            AddNewPTICommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewPTIAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction =x=>WindowClose()};
            CreateSMU = new CommandHandler { CanExecuteAction = CreateSMUValidate, ExecuteAction = CreateSMUAction };
            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            Source = new ObservableCollection<PTI>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Refresh();
            RefreshCommand.Execute(null);
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
                ptiContext.SetCancelPTI(SelectedPTI, obj.ToString());
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

                Helpers.PrintWithFormActionTwoSource(header,source, "MainApp.Reports.Layouts.PTI.rdlc", null);
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
                        new ReportParameter("Nomor",SelectedPTI.Code),
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
                        CreatedDate = vm.CreatedDate,
                        Id = vm.Id,
                        PayType = vm.PayType,
                        Pcs = vm.Collies.Sum(O => O.Pcs),
                        Weight = vm.Collies.Sum(O => O.Weight),
                        PTINumber = vm.PTINumber,
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

        private GridLength myHorizontalInputRegionSize = new GridLength(0, GridUnitType.Pixel);
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
    }
}
