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
        dbContext _entity = new dbContext();

        CollectionViewSource project_taskViewSource;
        CollectionViewSource projectViewSource;
        CollectionViewSource itemSearchViewSource;
        CollectionViewSource project_templateproject_template_detailViewSource;
        CollectionViewSource project_templateViewSource;
        CollectionViewSource project_task_dimensionViewSource;

        entity.Properties.Settings _Setting = new entity.Properties.Settings();
        public Boolean ViewAll { get; set; }
        public TaskView()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_task_dimensionViewSource = ((CollectionViewSource)(FindResource("project_task_dimensionViewSource")));
            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            project_templateproject_template_detailViewSource = ((CollectionViewSource)(FindResource("project_templateproject_template_detailViewSource")));
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            _entity.db.projects.Where(a => a.is_active == true && a.id_company == _Setting.company_ID).Include("project_task").Load();
            projectViewSource.Source = _entity.db.projects.Local;

            _entity.db.project_task_dimension.Where(a => a.id_company == _Setting.company_ID).Load();
            project_task_dimensionViewSource.Source = _entity.db.project_task_dimension.Local;

            project_templateViewSource = ((CollectionViewSource)(FindResource("project_templateViewSource")));
            await _entity.db.project_template.Where(a => a.is_active == true && a.id_company == _Setting.company_ID).LoadAsync();
            project_templateViewSource.Source = _entity.db.project_template.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            await _entity.db.app_dimension.Where(a => a.id_company == _Setting.company_ID).LoadAsync();
            app_dimensionViewSource.Source = _entity.db.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            await _entity.db.app_measurement.Where(a => a.id_company == _Setting.company_ID).LoadAsync();
            app_measurementViewSource.Source = _entity.db.app_measurement.Local;

            //Loading Products
            //_entity.db.items.Where(a => a.id_company == _Setting.company_ID).OrderBy(b => b.name).Load();

           
            //itemSearchViewSource = ((CollectionViewSource)(FindResource("itemSearchViewSource")));
            //itemSearchViewSource.Source = _entity.db.items.Local;
            cbxItemType.ItemsSource = Enum.GetValues(typeof(item.item_type));

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

        #region Toolbar Events
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
                _entity.SaveChanges();
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
        private void btnNewTask_Click(object sender, EventArgs e)
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

            project project = projectViewSource.View.CurrentItem as project;
            project_task project_task = treeProject.SelectedItem as project_task;

            if (project_task != null)
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
                        _entity.db.project_task.Add(n_project_task);
                        project_taskViewSource.View.Refresh();
                        project_taskViewSource.View.MoveCurrentTo(n_project_task);

                    }
                }
            }
            else
            {
                project_task n_project_task = new project_task();
                n_project_task.id_project = project.id_project;
                n_project_task.status = entity.Status.Project.Pending;
                _entity.db.project_task.Add(n_project_task);

                project_taskViewSource.View.Filter = null;
                project_taskViewSource.View.MoveCurrentTo(n_project_task);
                filter_task();
            }
            //   btnExpandAll.IsChecked = true;
        }

        private void btnEditTask_Click(object sender, EventArgs e)
        {
            stpcode.IsEnabled = true;
        }

        private void btnSaveTask_Click(object sender, EventArgs e)
        {
            try
            {

                IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    cbxItemType.ItemsSource = Enum.GetValues(typeof(item.item_type));

                    _entity.db.SaveChanges();
                    toolBar.msgDone("Yay!");
                    stpcode.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void btnDeleteTask_Click(object sender, EventArgs e)
        {
            toolBar_btnAnull_Click(sender);
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
               // itemSearchViewSource.Source = _entity.db.items.Where(x =>x.id_company==_Setting.company_ID && x.id_item_type == Item_Type).ToList();


                if (Item_Type == entity.item.item_type.Task)
                {
                    stpdate.Visibility = Visibility.Visible;
                    stpitem.Visibility = Visibility.Collapsed;
                }
                else
                {
                    stpdate.Visibility = Visibility.Collapsed;
                    stpitem.Visibility = Visibility.Visible;
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

        private void toolBar_btnApprove_Click(object sender)
        {
            project_taskViewSource.View.Filter = null;
            List<project_task> _project_task = treeProject.ItemsSource.Cast<project_task>().ToList();
            _project_task = _project_task.Where(x => x.IsSelected == true).ToList();

            foreach (project_task project_task in _project_task)
            {
                //if (project_task.Error == null)
                //{
                    if (project_task.status == Status.Project.Pending || project_task.status == null)
                    {
                        project_task.status = Status.Project.Approved;
                    }
                //}
                //else
                //{
                //    toolBar.msgWarning(project_task.name + "Error");
                //}

                project_task.IsSelected = false;
            }
            _entity.db.SaveChanges();
            filter_task();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            project_taskViewSource.View.Filter = null;
            List<project_task> project_taskLIST = treeProject.ItemsSource.Cast<project_task>().ToList();
            project_taskLIST = project_taskLIST.Where(x => x.IsSelected == true).ToList();
            foreach (project_task project_task in project_taskLIST)
            {
                //if (project_task.Error == null)
                //{
                project_task.status = Status.Project.Rejected;
                project_task.IsSelected = false;

                //}
            }
            _entity.db.SaveChanges();
            toolBar.msgDone("Yay!");
            filter_task();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();
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

            filter_task();
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == System.Windows.Visibility.Hidden)
            {
                projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
                _entity.db.Database.Initialize(true);
                projectViewSource.Source = _entity.db.projects.Where(a => a.is_active == true && a.id_company == _Setting.company_ID).ToList();
                projectViewSource.View.MoveCurrentToLast();
                filter_task();
            }
        }

        private void treeProject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeProject.SelectedItem != null)
            {
                project_task project_task = (project_task)treeProject.SelectedItem;
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
            toolBar_btnApprove_Click(sender);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            toolBar_btnAnull_Click(sender);
        }

        private void project_task_dimensionDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            project_task project_task = (project_task)treeProject.SelectedItem;
            project_task_dimension project_task_dimension = e.Row.Item as project_task_dimension;

            //project_task_dimension.id_project_task = project_task.id_project_task;
            project_task.project_task_dimension.Add(project_task_dimension);
        }
        private void item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item item = _entity.db.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                if (item.item_dimension != null)
                {
                    project_task project_task = (project_task)treeProject.SelectedItem;
                    project_task.items = item;
                    foreach (item_dimension _item_dimension in item.item_dimension)
                    {
                        project_task_dimension project_task_dimension = new project_task_dimension();
                        project_task_dimension.id_dimension = _item_dimension.id_app_dimension;
                        project_task_dimension.value = _item_dimension.value;
                        project_task_dimension.id_measurement = _item_dimension.id_measurement;
                        project_task_dimension.id_project_task = project_task.id_project_task;
                        project_task_dimension.project_task = project_task;
                        _entity.db.project_task_dimension.Add(project_task_dimension);
                        project_task_dimensionViewSource.View.MoveCurrentToLast();
                    }


                }
               

            }
        }
        //private void cbxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    item item = (item)cbxItem.Data;
        //    cbxItem.focusGrid = false;
        //    cbxItem.Text = ((item)cbxItem.Data).name;
        //    if (item.item_dimension != null)
        //    {
        //        project_task project_task = (project_task)treeProject.SelectedItem;
        //        project_task.items = item;
        //        foreach (item_dimension _item_dimension in item.item_dimension)
        //        {
        //            project_task_dimension project_task_dimension = new project_task_dimension();
        //            project_task_dimension.id_dimension = _item_dimension.id_app_dimension;
        //            project_task_dimension.value = _item_dimension.value;
        //            project_task_dimension.id_measurement = _item_dimension.id_measurement;
        //            project_task_dimension.id_project_task = project_task.id_project_task;
        //            project_task_dimension.project_task = project_task;
        //            _entity.db.project_task_dimension.Add(project_task_dimension);
        //            project_task_dimensionViewSource.View.MoveCurrentToLast();
        //        }


        //    }
        //}

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

        //private void cbxItem_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        item item = (item)cbxItem.Data;
        //        cbxItem.focusGrid = false;
        //        cbxItem.Text = ((item)cbxItem.Data).name;
        //        if (item.item_dimension != null)
        //        {
        //            project_task project_task = (project_task)treeProject.SelectedItem;
        //            project_task.items = item;
        //            foreach (item_dimension _item_dimension in item.item_dimension)
        //            {
        //                project_task_dimension project_task_dimension = new project_task_dimension();
        //                project_task_dimension.id_dimension = _item_dimension.id_app_dimension;
        //                project_task_dimension.value = _item_dimension.value;
        //                project_task_dimension.id_measurement = _item_dimension.id_measurement;
        //                project_task_dimension.id_project_task = project_task.id_project_task;
        //                project_task_dimension.project_task = project_task;
        //                _entity.db.project_task_dimension.Add(project_task_dimension);
        //                project_task_dimensionViewSource.View.MoveCurrentToLast();
        //            }


        //        }
        //    }
        //}

        private void toolBar_Mini_btnNew_Click(object sender)
        {

        }

        private void toolBar_Mini_btnEdit_Click(object sender)
        {

        }

        private void toolBar_Mini_btnSave_Click(object sender)
        {

        }

        private void toolBar_Mini_btnDelete_Click(object sender)
        {

        }

        private void toolBar_Mini_btnApprove_Click(object sender)
        {

        }

        private void toolBar_Mini_btnAnull_Click(object sender)
        {

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            project_taskViewSource.View.Filter = null;
            List<project_task> _project_task = treeProject.ItemsSource.Cast<project_task>().ToList();
            _project_task = _project_task.Where(x => x.IsSelected == true).ToList();

           
            foreach (project_task project_task in _project_task)
            {
                if (project_task.status ==Status.Project.Pending)
                {
                    if (project_task.ProjectStatus == Status.ProjectStatus.Pending || project_task.ProjectStatus == null)
                    {
                        project_task.ProjectStatus = Status.ProjectStatus.Approved;
                    }
                }
               

                project_task.IsSelected = false;
            }
            _entity.db.SaveChanges();
            filter_task();
        }

        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            project_taskViewSource.View.Filter = null;
            List<project_task> project_taskLIST = treeProject.ItemsSource.Cast<project_task>().ToList();
            project_taskLIST = project_taskLIST.Where(x => x.IsSelected == true).ToList();
            foreach (project_task project_task in project_taskLIST)
            {
                //if (project_task.Error == null)
                //{
                project_task.ProjectStatus = Status.ProjectStatus.Rejected;
                project_task.IsSelected = false;

                //}
            }
            _entity.db.SaveChanges();
            toolBar.msgDone("Yay!");
            filter_task();
        }

        

       
    }
}
