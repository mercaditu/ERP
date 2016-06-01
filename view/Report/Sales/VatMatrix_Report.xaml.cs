using cntrl.Controls;
using entity;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Cognitivo.Report
{
    public partial class VatMatrix_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public VatMatrix_Report()
        {
            InitializeComponent();
        }

        private void rpt_Loaded(object sender, RoutedEventArgs e)
        {
            ReportPage = Application.Current.Windows.OfType<ReportPage>().FirstOrDefault() as ReportPage;

            QueryBuilder();
        }

        private void QueryBuilder()
        {
            var predicate = PredicateBuilder.True<entity.sales_invoice>();

           

            //if (ReportPage.start_Range != Convert.ToDateTime("1/1/0001"))
            //{
            //    predicate = predicate.And(x => x.trans_date >= ReportPage.start_Range);

            //}
            //if (ReportPage.end_Range != Convert.ToDateTime("1/1/0001"))
            //{
            //    predicate = predicate.And(x => x.trans_date <= ReportPage.end_Range);

            //}
         

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<sales_invoice> sales_invoiceList = db.sales_invoice.Where(predicate).ToList();
            var salesinvoiceList = sales_invoiceList.Where(x => x.trans_date.Date >= ReportPage.start_Range && x.trans_date <= ReportPage.end_Range)
                  .Join(db.sales_invoice_detail, u => u.id_sales_invoice, sid => sid.id_sales_invoice, (sales_invoice, sid) => new { sales_invoice, sid }).Select(g => new
              {
                  id_branch = g.sales_invoice != null ? g.sales_invoice.id_branch : 0,
                  branch_name = g.sales_invoice != null ? g.sales_invoice.app_branch.name : "",
                  trans_date = g.sales_invoice != null ? g.sales_invoice.trans_date.ToString() : "",
                  sales_number = g.sales_invoice != null ? g.sales_invoice.number : "",
                  customer_contact_name = g.sales_invoice != null ? g.sales_invoice.contact.name : "",
                  gov_id = g.sales_invoice != null ? g.sales_invoice.contact.gov_code : "",
                  vat_group = g.sid.app_vat_group.name,
                  value=g.sid.SubTotal_Vat-g.sid.SubTotal
              }).ToList();

         

            reportDataSource.Value = salesinvoiceList;
            reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\MatrixVat.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }
   
    }
}
