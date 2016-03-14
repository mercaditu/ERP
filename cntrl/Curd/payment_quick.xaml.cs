using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace cntrl.Curd
{
    public partial class payment_quick : UserControl
    {
        dbContext dbContext = new dbContext();
        public enum modes
        {
            sales,
            purchase
        }
        public payment_quick()
        {
            InitializeComponent();
        }

        public List<entity.contact> contacts { get; set; }
        public payment_detail payment_detail { get; set; }
        public modes mode { get; set; }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _Settings = new entity.Properties.Settings();

            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            dbContext.db.payment_type.Where(a => a.is_active == true).Load();
            payment_typeViewSource.Source = dbContext.db.payment_type.Local;

            CollectionViewSource contactViewSource = (CollectionViewSource)this.FindResource("contactViewSource");
            contactViewSource.Source = contacts;

            CollectionViewSource app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            dbContext.db.app_account.Where(a => a.is_active == true && a.id_company == _Settings.company_ID).Load();
            app_accountViewSource.Source = dbContext.db.app_account.Local;

            CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
            List<payment_detail> payment_detailList = new List<payment_detail>();
            payment_detailList.Add(payment_detail);
            paymentpayment_detailViewSource.Source = payment_detailList;

            CollectionViewSource purchase_returnViewSource = (CollectionViewSource)this.FindResource("purchase_returnViewSource");
            dbContext.db.purchase_return.Where(x=>x.id_contact==payment_detail.payment.id_contact).Load();
            purchase_returnViewSource.Source = dbContext.db.purchase_return.Local;
            CollectionViewSource sales_returnViewSource = (CollectionViewSource)this.FindResource("sales_returnViewSource");
            dbContext.db.sales_return.Where(x => x.id_contact == payment_detail.payment.id_contact && x.sales_invoice==null).Load();
            sales_returnViewSource.Source = dbContext.db.sales_return.Local;

            cbxDocument.ItemsSource = dbContext.db.app_document_range.Where(d => d.is_active == true
                                           && d.app_document.id_application == entity.App.Names.PaymentUtility
                                           && d.id_company == _Settings.company_ID).Include(i => i.app_document).ToList(); 

            // entity.db.payment_detail.Add(payment_detail);
            paymentpayment_detailViewSource.View.Refresh();
            paymentpayment_detailViewSource.View.MoveCurrentToLast();

        }



        public event btnSave_ClickedEventHandler btnSave_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (btnSave_Click != null)
            {
                btnSave_Click(sender);
            }
        }

        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void cbxPamentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxPamentType.SelectedItem != null)
            {
                entity.payment_type payment_type = (entity.payment_type)cbxPamentType.SelectedItem;
                if (payment_type.payment_behavior == global::entity.payment_type.payment_behaviours.Normal)
                {
                    stpaccount.Visibility = Visibility.Visible;
                    stpcreditpurchase.Visibility = Visibility.Collapsed;
                    stpcreditsales.Visibility = Visibility.Collapsed;

                }
                else if(payment_type.payment_behavior == global::entity.payment_type.payment_behaviours.CreditNote)
                {
                    stpaccount.Visibility = Visibility.Collapsed;
                    if (mode == modes.purchase)
                    {
                        stpcreditpurchase.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stpcreditsales.Visibility = Visibility.Visible;
                    }
                    stpcreditpurchase.Visibility = Visibility.Visible;
                   
                }
                else
                {
                    stpaccount.Visibility = Visibility.Collapsed;
                    stpcreditpurchase.Visibility = Visibility.Collapsed;
                    stpcreditsales.Visibility = Visibility.Collapsed;
                }

                if (payment_type.id_document > 0)
                {
                    stpDetailDocument.Visibility = Visibility.Visible;
                    payment_detail.id_range = dbContext.db.app_document_range.Where(d => d.id_document == payment_type.id_document && d.is_active == true).Include(i => i.app_document).FirstOrDefault().id_range;
                }
                else
                {
                    stpDetailDocument.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void purchasereturnComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                try
                {
                    if (purchasereturnComboBox.Data != null)
                    {
                        purchase_return purchase_return = (purchase_return)purchasereturnComboBox.Data;
                        purchasereturnComboBox.Text = purchase_return.number;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // toolBar.msgError(ex);
                }
            }
        }
        
        private void purchasereturnComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (purchasereturnComboBox.Data != null)
                {
                    purchase_return purchase_return = (purchase_return)purchasereturnComboBox.Data;
                    purchasereturnComboBox.Text = purchase_return.number;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                // toolBar.msgError(ex);
            }
        }

        private void salesreturnComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (salesreturnComboBox.Data != null)
                    {
                        CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
                        payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
                        sales_return sales_return = (sales_return)salesreturnComboBox.Data;
                        salesreturnComboBox.Text = sales_return.number;
                        payment_detail.value = (Convert.ToDecimal(txtAmount.Text) - sales_return.GrandTotal);
                        payment_detail.RaisePropertyChanged("value");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // toolBar.msgError(ex);
                }
            }
        }

        private void salesreturnComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
         
                if (salesreturnComboBox.Data != null)
                {
                    CollectionViewSource paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");
                    payment_detail payment_detail = paymentpayment_detailViewSource.View.CurrentItem as payment_detail;
                    sales_return sales_return = (sales_return)salesreturnComboBox.Data;
                    salesreturnComboBox.Text = sales_return.number;
                    payment_detail.value = (payment_detail.value - sales_return.GrandTotal);
                    payment_detail.RaisePropertyChanged("value");
                }
            }
            catch (Exception ex)
            {
                throw ex;
                // toolBar.msgError(ex);
            }
        }
    }
}
