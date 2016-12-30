using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.ComponentModel;
using System.Data.Entity.Validation;
namespace cntrl.Curd
{
    public partial class receive_payment : UserControl, INotifyPropertyChanged
    {
        dbContext dbContext = new dbContext();

        public sales_invoice sales_invoice { get; set; }
        public purchase_invoice purchase_invoice { get; set; }
        public decimal invoice_total { get; set; }
        public string currency { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public receive_payment()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_accountViewSource = this.FindResource("app_accountViewSource") as CollectionViewSource;
            app_accountViewSource.Source = dbContext.db.app_account.Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company).ToList();

            DateTime start_date = DateTime.Now.AddDays(-2).Date;
            DateTime end_date = DateTime.Now.AddDays(2).Date;

            if (purchase_invoice != null)
            {
                List<payment_schedual> payment_sceduallist = dbContext.db.payment_schedual
                                                                        .Where(x => x.id_payment_detail == null
                                                                            && x.id_purchase_invoice == purchase_invoice.id_purchase_invoice && (x.trans_date >= start_date && x.trans_date <= end_date)
                                                                            && (x.credit - (x.child.Count() > 0 ? x.child.Sum(y => y.debit) : 0)) > 0).ToList();

                if (payment_sceduallist.Count > 0)
                {
                    invoice_total = payment_sceduallist.FirstOrDefault().AccountPayableBalance;
                    if (purchase_invoice.app_currencyfx != null && purchase_invoice.app_currencyfx.app_currency != null)
                        currency = purchase_invoice.app_currencyfx.app_currency.name;
                    else
                        currency = string.Empty;
                }
            }
            else if (sales_invoice != null)
            {
                List<payment_schedual> payment_sceduallist = dbContext.db.payment_schedual
                                                                        .Where(x => x.id_payment_detail == null
                                                                            && x.id_sales_invoice == sales_invoice.id_sales_invoice && (x.trans_date >= start_date && x.trans_date <= end_date)
                                                                            && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0).ToList();
                    
                if (payment_sceduallist.Count > 0)
                {
                    invoice_total = payment_sceduallist.FirstOrDefault().AccountReceivableBalance;
                    if (sales_invoice.app_currencyfx != null && sales_invoice.app_currencyfx.app_currency != null)
                        currency = sales_invoice.app_currencyfx.app_currency.name;
                    else
                        currency = string.Empty;
                }
            }
            else
            {
                invoice_total = 0;
                currency = string.Empty;
            }

            RaisePropertyChanged("invoice_total");
            RaisePropertyChanged("currency");
        }

