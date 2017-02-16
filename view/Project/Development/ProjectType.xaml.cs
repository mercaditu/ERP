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
    public partial class ProjectType : Page
    {
        private ProjectTemplateDB ProjectTemplateDB = new ProjectTemplateDB();

        private CollectionViewSource projectproject_template_detailViewSource;
        private CollectionViewSource project_templateViewSource;

        public ProjectType()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_templateViewSource = FindResource("project_templateViewSource") as CollectionViewSource;
            await ProjectTemplateDB.project_template.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            project_templateViewSource.Source = ProjectTemplateDB.project_template.Local;

            projectproject_template_detailViewSource = FindResource("projectproject_template_detailViewSource") as CollectionViewSource;

            cbxItemType.ItemsSource = Enum.GetValues(typeof(item.item_type));

            filter_task();
        }

        public void filter_task()
        {
            if (projectproject_template_detailViewSource != null)
            {
                if (projectproject_template_detailViewSource.View != null)
                {
                    projectproject_template_detailViewSource.View.Filter = i =>
                    {
                        project_template_detail project_template_detail = (project_template_detail)i;
                        if (project_template_detail.State != EntityState.Deleted)
                        {
                            if (project_template_detail.parent == null)
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
            }
        }

        #region Toolbar Events

        private void toolBar_btnNew_Click(object sender)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.projecttemplate projecttemplate = new cntrl.Curd.projecttemplate();
            project_template projecttemplateobject = new project_template();
            ProjectTemplateDB.project_template.Add(projecttemplateobject);

            project_templateViewSource.View.Refresh();

            projecttemplate.projecttemplateobject = projecttemplateobject;
            projecttemplate.projecttemplateViewSource = project_templateViewSource;
            projecttemplate.projecttemplatedetailViewSource = projectproject_template_detailViewSource;
            projecttemplate._entity = ProjectTemplateDB;

            crud_modal.Children.Add(projecttemplate);
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            //MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (res == MessageBoxResult.Yes)
            //{
            //    project_template projects = project_templateViewSource.View.CurrentItem as project_template;
            //    projects.is_active = false;
            //    _entity.SaveChanges();

            //}
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.projecttemplate projecttemplate = new cntrl.Curd.projecttemplate();
            projecttemplate.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            projecttemplate.projecttemplateobject = (project_template)project_templateViewSource.View.CurrentItem;
            projecttemplate.projecttemplatedetailViewSource = projectproject_template_detailViewSource;
            projecttemplate.projecttemplateViewSource = project_templateViewSource;

            projecttemplate._entity = ProjectTemplateDB;
            crud_modal.Children.Add(projecttemplate);
        }

        #endregion Toolbar Events

        #region Project Type Events

        private void btnNewTask_Click(object sender)
        {
            stpcode.IsEnabled = true;
            //itemSearchViewSource.View.Filter = i =>
            //{
            //    item item = (item)i;
            //    if (item.is_active == true)
            //        return true;
            //    else
            //        return false;
            //};

            project_template project_template = project_templateViewSource.View.CurrentItem as project_template;
            project_template_detail project_template_detail = treeProject.SelectedItem_ as project_template_detail;

            if (project_template_detail != null && project_template_detail.item.id_item_type == entity.item.item_type.Task)
            {
                //Adding a Child Item.
                project_template_detail n_project_template = new project_template_detail();
                n_project_template.id_project_template = project_template.id_project_template;
                n_project_template.status = Status.Project.Pending;
                project_template_detail.child.Add(n_project_template);

                ProjectTemplateDB.project_template_detail.Add(n_project_template);

                projectproject_template_detailViewSource.View.Filter = null;

                filter_task();
                treeProject.SelectedItem_ = n_project_template;
            }
            else
            {
                toolBar.msgWarning("Please Select Task");
            }
            projectproject_template_detailViewSource.View.MoveCurrentToLast();
        }

        private void btnAddParentTask_Click(object sender)
        {
            stpcode.IsEnabled = true;
            //itemSearchViewSource.View.Filter = i =>
            //{
            //    item item = (item)i;
            //    if (item.is_active == true)
            //        return true;
            //    else
            //        return false;
            //};
            project_template project_template = project_templateViewSource.View.CurrentItem as project_template;

            project_template_detail n_project_template = new project_template_detail();
            n_project_template.id_project_template = project_template.id_project_template;
            n_project_template.status = Status.Project.Pending;
            ProjectTemplateDB.project_template_detail.Add(n_project_template);

            projectproject_template_detailViewSource.View.Filter = null;

            filter_task();
            projectproject_template_detailViewSource.View.MoveCurrentToLast();
            treeProject.SelectedItem_ = n_project_template;
        }

        private void btnEditTask_Click(object sender)
        {
            stpcode.IsEnabled = true;
        }

        private void btnSaveTask_Click(object sender)
        {
            if (ProjectTemplateDB.SaveChanges() > 0)
            {
                stpcode.IsEnabled = false;
                toolBar.msgSaved(ProjectTemplateDB.NumberOfRecords);
            }
        }

        private void btnDeleteTask_Click(object sender)
        {
            if (projectproject_template_detailViewSource.View != null)
            {
                projectproject_template_detailViewSource.View.Filter = null;
                List<project_template_detail> project_template_detailLIST = treeProject.ItemsSource.Cast<project_template_detail>().ToList();
                project_template_detailLIST = project_template_detailLIST.Where(x => x.IsSelected == true).ToList();
                using (db db = new db())
                {
                    foreach (project_template_detail project_template_detail in project_template_detailLIST)
                    {
                        project_template_detail _project_template_detail = db.project_template_detail.Where(x => x.id_template_detail == project_template_detail.id_template_detail).FirstOrDefault();

                        db.project_template_detail.Remove(_project_template_detail);
                    }
                    db.SaveChanges();
                }

                ProjectTemplateDB = new ProjectTemplateDB();
                project_templateViewSource = ((CollectionViewSource)(FindResource("project_templateViewSource")));
                ProjectTemplateDB.project_template.Where(a => a.id_company == CurrentSession.Id_Company).Load();
                project_templateViewSource.Source = ProjectTemplateDB.project_template.Local;
                filter_task();
            }
        }

        #endregion Project Type Events

        private void btnChild_MouseUp(object sender, MouseButtonEventArgs e)
        {
            stpcode.Visibility = Visibility.Visible;
        }

        private void cbxItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxItemType = (ComboBox)sender;
            if (cbxItemType.SelectedItem != null)
            {
                item.item_type Item_Type = (item.item_type)cbxItemType.SelectedItem;
                sbxItem.item_types = Item_Type;
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    project_templateViewSource.View.Filter = i =>
                    {
                        project_template project_template = i as project_template;
                        if (project_template.name != null)
                        {
                            if (project_template.name.ToLower().Contains(query.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    project_templateViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
            filter_task();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();
        }

        private void toolBar_btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectTemplateDB.Approve())
            {
                toolBar.msgApproved(ProjectTemplateDB.NumberOfRecords);
                project_templateViewSource.View.Refresh();
            }
        }

        private void toolBar_btnAnull_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectTemplateDB.Anull())
            {
                toolBar.msgAnnulled(ProjectTemplateDB.NumberOfRecords);
                project_templateViewSource.View.Refresh();
            }
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item item = ProjectTemplateDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                project_template_detail project_template_detail = (project_template_detail)treeProject.SelectedItem_;
                project_template_detail.id_item = item.id_item;
                project_template_detail.item = item;
            }
        }
    }
}