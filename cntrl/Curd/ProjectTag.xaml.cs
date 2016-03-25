using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for ProjectTag.xaml
    /// </summary>
    public partial class ProjectTag : UserControl
    {
         entity.Properties.Settings _setting = new entity.Properties.Settings();

         CollectionViewSource _project_tagViewSource = null;
         public CollectionViewSource project_tagViewSource { get { return _project_tagViewSource; } set { _project_tagViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        public ProjectTag()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                try
                {
                    stackMain.DataContext = project_tagViewSource;
                    CollectionViewSource projectsViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("projectsViewSource");
                    projectsViewSource.Source = entity.db.projects.Where(a => a.is_active == true && a.id_company == _setting.company_ID).OrderBy(a => a.name).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.db.SaveChanges();
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                entity.CancelChanges();
                project_tagViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.project_tag project_tag = project_tagViewSource.View.CurrentItem as entity.project_tag;
                project_tag.is_active = false;
                btnSave_Click(sender, e);
            }
        }
    }
}
