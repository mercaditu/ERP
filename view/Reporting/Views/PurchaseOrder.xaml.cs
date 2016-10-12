using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using entity;
using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;

namespace Cognitivo.Reporting.Views
{
    public partial class PurchaseOrder : Page
    {
        public PurchaseOrder()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.PurchaseDS PurchaseDB = new Data.PurchaseDS();

            PurchaseDB.BeginInit();

            Data.PurchaseDSTableAdapters.PurchaseOrderSummaryTableAdapter PurchaseOrderSummaryTableAdapter = new Data.PurchaseDSTableAdapters.PurchaseOrderSummaryTableAdapter();
                
            DataTable dt = new DataTable();

            //if (ReportPanel.SupplierID > 0)
            //{
            //    dt = PurchaseOrderSummaryTableAdapter.GetDataByBranch(ReportPanel.StartDate, ReportPanel.EndDate, ReportPanel.Branch.id_branch);
            //}
            //else
            //{
                dt = PurchaseOrderSummaryTableAdapter.GetDataBy(ReportPanel.StartDate, ReportPanel.EndDate);
            //}
         
                ReportPanel.ReportDt = dt;
            

            reportDataSource1.Name = "PurchaseOrderSummary";
            reportDataSource1.Value = dt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PurchaseOrderSummary.rdlc";

            PurchaseDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        public void Filter(object sender, RoutedEventArgs e)
        {
          
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
         
            reportDataSource1.Name = "PurchaseOrderSummary";
            reportDataSource1.Value = ReportPanel.Filterdt;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PurchaseOrderSummary.rdlc";

           

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }
    }
}
