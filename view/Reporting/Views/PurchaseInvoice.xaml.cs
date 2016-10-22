using System.Data;
using System.Windows;
using System.Windows.Controls;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class PurchaseInvoice : Page
    {
        public PurchaseInvoice()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.PurchaseDS PurchaseDB = new Data.PurchaseDS();

            PurchaseDB.BeginInit();

            Data.PurchaseDSTableAdapters.PurchaseInvoiceSummaryTableAdapter PurchaseInvoiceSummaryTableAdapter = new Data.PurchaseDSTableAdapters.PurchaseInvoiceSummaryTableAdapter();
                
            DataTable dt = new DataTable();

            dt = PurchaseInvoiceSummaryTableAdapter.GetDataByDate(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
            ReportPanel.ReportDt = dt;
         
            reportDataSource1.Name = "PurchaseInvoice"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value =dt; 
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PurchaseInvoice.rdlc";

            PurchaseDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        public void Filter(object sender, RoutedEventArgs e)
        {
      
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
           
            reportDataSource1.Name = "PurchaseInvoice"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PurchaseInvoice.rdlc";

         

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
