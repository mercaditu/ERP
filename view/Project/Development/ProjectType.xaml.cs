using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;

namespace Cognitivo.Project.Development
{
    public partial class ProjectType : Page
    {
        ProjectTemplateDB dbContext = new ProjectTemplateDB();

        CollectionViewSource projectproject_template_detailViewSource;
        CollectionViewSource project_templateViewSource;
        CollectionViewSource itemSearchViewSource;
        entity.Properties.Settings _Setting = new entity.Properties.Settings();


        //SetIsEnableProperty



        public ProjectType()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           

            project_templateViewSource = ((CollectionViewSource)(FindResource("project_templateViewSource")));
            dbContext.project_template.Where(a => a.id_company == _Setting.company_ID).LoadAsync();
            project_templateViewSource.Source = dbContext.project_template.Local;


            //Loading Products
            dbContext.items.Where(a => a.id_company == _Setting.company_ID).OrderBy(b => b.name).LoadAsync();

            CollectionViewSource itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
            itemViewSource.Source = dbContext.items.Local;

            itemSearchViewSource = ((CollectionViewSource)(FindResource("itemSearchViewSource")));
            itemSearchViewSource.Source = dbContext.items.Local;


            //Filter to remove all items that are not top level.
            projectproject_template_detailViewSource = ((CollectionViewSource)(FindResource("projectproject_template_detailViewSource")));

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
                        if (project_template_detail.parent == null)
                            return true;
                        else
                            return false;
                    };
                }
            }
        }

        #region Toolbar Events
        private void toolBar_btnNew_Click(object sender)
        {

            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.projecttemplate projecttemplate = new cntrl.Curd.projecttemplate();
            projecttemplate.projecttemplateViewSource = project_templateViewSource;
            projecttemplate.projecttemplatedetailViewSource = projectproject_template_detailViewSource;
            projecttemplate._entity = dbContext;

            crud_modal.Children.Add(projecttemplate);
            //project_templateViewSource.View.MoveCurrentToLast();
            //project_templateViewSource.View.Refresh();

            // dbContext.SaveChanges();

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

            projecttemplate._entity = dbContext;

            crud_modal.Children.Add(projecttemplate);

            //cntrl.Curd.contact contact = new cntrl.Curd.contact();

        }
        #endregion

        #region Project Type Events

        private void btnNewTask_Click(object sender, EventArgs e)
        {
            stpcode.IsEnabled = true;
            itemSearchViewSource.View.Filter = i =>
            {
                item item = (item)i;
                if (item.is_active == true)
                    return true;
                else
                    return false;
            };


            project_template project_template = project_templateViewSource.View.CurrentItem as project_template;
            project_template_detail project_template_detail = treeProject.SelectedItem as project_template_detail;

            if (project_template_detail != null)
            {
                //Adding a Child Item.
                project_template_detail n_project_template = new project_template_detail();
                n_project_template.id_project_template = project_template.id_project_template;
                n_project_template.status = Status.Project.Approved;
                project_template_detail.child.Add(n_project_template);

                dbContext.project_template_detail.Add(n_project_template);

                projectproject_template_detailViewSource.View.Filter = null;

                filter_task();
            }
            else
            {
                //Adding First Parent.
                project_template_detail n_project_template = new project_template_detail();
                n_project_template.id_project_template = project_template.id_project_template;
                n_project_template.status = Status.Project.Approved;
                dbContext.project_template_detail.Add(n_project_template);

                projectproject_template_detailViewSource.View.Filter = null;

                filter_task();
            }
            projectproject_template_detailViewSource.View.MoveCurrentToLast();
        }

        private void btnEditTask_Click(object sender, EventArgs e)
        {
            stpcode.IsEnabled = true;
        }

        private void btnSaveTask_Click(object sender, EventArgs e)
        {

            try
            {

                dbContext.SaveChanges();
                toolBar.msgDone("Yay");
                stpcode.IsEnabled = false;

            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }

        }

        private void btnDeleteTask_Click(object sender, EventArgs e)
        {
            project_template_detail project_template_detail = treeProject.SelectedItem as project_template_detail;

            project_template_detail.status = Status.Project.Rejected;
            project_template_detail.IsSelected = false;
            projectproject_template_detailViewSource.View.Filter = null;

            filter_task();
            projectproject_template_detailViewSource.View.MoveCurrentToLast();
        }

        #endregion

        //private void projectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    filter_task();
        //}

        private void btnChild_MouseUp(object sender, MouseButtonEventArgs e)
        {
            stpcode.Visibility = Visibility.Visible;
        }

        private void cbxItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxItemType = (ComboBox)sender;
            if (itemSearchViewSource != null && cbxItemType.SelectedItem != null)
            {
                item.item_type Item_Type = (item.item_type)cbxItemType.SelectedItem;

                itemSearchViewSource.View.Filter = i =>
                {
                    item item = i as item;
                    //item.item_type j = (item.item_type)cbxItemType.SelectedValue;
                    if (item.id_item_type == Item_Type)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
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
                        if (project_template.name!=null)
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


      
      

       

        private void toolBar_btnApprove_Click(object sender,RoutedEventArgs e)
        {
            dbContext.Approve();
            project_templateViewSource.View.Refresh();
        }

        private void toolBar_btnAnull_Click(object sender, RoutedEventArgs e)
        {
            dbContext.Anull();
            project_templateViewSource.View.Refresh();
        }
        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                project_template_detail project_template_detail = (project_template_detail)treeProject.SelectedItem;
                project_template_detail.id_item = item.id_item;
                project_template_detail.item = item;

             
            }
        }
       

    }
}
