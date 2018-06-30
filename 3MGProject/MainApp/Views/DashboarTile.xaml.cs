using Ocph.DAL;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for DashboarTile.xaml
    /// </summary>
    public partial class DashboarTile : UserControl
    {
        public DashboarTile()
        {
            InitializeComponent();
        }

        public LinearGradientBrush BackgroundColor { get; set; }
        public string TitleTile { get; set; }
        public string Nilai { get; set; }
        public double ProgressValue { get; set; }
        public double MaxValue { get; set; }
    }

    public class DashboardTileViewModel:BaseNotify
    {
        

    }
}
