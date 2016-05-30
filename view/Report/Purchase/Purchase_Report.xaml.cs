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
    public partial class Purchase_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public Purchase_Report()
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
            var predicate = PredicateBuilder.True<entity.purchase_invoice>();

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
            List<purchase_invoice> purchase_invoiceList = db.purchase_invoice.Where(predicate).ToList();
            reportDataSource.Value = purchase_invoiceList
                .Join(db.purchase_invoice_detail, u => u.id_purchase_invoice, sid => sid.id_purchase_invoice, (purchase_invoice, pid) => new { purchase_invoice, pid }).Select(g => new
            {
                id_purchase_invoice = g.purchase_invoice != null ? g.purchase_invoice.id_purchase_invoice : 0,
                geo_name = g.purchase_invoice != null ? g.purchase_invoice.contact.app_geography != null ? g.purchase_invoice.contact.app_geography.name : "" : "",
                id_company = g.pid.id_company,
                add1 = g.purchase_invoice != null ? g.purchase_invoice.contact.address != null ? g.purchase_invoice.contact.address : "" : "",
                telephone = g.purchase_invoice != null ? g.purchase_invoice.contact.telephone != null ? g.purchase_invoice.contact.telephone : "" : "",
                email = g.purchase_invoice != null ? g.purchase_invoice.contact.email != null ? g.purchase_invoice.contact.email : "" : "",
                company_name = g.purchase_invoice != null ? g.purchase_invoice.app_company != null ? g.purchase_invoice.app_company.name : "" : "",
                item_code = g.pid.item != null ? g.pid.item.code : "",
                item_name = g.pid.item != null ? g.pid.item.name : "",
                item_description = g.pid.item_description,
                item_type = g.pid.item != null ? g.pid.item.id_item_type.ToString() : "",
                Description = g.pid.item != null ? g.pid.item.item_brand != null ? g.pid.item.item_brand.name : "" : "",
                currency = g.purchase_invoice != null ? g.purchase_invoice.app_currencyfx.app_currency.name : "",
                currencyfx_rate = g.purchase_invoice != null ? g.purchase_invoice.app_currencyfx.sell_value : 0,
                quantity = g.pid.quantity,
                sub_Total = g.pid.SubTotal,
                sub_Total_vat = g.pid.SubTotal_Vat,
                sub_Total_Vat_Discount = g.pid.Discount_SubTotal_Vat,
               unit_price = g.pid.unit_cost,
                unit_price_vat = g.pid.unit_cost,
                terminal_name = g.purchase_invoice != null ? g.purchase_invoice.app_terminal != null ? g.purchase_invoice.app_terminal.name : "" : "",
                code = g.purchase_invoice != null ? g.purchase_invoice.code != null ? g.purchase_invoice.code : "" : "",
                customer_contact_name = g.purchase_invoice != null ? g.purchase_invoice.contact.name : "",
                customer_code = g.purchase_invoice != null ? g.purchase_invoice.contact.code : "",
                customer_alias = g.purchase_invoice != null ? g.purchase_invoice.contact.alias : "",
                project_name = g.purchase_invoice != null ? g.purchase_invoice.project != null ? g.purchase_invoice.project.name : "" : "",
                purchase_invoice_rep_name = g.purchase_invoice != null ? g.purchase_invoice.sales_rep != null ? g.purchase_invoice.sales_rep.name : "" : "",
                trans_date = g.purchase_invoice != null ? g.purchase_invoice.trans_date.ToString() : "",
                id_vat_group = g.pid.id_vat_group,
                gov_id = g.purchase_invoice != null ? g.purchase_invoice.contact.gov_code : "",
                purchase_invoice_contract = g.purchase_invoice != null ? g.purchase_invoice.app_contract.name : "",
                purchase_invoice_condition = g.purchase_invoice != null ? g.purchase_invoice.app_contract.app_condition.name : "",
                purchase_number = g.purchase_invoice != null ? g.purchase_invoice.number : "",
                purchase_invoice_Comment = g.purchase_invoice != null ? g.purchase_invoice.comment : "",
                purchase_order = g.purchase_invoice != null ? g.pid.purchase_order_detail != null ? g.pid.purchase_order_detail.purchase_order.number : "" : "",
                HasRounding = g.purchase_invoice != null ? g.purchase_invoice.app_currencyfx != null ? g.purchase_invoice.app_currencyfx.app_currency != null ? g.purchase_invoice.app_currencyfx.app_currency.has_rounding != null ? g.purchase_invoice.app_currencyfx.app_currency.has_rounding : false : false : false : false,
                unit_price_discount = g.pid.discount != null ? g.pid.discount : 0,
            }).ToList();

            reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\PurchaseInvoiceReport.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }   
    }
}
