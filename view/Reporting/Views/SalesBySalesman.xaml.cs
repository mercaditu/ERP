using entity;
using System;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    /// <summary>
    /// Interaction logic for SalesByItem.xaml
    /// </summary>
    public partial class SalesBySalesman : Page
    {
        public SalesBySalesman()
        {
            InitializeComponent();

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.SalesDS SalesDB = new Data.SalesDS();

            SalesDB.BeginInit();

            Data.SalesDSTableAdapters.SalesBySalesRepTableAdapter SalesBySalesRepTableAdapter = new Data.SalesDSTableAdapters.SalesBySalesRepTableAdapter();

            //fill data
            SalesBySalesRepTableAdapter.ClearBeforeFill = true;
            DataTable dt = new DataTable();

            dt = SalesBySalesRepTableAdapter.GetData(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
           
                ReportPanel.ReportDt = dt;
            
            reportDataSource1.Name = "SalesBySalesRep"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesBySalesRep.rdlc";

            SalesDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
        public void Filter(object sender, EventArgs e)
        {
           
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
         
            reportDataSource1.Name = "SalesBySalesRep"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesBySalesRep.rdlc";

          

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
