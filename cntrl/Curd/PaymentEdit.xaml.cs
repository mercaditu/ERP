using entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Curd
{
    public partial class PaymentEdit : UserControl
    {
        //  PaymentDB PaymentDB = new PaymentDB();

        public enum Modes
        {
            Recievable,
            Payable
        }

        private Modes Mode;
        private CollectionViewSource paymentpayment_detailViewSource;
        private CollectionViewSource paymentViewSource;
        private CollectionViewSource app_accountViewSource;

        public PaymentDB PaymentDB { get; set; }

        public PaymentEdit(Modes App_Mode, payment _payment, PaymentDB _PaymentDB)
        {
            InitializeComponent();

            //Setting the Mode for this Window. Result of this variable will determine logic of the certain Behaviours.
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            PaymentDB.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Load();
            payment_typeViewSource.Source = PaymentDB.payment_type.Local;

            app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            PaymentDB.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Load();
            app_accountViewSource.Source = PaymentDB.app_account.ToList();

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PaymentDB, App.Names.PaymentUtility, CurrentSession.Id_Branch, CurrentSession.Id_Company);

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

        #endregion Events

        private void SaveChanges()
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;
            foreach (payment_detail payment_detail in payment.payment_detail)
            {
                if (PaymentDB.payment_schedual.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault() != null)
                {
                    payment_schedual payment_schedual = PaymentDB.payment_schedual.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault();
                    if (payment_detail.value != payment_schedual.credit)
                    {
                        payment_schedual.credit = payment_detail.value;
                    }
                }
                if (PaymentDB.app_account_detail.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault() != null)
                {
                    app_account_detail app_account_detail = PaymentDB.app_account_detail.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault();
                    if (payment_detail.value != app_account_detail.credit)
                    {
                        app_account_detail.credit = payment_detail.value;
                    }
                }
            }

            lblCancel_MouseDown(null, null);
        }

        private void cbxPamentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

                            CollectionViewSource purchase_returnViewSource = this.FindResource("purchase_returnViewSource") as CollectionViewSource;
                            PaymentDB.purchase_return.Where(x => x.id_contact == payment.id_contact).Load();
                            purchase_returnViewSource.Source = PaymentDB.purchase_return.Local.Where(x => (x.purchase_invoice.GrandTotal - x.GrandTotal) > 0);
                        }
                        else
                        {
                            //If Recievable, then Hide->Payment and Show->Sales
                            stpcreditpurchase.Visibility = Visibility.Collapsed;
                            stpcreditsales.Visibility = Visibility.Visible;

                            CollectionViewSource sales_returnViewSource = this.FindResource("sales_returnViewSource") as CollectionViewSource;
                            PaymentDB.sales_return.Where(x => x.id_contact == payment.id_contact).Load();
                            sales_returnViewSource.Source = PaymentDB.sales_return.Local.Where(x => (x.sales_invoice.GrandTotal - x.GrandTotal) > 0);
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
                        payment_detail.id_range = PaymentDB.app_document_range.Where(d => d.id_document == payment_type.id_document && d.is_active == true).Include(i => i.app_document).FirstOrDefault().id_range;
                    }
                    else
                    {
                        stpDetailDocument.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #region Purchase and Sales Returns

        private void purchasereturnComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                purchasereturnComboBox_MouseDoubleClick(null, null);
            }
        }

        private void purchasereturnComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                try
                {
                    if (purchasereturnComboBox.Data != null)
                    {
                        CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
                        payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
                        purchase_return purchase_return = (purchase_return)purchasereturnComboBox.Data;
                        purchasereturnComboBox.Text = purchase_return.number;
                        decimal return_value = (purchase_return.GrandTotal - purchase_return.payment_schedual.Where(x => x.id_purchase_return == purchase_return.id_purchase_return).Sum(x => x.debit));
                        payment_detail.id_purchase_return = purchase_return.id_purchase_return;

                        if (payment_detail.value > return_value)
                        {
                            payment_detail.value = return_value;
                            payment_detail.RaisePropertyChanged("value");
                        }
                        else
                        {
                            payment_detail.value = payment_detail.value;
                            payment_detail.RaisePropertyChanged("value");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void salesreturnComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                salesreturnComboBox_MouseDoubleClick(null, null);
            }
        }

        private void salesreturnComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (salesreturnComboBox.Data != null)
                {
                    CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
                    payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
                    sales_return sales_return = (sales_return)salesreturnComboBox.Data;
                    salesreturnComboBox.Text = sales_return.number;
                    decimal return_value = (sales_return.GrandTotal - sales_return.payment_schedual.Where(x => x.id_sales_return == sales_return.id_sales_return).Sum(x => x.credit));
                    payment_detail.id_sales_return = sales_return.id_sales_return;

                    if (payment_detail.value > return_value)
                    {
                        payment_detail.value = return_value;
                        payment_detail.RaisePropertyChanged("value");
                    }
                    else
                    {
                        payment_detail.value = payment_detail.value;
                        payment_detail.RaisePropertyChanged("value");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Purchase and Sales Returns

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detailold = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
            payment_detail payment_detail = new payment_detail();
            payment_detail.payment = payment;
            payment_detail.value = 0;

            int id_currencyfx = payment_detailold.payment_schedual.FirstOrDefault().id_currencyfx;
            if (PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
            {
                payment_detail.id_currencyfx = id_currencyfx;
                payment_detail.app_currencyfx = PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault();
            }
            int id_payment_schedual = payment_detailold.payment_schedual.FirstOrDefault().id_payment_schedual;
            if (PaymentDB.payment_schedual.Where(x => x.id_payment_schedual == id_payment_schedual).FirstOrDefault() != null)
            {
                payment_schedual _payment_schedual = PaymentDB.payment_schedual.Where(x => x.id_payment_schedual == id_payment_schedual).FirstOrDefault();
                payment_detail.payment_schedual.Add(_payment_schedual);
            }

            payment.payment_detail.Add(payment_detail);
            paymentpayment_detailViewSource.View.Refresh();
        }

        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
            payment.payment_detail.Remove(payment_detail);
            paymentpayment_detailViewSource.View.Refresh();
        }

        private void cbxPaymentSchedual_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
            //if (payment_detail != null)
            //{
            //    if (cbxPaymentSchedual.SelectedItem != null)
            //    {
            //        payment_detail.payment_schedual.Add(cbxPaymentSchedual.SelectedItem as payment_schedual);
            //    }
            //}
        }

        private void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {
        }

        private void dgvPaymentDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;

            paymentpayment_detailViewSource.View.MoveCurrentTo(payment_detail);
        }
    }
}