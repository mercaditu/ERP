using System.Data;
using System.Windows;
using System.Windows.Controls;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class ProductionOrderStatus : Page
    {
        public ProductionOrderStatus()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.ProductDS ProductionDB = new Data.ProductDS();

            ProductionDB.BeginInit();

            Data.ProductionDSTableAdapters.ProductionStatusTableAdapter ProductionStatusTableAdapter = new Data.ProductionDSTableAdapters.ProductionStatusTableAdapter();
                
            DataTable dt = new DataTable();

            //if (ReportPanel.Branch != null)
            //{
            //    dt = ProductionStatusTableAdapter.GetDataByBranch(ReportPanel.StartDate, ReportPanel.EndDate, ReportPanel.Branch.id_branch,CurrentSession.Id_Company);
            //}
            //else
            //{
                dt = ProductionStatusTableAdapter.GetDataBy(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
            //}
            if (ReportPanel.ReportDt == null)
            {
                ReportPanel.ReportDt = dt;
            }
            reportDataSource1.Name = "ProductionStatus"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.ProductionStatus.rdlc";

            ProductionDB.EndInit();

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }

        private void rptPanel_Update(object sender, RoutedEventArgs e)
        {
            Fill(null, null);
        }
    }
}
