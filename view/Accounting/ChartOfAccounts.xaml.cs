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
       // CollectionViewSource accounting_chartParentViewSource;

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
                                                     && a.id_company == CurrentSession.Id_Company).ToListAsync();

            AccountingChartDB.contacts.Where(t => t.is_active == true && t.id_company == CurrentSession.Id_Company).Load();

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
            app_vatViewSource.Source = AccountingChartDB.app_vat.Where(t => t.id_company == CurrentSession.Id_Company && t.is_active == true).ToList();

            CollectionViewSource app_cost_centerViewSource = FindResource("app_cost_centerViewSource") as CollectionViewSource;
            app_cost_centerViewSource.Source = AccountingChartDB.app_cost_center.Where(t => t.id_company == CurrentSession.Id_Company && t.is_active == true && t.is_administrative == true).ToList();

            CollectionViewSource itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
            itemViewSource.Source = AccountingChartDB.items.Where(t => t.is_active == true && t.id_company == CurrentSession.Id_Company).ToList();

            CollectionViewSource item_tagViewSource = FindResource("item_tagViewSource") as CollectionViewSource;
            item_tagViewSource.Source = AccountingChartDB.item_tag.Where(t => t.is_active == true && t.id_company == CurrentSession.Id_Company).ToList();

            CollectionViewSource item_asset_groupViewSource = FindResource("item_asset_groupViewSource") as CollectionViewSource;
            item_asset_groupViewSource.Source = AccountingChartDB.item_asset_group.ToList();

            cbxChartType.ItemsSource = Enum.GetValues(typeof(accounting_chart.ChartType));
            cbxChartSubType.ItemsSource = Enum.GetValues(typeof(accounting_chart.ChartSubType));

            accounting_chartViewSource = FindResource("accounting_chartViewSource") as CollectionViewSource;
           // accounting_chartParentViewSource = FindResource("accounting_chartParentViewSource") as CollectionViewSource;

            AccountingChartDB.accounting_chart.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
            accounting_chartViewSource.Source = AccountingChartDB.accounting_chart.Local;
            //accounting_chartParentViewSource.Source = AccountingChartDB.accounting_chart.Local;

            //filter_Parentchart();
            filter_chart();

            treeProject.SelectedItem_ = accounting_chartViewSource.View.CurrentItem as accounting_chart;
        }

        public void filter_chart()
        {
            if (accounting_chartViewSource != null)
            {
                if (accounting_chartViewSource.View != null)
                {
                    accounting_chartViewSource.View.Filter = i =>
                    {
                        accounting_chart accounting_chart = i as accounting_chart;
                        if (accounting_chart.parent == null)
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
        }
        #region toolBar Events

        private void toolBar_btnNew_Click(object sender)
        {
            accounting_chart accounting_chart = (accounting_chart)treeProject.SelectedItem_;

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
                treeProject.SelectedItem_ = n_accounting_chart;
              

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
                treeProject.SelectedItem_ = n_accounting_chart;
              
            }
          filter_chart();
           
            //filter_Parentchart();
          
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            accounting_chart accounting_chart = treeProject.SelectedItem_ as accounting_chart;
            accounting_chart.State = EntityState.Modified;
            accounting_chart.IsSelected = true;
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                accounting_chart accounting_chart = (accounting_chart)treeProject.SelectedItem_;
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
            filter_chart();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            accounting_chart accounting_chart = treeProject.SelectedItem_ as accounting_chart;

            if (accounting_chart != null)
            {
                accounting_chart.State = System.Data.Entity.EntityState.Unchanged;
                accounting_chart.chart_type = (accounting_chart.ChartType)cbxChartType.SelectedItem;
            }

            if (AccountingChartDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(AccountingChartDB.NumberOfRecords);
                filter_chart();
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            accounting_chart accounting_chart = treeProject.SelectedItem_ as accounting_chart;
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

            filter_chart();
            accounting_chartViewSource.View.Refresh();
            accounting_chartViewSource.View.MoveCurrentToLast();
        }

        #endregion

        private void rbtnCash_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rd = (RadioButton)sender;
            int chartsubtype = Convert.ToInt32(rd.Tag);
            if (rd.IsChecked == true)
            {
                accounting_chart accounting_chart = treeProject.SelectedItem_ as accounting_chart;
                accounting_chart.chartsub_type = (accounting_chart.ChartSubType)chartsubtype;
            }
        }

        private void rbtnRevenue_Loaded(object sender, RoutedEventArgs e)
        {
            accounting_chart accounting_chart = treeProject.SelectedItem_ as accounting_chart;
            if (accounting_chart != null)
            {
                RadioButton rd = (RadioButton)sender;
                int chartsubtype = Convert.ToInt32(accounting_chart.chartsub_type);
                if (Convert.ToInt32(rd.Tag) == chartsubtype)
                {
                    rd.IsChecked = true;
                }
            }
        }

        private void btnParaguayChart_Click(object sender, RoutedEventArgs e)
        {
            entity.Brillo.Seed_Data.ChartOfAccounts Charts = new entity.Brillo.Seed_Data.ChartOfAccounts();
            Charts.Paraguay((bool)chbxDelete.IsChecked);

            //accounting_chartViewSource = FindResource("accounting_chartViewSource") as CollectionViewSource;
            AccountingChartDB = new entity.AccountingChartDB();
            AccountingChartDB.accounting_chart.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
            accounting_chartViewSource.Source = AccountingChartDB.accounting_chart.Local;

            accounting_chartViewSource.View.Refresh();
            filter_chart();
        }
    }
}
