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
    public partial class bank : UserControl
    {
        CollectionViewSource _app_bankViewSource = null;
        public CollectionViewSource app_bankViewSource { get { return _app_bankViewSource; } set { _app_bankViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        public bank()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = app_bankViewSource;

                //CollectionViewSource geo_countryViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("geo_countryViewSource");
                //geo_countryViewSource.Source = _entity.db.geo_country.OrderBy(a => a.name).ToList();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    IEnumerable<app_account> app_account = app_bankapp_accountdatagrid.ItemsSource.OfType<app_account>();
                    foreach (var item in app_account)
                    {
                        item.id_account_type = entity.app_account.app_account_type.Bank;
                    }
                    _entity.db.SaveChanges();
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
                _entity.CancelChanges();
                app_bankViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.app_bank app_bank = app_bankViewSource.View.CurrentItem as entity.app_bank;
                app_bank.is_active = false;
                btnSave_Click(sender, e);
            }
        }
    }
}
