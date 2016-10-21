using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System;
using System.Threading.Tasks;

namespace cntrl.PanelAdv
{
    public partial class pnlPurchaseOrder : UserControl
    {
        CollectionViewSource purchase_orderViewSource;
        public enum module
        {
            sales_invoice,
            packing_list
        }
        public module mode { get; set; }
        private List<purchase_order> _selected_purchase_order = null;
        public List<purchase_order> selected_purchase_order { get { return _selected_purchase_order; } set { _selected_purchase_order = value; } }
  
        public db _entity { get; set; }
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
                    sbxContact.Text = _contact.name;
                    load_PurchaseOrder(_contact.id_contact);
                }
            }
        }

      
        public event btnSave_ClickedEventHandler PurchaseOrder_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (purchase_orderDatagrid.ItemsSource != null)
            {
                List<purchase_order> purchase_order = purchase_orderDatagrid.ItemsSource.OfType<purchase_order>().ToList();
                selected_purchase_order = purchase_order.Where(x => x.IsSelected == true).ToList();
                if (mode == module.sales_invoice)
                {
                    if (selected_purchase_order.Count() > 1)
                    {
                        MessageBox.Show("only one order will be linked..");
                        return;
                    }
                }


                if (PurchaseOrder_Click != null)
                {
                    PurchaseOrder_Click(sender);
                }
            }
        }

        private void sales_orderDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            if (_entity.purchase_order_detail.Count() > 0)
            {
                purchase_order _purchase_order = ((System.Windows.Controls.DataGrid)sender).SelectedItem as purchase_order;
                Task task_PrimaryData = Task.Factory.StartNew(() => LoadOrderDetail(_purchase_order, e));
            }
        }
        private void LoadOrderDetail(purchase_order purchase_order, DataGridRowDetailsEventArgs e)
        {
            if (_entity.purchase_order_detail.Count() > 0)
            {


                Dispatcher.Invoke(new Action(() =>
                {
                    
                    System.Windows.Controls.DataGrid RowDataGrid = e.DetailsElement as System.Windows.Controls.DataGrid;
                    if (RowDataGrid!=null)
                    {
                        RowDataGrid.ItemsSource = purchase_order.purchase_order_detail;
                    }
                   
                }));
            }
        }
     
        private void ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = _entity.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                load_PurchaseOrder(contact.id_contact);
                
            }
        }

        private void load_PurchaseOrder(int id_contact)
        {
            var order = (from purchase_order_detail in _entity.purchase_order_detail
                         where purchase_order_detail.purchase_order.id_contact == id_contact
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
                                 id_purchase_order = grouped.Key.purchase_order_detail.purchase_order.id_purchase_order,
                                 purchaseOrder = grouped.Key.purchase_order_detail.purchase_order,
                                 balance = grouped.Key.purchase_order_detail != null ? grouped.Key.purchase_order_detail.quantity : 0 - grouped.Sum(x => x.quantity)
                             }).ToList().Select(x => x.id_purchase_order);

            purchase_orderViewSource = (CollectionViewSource)Resources["purchase_orderViewSource"];

            purchase_orderViewSource.Source = _entity.purchase_order.Where(x => order.Contains(x.id_purchase_order)).ToList();
            filter_sales();
        }
        void filter_sales()
        {
            if (purchase_orderViewSource != null)
            {
                if (purchase_orderViewSource.View != null)
                {
                    if (purchase_orderViewSource.View.OfType<purchase_order>().Count() > 0)
                    {
                        purchase_orderViewSource.View.Filter = i =>
                        {
                            purchase_order purchase_order = (purchase_order)i;
                            if (purchase_order.purchase_order_detail.Sum(x => x.balance) > 0)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }

    }
}
