using entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd
{
    public partial class PaymentGroup : UserControl
    {
        //  PaymentDB PaymentDB = new PaymentDB();

        public enum Modes
        {
            Recievable,
            Payable
        }

        private Modes Mode;
        private CollectionViewSource paymentpayment_detailViewSource;
        private CollectionViewSource paymentViewSource, payment_schedualViewSource;

        public PaymentDB PaymentDB { get; set; }

        public PaymentGroup(Modes App_Mode, payment _payment, PaymentDB _PaymentDB)
        {
            InitializeComponent();

            Mode = App_Mode;
            PaymentDB = _PaymentDB;
            paymentViewSource = (CollectionViewSource)this.FindResource("paymentViewSource");
            paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");

            paymentViewSource.Source = PaymentDB.payments.Local;

            if (paymentViewSource != null)
            {
                if (paymentViewSource.View != null)
                {
                    paymentViewSource.View.Filter = i =>
                    {
                        int id_payment = _payment.id_payment;
                        payment payment = (payment)i;
                        if (payment.id_payment == id_payment)
                            return true;
                        else
                            return false;
                    };
                }
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");
            await PaymentDB.payment_schedual
                    .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
                        && (x.id_sales_invoice > 0 || x.id_sales_order > 0)
                        && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                        .Include(x => x.sales_invoice)
                        .Include(x => x.contact)
                        .OrderBy(x => x.expire_date)
                        .LoadAsync();
                payment_schedualViewSource.Source = PaymentDB.payment_schedual.Local;

            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            PaymentDB.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Load();
            payment_typeViewSource.Source = PaymentDB.payment_type.Local;

            CollectionViewSource app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            PaymentDB.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Load();
            app_accountViewSource.Source = PaymentDB.app_account.Local;

            paymentViewSource.View.Refresh();
            paymentpayment_detailViewSource.View.Refresh();
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            foreach (payment_schedual schedual in PaymentDB.payment_schedual.Local.Where(x => x.IsSelected))
            {
                payment payment = new payment();
                payment.trans_date = DateTime.Now;
                payment.id_contact = schedual.id_contact;
                payment.id_branch = CurrentSession.Id_Branch;
                payment.id_user = CurrentSession.Id_User;

                payment_detail detail = new payment_detail();
                detail.value = schedual.AccountReceivableBalance;
                //Change this. We should get current active FX Rate for the same currency in Schedual. But not same fx rate as schedual.
                detail.id_currencyfx = 1;
                detail.id_account = (int)cbxPamentType.SelectedItem; //this is wrong. give Account Cbx a Name and Linked it here.
                detail.id_payment_type = (int)cbxPamentType.SelectedItem;

                //Only one detail per customer. see if you can group by customer.


            //Payment approval code so that it inserts into Schedual (Balance) and into Account Detail.
            }
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int ConditionID = Convert.ToInt32(cbxCondition.SelectedIndex);

            if (payment_schedualViewSource.View != null && ConditionID > 0)
            {
                payment_schedualViewSource.View.Filter = i =>
                {
                    payment_schedual payment_schedual = i as payment_schedual;
                    if (payment_schedual.AccountReceivableBalance > 0 &&
                        payment_schedual.sales_invoice.id_condition == ConditionID)
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

        private void dgvPaymentDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
            paymentpayment_detailViewSource.View.MoveCurrentTo(payment_detail);
        }
    }
}