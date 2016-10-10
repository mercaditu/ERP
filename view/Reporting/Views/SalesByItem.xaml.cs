using entity;
using System;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
    /// <summary>
    /// Interaction logic for SalesByItem.xaml
    /// </summary>
    public partial class SalesByItem : Page
    {
        public SalesByItem()
        {
            InitializeComponent();

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.SalesDS SalesDB = new Data.SalesDS();

            SalesDB.BeginInit();

            Data.SalesDSTableAdapters.SalesByItemTableAdapter SalesByItemTableAdapter = new Data.SalesDSTableAdapters.SalesByItemTableAdapter();

            //fill data
            SalesByItemTableAdapter.ClearBeforeFill = true;
            DataTable dt = new DataTable();

            //if (ReportPanel.Branch != null)
            //{
            //    dt = SalesByItemTableAdapter.GetDataByBranch(ReportPanel.StartDate, ReportPanel.EndDate, ReportPanel.Branch.id_branch, CurrentSession.Id_Company);
            //}
            //else
            //{
                dt = SalesByItemTableAdapter.GetDataByGeneral(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
            //}

            //string where = string.Empty;

            //if (ReportPanel.CustomerID > 0)
            //{
            //    where = where + "CustomerID = " + ReportPanel.CustomerID;
            //}

            //if (ReportPanel.CustomerID > 0 && ReportPanel.Branch != null)
            //{
            //    where = where + " and ";
            //}

            //if (ReportPanel.Branch != null)
            //{
            //    where = where + "BranchID = " + ReportPanel.Branch.id_branch;
            //}

            if (ReportPanel.ReportDt == null)
            {
                ReportPanel.ReportDt = dt;
            }

            reportDataSource1.Name = "SalesByItem"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByItem.rdlc";

            SalesDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
