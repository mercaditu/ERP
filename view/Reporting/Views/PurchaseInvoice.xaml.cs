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

            if (ReportPanel.SupplierID > 0)
            {
                dt = PurchaseInvoiceSummaryTableAdapter.GetDataBySupplier(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company, ReportPanel.SupplierID);
            }
            else
            {
                dt = PurchaseInvoiceSummaryTableAdapter.GetDataByDate(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
            }

            reportDataSource1.Name = "PurchaseInvoice"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; 
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PurchaseInvoice.rdlc";

            PurchaseDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        private void rptPanel_Update(object sender, RoutedEventArgs e)
        {
            Fill(null, null);
        }
    }
}
