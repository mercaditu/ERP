using System;
using System.Collections.Generic;
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

namespace Cognitivo.Accounting
{
    /// <summary>
    /// Loads ERP Data and serializes into Json for DebeHaber.
    /// </summary>
    public partial class DebeHaberSync : Page
    {
        CollectionViewSource sales_invoiceViewSource;
        CollectionViewSource sales_returnViewSource;
        CollectionViewSource purchase_invoiceViewSource;
        CollectionViewSource purchase_returnViewSource;
        CollectionViewSource paymentViewSource;

        entity.db db = new entity.db();

        public DebeHaberSync()
        {
            InitializeComponent();

            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_returnViewSource = ((CollectionViewSource)(FindResource("sales_returnViewSource")));
            purchase_invoiceViewSource = ((CollectionViewSource)(FindResource("purchase_invoiceViewSource")));
            purchase_returnViewSource = ((CollectionViewSource)(FindResource("purchase_returnViewSource")));
            paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
        }

        private void btnData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_SalesInvoice();
        }

        public void Get_SalesInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_invoiceViewSource.Source = db.sales_invoice.Where(x => x.id_company == entity.CurrentSession.Id_Company && x.is_head).ToList();
        }

        private void btnData_Sync(object sender, RoutedEventArgs e)
        {
            SalesInvoice_Sync();
        }

        private void SalesInvoice_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> SalesInvoiceLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            foreach (entity.sales_invoice sales_invoice in db.sales_invoice.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Commercial_Invoice Invoice = new entity.DebeHaber.Commercial_Invoice();

                Invoice.Reference_ID = sales_invoice.id_sales_invoice;
                Invoice.Type = entity.DebeHaber.TransactionTypes.Sales;
                Invoice.BranchCode = sales_invoice.app_branch.code;
                Invoice.BranchName = sales_invoice.app_branch.name;
                Invoice.Comment = sales_invoice.comment;
                Invoice.Contact_GovCode = sales_invoice.contact.gov_code;
                Invoice.CurrencyISO_Code = sales_invoice.app_currencyfx.app_currency.name;
                Invoice.GrandTotal = sales_invoice.GrandTotal;
                Invoice.PaymentCondition = sales_invoice.app_contract.app_contract_detail.Sum(x => x.interval);
                
                Invoice.InvoiceCode = sales_invoice.app_document_range != null ? sales_invoice.app_document_range.code : "NA";
                Invoice.InvoiceCode_ExpDate = (sales_invoice.app_document_range != null ? (DateTime)sales_invoice.app_document_range.expire_date : DateTime.Now);
                
                Invoice.InvoiceNumber = sales_invoice.number;
                Invoice.InvoiceDate = sales_invoice.trans_date;

                ///Loop through details.
                foreach (entity.sales_invoice_detail Detail in sales_invoice.sales_invoice_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    CommercialInvoice_Detail.VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
                    CommercialInvoice_Detail.Value = Detail.SubTotal_Vat;
                    CommercialInvoice_Detail.Comment = Detail.item_description;

                    Invoice.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in sales_invoice.payment_schedual.Where(x => x.id_payment_detail > 0))
                {
                    entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                    Invoice.Payments.Add(Payments);
                }

                SalesInvoiceLIST.Add(Invoice);

                ///Serealize SalesInvoiceLIST into Json and send it to Server Address specified.
            }
        }
    }
}
