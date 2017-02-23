using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    public partial class pnlSalesOrder : UserControl
    {
        private CollectionViewSource sales_orderViewSource;

        public enum module
        {
            sales_invoice,
            packing_list
        }

        public module mode { get; set; }
        private List<sales_order> _selected_sales_order = null;
        public List<sales_order> selected_sales_order { get { return _selected_sales_order; } set { _selected_sales_order = value; } }
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
                load_SalesOrder(_contact.id_contact);
            }
        }

        private void ContactPref(object sender, RoutedEventArgs e)
        {
            contact contact = _entity.contacts.Find(sbxContact.ContactID);
            if (contact != null)
            {
                load_SalesOrder(contact.id_contact);
            }
        }

        private void load_SalesOrder(int id_contact)
        {
            var order = (from sales_order_detail in _entity.sales_order_detail
                         where sales_order_detail.sales_order.id_contact == id_contact && sales_order_detail.sales_order.status == Status.Documents_General.Approved
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
                             id_sales_order = grouped.Key.sales_order_detail.sales_order.id_sales_order,
                             salesOrder = grouped.Key.sales_order_detail.sales_order,
                             balance = grouped.Key.sales_order_detail != null ? grouped.Key.sales_order_detail.quantity : 0 - grouped.Sum(x => x.quantity)
                         }).ToList().Select(x => x.id_sales_order);

            sales_orderViewSource = (CollectionViewSource)Resources["sales_orderViewSource"];

            sales_orderViewSource.Source = _entity.sales_order.Where(x => order.Contains(x.id_sales_order) ).ToList();
            filter_sales();
        }

        private void filter_sales()
        {
            if (sales_orderViewSource != null)
            {
                if (sales_orderViewSource.View != null)
                {
                    if (sales_orderViewSource.View.OfType<sales_order>().Count() > 0)
                    {
                        sales_orderViewSource.View.Filter = i =>
                        {
                            sales_order sales_order = (sales_order)i;
                            if (sales_order.sales_order_detail.Sum(x => x.balance) > 0)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }

        public event btnSave_ClickedEventHandler SalesOrder_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (sales_orderDatagrid.ItemsSource != null)
            {
                List<sales_order> sales_order = sales_orderDatagrid.ItemsSource.OfType<sales_order>().ToList();
                selected_sales_order = sales_order.Where(x => x.IsSelected == true).ToList();
                if (mode == module.sales_invoice)
                {
                    if (selected_sales_order.Count() > 1)
                    {
                        MessageBox.Show("only one order will be linked..");
                        return;
                    }
                }

                SalesOrder_Click?.Invoke(sender);
            }
        }

        private void sales_orderDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            sales_order _sales_order = ((DataGrid)sender).SelectedItem as sales_order;
            if (_sales_order != null)
            {
                int id_salesOrder = _sales_order.id_sales_order;
                Task task_PrimaryData = Task.Factory.StartNew(() => LoadOrderDetail(id_salesOrder, e));
            }
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
                                      quantity = grouped.Key.sales_order_detail != null ? grouped.Key.sales_order_detail.quantity : 0,
                                      balance = grouped.Key.sales_order_detail != null ? grouped.Key.sales_order_detail.quantity : 0 - grouped.Sum(x => x.quantity),
                                  }).ToList().Where(x => x.balance > 0).ToList();

                Dispatcher.Invoke(new Action(() =>
                {
                    DataGrid RowDataGrid = e.DetailsElement as DataGrid;
                    RowDataGrid.ItemsSource = salesOrder;
                }));
            }
        }

        private void set_ContactPref(object sender, MouseButtonEventArgs e)
        {
            ContactPref(sender, e);
        }
    }
}