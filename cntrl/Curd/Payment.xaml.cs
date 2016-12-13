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

            foreach (var id in payment_schedualList.GroupBy(x => x.app_currencyfx).Select(x => new { x.Key.id_currencyfx }))
            {

                int id_currencyfx = (int)id.id_currencyfx;
                payment_detail payment_detail = new payment_detail();
                payment_detail.payment = payment;


                app_currencyfx app_currencyfx = PaymentDB.app_currencyfx.Find(id_currencyfx);
                if (app_currencyfx != null)
                {
                    payment_detail.id_currencyfx = id_currencyfx;
                    payment_detail.payment.id_currencyfx = id_currencyfx;
                    payment_detail.app_currencyfx = app_currencyfx;
                }


                payment_detail.IsSelected = true;



                if (Mode == Modes.Recievable)
                {
                    payment_detail.value = payment_schedualList.Where(x => x.id_currencyfx == id_currencyfx).Sum(x => x.AccountReceivableBalance);
                   
                   
                }
                else
                {
                    payment_detail.value = payment_schedualList.Where(x => x.id_currencyfx == id_currencyfx).Sum(x => x.AccountPayableBalance);
                
                }


                payment.payment_detail.Add(payment_detail);
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
            PaymentDB.payment_detail.RemoveRange(payment.payment_detail.Where(x => x.IsSelected == false));
            PaymentDB.SaveChanges();
            List<payment_detail> payment_detailList = payment.payment_detail.Where(x => x.IsSelected).ToList();
            foreach (payment_detail _payment_detail in payment_detailList)
            {

                decimal amount = _payment_detail.value;




                foreach (payment_schedual payment_schedual in payment_schedualList.Where(x => x.id_currencyfx == _payment_detail.id_currencyfx))
                {
                    if (amount > 0)
                    {


                        int id_currencyfx = payment_schedual.id_currencyfx;
                        payment_detail payment_detail = new payment_detail();
                        payment_detail.payment = payment;


                        app_currencyfx app_currencyfx = PaymentDB.app_currencyfx.Find(id_currencyfx);
                        if (app_currencyfx != null)
                        {
                            payment_detail.id_currencyfx = id_currencyfx;
                            payment_detail.payment.id_currencyfx = id_currencyfx;
                            payment_detail.app_currencyfx = app_currencyfx;
                        }


                        payment_detail.IsSelected = true;





                        payment_detail.value = amount;

                        payment_detail.id_payment_schedual = payment_schedual.id_payment_schedual;
                        payment_detail.IsSelected = true;
                        payment.payment_detail.Add(payment_detail);

                    }
                }
                payment.payment_detail.Remove(_payment_detail);


            }
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
                decimal return_value = (purchase_return.GrandTotal - purchase_return.payment_schedual.Where(x => x.id_sales_return == purchase_return.id_purchase_return).Sum(x => x.debit));
                payment_detail.id_purchase_return = purchase_return.id_purchase_return;
                payment_detail.Max_Value = return_value;
                sbxPurchaseReturn.Text = purchase_return.contact.name;
            }

        }
        private void sbxReturn_Select(object sender, RoutedEventArgs e)
        {
            if (sbxReturn.ReturnID > 0)
            {
                CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
                payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
                sales_return sales_return = PaymentDB.sales_return.Find(sbxReturn.ReturnID);
                decimal return_value = (sales_return.GrandTotal - sales_return.payment_schedual.Where(x => x.id_sales_return == sales_return.id_sales_return).Sum(x => x.credit));
                payment_detail.id_sales_return = sales_return.id_sales_return;
                payment_detail.Max_Value = return_value;
                sbxReturn.Text = sales_return.contact.name;
            }

        }

        #endregion

        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            payment payment = paymentViewSource.View.CurrentItem as payment;
            payment_detail payment_detail = new payment_detail();
            payment_detail.id_payment = payment.id_payment;
            payment_detail.payment = payment;
            payment_detail.IsSelected = true;

            int id_currencyfx = payment_schedualList.FirstOrDefault().id_currencyfx;
            if (PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
            {
                payment_detail.id_currency = PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault().id_currency;
                payment_detail.id_currencyfx = id_currencyfx;
                payment_detail.app_currencyfx = PaymentDB.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault();
            }
            int id_payment_schedual = payment_schedualList.FirstOrDefault().id_payment_schedual;
            if (PaymentDB.payment_schedual.Where(x => x.id_payment_schedual == id_payment_schedual).FirstOrDefault() != null)
            {
                payment_schedual _payment_schedual = PaymentDB.payment_schedual.Where(x => x.id_payment_schedual == id_payment_schedual).FirstOrDefault();
                payment_detail.payment_schedual.Add(_payment_schedual);

            }
            payment_detail.value = payment_detail.ValueInDefaultCurrency;
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

        private void btnEditDetail_Click(object sender, RoutedEventArgs e)
        {

        }

    
    }
}
