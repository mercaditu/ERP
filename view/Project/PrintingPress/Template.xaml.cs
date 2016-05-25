using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace Cognitivo.Project.PrintingPress
{
    /// <summary>
    /// Interaction logic for Template.xaml
    /// </summary>
    public partial class Template
    {
        db entity = new db();
        CollectionViewSource project_templateViewSource = null;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Template()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            try
            {
                project_templateViewSource = (CollectionViewSource)this.FindResource("project_templateViewSource");
                entity.project_template.Include("project_template_detail").Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
                project_templateViewSource.Source = entity.project_template.Local;

                CollectionViewSource itemViewSource = (CollectionViewSource)this.FindResource("itemViewSource");
                itemViewSource.Source = entity.items.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).ToList();

                CollectionViewSource item_tagViewSource = (CollectionViewSource)this.FindResource("item_tagViewSource");
                item_tagViewSource.Source = entity.item_tag.Where(a => a.id_company == _entity.company_ID && a.is_active == true).OrderBy(a => a.name).ToList();

                CollectionViewSource project_taskViewSource = (CollectionViewSource)this.FindResource("project_taskViewSource");
                project_taskViewSource.Source = entity.project_task.Where(a=>a.id_company == _entity.company_ID).OrderBy(a => a.item_description).ToList();

                List<Class.clsLogic> list_cls_logic = new List<Class.clsLogic>();
                Class.clsLogic objpaper = new Class.clsLogic();
                objpaper.NameProperty = "paper";
                list_cls_logic.Add(objpaper);
                Class.clsLogic objink = new Class.clsLogic();
                objink.NameProperty = "ink";
                list_cls_logic.Add(objink);
                Class.clsLogic objprinter = new Class.clsLogic();
                objprinter.NameProperty = "printer";
                list_cls_logic.Add(objprinter);
                //Class.clsLogic objcard = new Class.clsLogic();
                //objcard.NameProperty = "card";
                //list_cls_logic.Add(objcard);
                Class.clsLogic objcutting = new Class.clsLogic();
                objcutting.NameProperty = "cutting";
                list_cls_logic.Add(objcutting);
                Class.clsLogic objaccessories = new Class.clsLogic();
                objaccessories.NameProperty = "accessories";
                list_cls_logic.Add(objaccessories);
                Class.clsLogic Toner = new Class.clsLogic();
                Toner.NameProperty = "Toner";
                list_cls_logic.Add(Toner);
                Class.clsLogic objHall = new Class.clsLogic();
                objHall.NameProperty = "Hall";
                list_cls_logic.Add(objHall);
                Class.clsLogic objServicePerPerson = new Class.clsLogic();
                objServicePerPerson.NameProperty = "Per Person Service";
                list_cls_logic.Add(objServicePerPerson);
                Class.clsLogic objServicePerEvent = new Class.clsLogic();
                objServicePerEvent.NameProperty = "Per Event Service";
                list_cls_logic.Add(objServicePerEvent);
                
                id_item_logicComboBox.ItemsSource = list_cls_logic.OrderBy(x => x.NameProperty);

                List<Class.clsProjectTemplateType> list_project_template_type = new List<Class.clsProjectTemplateType>();
                Class.clsProjectTemplateType objTemplateType1 = new Class.clsProjectTemplateType();
                objTemplateType1.ProjectTemplateTypeId = "1";
                objTemplateType1.ProjectTemplateType = "Materials to Consume (Individual)";
                list_project_template_type.Add(objTemplateType1);

                Class.clsProjectTemplateType objTemplateType2 = new Class.clsProjectTemplateType();
                objTemplateType2.ProjectTemplateTypeId = "2";
                objTemplateType2.ProjectTemplateType = "Materials to Product (Direct)";
                list_project_template_type.Add(objTemplateType2);

                Class.clsProjectTemplateType objTemplateType3 = new Class.clsProjectTemplateType();
                objTemplateType3.ProjectTemplateTypeId = "3";
                objTemplateType3.ProjectTemplateType = "Product to Materials (Backward)";
                list_project_template_type.Add(objTemplateType3);
                cmbType.ItemsSource = list_project_template_type;
            }
            catch { }
        }

        #region Toolbar 
        private void ctrlToolBar_btnCancel_Click(object sender)
        {
            project_type_templateDataGrid.CancelEdit();
            project_templateViewSource.View.MoveCurrentToFirst();
            
            stackMain.IsEnabled = false;
            project_type_templateDataGrid.IsReadOnly = true;
        }

        private void ctrlToolBar_btnDelete_Click(object sender)
        {
             
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.project_template.Remove((project_template)project_templateDataGrid.SelectedItem);
                project_templateViewSource.View.MoveCurrentToFirst();
                ctrlToolBar_btnSave_Click(sender);
            }
        }

        private void ctrlToolBar_btnEdit_Click(object sender)
        {
            stackMain.IsEnabled = true;
            project_type_templateDataGrid.IsReadOnly = false;
        }

        private void ctrlToolBar_btnNew_Click(object sender)
        {
            stackMain.IsEnabled = true;
            project_type_templateDataGrid.IsReadOnly = false;
            project_template objProjectTemp = new project_template();
            entity.project_template.Add(objProjectTemp);
            project_templateViewSource.View.MoveCurrentToLast();
        }

        private void ctrlToolBar_btnSave_Click(object sender)
        {
            if (entity.SaveChanges() > 0)
            {
                stackMain.IsEnabled = false;
                project_type_templateDataGrid.IsReadOnly = true;
                ctrlToolBar.msgSaved(1);
            }
        }
        #endregion
    }
}
