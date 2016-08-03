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
    public partial class Payment : UserControl
    {
        PaymentDB PaymentDB = new PaymentDB();

        public enum Modes
        {
            Recievable,
            Payable
        }

        private Modes Mode;
        CollectionViewSource paymentpayment_detailViewSource;
        CollectionViewSource paymentViewSource;
        public List<payment_schedual> payment_schedualList { get; set; }

        public Payment(Modes App_Mode, List<payment_schedual> _payment_schedualList)
        {
            InitializeComponent();

            //Setting the Mode for this Window. Result of this variable will determine logic of the certain Behaviours.
            Mode = App_Mode;

            paymentViewSource = (CollectionViewSource)this.FindResource("paymentViewSource");
            paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
            payment_schedualList = _payment_schedualList;
            payment payment = PaymentDB.New();
            PaymentDB.payments.Add(payment);
            paymentViewSource.Source = PaymentDB.payments.Local;
            
            if (Mode == Modes.Recievable)
            {
                payment.GrandTotal = payment_schedualList.Sum(x => x.AccountReceivableBalance);
            }
            else
            {
                payment.GrandTotal = payment_schedualList.Sum(x => x.AccountPayableBalance);
            }

            int id_contact = payment_schedualList.FirstOrDefault().id_contact;
            
            if (PaymentDB.contacts.Where(x => x.id_contact == id_contact).FirstOrDefault() != null)
            {
                payment.id_contact = id_contact;
                payment.contact = PaymentDB.contacts.Where(x => x.id_contact == id_contact).FirstOrDefault();
            }

            foreach (payment_schedual payment_schedual in payment_schedualList)
            {
                payment_detail payment_detail = new payment_detail();
                payment_detail.IsSelected = true;
                payment_detail.payment = payment;

                int id_currencyfx = _payment_schedualList.FirstOrDefault().id_currencyfx;

                if (PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
                {
                    payment_detail.id_currencyfx = id_currencyfx;
                    payment_detail.app_currencyfx = PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault();
                }
                if (Mode == Modes.Recievable)
                {
                    payment_detail.value = payment_schedual.AccountReceivableBalance;
                }
                else
                {
                    payment_detail.value = payment_schedual.AccountPayableBalance;
                }

               

                payment_detail.id_payment_schedual = payment_schedual.id_payment_schedual;
              
                payment.payment_detail.Add(payment_detail);
            }

            paymentViewSource.View.MoveCurrentTo(payment);
            paymentpayment_detailViewSource.View.MoveCurrentToFirst();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            PaymentDB.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Load();
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
            PaymentDB.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Load();

            //Fix if Payment Type not inserted.
            if (PaymentDB.app_account.Local.Count == 0)
            {
                entity.app_account app_account = new entity.app_account();
                app_account.name = "CashBox";
                app_account.code = "Generic";
                app_account.id_account_type = entity.app_account.app_account_type.Terminal;
                app_account.id_terminal = CurrentSession.Id_Terminal;
                app_account.is_active = true;

                PaymentDB.app_account.Add(app_account);
            }
            app_accountViewSource.Source = PaymentDB.app_account.Local;

            if (Mode == Modes.Recievable)
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(App.Names.PaymentUtility, CurrentSession.Id_Branch, CurrentSession.Id_Company);
                stackDocument.Visibility = System.Windows.Visibility.Visible;
            }

            paymentViewSource.View.Refresh();
            paymentpayment_detailViewSource.View.Refresh();
            payment payment = paymentViewSource.View.CurrentItem as payment;
            if (payment!=null)
            {
                app_account app_account = app_accountViewSource.View.CurrentItem as app_account;
                if (app_account!=null)
                {
                    foreach (payment_detail payment_detail in payment.payment_detail)
                    {
                        payment_detail.id_account = app_account.id_account;
                    }
                }
            }
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

        private void SaveChanges()
        {
            paymentpayment_detailViewSource.View.Refresh();
            payment payment = paymentViewSource.View.CurrentItem as payment;

            if (payment.payment_detail.Where(x=>x.IsSelected).Count()>0)
            {
                PaymentDB.payment_detail.RemoveRange(payment.payment_detail.Where(x => x.IsSelected == false));
                PaymentDB.SaveChanges();
                foreach (payment_detail payment_detail in payment.payment_detail.Where(x => x.IsSelected))
                {
                    if (Mode == Modes.Recievable)
                    {
                        PaymentDB.Approve(payment_detail.id_payment_schedual, true);
                    }
                    else
                    {
                        PaymentDB.Approve(payment_detail.id_payment_schedual, false);
                    }
                }
                lblCancel_MouseDown(null, null);
            }
            else
            {
                MessageBox.Show("Please select Payment..");
            }
         
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
                            sales_returnViewSource.Source = PaymentDB.sales_return.Local.Where(x=>(x.sales_invoice.GrandTotal - x.GrandTotal) > 0);
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

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            
        }



        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = new payment_detail();
            payment_detail.payment = payment;
            payment_detail.value = 0;


            int id_currencyfx = payment_schedualList.FirstOrDefault().id_currencyfx;
            if (PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
            {
                payment_detail.id_currencyfx = id_currencyfx;
                payment_detail.app_currencyfx = PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault();
            }
            int id_payment_schedual = payment_schedualList.FirstOrDefault().id_payment_schedual;
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
