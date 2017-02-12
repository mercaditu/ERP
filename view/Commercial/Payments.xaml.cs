
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.ComponentModel;

namespace Cognitivo.Commercial
{
    public partial class Payments : Page, INotifyPropertyChanged
    {
        CollectionViewSource payment_detailMadeViewSource, payment_detailReceive, contactViewSource;
        PaymentDB PaymentDB = new PaymentDB();

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime PaymentDate
        {
            get { return _PaymentDate; }
            set
            {
                _PaymentDate = value;
                RaisePropertyChanged("PaymentDate");

                FilterPaymentsPaid(0);
                FilterPaymentsRecieved(0);
            }
        }
        DateTime _PaymentDate = DateTime.Now;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Payments()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            PaymentDB.contacts.Where(a => a.id_company == CurrentSession.Id_Company && a.is_employee == false).OrderBy(a => a.name).Load();
            contactViewSource.Source = PaymentDB.contacts.Local;

            payment_detailMadeViewSource = FindResource("payment_detailMadeViewSource") as CollectionViewSource;
            payment_detailReceive = FindResource("payment_detailReceive") as CollectionViewSource;
            await PaymentDB.payments.Where(x => x.id_company == CurrentSession.Id_Company).Include(x => x.contact).LoadAsync();
            //Logic to bring Data into view.

            payment_detailReceive.Source = PaymentDB.payments.Local;
            payment_detailMadeViewSource.Source = PaymentDB.payments.Local;

            FilterPaymentsPaid(0);
            FilterPaymentsRecieved(0);
        }

        private void FilterPaymentsPaid(int id_contact)
        {
            try
            {
                List<int?> paymentid = PaymentDB.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company
                          && (
                             x.payment_schedual.Where(y => y.id_purchase_invoice != null).Count() > 0
                          )
                          ).Select(x => x.id_payment).ToList();
                payment_detailMadeViewSource.View.Filter = i =>
                {
                    payment payment = i as payment;
                    if (
                        paymentid.Contains(payment.id_payment)
                       )
                    {
                        if (id_contact > 0)
                        {
                            if (payment.id_contact == id_contact)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
                payment_detailMadeViewSource.View.Refresh();
            }
            catch { }
        
        }

        private void FilterPaymentsRecieved(int id_contact)
        {
            try
            {
                List<int?> paymentid = PaymentDB.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company
                        && (
                           x.payment_schedual.Where(y => y.id_sales_invoice != null).Count() > 0
                        )
                        ).Select(x => x.id_payment).ToList();
                payment_detailReceive.View.Filter = i =>
                {
                    payment payment = i as payment;
                    if ( paymentid.Contains(payment.id_payment)
                        )
                    {
                        if (id_contact > 0)
                        {
                            if (payment.id_contact == id_contact)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
                payment_detailReceive.View.Refresh();
            }
            catch { }
         
        }
        
        private void listContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null)
            {
                FilterPaymentsRecieved(contact.id_contact);
                FilterPaymentsPaid(contact.id_contact);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FilterPaymentsRecieved(0);
            FilterPaymentsPaid(0);
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            payment payment = payment_detailReceive.View.CurrentItem as payment;
            if (payment != null)
            {
                payment.State = EntityState.Modified;
                cntrl.Curd.PaymentEdit PaymentEdit = new cntrl.Curd.PaymentEdit(cntrl.Curd.PaymentEdit.Modes.Recievable, payment, PaymentDB);

                crud_modal.Visibility = Visibility.Visible;
                crud_modal.Children.Add(PaymentEdit);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            payment payment = payment_detailReceive.View.CurrentItem as payment;
            if (payment != null)
            {
                payment.State = EntityState.Unchanged;
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            PaymentDB.Anull();
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == Visibility.Hidden)
            {
                payment_detailReceive.View.Refresh();
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
                    toolBar.msgError(ex);
                }
            }
            else
            {
                contactViewSource.View.Filter = null;
            }
        }
    }
}
