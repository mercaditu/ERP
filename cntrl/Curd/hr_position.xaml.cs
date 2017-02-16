using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl
{
    /// <summary>
    /// Interaction logic for contact_role.xaml
    /// </summary>
    public partial class hr_position : UserControl
    {
        private CollectionViewSource _objCollectionViewSource = null;
        public CollectionViewSource objCollectionViewSource { get { return _objCollectionViewSource; } set { _objCollectionViewSource = value; } }

        public dbContext entity { get; set; }
        //  entity.Properties.Settings _entity = new entity.Properties.Settings();

        private entity.hr_position _hr_positionobj = null;
        public entity.hr_position hr_positionobject { get { return _hr_positionobj; } set { _hr_positionobj = value; } }

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }

        public hr_position()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CollectionViewSource app_departmentViewSource = (CollectionViewSource)FindResource("app_departmentViewSource");
                entity.db.app_department.Load();
                app_departmentViewSource.Source = entity.db.app_department.Local;

                entity.db.hr_position.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                objCollectionViewSource.Source = entity.db.hr_position.Local;

                if (operationMode == Class.clsCommon.Mode.Add)
                {
                    entity.hr_position hr_position = new entity.hr_position();
                    entity.db.hr_position.Add(hr_position);

                    objCollectionViewSource.View.Refresh();
                    objCollectionViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    objCollectionViewSource.View.MoveCurrentTo(entity.db.hr_position.Where(x => x.id_position == hr_positionobject.id_position).FirstOrDefault());
                }
                stackFields.DataContext = objCollectionViewSource;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.CancelChanges();
                objCollectionViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show(ex.InnerException.ToString());
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (res == MessageBoxResult.Yes)
            //{
            //    entity.hr_time_coefficient hr_time_coefficient = objCollectionViewSource.View.CurrentItem as entity.hr_time_coefficient;
            //    hr_time_coefficient.is_active = false;
            //    btnSave_Click(sender, e);
            //}
        }

        private void sbxContact_Select(object sender, RoutedEventArgs e)
        {
            entity.hr_position hr_position = objCollectionViewSource.View.CurrentItem as entity.hr_position;
            if (sbxContact.ContactID > 0)
            {
                if (hr_position != null)
                {
                    contact contact = entity.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    hr_position.id_contact = contact.id_contact;
                    hr_position.contact = contact;
                }
            }
        }
    }
}