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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            await SalesOrderDB.projects.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Include(x => x.project_task).LoadAsync();
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