using DataAccessLayer.Bussines;
using DataAccessLayer.Models;
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

        }

        private void dg_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Space || e.Key== Key.Enter || e.Key== Key.Return)
            {
                var item = dg.SelectedItem as PreFligtManifest;
                if(item!=null)
                {
                    item.IsSended = !item.IsSended;
                }
                vm.HitungTotal();
               
            }
        }
    }


    public class PreScheduleManifestViewModel : BaseNotify
    {

        private LaporanBussines context = new LaporanBussines();
        public PreScheduleManifestViewModel()
        {

            SplitCommand = new CommandHandler { CanExecuteAction = x => SelectedPTI!=null, ExecuteAction = SplitAction };
            RefreshCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = RefreshAction };
            PrintCommand = new CommandHandler { CanExecuteAction = x => TotalBerat > 0, ExecuteAction = PrintActiont };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };

           
            Source = new ObservableCollection<PreFligtManifest>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.SortDescriptions.Add(new SortDescription("PTIID", ListSortDirection.Ascending));
            RefreshCommand.Execute(null);
            
        }

        private void PrintActiont(object obj)
        {

            var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "DataSet1", Value = Source.Where(O => O.IsSended).ToList() };
            Helpers.PrintPreviewWithFormAction("PRE SCHEDULE FLIGHT", reportDataSource, "MainApp.Reports.Layouts.PreScheduleManifest.rdlc", null);
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
            TotalBerat = Source.Where(O => O.IsSended).Sum(O => O.TotalWeight);
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
