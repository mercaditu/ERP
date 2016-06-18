using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class currency : UserControl
    {
        dbContext entity = new dbContext();
        CollectionViewSource app_currencyViewSource, app_currencyapp_currencyfxViewSource;
        bool isLoadedFirstTime = true;
        private int Id;
        public int CurrencyId { get { return Id; } set { Id = value; } }

        public currency()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                app_currencyViewSource = ((CollectionViewSource)(this.FindResource("app_currencyViewSource")));
                entity.db.app_currency.Where(x => x.id_currency == CurrencyId).Load();

                app_currencyViewSource.Source = entity.db.app_currency.Local;
                app_currencyapp_currencyfxViewSource = this.FindResource("app_currencyapp_currencyfxViewSource") as CollectionViewSource;

                if (CurrencyId == 0)
                { AddNew(); }

                isLoadedFirstTime = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {

                    _SaveChanges();
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _SaveChanges()
        {
            app_currency _app_currency = app_currencyViewSource.View.CurrentItem as app_currency;

            if (_app_currency.is_priority == true)
            {
                List<app_currency> list_app_currency = entity.db.app_currency.Where(a => a.id_currency != _app_currency.id_currency && a.id_company == CurrentSession.Id_Company).ToList();
                foreach (var item in list_app_currency)
                {
                    item.is_priority = false;
                }
            }

            if(_app_currency.app_currencyfx.Any(x => x.is_active) == false && _app_currency.app_currencyfx.Count > 0)
            {
                app_currencyfx app_currencyfx;
                app_currencyfx = _app_currency.app_currencyfx.OrderBy(x => x.timestamp).FirstOrDefault();
                app_currencyfx.is_active = true;
                app_currencyfx.is_reverse = false;
            }

            entity.db.SaveChanges();
        }

        private void AddNew()
        {
            app_currency objCurrency = new app_currency();
            entity.db.app_currency.Add(objCurrency);
            app_currencyViewSource.View.MoveCurrentToLast();
            btnDelete.Visibility = Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Grid parentGrid = (Grid)Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cbxCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cbxCountry.SelectedItem != null)
            //{
            //    //geo_country _geo_country = cbxCountry.SelectedItem as geo_country;
            //    //tbxCurrency.Text = _geo_country.currency;
            //}
        }

        List<app_currencyfx> listapp_currencyfx = null;
        private void chkIsPriority_Checked(object sender, RoutedEventArgs e)
        {
            if (!isLoadedFirstTime)
            {
                app_currency _app_currency = app_currencyViewSource.View.CurrentItem as app_currency;
                if (chkIsPriority.IsChecked == true)
                {
                    listapp_currencyfx = new List<app_currencyfx>();
                    if (_app_currency.app_currencyfx.Count > 0)
                        listapp_currencyfx = _app_currency.app_currencyfx.ToList();

                    foreach (var item in listapp_currencyfx)
                    {
                        entity.db.app_currencyfx.Remove(item);
                    }

                    app_currencyfx _app_currencyfx = new app_currencyfx();
                    _app_currencyfx.is_active = true;
                    _app_currencyfx.buy_value = 1;
                    _app_currencyfx.sell_value = 1;
                    _app_currency.app_currencyfx.Add(_app_currencyfx);

                    dataCurrencyfx.IsEnabled = false;
                }
                else
                {
                    entity.db.app_currencyfx.Remove(_app_currency.app_currencyfx.First());
                    dataCurrencyfx.IsEnabled = true;
                }
                app_currencyViewSource.View.Refresh();
                app_currencyapp_currencyfxViewSource.View.Refresh();
            }
            else
            {
                if (chkIsPriority.IsChecked == true)
                    dataCurrencyfx.IsEnabled = false;
                else
                    dataCurrencyfx.IsEnabled = true;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrencyId > 0)
                {
                    MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        app_currency app_currency = entity.db.app_currency.Where(a => a.id_currency == CurrencyId).First();
                        app_currency.is_active = false;
                        btnSave_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void chkIsActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox objIsActive = sender as CheckBox;
                int isCheckCtr = 0;
                foreach (CheckBox checkBox in Class.clsCommon.FindVisualChildren<CheckBox>(dataCurrencyfx, "chkIsActive"))
                {
                    if (checkBox.IsChecked == true)
                        isCheckCtr++;
                }
                if (isCheckCtr > 1)
                {
                    foreach (CheckBox checkBox in Class.clsCommon.FindVisualChildren<CheckBox>(dataCurrencyfx, "chkIsActive"))
                    {
                        checkBox.IsChecked = false;
                    }
                    objIsActive.IsChecked = true;
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }
        private void chkIsDivisble_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox objIsActive = sender as CheckBox;
                int isCheckCtr = 0;
                foreach (CheckBox checkBox in Class.clsCommon.FindVisualChildren<CheckBox>(dataCurrencyfx, "chkIsDivisible"))
                {
                    if (checkBox.IsChecked == true)
                        isCheckCtr++;
                }
                if (isCheckCtr > 1)
                {
                    foreach (CheckBox checkBox in Class.clsCommon.FindVisualChildren<CheckBox>(dataCurrencyfx, "chkIsDivisible"))
                    {
                        checkBox.IsChecked = false;
                    }
                    objIsActive.IsChecked = true;
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }
    }
}
