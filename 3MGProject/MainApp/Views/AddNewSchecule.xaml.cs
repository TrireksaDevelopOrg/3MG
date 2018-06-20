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
using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for AddNewSchecule.xaml
    /// </summary>
    public partial class AddNewSchecule : Window
    {
        public AddNewSchecule()
        {
            InitializeComponent();
            this.DataContext = new AddNewScheduleViewModel() { WindowClose = Close };
        }


    }

    public class AddNewScheduleViewModel : schedules,IDataErrorInfo
    {
        ScheduleBussines context = new ScheduleBussines();
        private planes _planeSelected;
        private ports _destiSelected;
        private ports _originSelected;

        public AddNewScheduleViewModel()
        {
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction =x=>true, ExecuteAction =x=> WindowClose()};
            AddNewPortCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewPortAction };
            AddNewPlaneCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddPlaneAction };
            Planes = new ObservableCollection<planes>();
            Ports = new ObservableCollection<ports>();

            PlanesView = (CollectionView)CollectionViewSource.GetDefaultView(Planes);
            Destinations = (CollectionView)CollectionViewSource.GetDefaultView(Ports);
            Origins = (CollectionView)CollectionViewSource.GetDefaultView(Ports);
            LoadData();
        }

        private void AddNewPortAction(object obj)
        {
            var form = new Views.AddNewPort();
            form.ShowDialog();
            var vm = (AddNewPortViewModel)form.DataContext;
            if(vm.SaveSuccess && vm.SaveResult!=null)
            {
                Ports.Add(vm.SaveResult);
            }
            Destinations.Refresh();
            Origins.Refresh();
        }

        private void AddPlaneAction(object obj)
        {
            var form = new Views.AddNewPlanes();
            form.ShowDialog();
            var vm = (AddNewPlaneViewModel)form.DataContext;
            if (vm.SaveSuccess && vm.SaveResult != null)
            {
                Planes.Add(vm.SaveResult);
            }
            PlanesView.Refresh();
        }

        private async void SaveAction(object obj)
        {
            try
            {
                schedules model = (schedules)this;
               var result= await context.AddNewSchedule(model);
                if(result!=null)
                {
                    var newSchedule = new Schedule
                    {
                        Complete = result.Complete,
                        CreatedDate = result.CreatedDate,
                        DestinationCityCode = DestinationSelected.Code,
                        DestinationPortCode = DestinationSelected.Code,
                        DestinationCityName = DestinationSelected.City,
                        DestinationPortName = DestinationSelected.Name,
                        End = result.End,
                        Id = result.Id,
                        OriginCityCode = OriginSelected.Code,
                        OriginCityName = OriginSelected.City,
                        OriginPortCode = OriginSelected.Code,
                        OriginPortName = OriginSelected.Name,
                        PlaneId = result.PlaneId,
                        PortFrom = result.PortFrom,
                        Start = result.Start,
                        Tanggal = result.Tanggal, PortTo=result.PortTo, Capasities=result.Capacities
                    };
                    this.SchedulteCreated = newSchedule;
                    Success = true;
                    WindowClose();
                }
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);

            }
        }

        private bool SaveValidate(object obj)
        {
            var result = true;
            if (this.End == new TimeSpan() || Start == new TimeSpan())
                result = false;

            if (Capacities<=0|| PlaneId <= 0 || PortTo <= 0 || PortFrom <= 0)
                result = false;

            if (Start > End || Start == End)
                result =false;

            return result;

        }

        private async void LoadData()
        {
            var planes = await context.GetPlanes();
            var ports = await context.GetPorts();

            foreach (var item in ports)
            {
                Ports.Add(item);
            }
            Destinations.Refresh();
            Origins.Refresh();

            foreach (var item in planes)
            {
                Planes.Add(item);
            }

            PlanesView.Refresh();
        }

        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }

        public CommandHandler AddNewPortCommand { get; }
        public CommandHandler AddNewPlaneCommand { get; }
        public ObservableCollection<planes> Planes { get; }
        public ObservableCollection<ports> Ports { get; }
        public CollectionView PlanesView { get; }
        public CollectionView Destinations { get; }
        public CollectionView Origins { get; }
        public Action WindowClose { get; internal set; }
        public bool Success { get; internal set; }

        public planes PlaneSelected
        {
            get { return _planeSelected; }
            set
            {
                SetProperty(ref _planeSelected, value);
            }
        }

        public ports DestinationSelected
        {
            get { return _destiSelected; }
            set
            {
                SetProperty(ref _destiSelected, value);
            }
        }

        public ports OriginSelected
        {
            get { return _originSelected; }
            set
            {
                SetProperty(ref _originSelected, value);
            }
        }

        public Schedule SchedulteCreated { get; private set; }

        public string Error => throw new NotImplementedException();

        public string this[string columnName] => throw new NotImplementedException();
    }
}
