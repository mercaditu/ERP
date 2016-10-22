using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace cntrl
{
    /// <summary>
    /// Interaction logic for contact_role.xaml
    /// </summary>
    public partial class hr_coefficient : UserControl
    {
        CollectionViewSource _objCollectionViewSource = null;
        public CollectionViewSource objCollectionViewSource { get { return _objCollectionViewSource; } set { _objCollectionViewSource = value; } }

        private dbContext entity = new dbContext();
      //  entity.Properties.Settings _entity = new entity.Properties.Settings();

        private entity.hr_time_coefficient _hr_time_coefficientobject = null;
        public entity.hr_time_coefficient hr_time_coefficientobject { get { return _hr_time_coefficientobject; } set { _hr_time_coefficientobject = value; } }

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }

        public hr_coefficient()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
               
                entity.db.hr_time_coefficient.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                objCollectionViewSource.Source = entity.db.hr_time_coefficient.Local;

                if (operationMode == Class.clsCommon.Mode.Add)
                {
                    hr_time_coefficient hr_time_coefficient = new hr_time_coefficient();
                    entity.db.hr_time_coefficient.Add(hr_time_coefficient);

                    objCollectionViewSource.View.Refresh();
                    objCollectionViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    objCollectionViewSource.View.MoveCurrentTo(entity.db.hr_time_coefficient.Where(x => x.id_time_coefficient == hr_time_coefficientobject.id_time_coefficient).FirstOrDefault());
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
    }
}
