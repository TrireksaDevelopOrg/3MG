using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using Ocph.DAL;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using DataAccessLayer.Models;
using System.Threading.Tasks;

namespace MainApp.Reports.Forms
{
    /// <summary>
    /// Interaction logic for CargoTerangkut.xaml
    /// </summary>
    public partial class CargoTerangkut : Page
    {
        private CargoTerangkutViewModel vm;
        private ReportViewer reportViewer;

        public CargoTerangkut(ReportViewer report)
        {
            InitializeComponent();
            vm= new CargoTerangkutViewModel();
            this.DataContext = vm;

            reportViewer = report;
            reportViewer.LocalReport.ReportEmbeddedResource = "MainApp.Reports.Layouts.CargoTerangkut.rdlc";
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
        }

        private async void sampaiTanggal_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dariTanggal.SelectedDate!=null && dariTanggal.SelectedDate<=sampaiTanggal.SelectedDate)
            {
              var datas =await vm.LoadData(dariTanggal.SelectedDate.Value, sampaiTanggal.SelectedDate.Value);

                reportViewer.LocalReport.DataSources.Clear();
                var datasource = new ReportDataSource { Name = "DataSet1", Value = datas };
                reportViewer.LocalReport.DataSources.Add(datasource);
                reportViewer.RefreshReport();
            }
        }

        
    }

    public class CargoTerangkutViewModel:BaseNotify
    {

        LaporanBussines context = new LaporanBussines();
        CustomerBussiness custContext = new CustomerBussiness();


        public CargoTerangkutViewModel()
        {
           var dataCustomer= custContext.GetCustomersDeposites();
            Customers = new  ObservableCollection<customer>(dataCustomer);
        }

        public ObservableCollection<customer> Customers { get; }

        public async Task<List<PreFligtManifest>> LoadData(DateTime From ,DateTime To)
        {
            try
            {
                if (Customer != null)
                {
                    var datas = await context.GetDataCargoterangkut(Customer.Id, From, To);
                    if(datas!=null && datas.Count()>0)
                    {
                       return datas;
                    }
                    else
                    {
                        throw new SystemException("Data Tidak Ada");
                    }
                }
                else
                    throw new SystemException("Pilih Customer");
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex.Message);
                return null;
            }
        }

      

        private customer cust;

        public customer Customer
        {
            get { return cust; }
            set { SetProperty(ref cust ,value); }
        }

    }
}
