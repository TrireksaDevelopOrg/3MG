using DataAccessLayer.Bussines;
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
    /// Interaction logic for BrowseSMU.xaml
    /// </summary>
    public partial class BrowseSMU : Window
    {
        public BrowseSMU()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var data = dg.SelectedItem as SMU;
            if ( data != null)
                data.IsSended = !data.IsSended;
        }
    }

    public class BrowseSMUViewModel:BaseNotify
    {


        ManifestBussiness context = new ManifestBussiness();
        public BrowseSMUViewModel(Manifest manifestSelected)
        {
            ManifestSelected = manifestSelected;
            OKCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = OKAction };
            Source = new ObservableCollection<SMU>();
            LoadData();
        }

        private async void LoadData()
        {
            var data = await context.GetSMUForCreateManifest();
            foreach (var item in data)
            {
                var result = ManifestSelected.Details.Where(O => O.Id == item.Id).FirstOrDefault();
                if (result == null)
                    Source.Add(item);
            }
        }

        public Manifest ManifestSelected { get; }
        public CommandHandler OKCommand { get; }
        public ObservableCollection<SMU> Source { get; }
        public Action WindowClose { get; internal set; }

        private void OKAction(object obj)
        {
            try
            {
                context.InsertNewSMU(ManifestSelected, Source.Where(O => O.IsSended));
                foreach(var item in Source.Where(O=>O.IsSended))
                {
                    ManifestSelected.Details.Add(item);
                }
                Helpers.ShowMessage("SMU Berhasil Ditambah");
                WindowClose();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
        }
    }
}
