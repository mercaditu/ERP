using System.Data;
using System.Windows;
using System.Windows.Controls;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class MerchandiseExit : Page
    {
        public MerchandiseExit()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            //Data.ProductDS ProductDS = new Data.ProductDS();
                
            DataTable dt = new DataTable();

            Class.StockCalculations Stock = new Class.StockCalculations();
            dt = Stock.MerchandiseExit(ReportPanel.StartDate, ReportPanel.EndDate);

            ReportPanel.ReportDt = dt;
          
            reportDataSource1.Name = "MerchandiseExit"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.MerchandiseExit.rdlc";

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        public void Filter(object sender, RoutedEventArgs e)
        {
         
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
         
            reportDataSource1.Name = "MerchandiseEntry"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.MerchandiseEntry.rdlc";

         

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
     
    }
}