        private void imgCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (purchase_invoice != null)
            {
                if (dbContext.db.payment_schedual.Where(a => a.id_purchase_invoice == purchase_invoice.id_purchase_invoice && a.id_contact == purchase_invoice.id_contact).FirstOrDefault() != null)
                {
                    payment_schedual payment_schedual = dbContext.db.payment_schedual.Where(a => a.id_purchase_invoice == purchase_invoice.id_purchase_invoice && a.id_contact == purchase_invoice.id_contact).FirstOrDefault();
                    
                    if (invoice_total > 0)
                    {
                        payment_detail payment_detail = new payment_detail();
                        payment_detail.value = invoice_total;
                        payment_detail.id_account = (int)app_accountComboBox.SelectedValue;
                        payment payment = new payment();
                        if (payment_schedual != null)
                        {
                            payment.id_contact = payment_schedual.id_contact;
                            payment.contact = payment_schedual.contact;
                            payment_detail.id_currencyfx = payment_schedual.id_currencyfx;

                            if (dbContext.db.payment_type.Where(x => x.is_default).FirstOrDefault() != null)
                            {
                                payment_detail.id_payment_type = dbContext.db.payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
                            }
                            else
                            {
                                MessageBox.Show("Please insert paymnent Type");
                                return;
                            }

                        }

                        payment_detail.App_Name = global::entity.App.Names.PurchaseInvoice;

                        payment_schedual _payment_schedual = new payment_schedual();
                        _payment_schedual.debit = invoice_total;
                        _payment_schedual.parent = payment_schedual;
                        _payment_schedual.expire_date = payment_schedual.expire_date;
                        _payment_schedual.status = payment_schedual.status;
                        _payment_schedual.id_contact = payment_schedual.id_contact;
                        _payment_schedual.id_currencyfx = payment_schedual.id_currencyfx;
                        _payment_schedual.id_purchase_invoice = payment_schedual.id_purchase_invoice;
                        _payment_schedual.trans_date = DateTime.Now;
                        _payment_schedual.AccountReceivableBalance = invoice_total;

                        payment_detail.payment_schedual.Add(_payment_schedual);
                        payment.payment_detail.Add(payment_detail);

                        //Add Account Logic. With IF FUnction if payment type is Basic Behaviour. If not ignore.
                        app_account_detail app_account_detail = new app_account_detail();
                        if (dbContext.db.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault() != null)
                        {
                            app_account_detail.id_session = dbContext.db.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault().id_session;
                        }
                        app_account_detail.id_account = (int)payment_detail.id_account;
                        app_account_detail.id_currencyfx = payment_schedual.id_currencyfx;
                        app_account_detail.id_payment_type = payment_detail.id_payment_type;
                        app_account_detail.debit = Convert.ToDecimal(payment_detail.value);
                        app_account_detail.credit = 0;
                        dbContext.db.app_account_detail.Add(app_account_detail);

                        dbContext.db.payments.Add(payment);

                        IEnumerable<DbEntityValidationResult> validationresult = dbContext.db.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            dbContext.db.SaveChanges();

                            entity.Brillo.Logic.Document Document = new entity.Brillo.Logic.Document();
                            Document.Document_PrintPaymentReceipt(payment);

                            imgCancel_MouseDown(null, null);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Save and Approve invoice first to Make Payment.", "Cognitivo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            if (sales_invoice != null)
            {
                PaymentDB PaymentDB = new entity.PaymentDB();

                if (PaymentDB.payment_schedual.Where(a => a.id_sales_invoice == sales_invoice.id_sales_invoice && a.id_contact == sales_invoice.id_contact).FirstOrDefault() != null)
                {
                    payment_schedual payment_schedual = PaymentDB.payment_schedual.Where(a => a.id_sales_invoice == sales_invoice.id_sales_invoice && a.id_contact == sales_invoice.id_contact).FirstOrDefault();
              
                    if (invoice_total > 0)
                    {
                        payment_detail payment_detail = new payment_detail();
                        payment_detail.value = invoice_total;
                        payment_detail.id_account = (int)app_accountComboBox.SelectedValue;
                        payment payment = new payment();
                        if (payment_schedual != null)
                        {
                            payment.id_contact = payment_schedual.id_contact;
                            payment.contact = payment_schedual.contact;
                            payment_detail.id_currencyfx = payment_schedual.id_currencyfx;
                            if (PaymentDB.payment_type.Where(x => x.is_default).FirstOrDefault() != null)
                            {
                                payment_detail.id_payment_type = PaymentDB.payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
                            }
                            else
                            {
                                MessageBox.Show("Please insert paymnent Type");
                                return;
                            }

                        }

                        payment_detail.IsSelected = true;
                        payment_detail.App_Name = global::entity.App.Names.SalesInvoice;
                        payment.payment_detail.Add(payment_detail);

                        //payment_schedual _payment_schedual = new payment_schedual();
                        //_payment_schedual.credit = invoice_total;
                        //_payment_schedual.parent = payment_schedual;
                        //_payment_schedual.expire_date = payment_schedual.expire_date;
                        //_payment_schedual.status = payment_schedual.status;
                        //_payment_schedual.id_contact = payment_schedual.id_contact;
                        //_payment_schedual.id_currencyfx = payment_schedual.id_currencyfx;
                        //_payment_schedual.id_sales_invoice = payment_schedual.id_sales_invoice;
                        //_payment_schedual.trans_date = payment_schedual.trans_date;
                        //_payment_schedual.AccountReceivableBalance = invoice_total;

                        //payment_detail.payment_schedual.Add(_payment_schedual);
                        //payment.payment_detail.Add(payment_detail);

                        ////Add Account Logic. With IF FUnction if payment type is Basic Behaviour. If not ignore.
                        //app_account_detail app_account_detail = new app_account_detail();
                        //if (dbContext.db.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault() != null)
                        //{
                        //    app_account_detail.id_session = dbContext.db.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault().id_session;
                        //}
                        //app_account_detail.id_account = (int)payment_detail.id_account;
                        //app_account_detail.id_currencyfx = payment_schedual.id_currencyfx;
                        //app_account_detail.id_payment_type = payment_detail.id_payment_type;
                        //app_account_detail.debit = 0;
                        //app_account_detail.credit = Convert.ToDecimal(payment_detail.value);
                        //dbContext.db.app_account_detail.Add(app_account_detail);

                        PaymentDB.payments.Add(payment);
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        payment_schedualList.Add(payment_schedual);
                        PaymentDB.MakePayment(payment_schedualList, payment,true);
                        imgCancel_MouseDown(null, null);
                   
                    }
                }
                else
                {
                    MessageBox.Show("Please Save and Approve invoice first to Make Payment.", "Cognitivo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

