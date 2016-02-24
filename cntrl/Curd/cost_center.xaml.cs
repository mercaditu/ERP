using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for cost_center.xaml
    /// </summary>
    public partial class cost_center : UserControl
    {
        CollectionViewSource _app_cost_centerViewSource = null;
        public CollectionViewSource app_cost_centerViewSource { get { return _app_cost_centerViewSource; } set { _app_cost_centerViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }
        
        public cost_center()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                stackMain.DataContext = app_cost_centerViewSource;
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
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            entity.CancelChanges();
            app_cost_centerViewSource.View.Refresh();
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    app_cost_center app_cost_center = app_cost_centerViewSource.View.CurrentItem as entity.app_cost_center;
                    app_cost_center.is_active = false;
                    btnSave_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }
}
