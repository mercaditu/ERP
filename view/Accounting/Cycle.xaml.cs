using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity.Validation;

namespace Cognitivo.Accounting
{
    public partial class Cycle : Page
    {
        entity.AccountingCycleDB dbContext = new entity.AccountingCycleDB();
        CollectionViewSource accounting_cycleViewSource = null;
        entity.Properties.Settings _settings = new entity.Properties.Settings();


    

        public Cycle()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource accounting_chartViewSource = this.FindResource("accounting_chartViewSource") as CollectionViewSource;
            accounting_chartViewSource.Source = dbContext.accounting_chart.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();

            accounting_cycleViewSource = (System.Windows.Data.CollectionViewSource)
                    this.FindResource("accounting_cycleViewSource");
            dbContext.accounting_cycle.Where(t => t.id_company == _settings.company_ID).Load();
            accounting_cycleViewSource.Source = dbContext.accounting_cycle.Local;
        }

        private void toolBar_btnNew_Click(object sender)
        {
          
            accounting_cycle accounting_cycle = new accounting_cycle();
            accounting_cycle.State = EntityState.Added;
            accounting_cycle.IsSelected = true;
            dbContext.Entry(accounting_cycle).State = EntityState.Added;
            accounting_cycleViewSource.View.Refresh();
            accounting_cycleViewSource.View.MoveCurrentToLast();
        
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (accounting_cycleDataGrid.SelectedItem != null)
            {
                accounting_cycle accounting_cycle_old = (accounting_cycle)accounting_cycleDataGrid.SelectedItem;
                accounting_cycle_old.IsSelected = true;
                accounting_cycle_old.State = EntityState.Modified;
                dbContext.Entry(accounting_cycle_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    accounting_cycle accounting_cycle = (accounting_cycle)accounting_cycleDataGrid.SelectedItem;
                    accounting_cycle.is_head = false;
                    accounting_cycle.State = EntityState.Deleted;
                    accounting_cycle.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() == 1)
            {
                accounting_cycleViewSource.View.Refresh();
                toolBar.msgSaved(dbContext.NumberOfRecords);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //using (db db = new db())
            //{
            //    // Take values in Calculated field for previous (currently is active period) and insert those values into new accounting period.
            //    // take currency period, and mark as is_active.
            //}
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            if (accounting_cycleDataGrid.SelectedItem != null)
            {
                accounting_cycle accounting_cycle_old = (accounting_cycle)accounting_cycleDataGrid.SelectedItem;
                accounting_cycle_old.IsSelected = true;
                accounting_cycle_old.State = EntityState.Modified;
                accounting_cycle_old.is_active = false;
                dbContext.Entry(accounting_cycle_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
            // add logic not to allow previously closed accounts from CLOSING again.
            //using (db db = new db())
            //{
            //    // take values in calculatd field for current (currently is acitve period) and insert those values into opposite credit/debit values of current
            //    //account period.
            //}
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            //using (entity.db db = new entity.db())
            //{
            //#TODO > create select that summarizes values of the accounts with movements for that accounting period.
            //CollectionViewSource accounting_transactionViewSource = (System.Windows.Data.CollectionViewSource)
            //    this.FindResource("accounting_transactionViewSource");
            //accounting_transactionViewSource.Source = db.accounting_journal.AsNoTracking().ToList();
            //}



            //accounting_cycle accounting_cycle = accounting_cycleViewSource.View.CurrentItem as accounting_cycle;
            //CurrentBalanceDataGrid.ItemsSource =
            //    _entity.db.accounting_journal.Where(a => a.id_company == _settings.company_ID && a.id_cycle == accounting_cycle.id_cycle)
            //    .GroupBy(g => new { g.accounting_journal_detail })
            //    .Select(s => new
            //    {
            //        Account = s.Key.accounting_journal_detail.FirstOrDefault().accounting_chart.name,
            //        Amount = s.Key.accounting_journal_detail.Sum(x => x.credit) - s.Key.accounting_journal_detail.Sum(x => x.debit)
            //    }).ToList();
        }

        private void btnBudget_Click(object sender, RoutedEventArgs e)
        {
            //using (entity.db db = new entity.db())
            //{
            //    //#TODO > list all avialable accounts for this company. allow user to add debit or credit values for each item, 
            //    //and update accounting_budget table under this accounting period.

            //    CollectionViewSource accounting_budgetViewSource = (System.Windows.Data.CollectionViewSource)
            //        this.FindResource("accounting_budgetViewSource");
            //    accounting_budgetViewSource.Source = db.accounting_journal.ToList();
            //}
        }


    }
}
