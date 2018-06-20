using System;
using System.Collections.Generic;
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

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for AddNewCity.xaml
    /// </summary>
    public partial class AddNewCity : Window
    {
        public AddNewCity()
        {
            InitializeComponent();
            this.DataContext = new AddNewCityViewModel() { WindowClose = Close };
        }

    }


    public class AddNewCityViewModel:city
    {
        ScheduleBussines context = new ScheduleBussines();
        public AddNewCityViewModel()
        {
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidation, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction =x=>WindowClose() };
        }


        public CommandHandler SaveCommand { get; set; }
        public CommandHandler CancelCommand { get; set; }

        public async void SaveAction(object obj)
        {
            try
            {
                var item = (city)this;
               var result = await context.CreateNewCity(item);
                SaveSuccess = true;
                SavedResult = result;
                WindowClose();
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        public bool SaveValidation(object obj)
        {
            if (string.IsNullOrEmpty(this.CityName) || string.IsNullOrEmpty(this.CityCode))
                return false;
            else
                return true;
        }


        public void CancelAction()
        {
            WindowClose();
        }

        public Action WindowClose { get; set; }
        public bool SaveSuccess { get; private set; }
        public city SavedResult { get; private set; }
    }



}
