using Cognitivo.Menu;
using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Sales
{
    public partial class Restaurant : Page
    {
        private CollectionViewSource sales_invoiceViewSource;
        private CollectionViewSource paymentViewSource;

        private entity.Controller.Sales.InvoiceController SalesDB;
        private entity.Controller.Finance.Payment PaymentDB;

        public Restaurant()
        {
            InitializeComponent();

            SalesDB = FindResource("SalesDB") as entity.Controller.Sales.InvoiceController;
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                //Load Controller.
                SalesDB.Initialize();
                SalesDB.LoadPromotion();
            }
            PaymentDB = FindResource("PaymentDB") as entity.Controller.Finance.Payment;
            //Share DB to increase efficiency.
            PaymentDB.db = SalesDB.db;
        }

        #region ActionButtons

        /// <summary>
        /// Navigates to CLIENT Tab
        /// </summary>
        private void Client_Click(object sender, EventArgs e)
        {
            tabContact.IsSelected = true;
        }

        /// <summary>
        /// Navigates to ACCOUNT UTILITY Tab
        /// </summary>
        private void Account_Click(object sender, EventArgs e)
        {
            frmaccount.Navigate(new Configs.AccountActive(CurrentSession.Id_Account));
            tabAccount.IsSelected = true;
            tabAccount.Focus();
        }

        private void Sales_Click(object sender, EventArgs e)
        {
            tabSales.IsSelected = true;
        }

        private void Payment_Click(object sender, EventArgs e)
        {
            tabPayment.IsSelected = true;
            Promotion_Click(sender, e);
        }

        private void Save_Click(object sender, EventArgs e)
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
            if (payment.GrandTotalDetail < Math.Round(sales_invoice.GrandTotal, 2))
            {
                tabPayment.Focus();
                return;
            }

            if (payment.GrandTotalDetail > Math.Round(sales_invoice.GrandTotal, 2))
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
                SalesDB.Approve();

                List<payment_schedual> payment_schedualList = SalesDB.db.payment_schedual.Where(x => x.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).ToList();
                PaymentDB.Approve(payment_schedualList, true, (bool)chkreceipt.IsChecked);

                //Start New Sale
                New_Sale_Payment();
            }
        }

        private void New_Sale_Payment()
        {
            ///Creating new SALES INVOICE for upcomming sale.
            ///TransDate = 0 because in Point of Sale we are assuming sale will always be done today.
            Settings SalesSettings = new Settings();

            sales_invoice sales_invoice = SalesDB.Create(SalesSettings.TransDate_Offset, false);
            SalesDB.db.sales_invoice.Add(sales_invoice);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                sales_invoiceViewSource = FindResource("sales_invoiceViewSource") as CollectionViewSource;
                sales_invoiceViewSource.Source = SalesDB.db.sales_invoice.Local;
                sales_invoiceViewSource.View.MoveCurrentTo(sales_invoice);
            }));


            ///Creating new PAYMENT for upcomming sale.
            payment payment = PaymentDB.New(true);
            payment.id_currencyfx = sales_invoice.id_currencyfx;
            SalesDB.db.payments.Add(payment);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                paymentViewSource = FindResource("paymentViewSource") as CollectionViewSource;
                paymentViewSource.Source = SalesDB.db.payments.Local;
                paymentViewSource.View.MoveCurrentTo(payment);

                tabContact.Focus();
                sbxContact.Text = "";
            }));
        }

        #endregion ActionButtons

        #region SmartBox Selection

        private async void Contact_Select(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = await SalesDB.db.contacts.FindAsync(sbxContact.ContactID);
                if (contact != null)
                {
                    sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                    payment payment = paymentViewSource.View.CurrentItem as payment;
                    sales_invoice.id_contact = contact.id_contact;
                    sales_invoice.contact = contact;
                    payment.id_contact = contact.id_contact;
                }
            }
        }

        private async void Item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                if (sales_invoiceViewSource.View.CurrentItem is sales_invoice sales_invoice)
                {
                    item item = await SalesDB.db.items.FindAsync(sbxItem.ItemID);
                    item_product item_product = item.item_product.FirstOrDefault();

                    if (item_product != null && item_product.can_expire)
                    {
                        crud_modalExpire.Visibility = Visibility.Visible;
                        cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(sales_invoice.id_branch, null, item.item_product.FirstOrDefault().id_item_product);
                        crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                    }
                    else
                    {
                        decimal QuantityInStock = sbxItem.QuantityInStock;
                        sales_invoice_detail _sales_invoice_detail =
                              SalesDB.Create_Detail(ref sales_invoice, item, null,
                                new Settings().AllowDuplicateItem,
                                sbxItem.QuantityInStock,
                                sbxItem.Quantity);
                    }

                    sales_invoiceViewSource.View.Refresh();
                    CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                    sales_invoicesales_invoice_detailViewSource.View.Refresh();
                    paymentViewSource.View.Refresh();
                }
            }
        }

        #endregion SmartBox Selection

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SalesDB.Initialize();

            New_Sale_Payment();

            //PAYMENT TYPE
            await SalesDB.db.payment_type.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company && a.payment_behavior == payment_type.payment_behaviours.Normal).LoadAsync();
            CollectionViewSource payment_typeViewSource = FindResource("payment_typeViewSource") as CollectionViewSource;
            payment_typeViewSource.Source = SalesDB.db.payment_type.Local;

            cbxSalesRep.ItemsSource = CurrentSession.SalesReps; //await SalesInvoiceDB.sales_rep.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).ToListAsync(); //CurrentSession.Get_SalesRep();

            CollectionViewSource app_currencyViewSource = FindResource("app_currencyViewSource") as CollectionViewSource;
            app_currencyViewSource.Source = CurrentSession.Currencies;

            int Id_Account = CurrentSession.Id_Account;
            app_account app_account = await SalesDB.db.app_account.FindAsync(CurrentSession.Id_Account);

            if (app_account != null)
            {
                //If Account Session has 1 cl_date as null, means Account is still open. If False, means account is closed.
                if (app_account.app_account_session.Where(x => x.cl_date == null).Any() == false)
                {
                    Account_Click(null, null);
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
                Save_Click(sender, e);
            }
        }

        private void Cancel_MouseDown(object sender, EventArgs e)
        {
            SalesDB.CancelAllChanges();
            PaymentDB.CancelAllChanges();

            New_Sale_Payment();

            //Clean up Contact Data.
            sbxContact.Text = "";
            sbxContact.ContactID = 0;

            sbxItem.Text = "";
            sbxItem.ItemID = 0;

            tabContact.IsSelected = true;
        }

        #region Details

        private void PaymentDetail_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;

            payment_detail payment_detail = e.NewItem as payment_detail;
            if (payment_detail != null && payment != null && sales_invoiceViewSource.View.CurrentItem as sales_invoice != null)
            {
                payment_detail.State = EntityState.Added;
                payment_detail.IsSelected = true;
                payment_detail.Default_id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                payment_detail.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                payment_detail.id_currency = CurrentSession.Currency_Default.id_currency;

                payment_detail.id_payment = payment.id_payment;
                payment_detail.payment = payment;
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
            if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                sales_invoice_detail sales_invoice_detail = e.Parameter as sales_invoice_detail;
                payment_detail payment_detail = e.Parameter as payment_detail;

                dgvSalesDetail.CommitEdit();
                dgvPaymentDetail.CommitEdit();

                if (sales_invoice_detail != null)
                {
                    if (sales_invoiceViewSource.View.CurrentItem is sales_invoice sales_invoice)
                    {
                        sales_invoice.sales_invoice_detail.Remove(sales_invoice_detail);

                        if (FindResource("sales_invoicesales_invoice_detailViewSource") is CollectionViewSource sales_invoicesales_invoice_detailViewSource)
                        {
                            if (sales_invoicesales_invoice_detailViewSource.View != null)
                            {
                                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                            }
                        }
                    }
                }
                else if (payment_detail != null)
                {
                    if (paymentViewSource.View.CurrentItem is payment payment)
                    {
                        payment.payment_detail.Remove(payment_detail);
                        paymentViewSource.View.Refresh();

                        if (FindResource("paymentpayment_detailViewSource") is CollectionViewSource paymentpayment_detailViewSource)
                        {
                            if (paymentpayment_detailViewSource.View != null)
                            {
                                paymentpayment_detailViewSource.View.Refresh();
                            }
                        }
                    }
                }
            }
        }

        #endregion Details

        private void GrandTotalsales_DataContextChanged(object sender, EventArgs e)
        {
            if (sales_invoiceViewSource != null && paymentViewSource != null)
            {
                if (sales_invoiceViewSource.View != null && paymentViewSource.View != null)
                {
                    if (sales_invoiceViewSource.View.CurrentItem != null && paymentViewSource.View.CurrentItem != null)
                    {
                        (paymentViewSource.View.CurrentItem as payment).GrandTotal = (sales_invoiceViewSource.View.CurrentItem as sales_invoice).GrandTotal;
                    }
                }
            }
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

        private void Promotion_Click(object sender, EventArgs e)
        {
            sales_invoice Invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            if (Invoice != null)
            {
                SalesDB.Check_Promotions(Invoice);

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = (CollectionViewSource)this.FindResource("sales_invoicesales_invoice_detailViewSource");
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                Invoice.RaisePropertyChanged("GrandTotal");
            }
        }

        private void Expire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                item item = SalesDB.db.items.Find(sbxItem.ItemID);

                cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry = crud_modalExpire.Children.OfType<cntrl.Panels.pnl_ItemMovementExpiry>().FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_invoice != null)
                {

                    if (pnl_ItemMovementExpiry.MovementID > 0)
                    {
                        Settings SalesSettings = new Settings();

                        item_movement item_movement = SalesDB.db.item_movement.Find(pnl_ItemMovementExpiry.MovementID);
                        decimal QuantityInStock = sbxItem.QuantityInStock;

                        sales_invoice_detail _sales_invoice_detail =
                              SalesDB.Create_Detail(ref sales_invoice, item, null,
                                SalesSettings.AllowDuplicateItem,
                                sbxItem.QuantityInStock,
                                sbxItem.Quantity);
                        (sales_invoiceViewSource.View.CurrentItem as sales_invoice).RaisePropertyChanged("GrandTotal");
                    }
                }
                sales_invoiceViewSource.View.Refresh();
                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                paymentViewSource.View.Refresh();

                //Cleans for reuse.
                crud_modalExpire.Children.Clear();
            }
        }
    }
}