using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
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
    /// Interaction logic for FlightView.xaml
    /// </summary>
    public partial class FlightView : Window
    {
        public FlightView()
        {
            InitializeComponent();
            this.DataContext = new FlightViewModel() { WindowClose = Close };
        }
    }

    public class FlightViewModel:Authorization
    {

        ScheduleBussines context = new ScheduleBussines();

        private DateTime date;

        public DateTime SelectedDate
        {
            get { return date; }
            set {
                SetProperty(ref date ,value);
                if(date!=null)
                    LoadData(date);
            }
        }


        public FlightViewModel():base(typeof(ScheduleBussines))
        {
            MyTitle = "FLIGHT SCHEDULE";
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
            AddNewJadwal = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewJadwalAction };
            Source = new ObservableCollection<Schedule>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            date = DateTime.Now;
            LoadData(date);
        }

        public async void LoadData(DateTime date)
        {
            try
            {
                List<Schedule> datas = await context.GetSchedules(date);
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

        public CommandHandler CancelCommand { get; }
        public CommandHandler AddNewJadwal { get; }
        public ObservableCollection<Schedule> Source { get; }
        public CollectionView SourceView { get; }
        public Action WindowClose { get; internal set; }

        private void AddNewJadwalAction(object obj)
        {
            var form = new Views.AddNewSchecule();
            form.ShowDialog();
            var vm = (AddNewScheduleViewModel)form.DataContext;
            if(vm.Success && vm.SchedulteCreated!=null)
            {
                Source.Add(vm.SchedulteCreated);
            }
            SourceView.Refresh();
        }
    }
}
