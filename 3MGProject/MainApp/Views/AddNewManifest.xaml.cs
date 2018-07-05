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
        private AddNewManifestViewModel vm;

        public AddNewManifest()
        {
            InitializeComponent();
            vm = new AddNewManifestViewModel() { WindowClose = this.Close};
            this.DataContext = vm;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataSMU = ((DataGrid)sender).SelectedItem as SMU;
            if(dataSMU!=null)
            {
                dataSMU.IsSended = !dataSMU.IsSended;
            }

        }
    }


    public class AddNewManifestViewModel:manifestoutgoing
    {
        public new string MyTitle { get; set; } = "MANIFEST BARU";
        ManifestBussiness context = new ManifestBussiness();
        ScheduleBussines scheduleBussines = new ScheduleBussines();
        SMUBussines smuBUssines = new SMUBussines();
        public AddNewManifestViewModel()
        {

            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction =x=>true, ExecuteAction =x=> WindowClose()};
            SplitCommand = new CommandHandler { CanExecuteAction = SplitValidate, ExecuteAction = SplitAction };
            AddNewSchedule = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = addNewScheduleAction };

            Source = new ObservableCollection<SMU>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceSchedules = new ObservableCollection<Schedule>();
            SourceViewSchedules = (CollectionView)CollectionViewSource.GetDefaultView(SourceSchedules);
            SourceView.Refresh();
            LoadData();
            IsNew = true;
        }

        private void addNewScheduleAction(object obj)
        {
            var form = new Views.AddNewSchecule();
            form.ShowDialog();
            var vm = form.DataContext as AddNewScheduleViewModel;
            if(vm.Success && vm.SchedulteCreated!=null)
            {
                SourceSchedules.Add(vm.SchedulteCreated);
                ScheduleSelected = vm.SchedulteCreated;

            }
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
                string message = string.Format("Telah Di Split Ke :\r SMU : T{0:d9} \r SMU : T{1:D9}", viewmodel.SplitResult.Item1.Id, viewmodel.SplitResult.Item2.Id);
               Helpers.ShowMessage(message, string.Format("SMU T{0:D8}"+ viewmodel.SplitResult.Item1.Id));
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

        private async void SaveAction(object obj)
        {
            try
            {
                manifestoutgoing manifest = (manifestoutgoing)this;
                var result = await context.CreateNewManifest(ScheduleSelected, manifest, Source);
                Success = true;

                var man = new Manifest
                {
                    Complete = false,
                    CreatedDate = result.CreatedDate,
                    OriginPortName = ScheduleSelected.OriginPortName,
                    DestinationPortName = ScheduleSelected.DestinationPortName,
                    DestinationPortCode = ScheduleSelected.DestinationPortCode,
                    OriginPortCode = ScheduleSelected.OriginPortCode,
                    PlaneName = ScheduleSelected.PlaneName,
                    PlaneCode = ScheduleSelected.PlaneCode,
                    End = ScheduleSelected.End,
                    Start = ScheduleSelected.Start,
                    IsTakeOff = false,
                    User = result.User,
                    Tanggal = result.CreatedDate,
                    Id = result.Id,
                    PlaneId = ScheduleSelected.PlaneId,
                    PortFrom = ScheduleSelected.PortFrom,
                    PortTo = ScheduleSelected.PortTo,
                    SchedulesId = SchedulesId
                };

                this.SavedResult = man;
                WindowClose();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private async void LoadData()
        {
            try
            {
                this.CreatedDate = DateTime.Now;
                var scedules = await scheduleBussines.GetSchedules(DateTime.Now);
                foreach (var item in scedules.Where(O => !O.Complete))
                {
                    this.SourceSchedules.Add(item);
                }
                var data = await context.GetSMUForCreateManifest();
                foreach (var item in data)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    Source.Add(item);
                }
            }
            catch (Exception EX)
            {

                Helpers.ShowErrorMessage(EX.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var id = 0;
            try
            {

           
                var item = (SMU)sender;
                if(item!=null && item.IsSended)
                {
                    if (this.ScheduleSelected==null)
                    {
                        id++;
                        item.SetSilentIsSended(false);
                        //item.IsSended = false;
                        throw new SystemException("Anda Belum Memilih Jadwal");
                    }
                    else
                    {
                        if (item.PayType == PayType.Deposit)
                        {
                            var result = context.CustomerDepositCukup(item.ShiperId, item.Total);
                            if (!result.Item1)
                            {
                                item.IsSended = false;
                                MyMessage.Show(string.Format("Sisa Saldo {0} Sebesar : Rp{1:N2}", item.ShiperName, result.Item2), "Saldo Deposit Kurang", MessageBoxButton.OK);
                            }else
                            {
                                if(result.Item2-Source.Where(O=>O.ShiperId==item.ShiperId && O.IsSended).Sum(O=>O.Total)<0)
                                    MyMessage.Show(string.Format("Sisa Saldo {0} Sebesar : Rp{1:N2}", item.ShiperName, result.Item2), "Saldo Deposit Kurang", MessageBoxButton.OK);
                            }
                        }
                    }
                }

                HitungSisa();

                
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
           
        }


        private async void HitungSisa()
        {
            await Task.Delay(200);
            TotalColly = Source.Where(O => O.IsSended).Sum(O => O.Pcs);
            TotalBerat = Source.Where(O => O.IsSended).Sum(O => O.Weight);
            Sisa = ScheduleSelected.Capasities - TotalBerat;
           // SourceView.Refresh();
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
        public CommandHandler AddNewSchedule { get; }
        public ObservableCollection<SMU> Source { get; }
        public CollectionView SourceView { get; }
        public ObservableCollection<Schedule> SourceSchedules { get; }
        public CollectionView SourceViewSchedules { get; }
        public bool IsNew { get; }
        public Action WindowClose { get;  set; }
        public bool Success { get; private set; }
        public Manifest SavedResult { get; private set; }
    }
}
