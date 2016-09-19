using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using System.ComponentModel;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Cognitivo.Project
{
    public partial class ProjectFinance : Page, INotifyPropertyChanged
    {
        SalesOrderDB SalesOrderDB = new SalesOrderDB();
        CollectionViewSource project_taskViewSource;
        CollectionViewSource projectViewSource;

        public bool ViewAll { get; set; }

        public ProjectFinance()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            SalesOrderDB.projects.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
            projectViewSource.Source = SalesOrderDB.projects.Local;
            set_price();
            filter_task();
        }

        public void set_price()
        {
            if (project_taskViewSource != null)
            {
                if (project_taskViewSource.View != null)
                {
                    foreach (project_task project_task in project_taskViewSource.View.OfType<project_task>())
                    {
                        if (project_task.sales_detail != null)
                        {
                            project_task.unit_price_vat = project_task.sales_detail.UnitPrice_Vat;
                            project_task.RaisePropertyChanged("unit_price_vat");


                        }
                        if (project_task.production_execution_detail != null)
                        {
                            if (project_task.production_execution_detail.Count() > 0)
                            {
                                //Abhi... I would like to handle such things from Entity Level.
                                //project_task.quantity_exe = (decimal)(project_task.production_execution_detail.Sum(x => x.quantity) == 0 ? 1M : project_task.production_execution_detail.Sum(x => x.quantity));
                                //project_task.RaisePropertyChanged("quantity_exec");
                            }

                        }
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
                            if (_project_task.parent == null && _project_task.is_active == true && _project_task.status!=Status.Project.Rejected)
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
            set_price();
            filter_task();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            project project = projectViewSource.View.CurrentItem as project;
            
            if (project != null)
            {
                cntrl.SalesOrder objSalesOrder = new cntrl.SalesOrder();

                objSalesOrder.project = project;
                objSalesOrder.SalesOrderDB = SalesOrderDB;
                
                objSalesOrder.Generate_Invoice = (bool)chkinvoice.IsChecked;
                objSalesOrder.Generate_Budget = (bool)chkbudget.IsChecked;

                ///Crud Modal Visibility and Add.
                crud_modal.Visibility = Visibility.Visible;
                crud_modal.Children.Add(objSalesOrder);   
            }
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
                        List<entity.project_tag_detail> project_tag_detail = new List<entity.project_tag_detail>();
                        project_tag_detail = project.project_tag_detail.ToList();

                        if (project.name.ToLower().Contains(query.ToLower()) || project_tag_detail.Where(x => x.project_tag.name.ToLower().Contains(query.ToLower())).Any())
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

        private void toggleQuantity_CheckedChange(object sender, EventArgs e)
        {
            project project = projectViewSource.View.CurrentItem as project;
            
            if (project != null)
            {
                if (ToggleQuantity.IsChecked == true)
                {
                    foreach (project_task project_task in project.project_task)
                    {
                        project_task.CalcExecutedQty_TimerTaks();
                    }
                }
            }
        }

        private void toolBar_btnSearch_Click_1(object sender, string query)
        {

        }


    }
}