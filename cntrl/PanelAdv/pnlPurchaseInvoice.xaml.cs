
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System;

namespace cntrl.PanelAdv
{
    public partial class pnlPurchaseInvoice : UserControl
    {
        CollectionViewSource purchase_invoiceViewSource;

        private List<purchase_invoice> _selected_purchase_invoice = null;
        public List<purchase_invoice> selected_purchase_invoice { get { return _selected_purchase_invoice; } set { _selected_purchase_invoice = value; } }
  
        public ImpexDB _entity { get; set; }
        public contact _contact { get; set; }
        public pnlPurchaseInvoice()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
              
              
                if (_contact != null)
                {


                    purchase_invoiceViewSource = (CollectionViewSource)Resources["purchase_invoiceViewSource"];
                    purchase_invoiceViewSource.Source = _entity.purchase_invoice.Where(x => x.id_contact == _contact.id_contact).ToList();
                }

            }
        }

      
        public event btnSave_ClickedEventHandler PurchaseInvoice_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            List<purchase_invoice> purchase_invoice = purchase_invoiceDatagrid.ItemsSource.OfType<purchase_invoice>().ToList();
            selected_purchase_invoice = purchase_invoice.Where(x => x.IsSelected == true).ToList();
            if (PurchaseInvoice_Click != null)
            {
                PurchaseInvoice_Click(sender);
            }

        }

        private void sales_orderDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            if (_entity.purchase_invoice_detail.Count() > 0)
            {


                purchase_invoice _purchase_invoice = ((System.Windows.Controls.DataGrid)sender).SelectedItem as purchase_invoice;
                int id_purchase_invoice = _purchase_invoice.id_purchase_invoice;
                System.Windows.Controls.DataGrid RowDataGrid = e.DetailsElement as System.Windows.Controls.DataGrid;
                var purchaseInvoice = _purchase_invoice.purchase_invoice_detail;
                RowDataGrid.ItemsSource = purchaseInvoice;
            }
        }

     
        private void ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = _entity.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                purchase_invoiceViewSource = (CollectionViewSource)Resources["purchase_invoiceViewSource"];
                purchase_invoiceViewSource.Source = _entity.purchase_invoice.Where(x => x.id_contact == _contact.id_contact).ToList();
            }
        }
       

        //private void purchase_orderDatagrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (purchase_orderViewSource != null)
        //    {
        //        List<purchase_order> purchase_order = purchase_orderViewSource.View.Cast<purchase_order>().ToList();
        //        foreach (purchase_order order in purchase_order)
        //        {
        //            if (order != null)
        //            {
        //                order.get_Puchase_Total();
        //            }
        //        }
        //    }
        //}

    }
}
