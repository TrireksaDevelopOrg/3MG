using DataAccessLayer.Bussines;
using DataAccessLayer.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApp.Reports.Forms
{
    /// <summary>
    /// Interaction logic for BufferStock.xaml
    /// </summary>
    public partial class BufferStock : Page
    {
        private BufferViewModel vm;

        public BufferStock()
        {
            InitializeComponent();
            vm = new BufferViewModel();
            this.DataContext = vm;
            reportViewer.LocalReport.ReportEmbeddedResource = "MainApp.Reports.Layouts.BufferStock.rdlc";
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
            this.Loaded += BufferStock_Loaded;
        }

        private async void BufferStock_Loaded(object sender, RoutedEventArgs e)
        {
            var datas = await vm.LoadData();
            reportViewer.LocalReport.DataSources.Clear();
            var datasource = new ReportDataSource { Name = "DataSet1", Value = datas };
            reportViewer.LocalReport.DataSources.Add(datasource);
            reportViewer.RefreshReport();
        }
    }

    public class BufferViewModel : BaseNotify
    {
        LaporanBussines context = new LaporanBussines();

        public async Task<List<PreFligtManifest>> LoadData()
        {
            try
            {
                var datas = await context.GetDataBufferStock();
                if (datas != null && datas.Count() > 0)
                {
                    return datas;
                }
                else
                {
                    throw new SystemException("Data Tidak Ada");
                }
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
                return null;
            }
        }

    }
}
