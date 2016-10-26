using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.Data.Entity;
using System;

namespace cntrl.PanelAdv
{
    public partial class pnlSalesBudget : UserControl
    {
        CollectionViewSource sales_budgetViewSource;
        public contact _contact { get; set; }

        public sales_order sales_order { get; set; }
        public db db { get; set; }
        public pnlSalesBudget()
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



                    load_SalesBudget(_contact.id_contact);
                }
                filter_sales();

            }
        }
        private void load_SalesBudget(int id_contact)
        {
            var salesBudget = (from sales_budget_detail in db.sales_budget_detail
                               where sales_budget_detail.sales_budget.status == Status.Documents_General.Approved
                               && sales_budget_detail.sales_budget.contact.id_contact == id_contact
                               join sales_order_detail in db.sales_order_detail
                              on sales_budget_detail.id_sales_budget_detail equals sales_order_detail.id_sales_budget_detail into lst
                               from list in lst.DefaultIfEmpty()
                               group list by new
                               {
                                   sales_budget_detail = sales_budget_detail,
                               }
                                   into grouped
                                   select new
                                   {
                                       id = grouped.Key.sales_budget_detail.id_sales_budget,
                                       item = grouped.Key.sales_budget_detail.item_description,
                                       quantity = grouped.Key.sales_budget_detail.quantity > 0 ? grouped.Key.sales_budget_detail.quantity : 0,
                                       balance = grouped.Key.sales_budget_detail.quantity > 0 ? grouped.Key.sales_budget_detail.quantity : 0 - grouped.Sum(x => x.quantity > 0 ? x.quantity : 0),
                                   }).ToList()
                 
                  .Select(x => x.id);
            sales_budgetViewSource = (CollectionViewSource)Resources["sales_budgetViewSource"];
            sales_budgetViewSource.Source = db.sales_budget.Where(x => salesBudget.Contains(x.id_sales_budget)).ToList();

        }
        void filter_sales()
        {
            if (sales_budgetViewSource != null)
            {
                if (sales_budgetViewSource.View != null)
                {
                    if (sales_budgetViewSource.View.OfType<sales_budget>().Count() > 0)
                    {
                        sales_budgetViewSource.View.Filter = i =>
                        {
                            sales_budget sales_budget = (sales_budget)i;
                            if (sales_budget.sales_budget_detail.Sum(x => x.balance) > 0)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }
        public event btnSave_ClickedEventHandler SalesBudget_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (SalesBudget_Click != null)
            {
                foreach (sales_budget sales_budget in sales_budgetViewSource.View.OfType<sales_budget>().Where(x => x.IsSelected))
                {
                    sales_order.id_condition = sales_budget.id_condition;
                    sales_order.id_contract = sales_budget.id_contract;
                    sales_order.id_currencyfx = sales_budget.id_currencyfx;
                    //sales_order.contact = sales_budget.contact;
                    sales_order.id_contact = sales_budget.id_contact;
                    foreach (sales_budget_detail sales_budget_detail in sales_budget.sales_budget_detail)
                    {


                        sales_order_detail sales_order_detail = new sales_order_detail();
                        sales_order_detail.sales_budget_detail = sales_budget_detail;
                        sales_order_detail.id_sales_budget_detail = sales_budget_detail.id_sales_budget_detail;
                        sales_order_detail.id_item = sales_budget_detail.id_item;
                        sales_order_detail.id_vat_group = sales_budget_detail.id_vat_group;
                        sales_order_detail.unit_price = sales_budget_detail.unit_price;
                        
                        decimal quantity = 0;

                        quantity = db.sales_order_detail
                            .Where(x => x.id_sales_budget_detail == sales_budget_detail.id_sales_budget_detail
                            && (x.sales_order.status == Status.Documents_General.Approved || x.sales_order.status == Status.Documents_General.Pending))
                            .GroupBy(x => x.id_sales_budget_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault();

                        sales_order_detail.quantity = sales_budget_detail.quantity - quantity;

                        if (sales_order_detail.quantity <= 0)
                        {
                            continue;
                        }


                        sales_order.sales_order_detail.Add(sales_order_detail);
                    }
                }



                SalesBudget_Click(sender);
            }

        }



        private void sales_orderDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            sales_budget _sales_budget = ((DataGrid)sender).SelectedItem as sales_budget;
            int id_sales_budget = _sales_budget.id_sales_budget;
            DataGrid RowDataGrid = e.DetailsElement as DataGrid;


            var salesBudget = (from sales_budget_detail in db.sales_budget_detail
                               where sales_budget_detail.id_sales_budget == id_sales_budget
                               && sales_budget_detail.sales_budget.status == Status.Documents_General.Approved
                               join sales_order_detail in db.sales_order_detail
                               on sales_budget_detail.id_sales_budget_detail equals sales_order_detail.id_sales_order_detail into lst
                               from list in lst.DefaultIfEmpty()
                               group list by new
                               {
                                   sales_budget_detail = sales_budget_detail,

                               }
                                   into grouped
                                   select new
                                   {
                                       item = grouped.Key.sales_budget_detail.item_description,
                                       unitprice = grouped.Key.sales_budget_detail.unit_price > 0 ? grouped.Key.sales_budget_detail.unit_price : 0,
                                       balance = grouped.Key.sales_budget_detail.quantity > 0 ? grouped.Key.sales_budget_detail.quantity : 0 - grouped.Sum(x => x.quantity > 0 ? x.quantity : 0),

                                   }).ToList();

            RowDataGrid.ItemsSource = salesBudget;

        }


        private void set_ContactPrefKeyStroke(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                set_ContactPref(sender, null);
            }
        }

        private void set_ContactPref(object sender, MouseButtonEventArgs e)
        {
            if (contactComboBox.Data != null)
            {
                sales_budget sales_budget = (sales_budget)contactComboBox.Data;
                contactComboBox.focusGrid = false;
                contactComboBox.Text = sales_budget.number;
            }
        }


    }
}