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

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for ManivestView.xaml
    /// </summary>
    public partial class ManivestView : Window
    {

        public ManivestView()
        {
            InitializeComponent();
            this.DataContext = new ManifestViewModel();
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

        public ManifestViewModel()
        {
            AddNewManifestCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewManifestAction };
            Source = new ObservableCollection<Manifest>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);

            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            LoadData(startDate,endDate);
        }



        public async void LoadData(DateTime start,DateTime end)
        {
            try
            {
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
            
        }

        public CommandHandler AddNewManifestCommand { get; }

        public ObservableCollection<Manifest>Source {get;set ;}
        public CollectionView SourceView { get; }

        private void AddNewManifestAction(object obj)
        {
            var form = new Views.AddNewManifest();
            form.ShowDialog();
        }
    }
}
