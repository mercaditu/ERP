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
    public partial class Sales_Report : Page
    {
        ReportPage ReportPage = null; // Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        db db = new db();

        public Sales_Report()
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
            List<sales_invoice_detail> sales_invoice_detail = db.sales_invoice_detail.ToList();
            reportDataSource.Value = sales_invoice_detail.Select(g => new
            {
                geo_name = g.sales_invoice != null ? g.sales_invoice.contact.app_geography != null ? g.sales_invoice.contact.app_geography.name : "" : "",
                sales_invoice = g.sales_invoice != null ? g.sales_invoice.id_sales_invoice : 0,
                id_company = g.id_company,
                add1 = g.sales_invoice != null ? g.sales_invoice.contact.address != null ? g.sales_invoice.contact.address : "" : "",
                telephone = g.sales_invoice != null ? g.sales_invoice.contact.telephone != null ? g.sales_invoice.contact.telephone : "" : "",
                email = g.sales_invoice != null ? g.sales_invoice.contact.email != null ? g.sales_invoice.contact.email : "" : "",
                company_name = g.sales_invoice != null ? g.sales_invoice.app_company != null ? g.sales_invoice.app_company.name : "" : "",
                item_code = g.item != null ? g.item.code : "",
                item_name = g.item != null ? g.item.name : "",
                item_description = g.item_description,
                Description = g.item != null ? g.item.item_brand != null ? g.item.item_brand.name : "" : "",
                currency = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.app_currency.name : "",
                currencyfx_rate = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.sell_value : 0,
                quantity = g.quantity,
                sub_Total = g.SubTotal,
                sub_Total_vat = g.SubTotal_Vat,
                sub_Total_Vat_Discount = g.Discount_SubTotal_Vat,
                unit_cost = g.unit_cost,
                unit_price = g.unit_price,
                unit_price_vat = g.UnitPrice_Vat,
                terminal_name = g.sales_invoice != null ? g.sales_invoice.app_terminal != null ? g.sales_invoice.app_terminal.name : "" : "",
                code = g.sales_invoice != null ? g.sales_invoice.code != null ? g.sales_invoice.code : "" : "",
                customer_contact_name = g.sales_invoice != null ? g.sales_invoice.contact.name : "",
                customer_code = g.sales_invoice != null ? g.sales_invoice.contact.code : "",
                customer_alias = g.sales_invoice != null ? g.sales_invoice.contact.alias : "",
                project_name = g.sales_invoice != null ? g.sales_invoice.project != null ? g.sales_invoice.project.name : "" : "",
                sales_invoice_rep_name = g.sales_invoice != null ? g.sales_invoice.sales_rep != null ? g.sales_invoice.sales_rep.name : "" : "",
                trans_date = g.sales_invoice != null ? g.sales_invoice.trans_date.ToString() : "",
                id_vat_group = g.id_vat_group,
                gov_id = g.sales_invoice != null ? g.sales_invoice.contact.gov_code : "",
                sales_invoice_contract = g.sales_invoice != null ? g.sales_invoice.app_contract.name : "",
                sales_invoice_condition = g.sales_invoice != null ? g.sales_invoice.app_contract.app_condition.name : "",
                sales_number = g.sales_invoice != null ? g.sales_invoice.number : "",
                sales_invoice_Comment = g.sales_invoice != null ? g.sales_invoice.comment : "",
                sales_order = g.sales_invoice != null ? g.sales_order_detail != null ? g.sales_order_detail.sales_order.number : "" : "",
                HasRounding = g.sales_invoice != null ? g.sales_invoice.app_currencyfx != null ? g.sales_invoice.app_currencyfx.app_currency != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding : false : false : false : false,
                unit_price_discount = g.discount != null ? g.discount : 0,
            }).ToList();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + SubFolder);
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\SalesInvoiceReport.rdlc", path + SubFolder + "\\SalesInvoiceReport.rdlc");
            }
            else if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\SalesInvoiceReport.rdlc", path + SubFolder + "\\SalesInvoiceReport.rdlc");

            }
            else if (!File.Exists(path + SubFolder + "\\SalesInvoiceReport.rdlc"))
            {
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Report\\SalesInvoiceReport.rdlc", path + SubFolder + "\\SalesInvoiceReport.rdlc");
            }


            reportViewer.LocalReport.ReportPath = path + SubFolder + "\\SalesInvoiceReport.rdlc"; // Path of the rdlc file
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();



        }   
    }
}
