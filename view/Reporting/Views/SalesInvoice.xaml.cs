using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using entity;
using Microsoft.Reporting.WinForms;

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
            //cntrl.ReportVariation One = new cntrl.ReportVariation
            //{
            //    Name = "Sales",
            //    ID = "SalesInvoiceSummary"
            //};

         //   ReportPanel.Reports.Add(One);

            this.reportViewer.Reset();

            ReportDataSource reportDataSource1 = new ReportDataSource();
            Data.SalesDS SalesDB = new Data.SalesDS();

            SalesDB.BeginInit();

            Data.SalesDSTableAdapters.SalesInvoiceSummaryTableAdapter SalesInvoiceSummaryTableAdapter = new Data.SalesDSTableAdapters.SalesInvoiceSummaryTableAdapter();

            DataTable dt = new DataTable();

            dt = SalesInvoiceSummaryTableAdapter.GetDataBy(ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);

         
                ReportPanel.ReportDt = dt;
           
        
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

            reportViewer.LocalReport.DataSources.Add(reportDataSource1);

            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesInvoiceSummary.rdlc";

            foreach (cntrl.ReportColumns ReportColumns in ReportColumnsList)
            {

                if (ReportColumns.Columname == "VAT_SubTotal")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVAT", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "Profit")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowProfit", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "MarkUp")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMarkup", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "Margin")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMargin", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "SubTotalCost")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowCost", ReportColumns.IsVisibility.ToString()));

                }
                if (ReportColumns.Columname == "SubTotalVAT")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVATTotal", ReportColumns.IsVisibility.ToString()));
                }




            }
            SalesDB.EndInit();

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
        public void Filter(object sender, RoutedEventArgs e)
        {
       
            this.reportViewer.Reset();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
           

     

            if (ReportColumnsList.Count() == 0)
            {
                foreach (DataColumn item in ReportPanel.Filterdt.Columns)
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
            reportDataSource1.Value = ReportPanel.Filterdt; //SalesDB.SalesByDate;

            reportViewer.LocalReport.DataSources.Add(reportDataSource1);

            reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesInvoiceSummary.rdlc";

            foreach (cntrl.ReportColumns ReportColumns in ReportColumnsList)
            {

                if (ReportColumns.Columname == "VAT_SubTotal")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVAT", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "Profit")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowProfit", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "MarkUp")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMarkup", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "Margin")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMargin", ReportColumns.IsVisibility.ToString()));
                }
                if (ReportColumns.Columname == "SubTotalCost")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowCost", ReportColumns.IsVisibility.ToString()));

                }
                if (ReportColumns.Columname == "SubTotalVAT")
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVATTotal", ReportColumns.IsVisibility.ToString()));
                }




            }
        

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }



    }
}
