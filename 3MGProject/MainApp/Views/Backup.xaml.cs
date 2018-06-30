using DataAccessLayer;
using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for Backup.xaml
    /// </summary>
    public partial class Backup : Window
    {
        private BackUpViewModel vm;

        public Backup()
        {
            InitializeComponent();
            vm = new BackUpViewModel() { WindowClose = Close };
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var f = new FolderBrowserDialog();
            f.SelectedPath = SettingConfiguration.GetStringValue("BackupPath");
            f.ShowDialog();
            if(f!=null && !string.IsNullOrEmpty(f.SelectedPath))
            {
                SettingConfiguration.UpdateKey("BackupPath", f.SelectedPath);
                vm.DataPath= f.SelectedPath+vm.GetFileName();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vm.BackupAction();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }



    public class BackUpViewModel:BackupMySqlBase
    {
        public BackUpViewModel()
        {
            MyTitle = "Backup Data";
            this.OnError += BackUpViewModel_OnError;
            this.OnComplete += BackUpViewModel_OnComplete;
            TotalData = 100;
            ProgressValue = 0;
            var path = SettingConfiguration.GetStringValue("BackupPath");
            if(!string.IsNullOrEmpty(path))
            {
                DataPath = path + GetFileName();
            }
        }

        public string GetFileName()
        {
            var date = DateTime.Now;

            return string.Format(@"\Data{0}{1:D2}{2:D2}{3}{4}.sql", date.Year, date.Month, date.Day, date.Hour, date.Minute);
        }

        private void BackUpViewModel_OnComplete(string message)
        {
            Helpers.ShowMessage(message);
        }

        private void BackUpViewModel_OnError(string message)
        {
            Helpers.ShowErrorMessage(message);
        }

        internal void BackupAction()
        {
            ProgressValue = 0;
            Task.Delay(1000);
            BackupAction(DataPath);
            TableName = "";

        }

        private string path;

        public string DataPath
        {
            get { return path; }
            set { SetProperty(ref path ,value); }
        }

        public Action WindowClose { get; internal set; }

    }
}
