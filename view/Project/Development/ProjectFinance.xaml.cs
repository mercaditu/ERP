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
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Cognitivo.Project
{
    /// <summary>
    /// Interaction logic for ProjectInvoice.xaml
    /// </summary>
    public partial class ProjectFinance : Page, INotifyPropertyChanged
    {
        SalesOrderDB SalesOrderDB = new entity.SalesOrderDB();
      

        CollectionViewSource project_taskViewSource;
        CollectionViewSource projectViewSource;

        public Boolean ViewAll { get; set; }
        public ProjectFinance()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            SalesOrderDB.projects.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Include(x => x.project_task).Load();
            projectViewSource.Source = SalesOrderDB.projects.Local;

            filter_task();

            set_price();

        }

        public void set_price()
        {
            if (project_taskViewSource != null)
            {
                foreach (project_task project_task in project_taskViewSource.View.OfType<project_task>())
                {
                    if (project_task.sales_detail != null)
                    {
                        project_task.unit_price_vat = project_task.sales_detail.UnitPrice_Vat;
                        project_task.RaisePropertyChanged("unit_price_vat");
                    }
                }
            }
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
           calculate_total();
            

        }
        private void calculate_total()
        {
            project project = projectViewSource.View.CurrentItem as project;
            if (project != null)
            {
                project.total_cost = (decimal)project.project_task.Sum(x => x.unit_price_vat);
                project.RaisePropertyChanged("total_cost");
                project.total_paid = 0;
                foreach (project_task project_task in project.project_task)
                {
                    if (project_task.sales_detail != null)
                    {
                        sales_order_detail sales_order_detail = project_task.sales_detail;
                        if (sales_order_detail.sales_invoice_detail != null)
                        {
                            foreach (sales_invoice_detail sales_invoice_detail in sales_order_detail.sales_invoice_detail)
                            {
                                sales_invoice sales_invoice = sales_invoice_detail.sales_invoice;
                                if (sales_invoice.payment_schedual.Sum(x => x.AccountReceivableBalance) == 0)
                                {
                                    project.total_paid = Convert.ToDecimal(project.total_paid + project_task.unit_price_vat);
                                }
                            }
                        }
                    }
                    else if (project_task.sales_invoice_detail != null)
                    {
                        foreach (sales_invoice_detail sales_invoice_detail in project_task.sales_invoice_detail)
                        {
                            sales_invoice sales_invoice = sales_invoice_detail.sales_invoice;
                            if (sales_invoice.payment_schedual.Sum(x => x.AccountReceivableBalance) == 0)
                            {
                                project.total_paid = Convert.ToDecimal(project.total_paid + project_task.unit_price_vat);
                            }
                            
                        }
                        
                    }

                }
                project.RaisePropertyChanged("total_paid");
                project.pending_payment = project.total_cost - project.total_paid;
                project.RaisePropertyChanged("pending_payment");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
          
                project project = projectViewSource.View.CurrentItem as project;
                crud_modal.Visibility = System.Windows.Visibility.Visible;
                cntrl.SalesOrder objSalesOrder = new cntrl.SalesOrder();
                objSalesOrder.project = project;
                objSalesOrder.SalesOrderDB = SalesOrderDB;
                objSalesOrder.Generate_Invoice =(bool)chkinvoice.IsChecked;
                crud_modal.Children.Add(objSalesOrder);
          

            
            
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

        private void salesorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            sales_order sales_order = (sales_order)Hyperlink.Tag;
            if (sales_order != null)
            {
                entity.Brillo.Document.Start.Automatic(sales_order, sales_order.app_document_range);
            }
        }
    }
}