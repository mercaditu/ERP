using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Project
{
    public partial class ProjectExecution : Page
    {
        dbContext dbContext = new dbContext();
        CollectionViewSource project_taskViewSource, projectViewSource;

        public List<project_task> project_task { get; set; }
        public List<project_task> project_main_task { get; set; }

        public ProjectExecution()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_task = new List<entity.project_task>();
            project_taskViewSource = ((CollectionViewSource)(this.FindResource("projectproject_taskViewSource")));

            projectViewSource = ((CollectionViewSource)(this.FindResource("projectViewSource")));
            dbContext.db.projects.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
            projectViewSource.Source = dbContext.db.projects.Local;
            filter_task();
        }

        private void btnProductionOrder(object sender, RoutedEventArgs e)
        {
            if(project_taskViewSource.View != null)
            {
                project_taskViewSource.View.Filter = null;
                project_task = treeProject.ItemsSource.Cast<project_task>().ToList();
                project_task = project_task.Where(x => x.IsSelected == true && (x.status == Status.Project.Approved)).ToList();

                crud_modal.Visibility = Visibility.Visible;

                cntrl.PanelAdv.pnlOrder pnlOrder = new cntrl.PanelAdv.pnlOrder();
                pnlOrder.project_taskLIST = project_task;
                pnlOrder.projectViewSource = project_taskViewSource;

                pnlOrder.shared_dbContext = dbContext;
                crud_modal.Children.Add(pnlOrder);

                filter_task();
            }
        }

        public void filter_task()
        {
            if (project_taskViewSource != null)
            {
                if (project_taskViewSource.View!=null)
                {
                    project_taskViewSource.View.Filter = i =>
                    {
                        project_task _project_task = (project_task)i;
                        if (_project_task.parent == null )
                            return true;
                        else
                        {
                           return false;
                        }
                    };
                }
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    projectViewSource.View.Filter = i =>
                    {
                        project project = i as project;
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
                else
                {
                    projectViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void projectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();
        }
    }
}
