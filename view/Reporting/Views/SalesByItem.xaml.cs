using entity;
using System;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    public partial class SalesByItem : Page
    {
        public SalesByItem()
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

            Data.SalesDSTableAdapters.SalesByItemTableAdapter SalesByItemTableAdapter = new Data.SalesDSTableAdapters.SalesByItemTableAdapter();

            //fill data
            SalesByItemTableAdapter.ClearBeforeFill = true;
            DataTable dt = new DataTable();
            dt = SalesByItemTableAdapter.GetDataBy(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);

            ReportPanel.ReportDt = dt;
  
            reportDataSource1.Name = "SalesByItem"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByItem.rdlc";

            SalesDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
        public void Filter(object sender, EventArgs e)
        {
          
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
           
            reportDataSource1.Name = "SalesByItem"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByItem.rdlc";
 

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
