using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Curd
{
    public partial class project : UserControl
    {
        private entity.project _project_crud;

        public entity.project project_crud
        {
            get { return _project_crud; }
            set { _project_crud = value; }
        }

        private CollectionViewSource projectViewSource, project_tagViewSource;

        // entity.Properties.Settings _settings = new entity.Properties.Settings();

        private db db = new db();

        public project()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CollectionViewSource branchViewSource = (CollectionViewSource)FindResource("branchViewSource");
                branchViewSource.Source = await db.app_branch.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).AsNoTracking().ToListAsync();

                CollectionViewSource currencyViewSource = (CollectionViewSource)FindResource("currencyViewSource");
                currencyViewSource.Source = await db.app_currency.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).AsNoTracking().ToListAsync();
                //CollectionViewSource contactViewSource = (CollectionViewSource)FindResource("contactViewSource");
                //contactViewSource.Source = await db.contacts.Where(a => a.is_active == true && a.id_company == _settings.company_ID && a.is_customer == true).OrderBy(b => b.name).ToListAsync();

                CollectionViewSource templateViewSource = (CollectionViewSource)FindResource("project_templateViewSource");
                templateViewSource.Source = await db.project_template.Where(x => x.is_active == true && x.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).AsNoTracking().ToListAsync();

                projectViewSource = (CollectionViewSource)FindResource("projectViewSource");
                await db.projects.Where(x => x.is_active == true && x.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
                projectViewSource.Source = db.projects.Local;

                project_tagViewSource = (CollectionViewSource)FindResource("project_tagViewSource");
                await db.project_tag.Where(x => x.is_active == true && x.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
                project_tagViewSource.Source = db.project_tag.Local;

                if (project_crud != null)
                {
                    // db.projects.Attach(project_crud);
                    projectViewSource.View.Refresh();
                    projectViewSource.View.MoveCurrentTo(db.projects.Where(x => x.id_project == project_crud.id_project).FirstOrDefault());
                }
                else
                {
                    entity.project _project = new entity.project();
                    _project.is_active = true;
                    db.projects.Add(_project);  //If project_crud has no value, then it must be for inserting.
                    projectViewSource.View.Refresh();
                    projectViewSource.View.MoveCurrentToLast();
                }

                stackMain.DataContext = projectViewSource;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            entity.project _project = ((entity.project)projectViewSource.View.CurrentItem);
            int id_type = 0;

            if (_project.id_project_template != null)
            {
                id_type = (int)_project.id_project_template;
            }

            await db.project_template_detail.Where(x => x.id_project_template == id_type).ToListAsync();
            List<project_template_detail> project_template_detail = db.project_template_detail.Local.ToList();

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
                        project_task _project_task = db.project_task.Local.Where(x => x.id_project_task == item.parent.id_template_detail).FirstOrDefault();
                        _project_task.child.Add(project_task);
                    }

                    _project.project_task.Add(project_task);
                    // }
                }
            }

            db.SaveChanges();

            string[] tagsArray = tbxTag.Text.Split(',');
            foreach (string strTag in tagsArray)
            {
                if (strTag!="")
                {
                    project_tag tag = db.project_tag.Where(x => x.name == strTag).FirstOrDefault();
                    if (tag == null)
                    {
                        tag = new project_tag();
                        tag.name = strTag;
                        db.project_tag.Add(tag);
                        db.SaveChanges();
                    }
                    project_tag_detail tag_detail = db.project_tag_detail.Where(x => x.id_tag == tag.id_tag && x.id_project == _project.id_project).FirstOrDefault();
                    if (tag_detail == null)
                    {
                        tag_detail = new project_tag_detail();
                        tag_detail.id_project = _project.id_project;
                        tag_detail.id_tag = tag.id_tag;
                        db.project_tag_detail.Add(tag_detail);
                        db.SaveChanges();
                    }
                }
               

              
            }

            btnCancel_Click(sender, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            entity.project _project = (entity.project)projectViewSource.View.CurrentItem;

            if (_project.id_project == 0)
            {
                db.projects.Remove(_project);
                db.SaveChanges();
                projectViewSource.Source = db.projects.Where(x => x.is_active == true && x.id_company == CurrentSession.Id_Company).ToList();
                projectViewSource.View.MoveCurrentToLast();
            }

            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Text<string>("Question_Delete"), "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                project _project = projectViewSource.View.CurrentItem as project;
                btnSave_Click(sender, e);
            }
        }

        private void contactComboBox_Select(object sender, RoutedEventArgs e)
        {
            if (contactComboBox.ContactID > 0)
            {
                entity.contact contact = db.contacts.Where(x => x.id_contact == contactComboBox.ContactID).FirstOrDefault();
                if (contact != null)
                {
                    entity.project _project = projectViewSource.View.CurrentItem as entity.project;
                    _project.id_contact = contact.id_contact;
                    _project.contact = contact;
                }
            }
        }

        //private void cbxTag_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        Add_Tag();
        //    }
        //}

        //private void cbxTag_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Add_Tag();
        //}

        //private void Add_Tag()
        //{
        //    // CollectionViewSource item_tagViewSource = ((CollectionViewSource)(FindResource("item_tagViewSource")));
        //    if (cbxTag.Data != null)
        //    {
        //        int id = Convert.ToInt32(((project_tag)cbxTag.Data).id_tag);
        //        if (id > 0)
        //        {
        //            entity.project project = projectViewSource.View.CurrentItem as entity.project;
        //            if (project != null)
        //            {
        //                project_tag_detail project_tag_detail = new project_tag_detail();
        //                project_tag_detail.id_tag = ((project_tag)cbxTag.Data).id_tag;
        //                project_tag_detail.project_tag = ((project_tag)cbxTag.Data);
        //                project.project_tag_detail.Add(project_tag_detail);
        //                CollectionViewSource Projectproject_tag_detail = FindResource("Projectproject_tag_detail") as CollectionViewSource;
        //                Projectproject_tag_detail.View.Refresh();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        entity.project project = projectViewSource.View.CurrentItem as entity.project;
        //        if (project != null)
        //        {
        //            project_tag project_tag = new project_tag();
        //            project_tag.name = cbxTag.Text;
        //            db.project_tag.Add(project_tag);
        //            db.SaveChanges();
        //            project_tag_detail project_tag_detail = new project_tag_detail();
        //            project_tag_detail.id_tag = project_tag.id_tag;
        //           project_tag_detail.project_tag = project_tag;
        //            project.project_tag_detail.Add(project_tag_detail);
        //            CollectionViewSource Projectproject_tag_detail = FindResource("Projectproject_tag_detail") as CollectionViewSource;
        //            Projectproject_tag_detail.View.Refresh();
        //        }
        //    }
        //}

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as project_tag_detail != null)
            {
                e.CanExecute = true;
            }
        }

        //private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    try
        //    {
        //        MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //        if (result == MessageBoxResult.Yes)
        //        {
        //            //DeleteDetailGridRow

        //            if (e.Parameter as project_tag_detail != null)
        //            {
        //                project_tag_detailDataGrid.CancelEdit();
        //                db.project_tag_detail.Remove(e.Parameter as project_tag_detail);

        //                CollectionViewSource Projectproject_tag_detail = FindResource("Projectproject_tag_detail") as CollectionViewSource;
        //                Projectproject_tag_detail.View.Refresh();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //throw;
        //    }
        //}
    }
}