using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.Entity;

namespace cntrl
{
    /// <summary>
    /// Interaction logic for SalesOrder.xaml
    /// </summary>
    public partial class SalesOrder : UserControl
    {
       
        public project project { get; set; }
        public db db { get; set; }
        public SalesOrder()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            List<project_task> project_task = project.project_task.Where(x => x.IsSelected).ToList();
            sales_order sales_order = new entity.sales_order();
            sales_order.id_contact = (int)project.id_contact;
            sales_order.contact = db.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault();
            if (db.app_document_range.Where(x => x.app_document.id_application == entity.App.Names.SalesOrder && x.is_active == true).FirstOrDefault() != null)
            {
                sales_order.id_range = db.app_document_range.Where(x => x.app_document.id_application == entity.App.Names.SalesOrder && x.is_active == true).FirstOrDefault().id_range;
            }
            sales_order.id_project = project.id_project;
            sales_order.id_condition = (int)cbxCondition.SelectedValue;
            sales_order.id_contract = (int)cbxContract.SelectedValue;
            sales_order.id_currencyfx = (int)cbxCurrency.SelectedValue;
            sales_order.comment = "Generate From Project";
            sales_order_detail sales_order_detail = null;
            foreach (project_task _project_task in project_task)
            {
               
                if (_project_task.items.id_item_type==item.item_type.Task)
                {
                    sales_order_detail = new sales_order_detail();
                    sales_order_detail.id_sales_order = sales_order.id_sales_order;
                    sales_order_detail.sales_order = sales_order;
                    sales_order_detail.id_item = (int)_project_task.id_item;
                    sales_order_detail.item_description = _project_task.item_description;
                    sales_order_detail.quantity = (int)_project_task.quantity_est;
                    sales_order_detail.UnitPrice_Vat = (int)_project_task.unit_cost_est;
                    _project_task.sales_detail = sales_order_detail;
                    _project_task.IsSelected = false;
                   
                }
                else
                {
                    if (sales_order_detail!=null)
                    {
                        _project_task.sales_detail = sales_order_detail;
                        _project_task.IsSelected = false;
                    }
                    
                }
                sales_order.sales_order_detail.Add(sales_order_detail);
              
            }
            sales_order.State = EntityState.Added;
            sales_order.IsSelected = true;
            db.sales_order.Add(sales_order);
            db.SaveChanges();
            btnCancel_Click(null, null);
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
               
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            db.app_contract.Where(a => a.is_active == true && a.id_company == entity.Properties.Settings.Default.company_ID).ToList();

            cbxContract.ItemsSource = db.app_contract.Local;


            db.app_condition.Where(a => a.is_active == true && a.id_company == entity.Properties.Settings.Default.company_ID).OrderBy(a => a.name).ToList();

            cbxCondition.ItemsSource = db.app_condition.Local;

            stackMain.DataContext = project;
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxCondition.SelectedItem != null)
            {
                
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = db.app_contract.Where(a => a.is_active == true
                                                                        && a.id_company == entity.Properties.Settings.Default.company_ID
                                                                        && a.id_condition == app_condition.id_condition).ToList();
                cbxContract.SelectedIndex = 0;
            }
        }
    }
}
