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
    public partial class company : UserControl
    {
        CollectionViewSource _app_companyViewSource = null;
        public CollectionViewSource app_companyViewSource { get { return _app_companyViewSource; } set { _app_companyViewSource = value; } }

        private dbContext _entity = null;
        public dbContext objEntity { get { return _entity; } set { _entity = value; } }

        public enum Mode
        {
            Add,
            Edit
        }

        public bool canedit { get; set; }
        public bool candelete { get; set; }
        public Mode EnterMode { get; set; }

        public company()
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
                try
                {
                    stackMain.DataContext = app_companyViewSource;

                    //CollectionViewSource geo_countryViewSource = this.FindResource("geo_countryViewSource") as CollectionViewSource;
                    //geo_countryViewSource.Source = objEntity.db.geo_country.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = objEntity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    objEntity.SaveChanges();
                    entity.Properties.Settings.Default.company_ID = objEntity.db.app_company.FirstOrDefault().id_company;
                    entity.Properties.Settings.Default.company_Name = objEntity.db.app_company.FirstOrDefault().alias;
                    entity.Properties.Settings.Default.Save();
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
            try
            {
                objEntity.CancelChanges();
                app_companyViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
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
                    app_company _app_company = app_companyViewSource.View.CurrentItem as entity.app_company;
                    _app_company.is_active = false;
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
