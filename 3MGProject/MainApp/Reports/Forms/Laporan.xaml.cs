using Microsoft.Reporting.WinForms;
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
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
        }



        private void penjualan_Click(object sender, RoutedEventArgs e)
        {
            reportViewer.Clear();
            Main.Content = new Reports.Forms.PenjualanForm(reportViewer);
            viewmodel.MyTitle = "LAPORAN PENJUALAN";
        }

        private void keluar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void terangkut_Click(object sender, RoutedEventArgs e)
        {
            reportViewer.Clear();
            reportViewer.LocalReport.DataSources.Clear();
            Main.Content = new Reports.Forms.CargoTerangkut(reportViewer);
            viewmodel.MyTitle = "CARGO TERANGKUT";
        }

        private void borderel_Click(object sender, RoutedEventArgs e)
        {
            reportViewer.Clear();
            reportViewer.LocalReport.DataSources.Clear();
            Main.Content = new Reports.Forms.BorderelCargo(reportViewer);
            viewmodel.MyTitle = "BORDEREL CARGO";
        }

        private void buffer_Click(object sender, RoutedEventArgs e)
        {
            reportViewer.Clear();
            reportViewer.LocalReport.DataSources.Clear();
            Main.Content = new Reports.Forms.BufferStock(reportViewer);
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
