using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl
{
    public partial class VATWithholding : UserControl
    {
        private List<object> _invoiceList = null;
        public List<object> invoiceList { get { return _invoiceList; } set { _invoiceList = value; } }

        public PaymentDB PaymentDB { get; set; }

        //Change to List. We will need to add multiple payment scheduals.
        public payment_schedual payment_schedual { get; set; }

        public decimal percentage { get; set; }

        public enum Mode
        {
            Add,
            Edit
        }

        public VATWithholding()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PaymentDB, entity.App.Names.PaymentWithHolding, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                    CollectionViewSource invoiceViewSource = new CollectionViewSource();
                    invoiceViewSource.Source = _invoiceList;
                    stackMain.DataContext = invoiceViewSource;
                    DtpTransdate.SelectedDate = DateTime.Now;

                    if (_invoiceList.FirstOrDefault().GetType().BaseType == typeof(sales_invoice))
                    {
                        sales_invoice sales_invoice = (sales_invoice)_invoiceList.FirstOrDefault();
                        if (sales_invoice.GrandTotal > 0)
                        {
                            lbltotalvat.Content = Math.Round((((sales_invoice.TotalVat * payment_schedual.AccountReceivableBalance) / sales_invoice.GrandTotal)) * percentage, 4);
                        }
                    }
                    else if (_invoiceList.FirstOrDefault().GetType().BaseType == typeof(purchase_invoice))
                    {
                        purchase_invoice purchase_invoice = (purchase_invoice)_invoiceList.FirstOrDefault();
                        if (purchase_invoice.GrandTotal > 0)
                        {
                            lbltotalvat.Content = Math.Round(((purchase_invoice.TotalVat * payment_schedual.AccountPayableBalance) / purchase_invoice.GrandTotal) * percentage, 4);
                        }
                    }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
                payment_withholding_tax payment_withholding_tax = new payment_withholding_tax();
                payment_withholding_tax.status = Status.Documents_General.Pending;
                payment_withholding_tax.id_contact = ((dynamic)_invoiceList.FirstOrDefault()).id_contact;

            if (cbxDocument.SelectedValue != null)
                {
                    payment_withholding_tax.id_range = (int)cbxDocument.SelectedValue;
                }

                payment_withholding_tax.id_currencyfx = ((dynamic)_invoiceList.FirstOrDefault()).id_currencyfx;
                payment_withholding_tax.withholding_number = txtnumber.Text;
                payment_withholding_tax.value = (decimal)lbltotalvat.Content;
                payment_withholding_tax.trans_date = (DateTime)DtpTransdate.SelectedDate;
                payment_withholding_tax.expire_date = (DateTime)DtpTransdate.SelectedDate;

                payment_withholding_detail payment_withholding_details = new payment_withholding_detail();
                if (_invoiceList.FirstOrDefault().GetType() == typeof(sales_invoice) || _invoiceList.FirstOrDefault().GetType().BaseType == typeof(sales_invoice))
                {
                    sales_invoice sales_invoice = (sales_invoice)_invoiceList.FirstOrDefault();
                    payment_withholding_details.id_sales_invoice = sales_invoice.id_sales_invoice;
                }
                else if (_invoiceList.FirstOrDefault().GetType() == typeof(purchase_invoice) || _invoiceList.FirstOrDefault().GetType().BaseType == typeof(purchase_invoice))
                {
                    purchase_invoice purchase_invoice = (purchase_invoice)_invoiceList.FirstOrDefault();
                    payment_withholding_details.id_purchase_invoice = purchase_invoice.id_purchase_invoice;
                }

                payment_withholding_tax.payment_withholding_detail.Add(payment_withholding_details);

                PaymentDB.payment_withholding_tax.Add(payment_withholding_tax);
                payment_schedual _payment_schedual = new payment_schedual();

                if (_invoiceList.FirstOrDefault().GetType().BaseType == typeof(sales_invoice))
                {
                    _payment_schedual.credit = Convert.ToDecimal(lbltotalvat.Content);
                }
                else if (_invoiceList.FirstOrDefault().GetType().BaseType == typeof(purchase_invoice))
                {
                    _payment_schedual.debit = Convert.ToDecimal(lbltotalvat.Content);
                }

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
                _payment_schedual.trans_date = (DateTime)DtpTransdate.SelectedDate;

                PaymentDB.payment_schedual.Add(_payment_schedual);

                PaymentDB.SaveChanges();

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
                PaymentDB.CancelAllChanges();

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_invoiceList.FirstOrDefault().GetType().BaseType == typeof(sales_invoice))
            {
                sales_invoice sales_invoice = (sales_invoice)_invoiceList.FirstOrDefault();
                if (sales_invoice.GrandTotal > 0)
                {
                    if (sales_invoice.vatwithholdingpercentage > 0)
                    {
                        percentage = sales_invoice.vatwithholdingpercentage;
                        if (percentage <= 1)
                        {
                            if (sales_invoice.GrandTotal > 0)
                            {
                                lbltotalvat.Content = Math.Round((((sales_invoice.TotalVat * payment_schedual.AccountReceivableBalance) / sales_invoice.GrandTotal)) * percentage, 4);
                            }
                        }
                        else
                        {
                            MessageBox.Show("not Exceed to Hundred %");
                            lbltotalvat.Content = Math.Round((((sales_invoice.TotalVat * payment_schedual.AccountReceivableBalance) / sales_invoice.GrandTotal)), 4);
                        }
                    }
                    else
                    {
                        lbltotalvat.Content = Math.Round((((sales_invoice.TotalVat * payment_schedual.AccountReceivableBalance) / sales_invoice.GrandTotal)), 4);
                    }
                }
            }
            else if (_invoiceList.FirstOrDefault().GetType().BaseType == typeof(purchase_invoice))
            {
                purchase_invoice purchase_invoice = (purchase_invoice)_invoiceList.FirstOrDefault();
                if (purchase_invoice.GrandTotal > 0)
                {
                    lbltotalvat.Content = Math.Round(((purchase_invoice.TotalVat * payment_schedual.AccountPayableBalance) / purchase_invoice.GrandTotal) * percentage, 4);
                }
            }
        }
    }
}