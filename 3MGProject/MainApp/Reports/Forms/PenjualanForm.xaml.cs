using DataAccessLayer.Models;
using Microsoft.Reporting.WinForms;
using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MainApp.Reports.Forms
{
    /// <summary>
    /// Interaction logic for PenjualanForm.xaml
    /// </summary>
    public partial class PenjualanForm : Page
    {
        PenjualanViewModel viewmodel = new PenjualanViewModel();
        private ReportViewer reportViewer;

        public PenjualanForm(ReportViewer report)
        {
            InitializeComponent();

            this.DataContext = new PenjualanViewModel();
            reportViewer = report;
            reportViewer.LocalReport.ReportEmbeddedResource = "MainApp.Reports.Layouts.Penjualan.rdlc";
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
        }


        private void Refresh(List<Penjualan> datas,DateTime dari,DateTime sampai)
        {
            ReportParameter[] parameters =
                {
                       new ReportParameter("DariTanggal",String.Format("{0}-{1}-{2}",dari.Day,dari.Month,dari.Year)),
                    new ReportParameter("SampaiTanggal",String.Format("{0}-{1}-{2}",sampai.Day,sampai.Month,sampai.Year))
                };

            reportViewer.LocalReport.DataSources.Clear();
            var datasource = new ReportDataSource { Name = "DataSet1", Value = datas };
            reportViewer.LocalReport.DataSources.Add(datasource);
            if (parameters != null)
                    reportViewer.LocalReport.SetParameters(parameters);
            reportViewer.RefreshReport();
        }

        private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var dari = dariTanggal.SelectedDate.Value;
            var sampai = sampaiTanggal.SelectedDate.Value;
            if (sampaiTanggal.SelectedDate!=null && dariTanggal.SelectedDate!=null && (sampaiTanggal.SelectedDate>=dariTanggal.SelectedDate))
            {
                var data = await viewmodel.LoadData(dariTanggal.SelectedDate.Value, sampaiTanggal.SelectedDate.Value);
                Refresh(data,dari,sampai);
            }
         
        }
    }




    public class PenjualanViewModel : BaseNotify
    {


        DataAccessLayer.Bussines.LaporanBussines context = new DataAccessLayer.Bussines.LaporanBussines();
        private DateTime dari;

        public DateTime Dari
        {
            get { return dari; }
            set {SetProperty (ref dari ,value);
            }
        }


        private DateTime sampai;

        public DateTime Sampai
        {
            get { return sampai; }
            set {SetProperty(ref sampai ,value);
            }
        }
        public PenjualanViewModel()
        {
            MyTitle = "LAPORAN PENJUALAN";
        }

        public Task<List<Penjualan>> LoadData(DateTime Dari,DateTime Sampai)
        {
            var result = context.GetDataLaporan(Dari, Sampai);
            return result;
        }



    }

}
