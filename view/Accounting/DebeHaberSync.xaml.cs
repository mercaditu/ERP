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
using System.Web.Script.Serialization;
using System.IO;

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
            Get_PurchaseInvoice();
            Get_PurchaseReturnInvoice();
            Get_SalesReturn();
            Get_Payment();

        }

        public void Get_SalesInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_invoiceViewSource.Source = db.sales_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }
        public void Get_Payment()
        {
            //x.Is Head replace with Is_Accounted = True.
            paymentViewSource.Source = db.payments.Where(x =>
                 x.id_company == entity.CurrentSession.Id_Company &&
                 x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_SalesReturn()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_returnViewSource.Source = db.sales_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_PurchaseReturnInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_returnViewSource.Source = db.purchase_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_PurchaseInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_invoiceViewSource.Source = db.purchase_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }

        private void btnData_Sync(object sender, RoutedEventArgs e)
        {
            SalesInvoice_Sync();
            PurchaseInvoice_Sync();
            SalesReturn_Sync();
            PurchaseReturn_Sync();
            Payment_Sync();
        }


        private void Payment_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> PaymentLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            foreach (entity.payment payment in db.payments.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Commercial_Invoice Invoice = new entity.DebeHaber.Commercial_Invoice();

                Invoice.ID = payment.id_payment;
                Invoice.Type = entity.DebeHaber.TransactionTypes.Sales;
                Invoice.BranchCode = payment.app_branch.code;
                Invoice.BranchName = payment.app_branch.name;
              //  Invoice.Comment = payment.comm;
                Invoice.Contact_GovCode = payment.contact.gov_code;
               // Invoice.CurrencyISO_Code = payment..app_currency.name;
                Invoice.InvoiceTotal = payment.GrandTotal;
              //  Invoice.PaymentCondition = payment.app_contract.app_contract_detail.Sum(x => x.interval);

                Invoice.InvoiceCode = payment.app_document_range != null ? payment.app_document_range.code : "NA";
                Invoice.InvoiceCode_ExpDate = (payment.app_document_range != null ? (DateTime)payment.app_document_range.expire_date : DateTime.Now);

                Invoice.InvoiceNumber = payment.number;
                Invoice.InvoiceDate = payment.trans_date;

                ///Loop through details.
                //foreach (entity.payment_detail Detail in payment.payment_detail)
                //{
                //    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                //    CommercialInvoice_Detail.VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
                //    CommercialInvoice_Detail.Value = Detail.SubTotal_Vat;
                //    CommercialInvoice_Detail.Comment = Detail.item_description;

                //    Invoice.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                //}

                //Loop through payments made.
                //foreach (entity.payment_schedual schedual in payment.payment_schedual.Where(x => x.id_payment_detail > 0))
                //{
                //    entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                //    Invoice.Payments.Add(Payments);
                //}

                PaymentLIST.Add(Invoice);

                ///Serealize SalesInvoiceLIST into Json
                var Payment_Json = new JavaScriptSerializer().Serialize(PaymentLIST);

                file_create(Payment_Json as string, "Payment");
                //Send Sales_Json send it to Server Address specified.
            }
        }
        private void SalesInvoice_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> SalesInvoiceLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            foreach (entity.sales_invoice sales_invoice in db.sales_invoice.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Commercial_Invoice Invoice = new entity.DebeHaber.Commercial_Invoice();

                Invoice.ID = sales_invoice.id_sales_invoice;
                Invoice.Type = entity.DebeHaber.TransactionTypes.Sales;
                Invoice.BranchCode = sales_invoice.app_branch.code;
                Invoice.BranchName = sales_invoice.app_branch.name;
                Invoice.Comment = sales_invoice.comment;
                Invoice.Contact_GovCode = sales_invoice.contact.gov_code;
                Invoice.CurrencyISO_Code = sales_invoice.app_currencyfx.app_currency.name;
                Invoice.InvoiceTotal = sales_invoice.GrandTotal;
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

                ///Serealize SalesInvoiceLIST into Json
                var Sales_Json = new JavaScriptSerializer().Serialize(SalesInvoiceLIST);

                file_create(Sales_Json as string,"sales_invoice");
                //Send Sales_Json send it to Server Address specified.
            }
        }
        private void SalesReturn_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> SalesReturnLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            foreach (entity.sales_return sales_return in db.sales_return.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Commercial_Invoice Invoice = new entity.DebeHaber.Commercial_Invoice();

                Invoice.ID = sales_return.id_sales_return;
                Invoice.Type = entity.DebeHaber.TransactionTypes.Sales;
                Invoice.BranchCode = sales_return.app_branch.code;
                Invoice.BranchName = sales_return.app_branch.name;
                Invoice.Comment = sales_return.comment;
                Invoice.Contact_GovCode = sales_return.contact.gov_code;
                Invoice.CurrencyISO_Code = sales_return.app_currencyfx.app_currency.name;
                Invoice.InvoiceTotal = sales_return.GrandTotal;
                Invoice.PaymentCondition = sales_return.app_contract.app_contract_detail.Sum(x => x.interval);

                Invoice.InvoiceCode = sales_return.app_document_range != null ? sales_return.app_document_range.code : "NA";
                Invoice.InvoiceCode_ExpDate = (sales_return.app_document_range != null ? (DateTime)sales_return.app_document_range.expire_date : DateTime.Now);

                Invoice.InvoiceNumber = sales_return.number;
                Invoice.InvoiceDate = sales_return.trans_date;

                ///Loop through details.
                foreach (entity.sales_return_detail Detail in sales_return.sales_return_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    CommercialInvoice_Detail.VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
                    CommercialInvoice_Detail.Value = Detail.SubTotal_Vat;
                    CommercialInvoice_Detail.Comment = Detail.item_description;

                    Invoice.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in sales_return.payment_schedual.Where(x => x.id_payment_detail > 0))
                {
                    entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                    Invoice.Payments.Add(Payments);
                }

                SalesReturnLIST.Add(Invoice);

                ///Serealize SalesInvoiceLIST into Json
                var Sales_return_Json = new JavaScriptSerializer().Serialize(SalesReturnLIST);

                file_create(Sales_return_Json as string, "sales_return");
                //Send Sales_Json send it to Server Address specified.
            }
        }

        private void PurchaseInvoice_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> PurchaseInvoiceLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            foreach (entity.purchase_invoice invoice in db.purchase_invoice.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Commercial_Invoice Invoice = new entity.DebeHaber.Commercial_Invoice();

                Invoice.ID = invoice.id_purchase_invoice;
                Invoice.Type = entity.DebeHaber.TransactionTypes.Purchase;
                Invoice.BranchCode = invoice.app_branch.code;
                Invoice.BranchName = invoice.app_branch.name;
                Invoice.Comment = invoice.comment;
                Invoice.Contact_GovCode = invoice.contact.gov_code;
                Invoice.CurrencyISO_Code = invoice.app_currencyfx.app_currency.name;
                Invoice.InvoiceTotal = invoice.GrandTotal;
                Invoice.PaymentCondition = invoice.app_contract.app_contract_detail.Sum(x => x.interval);

                Invoice.InvoiceCode = invoice.code;
                //Invoice.InvoiceCode_ExpDate = //invoice. Here we need to add new field in database

                Invoice.InvoiceNumber = invoice.number;
                Invoice.InvoiceDate = invoice.trans_date;

                ///Loop through details.
                foreach (entity.purchase_invoice_detail Detail in invoice.purchase_invoice_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    CommercialInvoice_Detail.VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
                    CommercialInvoice_Detail.Value = Detail.SubTotal_Vat;
                    CommercialInvoice_Detail.Comment = Detail.item_description;

                    Invoice.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in invoice.payment_schedual.Where(x => x.id_payment_detail > 0))
                {
                    entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                    Invoice.Payments.Add(Payments);
                }

                PurchaseInvoiceLIST.Add(Invoice);

                ///Serealize SalesInvoiceLIST into Json
                var Purchase_Json = new JavaScriptSerializer().Serialize(PurchaseInvoiceLIST);
                file_create(Purchase_Json as string, "purchase_invoice");
                //Send Sales_Json send it to Server Address specified.
            }
        }

        private void PurchaseReturn_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> PurchaseReturnLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            foreach (entity.purchase_return Purcahsereturn in db.purchase_return.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Commercial_Invoice Invoice = new entity.DebeHaber.Commercial_Invoice();

                Invoice.ID = Purcahsereturn.id_purchase_invoice;
                Invoice.Type = entity.DebeHaber.TransactionTypes.Purchase;
                Invoice.BranchCode = Purcahsereturn.app_branch.code;
                Invoice.BranchName = Purcahsereturn.app_branch.name;
                Invoice.Comment = Purcahsereturn.comment;
                Invoice.Contact_GovCode = Purcahsereturn.contact.gov_code;
                Invoice.CurrencyISO_Code = Purcahsereturn.app_currencyfx.app_currency.name;
                Invoice.InvoiceTotal = Purcahsereturn.GrandTotal;
                Invoice.PaymentCondition = Purcahsereturn.app_contract.app_contract_detail.Sum(x => x.interval);

                Invoice.InvoiceCode = Purcahsereturn.code;
                //Invoice.InvoiceCode_ExpDate = //invoice. Here we need to add new field in database

                Invoice.InvoiceNumber = Purcahsereturn.number;
                Invoice.InvoiceDate = Purcahsereturn.trans_date;

                ///Loop through details.
                foreach (entity.purchase_return_detail Detail in Purcahsereturn.purchase_return_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    CommercialInvoice_Detail.VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
                    CommercialInvoice_Detail.Value = Detail.SubTotal_Vat;
                    CommercialInvoice_Detail.Comment = Detail.item_description;

                    Invoice.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in Purcahsereturn.payment_schedual.Where(x => x.id_payment_detail > 0))
                {
                    entity.DebeHaber.Payments Payments = new entity.DebeHaber.Payments();
                    Invoice.Payments.Add(Payments);
                }

                PurchaseReturnLIST.Add(Invoice);

                ///Serealize SalesInvoiceLIST into Json
                var PurchaseReturn_Json = new JavaScriptSerializer().Serialize(PurchaseReturnLIST);
                file_create(PurchaseReturn_Json as string, "Purcahse_return");
                //Send Sales_Json send it to Server Address specified.
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource.View != null)
            {
                foreach (entity.sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<entity.sales_invoice>().ToList())
                {
                    sales_invoice.IsSelected = true;
                }
                sales_invoiceViewSource.View.Refresh();
            }

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource.View != null)
            {
                foreach (entity.sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<entity.sales_invoice>().ToList())
                {
                    sales_invoice.IsSelected = false;
                }
                sales_invoiceViewSource.View.Refresh();
            }

        }



        private void SalesReturn_Checked(object sender, RoutedEventArgs e)
        {
            if (sales_returnViewSource.View != null)
            {
                foreach (entity.sales_return sales_return in sales_returnViewSource.View.OfType<entity.sales_return>().ToList())
                {
                    sales_return.IsSelected = true;
                }
                sales_returnViewSource.View.Refresh();
            }
        }

        private void SalesReturn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sales_returnViewSource.View != null)
            {
                foreach (entity.sales_return sales_return in sales_returnViewSource.View.OfType<entity.sales_return>().ToList())
                {
                    sales_return.IsSelected = false;
                }
                sales_returnViewSource.View.Refresh();
            }
        }





        private void Purchase_Checked(object sender, RoutedEventArgs e)
        {
            if (purchase_invoiceViewSource.View != null)
            {
                foreach (entity.purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<entity.purchase_invoice>().ToList())
                {
                    purchase_invoice.IsSelected = true;
                }
                purchase_invoiceViewSource.View.Refresh();
            }
        }

        private void Purchase_UnChecked(object sender, RoutedEventArgs e)
        {
            if (purchase_invoiceViewSource.View != null)
            {
                foreach (entity.purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<entity.purchase_invoice>().ToList())
                {
                    purchase_invoice.IsSelected = false;
                }
                purchase_invoiceViewSource.View.Refresh();
            }
        }

        private void PurchaseRetuen_Checked(object sender, RoutedEventArgs e)
        {
            if (purchase_returnViewSource.View != null)
            {
                foreach (entity.purchase_return purchase_return in purchase_returnViewSource.View.OfType<entity.purchase_return>().ToList())
                {
                    purchase_return.IsSelected = true;
                }
                purchase_returnViewSource.View.Refresh();
            }

        }

        private void PurchaseReturn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (purchase_returnViewSource.View != null)
            {
                foreach (entity.purchase_return purchase_return in purchase_returnViewSource.View.OfType<entity.purchase_return>().ToList())
                {
                    purchase_return.IsSelected = false;
                }
                purchase_returnViewSource.View.Refresh();
            }
        }

        private void Payment_Checked(object sender, RoutedEventArgs e)
        {
            if (paymentViewSource.View != null)
            {
                foreach (entity.payment payment in paymentViewSource.View.OfType<entity.payment>().ToList())
                {
                    payment.IsSelected = true;
                }
                paymentViewSource.View.Refresh();
            }

        }

        private void Payment_UnChecked(object sender, RoutedEventArgs e)
        {
            if (paymentViewSource.View != null)
            {
                foreach (entity.payment payment in paymentViewSource.View.OfType<entity.payment>().ToList())
                {
                    payment.IsSelected = false;
                }
                paymentViewSource.View.Refresh();
            }
        }
        public void file_create(String Data,String filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + filename + ".json";
            if (!System.IO.File.Exists(path))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(path))
                {
                    using (var fw = new StreamWriter(fs))
                    {
                        fw.Write(Data);
                        fw.Flush();
                    }
                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", filename);
                return;
            }
        }
    }
}
