using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace cntrl.Curd
{
    public partial class payment_quick : UserControl
    {
        PaymentDB PaymentDB = new PaymentDB();


        CollectionViewSource paymentpayment_detailViewSource;
        CollectionViewSource paymentViewSource;
        CollectionViewSource payment_schedualViewSource;

        

        public payment_quick( payment_schedual _payment_schedual)
        {
            InitializeComponent();

            //Setting the Mode for this Window. Result of this variable will determine logic of the certain Behaviours.

            payment_schedualViewSource = (CollectionViewSource)this.FindResource("payment_schedualViewSource");
            paymentViewSource = (CollectionViewSource)this.FindResource("paymentViewSource");
            paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
            paymentpayment_detailViewSource.Source = _payment_schedual;

            payment payment = PaymentDB.New(true);
            PaymentDB.payments.Add(payment);
            paymentViewSource.Source = PaymentDB.payments.Local;

            int id_contact = _payment_schedual.id_contact;
            if (PaymentDB.contacts.Where(x => x.id_contact == id_contact).FirstOrDefault() != null)
            {
                payment.id_contact = id_contact;
                payment.contact = PaymentDB.contacts.Where(x => x.id_contact == id_contact).FirstOrDefault();
            }

            payment_detail payment_detail = new payment_detail();
            payment_detail.payment = payment;
            payment_detail.value = _payment_schedual.AccountPayableBalance;
            payment.payment_detail.Add(payment_detail);





            paymentViewSource.View.MoveCurrentTo(payment);
            paymentpayment_detailViewSource.View.MoveCurrentToFirst();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            PaymentDB.payment_type.Where(a => a.is_active).Load();
            payment_typeViewSource.Source = PaymentDB.payment_type.Local;

            CollectionViewSource app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            PaymentDB.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Load();
            app_accountViewSource.Source = PaymentDB.app_account.Local;

            //CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
            //List<payment_detail> payment_detailList = new List<payment_detail>();
            //payment_detailList.Add(payment_detail);
            //paymentpayment_detailViewSource.Source = payment_detailList;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PaymentDB, App.Names.Payment, CurrentSession.Id_Branch, CurrentSession.Id_Company);

            //paymentpayment_detailViewSource.View.Refresh();
            //paymentpayment_detailViewSource.View.MoveCurrentToLast();



            paymentViewSource.View.Refresh();
            paymentpayment_detailViewSource.View.Refresh();
        }

        #region Events



        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        #endregion







        private void Button_Click(object sender, RoutedEventArgs e)
        {
            payment_schedual payment_schedual = payment_schedualViewSource.View.CurrentItem as payment_schedual;
            payment_schedual.status = Status.Documents_General.Approved;
            PaymentDB.SaveChanges();
            lblCancel_MouseDown(sender, null);
        }




    }
}
