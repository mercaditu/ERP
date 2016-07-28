
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

            //Logic to bring Data into view.
            PaymentDB.payment_detail.Where(x => x.id_company == CurrentSession.Id_Company
                            && (
                               x.payment_schedual.Where(y => y.id_purchase_invoice != null).Count() > 0
                               ||
                               x.payment_schedual.Where(y => y.id_sales_invoice != null).Count() > 0
                               )
                            ).LoadAsync();

            payment_detailReceive.Source = PaymentDB.payment_detail.Local;
            payment_detailMadeViewSource.Source = PaymentDB.payment_detail.Local;
            
            FilterPaymentsPaid(0);
            FilterPaymentsRecieved(0);
         
        }

        private void FilterPaymentsPaid(int id_contact)
        {
            try
            {
                payment_detailMadeViewSource.View.Filter = i =>
                {
                    payment_detail payment_detail = i as payment_detail;
                    if (
                        payment_detail.payment_schedual.Where(y => y.id_purchase_invoice != null).Count() > 0
                        &&
                        payment_detail.payment.trans_date < _PaymentDate)
                    {
                        if (id_contact > 0)
                        {
                            if (payment_detail.payment.id_contact == id_contact)
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
            }
            catch { }
        }

        private void FilterPaymentsRecieved(int id_contact)
        {
            try
            {
                payment_detailReceive.View.Filter = i =>
                {
                    payment_detail payment_detail = i as payment_detail;
                    if (payment_detail.payment_schedual.Where(y => y.id_sales_invoice != null).Count() > 0)
                    {
                        if (id_contact > 0)
                        {
                            if (payment_detail.payment.id_contact == id_contact)
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
            }
            catch { }
        }

        private void EditCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as payment_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            payment_detail payment_detail = e.Parameter as payment_detail;

            cntrl.Curd.payment_display payment_display = new cntrl.Curd.payment_display(cntrl.Curd.payment_display.Modes.Recievable, payment_detail.payment.id_contact, payment_detail.payment.id_payment, payment_detail.id_payment_detail);

            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(payment_display);
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

        }

        private void btnSave_Click(object sender)
        {

        }

        private void toolBar_btnCancel_Click(object sender)
        {

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
