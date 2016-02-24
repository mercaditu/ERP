using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd
{


    /// <summary>
    /// Interaction logic for contact_role.xaml
    /// </summary>
    public partial class Geography : UserControl
    {
        CollectionViewSource _objCollectionViewSource = null;
        public CollectionViewSource objCollectionViewSource { get { return _objCollectionViewSource; } set { _objCollectionViewSource = value; } }
        CollectionViewSource app_geographyViewSource=null;
        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        public Geography()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                cbxType.ItemsSource = Enum.GetValues(typeof(app_geography.geo_types));
                app_geographyViewSource = (CollectionViewSource)FindResource("app_geographyViewSource");
                app_geographyViewSource.Source = entity.db.app_geography.Where(a => a.is_active == true).OrderBy(a => a.name).ToList();
                stackFields.DataContext = objCollectionViewSource;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.CancelChanges();
                objCollectionViewSource.View.Refresh();
                Grid parentGrid = (Grid)Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
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
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.contact_role contact_role = objCollectionViewSource.View.CurrentItem as entity.contact_role;
                contact_role.is_active = false;
                btnSave_Click(sender, e);
            }
        }

        private void cbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxType.SelectedValue!=null)
            {
                 if (app_geographyViewSource != null)
            {
                if (app_geographyViewSource.View != null)
                {
                    app_geographyViewSource.View.Filter = i =>
                    {
                        app_geography app_geography = (app_geography)i;
                        if (Convert.ToInt32(app_geography.type) == (int)cbxType.SelectedValue)
                            return true;
                        else
                            return false;
                    };
                }
            }
            }
        }
    }
}
