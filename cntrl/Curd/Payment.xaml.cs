using System.Collections.Generic;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System;

namespace cntrl.Curd
{
    public partial class Payment : UserControl
    {

        public PaymentDB PaymentDB { get; set; }
        public enum Modes
        {
            Recievable,
            Payable
        }

        private Modes Mode;
        CollectionViewSource paymentpayment_detailViewSource;
        CollectionViewSource paymentViewSource;
        public List<payment_schedual> payment_schedualList { get; set; }

        public Payment(Modes App_Mode, List<payment_schedual> _payment_schedualList, ref PaymentDB PaymentDB)
        {
            InitializeComponent();

            //Setting the Mode for this Window. Result of this variable will determine logic of the certain Behaviours.
            Mode = App_Mode;
            this.PaymentDB = PaymentDB;
            paymentViewSource = (CollectionViewSource)FindResource("paymentViewSource");
            paymentpayment_detailViewSource = (CollectionViewSource)FindResource("paymentpayment_detailViewSource");
            payment_schedualList = _payment_schedualList;

            payment payment = new payment();
            payment = (Mode == Modes.Recievable) ? PaymentDB.New(true) : PaymentDB.New(false);

            PaymentDB.payments.Add(payment);
            paymentViewSource.Source = PaymentDB.payments.Local;

            int id_contact = payment_schedualList.FirstOrDefault().id_contact;
            sbxReturn.ContactID = id_contact;

            entity.contact contacts = PaymentDB.contacts.Find(id_contact);
            if (contacts != null)
            {
                payment.id_contact = contacts.id_contact;
                payment.contact = contacts;
            }

            foreach (var id in payment_schedualList.GroupBy(x => x.app_currencyfx).Select(x => new { x.Key.id_currency }))
            {
                //Get list by Currency, not CurrencyFX as Rates can change. You can buy at 65 INR but pay at 67.
                Add_PaymentDetail(id.id_currency);
            }

            payment.RaisePropertyChanged("GrandTotal");
            payment.RaisePropertyChanged("GrandTotalDetail");

            paymentViewSource.View.MoveCurrentTo(payment);
            paymentpayment_detailViewSource.View.MoveCurrentToFirst();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            await PaymentDB.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();

            //Fix if Payment Type not inserted.
            if (PaymentDB.payment_type.Local.Count == 0)
            {
                entity.payment_type payment_type = new entity.payment_type();
                payment_type.name = "Cash";
                payment_type.is_active = true;
                payment_type.is_default = true;

                PaymentDB.payment_type.Add(payment_type);
            }
            payment_typeViewSource.Source = PaymentDB.payment_type.Local;

            CollectionViewSource app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            await PaymentDB.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();

            //Fix if Payment Type not inserted.
            if (PaymentDB.app_account.Local.Count == 0)
            {
                app_account app_account = new app_account();
                app_account.name = "CashBox";
                app_account.code = "Generic";
                app_account.id_account_type = entity.app_account.app_account_type.Terminal;
                app_account.id_terminal = CurrentSession.Id_Terminal;
                app_account.is_active = true;

                PaymentDB.app_account.Add(app_account);
            }
            app_accountViewSource.Source = PaymentDB.app_account.Local;

            CollectionViewSource salesRepViewSourceCollector = (CollectionViewSource)this.FindResource("salesRepViewSourceCollector");
            salesRepViewSourceCollector.Source = await PaymentDB.sales_rep.Where(a => a.enum_type == sales_rep.SalesRepType.Collector && a.is_active && a.id_company == CurrentSession.Id_Company).ToListAsync();

            if (Mode == Modes.Recievable)
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PaymentDB, App.Names.PaymentUtility, CurrentSession.Id_Branch, CurrentSession.Id_Company);
                stackDocument.Visibility = Visibility.Visible;
            }

