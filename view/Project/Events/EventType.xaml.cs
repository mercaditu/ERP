using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.Data.Entity;

namespace Cognitivo.Project
{
    public partial class EventType : Page
    {
        entity.EventManagement.TemplateDB ProjectTemplateDB = new entity.EventManagement.TemplateDB();
        CollectionViewSource template_designerViewSource, template_designerservices_per_eventViewSource, template_designerproject_event_template_variableViewSource;

        public EventType()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _settings = new entity.Properties.Settings();

            template_designerViewSource = FindResource("template_designerViewSource") as CollectionViewSource;
            ProjectTemplateDB.project_event_template.Where(a => a.is_active == true && a.id_company == _settings.company_ID).Load();
            template_designerViewSource.Source = ProjectTemplateDB.project_event_template.Local;
            template_designerservices_per_eventViewSource = FindResource("template_designerservices_per_eventViewSource") as CollectionViewSource;
            template_designerproject_event_template_variableViewSource = FindResource("template_designerproject_event_template_variableViewSource") as CollectionViewSource;

            List<item_tag> item_tag = new List<item_tag>();
            item_tag = ProjectTemplateDB.item_tag.Where(a => a.id_company == _settings.company_ID && a.is_active == true).ToList();
            CollectionViewSource item_tagViewSource = FindResource("item_tagViewSource") as CollectionViewSource;
            item_tagViewSource.Source = item_tag.OrderBy(x=>x.name);
            CollectionViewSource item_tagViewSourceForEvents = FindResource("item_tagViewSourceForEvents") as CollectionViewSource;
            item_tagViewSourceForEvents.Source = item_tag.OrderBy(x => x.name);
            CollectionViewSource item_tagViewSourceForPerson = FindResource("item_tagViewSourceForPerson") as CollectionViewSource;
            item_tagViewSourceForPerson.Source = item_tag.OrderBy(x => x.name);
        }

        #region ToolBarEvents
        private void toolBar_btnNew_Click(object sender)
        {
            project_event_template project_event_template = new project_event_template();
            project_event_template.IsSelected = true;
            project_event_template.State = EntityState.Added;

            ProjectTemplateDB.project_event_template.Add(project_event_template);
            template_designerViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            project_event_template_variableDataGrid.CancelEdit();
            services_per_eventDataGrid.CancelEdit();

            template_designerViewSource.View.MoveCurrentToFirst();
            ProjectTemplateDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                project_event_template project_event_template = template_designerViewSource.View.CurrentItem as project_event_template;
                project_event_template.is_active = false;
                toolBar_btnSave_Click(sender);
                template_designerViewSource.View.Refresh();
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            project_event_template project_event_template = template_designerDataGrid.SelectedValue as project_event_template;
            project_event_template.IsSelected = true;
            project_event_template.State = EntityState.Modified;
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (ProjectTemplateDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ProjectTemplateDB.NumberOfRecords);
            }
        }
        #endregion

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as project_event_template_variable != null)
            {
                e.CanExecute = true;
            }
            else if (e.Parameter as project_event_template_fixed != null)
            {
                e.CanExecute = true;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (e.Parameter as project_event_template_variable != null)
                {
                    project_event_template_variableDataGrid.CancelEdit();
                    ProjectTemplateDB.project_event_template_variable.Remove(e.Parameter as project_event_template_variable);
                    template_designerproject_event_template_variableViewSource.View.Refresh();
                }
                else if (e.Parameter as project_event_template_fixed != null)
                {
                    services_per_eventDataGrid.CancelEdit();
                    ProjectTemplateDB.project_event_template_fixed.Remove(e.Parameter as project_event_template_fixed);
                    template_designerservices_per_eventViewSource.View.Refresh();
                }
            }
        }
    }
}
