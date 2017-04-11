using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Project.Development
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class Project : Page
    {
        private ProjectDB ProjectDB = new ProjectDB();
        private CollectionViewSource ProjectViewSource, Projectproject_tag_detail;

        public Project()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender)
        {
            project project = new project();
            project.IsSelected = true;
            project.State = EntityState.Added;
            ProjectDB.Entry(project).State = EntityState.Added;
            ProjectViewSource.View.MoveCurrentToLast();
        }

        private void btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    project project = (project)ProjectDataGrid.SelectedItem;
                    project.is_head = false;
                    project.State = EntityState.Deleted;
                    project.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private async void btnSave_Click(object sender)
        {
            entity.project _project = ((entity.project)ProjectViewSource.View.CurrentItem);
            int id_type = 0;
            if (_project.State==EntityState.Added)
            {
                if (_project != null)
                {


                    if (_project.id_project_template != null)
                    {
                        id_type = (int)_project.id_project_template;
                    }

                    await ProjectDB.project_template_detail.Where(x => x.id_project_template == id_type).ToListAsync();
                    List<project_template_detail> project_template_detail = ProjectDB.project_template_detail.Local.ToList();

                    if (project_template_detail != null)
                    {
                        foreach (project_template_detail item in project_template_detail)
                        {
                            project_task project_task = new project_task();
                            project_task.id_project_task = item.id_template_detail;
                            project_task.items = item.item;
                            if (item.item_description != null)
                            {
                                project_task.item_description = item.item_description;
                            }
                            else
                            {
                                project_task.item_description = "Generic Task - Please Replace";
                            }

                            project_task.code = item.code;
                            project_task.id_item = item.id_item;
                            project_task.status = Status.Project.Pending;

                            if (item.parent != null)
                            {
                                project_task _project_task = ProjectDB.project_task.Local.Where(x => x.id_project_task == item.parent.id_template_detail).FirstOrDefault();
                                _project_task.child.Add(project_task);
                            }

                            _project.project_task.Add(project_task);
                            // }
                        }
                    }
                }
            }

         

            if (ProjectDB.SaveChanges() > 0)
            {
                ProjectViewSource.View.Refresh();
                toolBar.msgSaved(ProjectDB.NumberOfRecords);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (ProjectDataGrid.SelectedItem != null)
            {
                project project_old = (project)ProjectDataGrid.SelectedItem;
                project_old.IsSelected = true;
                project_old.State = EntityState.Modified;
                ProjectDB.Entry(project_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && ProjectViewSource != null)
            {
                try
                {
                    ProjectViewSource.View.Filter = i =>
                    {
                        project project = i as project;
                        List<project_tag_detail> project_tag_detail = new List<entity.project_tag_detail>();
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
                ProjectViewSource.View.Filter = null;
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ProjectDB.CancelAllChanges();
        }

        private async void Project_Loaded(object sender, EventArgs e)
        {
            await ProjectDB.projects.Where(a => a.id_company == CurrentSession.Id_Company
                                            && (a.is_head == true)).Include(y => y.contact).ToListAsync();

            ProjectViewSource = ((CollectionViewSource)(FindResource("ProjectViewSource")));
            ProjectViewSource.Source = ProjectDB.projects.Local;


            CollectionViewSource templateViewSource = (CollectionViewSource)FindResource("project_templateViewSource");
            templateViewSource.Source = await ProjectDB.project_template.Where(x => x.is_active == true && x.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).AsNoTracking().ToListAsync();


            CollectionViewSource branchViewSource = ((CollectionViewSource)(FindResource("branchViewSource")));
            branchViewSource.Source = await ProjectDB.app_branch.Where(x => x.id_company == CurrentSession.Id_Company).ToListAsync();

            await ProjectDB.project_tag
            .Where(x => x.id_company == CurrentSession.Id_Company && x.is_active == true)
            .OrderBy(x => x.name).LoadAsync();

            Projectproject_tag_detail = ((CollectionViewSource)(FindResource("Projectproject_tag_detail")));
            CollectionViewSource project_tagViewSource = ((CollectionViewSource)(FindResource("project_tagViewSource")));
            project_tagViewSource.Source = ProjectDB.project_tag.Local;
        }

        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            ProjectDB.ActivateProject();
            ProjectViewSource.View.Refresh();
        }

        private void DeActivate_Click(object sender, RoutedEventArgs e)
        {
            ProjectDB.DeActivateProject();
            ProjectViewSource.View.Refresh();
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as project_tag_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (e.Parameter as project_tag_detail != null)
                    {
                        project_tag_detailDataGrid.CancelEdit();
                        ProjectDB.project_tag_detail.Remove(e.Parameter as project_tag_detail);
                        Projectproject_tag_detail.View.Refresh();
                    }
                }
            }
            catch
            {
            }
        }

        private void cbxTag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add_Tag();
            }
        }

        private void cbxTag_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Add_Tag();
        }

        private void Add_Tag()
        {
            // CollectionViewSource item_tagViewSource = ((CollectionViewSource)(FindResource("item_tagViewSource")));
            if (cbxTag.Data != null)
            {
                int id = Convert.ToInt32(((project_tag)cbxTag.Data).id_tag);
                if (id > 0)
                {
                    project project = ProjectViewSource.View.CurrentItem as project;
                    if (project != null)
                    {
                        project_tag_detail project_tag_detail = new project_tag_detail();
                        project_tag_detail.id_tag = ((project_tag)cbxTag.Data).id_tag;
                        project_tag_detail.project_tag = ((project_tag)cbxTag.Data);
                        project.project_tag_detail.Add(project_tag_detail);
                        CollectionViewSource Projectproject_tag_detail = FindResource("Projectproject_tag_detail") as CollectionViewSource;
                        Projectproject_tag_detail.View.Refresh();
                    }
                }
            }
        }

        private void contactComboBox_Select(object sender, RoutedEventArgs e)
        {
        }
    }
}