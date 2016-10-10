using System;
using System.Data;
using System.Windows.Controls;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class HR_Used : Page
    {
        public HR_Used()
        {
            InitializeComponent();
           
            
            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.ProductionDS ProductionDS = new Data.ProductionDS();

            ProductionDS.BeginInit();

            Data.ProductionDSTableAdapters.EmployeesInProductionTableAdapter EmployeesInProductionTableAdapter = new Data.ProductionDSTableAdapters.EmployeesInProductionTableAdapter();
            DataTable dt = EmployeesInProductionTableAdapter.GetData(CurrentSession.Id_Company, ReportPanel.StartDate, ReportPanel.EndDate);
            if (ReportPanel.ReportDt == null)
            {
                ReportPanel.ReportDt = dt;
            }
            reportDataSource1.Name = "ProductionEmployee";
            reportDataSource1.Value = ReportPanel.Filterdt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.ProductionEmployee.rdlc";

            ProductionDS.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();  
        }
    }
}
