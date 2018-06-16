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
                var context = new DataAccessLayer.Bussines.SMUBussines();
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


        public SMUViewModel()
        {
            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
            Source = new ObservableCollection<SMU>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Refresh();
            LoadData(StartDate, EndDate);
        }

    }
}
