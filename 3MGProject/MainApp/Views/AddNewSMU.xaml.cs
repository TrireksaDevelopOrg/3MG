using DataAccessLayer;
using DataAccessLayer.DataModels;
using DataAccessLayer.Models;
using Ocph.DAL;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for AddNewSMU.xaml
    /// </summary>
    public partial class AddNewSMU : Window
    {
        public AddNewSMU()
        {
            InitializeComponent();
        }

        private void checkALL_Click(object sender, RoutedEventArgs e)
        {
            var vm = (AddNewSMUViewModel)DataContext;
            var res = ((CheckBox)sender).IsChecked;
            dg.CommitEdit();
            dg.CancelEdit();
            dg.Items.Refresh();
            vm.CheckAll = res.Value;
           
        }
    }

    public class AddNewSMUViewModel:BaseNotify
    {
        private bool checkAll;

        public bool CheckAll
        {
            get { return checkAll; }
            set {
     
                SetProperty(ref checkAll ,value);
                SetCheckAll(value);
            }
        }

        private async void SetCheckAll(bool value)
        {
            try
            {
                foreach (var item in Source)
                {
                    item.IsSended = value;
                }
                
                SourceView.Refresh();
            }
            catch (Exception)
            {
                await Task.Delay(1000);
            }finally
            {
                SourceView.Refresh();
            }
        }

        public AddNewSMUViewModel(PTI ptiSelected)
        {
            this.PTISelected = ptiSelected;
            SaveCommand = new CommandHandler { CanExecuteAction = SaveValidate, ExecuteAction = SaveAction };
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
            Source = new ObservableCollection<collies>();
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            LoadData(ptiSelected);

        }

        private bool SaveValidate(object obj)
        {
            SMUCode = CodeGenerate.SMU(CodeGenerate.GetNewSMUNumber().Result);
            if(Source.Where(O=>O.IsSended).Count()>0)
              return true;
            return false;
        }

        private void SaveAction(object obj)
        {
            try
            {
                var context = new DataAccessLayer.Bussines.SMUBussines();
                context.CreateNewSMU(PTISelected,Source);
                Helpers.ShowMessage("SMU Tersimpan");
                Saved = true;
                WindowClose();
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
            }
        }

        public ObservableCollection<collies> Source { get; }
        public CollectionView SourceView { get; }
        public Action WindowClose { get; set; }
        public PTI PTISelected { get; }
        public CommandHandler SaveCommand { get; }
        public CommandHandler CancelCommand { get; }
        public bool Saved { get; private set; }

        private async void LoadData(PTI ptiSelected)
        {
            var context = new DataAccessLayer.Bussines.PtiBussines();
            var res =await context.GetColliesOutOfSMU(ptiSelected.Id);
            Source.Clear();
            foreach(var item in res)
            {
                Source.Add(item);
            }
            SourceView.Refresh();
        }



        private string smucode;

        public string SMUCode
        {
            get { return smucode; }
            set { SetProperty(ref smucode, value); }
        }

    }









}
