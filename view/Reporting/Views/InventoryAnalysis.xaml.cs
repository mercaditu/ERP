using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    public partial class InventoryAnalysis : Page
    {
        public InventoryAnalysis()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            DataTable dt = new DataTable();

            Class.StockCalculations Stock = new Class.StockCalculations();
            dt = Stock.Inventory_Analysis(ReportPanel.StartDate, ReportPanel.EndDate);

            ReportPanel.ReportDt = dt;
          
            reportDataSource1.Name = "InventoryAnalysis"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.InventoryAnalysis.rdlc";
            
            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        public void Filter(object sender, RoutedEventArgs e)
        {
         
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
         
            reportDataSource1.Name = "InventoryAnalysis"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.InventoryAnalysis.rdlc";

         

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
     
    }
}
