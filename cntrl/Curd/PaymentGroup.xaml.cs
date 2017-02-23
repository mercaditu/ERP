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
        //  PaymentDB PaymentDB = new PaymentDB();
        

     
        private CollectionViewSource  payment_schedualViewSource;

        public PaymentDB PaymentDB { get; set; }

        public PaymentGroup(ref PaymentDB _PaymentDB)
        {
            InitializeComponent();

          
            PaymentDB = _PaymentDB;
          
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

        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            List<payment_schedual> payment_schedualList = PaymentDB.payment_schedual.Local.Where(x => x.IsSelected).ToList();
            foreach (payment_schedual schedual in payment_schedualList)
            {
                payment payment = new payment();
                payment.trans_date = DateTime.Now;
                payment.id_contact = schedual.id_contact;
                payment.id_branch = CurrentSession.Id_Branch;
                payment.id_user = CurrentSession.Id_User;

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

                payment.payment_detail.Add(payment_detail);
                PaymentDB.payments.Add(payment);
               

                //Payment approval code so that it inserts into Schedual (Balance) and into Account Detail.
                
            }
            PaymentDB.Approve(payment_schedualList, true,true);

          
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

      
        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }
    }
}