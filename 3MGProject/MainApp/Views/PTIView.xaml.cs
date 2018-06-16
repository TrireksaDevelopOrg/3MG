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
using DataAccessLayer;
using DataAccessLayer.Bussines;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for PTIView.xaml
    /// </summary>
    public partial class PTIView : Window
    {
        public PTIView()
        {
            InitializeComponent();
        }
    }

    public class PTIViewModel : BaseNotify
    {

        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate ,value);
                if (StartDate < EndDate)
                    LoadData(StartDate, EndDate);
            }
        }

        private DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { SetProperty(ref  endDate ,value);
                if (StartDate < EndDate)
                    LoadData(StartDate, EndDate);
            }
        }


        private PTI selectedPTI;

        public PTI SelectedPTI
        {
            get { return selectedPTI; }
            set { SetProperty(ref selectedPTI ,value); }
        }


        public ObservableCollection<PTI> Source { get; }
        public CollectionView SourceView { get; }
        public Action WindowClose { get; internal set; }
        public CommandHandler AddNewPTICommand { get; }
        public CommandHandler CancelCommand { get; }
        public CommandHandler CreateSMU { get; }

        DataAccessLayer.Bussines.PtiBussines ptiContext = new DataAccessLayer.Bussines.PtiBussines();
        public PTIViewModel()
        {
            AddNewPTICommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewPTIAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction =x=>WindowClose()};
            CreateSMU = new CommandHandler { CanExecuteAction = CreateSMUValidate, ExecuteAction = CreateSMUAction };

            var date = DateTime.Now;
            startDate = new DateTime(date.Year, date.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            Source = new ObservableCollection<PTI>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Refresh();

            LoadData(StartDate, EndDate);
        }

        private bool CreateSMUValidate(object obj)
        {
            if (SelectedPTI != null)
                return true;
            return false;
        }

        private void CreateSMUAction(object obj)
        {
            var form = new Views.AddNewSMU();
            var vm = new Views.AddNewSMUViewModel(SelectedPTI) { WindowClose = form.Close };
            form.DataContext = vm;
            form.ShowDialog();

        }

        private void AddNewPTIAction(object obj)
        {
            var form = new Views.AddNewPTI();
            form.ShowDialog();
            var vm = (AddNewPTIViewModel)form.DataContext;
            if(vm.Saved)
            {
                PTI p = new PTI
                {
                    CreatedDate = vm.CreatedDate,
                    Id = vm.Id,
                    PayType = vm.PayType,
                    Pcs = vm.Collies.Sum(O => O.Pcs),
                    Weight = vm.Collies.Sum(O => O.Weight),
                    PTINumber = vm.PTINumber,
                    RecieverName = vm.Reciever.Name,
                    ShiperName = vm.Shiper.Name,
                    Biaya = vm.Collies.Sum(O => O.Biaya),
                    User = Authorization.User.Name
                };
                Source.Add(p);
            }
            SourceView.Refresh();
        }

        private async void LoadData(DateTime startDate, DateTime endDate)
        {
           var result = await ptiContext.GetPTIFromTo(StartDate, EndDate);
            Source.Clear();
            foreach(var item in result)
            {
                Source.Add(item);
            }
            SourceView.Refresh();
        }
    }
}
