using System.Data;
using System.Windows;
using System.Windows.Controls;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class ProductionOrder : Page
    {
        public ProductionOrder()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
          
                
            DataTable dt = new DataTable();
            Class.Production Production = new Class.Production();
            
            dt = Production.Get_Production(CurrentSession.Id_Company, ReportPanel.StartDate, ReportPanel.EndDate);
          
                ReportPanel.ReportDt = dt;
           
            reportDataSource1.Name = "ProductionOrder"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.ProductionOrder.rdlc";

     

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }

        public void Filter(object sender, RoutedEventArgs e)
        {
        
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
          
            reportDataSource1.Name = "ProductionOrder"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.ProductionOrder.rdlc";

           
            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
    }
}
