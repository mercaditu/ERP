using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Data.Entity.Validation;
using entity.Brillo;

namespace Cognitivo.Sales
{
    public partial class PointOfSale : Page
    {
        entity.SalesInvoiceDB SalesInvoiceDB = new entity.SalesInvoiceDB();

        CollectionViewSource sales_invoiceViewSource, paymentViewSource, app_currencyViewSource;
        List<sales_invoice> salesinvoiceList = new List<sales_invoice>();
        List<payment> paymentList = new List<payment>();
        public PointOfSale()
        {
            InitializeComponent();
        }

        #region Buttons

        private void btnClient_Click(object sender, EventArgs e)
        {
            tabContact.IsSelected = true;
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            tabSales.IsSelected = true;
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            tabPayment.IsSelected = true;
        }


        public entity.app_contract app_contract { get; set; }
        public entity.app_condition app_condition { get; set; }
        public entity.app_currencyfx app_currencyfx { get; set; }
        public entity.app_document_range app_document_range { get; set; }

        private void btnSave_Click(object sender, EventArgs e)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            sales_invoice.app_branch = SalesInvoiceDB.app_branch.Where(x => x.id_branch == sales_invoice.id_branch).FirstOrDefault();

            if (app_document_range.id_range != null)
            {
                sales_invoice.id_range = app_document_range.id_range;
            }

            if (app_condition != null)
            {
                sales_invoice.id_condition = app_condition.id_condition;
                sales_invoice.app_condition = app_condition;
            }
            if (app_contract != null)
            {
                sales_invoice.id_contract = app_contract.id_contract;
                sales_invoice.app_contract = app_contract;
            }
            if (app_currencyfx != null)
            {
                sales_invoice.id_currencyfx = app_currencyfx.id_currencyfx;
                sales_invoice.app_currencyfx = app_currencyfx;
            }

            IEnumerable<DbEntityValidationResult> validationresult = SalesInvoiceDB.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                SalesInvoiceDB.SaveChanges();
            }
            else
            {
                MessageBox.Show("Some Value is missing..");
            }
            SalesInvoiceDB.Approve(true);



            payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
            SalesInvoiceDB.payments.Add(payment);
            if (SalesInvoiceDB.app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PaymentUtility).FirstOrDefault() != null)
            {
                app_document_range = SalesInvoiceDB.app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PaymentUtility).FirstOrDefault();
                payment.id_range = app_document_range.id_range;
            }


            foreach (payment_detail payment_detail in payment.payment_detail)
            {

                payment_detail.id_account = CurrentSession.Id_Account;
                payment_detail.app_currencyfx = SalesInvoiceDB.app_currencyfx.Where(x => x.id_currencyfx == payment_detail.id_currencyfx).FirstOrDefault();
                payment_schedual _payment_schedual = new payment_schedual();

                _payment_schedual.credit = Convert.ToDecimal(payment_detail.value);
                _payment_schedual.parent = SalesInvoiceDB.payment_schedual.Where(x => x.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).FirstOrDefault();
                _payment_schedual.expire_date = DateTime.Now;
                _payment_schedual.status = Status.Documents_General.Approved;
                _payment_schedual.id_contact = sales_invoice.id_contact;
                _payment_schedual.id_currencyfx = payment_detail.id_currencyfx;
                _payment_schedual.id_sales_invoice = sales_invoice.id_sales_invoice;
                _payment_schedual.trans_date = sales_invoice.trans_date;

                payment_detail.payment_schedual.Add(_payment_schedual);
            }






            IEnumerable<DbEntityValidationResult> validationresultpaymnet = SalesInvoiceDB.GetValidationErrors();
            if (validationresultpaymnet.Count() == 0)
            {
                SalesInvoiceDB.SaveChanges();
            }
            else
            {
                MessageBox.Show("Some Value is missing..");
            }
            entity.Brillo.Document.Start.Automatic(payment, payment.app_document_range);

            Settings SalesSettings = new Settings();
            sales_invoice Newsales_invoice = SalesInvoiceDB.New(SalesSettings.TransDate_Offset);


            SalesInvoiceDB.sales_invoice.Add(Newsales_invoice);


            salesinvoiceList.Add(Newsales_invoice);
            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_invoiceViewSource.Source = salesinvoiceList;
            sales_invoiceViewSource.View.Refresh();
            sales_invoiceViewSource.View.MoveCurrentToLast();
            payment paymentnew = new entity.payment();
            paymentnew.status = Status.Documents_General.Pending;
            payment_detail payment_detailnew = new entity.payment_detail();
            paymentnew.payment_detail.Add(payment_detailnew);
            // SalesInvoiceDB.payments.Add(paymentnew);
            paymentList.Add(paymentnew);
            paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
            paymentViewSource.Source = paymentList;
            paymentViewSource.View.Refresh();
            paymentViewSource.View.MoveCurrentToLast();
            tabContact.IsSelected = true;
            sbxContact.Text = "";
            //Run approve code here.
        }

        #endregion

        #region SmartBox Selection

        private void sbxContact_Select(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = SalesInvoiceDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                sales_invoice sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
                sales_invoice.id_contact = contact.id_contact;
                sales_invoice.contact = contact;
                payment.id_contact = contact.id_contact;
                payment.contact = contact;
            }
        }

        private void sbxItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                Settings SalesSettings = new Settings();
                sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                item item = SalesInvoiceDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                SalesInvoiceDB.Select_Item(ref sales_invoice, item, SalesSettings.AllowDuplicateItem);

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();

                payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
                payment.payment_detail.FirstOrDefault().value = sales_invoice.GrandTotal;

                sbxItem.Focus();
            }
        }

        //private void select_Item(sales_invoice sales_invoice, item item)
        //{
        //    Settings SalesSettings = new Settings();
        //    if (sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || SalesSettings.AllowDuplicateItem)
        //    {
        //        sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail();
        //        _sales_invoice_detail.sales_invoice = sales_invoice;
        //        _sales_invoice_detail.Contact = sales_invoice.contact;
        //        _sales_invoice_detail.item_description = item.description;
        //        _sales_invoice_detail.item = item;
        //        _sales_invoice_detail.id_item = item.id_item;
        //        _sales_invoice_detail.quantity += 1;

        //        sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);
        //    }
        //    else
        //    {
        //        sales_invoice_detail sales_invoice_detail = sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
        //        sales_invoice_detail.quantity += 1;
        //    }

        //    Dispatcher.BeginInvoke((Action)(() =>
        //    {
        //        CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
        //        sales_invoicesales_invoice_detailViewSource.View.Refresh();

        //        sbxItem.Focus();
        //    }));
        //}

        #endregion

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //We do not need to load previous Sales Data. This defeats the purpose of Point of Sale
            if (SalesInvoiceDB.app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.SalesInvoice).FirstOrDefault() != null)
            {
                app_document_range = SalesInvoiceDB.app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.SalesInvoice).FirstOrDefault();
            }

            if (SalesInvoiceDB.app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault() != null)
            {
                if (SalesInvoiceDB.app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault().app_condition != null)
                {
                    app_condition = SalesInvoiceDB.app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault().app_condition;
                    app_contract = SalesInvoiceDB.app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault();
                }
            }

            //No need to run this every time, we can do this on Load and Save values.
            if (SalesInvoiceDB.app_currency.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_priority).FirstOrDefault() != null)
            {
                if (SalesInvoiceDB.app_currency.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_priority).FirstOrDefault().app_currencyfx.Where(y => y.is_active).FirstOrDefault() != null)
                {
                    app_currencyfx = SalesInvoiceDB.app_currency.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_priority).FirstOrDefault().app_currencyfx.Where(y => y.is_active).FirstOrDefault();
                }
            }

            await Dispatcher.InvokeAsync(new Action(() =>
            {

                sales_invoice sales_invoice = SalesInvoiceDB.New(0);
                if (app_condition != null)
                {
                    sales_invoice.id_condition = app_condition.id_condition;
                    sales_invoice.app_condition = app_condition;
                }
                if (app_contract != null)
                {
                    sales_invoice.id_contract = app_contract.id_contract;
                    sales_invoice.app_contract = app_contract;
                }
                if (app_currencyfx != null)
                {
                    sales_invoice.id_currencyfx = app_currencyfx.id_currencyfx;
                    sales_invoice.app_currencyfx = app_currencyfx;
                }





                SalesInvoiceDB.sales_invoice.Add(sales_invoice);

                salesinvoiceList.Add(sales_invoice);
                sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
                sales_invoiceViewSource.Source = salesinvoiceList;
                sales_invoiceViewSource.View.MoveCurrentToLast();

                CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
                SalesInvoiceDB.payment_type.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company && a.payment_behavior == payment_type.payment_behaviours.Normal).Load();
                payment_typeViewSource.Source = SalesInvoiceDB.payment_type.Local;

                app_currencyViewSource = (CollectionViewSource)this.FindResource("app_currencyViewSource");
                SalesInvoiceDB.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
                app_currencyViewSource.Source = SalesInvoiceDB.app_currency.Local;


                payment paymentnew = new entity.payment();
                paymentnew.status = Status.Documents_General.Pending;
                payment_detail payment_detailnew = new entity.payment_detail();
               
                payment_detailnew.id_currency = sales_invoice.app_currencyfx.id_currency;
                paymentnew.payment_detail.Add(payment_detailnew);
                // SalesInvoiceDB.payments.Add(paymentnew);
                paymentList.Add(paymentnew);
                paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
                paymentViewSource.Source = paymentList;
            }));




            //Not necesary. We will not give option to change VAT.
            //SalesInvoiceDB.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

            //await Dispatcher.InvokeAsync(new Action(() =>
            //{
            //    CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            //    app_vat_groupViewSource.Source = SalesInvoiceDB.app_vat_group.Local;
            //}));
        }



        private void dgvPaymentDetail_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = e.NewItem as payment_detail;
            payment_detail.id_currencyfx = sales_invoice.id_currencyfx;
            payment_detail.id_currency = sales_invoice.app_currencyfx.id_currency;
            List<payment_detail> payment_detaillist = payment.payment_detail.GroupBy(x => x.id_currencyfx).Select(x => x.FirstOrDefault()).ToList();
            decimal totalpaid=0;
            foreach (app_currency app_currency in app_currencyViewSource.View.Cast<app_currency>().ToList())
            {
              decimal amount=payment_detaillist.Where(x=>x.id_currency==app_currency.id_currency).Sum(x=>x.value);
                if (sales_invoice.app_currencyfx.id_currency == app_currency.id_currency)
                {
                    totalpaid += amount;
                }
                else
                {
                    foreach (payment_detail _payment_detail in payment_detaillist.Where(x => x.id_currency == app_currency.id_currency).ToList())
                    {
                        totalpaid += Currency.convert_Value(_payment_detail.value, _payment_detail.id_currencyfx, entity.App.Modules.Sales);
                    }
                    
                }

            }
            payment_detail.value = sales_invoice.GrandTotal - totalpaid;

        }

       
    }
}
