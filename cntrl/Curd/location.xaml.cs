using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class location : UserControl
    {
        //public db db { get; set; }
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        CollectionViewSource _app_locationViewSource = null;
        public CollectionViewSource app_locationViewSource { get { return _app_locationViewSource; } set { _app_locationViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        public location()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = app_locationViewSource;

                CollectionViewSource branchViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_branchViewSource");
                branchViewSource.Source = _entity.db.app_branch.Where(a => a.is_active == true && a.id_company == _setting.company_ID).OrderBy(a => a.name).ToList();
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
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    app_location app_location = app_locationViewSource.View.CurrentItem as entity.app_location;
                    app_location.is_active = false;
                    btnSave_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _entity.CancelChanges();
                app_locationViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
