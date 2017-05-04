using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd
{
    public partial class Refinance : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public enum Mode
        {
            AccountReceivable,
            AccountPayable
        }

        public Mode WindowsMode
        {
            get
            {
                return _WinMode;
            }
            set
            {
                _WinMode = value;
                if (_WinMode == Mode.AccountPayable)
                {
                    Payable = true;
                    RaisePropertyChanged("Payable");
                    Recievable = false;
                    RaisePropertyChanged("Recievable");
                }
                else
                {
                    Payable = false;
                    RaisePropertyChanged("Payable");
                    Recievable = true;
                    RaisePropertyChanged("Recievable");
                }
            }
        }

        private Mode _WinMode;

        public bool Payable { get { return _Payable; } set { _Payable = value; } }
        private bool _Payable;
        public bool Recievable { get { return _Recievable; } set { _Recievable = value; } }
        private bool _Recievable;
        private List<payment_schedual> _payment_schedualList = null;
        public List<payment_schedual> payment_schedualList { get { return _payment_schedualList; } set { _payment_schedualList = value; } }

        private PaymentDB _entity = null;
        public PaymentDB objEntity { get { return _entity; } set { _entity = value; } }
        public int id_contact { get; set; }
        public int id_currency { get; set; }
        private decimal total = 0;

        public Refinance(Mode Mode)
        {
            WindowsMode = Mode;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    CollectionViewSource app_currencyViewSource = (CollectionViewSource)this.FindResource("app_currencyViewSource");
                    objEntity.app_currency.Where(a => a.is_active == true).Load();
                    app_currencyViewSource.Source = objEntity.app_currency.Local;

                    lbldiff.Content = 0;

                    stackMain.DataContext = payment_schedualList;
                    decimal amount = 0;

                    if (WindowsMode == Mode.AccountPayable)
                    {
                        amount = payment_schedualList.Sum(x => x.credit);
                        total = payment_schedualList.Sum(x => x.credit);
                    }
                    else
                    {
                        amount = payment_schedualList.Sum(x => x.debit);
                        total = payment_schedualList.Sum(x => x.debit);
                    }
                    lblBalance.Content = amount;

                    // payment
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public event btnSave_ClickedEventHandler btnSave_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (btnSave_Click != null && Convert.ToDecimal(lbldiff.Content) == 0)
            {
                btnSave_Click(sender);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objEntity.CancelAllChanges();

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            payment_schedual payment_schedual = dgFinance.SelectedItem as payment_schedual;
            if (payment_schedual.id_payment_schedual == 0)
            {
                payment_schedual Firstpayment_schedual = payment_schedualList.FirstOrDefault() as payment_schedual;

                if (WindowsMode == Mode.AccountPayable)
                {
                    payment_schedual.credit = payment_schedual.credit;
                    payment_schedual.debit = 0;
                    payment_schedual.AccountPayableBalance = payment_schedual.credit;


                    payment_schedual.RaisePropertyChanged("app_currencyfx");
                    //payment_schedual.purchase_invoice = Firstpayment_schedual.purchase_invoice;
                    payment_schedual.id_purchase_invoice = Firstpayment_schedual.purchase_invoice.id_purchase_invoice;
                    payment_schedual.trans_date = Firstpayment_schedual.purchase_invoice.trans_date;
                    payment_schedual.status = entity.Status.Documents_General.Pending;
                    payment_schedual.id_currencyfx = Firstpayment_schedual.id_currencyfx;
                    // payment_schedual.app_currencyfx = Firstpayment_schedual.app_currencyfx;
                    payment_schedual.id_contact = Firstpayment_schedual.purchase_invoice.id_contact;
                    // payment_schedual.contact = Firstpayment_schedual.purchase_invoice.contact;
                }
                else
                {
                    payment_schedual.credit = 0;
                    payment_schedual.debit = payment_schedual.debit;
                    payment_schedual.AccountReceivableBalance = payment_schedual.debit;


                    payment_schedual.RaisePropertyChanged("app_currencyfx");
                    // payment_schedual.sales_invoice = Firstpayment_schedual.sales_invoice;
                    payment_schedual.id_sales_invoice = Firstpayment_schedual.sales_invoice.id_sales_invoice;
                    payment_schedual.trans_date = Firstpayment_schedual.sales_invoice.trans_date;
                    payment_schedual.status = entity.Status.Documents_General.Pending;
                    // payment_schedual.app_currencyfx = Firstpayment_schedual.app_currencyfx;
                    payment_schedual.id_currencyfx = Firstpayment_schedual.id_currencyfx;
                    payment_schedual.id_contact = Firstpayment_schedual.sales_invoice.id_contact;
                    //   payment_schedual.contact = Firstpayment_schedual.sales_invoice.contact;
                }

                payment_schedual.RaisePropertyChanged("contact");
            }

            if (WindowsMode == Mode.AccountPayable)
            {
                lbldiff.Content = Convert.ToDecimal(lblBalance.Content) - payment_schedualList.Sum(x => x.credit);
            }
            else
            {
                lbldiff.Content = Convert.ToDecimal(lblBalance.Content) - payment_schedualList.Sum(x => x.debit);
            }
        }

        private void cbxCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            id_currency = (int)cbxCurrency.SelectedValue;

            //payment_schedualViewSource.View.Filter = i =>
            //{
            //    payment_schedual payment_schedual = i as payment_schedual;
            //    if (payment_schedual.id_contact == id_contact && payment_schedual.app_currencyfx.id_currency == id_currency)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //};
        }
    }
}