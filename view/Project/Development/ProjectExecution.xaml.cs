using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Project
{
    public partial class ProjectExecution : Page
    {
        private db db = new db();
        private CollectionViewSource project_taskViewSource, projectViewSource;

        public List<project_task> project_task { get; set; }
        public List<project_task> project_main_task { get; set; }

        public ProjectExecution()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, EventArgs e)
        {
            project_task = new List<entity.project_task>();
            project_taskViewSource = ((CollectionViewSource)(this.FindResource("projectproject_taskViewSource")));

            projectViewSource = ((CollectionViewSource)(this.FindResource("projectViewSource")));
            await db.projects.Where(a => a.is_active == true && a.is_archived == false && a.id_company == CurrentSession.Id_Company).LoadAsync();
            projectViewSource.Source = db.projects.Local;
            filter_task();
        }

        private void btnProductionOrder(object sender, RoutedEventArgs e)
        {
            if (project_taskViewSource.View != null)
            {
                project_taskViewSource.View.Filter = null;
                foreach (project_task project_task in treeProject.ItemsSource.Cast<project_task>().ToList().Where(x => x.IsSelected == true && (x.status == Status.Project.Approved || x.status == Status.Project.InProcess)))
                {
                    if (project_task.parent != null)
                    {
                        project_task.parent.Parent_Selection();
                    }
                }
                project_task = treeProject.ItemsSource.Cast<project_task>().ToList().Where(x => x.IsSelected == true).ToList();

                if (project_task.Count() > 0)
                {
                    crud_modal.Visibility = Visibility.Visible;

                    cntrl.PanelAdv.pnlOrder pnlOrder = new cntrl.PanelAdv.pnlOrder();
                    pnlOrder.project_taskLIST = project_task;
                    pnlOrder.projectViewSource = project_taskViewSource;

                    pnlOrder.Shared_dbContext = db;
                    crud_modal.Children.Add(pnlOrder);
                }
                else
                {
                    toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
                }

                filter_task();
            }
        }

        public void filter_task()
        {
            if (project_taskViewSource != null)
            {
                if (project_taskViewSource.View != null)
                {
                    project_taskViewSource.View.Filter = i =>
                    {
                        project_task _project_task = (project_task)i;
                        if (_project_task.parent == null)
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
                    // projectViewSource.View.Filter = null;
                }
                filter_task();
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

        private void btnExpandAll_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}