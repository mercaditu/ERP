using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace Cognitivo.Project
{
    public partial class ProjectFinance : Page, INotifyPropertyChanged
    {
        private SalesOrderDB SalesOrderDB = new SalesOrderDB();
        private CollectionViewSource project_taskViewSource;
        private CollectionViewSource projectViewSource;

        public bool ViewAll { get; set; }

        public ProjectFinance()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));

            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            await SalesOrderDB.projects.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            projectViewSource.Source = SalesOrderDB.projects.Local;

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
                            if (_project_task.parent == null && _project_task.is_active == true && _project_task.status != Status.Project.Rejected)
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
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            project project = projectViewSource.View.CurrentItem as project;

            if (project != null)
            {
                cntrl.SalesOrder objSalesOrder = new cntrl.SalesOrder();

                objSalesOrder.project = project;
                objSalesOrder.db = SalesOrderDB;
                objSalesOrder.Percentage = 0;

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
                        //bool HasTag = false;

                        //if (project != null)
                        //{
                        //    if (project.project_tag_detail.Count() > 0)
                        //    {
                        //        List<project_tag_detail> project_tag_detail = project.project_tag_detail.ToList();
                        //        HasTag = project_tag_detail.Where(x => x.project_tag.name.ToLower().Contains(query.ToLower())).Any();
                        //    }
                        //}

                        if (project.name.ToLower().Contains(query.ToLower()))
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
            filter_task();
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

        //private void toggleQuantity_CheckedChange(object sender, EventArgs e)
        //{
        //    project project = projectViewSource.View.CurrentItem as project;

        //    if (project != null)
        //    {
        //        if (ToggleQuantity.IsChecked == true)
        //        {
        //            foreach (project_task project_task in project.project_task)
        //            {
        //                project_task.CalcExecutedQty_TimerTaks();
        //            }
        //        }
        //    }
        //}

        private void Price_Click(object sender, RoutedEventArgs e)
        {
            set_price();
        }

        public void set_price()
        {
            project project = projectViewSource.View.CurrentItem as project;
            if (project_taskViewSource != null)
            {
                if (project_taskViewSource.View != null)
                {
                    if (project != null)
                    {
                        List<project_task> project_taskLIST = project.project_task.Where(x => x.IsSelected).OrderByDescending(x => x.id_project_task).ToList();
                        foreach (project_task project_task in project_taskLIST)
                        {
                            if (project_task.items.id_item_type == item.item_type.Task)
                            {
                                project_task.CalcSalePrice_TimerParentTaks();
                            }
                            else
                            {
                                project_task.CalcSalePrice_TimerTaks();
                            }
                        }
                    }
                }
            }
        }

        private void Quantity_Click(object sender, RoutedEventArgs e)
        {
            project project = projectViewSource.View.CurrentItem as project;
            if (project_taskViewSource != null)
            {
                if (project_taskViewSource.View != null)
                {
                    if (project != null)
                    {
                        List<project_task> project_taskLIST = project.project_task.Where(x => x.IsSelected).OrderByDescending(x => x.id_project_task).ToList();
                        foreach (project_task project_task in project_taskLIST)
                        {
                            project_task.CalcExecutedQty_TimerTaks();
                        }
                    }
                }
            }
        }
    }
}