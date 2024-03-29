﻿using DataAccessLayer.Bussines;
using DataAccessLayer.Models;
using Microsoft.Reporting.WinForms;
using Ocph.DAL;
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
    /// Interaction logic for PreScheduleManifest.xaml
    /// </summary>
    public partial class PreScheduleManifest : Window
    {
        private PreScheduleManifestViewModel vm;
        public PreScheduleManifest()
        {
            InitializeComponent();
            vm = new PreScheduleManifestViewModel() { MyTitle = "PRE SCHEDULE MANIFEST", WindowClose = Close };
            this.DataContext = vm;
        }

        private void dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = dg.SelectedItem as PreFligtManifest;
            if (item != null)
            {
                item.IsSelected = !item.IsSelected;
            }
            vm.HitungTotal();
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if(e.Key== Key.Escape && vm!=null)
            {
                vm.Cari = string.Empty;
            }

            if (e.Key == Key.F4)
            {
                var searchBox = new SearchView();
                searchBox.OnCari += SearchBox_OnCari;
                searchBox.ShowDialog();
            }

            if (e.Key == Key.Escape)
            {
                vm.Cari = string.Empty;
            }
        }

        private void dg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter || e.Key == Key.Return)
            {
                var item = dg.SelectedItem as PreFligtManifest;
                if (item != null)
                {
                    item.IsSelected = !item.IsSelected;
                }
                vm.HitungTotal();
            }


        }

        private void SearchBox_OnCari(string message)
        {
            vm.Cari = message;
        }
    }


    public class PreScheduleManifestViewModel : BaseNotify
    {

        private string cari;

        public string Cari
        {
            get { return cari; }
            set
            {
                SetProperty(ref cari, value);
                SourceView.Refresh();
            }
        }


        private LaporanBussines context = new LaporanBussines();
        public PreScheduleManifestViewModel()
        {
            SplitCommand = new CommandHandler { CanExecuteAction = x => SelectedPTI!=null, ExecuteAction = SplitAction };
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshAction };
            PrintCommand = new CommandHandler { CanExecuteAction = x => TotalBerat > 0, ExecuteAction = PrintActiont };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
            Source = new ObservableCollection<PreFligtManifest>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Filter = DataFilter;
            SourceView.SortDescriptions.Add(new SortDescription("PTIID", ListSortDirection.Ascending));
            RefreshCommand.Execute(null);
        }

        public bool DataFilter(object obj)
        {
            try
            {
                var item = (PreFligtManifest)obj;
                if (item != null && !string.IsNullOrEmpty(Cari))
                {
                    var pti = string.Format("{0:D8}", item.PTIId);
                    if (pti.Contains(Cari) || item.ShiperName.ToUpper().Contains(Cari.ToUpper()) || item.RecieverName.ToUpper().Contains(Cari.ToUpper()))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {

                return true;
            }
            return true;
        }

        private void PrintActiont(object obj)
        {

            var message = new MyMessageBox("Print Pre Shcedule", "Tentukan Tanggal", MessageBoxButton.YesNo);
            var picker = new DatePicker() { Margin=new Thickness(10,3,0,0), Width=200};
            picker.SelectedDate = DateTime.Now;
            message.customPanel.Children.Add(picker);
            message.ShowDialog();
            if(message.MessageResult== MessageBoxResult.Yes)
            {
                var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet1", Value = Source.Where(O => O.IsSelected).OrderBy(O=>O.PTIId).ToList() };



                Helpers.PrintPreviewWithFormAction("PRE SCHEDULE FLIGHT", reportDataSource, "MainApp.Reports.Layouts.PreScheduleManifest.rdlc",
                    new ReportParameter[] {
                        new ReportParameter("Tanggal", picker.SelectedDate.Value.ToShortDateString())
                    }
                    
                    );
            }
         
        }

        private void SplitAction(object obj)
        {
            var form = new Views.SplitItemPTI();
            var vm = new Views.SplitItemPTIViewModel(SelectedPTI) { WindowClose=form.Close};
            form.DataContext = vm;
            form.ShowDialog();
            if(vm.Success)
            {
                Source.Add(vm.NewItem);
            }
            SourceView.Refresh();
        }

        private async void RefreshAction(object obj)
        {
            Cari = string.Empty;
            var datas = await context.GetDataBufferStock();
            Source.Clear();
            if(datas.Count>0)
            {
                foreach (var item in datas)
                    Source.Add(item);
            }
            SourceView.Refresh();
        }

        public Action WindowClose { get; internal set; }
        public CommandHandler SplitCommand { get; }
        public CommandHandler RefreshCommand { get; }
        public CommandHandler PrintCommand { get; }
        public CommandHandler CancelCommand { get; }
        public ObservableCollection<PreFligtManifest> Source { get; }
        public CollectionView SourceView { get; }


        private PreFligtManifest selectedPTI;

        public PreFligtManifest SelectedPTI
        {
            get { return selectedPTI; }
            set { SetProperty(ref selectedPTI ,value);
            }
        }

        public Task HitungTotal()
        {
            TotalBerat = Source.Where(O => O.IsSelected).Sum(O => O.TotalWeight);
            return Task.FromResult(0);
        }

        private double totalBerat;

        public double TotalBerat
        {
            get { return totalBerat; }
            set { SetProperty(ref totalBerat ,value); }
        }


    }
}
