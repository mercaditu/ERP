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
using System.Net;

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

        string RelationshipHash = string.Empty;

        public DebeHaberSync()
        {
            InitializeComponent();

            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_returnViewSource = ((CollectionViewSource)(FindResource("sales_returnViewSource")));
            purchase_invoiceViewSource = ((CollectionViewSource)(FindResource("purchase_invoiceViewSource")));
            purchase_returnViewSource = ((CollectionViewSource)(FindResource("purchase_returnViewSource")));
            paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));

            RelationshipHash = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault().domain;
        }

        private void btnData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_SalesInvoice();
            Get_PurchaseInvoice();
            Get_PurchaseReturnInvoice();
            Get_SalesReturn();
            Get_Payment();
        }

        #region LoadData
        public void Get_SalesInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_invoiceViewSource.Source = db.sales_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }
        
        public void Get_Payment()
        {
            //x.Is Head replace with Is_Accounted = True.
            paymentViewSource.Source = db.payments.Where(x =>
                 x.id_company == entity.CurrentSession.Id_Company &&
                 x.is_accounted == false &&
                 x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_SalesReturn()
        {
            //x.Is Head replace with Is_Accounted = True.
            sales_returnViewSource.Source = db.sales_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_PurchaseReturnInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_returnViewSource.Source = db.purchase_return.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }

        public void Get_PurchaseInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            purchase_invoiceViewSource.Source = db.purchase_invoice.Where(x =>
                x.id_company == entity.CurrentSession.Id_Company &&
                x.is_accounted == false &&
                x.status == entity.Status.Documents_General.Approved).ToList();
        }
        #endregion

        private void btnData_Sync(object sender, RoutedEventArgs e)
        {
            SalesInvoice_Sync();
        }


        //private void Payment_Sync()
        //{
        //    List<entity.DebeHaber.Commercial_Invoice> PaymentLIST = new List<entity.DebeHaber.Commercial_Invoice>();

        //    foreach (entity.payment payment in db.payments.Local.Where(x => x.IsSelected))
        //    {
        //        entity.DebeHaber.Commercial_Invoice Invoice = new entity.DebeHaber.Commercial_Invoice();

        //        Invoice.ID = payment.id_payment;
        //        Invoice.Type = (int)entity.DebeHaber.TransactionTypes.Sales;
        //        Invoice.BranchCode = payment.app_branch.code;
        //        Invoice.BranchName = payment.app_branch.name;
        //      //  Invoice.Comment = payment.comm;
        //        Invoice.Gov_Code = payment.contact.gov_code;
        //       // Invoice.CurrencyISO_Code = payment..app_currency.name;
        //        Invoice.InvoiceTotal = payment.GrandTotal;
        //      //  Invoice.PaymentCondition = payment.app_contract.app_contract_detail.Sum(x => x.interval);

        //        Invoice.InvoiceCode = payment.app_document_range != null ? payment.app_document_range.code : "NA";
        //        Invoice.InvoiceCode_ExpDate = (payment.app_document_range != null ? (DateTime)payment.app_document_range.expire_date : DateTime.Now);

        //        Invoice.DocNumber = payment.number;
        //        Invoice.InvoiceDate = payment.trans_date;

        //        PaymentLIST.Add(Invoice);

        //        ///Serealize SalesInvoiceLIST into Json
        //        var Payment_Json = new JavaScriptSerializer().Serialize(PaymentLIST);

        //        Send2API(Payment_Json);
        //        //file_create(Payment_Json as string, "Payment");
        //        //Send Sales_Json send it to Server Address specified.
        //    }
        //}
        
        private void SalesInvoice_Sync()
        {
            List<entity.DebeHaber.Commercial_Invoice> SalesInvoiceLIST = new List<entity.DebeHaber.Commercial_Invoice>();

            //Loop through
            foreach (entity.sales_invoice sales_invoice in db.sales_invoice.Local.Where(x => x.IsSelected))
            {
                entity.DebeHaber.Commercial_Invoice Sales = new entity.DebeHaber.Commercial_Invoice();

                Sales.Type = entity.DebeHaber.TransactionTypes.Sales;
                Sales.TransDate = sales_invoice.trans_date;
                Sales.Gov_Code = sales_invoice.contact.gov_code;
                Sales.Comment = sales_invoice.comment;
                Sales.CurrencyName = sales_invoice.app_currencyfx.app_currency.name;

                Sales.DocNumber = sales_invoice.number;
                Sales.DocCode = sales_invoice.app_document_range != null ? sales_invoice.app_document_range.code : "";
                Sales.DocExpiry = (sales_invoice.app_document_range != null ? (DateTime)sales_invoice.app_document_range.expire_date : DateTime.Now);

                ///Loop through Details.
                foreach (entity.sales_invoice_detail Detail in sales_invoice.sales_invoice_detail)
                {
                    entity.DebeHaber.CommercialInvoice_Detail CommercialInvoice_Detail = new entity.DebeHaber.CommercialInvoice_Detail();
                    CommercialInvoice_Detail.VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
                    CommercialInvoice_Detail.UnitValue_WithVAT = Detail.SubTotal_Vat;
                    CommercialInvoice_Detail.Comment = Detail.item_description;

                    entity.DebeHaber.CostCenter CostCenter = new entity.DebeHaber.CostCenter();

                    // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
                    if (Detail.item.id_item_type == entity.item.item_type.FixedAssets)
                    {
                        CostCenter.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                        CostCenter.Type = entity.DebeHaber.CostCenterTypes.FixedAsset;

                        //Add CostCenter into Detail.
                        CommercialInvoice_Detail.CostCenter.Add(CostCenter);
                    }
                    // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
                    else if (Detail.item.id_item_type == entity.item.item_type.Service || Detail.item.id_item_type == entity.item.item_type.Task || Detail.item.id_item_type == entity.item.item_type.ServiceContract)
                    {
                        if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                        { CostCenter.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                        else
                        { CostCenter.Name = Detail.item_description; }

                        CostCenter.Type = entity.DebeHaber.CostCenterTypes.Income;

                        //Add CostCenter into Detail.
                        CommercialInvoice_Detail.CostCenter.Add(CostCenter);
                    }
                    // Finally if all else fails, assume Item being sold is Merchendice.
                    else
                    {
                        CostCenter.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                        CostCenter.Type = entity.DebeHaber.CostCenterTypes.Merchendice;

                        //Add CostCenter into Detail.
                        CommercialInvoice_Detail.CostCenter.Add(CostCenter);
                    }

                    //Add Detail into Sales.
                    Sales.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                }

                //Loop through payments made.
                foreach (entity.payment_schedual schedual in sales_invoice.payment_schedual.Where(x => x.id_payment_detail > 0 && x.parent!=null && x.payment_detail.payment.is_accounted == false))
                {
                    entity.DebeHaber.Payments Payment = new entity.DebeHaber.Payments();
                    Payment.Type = 3;
                    Payment.TransDate = schedual.payment_detail.payment.trans_date;

                    if (schedual.parent.sales_invoice != null)
                    {
                        Payment.Parent = schedual.parent.sales_invoice.number;
                        Payment.Gov_Code = schedual.payment_detail.payment.contact != null ? schedual.payment_detail.payment.contact.gov_code : "";
                        Payment.DocCode = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.code : "";
                        Payment.DocExpiry = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.expire_date : DateTime.Now;
                        Payment.DocNumber = schedual.payment_detail.payment.number;
                    }

                    Payment.Account = schedual.payment_detail.app_account.name;
                    Payment.Value = schedual.debit;
                    
                    Sales.Payments.Add(Payment);

                    //This will make the Sales Invoice hide from the next load.
                    schedual.payment_detail.payment.is_accounted = true;
                }

                SalesInvoiceLIST.Add(Sales);

                //This will make the Sales Invoice hide from the next load.
                sales_invoice.is_accounted = true;
            }

            try
            {
                ///Serealize SalesInvoiceLIST into Json
                var Sales_Json = new JavaScriptSerializer().Serialize(SalesInvoiceLIST);

                Send2API(Sales_Json);
                file_create(Sales_Json as string, "sales_invoice");
                //Send Sales_Json send it to Server Address specified.

                //If all success, then SaveChanges.
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error: " + ex.Message);
            }
        }

        #region CheckBox Check/UnCheck Methods
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
        #endregion

        private void Send2API(string Json)
        {
            var webAddr = Cognitivo.Properties.Settings.Default.DebeHaberConnString + "/api_transactions/" + RelationshipHash + "/";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(Json);
                streamWriter.Flush();
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
