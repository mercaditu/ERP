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
    public partial class PaymentGroup : UserControl
    {
        private CollectionViewSource payment_schedualViewSource;
        private CollectionViewSource app_accountViewSource;
        public PaymentDB PaymentDB { get; set; }

        public PaymentGroup(ref PaymentDB _PaymentDB)
        {
            InitializeComponent();
            PaymentDB = _PaymentDB;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");
            payment_schedualViewSource.Source = await PaymentDB.payment_schedual
                    .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
                        && (x.id_sales_invoice > 0 || x.id_sales_order > 0)
                        && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                        .Include(x => x.sales_invoice)
                        .Include(x => x.contact)
                        .OrderBy(x => x.expire_date)
                        .ToListAsync();

            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            await PaymentDB.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            payment_typeViewSource.Source = PaymentDB.payment_type.Local;

            app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            await PaymentDB.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company &&
                    a.id_account_type == app_account.app_account_type.Bank || a.id_account == CurrentSession.Id_Account).LoadAsync();
            app_accountViewSource.Source = PaymentDB.app_account.Local;

            CollectionViewSource app_conditionViewSource = (CollectionViewSource)this.FindResource("app_conditionViewSource");
            await PaymentDB.app_condition.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            app_conditionViewSource.Source = PaymentDB.app_condition.Local;

            dtpPaymentDate.SelectedDate = DateTime.Now;
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            List<payment_schedual> payment_schedualList = PaymentDB.payment_schedual.Local.Where(x => x.IsSelected).ToList();
            foreach (payment_schedual schedual in payment_schedualList)
            {
                payment payment = new payment();
                payment.trans_date = dtpPaymentDate.SelectedDate != null ? (DateTime)dtpPaymentDate.SelectedDate : DateTime.Now;
                payment.id_contact = schedual.id_contact;
                payment.id_branch = CurrentSession.Id_Branch;
                payment.id_user = CurrentSession.Id_User;
                payment.IsSelected = true;
                payment.status = Status.Documents_General.Pending;

                payment_detail payment_detail = new payment_detail();
                //Only one detail per customer. see if you can group by customer.
                payment_detail.payment = payment;

                //Get current Active Rate of selected Currency.
                app_currencyfx app_currencyfx = PaymentDB.app_currencyfx.Where(x => x.id_currency == schedual.app_currencyfx.id_currency && x.id_company == CurrentSession.Id_Company && x.is_active).FirstOrDefault();
                if (app_currencyfx != null)
                {
                    payment_detail.Default_id_currencyfx = app_currencyfx.id_currencyfx;
                    payment_detail.id_currencyfx = app_currencyfx.id_currencyfx;
                    payment_detail.payment.id_currencyfx = app_currencyfx.id_currencyfx;
                    payment_detail.app_currencyfx = app_currencyfx;
                }

                payment_detail.value = schedual.AccountReceivableBalance;
                payment_detail.IsLocked = false;
                payment_detail.id_account = (int)cbxAccount.SelectedValue;
                payment_detail.id_payment_type = (int)cbxPamentType.SelectedValue;
                payment_detail.IsSelected = true;
                payment_detail.comment = txtComment.Text;
                payment_detail.id_payment_schedual = schedual.id_payment_schedual;
                payment.payment_detail.Add(payment_detail);
                PaymentDB.payments.Add(payment);
                List<payment_schedual> listschedual = new List<payment_schedual>();
                listschedual.Add(schedual);
                PaymentDB.Approve(listschedual, true, false,false);

                //dispatcher
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    schedual.IsSelected = false;
                }));
            }

            //Reloads all data again, to clean old values.
            UserControl_Loaded(null, null);
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter_Changed();
        }

        private void dpDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            Filter_Changed();
        }

        private void Filter_Changed()
        {
            DateTime Date = Convert.ToDateTime(dpDate.SelectedDate);
            app_condition app_condition = cbxCondition.SelectedItem as app_condition;

            if (payment_schedualViewSource.View != null && Date != null && app_condition != null)
            {
                payment_schedualViewSource.View.Filter = i =>
                {
                    payment_schedual payment_schedual = i as payment_schedual;
                    if (payment_schedual.AccountReceivableBalance > 0 &&
                        payment_schedual.expire_date.Date == Date.Date &&
                        payment_schedual.sales_invoice.id_condition == app_condition.id_condition)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }

        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void chbxSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (check.IsChecked == true)
            {
                foreach (payment_schedual schedual in payment_schedualViewSource.View)
                {
                    schedual.IsSelected = true;
                }
            }
            else
            {
                foreach (payment_schedual schedual in payment_schedualViewSource.View)
                {
                    schedual.IsSelected = false;
                }
            }
        }

        private void dtpPaymentDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (dtpPaymentDate.SelectedDate != null)
            {
                List<int> app_account_sessionList = PaymentDB.app_account_session.Where(y => y.is_active && y.op_date < dtpPaymentDate.SelectedDate).Select(x => x.id_account).ToList();
                List<app_account> app_accountList = PaymentDB.app_account.Where(x => app_account_sessionList.Contains(x.id_account)).ToList();
                app_accountViewSource.Source = app_accountList;
            }
        }
    }
}