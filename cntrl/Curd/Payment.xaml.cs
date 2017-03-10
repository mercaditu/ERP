using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


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
        private CollectionViewSource paymentpayment_detailViewSource;
        private CollectionViewSource paymentViewSource;
        CollectionViewSource app_accountViewSource;
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

            int id_contact = payment_schedualList.Select(x => x.id_contact).FirstOrDefault();
            sbxReturn.ContactID = id_contact;

            entity.contact contacts = PaymentDB.contacts.Find(id_contact);
            if (contacts != null)
            {
                payment.id_contact = contacts.id_contact;
                payment.contact = contacts;
            }

            foreach (var id in payment_schedualList.Where(x => x.payment_approve_detail != null)
                .GroupBy(x => new
                {
                    payment_type = x.payment_approve_detail.id_payment_type,
                    Account = x.payment_approve_detail.id_account,
                    Currency = x.payment_approve_detail.id_currency
                }).Select(x => new { x.Key.payment_type, x.Key.Account, x.Key.Currency }))
            {
                //Get list by Currency, not CurrencyFX as Rates can change. You can buy at 65 INR but pay at 67.
                Add_PaymentDetail(id.Currency, id.payment_type, id.Account);
            }

            foreach (var id in payment_schedualList.Where(x => x.payment_approve_detail == null)
                .GroupBy(x => x.app_currencyfx).Select(x => new { x.Key.id_currency }))
            {
                //Get list by Currency, not CurrencyFX as Rates can change. You can buy at 65 INR but pay at 67.
                Add_PaymentDetail(id.id_currency, null, null);
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

            app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            await PaymentDB.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company &&
            (a.id_account_type == app_account.app_account_type.Bank || a.id_account == CurrentSession.Id_Account)).LoadAsync();

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

        #endregion Events

        private void SaveChanges(object sender, EventArgs e)
        {
            paymentpayment_detailViewSource.View.Refresh();
            payment payment = paymentViewSource.View.CurrentItem as payment;
            foreach (var id in payment_schedualList.GroupBy(x => x.app_currencyfx).Select(x => new { x.Key.id_currency }))
            {
                Decimal TotalPayable = 0;
                if (Mode == Modes.Recievable)
                {
                    TotalPayable = payment_schedualList.Where(x => x.app_currencyfx.id_currency == id.id_currency).Sum(x => x.AccountReceivableBalance);
                }
                else
                {
                    TotalPayable = payment_schedualList.Where(x => x.app_currencyfx.id_currency == id.id_currency).Sum(x => x.AccountPayableBalance);
                }

                Decimal TotalPaid = 0;
                foreach (payment_detail payment_detail in payment.payment_detail.Where(x => x.app_currencyfx.app_currency.id_currency == id.id_currency).ToList())
                {
                    TotalPaid += entity.Brillo.Currency.convert_Values(payment_detail.value, payment_detail.id_currencyfx, payment_detail.Default_id_currencyfx, App.Modules.Sales);
                }
                if (Math.Round(TotalPaid) > Math.Round(TotalPayable))
                {
                    String Currency = PaymentDB.app_currency.Where(x => x.id_currency == id.id_currency).FirstOrDefault().name;
                    MessageBox.Show("Your Amount Is Higher Than :-" + TotalPayable + Currency);
                    return;
                }
            }


            bool IsRecievable = Mode == Modes.Recievable ? true : false;
            bool IsPrintable = Mode == Modes.Recievable ? true : false;
            PaymentDB.Approve(payment_schedualList, IsRecievable, IsPrintable);

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
                        stptransdate.Visibility = Visibility.Visible;
                    }
                    else if (payment_type.payment_behavior == global::entity.payment_type.payment_behaviours.CreditNote)
                    {
                        //If payment behaviour is Credit Note, then hide Account.
                        stpaccount.Visibility = Visibility.Collapsed;
                        stptransdate.Visibility = Visibility.Visible;
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
                        if (payment_type.is_direct)
                        {
                            stptransdate.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            stptransdate.Visibility = Visibility.Visible;
                        }

                        stpcreditpurchase.Visibility = Visibility.Collapsed;
                        stpcreditsales.Visibility = Visibility.Collapsed;
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

        #endregion Purchase and Sales Returns

        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
            if (payment_detail != null)
            {
                Add_PaymentDetail(payment_detail.app_currencyfx.id_currency, null, null);
            }
        }

        private void Add_PaymentDetail(int CurrencyID, int? PaymentTypeID, int? AccountID)
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

                //Always get total value of Accounts Receivable from a particular Currency, and not Currency Rate. This is very important when Currency Fluctates.
                if (Mode == Modes.Recievable)
                {
                    payment_detail.value = payment_schedualList.Where(x => x.app_currencyfx.id_currency == CurrencyID).Sum(x => x.AccountReceivableBalance)
                                                                           - payment.payment_detail.Where(x => x.app_currencyfx.id_currency == CurrencyID).Sum(x => x.value);
                }
                else
                {
                    payment_detail.value = payment_schedualList.Where(x => x.app_currencyfx.id_currency == CurrencyID && x.payment_approve_detail == null).Sum(x => x.AccountPayableBalance)
                                                                            - payment.payment_detail.Where(x => x.app_currencyfx.id_currency == CurrencyID && x.IsLocked == false).Sum(x => x.value);
                }

                //If PaymentTypeID is not null, then this transaction has a PaymentApproval
                if (AccountID != null && PaymentTypeID != null)
                {
                    payment_detail.IsLocked = true;
                    payment_detail.id_account = (int)AccountID;
                    payment_detail.id_payment_type = (int)PaymentTypeID;
                    //Over wright Detail Value with Approved Value
                    payment_detail.value = payment_schedualList
                        .Where(x => x.payment_approve_detail != null &&
                                    x.payment_approve_detail.id_currency == CurrencyID &&
                                    x.payment_approve_detail.id_account == (int)AccountID &&
                                    x.payment_approve_detail.id_payment_type == PaymentTypeID)
                        .Sum(x => x.payment_approve_detail.value);
                }
                else
                {
                    payment_detail.IsLocked = false;
                    payment_detail.id_account = CurrentSession.Id_Account;
                }

                payment_detail.IsSelected = true;

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

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtptransdate.SelectedDate != null)
            {
                List<int> app_account_sessionList = PaymentDB.app_account_session.Where(y => y.is_active && y.op_date < dtptransdate.SelectedDate).Select(x => x.id_account).ToList();
                List<app_account> app_accountList = PaymentDB.app_account.Where(x => app_account_sessionList.Contains(x.id_account)).ToList();
                app_accountViewSource.Source = app_accountList;
            }
        }
    }
}