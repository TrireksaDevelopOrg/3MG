using DataAccessLayer.DataModels;
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

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for AddNewPlanes.xaml
    /// </summary>
    public partial class AddNewPlanes : Window
    {
        public AddNewPlanes()
        {
            InitializeComponent();
            this.DataContext = new AddNewPlaneViewModel() { WindowClose = Close };
        }
    }

    public class AddNewPlaneViewModel:planes
    {
        public new string MyTitle { get; set; } = "PESAWAT BARU";
        ScheduleBussines context = new ScheduleBussines();
        public Action WindowClose { get; set; }
        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }
        public bool SaveSuccess { get; private set; }
        public planes SaveResult { get; private set; }

        public AddNewPlaneViewModel()
        {
            SaveCommand = new CommandHandler { CanExecuteAction =x=> true, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction =x=> WindowClose()};

        }

        private async void SaveAction(object obj)
        {

            try
            {
                var item =(planes)this;
                planes result = await context.CreateNewPlane(item);
                SaveSuccess = true;
                SaveResult = result;
                WindowClose();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }

        }
    }
}
