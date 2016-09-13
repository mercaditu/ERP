using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;
using System.ComponentModel;

namespace Cognitivo.Commercial
{
    public partial class EmpRecievable : Page, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        #endregion

        CollectionViewSource payment_schedualViewSource, contactViewSource;
        PaymentDB PaymentDB = new entity.PaymentDB();



        public EmpRecievable()
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
            //List<contact> contactLIST = new List<contact>();

            //foreach (payment_schedual payment in PaymentDB.payment_schedual.Local.Where(x=>x.contact.is_employee).ToList())
            //{
            //    if (contactLIST.Contains(payment.contact) == false)
            //    {
            //        contact contact = new contact();
            //        contact = payment.contact;
            //        contactLIST.Add(contact);
            //    }
            //}

            contactViewSource.Source = PaymentDB.contacts.Where(x => x.is_employee).ToList();
        }

        private  void load_Schedual()
        {
            payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");
           PaymentDB.payment_schedual
                    .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company && x.contact.is_employee
                        && (x.id_sales_invoice > 0 || x.id_sales_order > 0) && x.id_note==null
                        && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                        .OrderBy(x => x.expire_date)
                        .Load();
           payment_schedualViewSource.Source = PaymentDB.payment_schedual.Local;
                
        }

        private void Payment_Click(object sender, RoutedEventArgs e)
        {
            List<payment_schedual> PaymentSchedualList = new List<payment_schedual>();

            if (payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList().Count > 0)
            {
                PaymentSchedualList = payment_schedualViewSource.View.OfType<payment_schedual>().Where(x => x.IsSelected == true).ToList();
            }
            else if (payment_schedualViewSource.View.OfType<payment_schedual>().ToList().Count > 0)
            {
                PaymentSchedualList = payment_schedualViewSource.View.OfType<payment_schedual>().ToList();
            }
            else
            {
                //If nothing found, then exit.
                return;
            }

            cntrl.Curd.Payment Payment = new cntrl.Curd.Payment(cntrl.Curd.Payment.Modes.Recievable, PaymentSchedualList);

            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(Payment);
        }

        public void Save_Click(object sender)
        {

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
            cntrl.Curd.Refinance Refinance = new cntrl.Curd.Refinance(cntrl.Curd.Refinance.Mode.AccountReceivable);

            Refinance.objEntity = PaymentDB;
            Refinance.payment_schedualViewSource = payment_schedualViewSource;
            Refinance.id_contact = PaymentSchedual.id_contact;
            Refinance.id_currency = PaymentSchedual.app_currencyfx.id_currency;
            Refinance.btnSave_Click += SaveRefinance_Click;
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(Refinance);
        }

        public void SaveRefinance_Click(object sender)
        {
            IEnumerable<DbEntityValidationResult> validationresult = PaymentDB.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                PaymentDB.SaveChanges();
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

                if (sales_invoice.payment_withholding_detail.Count() == 0)
                {
                    cntrl.VATWithholding VATWithholding = new cntrl.VATWithholding();

                    VATWithholding.invoiceList = new List<object>();
                    VATWithholding.invoiceList.Add(sales_invoice);
                    VATWithholding.PaymentDB = PaymentDB;
                    VATWithholding.payment_schedual = PaymentSchedualList.FirstOrDefault();
                    VATWithholding.percentage = sales_invoice.vatwithholdingpercentage;
                    crud_modal.Visibility = System.Windows.Visibility.Visible;
                    crud_modal.Children.Add(VATWithholding);
                }
                else
                {
                    toolbar.msgWarning("Linked With Vat Holding...");
                }

            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PaymentDB = new entity.PaymentDB();
            load_Schedual();
            ListBox_SelectionChanged(sender, null);
        }

        private void Rearrange_Click(object sender, RoutedEventArgs e)
        {
            PaymentDB.Rearrange_Payment();
        }

        
    }
}

