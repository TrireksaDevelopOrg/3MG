using DataAccessLayer;
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
    /// Interaction logic for AddNewManifest.xaml
    /// </summary>
    public partial class AddNewManifest : Window
    {
        private AddNewManifestViewModel viewmodel;
        public AddNewManifest()
        {
            InitializeComponent();
            viewmodel = new AddNewManifestViewModel() { WindowClose = Close };
            this.DataContext = viewmodel;
           
        }
    }


    public class AddNewManifestViewModel:manifestoutgoing
    {
        ManifestBussiness context = new ManifestBussiness();
        ScheduleBussines scheduleBussines = new ScheduleBussines();
        SMUBussines smuBUssines = new SMUBussines();
        public AddNewManifestViewModel()
        {

            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction =x=>true, ExecuteAction =x=> WindowClose()};
            SplitCommand = new CommandHandler { CanExecuteAction = SplitValidate, ExecuteAction = SplitAction };

            Source = new ObservableCollection<SMU>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceSchedules = new ObservableCollection<Schedule>();
            SourceViewSchedules = (CollectionView)CollectionViewSource.GetDefaultView(SourceSchedules);
            SourceView.Refresh();
            LoadData();
            IsNew = true;
        }

        private  void SplitAction(object obj)
        {
            var form = new Views.SplitSMUView();
            var viewmodel = new Views.SplitSMUViewModel(SMUSelected) { WindowClose=form.Close};
            form.DataContext = viewmodel;
            form.ShowDialog();
            if(viewmodel.Success && viewmodel.SplitResult!=null)
            {
                Source.Remove(SMUSelected);
                Source.Add(viewmodel.SplitResult.Item1);
                Source.Add(viewmodel.SplitResult.Item2);
                string message = string.Format("Telah Di Split Ke :\r SMU : {0} \r SMU : {1}", viewmodel.SplitResult.Item1.Code, viewmodel.SplitResult.Item2.Code);
                Helpers.ShowMessage(message);
                MessageBox.Show(message, "SMU "+ viewmodel.SplitResult.Item1.Code, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private bool SplitValidate(object obj)
        {
            if (SMUSelected != null)
                return true;
            return false;
        }


        private bool SaveValidate(object obj)
        {
            if(Sisa>=0 && Source.Where(O=>O.IsSended).Count()>0)
            return true;
            return false;
        }

        private void SaveAction(object obj)
        {
            try
            {
                manifestoutgoing manifest = (manifestoutgoing)this;
                context.CreateNewManifest(ScheduleSelected, manifest, Source);
                Success = true;
                WindowClose();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private async void LoadData()
        {
            this.CreatedDate = DateTime.Now;
            this.Code = await CodeGenerate.GetNewManifestNumber();
            var scedules = await scheduleBussines.GetSchedules(DateTime.Now);
            foreach (var item in scedules.Where(O => !O.Complete))
            {
           this.SourceSchedules.Add(item);
            }
            var data = await context.GetSMUForCreateManifest();
            foreach(var item in data)
            {
                item.PropertyChanged += Item_PropertyChanged;
                Source.Add(item);
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var item = (SMU)sender;
            if (this.SchedulesId <= 0 && item.IsSended)
            {
                item.IsSended = false;
                Helpers.ShowErrorMessage("Anda Belum Memilih Jadwal");
            }

            if(ScheduleSelected!=null)
            {
                TotalColly = Source.Where(O => O.IsSended).Sum(O => O.Pcs);
                TotalBerat = Source.Where(O => O.IsSended).Sum(O => O.Weight);
                Sisa = ScheduleSelected.Capasities - TotalBerat;
            }
        }

        private int totalColly;

        public int TotalColly
        {
            get { return totalColly; }
            set { SetProperty(ref totalColly ,value); }
        }
        private double totalBerat;

        public double TotalBerat
        {
            get { return totalBerat; }
            set { SetProperty(ref totalBerat, value); }
        }

        public double Sisa
        {
            get { return sisa; }
            set { SetProperty(ref sisa, value); }
        }


        private Schedule scheduleSelected;
        private double sisa;

        public Schedule ScheduleSelected
        {
            get { return scheduleSelected; }
            set { SetProperty(ref scheduleSelected ,value); }
        }

        private SMU smuSelected;
        public SMU SMUSelected
        {
            get { return smuSelected; }
            set { SetProperty(ref smuSelected, value); }
        }

        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }
        public CommandHandler SplitCommand { get; }
        public ObservableCollection<SMU> Source { get; }
        public CollectionView SourceView { get; }
        public ObservableCollection<Schedule> SourceSchedules { get; }
        public CollectionView SourceViewSchedules { get; }
        public bool IsNew { get; }
        public Action WindowClose { get; internal set; }
        public bool Success { get; private set; }
    }
}
