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
    public partial class pnlSalesOrder : UserControl
    {
        CollectionViewSource sales_orderViewSource;
        public enum module
        {
            sales_invoice,
            packing_list
        }
        public module mode { get; set; }
        private List<sales_order> _selected_sales_order = null;
        public List<sales_order> selected_sales_order { get { return _selected_sales_order; } set { _selected_sales_order = value; } }
       // public CollectionViewSource contactViewSource { get; set; }
        public contact _contact { get; set; }
        public db _entity { get; set; }
        public pnlSalesOrder()
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
               // contactComboBox.CollectionViewSource = contactViewSource;
            }
            if (_contact != null)
            {
                
               
                sbxContact.Text = _contact.name;
                sales_orderViewSource = (CollectionViewSource)Resources["sales_orderViewSource"];
                sales_orderViewSource.Source = _entity.sales_order.Where(x => x.id_contact == _contact.id_contact).ToList();
            }
        }



        private void ContactPref(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sbxContact.ContactID >0)
                {
                    contact contact = _entity.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault(); 
                                    sales_orderViewSource = (CollectionViewSource)Resources["sales_orderViewSource"];
                    sales_orderViewSource.Source = _entity.sales_order.Where(x => x.id_contact == contact.id_contact).ToList();
                }
            }
            catch
            {
              //  toolBar.msgError(ex);
            }
        }


        public event btnSave_ClickedEventHandler SalesOrder_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            List<sales_order> sales_order = sales_orderDatagrid.ItemsSource.OfType<sales_order>().ToList();
            selected_sales_order = sales_order.Where(x => x.IsSelected == true).ToList();
            if (mode==module.sales_invoice)
            {
                if (selected_sales_order.Count()>1)
                {
                    MessageBox.Show("only one order will be linked..");
                    return;
                }
            }
        
          
            if (SalesOrder_Click != null)
            {
                SalesOrder_Click(sender);
            }

        }

        private void sales_orderDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            sales_order _sales_order = ((System.Windows.Controls.DataGrid)sender).SelectedItem as sales_order;
            int id_salesOrder = _sales_order.id_sales_order;

            Task task_PrimaryData = Task.Factory.StartNew(() => LoadOrderDetail(id_salesOrder, e));
            
        }
        private void LoadOrderDetail(int id, DataGridRowDetailsEventArgs e)
        {
            if (_entity.sales_order_detail.Count() > 0)
            {


                var salesOrder = (from sales_order_detail in _entity.sales_order_detail
                                  where sales_order_detail.id_sales_order == id
                                  join sales_invoice_detail in _entity.sales_invoice_detail
                                  on sales_order_detail.id_sales_order_detail equals sales_invoice_detail.id_sales_order_detail into lst
                                  from list in lst.DefaultIfEmpty()
                                  group list by new
                                  {
                                      sales_order_detail = sales_order_detail,

                                  }
                                      into grouped
                                      select new
                                      {
                                          id = grouped.Key.sales_order_detail.id_sales_order_detail,
                                          name = grouped.Key.sales_order_detail.item.name,
                                          quantity = grouped.Key.sales_order_detail.quantity != null ? grouped.Key.sales_order_detail.quantity : 0,
                                          balance = grouped.Key.sales_order_detail.quantity != null ? grouped.Key.sales_order_detail.quantity : 0 - grouped.Sum(x => x.quantity != null ? x.quantity : 0),
                                      }).ToList().Where(x => x.balance > 0).ToList();
                Dispatcher.Invoke(new Action(() =>
                {
                    System.Windows.Controls.DataGrid RowDataGrid = e.DetailsElement as System.Windows.Controls.DataGrid;
                    RowDataGrid.ItemsSource = salesOrder;
                }));
            }
           
        }
        private void set_ContactPref(object sender, MouseButtonEventArgs e)
        {
            ContactPref(sender, e);
        }

        

        //private void sales_orderDatagrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (sales_orderViewSource != null)
        //    {
        //    List<sales_order> sales_order = sales_orderViewSource.View.Cast<sales_order>().ToList();
        //    foreach (sales_order item in sales_order)
        //    {
        //        if (item != null)
        //        {
        //            item.get_Sales_order_Total();
        //        }
        //    }

        //}

        //}
    }
}