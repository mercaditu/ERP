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
    public partial class PurchaseTender : Page
    {
        public PurchaseTender()
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

            Data.PurchaseDSTableAdapters.PurchaseTenderSummaryTableAdapter PurchaseTenderSummaryTableAdapter = new Data.PurchaseDSTableAdapters.PurchaseTenderSummaryTableAdapter();
                
            DataTable dt = new DataTable();
            dt = PurchaseTenderSummaryTableAdapter.GetDataByDates(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);
            if (ReportPanel.ReportDt == null)
            {
                ReportPanel.ReportDt = dt;
            }
            reportDataSource1.Name = "PurchaseTenderSummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PurchaseTenderSummary.rdlc";

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
