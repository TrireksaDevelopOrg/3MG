using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
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

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for BrowseColliesView.xaml
    /// </summary>
    public partial class BrowseColliesView : Window
    {
        private BrowseColliesViewModel vm;

        public BrowseColliesView()
        {
            InitializeComponent();
            this.Loaded += BrowseColliesView_Loaded;
        }

        private void BrowseColliesView_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as BrowseColliesViewModel;
        }

        private void dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var control = (DataGrid)sender;
            var obj = control.SelectedItem as collies;
            if(obj!=null)
            {
                obj.IsSended = !obj.IsSended;
               // vm.SourceView.Refresh();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }


    public class BrowseColliesViewModel:BaseNotify
    {
        private SMUBussines context = new SMUBussines();
        public BrowseColliesViewModel(SMU selected)
        {
            SMUSelected = selected;
            MyTitle = "Tambah Item PTI Ke SMU";
            Source = new ObservableCollection<collies>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            LoadOutOfManifestData(selected.PTIId);
            OKCommand = new CommandHandler { CanExecuteAction = OKCommandValidate, ExecuteAction = OKCommandAction };
        }

        private bool OKCommandValidate(object obj)
        {
            if (Source.Where(O => O.IsSended).Count() > 0)
                return true;
            else
                return false;
        }

        private async void OKCommandAction(object obj)
        {
            try
            {
                await context.InsertPTIItemToSMU(SMUSelected, Source);
                Success = true;
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        public ObservableCollection<collies> Source { get; }
        public CollectionView SourceView { get; }
        public CommandHandler OKCommand { get; }
        public SMU SMUSelected { get; }
        public bool Success { get; private set; }

        private async void LoadOutOfManifestData(int id)
        {
            var data = await context.GetSMUOutOfManifest(id);
            Source.Clear();
            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

    }

}
