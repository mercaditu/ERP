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
    public partial class SalesInvoiceSummary : Page
    {
        public SalesInvoiceSummary()
        {
            InitializeComponent();            
            Fill(null, null);
        }

        public void Fill(object sender, RoutedEventArgs e)
        {
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Data.SalesDS SalesDB = new Data.SalesDS();

            SalesDB.BeginInit();

            Data.SalesDSTableAdapters.SalesInvoiceSummaryTableAdapter SalesInvoiceSummaryTableAdapter = new Data.SalesDSTableAdapters.SalesInvoiceSummaryTableAdapter();
                
            DataTable dt = new DataTable();

            if (ReportPanel.Branch != null)
            {
                dt = SalesInvoiceSummaryTableAdapter.GetDataByBranch(ReportPanel.StartDate, ReportPanel.EndDate, ReportPanel.Branch.id_branch);
            }
            else
            {
                dt = SalesInvoiceSummaryTableAdapter.GetDataBy(ReportPanel.StartDate, ReportPanel.EndDate);
            }

            reportDataSource1.Name = "SalesInvoiceSummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesInvoiceSummary.rdlc";

            SalesDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        private void rptPanel_Update(object sender, RoutedEventArgs e)
        {
            Fill(null, null);
        }
    }
}
