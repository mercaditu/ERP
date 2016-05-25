using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Collections.Generic;
using System;
using System.Windows.Input;

namespace Cognitivo.Accounting
{

    public partial class IncomeJournal : Page
    {
        CollectionViewSource accounting_journalViewSource;
        AccountingJournalDB AccountingJournalDB = new AccountingJournalDB();
        SalesInvoiceDB SalesInvoiceDB = new SalesInvoiceDB();
        List<accounting_journal> Accounting_journalList = new List<accounting_journal>();

        public IncomeJournal()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _setting = new entity.Properties.Settings();

            accounting_journalViewSource = (CollectionViewSource)FindResource("accounting_journalViewSource");
            accounting_journalViewSource.Source = Accounting_journalList;

            List<sales_invoice> sales_invoiceList = AccountingJournalDB.sales_invoice.Where(i => i.accounting_journal == null && i.id_company == _setting.company_ID
                                                                                    && i.status == Status.Documents_General.Approved
                                                                                    && i.is_head == true)
                                                                                    .ToList();

            foreach (sales_invoice sales_invoice in sales_invoiceList)
            {
                entity.Brillo.Accounting.Income_Calc AccountingLogic = new entity.Brillo.Accounting.Income_Calc();
                accounting_journal accounting_journal = AccountingLogic.Start(AccountingJournalDB, sales_invoice);
                
                accounting_journal.IsSelected = false;
                Accounting_journalList.Add(accounting_journal);
                
                accounting_journalViewSource.View.Refresh();
                accounting_journalViewSource.View.MoveCurrentToLast();
            }

            //CollectionViewSource accounting_cycleViewSource = (CollectionViewSource)FindResource("accounting_cycleViewSource");
            //accounting_cycleViewSource.Source = Accounting_journalList.GroupBy(x => x.accounting_cycle).ToList();
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            foreach (accounting_journal item in Accounting_journalList)
            {
                if (item.IsSelected)
                {
                    if (item.accounting_journal_detail.Sum(x => x.credit) != item.accounting_journal_detail.Sum(x => x.debit))
                    {
                        toolbar.msgWarning("Verify balance :-" + item.code);
                    }
                    else
                    {
                        AccountingJournalDB.accounting_journal.Add(item);
                    }
                }
            }

            if (AccountingJournalDB.Approve())
            {
                accounting_journalViewSource.View.Refresh();
                accounting_journalViewSource.View.MoveCurrentToLast();
                toolbar.msgSaved(AccountingJournalDB.NumberOfRecords);
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {

        }

        private void toolBar_btnNew_Click(object sender)
        {

            accounting_journal accounting_journal = new accounting_journal();
            AccountingJournalDB.accounting_journal.Add(accounting_journal);
            accounting_journalViewSource.View.Refresh();
            accounting_journalViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            accounting_journal accounting_journal = (accounting_journal)accounting_journalViewSource.View.CurrentItem;
            accounting_journalViewSource.View.Refresh();
            accounting_journalViewSource.View.MoveCurrentTo(accounting_journal);
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Text<string>("Question_Delete"), "Cognitivo",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    AccountingJournalDB.accounting_journal.Remove((accounting_journal)accounting_journalDataGrid.SelectedItem);
                    accounting_journalViewSource.View.MoveCurrentToFirst();
                    toolbar_btnSave_Click(sender);
                }
            }
            catch (Exception ex)
            {
                toolbar.msgError(ex);
            }
        }

        private void toolbar_btnSave_Click(object sender)
        {
            if (AccountingJournalDB.SaveChanges() == 1)
            {
                accounting_journalViewSource.View.Refresh();
                accounting_journalViewSource.View.MoveCurrentToLast();
                toolbar.msgApproved(AccountingJournalDB.NumberOfRecords);
            }
        }


        private void cbxAccount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && cbxAccount.Data != null)
            {
                int id = Convert.ToInt32(((accounting_chart)cbxAccount.Data).id_chart);
                if (id > 0)
                {
                    accounting_journal accounting_journal = accounting_journalDataGrid.SelectedItem as accounting_journal;
                    if (accounting_journal != null)
                    {
                        accounting_journal_detail _accounting_journal_detail = new entity.accounting_journal_detail();
                        _accounting_journal_detail.accounting_journal = accounting_journal;
                        _accounting_journal_detail.is_head = true;
                        _accounting_journal_detail.accounting_chart = (accounting_chart)cbxAccount.Data;
                        _accounting_journal_detail.id_chart = id;
                        accounting_journal.accounting_journal_detail.Add(_accounting_journal_detail);
                        accounting_journalViewSource.View.Refresh();
                    }
                }
            }
        }

        private void cbxAccount_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            int id = Convert.ToInt32(((accounting_chart)cbxAccount.Data).id_chart);
            if (id > 0)
            {
                accounting_journal accounting_journal = accounting_journalDataGrid.SelectedItem as accounting_journal;
                if (accounting_journal != null)
                {

                    accounting_journal_detail _accounting_journal_detail = new entity.accounting_journal_detail();
                    _accounting_journal_detail.accounting_journal = accounting_journal;
                    _accounting_journal_detail.is_head = true;
                    _accounting_journal_detail.accounting_chart = (accounting_chart)cbxAccount.Data;
                    _accounting_journal_detail.id_chart = id;
                    accounting_journal.accounting_journal_detail.Add(_accounting_journal_detail);


                    accounting_journalViewSource.View.Refresh();

                }

            }
        }
        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cmbcycle.SelectedItem != null)
        //    {
        //        filter_chart(((accounting_cycle)cmbcycle.SelectedItem).id_cycle);
        //    }
        //}

        //public void filter_chart(int id_cycle)
        //{


        //    if (accounting_journalViewSource != null)
        //    {
        //        if (accounting_journalViewSource.View != null)
        //        {
        //            accounting_journalViewSource.View.Filter = i =>
        //            {
        //                accounting_journal accounting_journal = i as accounting_journal;
        //                if (accounting_journal.id_cycle == id_cycle)
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            };
        //        }


        //    }

        //}
    }
}
