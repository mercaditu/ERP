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
    public partial class pnlPurchaseOrder : UserControl
    {
        CollectionViewSource purchase_orderViewSource;

        private List<purchase_order> _selected_purchase_order = null;
        public List<purchase_order> selected_purchase_order { get { return _selected_purchase_order; } set { _selected_purchase_order = value; } }
  
        public PurchaseInvoiceDB _entity { get; set; }
        public contact _contact { get; set; }
        public pnlPurchaseOrder()
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

                   
                    purchase_orderViewSource = (CollectionViewSource)Resources["purchase_orderViewSource"];
                    purchase_orderViewSource.Source = _entity.purchase_order.Where(x => x.id_contact == _contact.id_contact).ToList();
                }

            }
        }

      
        public event btnSave_ClickedEventHandler PurchaseOrder_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            List<purchase_order> purchase_order = purchase_orderDatagrid.ItemsSource.OfType<purchase_order>().ToList();
            selected_purchase_order = purchase_order.Where(x => x.IsSelected == true).ToList();
            if (PurchaseOrder_Click != null)
            {
                PurchaseOrder_Click(sender);
            }

        }

        private void sales_orderDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            if (_entity.purchase_order_detail.Count() > 0)
            {


                purchase_order _purchase_order = ((System.Windows.Controls.DataGrid)sender).SelectedItem as purchase_order;
                int id_purchaeOrder = _purchase_order.id_purchase_order;
                System.Windows.Controls.DataGrid RowDataGrid = e.DetailsElement as System.Windows.Controls.DataGrid;
                var purchaseOrder = (from purchase_order_detail in _entity.purchase_order_detail
                                     where purchase_order_detail.id_purchase_order == id_purchaeOrder
                                     join purchase_invoice_detail in _entity.purchase_invoice_detail
                                  on purchase_order_detail.id_purchase_order_detail equals purchase_invoice_detail.id_purchase_order_detail into lst
                                     from list in lst.DefaultIfEmpty()
                                     group list by new
                                     {
                                         purchase_order_detail = purchase_order_detail,

                                     }
                                         into grouped
                                         select new
                                         {
                                             id = grouped.Key.purchase_order_detail.id_purchase_order_detail,
                                             quantity = grouped.Key.purchase_order_detail.quantity != null ? grouped.Key.purchase_order_detail.quantity : 0,
                                             balance = grouped.Key.purchase_order_detail.quantity != null ? grouped.Key.purchase_order_detail.quantity : 0 - grouped.Sum(x => x.quantity),
                                             //price = grouped.Key.sales_order_detail.unit_price_vat, 
                                             //subtotal_vat = grouped.Key.sales_order_detail.sub_Total_vat
                                         }).ToList();
                RowDataGrid.ItemsSource = purchaseOrder;
            }
        }

     
        private void ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = _entity.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault(); 
                   
                purchase_orderViewSource = (CollectionViewSource)Resources["purchase_orderViewSource"];
                purchase_orderViewSource.Source = _entity.purchase_order.Where(x => x.id_contact == contact.id_contact).ToList();
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
