using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;
using System.Windows.Input;

namespace Cognitivo.Commercial
{
    public partial class AccountsPayable : Page
    {
        dbContext dbContext = new dbContext();

        int id_comapny = entity.Properties.Settings.Default.company_ID;

        CollectionViewSource contactViewSource;
        CollectionViewSource payment_schedualViewSource;

       // List<contact> contactLIST;
        cntrl.Curd.payment_quick payment_quick = new cntrl.Curd.payment_quick();
        cntrl.Curd.Refinance Refinance = new cntrl.Curd.Refinance();
        cntrl.VATWithholding VATWithholding = new cntrl.VATWithholding();
        public AccountsPayable()
        {
            InitializeComponent();
        }

        private void toolBar_btnApprove_Click(object sender)
        {

        }

        private void toolBar_btnAnull_Click(object sender)
        {

        }

        private void purchase_returnDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact.id_contact > 0 && payment_schedualViewSource != null)
            {
                payment_schedualViewSource.View.Filter = i =>
                {
                    payment_schedual payment_schedual = i as payment_schedual;
                    if (payment_schedual.id_contact == contact.id_contact)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
            else
            {
                contactViewSource.View.Filter = null;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load_Schedual();

            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            List<contact> contactLIST = new List<contact>();

            foreach (payment_schedual payment in dbContext.db.payment_schedual.Local.ToList())
            {
                if (contactLIST.Contains(payment.contact) == false)
                {
                    contact contact = new contact();
                    contact = payment.contact;
                    contactLIST.Add(contact);
                }
            }

            contactViewSource.Source = contactLIST;
        }

        private async void load_Schedual()
        {
            payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");
            payment_schedualViewSource.Source = await dbContext.db.payment_schedual
                                                                    .Where(x => x.id_payment_detail == null && x.id_company == id_comapny
                                                                       && (x.id_purchase_invoice > 0 || x.id_purchase_order > 0) 
                                                                       && (x.credit -( x.child.Count()>0 ? x.child.Sum(y=>y.debit):0)) > 0)
                                                                    .ToListAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<payment_schedual> PaymentSchedualList = payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList();
            decimal total = PaymentSchedualList.Sum(x => x.AccountPayableBalance);

            payment_quick.payment_detail = new payment_detail();
            payment_quick.payment_detail.id_purchase_return = 0;
            payment_quick.payment_detail.id_sales_return = 0;
            payment_quick.payment_detail.value = total;
            payment_quick.payment_detail.payment = new payment();
            if (PaymentSchedualList.Count == 1)
            {
                payment_quick.payment_detail.payment.id_contact = PaymentSchedualList.FirstOrDefault().id_contact;
                payment_quick.payment_detail.payment.contact = PaymentSchedualList.FirstOrDefault().contact;
                payment_quick.payment_detail.id_currencyfx = PaymentSchedualList.FirstOrDefault().id_currencyfx;
               

            }
            if (dbContext.db.payment_type.Where(x => x.is_default).FirstOrDefault() != null)
            {
                payment_quick.payment_detail.id_payment_type = dbContext.db.payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;

            }
            else
            {
                toolbar.msgWarning("Please insert paymnent Type");
                return;
            }
        
            payment_quick.payment_detail.App_Name = global::entity.App.Names.PurchaseInvoice;
            payment_quick.contacts = contactViewSource.View.OfType<contact>().ToList();
            payment_quick.mode = cntrl.Curd.payment_quick.modes.purchase; 
            payment_quick.btnSave_Click += Save_Click;
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(payment_quick);
        }

        public void Save_Click(object sender)
        {
            List<payment_schedual> PaymentSchedual = payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList();
            decimal total = PaymentSchedual.Sum(x => x.AccountPayableBalance);
            
            foreach (payment_schedual payment_schedual in PaymentSchedual)
            {
                if (total > 0)
                {
                    payment payment = new payment();
                    payment.id_contact = payment_quick.payment_detail.payment.contact.id_contact;
                    payment.id_payment = payment_quick.payment_detail.payment.id_payment;
                    payment.number = payment_quick.payment_detail.payment.number;
                    payment_detail payment_detail = new payment_detail();
                    payment_detail.id_account = payment_quick.payment_detail.id_account;
                    payment_detail.id_currencyfx = payment_quick.payment_detail.id_currencyfx;
                    payment_detail.id_payment_type = payment_quick.payment_detail.id_payment_type;
                    
                    payment_detail.id_purchase_return = payment_quick.payment_detail.id_purchase_return;
                    payment_detail.id_sales_return = payment_quick.payment_detail.id_sales_return;
                    
                    payment_detail.value = payment_quick.payment_detail.value;
                    payment_detail.comment = payment_quick.payment_detail.comment;
                    payment_schedual _payment_schedual = new payment_schedual();

                    _payment_schedual.debit = Convert.ToDecimal(payment_quick.payment_detail.value);
                    _payment_schedual.parent = payment_schedual;
                    _payment_schedual.expire_date = payment_schedual.expire_date;
                    _payment_schedual.status = payment_schedual.status;
                    _payment_schedual.id_contact = payment_schedual.id_contact;
                    _payment_schedual.id_currencyfx = payment_schedual.id_currencyfx;
                    _payment_schedual.id_purchase_invoice = payment_schedual.id_purchase_invoice;
                    _payment_schedual.id_purchase_order = payment_schedual.id_purchase_order;
                    _payment_schedual.id_purchase_return = payment_schedual.id_purchase_return;
                    _payment_schedual.id_sales_invoice = payment_schedual.id_sales_invoice;
                    _payment_schedual.id_sales_order = payment_schedual.id_sales_order;
                    _payment_schedual.id_sales_return = payment_schedual.id_sales_return;
                    _payment_schedual.trans_date = payment_quick.payment_detail.trans_date;
                    total = total - payment_quick.payment_detail.value;
                    _payment_schedual.AccountPayableBalance = total;

                    payment_detail.payment_schedual.Add(_payment_schedual);
                    payment.payment_detail.Add(payment_detail);

                    //Add Account Logic. With IF FUnction if payment type is Basic Behaviour. If not ignore.

                    if (dbContext.db.payment_type.Where(x => x.id_payment_type == payment_quick.payment_detail.id_payment_type).FirstOrDefault().payment_behavior == payment_type.payment_behaviours.Normal)
                    {
                        app_account_detail app_account_detail = new app_account_detail();
                        app_account_detail.id_account = (int)payment_quick.payment_detail.id_account;
                        app_account_detail.id_currencyfx = payment_schedual.id_currencyfx;
                        app_account_detail.id_payment_type = payment_quick.payment_detail.id_payment_type;
                        app_account_detail.credit = 0;
                        app_account_detail.debit = Convert.ToDecimal(payment_quick.payment_detail.value);
                        dbContext.db.app_account_detail.Add(app_account_detail);
                    }

                    dbContext.db.payments.Add(payment);

                    IEnumerable<DbEntityValidationResult> validationresult = dbContext.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        dbContext.db.SaveChanges();
                        crud_modal.Children.Clear();
                        crud_modal.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    load_Schedual();
                }
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && contactViewSource != null)
            {
                try
                {
                    contactViewSource.View.Filter = i =>
                    {
                        contact contact = i as contact;
                        if (contact != null)
                        {
                            if (contact.name.ToLower().Contains(query.ToLower())
                             || contact.code.ToLower().Contains(query.ToLower())
                             || contact.gov_code.ToLower().Contains(query.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    toolbar.msgError(ex);
                }
            }
            else
            {
                contactViewSource.View.Filter = null;
            }
        }

        private void btnWithholding_Click(object sender, RoutedEventArgs e)
        {
            List<payment_schedual> PaymentSchedualList = payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList();

            if (PaymentSchedualList.Count > 0)
            {
                purchase_invoice  purchase_invoice=PaymentSchedualList.FirstOrDefault().purchase_invoice;
                if (purchase_invoice.payment_withholding_details.Count()==0)
                {
                    VATWithholding.invoiceList = new List<object>();
                    VATWithholding.invoiceList.Add(purchase_invoice);
                    VATWithholding.objEntity = dbContext;
                    VATWithholding.payment_schedual = PaymentSchedualList.FirstOrDefault();
                    VATWithholding.percentage = PaymentSetting.Default.vatwithholdingpercent;
                    crud_modal.Visibility = System.Windows.Visibility.Visible;
                    crud_modal.Children.Add(VATWithholding);   
                    
                }
                else
                {
                    toolbar.msgWarning("Alerady Link With Vat Holding...");
                }
             
            }
            
        }
        private void Refince_Click(object sender, RoutedEventArgs e)
        {
            payment_schedual PaymentSchedual = payment_schedualViewSource.View.CurrentItem as payment_schedual;


            Refinance.objEntity = dbContext;
            Refinance.payment_schedualViewSource = payment_schedualViewSource;
            Refinance.id_contact = PaymentSchedual.id_contact;
            Refinance.id_currency = PaymentSchedual.app_currencyfx.id_currency;
            Refinance.btnSave_Click += SaveRefinance_Click;
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(Refinance);
        }

        public void SaveRefinance_Click(object sender)
        {
            IEnumerable<DbEntityValidationResult> validationresult = dbContext.db.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                dbContext.db.SaveChanges();
                crud_modal.Children.Clear();
                crud_modal.Visibility = System.Windows.Visibility.Collapsed;
            }
            load_Schedual();
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            load_Schedual();
        }

        #region PrefSettings
        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            Commercial.PaymentSetting _pref_PaymentSetting = new Commercial.PaymentSetting();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Commercial.PaymentSetting.Default.Save();
            _pref_PaymentSetting = Commercial.PaymentSetting.Default;
            popupCustomize.IsOpen = false;
        }
        #endregion
    }
}
