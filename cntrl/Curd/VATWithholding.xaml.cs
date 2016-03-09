using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class VATWithholding : UserControl
    {
        List<object> _invoiceList = null;
        public List<object> invoiceList { get { return _invoiceList; } set { _invoiceList = value; } }

        private dbContext _entity = null;
        public dbContext objEntity { get { return _entity; } set { _entity = value; } }

        //Change to List. We will need to add multiple payment scheduals.
        public payment_schedual payment_schedual { get; set; }
        
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
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.PaymentWithHolding, CurrentSession.Id_Branch, CurrentSession.Id_terminal);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    CollectionViewSource invoiceViewSource = new CollectionViewSource();
                    invoiceViewSource.Source = _invoiceList;
                    stackMain.DataContext = invoiceViewSource;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                payment_withholding_tax payment_withholding_tax = new payment_withholding_tax();
                payment_withholding_tax.status = Status.Documents_General.Pending;
                payment_withholding_tax.id_contact = ((dynamic)_invoiceList.FirstOrDefault()).id_contact;
                if (cbxDocument.SelectedValue!=null)
                {
                    payment_withholding_tax.id_range = (int)cbxDocument.SelectedValue;
                }
               
                payment_withholding_tax.id_currencyfx = ((dynamic)_invoiceList.FirstOrDefault()).id_currencyfx;
                payment_withholding_tax.withholding_number = txtnumber.Text;
                payment_withholding_tax.value = (decimal)lbltotalvat.Content;
                payment_withholding_tax.trans_date =(DateTime) DtpTransdate.SelectedDate;
                payment_withholding_tax.expire_date = (DateTime)DtpTransdate.SelectedDate;

                foreach (dynamic item in _invoiceList)
                {
                    payment_withholding_details payment_withholding_details = new payment_withholding_details();
                    payment_withholding_details.id_purchase_invoice = item.id_purchase_invoice;
                    payment_withholding_tax.payment_withholding_details.Add(payment_withholding_details);
                }
                objEntity.db.payment_withholding_tax.Add(payment_withholding_tax);
                payment_schedual _payment_schedual = new payment_schedual();

                _payment_schedual.debit = Convert.ToDecimal(lbltotalvat.Content);
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

                objEntity.db.payment_schedual.Add(_payment_schedual);

                IEnumerable<DbEntityValidationResult> validationresult = objEntity.db.GetValidationErrors();

                if (validationresult.Count() == 0)
                {
                    objEntity.SaveChanges();
                    entity.Properties.Settings.Default.company_ID = objEntity.db.app_company.FirstOrDefault().id_company;
                    entity.Properties.Settings.Default.company_Name = objEntity.db.app_company.FirstOrDefault().alias;
                    entity.Properties.Settings.Default.Save();
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objEntity.CancelChanges();

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
