using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Data;

namespace Cognitivo.Accounting
{
    public partial class Journal : Page, INotifyPropertyChanged
    {
        dbContext entity = new dbContext();
        public event PropertyChangedEventHandler PropertyChanged;
        CollectionViewSource accounting_cycleViewSource, accounting_journalViewSource, app_currencyfxViewSource, accounting_chartViewSource, accounting_templateViewSource, accounting_templateaccounting_template_detailViewSource;
        entity.Properties.Settings _settings = new entity.Properties.Settings();
        cntrl.Curd.Acocunting_Template_Entry Acocunting_Template_Entry;
        cntrl.Curd.AccountingJournal AccountingJournal;
        public DateTime AccountDate
        {
            get { return _AccountDate; }
            set
            {
                _AccountDate = value;
                RaisePropertyChanged("AccountDate");

                slider.Maximum = DateTime.DaysInMonth(_AccountDate.Year, _AccountDate.Month);
                slider.Value = AccountDate.Day;
                filter_date();

            }
        }
        DateTime _AccountDate = DateTime.Now;

        public Journal()
        {
            InitializeComponent();

        }

        private void slider_ValueChanged(object sender, EventArgs e)
        {
            AccountDate = AccountDate.AddDays(slider.Value - AccountDate.Day);
            filter_date();
        }

        private void RRMonth_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AccountDate = AccountDate.AddMonths(-1);
            filter_date();
        }

