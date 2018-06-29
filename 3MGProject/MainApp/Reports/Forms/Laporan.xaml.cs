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
using System.Windows.Shapes;

namespace MainApp.Reports.Forms
{
    /// <summary>
    /// Interaction logic for Laporan.xaml
    /// </summary>
    public partial class Laporan : Window
    {
        LaporanViewModel viewmodel;
        public Laporan()
        {
            InitializeComponent();
            viewmodel = new LaporanViewModel();
            this.DataContext = viewmodel;
        }

        private void penjualan_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Reports.Forms.PenjualanForm();
            viewmodel.MyTitle = "LAPORAN PENJUALAN";
        }

        private void keluar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void terangkut_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Reports.Forms.CargoTerangkut();
            viewmodel.MyTitle = "CARGO TERANGKUT";
        }

        private void borderel_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Reports.Forms.BorderelCargo();
            viewmodel.MyTitle = "BORDEREL CARGO";
        }

        private void buffer_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Reports.Forms.BufferStock();
            viewmodel.MyTitle = "BUFFER STOCK";
        }
    }

    public class LaporanViewModel:BaseNotify
    {
        public LaporanViewModel()
        {
            MyTitle = "LAPORAN";
        }
    }
}
