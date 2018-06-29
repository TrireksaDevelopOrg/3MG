using DataAccessLayer.Models;
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
using System.Collections.ObjectModel;
using Ocph.DAL;
using DataAccessLayer;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for SplitSMUView.xaml
    /// </summary>
    public partial class SplitSMUView : Window
    {
        public SplitSMUView()
        {
            InitializeComponent();
        }
    }


    public class SplitSMUViewModel:BaseNotify
    {

        private SMUBussines context = new SMUBussines();

        public CommandHandler PindahkanCommand { get; }
        public CommandHandler KembaliCommand { get; }
        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }
        public ObservableCollection<SMUDetail> OriginSource { get; }
        public ObservableCollection<SMUDetail> DestinationSource { get; }
        public CollectionView OriginView { get; }
        public CollectionView DestinationView { get; }

        public SplitSMUViewModel(SMU SMUSelected)
        {
            MyTitle = "SPLIT SMU";
            SmuSelected = SMUSelected;
            PindahkanCommand = new CommandHandler { CanExecuteAction = PindahkanValidate, ExecuteAction = PindahkanAction };
            KembaliCommand = new CommandHandler { CanExecuteAction = KembaliValidate, ExecuteAction = KembaliAction };
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction =x=>true, ExecuteAction =x=>WindowClose()};

            OriginSource = new ObservableCollection<SMUDetail>();
            DestinationSource = new ObservableCollection<SMUDetail>();

            OriginView = (CollectionView)CollectionViewSource.GetDefaultView(OriginSource);
            DestinationView = (CollectionView)CollectionViewSource.GetDefaultView(DestinationSource);
            LoadData(SMUSelected);
            
        }

        private async void SaveAction(object obj)
        {
            try
            {
                var result = await context.SplitSMU(SmuSelected, OriginSource, DestinationSource);
                if(result!=null)
                {
                    this.Success = true;
                    this.SplitResult = result;
                    WindowClose();
                }
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        private async void LoadData(SMU SMUSelected)
        {
            var datas = await context.GetSMUDetail(SMUSelected.Id);
            foreach(var item in datas)
            {
                OriginSource.Add(item);
            }
            RefreshAll();
        }

        private bool SaveValidate(object obj)
        {
            if (OriginSource.Count > 0 && DestinationSource.Count > 0)
                return true;
            else
                return false;
        }

        private void KembaliAction(object obj)
        {
            foreach (var item in DestinationSource.Where(O => O.Selected).ToList())
            {
                var newItem = item.GetClone();
                newItem.Selected = false;
                OriginSource.Add(newItem);
                DestinationSource.Remove(item);
            }

            RefreshAll();
        }

        private bool KembaliValidate(object obj)
        {
            if (DestinationSource.Where(O => O.Selected).Count() > 0)
                return true;
            return false;
        }

        private bool PindahkanValidate(object obj)
        {
            if (OriginSource.Where(O => O.Selected).Count() > 0)
                return true;
            return false;
        }

        private  void PindahkanAction(object obj)
        {
            try
            {
                foreach (var item in OriginSource.Where(O => O.Selected).ToList())
                {
                    var newItem = item.GetClone();
                    newItem.Selected = false;
                    DestinationSource.Add(newItem);
                    OriginSource.Remove(item);
                }
              
                RefreshAll();
            }
            catch (Exception ex)
            {

                Helpers.ShowErrorMessage(ex.Message);
            }
         
        }


        public async void RefreshAll()
        {
            await Task.Delay(200);
            DestinationView.Refresh();
            OriginView.Refresh();
            TotalDestination = DestinationSource.Sum(O => O.Pcs* O.Weight);
            TOtalOrigin = OriginSource.Sum(O => O.Pcs* O.Weight);
        }


        public Action WindowClose { get; set; }
        public bool Success { get; internal set; }
        public Tuple<SMU, SMU> SplitResult { get; private set; }

        public double TOtalOrigin
        {
            get { return _totalOrigin; }
            set
            {
                SetProperty(ref _totalOrigin, value);
            }
        }

        public double TotalDestination
        {
            get { return _totalDestination; }
            set
            {
                SetProperty(ref _totalDestination, value);
            }
        }

        public SMU SmuSelected { get; }

       

        private double _totalDestination;
        private double _totalOrigin;
    }

}
