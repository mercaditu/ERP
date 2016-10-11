using System;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    /// <summary>
    /// Interaction logic for SalesByTag.xaml
    /// </summary>
    public partial class Movement : Page
    {
        public Movement()
        {
            InitializeComponent();
            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            Class.StockCalculations Stock = new Class.StockCalculations();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

            reportDataSource1.Name = "TransferSummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = Stock.TransferSummary(ReportPanel.StartDate, ReportPanel.EndDate);
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.TransferSummary.rdlc";

            this.reportViewer.RefreshReport();
        }
    }
}
