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
    /// Interaction logic for BorderelCargo.xaml
    /// </summary>
    public partial class BorderelCargo : Page
    {
        private BorderelViewModel vm;
        private ReportViewer reportViewer;

        public BorderelCargo(ReportViewer report)
        {
            InitializeComponent();
            vm = new BorderelViewModel();
            reportViewer = report;
            this.DataContext = vm;
            reportViewer.LocalReport.ReportEmbeddedResource = "MainApp.Reports.Layouts.BorderelCargoLayout.rdlc";
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
        }

        private async void sampaiTanggal_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( sampaiTanggal.SelectedDate!=null)
            {
                var datas = await vm.LoadData(sampaiTanggal.SelectedDate.Value);

                reportViewer.LocalReport.DataSources.Clear();
                var datasource = new ReportDataSource { Name = "DataSet1", Value = datas };
                reportViewer.LocalReport.DataSources.Add(datasource);
                reportViewer.RefreshReport();
            }
        }

    }

    public class BorderelViewModel:BaseNotify
    {
        LaporanBussines context = new LaporanBussines();
        public async Task<List<BorderelCargoModel>> LoadData(DateTime From)
        {
            try
            {
                var datas = await context.GetDataBorderelCargo(From);
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
