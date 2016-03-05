using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.ComponentModel;

namespace Cognitivo.Project
{
    /// <summary>
    /// Interaction logic for ProjectInvoice.xaml
    /// </summary>
    public partial class ProjectInvoice : Page, INotifyPropertyChanged
    {
        SalesOrderDB SalesOrderDB = new entity.SalesOrderDB();

        CollectionViewSource project_taskViewSource;
        CollectionViewSource projectViewSource;
        entity.Properties.Settings _Setting = new entity.Properties.Settings();
        public Boolean ViewAll { get; set; }
        public ProjectInvoice()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            SalesOrderDB.projects.Where(a => a.is_active == true && a.id_company == _Setting.company_ID).Include("project_task").Load();
            projectViewSource.Source = SalesOrderDB.projects.Local;

           


            //Filter to remove all items that are not top level.
            filter_task();
        }
        public void filter_task()
        {
            try
            {
                if (project_taskViewSource != null)
                {
                    if (project_taskViewSource.View != null)
                    {
                        project_taskViewSource.View.Filter = i =>
                        {
                            project_task _project_task = (project_task)i;
                            if (_project_task.parent == null && _project_task.is_active == true)
                                return true;
                            else
                                return false;
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void btnExpandAll_Checked(object sender, RoutedEventArgs e)
        {
            ViewAll = !ViewAll;
            RaisePropertyChanged("ViewAll");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }





        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            project project = projectViewSource.View.CurrentItem as project;
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.SalesOrder objSalesOrder = new cntrl.SalesOrder();
            objSalesOrder.project = project;
            objSalesOrder.db = SalesOrderDB;
            crud_modal.Children.Add(objSalesOrder);

            //project project = projectViewSource.View.CurrentItem as project;
            //List<project_task> project_task = project.project_task.Where(x => x.IsSelected).ToList();
            //sales_order sales_order = new entity.sales_order();
            //sales_order.id_contact = (int)project.id_contact;
            //sales_order.contact = SalesOrderDB.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault();
            //if (SalesOrderDB.app_document_range.Where(x => x.app_document.id_application == entity.App.Names.SalesOrder && x.is_active == true).FirstOrDefault() != null)
            //{
            //    sales_order.id_range = SalesOrderDB.app_document_range.Where(x => x.app_document.id_application == entity.App.Names.SalesOrder && x.is_active == true).FirstOrDefault().id_range;
            //}
            //sales_order.id_condition = (int)cbxCondition.SelectedValue;
            //sales_order.id_contract = (int)cbxContract.SelectedValue;
            //sales_order.id_currencyfx = (int)cbxCurrency.SelectedValue;
            //sales_order.comment = "Generate From Project";
            //sales_order_detail sales_order_detail = null;
            //foreach (project_task _project_task in project_task)
            //{
               
            //    if (_project_task.items.id_item_type==item.item_type.Task)
            //    {
            //        sales_order_detail = new sales_order_detail();
            //        sales_order_detail.id_sales_order = sales_order.id_sales_order;
            //        sales_order_detail.sales_order = sales_order;
            //        sales_order_detail.id_item = (int)_project_task.id_item;
            //        sales_order_detail.quantity = (int)_project_task.quantity_est;
            //        sales_order_detail.unit_cost = (int)_project_task.unit_cost_est;
            //        _project_task.sales_detail = sales_order_detail;
            //        _project_task.IsSelected = false;
                   
            //    }
            //    else
            //    {
            //        if (sales_order_detail!=null)
            //        {
            //            _project_task.sales_detail = sales_order_detail;
            //            _project_task.IsSelected = false;
            //        }
                    
            //    }
            //    sales_order.sales_order_detail.Add(sales_order_detail);
              
            //}
            //sales_order.State = EntityState.Added;
            //sales_order.IsSelected = true;
            //SalesOrderDB.sales_order.Add(sales_order);
            //SalesOrderDB.SaveChanges();
            //filter_task();
        }

       

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && projectViewSource != null)
            {
                try
                {
                    projectViewSource.View.Filter = i =>
                    {
                        project project = i as project;
                        if (project.name.ToLower().Contains(query.ToLower())
                            )
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    toolBar.msgError(ex);
                }
            }
            else
            {
                projectViewSource.View.Filter = null;
            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            filter_task();
        }

   

       
    }
}