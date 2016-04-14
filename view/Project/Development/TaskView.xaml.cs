using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.ComponentModel;

namespace Cognitivo.Project.Development
{
    public partial class TaskView : Page, INotifyPropertyChanged
    {
        ProjectTaskDB ProjectTaskDB = new ProjectTaskDB();

        CollectionViewSource project_taskViewSource;
        CollectionViewSource projectViewSource;
        //CollectionViewSource project_templateproject_template_detailViewSource;
        //CollectionViewSource project_templateViewSource;
        CollectionViewSource project_task_dimensionViewSource;
        cntrl.PanelAdv.Project_TaskApprove Project_TaskApprove;
        public Boolean ViewAll { get; set; }

        public TaskView()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_task_dimensionViewSource = ((CollectionViewSource)(FindResource("project_task_dimensionViewSource")));

            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            //project_templateproject_template_detailViewSource = ((CollectionViewSource)(FindResource("project_templateproject_template_detailViewSource")));
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));

            ProjectTaskDB.projects.Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company).Include(x => x.project_task).Load();
            projectViewSource.Source = ProjectTaskDB.projects.Local;

            ProjectTaskDB.project_task_dimension.Where(a => a.id_company == entity.CurrentSession.Id_Company).Load();
            project_task_dimensionViewSource.Source = ProjectTaskDB.project_task_dimension.Local;

            //project_templateViewSource = ((CollectionViewSource)(FindResource("project_templateViewSource")));
            //await ProjectTaskDB.project_template.Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company).LoadAsync();
            //project_templateViewSource.Source = ProjectTaskDB.project_template.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            await ProjectTaskDB.app_dimension.Where(a => a.id_company == entity.CurrentSession.Id_Company).LoadAsync();
            app_dimensionViewSource.Source = ProjectTaskDB.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            await ProjectTaskDB.app_measurement.Where(a => a.id_company == entity.CurrentSession.Id_Company).LoadAsync();
            app_measurementViewSource.Source = ProjectTaskDB.app_measurement.Local;

            cbxItemType.ItemsSource = Enum.GetValues(typeof(item.item_type));

            //Filter to remove all items that are not top level.
            filter_task();
            
            entity.Brillo.Security security = new entity.Brillo.Security(entity.App.Names.ActivityPlan);
            if (security.approve)
            {
                btnapprove.IsEnabled = true;
            }
            else
            {
                btnapprove.IsEnabled = false;
            }

            if (security.annul)
            {
                btnanull.IsEnabled = true;
            }
            else
            {
                btnanull.IsEnabled = false;
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
            catch  { }
        }

        #region Toolbar Events
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

            filter_task();
        }
        private void toolBar_btnApprove_Click(object sender)
        {

            if (project_taskViewSource.View != null)
            {
                project_taskViewSource.View.Filter = null;
                List<project_task> _project_task = treeProject.ItemsSource.Cast<project_task>().ToList();
                _project_task = _project_task.Where(x => x.IsSelected == true).ToList();

                foreach (project_task project_task in _project_task)
                {
                    if (Project_TaskApprove.id_range!=null)
                    {
                        project_task.id_range = Project_TaskApprove.id_range;
                       
                    }
                    project_task.number = Project_TaskApprove.number;
                    if (project_task.status == Status.Project.Management_Approved)
                    {
                        if (project_task.status == Status.Project.Management_Approved || project_task.status == null)
                        {
                            project_task.status = Status.Project.Approved;
                        }
                    }

                    project_task.IsSelected = false;
                }
                ProjectTaskDB.SaveChanges();
                entity.Brillo.Document.Start.Automatic(_project_task.FirstOrDefault().project, _project_task.FirstOrDefault().app_document_range);
                filter_task();
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            if (project_taskViewSource.View != null)
            {
                project_taskViewSource.View.Filter = null;
                List<project_task> project_taskLIST = treeProject.ItemsSource.Cast<project_task>().ToList();
                project_taskLIST = project_taskLIST.Where(x => x.IsSelected == true).ToList();
                foreach (project_task project_task in project_taskLIST)
                {
                    project_task.status = Status.Project.Rejected;
                    project_task.IsSelected = false;
                }
                ProjectTaskDB.SaveChanges();
                toolBar.msgDone();
                filter_task();
            }
        }
        private void toolBar_btnNew_Click(object sender)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.project project = new cntrl.Curd.project();
            crud_modal.Children.Add(project);

        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                project projects = projectViewSource.View.CurrentItem as project;
                projects.is_active = false;
                ProjectTaskDB.SaveChanges();
                projectViewSource.View.Filter = i =>
                {
                    project objProject = (project)i;
                    if (objProject.is_active == true)
                        return true;
                    else
                        return false;
                };
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.project project = new cntrl.Curd.project();
            project.project_crud = (project)projectViewSource.View.CurrentItem;
            crud_modal.Children.Add(project);
        }
        #endregion

        #region Project Task Events

        private void btnNewTask_Click(object sender)
        {
            stpcode.IsEnabled = true;

            project project = projectViewSource.View.CurrentItem as project;
            project_task project_task = treeProject.SelectedItem_ as project_task;

            if (project_task != null && project_task.items.item_recepie.Count()==0)
            {
                //Adding a Child Item.
                if (project_task.items != null)
                {
                    if (project_task.items.id_item_type == entity.item.item_type.Task)
                    {
                        project_task n_project_task = new project_task();
                        n_project_task.id_project = project.id_project;
                        n_project_task.status = entity.Status.Project.Pending;
                        n_project_task.quantity_est = 0;

                        project_task.child.Add(n_project_task);
                        ProjectTaskDB.project_task.Add(n_project_task);
                        project_taskViewSource.View.Refresh();
                        project_taskViewSource.View.MoveCurrentTo(n_project_task);

                        treeProject.SelectedItem_ = n_project_task;
                    }
                }
            }
            else
            {
                project_task n_project_task = new project_task();
                n_project_task.id_project = project.id_project;
                n_project_task.status = entity.Status.Project.Pending;
                ProjectTaskDB.project_task.Add(n_project_task);

                project_taskViewSource.View.Filter = null;
                project_taskViewSource.View.MoveCurrentTo(n_project_task);
                treeProject.SelectedItem_ = n_project_task;
                filter_task();
            }
           
        }

        private void btnAddParentTask_Click(object sender)
        {
            stpcode.IsEnabled = true;

            project project = projectViewSource.View.CurrentItem as project;
            project_task n_project_task = new project_task();
            n_project_task.id_project = project.id_project;
            n_project_task.status = entity.Status.Project.Pending;
            ProjectTaskDB.project_task.Add(n_project_task);

            project_taskViewSource.View.Filter = null;
            project_taskViewSource.View.MoveCurrentTo(n_project_task);
            treeProject.SelectedItem_ = n_project_task;
            filter_task();
        }

        private void btnEditTask_Click(object sender)
        {
            stpcode.IsEnabled = true;
            project_task project_task = treeProject.SelectedItem_ as project_task;
            project_task.State = EntityState.Modified;
        }

        private void btnSaveTask_Click(object sender)
        {
            ProjectTaskDB.SaveChanges();
            toolBar.msgSaved();
            stpcode.IsEnabled = false;
        }

        private void btnDeleteTask_Click(object sender)
        {
            toolBar_btnAnull_Click(sender);
        }

        private void btnApprove_Click(object sender)
        {
            if (project_taskViewSource.View != null)
            {
                project_taskViewSource.View.Filter = null;
                List<project_task> _project_task = treeProject.ItemsSource.Cast<project_task>().ToList();
                _project_task = _project_task.Where(x => x.IsSelected == true).ToList();


                foreach (project_task project_task in _project_task)
                {
                    if (project_task.status == Status.Project.Pending || project_task.status == null)
                    {
                        project_task.status = Status.Project.Management_Approved;
                    }

                    project_task.IsSelected = false;
                }

                ProjectTaskDB.SaveChanges();
                filter_task();
            }
        }

        private void btnAnull_Click(object sender)
        {
            if (project_taskViewSource.View != null)
            {
                project_taskViewSource.View.Filter = null;
                List<project_task> project_taskLIST = treeProject.ItemsSource.Cast<project_task>().ToList();
                project_taskLIST = project_taskLIST.Where(x => x.IsSelected == true).ToList();

                foreach (project_task project_task in project_taskLIST)
                {
                    project_task.status = Status.Project.Rejected;
                    project_task.IsSelected = false;
                }

                ProjectTaskDB.SaveChanges();
                toolBar.msgDone();
                filter_task();
            }
        }

        #endregion

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
               // itemSearchViewSource.Source = _entity.items.Where(x =>x.id_company==_Setting.company_ID && x.id_item_type == Item_Type).ToList();


                if (Item_Type == item.item_type.Task)
                {
                    stpdate.Visibility = Visibility.Visible;
                    stpdate.IsEnabled = true;
                    stpitem.Visibility = Visibility.Collapsed;
                    stpitem.IsEnabled = false;
                }
                else
                {
                    stpdate.Visibility = Visibility.Collapsed;
                    stpdate.IsEnabled = false;
                    stpitem.Visibility = Visibility.Visible;
                    stpitem.IsEnabled = true;
                }

                if (Item_Type == entity.item.item_type.Service)
                {
                    project_task_dimensionDataGrid.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    project_task_dimensionDataGrid.Visibility = System.Windows.Visibility.Visible;
                }

            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();
        }
        
        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == System.Windows.Visibility.Hidden)
            {
                projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
                ProjectTaskDB.Database.Initialize(true);
                projectViewSource.Source = ProjectTaskDB.projects.Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company).ToList();
                projectViewSource.View.MoveCurrentToLast();
                filter_task();
            }
        }

        private void treeProject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeProject.SelectedItem_ != null)
            {
                project_task project_task = (project_task)treeProject.SelectedItem_;
                project_taskViewSource.View.MoveCurrentTo(project_task);
                try
                {
                    if (project_task_dimensionViewSource != null)
                    {
                        if (project_task_dimensionViewSource.View != null)
                        {
                            project_task_dimensionViewSource.View.Filter = i =>
                            {
                                project_task_dimension _project_task_dimension = (project_task_dimension)i;
                                if (_project_task_dimension.id_project_task == project_task.id_project_task)
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            Project_TaskApprove = new cntrl.PanelAdv.Project_TaskApprove();
            Project_TaskApprove.Save_Click += toolBar_btnApprove_Click;
            crud_modal.Children.Add(Project_TaskApprove);
         
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            toolBar_btnAnull_Click(sender);
        }

        private void project_task_dimensionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            project_task project_task = (project_task)treeProject.SelectedItem_;
            project_task_dimension project_task_dimension = e.Row.Item as project_task_dimension;

            project_task.project_task_dimension.Add(project_task_dimension);
        }

        private void item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item item = ProjectTaskDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

              
                project_task project_task_output = treeProject.SelectedItem_ as project_task;
                if (project_task_output.parent!=null)
                {
                    if (project_task_output.parent.items.is_autorecepie)
                    {
                        MessageBox.Show("can't add becuse item is auto receipe");
                    }
                }
               
                if (item != null && item.id_item > 0 && item.is_autorecepie )
                {
                    project_task_output.id_item = item.id_item;
                    project_task_output.items = item;
                    project_task_output.RaisePropertyChanged("item");
                    project_task_output.quantity_est = 1;
                    foreach (item_recepie_detail item_recepie_detail in item.item_recepie.FirstOrDefault().item_recepie_detail)
                    {
                        project_task project_task = new project_task();

                        project_task.code = item_recepie_detail.item.name;
                        project_task.item_description = item_recepie_detail.item.name;
                        project_task.id_item = item_recepie_detail.item.id_item;
                        project_task.items = item_recepie_detail.item;
                        project_task.id_project = project_task_output.id_project;
                        project_task.status = Status.Project.Pending;
                        project_task.RaisePropertyChanged("item");
                        if (item_recepie_detail.quantity > 0)
                        {
                            project_task.quantity_est = (decimal)item_recepie_detail.quantity;
                        }
                        project_task_output.child.Add(project_task);
                    }
                    filter_task();
                }
                else
                {
                    project_task_output.id_item = item.id_item;
                    project_task_output.items = item;
                    project_task_output.RaisePropertyChanged("item");
                    project_task_output.quantity_est = 1;
                }

                if (item.item_dimension != null)
                {
                    project_task_output.items = item;
                    foreach (item_dimension _item_dimension in item.item_dimension)
                    {
                        project_task_dimension project_task_dimension = new project_task_dimension();
                        project_task_dimension.id_dimension = _item_dimension.id_app_dimension;
                        project_task_dimension.value = _item_dimension.value;
                        project_task_dimension.id_measurement = _item_dimension.id_measurement;
                        project_task_dimension.id_project_task = project_task_output.id_project_task;
                        project_task_dimension.project_task = project_task_output;
                        ProjectTaskDB.project_task_dimension.Add(project_task_dimension);
                        project_task_dimensionViewSource.View.MoveCurrentToLast();
                    }
                }
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
    }
}
