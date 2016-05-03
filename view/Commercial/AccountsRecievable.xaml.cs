using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;

namespace Cognitivo.Commercial
{
    public partial class AccountsRecievable : Page
    {
        dbContext _entity = new dbContext();
        //entity.Properties.Settings _Settings = new entity.Properties.Settings();
        //CollectionViewSource paymentViewSource;
        CollectionViewSource payment_schedualViewSource, contactViewSource;
        PaymentDB PaymentDB = new entity.PaymentDB();
        //List<contact> ContactList;
        cntrl.Curd.payment_quick payment_quick = new cntrl.Curd.payment_quick();
        cntrl.Curd.Refinance Refinance = new cntrl.Curd.Refinance();
        cntrl.VATWithholding VATWithholding = new cntrl.VATWithholding();

        public AccountsRecievable()
        {
            InitializeComponent();
        }

        private void toolBar_btnApprove_Click(object sender)
        {

        }

        private void toolBar_btnAnull_Click(object sender)
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

            foreach (payment_schedual payment in _entity.db.payment_schedual.Local.ToList())
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
            payment_schedualViewSource.Source = await _entity.db.payment_schedual
                    .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
                        && (x.id_sales_invoice > 0 || x.id_sales_order > 0)
                        && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                    .ToListAsync();
        }

        private void Payment_Click(object sender, RoutedEventArgs e)
        {
         
            List<payment_schedual> PaymentSchedualList = payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList();
            decimal total = PaymentSchedualList.Sum(x => x.AccountReceivableBalance);

            payment_quick.payment_detail = new payment_detail();
            payment_quick.payment_detail.id_purchase_return = 0;
            payment_quick.payment_detail.id_sales_return = 0;
            payment_quick.payment_detail.value = total;
            payment_quick.payment_detail.payment = PaymentDB.New();

            payment_quick.payment_detail.payment.IsSelected = true;
            payment_quick.payment_detail.payment.status = Status.Documents_General.Pending;
            if (PaymentSchedualList.Count == 1)
            {
                payment_quick.payment_detail.payment.id_contact = PaymentSchedualList.FirstOrDefault().id_contact;
                payment_quick.payment_detail.payment.contact = PaymentDB.contacts.Where(x => x.id_contact == payment_quick.payment_detail.payment.id_contact).FirstOrDefault();
                payment_quick.payment_detail.id_currencyfx = PaymentSchedualList.FirstOrDefault().id_currencyfx;
                if (PaymentDB.payment_type.Where(x => x.is_default).FirstOrDefault() != null)
                {
                    payment_quick.payment_detail.id_payment_type = PaymentDB.payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
                }
                else
                {
                    toolbar.msgWarning("Please insert paymnent Type");
                    return;
                }

            }
            payment_quick.payment_detail.App_Name = global::entity.App.Names.SalesInvoice;

            payment_quick.contacts = contactViewSource.View.OfType<contact>().ToList();
            payment_quick.mode = cntrl.Curd.payment_quick.modes.sales;
            payment_quick.btnSave_Click += Save_Click;
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(payment_quick);
        }

        public void Save_Click(object sender)
        {
            entity.Brillo.Logic.AccountReceivable AccountReceivable = new entity.Brillo.Logic.AccountReceivable();
            List<payment_schedual> PaymentSchedual = payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList();
            decimal total = PaymentSchedual.Sum(x => x.AccountReceivableBalance);
            foreach (payment_schedual payment_schedual in PaymentSchedual)
            {
                if (total > 0)
                {


                    payment_quick.payment_detail.payment.payment_detail.Add(payment_quick.payment_detail);
                    PaymentDB.payments.Add(payment_quick.payment_detail.payment);
                    PaymentDB.Approve(payment_schedual.id_payment_schedual,true);
                    //AccountReceivable.ReceivePayment(ref _entity, payment_schedual, (int)payment_quick.payment_detail.payment.id_range, payment_quick.payment_detail.id_currencyfx, payment_quick.payment_detail.id_payment_type,
                    //                                (int)payment_quick.payment_detail.id_purchase_return, (int)payment_quick.payment_detail.id_sales_return, payment_quick.payment_detail.value, payment_quick.payment_detail.comment, (int)payment_quick.payment_detail.id_account, payment_quick.payment_detail.trans_date);

                    total = total - payment_quick.payment_detail.value;
                  
                    
                }
            }
            crud_modal.Children.Clear();
            crud_modal.Visibility = System.Windows.Visibility.Collapsed;
            _entity = new dbContext();
            load_Schedual();
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
                                string name = "";
                                string code = "";
                                string gov_code = "";

                                if (contact.name != null)
                                {
                                    name = contact.name.ToLower();
                                }

                                if (contact.code != null)
                                {
                                    code = contact.code.ToLower();
                                }

                                if (contact.gov_code != null)
                                {
                                    gov_code = contact.gov_code.ToLower();
                                }

                                if (name.Contains(query.ToLower())
                                    || code.Contains(query.ToLower())
                                    || gov_code.Contains(query.ToLower()))
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

        private void Refince_Click(object sender, RoutedEventArgs e)
        {
            payment_schedual PaymentSchedual = payment_schedualViewSource.View.CurrentItem as payment_schedual;

            Refinance.WindowsMode = cntrl.Curd.Refinance.Mode.AccountReceivable;

            Refinance.objEntity = _entity;
            Refinance.payment_schedualViewSource = payment_schedualViewSource;
            Refinance.id_contact = PaymentSchedual.id_contact;
            Refinance.id_currency = PaymentSchedual.app_currencyfx.id_currency;
            Refinance.btnSave_Click += SaveRefinance_Click;
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(Refinance);
        }

        public void SaveRefinance_Click(object sender)
        {
            IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                _entity.SaveChanges();
                crud_modal.Children.Clear();
                crud_modal.Visibility = System.Windows.Visibility.Collapsed;
            }
            load_Schedual();
        }

        private void btnWithholding_Click(object sender, RoutedEventArgs e)
        {
            List<payment_schedual> PaymentSchedualList = payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList();

            if (PaymentSchedualList.Count > 0)
            {
                sales_invoice sales_invoice = PaymentSchedualList.FirstOrDefault().sales_invoice;
                if (sales_invoice.payment_withholding_details.Count() == 0)
                {
                    VATWithholding.invoiceList = new List<object>();
                    VATWithholding.invoiceList.Add(sales_invoice);
                    VATWithholding.objEntity = _entity;
                    VATWithholding.payment_schedual = PaymentSchedualList.FirstOrDefault();
                    VATWithholding.percentage = sales_invoice.vatwithholdingpercentage;
                    crud_modal.Visibility = System.Windows.Visibility.Visible;
                    crud_modal.Children.Add(VATWithholding);

                }
                else
                {
                    toolbar.msgWarning("Alerady Link With Vat Holding...");
                }

            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            load_Schedual();
        }
    }
}

