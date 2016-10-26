using System;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    public partial class CostBreakDown : Page
    {
        public CostBreakDown()
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

            Class.StockCalculations Stock = new Class.StockCalculations();
            dt = Stock.CostBreakDown(ReportPanel.StartDate, ReportPanel.EndDate);
            
            ReportPanel.ReportDt = dt;

            reportDataSource1.Name = "CostBreakDown"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.CostBreakDown.rdlc";
            reportViewer.LocalReport.DisplayName = entity.Brillo.Localize.StringText("CostBreakDown");

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
        public void Filter(object sender, EventArgs e)
        {
            reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
           
            reportDataSource1.Name = "CostBreakDown"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.CostBreakDown.rdlc";

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
    }
}
