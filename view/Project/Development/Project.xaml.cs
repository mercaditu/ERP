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
        CollectionViewSource ProjectViewSource;
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
            ProjectDB.SaveChanges();
            ProjectViewSource.View.Refresh();
            toolBar.msgSaved();

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
    }
}
