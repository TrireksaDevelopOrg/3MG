using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
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
    /// Interaction logic for AddNewPort.xaml
    /// </summary>
    public partial class AddNewPort : Window
    {
        public AddNewPort()
        {
            InitializeComponent();
            this.DataContext = new AddNewPortViewModel() { WindowClose = Close };
        }
    }

    public class AddNewPortViewModel : ports
    {
        ScheduleBussines context = new ScheduleBussines();
        public Action WindowClose { get; internal set; }
        public ObservableCollection<city> Cities { get; }
        public CollectionView CitiesView { get; }

        public CommandHandler AddNewCityCommand { get; }
        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }
        public bool SaveSuccess { get; private set; }
        public ports SaveResult { get; private set; }

        public AddNewPortViewModel()
        {
            AddNewCityCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewCityAction };
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidation, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction =x=>true, ExecuteAction =x=> WindowClose() };
            Cities = new ObservableCollection<city>();
            CitiesView = (CollectionView)CollectionViewSource.GetDefaultView(Cities);
            LoadData();
        }

        private void AddNewCityAction(object obj)
        {
            var form = new Views.AddNewCity();
            form.ShowDialog();
            var vm = (AddNewCityViewModel)form.DataContext;
            if(vm.SaveSuccess & vm.SavedResult!=null)
            {
                Cities.Add(vm.SavedResult);
            }
            CitiesView.Refresh();
        }


        private bool SaveValidation(object obj)
        {
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Code) || this.CityId <= 0)
                return false;
            else
                return true;
        }


        private async void SaveAction(object obj)
        {
            try
            {
                var item = (ports)this;
                ports result = await context.CreateNewPort(item);
                this.SaveSuccess = true;
                this.SaveResult = result;
                WindowClose();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private async void LoadData()
        {
            var datas = await context.GetCities();
            foreach(var item in datas)
            {
                Cities.Add(item);
            }
            CitiesView.Refresh();
        }
    }
}
