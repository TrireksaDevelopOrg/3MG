using DataAccessLayer.DataModels;
using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Xml;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            this.DataContext = new SettingViewModel() { WindowClose=Close};
        }

    }

    public class SettingViewModel:BaseNotify
    {
        private string company;

        public string Company
        {
            get {
                if(company==string.Empty)
                    company = Helpers.GetStringValue("Company");
                return company; }
            set {SetProperty(ref company ,value); }
        }

        private int cityId;

        public int CityId
        {
            get {
                if(cityId<=0)
                    cityId = Helpers.GetIntValue("CityId");
                return cityId; }
            set { SetProperty(ref cityId, value);
             
            }
        }

        private int portid;

        public int PortId
        {
            get {
                if(portid<=0)
                    portid = Helpers.GetIntValue("PortId");
                return portid; }
            set { SetProperty(ref portid, value);
               
            }
        }

        public CommandHandler SaveCommand { get; }
        public CommandHandler AddNewPortCommand { get; }
        public CommandHandler AddNewCityCommand { get; }
        public ObservableCollection<ports> PortSource { get; }
        public ObservableCollection<city> CitiesSource { get; }
        public CollectionView PortSourceView { get; }
        public CollectionView CitiesSourceView { get; }

        public SettingViewModel()
        {
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidation, ExecuteAction = SaveAction };
            AddNewPortCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewPortAction };
            AddNewCityCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewCitytAction };
            PortSource = new ObservableCollection<ports>();
            CitiesSource = new ObservableCollection<city>();
            PortSourceView = (CollectionView)CollectionViewSource.GetDefaultView(PortSource);
            CitiesSourceView = (CollectionView)CollectionViewSource.GetDefaultView(CitiesSource);

        }


        private ports selectedPort;

        public ports SelectedPort

        {
            get { return selectedPort; }
            set { SetProperty(ref selectedPort ,value);
                if (value != null)
                    CityId = value.CityId;
            }
        }


        private void AddNewPortAction(object obj)
        {
            var form = new Views.AddNewPort();
            form.ShowDialog();
            var vm = (AddNewPortViewModel)form.DataContext;
            if(vm.SaveSuccess && vm.SaveResult!=null)
            {
                PortSource.Add(vm.SaveResult);
            }
        }

        private void AddNewCitytAction(object obj)
        {
            throw new NotImplementedException();
        }

        private bool SaveValidation(object obj)
        {
            if (string.IsNullOrEmpty(Company) || PortId <= 0 || CityId <= 0)
                return false;
            return true;
        }

        private void SaveAction(object obj)
        {
            try
            {
               Helpers.UpdateKey("Company", Company);
                Helpers.UpdateKey("PortId", PortId.ToString());
                Helpers.UpdateKey("CityId", CityId.ToString());
                WindowClose();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        public Action WindowClose { get; set; }

    }
}
