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
using System.Data.Entity.Validation;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for projecttemplate.xaml
    /// </summary>
    public partial class projecttemplate : UserControl
    {
        public bool isValid { get; set; }
        CollectionViewSource _projecttemplateViewSource = null;
        public CollectionViewSource projecttemplateViewSource { get { return _projecttemplateViewSource; } set { _projecttemplateViewSource = value; } }

        CollectionViewSource _projecttemplatedetailViewSource = null;
        public CollectionViewSource projecttemplatedetailViewSource { get { return _projecttemplatedetailViewSource; } set { _projecttemplatedetailViewSource = value; } }

        entity.Properties.Settings _settings = new entity.Properties.Settings();

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }


        private entity.project_template _projecttemplateobject = null;
        public entity.project_template projecttemplateobject { get { return _projecttemplateobject; } set { _projecttemplateobject = value; } }

        private ProjectTemplateDB entity = null;
        public ProjectTemplateDB _entity { get { return entity; } set { entity = value; } }

        public enum Mode
        {
            Add,
            Edit
        }

        public bool canedit { get; set; }
        public bool candelete { get; set; }
        public Mode EnterMode { get; set; }

        public projecttemplate()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (EnterMode == Mode.Edit)
            {
                if (canedit == false)
                {
                    stpDisplay.IsEnabled = false;
                    btnSave.IsEnabled = false;
                }
            }
            if (EnterMode == Mode.Add)
            {
                stpDisplay.IsEnabled = true;
                btnSave.IsEnabled = true;
            }

            if (candelete == false)
            {
                btnDelete.IsEnabled = false;
            }

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                entity.project project = new entity.project();

              
                if (operationMode == Class.clsCommon.Mode.Add)
                {
                    entity.project_template newItem = new entity.project_template();
                    newItem.IsSelected = true;
                   //  newItem.status = Status.Documents_General.Approved;
                    _entity.project_template.Add(newItem);
                    _projecttemplateViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    _projecttemplateViewSource.View.MoveCurrentTo(projecttemplateobject);
                    btnDelete.Visibility = System.Windows.Visibility.Visible;
                }
                stackMain.DataContext = _projecttemplateViewSource;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.SaveChanges();
                    //if (operationMode == Class.clsCommon.Mode.Add)
                    //{
                    //    project_template project_template = _projecttemplateViewSource.View.CurrentItem as project_template;
                    //    project_template_detail n_project_template = new project_template_detail();
                    //    n_project_template.id_project_template = project_template.id_project_template;
                    //    // n_project_template.name = "task";
                    //    entity.project_template_detail.Add(n_project_template);
                    //    entity.SaveChanges();
                    //}
                    
                       // projecttemplatedetailViewSource.View.Filter = null;

                        filter_task();
                        
                    
                 
                   // btnCancel_Click(sender, e);
                    Grid parentGrid = (Grid)Parent;
                    parentGrid.Children.Clear();
                    parentGrid.Visibility = Visibility.Hidden;
                    projecttemplateViewSource.View.Refresh();
                   // projecttemplatedetailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void filter_task()
        {
            if (projecttemplatedetailViewSource.View != null)
            {
                projecttemplatedetailViewSource.View.Filter = i =>
                {
                    project_template_detail project_template_detail = (project_template_detail)i;
                    if (project_template_detail.parent == null)
                        return true;
                    else
                        return false;
                };
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            entity.CancelAllChanges();
            _projecttemplateViewSource.View.Refresh();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                project_template _project = _projecttemplateViewSource.View.CurrentItem as project_template;
                //_project.is_active = false;
                btnSave_Click(sender, e);
            }
        }
    }
}