        private void RRDay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AccountDate = AccountDate.AddDays(-1);
            filter_date();
        }

        private void FFDay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AccountDate = AccountDate.AddDays(1);
            filter_date();
        }

        private void FFMonth_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AccountDate = AccountDate.AddMonths(1);
            filter_date();
        }

        private void Today_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AccountDate = DateTime.Now;
            filter_date();
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void JournalPage_Loaded(object sender, RoutedEventArgs e)
        {
            accounting_templateViewSource = ((CollectionViewSource)(FindResource("accounting_templateViewSource")));
            entity.db.accounting_template.Where(x=>x.id_company==_settings.company_ID).Load();
            accounting_templateViewSource.Source = entity.db.accounting_template.Local;
            accounting_templateaccounting_template_detailViewSource = ((CollectionViewSource)(FindResource("accounting_templateaccounting_template_detailViewSource")));

            accounting_journalViewSource = ((CollectionViewSource)(FindResource("accounting_journalViewSource")));
            entity.db.accounting_journal.Where(x => x.id_company == _settings.company_ID).Load();
            accounting_journalViewSource.Source = entity.db.accounting_journal.Local;

            //accounting_journalaccounting_journal_detailViewSource = ((CollectionViewSource)(FindResource("accounting_journalaccounting_journal_detailViewSource")));
            accounting_chartViewSource = ((CollectionViewSource)(FindResource("accounting_chartViewSource")));
            accounting_chartViewSource.Source = entity.db.accounting_chart.Where(x => x.id_company == _settings.company_ID).ToList();

            app_currencyfxViewSource = ((CollectionViewSource)(FindResource("app_currencyfxViewSource")));
            app_currencyfxViewSource.Source = entity.db.app_currencyfx.Where(x => x.id_company == _settings.company_ID).ToList();

            accounting_cycleViewSource = ((CollectionViewSource)(FindResource("accounting_cycleViewSource")));
            accounting_cycleViewSource.Source = entity.db.accounting_cycle.Where(x => x.id_company == _settings.company_ID).ToList();

           
            AccountDate = DateTime.Now;
        }

        public void filter_date()
        {
            int id = 0;
            if (accounting_cycleViewSource != null)
            {
                if (accounting_cycleViewSource.View != null)
                {
                    if (accounting_cycleViewSource.View.CurrentItem != null)
                    {
                        id = ((accounting_cycle)accounting_cycleViewSource.View.CurrentItem).id_cycle;
                    }

                }
            }
            if (accounting_journal_detailDataGrid != null)
            {
                accounting_journal_detailDataGrid.ItemsSource = entity.db.accounting_journal_detail.Where(x =>x.id_company==_settings.company_ID && x.trans_date <= AccountDate && x.accounting_journal.id_cycle == id).ToList();
            }
        }

        private void accounting_journal_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            accounting_journal_detail accounting_journal_detail = (accounting_journal_detail)e.Row.Item;
            crud_modal.Visibility = Visibility.Visible;
            AccountingJournal = new cntrl.Curd.AccountingJournal();
            AccountingJournal.accounting_journal = accounting_journal_detail.accounting_journal;
            AccountingJournal.db =entity.db;

            crud_modal.Children.Add(AccountingJournal);
        }

        private void accounting_journal_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            accounting_journal_detail accounting_journal_detail = (accounting_journal_detail)e.Row.Item;
            crud_modal.Visibility = Visibility.Visible;
            AccountingJournal = new cntrl.Curd.AccountingJournal();
            AccountingJournal.accounting_journal = accounting_journal_detail.accounting_journal;
            AccountingJournal.db = entity.db;
           // AccountingJournal.Save_Click += Savejouranl_Click;

            crud_modal.Children.Add(AccountingJournal);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            Acocunting_Template_Entry = new cntrl.Curd.Acocunting_Template_Entry();
            Acocunting_Template_Entry.accounting_journalViewSource = accounting_journalViewSource;
            Acocunting_Template_Entry.Save_Click += Save_Click;
           
            crud_modal.Children.Add(Acocunting_Template_Entry);

            //filter_date();
        }

        public void Save_Click(object sender)
        { 
             try
            {
                if (Acocunting_Template_Entry.accounting_journal.accounting_journal_detail.Sum(x => x.credit) != Acocunting_Template_Entry.accounting_journal.accounting_journal_detail.Sum(x => x.debit))
                {
                    MessageBox.Show("Verify balance :-" + Acocunting_Template_Entry.accounting_journal.code);
                }
                else
                {
                    entity.db.accounting_journal.Add(Acocunting_Template_Entry.accounting_journal);
                    IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        entity.db.SaveChanges();
                        crud_modal.Children.Clear();
                        crud_modal.Visibility = Visibility.Collapsed;
                    }
                }
            }
             catch (Exception ex)
             { throw ex; }
             filter_date();
        }
        public void Savejouranl_Click(object sender)
        {
            try
            {
                if (AccountingJournal.accounting_journal.accounting_journal_detail.Sum(x => x.credit) != AccountingJournal.accounting_journal.accounting_journal_detail.Sum(x => x.debit))
                {
                    MessageBox.Show("Verify balance :-" + AccountingJournal.accounting_journal.code);
                }
                else
                {
                    entity.db.accounting_journal.Add(Acocunting_Template_Entry.accounting_journal);
                    IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        entity.db.SaveChanges();
                        crud_modal.Children.Clear();
                        crud_modal.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            { throw ex; }
            filter_date();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.Accounting_template Acocunting_Template = new cntrl.Curd.Accounting_template();
            accounting_template accounting_template = new accounting_template();
            entity.db.accounting_template.Add(accounting_template);
            accounting_templateViewSource.View.Refresh();
            accounting_templateViewSource.View.MoveCurrentToLast();
            Acocunting_Template.accounting_templateViewSource = accounting_templateViewSource;
            Acocunting_Template.accounting_templatedetailViewSource = accounting_templateaccounting_template_detailViewSource;
            Acocunting_Template.entity = entity;
            crud_modal.Children.Add(Acocunting_Template);
          //  filter_date();
        }

        private void accounting_cycleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int id = 0;
            //if (accounting_cycleViewSource != null)
            //{
            //    if (accounting_cycleViewSource.View != null)
            //    {
            //        id = ((accounting_cycle)accounting_cycleViewSource.View.CurrentItem).id_cycle;
            //    }
            //}

            //if (accounting_journalViewSource != null)
            //{
            //    if (accounting_journalViewSource.View != null)
            //    {
            //        accounting_journalViewSource.View.Filter = i =>
            //        {
            //            accounting_journal objaccounting_journal = (accounting_journal)i;
            //            if (objaccounting_journal.id_cycle == id)
            //            { return true; }
            //            else
            //            { return false; }
            //        };
            //    }
            //}
            filter_date();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //TODO. CODE NEEDS TO REFRESH FOR EACH FISCAL YEAR
            accounting_journalViewSource.View.Filter = null;
            int cycle_id = ((accounting_cycle)accounting_cycleDataGrid.SelectedItem).id_cycle;
            List<accounting_journal> accounting_journal = accounting_journalViewSource.View.Cast<accounting_journal>().ToList();

            int code = 0;
            foreach (accounting_journal item in accounting_journal.Where(w => w.id_company == _settings.company_ID && w.id_cycle == cycle_id).OrderBy(x => x.trans_date))
            {
                code += 1;
                item.code = code;
            }

            entity.db.SaveChanges();
            accounting_journalViewSource.View.Refresh();
            filter_date();
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            accounting_journalViewSource.View.Refresh();
            filter_date();
        }

       

     

     


    }
}
