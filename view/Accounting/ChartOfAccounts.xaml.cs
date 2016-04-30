using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace Cognitivo.Accounting
{
    public partial class ChartOfAccounts : Page
    {
        AccountingChartDB AccountingChartDB = new AccountingChartDB();
        CollectionViewSource accounting_chartViewSource = null;
        CollectionViewSource accounting_chartParentViewSource;
        entity.Properties.Settings _settings = new entity.Properties.Settings();
        
        public ChartOfAccounts()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_accountBankViewSource = FindResource("app_accountBankViewSource") as CollectionViewSource;
            app_accountBankViewSource.Source =
                await AccountingChartDB.app_account.Where(a => a.id_account_type == app_account.app_account_type.Bank
                                                     && a.is_active == true
                                                     && a.id_company == _settings.company_ID).ToListAsync();

            AccountingChartDB.contacts.Where(t => t.is_active == true && t.id_company == _settings.company_ID).Load();

            CollectionViewSource contactSupViewSource = FindResource("contactSupViewSource") as CollectionViewSource;
            contactSupViewSource.Source = AccountingChartDB.contacts.Local;

            if (contactSupViewSource.View != null)
            {
                contactSupViewSource.View.Filter = i =>
                {
                    contact contact = (contact)i;
                    if (contact.is_supplier == true)
                        return true;
                    else
                        return false;
                };
            }

            CollectionViewSource contactCustViewSource = FindResource("contactCustViewSource") as CollectionViewSource;
            contactCustViewSource.Source = AccountingChartDB.contacts.Local;
            if (contactCustViewSource.View != null)
            {
                contactCustViewSource.View.Filter = i =>
                {
                    contact contact = (contact)i;
                    if (contact.is_customer == true)
                        return true;
                    else
                        return false;
                };
            }


            CollectionViewSource app_vatViewSource = FindResource("app_vatViewSource") as CollectionViewSource;
            app_vatViewSource.Source = AccountingChartDB.app_vat.Where(t => t.id_company == _settings.company_ID && t.is_active == true).ToList();

            CollectionViewSource app_cost_centerViewSource = FindResource("app_cost_centerViewSource") as CollectionViewSource;
            app_cost_centerViewSource.Source = AccountingChartDB.app_cost_center.Where(t => t.id_company == _settings.company_ID && t.is_active == true && t.is_administrative == true).ToList();

            CollectionViewSource itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
            itemViewSource.Source = AccountingChartDB.items.Where(t => t.is_active == true && t.id_company == _settings.company_ID).ToList();

            CollectionViewSource item_tagViewSource = FindResource("item_tagViewSource") as CollectionViewSource;
            item_tagViewSource.Source = AccountingChartDB.item_tag.Where(t => t.is_active == true && t.id_company == _settings.company_ID).ToList();

            CollectionViewSource item_asset_groupViewSource = FindResource("item_asset_groupViewSource") as CollectionViewSource;
            item_asset_groupViewSource.Source = AccountingChartDB.item_asset_group.ToList();

            cbxChartType.ItemsSource = Enum.GetValues(typeof(accounting_chart.ChartType));

            accounting_chartViewSource = FindResource("accounting_chartViewSource") as CollectionViewSource;
            accounting_chartParentViewSource = FindResource("accounting_chartParentViewSource") as CollectionViewSource;

            AccountingChartDB.accounting_chart.Where(a => a.is_active == true && a.id_company == _settings.company_ID).Load();
            accounting_chartViewSource.Source = AccountingChartDB.accounting_chart.Local;
            accounting_chartParentViewSource.Source = AccountingChartDB.accounting_chart.Local;

            filter_chart();
        }

        public void filter_chart()
        {
            if (accounting_chartViewSource.View.CurrentItem != null)
            {


                int id_chart = ((accounting_chart)accounting_chartViewSource.View.CurrentItem).id_chart;

                if (accounting_chartParentViewSource != null)
                {
                    if (accounting_chartParentViewSource.View != null)
                    {
                        accounting_chartParentViewSource.View.Filter = i =>
                        {
                            accounting_chart accounting_chart = i as accounting_chart;
                            if (accounting_chart.id_chart != id_chart)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        };
                    }


                }
                cbxParent.SelectedItem = ((accounting_chart)accounting_chartViewSource.View.CurrentItem).parent;
            }
           
        }
        #region toolBar Events

        private void toolBar_btnNew_Click(object sender)
        {
            accounting_chart accounting_chart = (accounting_chart)accounting_chartViewSource.View.CurrentItem; 
           
            if (accounting_chart != null)
            {

                //Adding a Child Item.
                accounting_chartViewSource.View.Filter = null;
                accounting_chart n_accounting_chart = new accounting_chart();

                n_accounting_chart.chart_type = accounting_chart.chart_type;
                n_accounting_chart.is_active = true;
                n_accounting_chart.IsSelected = true;
                n_accounting_chart.State = EntityState.Added;
                n_accounting_chart.chartsub_type = accounting_chart.chartsub_type;
                accounting_chart.child.Add(n_accounting_chart);
                AccountingChartDB.accounting_chart.Add(n_accounting_chart);
             
            }
            else
            {
                //Adding First Parent.
                accounting_chartViewSource.View.Filter = null;
                accounting_chart n_accounting_chart = new accounting_chart();
                n_accounting_chart.is_active = true;
                n_accounting_chart.IsSelected = true;
                n_accounting_chart.State = EntityState.Added;
                AccountingChartDB.accounting_chart.Add(n_accounting_chart);
            }

            accounting_chartViewSource.View.Refresh();
            accounting_chartViewSource.View.MoveCurrentToLast();
            filter_chart();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            accounting_chart accounting_chart = accounting_chartViewSource.View.CurrentItem as accounting_chart;
            accounting_chart.State = EntityState.Modified;
            accounting_chart.IsSelected = true;
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                accounting_chart accounting_chart = (accounting_chart)accounting_chartViewSource.View.CurrentItem;
                accounting_chart.is_active = false;
                toolBar_btnSave_Click(sender);
                accounting_chartViewSource.View.Filter = i =>
                {
                    entity.accounting_chart objaccounting_chart = i as accounting_chart;
                    if (objaccounting_chart.is_active == true)
                        return true;
                    else
                        return false;
                };
            }
          
            accounting_chartViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            accounting_chart accounting_chart = accounting_chartViewSource.View.CurrentItem as accounting_chart;
            accounting_chart.State = System.Data.Entity.EntityState.Unchanged;
            if (accounting_chart != null)
            { accounting_chart.chart_type = (accounting_chart.ChartType)cbxChartType.SelectedItem; }


            if (cbxParent.SelectedItem != null)
            { accounting_chart.parent = (accounting_chart)cbxParent.SelectedItem; }

            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = AccountingChartDB.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    AccountingChartDB.SaveChanges();
                    toolBar.msgSaved();
                    accounting_chartViewSource.View.MoveCurrentTo(accounting_chart);
                  
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            accounting_chart accounting_chart = accounting_chartViewSource.View.CurrentItem as accounting_chart;
            accounting_chart.State = EntityState.Unchanged;
            accounting_chartViewSource.View.MoveCurrentTo(accounting_chart);

            foreach (DbEntityEntry entry in AccountingChartDB.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            } 
            accounting_chartViewSource.View.Refresh();
            accounting_chartViewSource.View.MoveCurrentToLast();
            //filter_task();
          
        }
        #endregion

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    accounting_chartViewSource.View.Filter = i =>
                    {
                        accounting_chart accounting_chart = i as accounting_chart;
                        if (accounting_chart.name.ToLower().Contains(query.ToLower())
                            || accounting_chart.code.ToLower().Contains(query.ToLower())
                            )
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    accounting_chartViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void chartdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_chart();
        }

        private void btnClearParent_Click(object sender, RoutedEventArgs e)
        {
            accounting_chart accounting_chart = accounting_chartViewSource.View.CurrentItem as accounting_chart;
            accounting_chart.parent = null;
            cbxParent.SelectedItem = null;
        }

        private void rbtnCash_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rd = (RadioButton)sender;
            int chartsubtype=Convert.ToInt32(rd.Tag);
            if (rd.IsChecked==true)
            {
                accounting_chart accounting_chart = accounting_chartViewSource.View.CurrentItem as accounting_chart;
                accounting_chart.chartsub_type=(accounting_chart.ChartSubType)chartsubtype;
            }
        }

        private void rbtnRevenue_Loaded(object sender, RoutedEventArgs e)
        {
            accounting_chart accounting_chart = accounting_chartViewSource.View.CurrentItem as accounting_chart;
            if (accounting_chart!=null)
            {
                RadioButton rd = (RadioButton)sender;
                int chartsubtype = Convert.ToInt32(accounting_chart.chartsub_type);
                if (Convert.ToInt32(rd.Tag)==chartsubtype)
                {
                    rd.IsChecked = true;
                }
            }
         
           
        }

        private void btnParaguayChart_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            entity.Brillo.Seed_Data.ChartOfAccounts Charts = new entity.Brillo.Seed_Data.ChartOfAccounts();
            Charts.Paraguay();
        }
    }
}
