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
        PaymentDB PaymentDB = new entity.PaymentDB();
        Settings SalesSettings = new Settings();

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

                //Sales

                //foreach (payment_detail payment_detail in payment.payment_detail)
                //{
                    payment_schedual payment_schedual = SalesInvoiceDB.payment_schedual.Where(x => x.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).FirstOrDefault();

                    payment.IsSelected = true;
                    payment.status = Status.Documents_General.Pending;
            
                    PaymentDB.payments.Add(payment);
                   
                    PaymentDB.Approve(payment_schedual.id_payment_schedual);

                //}

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
                //payment.contact = contact;
            }
        }

        private void sbxItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
             
                sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                item item = SalesInvoiceDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                SalesInvoiceDB.Select_Item(ref sales_invoice, item, SalesSettings.AllowDuplicateItem);

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();

                payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
                payment.payment_detail.FirstOrDefault().value = sales_invoice.GrandTotal;

                sales_invoiceViewSource.View.Refresh();
                paymentViewSource.View.Refresh();
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
          
            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_invoice sales_invoice = SalesInvoiceDB.New(SalesSettings.TransDate_Offset);
            SalesInvoiceDB.sales_invoice.Add(sales_invoice);

            sales_invoiceViewSource.Source = SalesInvoiceDB.sales_invoice.Local;
            sales_invoiceViewSource.View.MoveCurrentToLast();

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
                SalesInvoiceDB.payment_type.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company && a.payment_behavior == payment_type.payment_behaviours.Normal).Load();
                payment_typeViewSource.Source = SalesInvoiceDB.payment_type.Local;

                app_currencyViewSource = (CollectionViewSource)this.FindResource("app_currencyViewSource");
                SalesInvoiceDB.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
                app_currencyViewSource.Source = SalesInvoiceDB.app_currency.Local;

                payment payment = new entity.payment();
                payment.status = Status.Documents_General.Pending;
                payment_detail payment_detailnew = new entity.payment_detail();
                if (SalesInvoiceDB.payment_type.Where(x => x.is_default).FirstOrDefault()!=null)
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
                    foreach (payment_detail _payment_detail in payment_detaillist.Where(x => x.id_currency == app_currency.id_currency).ToList())
                    {
                        totalpaid += Currency.convert_Values(_payment_detail.value, sales_invoice.id_currencyfx, _payment_detail.id_currencyfx, entity.App.Modules.Sales);
                       // totalpaid += Currency.convert_Value(_payment_detail.value, _payment_detail.id_currencyfx, entity.App.Modules.Sales);
                    }
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
        }

        private void dgvSalesDetail_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            sales_invoiceViewSource.View.Refresh();
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            payment payment = (payment)paymentViewSource.View.CurrentItem as payment;
            payment.payment_detail.FirstOrDefault().value = sales_invoice.GrandTotal;


        }
        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_invoice_detail != null)
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
                    sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                    //DeleteDetailGridRow
                    dgvSalesDetail.CancelEdit();
                    sales_invoice.sales_invoice_detail.Remove(e.Parameter as sales_invoice_detail);
                   
                    sales_invoiceViewSource.View.Refresh();
                    CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                    sales_invoicesales_invoice_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnNewCustomer_MouseDown(object sender, EventArgs e)
        {

        }

        private void boderdiscount_MouseDown(object sender, EventArgs e)
        {
            popupVAT.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupVAT.IsOpen = false;
        }

     

    

    }
}
