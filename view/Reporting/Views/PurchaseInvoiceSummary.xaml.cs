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
    public partial class PurchaseInvoiceSummary : Page
    {
        public PurchaseInvoiceSummary()
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

            Data.PurchaseDSTableAdapters.PurchaseInvoiceSummaryTableAdapter PurchaseInvoiceSummaryTableAdapter = new Data.PurchaseDSTableAdapters.PurchaseInvoiceSummaryTableAdapter();
                
            DataTable dt = new DataTable();

            if (ReportPanel.Branch != null)
            {
                dt = PurchaseInvoiceSummaryTableAdapter.GetDataByBranch(ReportPanel.StartDate, ReportPanel.EndDate, ReportPanel.Branch.id_branch);
            }
            else
            {
                dt = PurchaseInvoiceSummaryTableAdapter.GetDataBy(ReportPanel.StartDate, ReportPanel.EndDate);
            }

            //ReportParameter[] parameters = new ReportParameter[x+1];

            reportDataSource1.Name = "PurchaseInvoiceSummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PurchaseInvoiceSummary.rdlc";
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //parameters[0] = new ReportParameter("name1", value1);
            //this.reportViewer.LocalReport.SetParameters("EndDate", dtEndDate, false);

            PurchaseDB.EndInit();

            this.reportViewer.Refresh();
            this.reportViewer.RefreshReport();
        }

        private void rptPanel_Update(object sender, RoutedEventArgs e)
        {
            Fill(null, null);
        }
    }
}
