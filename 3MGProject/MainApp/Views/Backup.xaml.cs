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
        public Backup()
        {
            InitializeComponent();
            this.DataContext = new BackUpViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var f = new FolderBrowserDialog();
            f.ShowDialog();

        }
    }
    public class BackUpViewModel:BaseNotify
    {
        public BackUpViewModel()
        {
            MyTitle = "Backup Data";
        }
        private string path;

        public string DataPath
        {
            get { return path; }
            set { SetProperty(ref path ,value); }
        }

    }
}
