using entity;
using System;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    /// <summary>
    /// Interaction logic for SalesByItem.xaml
    /// </summary>
    public partial class PendingReceivables : Page
    {
        public PendingReceivables()
        {
            InitializeComponent();

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            
            //fill data
            DataTable dt = new DataTable();

            Class.Finance Finance = new Class.Finance();
            dt = Finance.PendingRecievables(ReportPanel.EndDate);

            ReportPanel.ReportDt = dt;
            

            reportDataSource1.Name = "PendingReceivables"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PendingReceivables.rdlc";

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
