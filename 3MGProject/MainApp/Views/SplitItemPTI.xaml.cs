using DataAccessLayer.Bussines;
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

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for SplitItemPTI.xaml
    /// </summary>
    public partial class SplitItemPTI : Window
    {
        public SplitItemPTI()
        {
            InitializeComponent();
        }
    }

    public class SplitItemPTIViewModel:BaseNotify
    {
        public SplitItemPTIViewModel(PreFligtManifest itemPti)
        {
            MyTitle = "SPLIT ITEM PTI";
            SelectedItemPTI = itemPti;
            SourceView = new ObservableCollection<PreFligtManifest>(new List<PreFligtManifest> { SelectedItemPTI});
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };

        }

        private bool SaveValidate(object obj)
        {
            if (Jumlah >= 0 && Jumlah < SelectedItemPTI.Pcs)
                return true;
            return false;
        }

        private void SaveAction(object obj)
        {
            try
            {
                var newitem = context.SplitItemPTI(SelectedItemPTI, Jumlah);
                if (newitem != null)
                {
                    SelectedItemPTI.Pcs = SelectedItemPTI.Pcs - Jumlah;
                    NewItem = SelectedItemPTI.GetClone();
                    NewItem.Pcs = Jumlah;
                    Success = true;
                }
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }


        private PtiBussines context = new PtiBussines();

        public PreFligtManifest SelectedItemPTI { get; }
        public ObservableCollection<PreFligtManifest> SourceView { get; }
        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; private set; }
        public Action WindowClose { get; set; }

        private int _jumlah;

        public int Jumlah
        {
            get { return _jumlah; }
            set {SetProperty(ref _jumlah ,value);
                Total = value * SelectedItemPTI.Weight;
                Biaya = Total * SelectedItemPTI.Price;
            }
        }

        private double total;

        public double Total
        {
            get { return total; }
            set { SetProperty(ref total ,value); }
        }

        private double _biaya;

        public double Biaya
        {
            get { return _biaya; }
            set { SetProperty(ref _biaya ,value); }
        }

        public bool Success { get; private set; }
        public PreFligtManifest NewItem { get; internal set; }
    }
}
