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
using Microsoft.Reporting.WinForms;
using System.Xml;
using System.IO;

namespace Cognitivo.Reporting.Views
{
    public partial class SalesInvoice : Page
    {
        List<cntrl.ReportColumns> ReportColumnsList = new List<cntrl.ReportColumns>();
        public SalesInvoice()
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
            if (ReportColumnsList.Count() == 0)
            {
                foreach (DataColumn item in dt.Columns)
                {
                    if (ReportColumnsList.Where(x => x.Columname == item.ColumnName).Count() == 0)
                    {
                        if (item.ColumnName == "VAT_SubTotal" ||
                        item.ColumnName == "Profit" ||
                        item.ColumnName == "MarkUp" ||
                        item.ColumnName == "Margin" ||
                        item.ColumnName == "SubTotalCost" ||
                        item.ColumnName == "SubTotalVAT")
                        {
                            cntrl.ReportColumns ReportColumns = new cntrl.ReportColumns();
                            ReportColumns.Columname = item.ColumnName;
                            ReportColumns.IsVisibility = true;
                            ReportColumnsList.Add(ReportColumns);
                        }
                    }


                }

                ReportPanel.ReportColumn = ReportColumnsList;
            }






            reportDataSource1.Name = "SalesInvoiceSummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;

            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);

            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesInvoiceSummary.rdlc";

            foreach (cntrl.ReportColumns ReportColumns in ReportColumnsList)
            {

                if (ReportColumns.Columname == "VAT_SubTotal")
                {
                    this.reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVAT", ReportColumns.IsVisibility == null ? true.ToString() : ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "Profit")
                {
                    this.reportViewer.LocalReport.SetParameters(new ReportParameter("ShowProfit", ReportColumns.IsVisibility == null ? true.ToString() : ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "MarkUp")
                {
                    this.reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMarkup", ReportColumns.IsVisibility == null ? true.ToString() : ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "Margin")
                {
                    this.reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMargin", ReportColumns.IsVisibility == null ? true.ToString() : ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "SubTotalCost")
                {
                    this.reportViewer.LocalReport.SetParameters(new ReportParameter("ShowCost", ReportColumns.IsVisibility == null ? true.ToString() : ReportColumns.IsVisibility.ToString()));

                }
                if (ReportColumns.Columname == "SubTotalVAT")
                {
                    this.reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVATTotal", ReportColumns.IsVisibility == null ? true.ToString() : ReportColumns.IsVisibility.ToString()));
                }




            }
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
