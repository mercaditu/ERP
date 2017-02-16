using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for department.xaml
    /// </summary>
    public partial class department : UserControl
    {
        private CollectionViewSource _app_departmentViewSource = null;
        public CollectionViewSource app_departmentViewSource { get { return _app_departmentViewSource; } set { _app_departmentViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        public department()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = app_departmentViewSource;
            }
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _entity.CancelChanges();
                app_departmentViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    _entity.db.SaveChanges();
                    btnCancel_Click(sender, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.app_department app_department = app_departmentViewSource.View.CurrentItem as entity.app_department;
                app_department.is_active = false;
                btnSave_Click(sender, e);
            }
        }
    }
}