            payment payment = paymentViewSource.View.CurrentItem as payment;
            if (payment != null)
            {
                app_account app_account = app_accountViewSource.View.CurrentItem as app_account;
                if (app_account != null)
                {
                    foreach (payment_detail payment_detail in payment.payment_detail)
                    {
                        payment_detail.id_account = app_account.id_account;
                    }
                }
            }
        }

        #region Events
        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        #endregion

        private void SaveChanges(object sender, EventArgs e)
        {
            paymentpayment_detailViewSource.View.Refresh();
            payment payment = paymentViewSource.View.CurrentItem as payment;
            foreach (var id in payment_schedualList.GroupBy(x => x.app_currencyfx).Select(x => new { x.Key.id_currency }))
            {
                Decimal TotalPayable = 0;
                if (Mode==Modes.Recievable)
                {

                    TotalPayable = payment_schedualList.Where(x => x.app_currencyfx.id_currency == id.id_currency).Sum(x => x.AccountReceivableBalance);
                }
                else
                {
                    TotalPayable = payment_schedualList.Where(x => x.app_currencyfx.id_currency == id.id_currency).Sum(x => x.AccountPayableBalance);
                }

                Decimal TotalPaid = payment.payment_detail.Where(x => x.app_currencyfx.id_currency == id.id_currency).Sum(x => x.value);
                if (TotalPaid > TotalPayable)
                {
                    String Currency = PaymentDB.app_currency.Where(x => x.id_currency == id.id_currency).FirstOrDefault().name;
                    MessageBox.Show("Your Amount Is Higher Than :-" + TotalPayable + Currency);
                    return;
                }
            }

                foreach (payment_detail payment_detail in payment.payment_detail.ToList())
            {
                bool IsRecievable = Mode == Modes.Recievable ? true : false;
                PaymentDB.Approve(payment_schedualList, IsRecievable);
            }
            lblCancel_MouseDown(null, null);
        }

        private void cbxPamentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource purchase_returnViewSource = this.FindResource("purchase_returnViewSource") as CollectionViewSource;
            payment payment = paymentViewSource.View.CurrentItem as payment;
            if (cbxPamentType.SelectedItem != null)
            {
                entity.payment_type payment_type = cbxPamentType.SelectedItem as entity.payment_type;
                if (payment_type != null)
                {
                    if (payment_type.payment_behavior == global::entity.payment_type.payment_behaviours.WithHoldingVAT)
                    {
                        //If payment behaviour is WithHoldingVAT, hide everything.
                        stpaccount.Visibility = Visibility.Collapsed;
                        stpcreditpurchase.Visibility = Visibility.Collapsed;
                        stpcreditsales.Visibility = Visibility.Collapsed;
                    }
                    else if (payment_type.payment_behavior == global::entity.payment_type.payment_behaviours.CreditNote)
                    {
                        //If payment behaviour is Credit Note, then hide Account.
                        stpaccount.Visibility = Visibility.Collapsed;

                        //Check Mode. 
                        if (Mode == Modes.Payable)
                        {
                            //If Payable, then Hide->Sales and Show->Payment
                            stpcreditsales.Visibility = Visibility.Collapsed;
                            stpcreditpurchase.Visibility = Visibility.Visible;

                            PaymentDB.purchase_return.Where(x => x.id_contact == payment.id_contact).Include(x => x.payment_schedual).Load();
                            purchase_returnViewSource.Source = PaymentDB.purchase_return.Local.Where(x => (x.payment_schedual.Sum(y => y.debit) < x.payment_schedual.Sum(y => y.credit)));
                        }
                        else
                        {
                            //If Recievable, then Hide->Payment and Show->Sales
                            stpcreditpurchase.Visibility = Visibility.Collapsed;
                            stpcreditsales.Visibility = Visibility.Visible;

                            CollectionViewSource sales_returnViewSource = this.FindResource("sales_returnViewSource") as CollectionViewSource;
                            PaymentDB.sales_return.Where(x => x.id_contact == payment.id_contact).Include(x => x.payment_schedual).Load();
                            sales_returnViewSource.Source = PaymentDB.sales_return.Local.Where(x => (x.payment_schedual.Sum(y => y.credit) < x.payment_schedual.Sum(y => y.debit)));
                        }
                    }
                    else
                    {
                        //If paymentbehaviour is not WithHoldingVAT & CreditNote, it must be Normal, so only show Account.
                        stpaccount.Visibility = Visibility.Visible;
                        stpcreditpurchase.Visibility = Visibility.Collapsed;
                        stpcreditsales.Visibility = Visibility.Collapsed;
                    }

                    //If PaymentType has Document to print, then show Document. Example, Checks or Bank Transfers.
                    if (payment_type.id_document > 0 && paymentpayment_detailViewSource != null && paymentpayment_detailViewSource.View != null)
                    {
                        stpDetailDocument.Visibility = Visibility.Visible;
                        payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;

                        app_document_range app_document_range = PaymentDB.app_document_range.Where(d => d.id_document == payment_type.id_document && d.is_active == true).FirstOrDefault();
                        if (app_document_range != null && payment_detail != null)
                        {
                            payment_detail.id_range = app_document_range.id_range;
                        }
                    }
                    else
                    {
                        stpDetailDocument.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #region Purchase and Sales Returns
        private void sbxPurchaseReturn_Select(object sender, RoutedEventArgs e)
        {
            if (sbxPurchaseReturn.ReturnID > 0)
            {
                CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
                payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
                purchase_return purchase_return = PaymentDB.purchase_return.Find(sbxPurchaseReturn.ReturnID);
                decimal return_value = sbxPurchaseReturn.Balance;
                payment_detail.value = return_value;
                payment_detail.id_purchase_return = purchase_return.id_purchase_return;
                payment_detail.Max_Value = return_value;
                sbxPurchaseReturn.Text = purchase_return.number + "-" + purchase_return.trans_date; ;
            }

        }
        private void sbxReturn_Select(object sender, RoutedEventArgs e)
        {
            if (sbxReturn.ReturnID > 0)
            {
                CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
                payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
                if (payment_detail != null)
                {
                    sales_return sales_return = PaymentDB.sales_return.Find(sbxReturn.ReturnID);
                    decimal return_value = sbxReturn.Balance;
                    payment_detail.id_sales_return = sales_return.id_sales_return;
                    payment_detail.value = return_value;
                    payment_detail.Max_Value = return_value;
                    sbxReturn.Text = sales_return.number + "-" + sales_return.trans_date;
                    sbxReturn.RaisePropertyChanged("Text");
                }
            }

        }

        #endregion

        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            payment_detail payment_detail=paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
            if (payment_detail!=null)
            {
                Add_PaymentDetail(payment_detail.app_currencyfx.id_currency);
            }
        }
         

        private void Add_PaymentDetail(int CurrencyID)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;

            if (payment != null)
            {
                payment_detail payment_detail = new payment_detail();
                payment_detail.payment = payment;

                //Get current Active Rate of selected Currency.
                app_currencyfx app_currencyfx = PaymentDB.app_currencyfx.Where(x => x.id_currency == CurrencyID && x.id_company == CurrentSession.Id_Company && x.is_active).FirstOrDefault();

                if (app_currencyfx != null)
                {
                    payment_detail.Default_id_currencyfx = app_currencyfx.id_currencyfx;
                    payment_detail.id_currencyfx = app_currencyfx.id_currencyfx;
                    payment_detail.payment.id_currencyfx = app_currencyfx.id_currencyfx;
                    payment_detail.app_currencyfx = app_currencyfx;
                }

                payment_detail.IsSelected = true;

                //Always get total value of Accounts Receivable from a particular Currency, and not Currency Rate. This is very important when Currency Fluctates.
                if (Mode == Modes.Recievable)
                {
                    payment_detail.value = payment_schedualList.Where(x => x.app_currencyfx.id_currency == CurrencyID).Sum(x => x.AccountReceivableBalance)
                                                                           - payment.payment_detail.Where(x=>x.app_currencyfx.id_currency==CurrencyID).Sum(x => x.value);

                }
                else
                {
                    payment_detail.value = payment_schedualList.Where(x => x.app_currencyfx.id_currency == CurrencyID).Sum(x => x.AccountPayableBalance)
                                                                            - payment.payment_detail.Where(x => x.app_currencyfx.id_currency == CurrencyID).Sum(x => x.value);

                }

                payment.payment_detail.Add(payment_detail);
                paymentpayment_detailViewSource.View.Refresh();
            }
        }

        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
            PaymentDB.payment_detail.Remove(payment_detail);
            paymentpayment_detailViewSource.View.Refresh();
        }

        private void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
