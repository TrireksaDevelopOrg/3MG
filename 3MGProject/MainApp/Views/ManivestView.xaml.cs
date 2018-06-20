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
using Ocph.DAL;
using DataAccessLayer.Models;

using DataAccessLayer.Bussines;
using Microsoft.Reporting.WinForms;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for ManivestView.xaml
    /// </summary>
    public partial class ManivestView : Window
    {
        private ManifestViewModel viewmodel;
        public ManivestView()
        {
            InitializeComponent();
            viewmodel = new ManifestViewModel();
            this.DataContext = viewmodel;
           dg.PreviewKeyDown += Dg_PreviewKeyDown;
        }

        private void Dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter || e.Key== Key.Return)
            {
                DataGrid dg = (DataGrid)sender;
                var item = (Manifest)dg.SelectedItem;
                if (item != null)
                    viewmodel.ShowDetails(item);
            }
        }
    }


    public class ManifestViewModel:BaseNotify
    {
        ManifestBussiness context = new ManifestBussiness();
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

        private DateTime endDate;

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

        private Manifest manifestSelected;

        public Manifest ManifestSelected
        {
            get { return manifestSelected; }
            set { SetProperty(ref manifestSelected ,value);
                GridWidth = new GridLength(0, GridUnitType.Pixel);
            }
        }


        public ManifestViewModel()
        {

            PrintCommand = new CommandHandler { CanExecuteAction = x => ManifestSelected != null, ExecuteAction = PrintCommandAction };
            PrintPreviewCommand = new CommandHandler { CanExecuteAction = x => ManifestSelected != null, ExecuteAction = PrintPreviewCommandAction };
            CancelManifest = new CommandHandler { CanExecuteAction = Cancelmanifest, ExecuteAction = CancelManifestAction };
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshAction };
            AddNewManifestCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewManifestAction };
            Source = new ObservableCollection<Manifest>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Filter = DataFilter;
            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            Aktif = true;
            Batal = false;
            LoadData(startDate,endDate);
        }

        private async void PrintPreviewCommandAction(object obj)
        {
            try
            {
                var header = new List<Manifest>();
                header.Add(ManifestSelected);
                var details = await context.ManifestDetails(ManifestSelected);

                Helpers.PrintWithFormActionTwoSource(new ReportDataSource { Name = "Header", Value = header },
                    new ReportDataSource { Name = "Details", Value = details },
                    "MainApp.Reports.Layouts.CargoManifest.rdlc", null);

            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);

            }
        }

        private void PrintCommandAction(object obj)
        {
            throw new NotImplementedException();
        }

        private void CancelManifestAction(object obj)
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                    throw new SystemException("Tambahkan Alasan Pembatalan");

                context.SetCancelManifest(ManifestSelected, obj.ToString() );
                obj = string.Empty;
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

        private bool Cancelmanifest(object obj)
        {
            if (ManifestSelected != null && context.IsInRole("Manager").Result)
                return true;
            else
                return false;
        }

        private void RefreshAction(object obj)
        {
            LoadData(StartDate, EndDate);
        }

        public async void ShowDetails(Manifest selected)
        {
            if(selected.Details.Count<=0)
            {
                var result = await context.ManifestDetails(selected);
                foreach (var item in result)
                {
                    selected.Details.Add(item);
                }
                GridWidth = new GridLength(30, GridUnitType.Star);
            }
        }



        public async void LoadData(DateTime start,DateTime end)
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                var result = await context.GetManifest(start, end);
                foreach(var item in result)
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

        public CommandHandler PrintCommand { get; }
        public CommandHandler PrintPreviewCommand { get; }
        public CommandHandler CancelManifest { get; }
        public CommandHandler RefreshCommand { get; }
        public CommandHandler AddNewManifestCommand { get; }

        public ObservableCollection<Manifest>Source {get;set ;}
        public CollectionView SourceView { get; }

        private void AddNewManifestAction(object obj)
        {
            var form = new Views.AddNewManifest();
            form.ShowDialog();
            var vm = (AddNewManifestViewModel)form.DataContext;
            if(vm.Success && vm.SavedResult!=null)
            {
                Source.Add(vm.SavedResult);
            }
            SourceView.Refresh();
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
                SetProperty(ref myHorizontalInputRegionSize, value);
            }
        }

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
        private bool DataFilter(object obj)
        {
            var data = (Manifest)obj;
            if (data != null && data.ActiveStatus == DataAccessLayer.ActivedStatus.OK && this.Aktif)
                return true;
            if (data != null && data.ActiveStatus == DataAccessLayer.ActivedStatus.Cancel && this.Batal)
                return true;

            return false;
        }
    }
}
