
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;
using System.ComponentModel;

namespace Cognitivo.Commercial
{
    public partial class Payments : Page, INotifyPropertyChanged
    {
        CollectionViewSource payment_detailMadeViewSource, payment_detailReceive, contactViewSource;
        PaymentDB PaymentDB = new entity.PaymentDB();

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime PaymentDate
        {
            get { return _PaymentDate; }
            set
            {
                _PaymentDate = value;
                RaisePropertyChanged("PaymentDate");

                //slider.Maximum = DateTime.DaysInMonth(PaymentDate.Year, _PaymentDate.Month);
                //slider.Value = PaymentDate.Day;

                FilterPaymentsPaid(0);
                FilterPaymentsRecieved(0);
            }
        }
        DateTime _PaymentDate = DateTime.Now;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Payments()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
            PaymentDB.contacts.Where(a => a.id_company == CurrentSession.Id_Company && a.is_employee == false).OrderBy(a => a.name).Load();
            contactViewSource.Source = PaymentDB.contacts.Local;

            payment_detailMadeViewSource = (CollectionViewSource)FindResource("payment_detailMadeViewSource");
            payment_detailReceive = (CollectionViewSource)FindResource("payment_detailReceive");
            PaymentDB.payments.Load();
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
                List<int> paymentid = PaymentDB.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company
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
                List<int> paymentid = PaymentDB.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company
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

        //private void EditCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    if (e.Parameter as payment_detail != null)
        //    {
        //        e.CanExecute = true;
        //    }
        //}

        //private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    payment_detail payment_detail = e.Parameter as payment_detail;

        //    cntrl.Curd.PaymentEdit PaymentEdit = new cntrl.Curd.PaymentEdit(cntrl.Curd.PaymentEdit.Modes.Recievable, payment_detail.payment, PaymentDB);

        //    crud_modal.Visibility = System.Windows.Visibility.Visible;
        //    crud_modal.Children.Add(PaymentEdit);
        //}

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
            payment.State = EntityState.Modified;
            cntrl.Curd.PaymentEdit PaymentEdit = new cntrl.Curd.PaymentEdit(cntrl.Curd.PaymentEdit.Modes.Recievable, payment, PaymentDB);

            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(PaymentEdit);
        }

        private void btnSave_Click(object sender)
        {
            payment payment = payment_detailReceive.View.CurrentItem as payment;
            payment.State = EntityState.Unchanged;
            PaymentDB.SaveChanges();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            payment payment = payment_detailReceive.View.CurrentItem as payment;
            payment.State = EntityState.Unchanged;

        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == System.Windows.Visibility.Hidden)
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

        //private void FFMonth_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    PaymentDate = PaymentDate.AddMonths(1);
        //}

        //private void FFDay_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    PaymentDate = PaymentDate.AddDays(1);
        //}

        //private void RRDay_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    PaymentDate = PaymentDate.AddDays(-1);
        //}

        //private void RRMonth_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    PaymentDate = PaymentDate.AddMonths(-1);
        //}

        //private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    PaymentDate = PaymentDate.AddDays(slider.Value - PaymentDate.Day);
        //}

        //private void Today_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    PaymentDate = DateTime.Now;
        //}

    }
}
