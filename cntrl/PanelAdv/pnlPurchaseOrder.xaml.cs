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
                int id_purchase_order = _purchase_order.id_purchase_order;
                System.Windows.Controls.Grid Grid = e.DetailsElement as System.Windows.Controls.Grid;
                var purchaseorder = _purchase_order.purchase_order_detail;

                if (Grid != null)
                {
                    Grid.DataContext = purchaseorder;
                }
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
