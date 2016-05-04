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
using System.Data.Entity.Validation;
using entity.Brillo;

namespace Cognitivo.Sales
{
    public partial class PointOfSale : Page
    {
        SalesInvoiceDB SalesInvoiceDB = new SalesInvoiceDB();
        PaymentDB PaymentDB = new PaymentDB();
        Settings SalesSettings = new Settings();
        dbContext dbContext = new dbContext();
        CollectionViewSource sales_invoiceViewSource, paymentViewSource, app_currencyViewSource;

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

        private void btnAccount_Click(object sender, EventArgs e)
        {
            tabAccount.IsSelected = true;
            if (frmActive.Children.Count > 0)
            {
                frmActive.Children.RemoveAt(0);
            }
            Configs.AccountActive AccountActive = new Configs.AccountActive();
            frmActive.Children.Add(AccountActive);
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            tabSales.IsSelected = true;
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            tabPayment.IsSelected = true;
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
            if (payment.payment_detail.FirstOrDefault() != null)
            {
                payment.payment_detail.FirstOrDefault().value = sales_invoice.GrandTotal;
                payment.payment_detail.FirstOrDefault().RaisePropertyChanged("value");
            }

            sales_invoiceViewSource.View.Refresh();
            paymentViewSource.View.Refresh();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem as sales_invoice;

            if (sales_invoice.contact != null && sales_invoice.sales_invoice_detail.Count > 0)
            {
                IEnumerable<DbEntityValidationResult> validationresult = SalesInvoiceDB.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    SalesInvoiceDB.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Some Value is missing..");
                    return;
                }

                SalesInvoiceDB.Approve(true);

                payment payment = (payment)paymentViewSource.View.CurrentItem as payment;

                payment_schedual payment_schedual = SalesInvoiceDB.payment_schedual.Where(x => x.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).FirstOrDefault();

                payment.IsSelected = true;
                payment.status = Status.Documents_General.Pending;

                PaymentDB.payments.Add(payment);

                PaymentDB.Approve(payment_schedual.id_payment_schedual,(bool)chkreceipt.IsChecked);

                sales_invoice Newsales_invoice = SalesInvoiceDB.New(SalesSettings.TransDate_Offset);

                //Copy the Sales Rep of previous sale.
                Newsales_invoice.id_sales_rep = sales_invoice.id_sales_rep;
                SalesInvoiceDB.sales_invoice.Add(Newsales_invoice);
                
                sales_invoiceViewSource.View.Refresh();
                sales_invoiceViewSource.View.MoveCurrentToLast();
                payment paymentnew = new payment();
                paymentnew.id_range = GetDefault.Range(entity.App.Names.PaymentUtility);

                if (PaymentDB.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault() != null)
                {
                    payment.app_document_range = PaymentDB.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault();
                }

                paymentnew.status = Status.Documents_General.Pending;
                payment_detail payment_detailnew = new entity.payment_detail();
                payment_detailnew.id_payment_type = SalesInvoiceDB.payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
                payment_detailnew.id_currency = Newsales_invoice.app_currencyfx.id_currency;
                paymentnew.payment_detail.Add(payment_detailnew);
                paymentList.Add(paymentnew);
                paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
                paymentViewSource.Source = paymentList;
                paymentViewSource.View.Refresh();
                paymentViewSource.View.MoveCurrentToLast();
                tabContact.IsSelected = true;
                sbxContact.Text = "";
                //Run approve code here.	 
            }
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
            }
        }

        private void sbxItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_invoice sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem as sales_invoice;

                item item = SalesInvoiceDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                SalesInvoiceDB.Select_Item(ref sales_invoice, item, SalesSettings.AllowDuplicateItem);

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();

                payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
                if (payment.payment_detail.FirstOrDefault() != null)
                {
                    payment.payment_detail.FirstOrDefault().value = sales_invoice.GrandTotal;
                }

                sales_invoiceViewSource.View.Refresh();
                paymentViewSource.View.Refresh();
                sbxItem.Focus();
            }
        }

        #endregion

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_invoice sales_invoice = SalesInvoiceDB.New(SalesSettings.TransDate_Offset);
            SalesInvoiceDB.sales_invoice.Add(sales_invoice);

            sales_invoiceViewSource.Source = SalesInvoiceDB.sales_invoice.Local;
            sales_invoiceViewSource.View.MoveCurrentToLast();

            //cbxSalesRep
            SalesInvoiceDB.payment_type.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company && a.payment_behavior == payment_type.payment_behaviours.Normal).Load();
            SalesInvoiceDB.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxSalesRep.ItemsSource = SalesInvoiceDB.sales_rep.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).ToList();

                CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
                payment_typeViewSource.Source = SalesInvoiceDB.payment_type.Local;

                app_currencyViewSource = (CollectionViewSource)this.FindResource("app_currencyViewSource");
                app_currencyViewSource.Source = SalesInvoiceDB.app_currency.Local;

                payment payment = new payment();
                payment.id_range = GetDefault.Range(entity.App.Names.PaymentUtility);
                if (PaymentDB.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault() != null)
                {
                    payment.app_document_range = PaymentDB.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault();
                }
                payment.status = Status.Documents_General.Pending;
                payment_detail payment_detailnew = new payment_detail();
                if (SalesInvoiceDB.payment_type.Where(x => x.is_default).FirstOrDefault() != null)
                {
                    payment_detailnew.id_payment_type = SalesInvoiceDB.payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;

                }
                else
                {
                    MessageBox.Show("Please Select Defult Payment Type");
                }

                payment_detailnew.id_currency = sales_invoice.app_currencyfx.id_currency;
                payment.payment_detail.Add(payment_detailnew);
                paymentList.Add(payment);
                paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
                paymentViewSource.Source = paymentList;

                if (SalesInvoiceDB.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault() != null)
                {
                    if (SalesInvoiceDB.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault().is_active == false)
                    {
                        tabAccount.IsSelected = true;
                    }
                }
            }));
        }

        private void dgvPaymentDetail_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = e.NewItem as payment_detail;
            payment_detail.id_currencyfx = sales_invoice.id_currencyfx;
            payment_detail.id_currency = sales_invoice.app_currencyfx.id_currency;
            List<payment_detail> payment_detaillist = payment.payment_detail.GroupBy(x => x.id_currencyfx).Select(x => x.FirstOrDefault()).ToList();
            decimal totalpaid = 0;
            foreach (app_currency app_currency in app_currencyViewSource.View.Cast<app_currency>().ToList())
            {
                decimal amount = payment_detaillist.Where(x => x.id_currency == app_currency.id_currency).Sum(x => x.value);

                if (sales_invoice.app_currencyfx.id_currency == app_currency.id_currency)
                {
                    totalpaid += amount;
                }
                else
                {

                    totalpaid += Currency.convert_Values(amount, app_currency.id_currency, sales_invoice.id_currencyfx, entity.App.Modules.Sales);

                }
            }
            payment_detail.value = sales_invoice.GrandTotal - totalpaid;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                tabContact.IsSelected = true;
            }
            else if (e.Key == Key.F2)
            {
                tabSales.IsSelected = true;
            }
            else if (e.Key == Key.F3)
            {
                tabPayment.IsSelected = true;
            }
            else if (e.Key == Key.F4)
            {
                btnSave_Click(sender, e);
            }
            else if (e.Key == Key.F5)
            {
                boderdiscount_MouseDown(sender, e);
            }
        }

        private void dgvSalesDetail_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            sales_invoiceViewSource.View.Refresh();

            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;

            payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
            if (payment.payment_detail.FirstOrDefault() != null)
            {
                payment.payment_detail.FirstOrDefault().value = sales_invoice.GrandTotal;
            }
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_invoice_detail != null)
            {
                e.CanExecute = true;
            }
            else if (e.Parameter as payment_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (e.Parameter as sales_invoice_detail != null)
                    {
                        sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                        //DeleteDetailGridRow
                        dgvSalesDetail.CancelEdit();
                        sales_invoice.sales_invoice_detail.Remove(e.Parameter as sales_invoice_detail);

                        sales_invoiceViewSource.View.Refresh();
                        CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                        sales_invoicesales_invoice_detailViewSource.View.Refresh();

                    }
                    else if (e.Parameter as payment_detail != null)
                    {
                        payment payment = paymentViewSource.View.CurrentItem as payment;
                        //DeleteDetailGridRow
                        dgvPaymentDetail.CancelEdit();
                        payment.payment_detail.Remove(e.Parameter as payment_detail);

                        paymentViewSource.View.Refresh();
                        CollectionViewSource paymentpayment_detailViewSource = FindResource("paymentpayment_detailViewSource") as CollectionViewSource;
                        paymentpayment_detailViewSource.View.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnNewCustomer_MouseDown(object sender, EventArgs e)
        {
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Contact);
           
            if (Sec.create)
            {
                crudContact.contactobject = new entity.contact();

                crudContact.entity = dbContext;
                popCrud.IsOpen = true;

                popCrud.Visibility = Visibility.Visible;
            }
        }

        private void boderdiscount_MouseDown(object sender, EventArgs e)
        {
            popupDiscount.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupDiscount.IsOpen = false;
        }

        private void dgvPaymentDetail_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
            payment_detail _payment_detail = (payment_detail)dgvPaymentDetail.SelectedItem;
            decimal Selectedvalue = Currency.convert_Values(_payment_detail.value, _payment_detail.id_currencyfx, sales_invoice.id_currencyfx, entity.App.Modules.Sales);
            decimal amount = sales_invoice.GrandTotal - Selectedvalue;
            if (payment.payment_detail.Count() > 1)
            {
                //amount =Currency.convert_Values(amount, _payment_detail.id_currencyfx, sales_invoice.id_currencyfx, entity.App.Modules.Sales);
                decimal value = (amount / (payment.payment_detail.Count() - 1));

                foreach (payment_detail payment_detail in payment.payment_detail)
                {
                    if (payment_detail != _payment_detail)
                    {

                        payment_detail.value = value;
                        payment_detail.RaisePropertyChanged("value");
                    }
                }
            }
        }

        private void Cancel_MouseDown(object sender, EventArgs e)
        {
            sales_invoice old_salesinvoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            SalesInvoiceDB.sales_invoice.Remove(old_salesinvoice);
            payment payment = paymentViewSource.View.CurrentItem as payment;
            paymentList.Remove(payment);

            sales_invoice Newsales_invoice = SalesInvoiceDB.New(SalesSettings.TransDate_Offset);

            SalesInvoiceDB.sales_invoice.Add(Newsales_invoice);
            sales_invoiceViewSource.View.Refresh();
            sales_invoiceViewSource.View.MoveCurrentToLast();
            payment paymentnew = new entity.payment();
            paymentnew.status = Status.Documents_General.Pending;
            payment_detail payment_detailnew = new entity.payment_detail();
            payment_detailnew.id_payment_type = SalesInvoiceDB.payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
            paymentnew.payment_detail.Add(payment_detailnew);
            // SalesInvoiceDB.payments.Add(paymentnew);
            paymentList.Add(paymentnew);
            paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
            paymentViewSource.Source = paymentList;
            paymentViewSource.View.Refresh();
            paymentViewSource.View.MoveCurrentToLast();

            //Clean up Contact Data.
            sbxContact.Text = "";
            sbxContact.ContactID = 0;
            sbxContact.Contact = null;

            tabContact.IsSelected = true;
          
        }

        private void crudContact_btnSave_Click(object sender)
        {
            dbContext.db.contacts.Add(crudContact.contactobject);
            dbContext.SaveChanges();
            popCrud.IsOpen = false;
            popCrud.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void crudContact_btnCancel_Click(object sender)
        {
            popCrud.IsOpen = false;
        }


       

    }
}
