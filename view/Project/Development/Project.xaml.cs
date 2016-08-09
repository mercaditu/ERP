using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.Entity;
namespace Cognitivo.Project.Development
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class Project : Page
    {
        ProjectDB ProjectDB = new ProjectDB();
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        int company_ID;
        CollectionViewSource ProjectViewSource, Projectproject_tag_detail;
        public Project()
        {
            InitializeComponent();
            company_ID = _setting.company_ID;
        }

        private void btnNew_Click(object sender)
        {


            entity.project project =new  entity.project();
            project.IsSelected = true;

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

        private void btnSave_Click(object sender)
        {
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

        private void Project_Loaded(object sender, RoutedEventArgs e)
        {

            ProjectDB.projects.Where(a => a.id_company == company_ID
                                            && (a.is_head == true)).ToList();

            ProjectViewSource = ((CollectionViewSource)(FindResource("ProjectViewSource")));
            ProjectViewSource.Source = ProjectDB.projects.Local;


            ProjectDB.project_tag
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
                    //DeleteDetailGridRow


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
        void Add_Tag()
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
    }
}
