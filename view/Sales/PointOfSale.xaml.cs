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
        /// <summary>
        /// Context
        /// </summary>
        SalesInvoiceDB SalesInvoiceDB = new SalesInvoiceDB();
        PaymentDB PaymentDB = new PaymentDB();
        entity.Brillo.Promotion.Start StartPromo = new entity.Brillo.Promotion.Start(true);

        /// <summary>
        /// CollectionViewSource
        /// </summary>
        CollectionViewSource sales_invoiceViewSource;
        CollectionViewSource paymentViewSource;
        CollectionViewSource app_currencyViewSource;

        public PointOfSale()
        {
            InitializeComponent();
        }

        #region ActionButtons

        /// <summary>
        /// Navigates to CLIENT Tab
        /// </summary>
        private void btnClient_Click(object sender, EventArgs e)
        {
            tabContact.IsSelected = true;
        }

        /// <summary>
        /// Navigates to ACCOUNT UTILITY Tab
        /// </summary>
        private void btnAccount_Click(object sender, EventArgs e)
        {
            tabAccount.IsSelected = true;
            frmaccount.Refresh();
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
            payment payment = paymentViewSource.View.CurrentItem as payment;
            /// VALIDATIONS...
            /// 
            /// Validates if Contact is not assigned, then it will take user to the Contact Tab.
            if (sales_invoice.contact == null)
            {
                tabContact.Focus();
                return;
            }

            /// Validates if Sales Detail has 0 rows, then take you to Sales Tab.
            if (sales_invoice.sales_invoice_detail.Count == 0)
            {
                tabSales.Focus();
                return;
            }

            /// Validate Payment <= Sales.GrandTotal
            //if (payment.GrandTotal >= payment.GrandTotal_Detail)
            //{
            if (payment.GrandTotalDetail < sales_invoice.GrandTotal)
            {
                tabPayment.Focus();
                return;
            }

            /// If all validation is met, then we can start Sales Process.
            if (sales_invoice.contact != null && sales_invoice.sales_invoice_detail.Count > 0)
            {
                ///Approve Sales Invoice.
                ///Note> Approve includes Save Logic. No need to seperately Save.
                ///Plus we are passing True as default because in Point of Sale, we will always discount Stock.
                SalesInvoiceDB.Approve(true);

                payment_schedual payment_schedual = SalesInvoiceDB.payment_schedual.Where(x => x.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).FirstOrDefault();
                PaymentDB.Approve(payment_schedual.id_payment_schedual, (bool)chkreceipt.IsChecked);

                //Start New Sale
                New_Sale_Payment();
            }
        }

        private void New_Sale_Payment()
        {
            ///Creating new SALES INVOICE for upcomming sale. 
            ///TransDate = 0 because in Point of Sale we are assuming sale will always be done today.
            sales_invoice sales_invoice = SalesInvoiceDB.New(0, false);
            SalesInvoiceDB.sales_invoice.Add(sales_invoice);

            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_invoiceViewSource.Source = SalesInvoiceDB.sales_invoice.Local;
            sales_invoiceViewSource.View.MoveCurrentTo(sales_invoice);

            PaymentDB = new PaymentDB();
            ///Creating new PAYMENT for upcomming sale. 
            payment payment = PaymentDB.New(true);
            payment.id_currencyfx = sales_invoice.id_currencyfx;
            PaymentDB.payments.Add(payment);

            paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
            paymentViewSource.Source = PaymentDB.payments.Local;
            paymentViewSource.View.MoveCurrentTo(payment);

            tabContact.Focus();
            sbxContact.Text = "";
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
                sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;

                item item = SalesInvoiceDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                sales_invoice_detail _sales_invoice_detail = SalesInvoiceDB.Select_Item(ref sales_invoice, item, false);

                Class.StockCalculations StockCalculations = new Cognitivo.Class.StockCalculations();
                _sales_invoice_detail.Quantity_InStock = StockCalculations.Count_ByBranch(CurrentSession.Id_Branch, item.id_item, DateTime.Now);

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();

                sales_invoiceViewSource.View.Refresh();
                paymentViewSource.View.Refresh();
                btnPromotion_Click(sender, e);
            }
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ///This code will create a new Sale & Payment Information.
            New_Sale_Payment();

            //PAYMENT TYPE
            SalesInvoiceDB.payment_type.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company && a.payment_behavior == payment_type.payment_behaviours.Normal).Load();
            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            payment_typeViewSource.Source = SalesInvoiceDB.payment_type.Local;

            cbxSalesRep.ItemsSource = SalesInvoiceDB.sales_rep.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).ToList(); //CurrentSession.Get_SalesRep();

            app_currencyViewSource = (CollectionViewSource)this.FindResource("app_currencyViewSource");
            app_currencyViewSource.Source = CurrentSession.Get_Currency();

            int Id_Account = CurrentSession.Id_Account;
            if (SalesInvoiceDB.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault() != null)
            {
                if (SalesInvoiceDB.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault().is_active == false)
                {
                    btnAccount_Click(sender, e);
                    frmaccount.Refresh();
                }
            }
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
            else if (e.Key == Key.F12)
            {
                btnSave_Click(sender, e);
            }
            else if (e.Key == Key.F5)
            {
                boderdiscount_MouseDown(sender, e);
            }
        }

        private void Cancel_MouseDown(object sender, EventArgs e)
        {
            SalesInvoiceDB.CancelAllChanges();
            PaymentDB.CancelAllChanges();

            New_Sale_Payment();

            //Clean up Contact Data.
            sbxContact.Text = "";
            sbxContact.ContactID = 0;

            sbxItem.Text = string.Empty;
            sbxItem.ItemID = 0;
            sbxItem.Item = null;

            tabContact.IsSelected = true;
        }

        #region Details

        private void dgvSalesDetail_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

            sales_invoiceViewSource.View.Refresh();
        }

        private void dgvPaymentDetail_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = e.NewItem as payment_detail;

            payment_detail.State = EntityState.Added;
            payment_detail.IsSelected = true;
            payment_detail.id_currencyfx = sales_invoice.id_currencyfx;
            payment_detail.id_currency = sales_invoice.app_currencyfx.id_currency;

            payment_detail.id_payment = payment.id_payment;
            payment_detail.payment = payment;

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


                        btnPromotion_Click(sender, e);

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

        #endregion

        #region Discount

        private void boderdiscount_MouseDown(object sender, EventArgs e)
        {
            popupDiscount.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupDiscount.IsOpen = false;
        }

        #endregion

        #region Contact CRUD

        private void btnNewCustomer_MouseDown(object sender, EventArgs e)
        {
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Contact);

            if (Sec.create)
            {
                popCrud.IsOpen = true;
                popCrud.Visibility = Visibility.Visible;

                //Add CRUD Panel into View.
                cntrl.Curd.contact ContactCURD = new cntrl.Curd.contact();
                ContactCURD.IsCustomer = true;
                stackCustomer.Children.Add(ContactCURD);
            }
        }

        private void crudContact_btnCancel_Click(object sender)
        {
            stackCustomer.Children.RemoveAt(0);
            popCrud.IsOpen = false;
        }

        #endregion


        private void lblGrandTotalsales_DataContextChanged(object sender, EventArgs e)
        {
            if (sales_invoiceViewSource != null && paymentViewSource != null)
            {
                if (sales_invoiceViewSource.View != null && paymentViewSource.View != null)
                {
                    if (sales_invoiceViewSource.View.CurrentItem != null && paymentViewSource.View.CurrentItem != null)
                    {
                        sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                        payment payment = paymentViewSource.View.CurrentItem as payment;
                        payment.GrandTotal = sales_invoice.GrandTotal;
                    }
                }
            }
        }

        private void NewPayment_MouseUp(object sender, MouseButtonEventArgs e)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = new payment_detail();
            payment.payment_detail.Add(payment_detail);
        }



        private void Clear_MouseDown(object sender, EventArgs e)
        {
            if (sales_invoiceViewSource != null && paymentViewSource != null)
            {
                if (sales_invoiceViewSource.View != null && paymentViewSource.View != null)
                {
                    if (sales_invoiceViewSource.View.CurrentItem != null && paymentViewSource.View.CurrentItem != null)
                    {
                        sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                        if (sales_invoice.GrandTotal > 0)
                        {
                            decimal TrailingDecimals = sales_invoice.GrandTotal - Math.Floor(sales_invoice.GrandTotal);
                            sales_invoice.DiscountWithoutPercentage += TrailingDecimals;
                        }
                    }
                }
            }
        }

        private void btnPromotion_Click(object sender, EventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            SalesInvoiceDB.sales_invoice_detail.RemoveRange(sales_invoice.sales_invoice_detail.Where(x => x.IsPromo).ToList());
            StartPromo.Calculate_SalesInvoice(ref sales_invoice);


            CollectionViewSource sales_invoicesales_invoice_detailViewSource = (CollectionViewSource)this.FindResource("sales_invoicesales_invoice_detailViewSource");

            sales_invoicesales_invoice_detailViewSource.View.Refresh();



        }




    }
}
