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
    public partial class SalesBySalesman_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public SalesBySalesman_Report()
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

            if (ReportPage.ConditionArray != null)
            {
                if (ReportPage.ConditionArray.Count() > 0)
                {
                    predicate = predicate.And(x => ReportPage.ConditionArray.Contains(x.app_condition.name));
                }
            }

            if (ReportPage.ContractArray != null)
            {
                if (ReportPage.ContractArray.Count() > 0)
                {
                    predicate = predicate.And(x => ReportPage.ContractArray.Contains(x.app_contract.name));
                }
            }

            if (ReportPage.start_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date >= ReportPage.start_Range);

            }
            if (ReportPage.end_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date <= ReportPage.end_Range);

            }
            if (ReportPage.Contact != null)
            {
                predicate = predicate.And(x => x.contact == ReportPage.Contact);
            }
           
            
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<sales_invoice> sales_invoiceList = db.sales_invoice.Where(predicate).ToList();
            reportDataSource.Value = sales_invoiceList
            .Join(db.sales_invoice_detail, u => u.id_sales_invoice, sid => sid.id_sales_invoice, (sales_invoice, sid) => new { sales_invoice, sid }).Select(g => new
            {
                id_sales_invoice = g.sales_invoice != null ? g.sales_invoice.id_sales_invoice : 0,
                geo_name = g.sales_invoice != null ? g.sales_invoice.contact.app_geography != null ? g.sales_invoice.contact.app_geography.name : "" : "",
                sales_invoice = g.sales_invoice != null ? g.sales_invoice.id_sales_invoice : 0,
                id_company = g.sid.id_company,
                add1 = g.sales_invoice != null ? g.sales_invoice.contact.address != null ? g.sales_invoice.contact.address : "" : "",
                telephone = g.sales_invoice != null ? g.sales_invoice.contact.telephone != null ? g.sales_invoice.contact.telephone : "" : "",
                email = g.sales_invoice != null ? g.sales_invoice.contact.email != null ? g.sales_invoice.contact.email : "" : "",
                company_name = g.sales_invoice != null ? g.sales_invoice.app_company != null ? g.sales_invoice.app_company.name : "" : "",
                item_code = g.sid.item != null ? g.sid.item.code : "",
                item_name = g.sid.item != null ? g.sid.item.name : "",
                item_description = g.sid.item_description,
                id_item = g.sid.id_item,
                item_type = g.sid.item != null ? g.sid.item.id_item_type.ToString() : "",
                Description = g.sid.item != null ? g.sid.item.item_brand != null ? g.sid.item.item_brand.name : "" : "",
                currency = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.app_currency.name : "",
                currencyfx_rate = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.sell_value : 0,
                quantity = g.sid.quantity,
                sub_Total = g.sid.SubTotal,
                sub_Total_vat = g.sid.SubTotal_Vat,
                sub_Total_Vat_Discount = g.sid.Discount_SubTotal_Vat,
                unit_cost = g.sid.unit_cost,
                unit_price = g.sid.unit_price,
                unit_price_vat = g.sid.UnitPrice_Vat,
                terminal_name = g.sales_invoice != null ? g.sales_invoice.app_terminal != null ? g.sales_invoice.app_terminal.name : "" : "",
                code = g.sales_invoice != null ? g.sales_invoice.code != null ? g.sales_invoice.code : "" : "",
                customer_contact_name = g.sales_invoice != null ? g.sales_invoice.contact.name : "",
                customer_code = g.sales_invoice != null ? g.sales_invoice.contact.code : "",
                customer_alias = g.sales_invoice != null ? g.sales_invoice.contact.alias : "",
                project_name = g.sales_invoice != null ? g.sales_invoice.project != null ? g.sales_invoice.project.name : "" : "",
                id_sales_rep = g.sales_invoice != null ? g.sales_invoice.id_sales_rep != null ? g.sales_invoice.id_sales_rep : 0 : 0,
                sales_invoice_rep_name = g.sales_invoice != null ? g.sales_invoice.sales_rep != null ? g.sales_invoice.sales_rep.name : "" : "",
                trans_date = g.sales_invoice != null ? g.sales_invoice.trans_date.ToString() : "",
                id_vat_group = g.sid.id_vat_group,
                gov_id = g.sales_invoice != null ? g.sales_invoice.contact.gov_code : "",
                sales_invoice_contract = g.sales_invoice != null ? g.sales_invoice.app_contract.name : "",
                sales_invoice_condition = g.sales_invoice != null ? g.sales_invoice.app_contract.app_condition.name : "",
                sales_number = g.sales_invoice != null ? g.sales_invoice.number : "",
                sales_invoice_Comment = g.sales_invoice != null ? g.sales_invoice.comment : "",
                sales_order = g.sales_invoice != null ? g.sid.sales_order_detail != null ? g.sid.sales_order_detail.sales_order.number : "" : "",
                HasRounding = g.sales_invoice != null ? g.sales_invoice.app_currencyfx != null ? g.sales_invoice.app_currencyfx.app_currency != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding : false : false : false : false,
                unit_price_discount = g.sid.discount != null ? g.sid.discount : 0,
            }).ToList();

         

            reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\SalesInvoicebySalesMan.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }   
    }
}
