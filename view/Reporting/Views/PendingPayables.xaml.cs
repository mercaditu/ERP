using System;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    /// <summary>
    /// Interaction logic for SalesByItem.xaml
    /// </summary>
    public partial class PendingPayables : Page
    {
        public PendingPayables()
        {
            InitializeComponent();

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            
            //fill data
            DataTable dt = new DataTable();

            Class.Finance Finance = new Class.Finance();
            dt = Finance.PendingPayables(ReportPanel.EndDate);
            
            ReportPanel.ReportDt = dt;

            reportDataSource1.Name = "PendingReceivables"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PendingReceivables.rdlc";
            reportViewer.LocalReport.DisplayName = entity.Brillo.Localize.StringText("PendingPayable");

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
        public void Filter(object sender, EventArgs e)
        {
          
            reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
           
            reportDataSource1.Name = "SalesByItem"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByItem.rdlc";
 

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
    }
}
