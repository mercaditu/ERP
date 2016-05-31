using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Collections.Generic;


namespace Cognitivo.Accounting
{
    public partial class ExpenseJournal : Page
    {
        CollectionViewSource accounting_journalViewSource;
        AccountingJournalDB AccountingJournalDB = new AccountingJournalDB();
        List<accounting_journal> Accounting_journalList = new List<accounting_journal>();

        public ExpenseJournal()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _setting = new entity.Properties.Settings();

            accounting_journalViewSource = (CollectionViewSource)FindResource("accounting_journalViewSource");
            accounting_journalViewSource.Source = Accounting_journalList;

            List<purchase_invoice> purchase_invoiceList = AccountingJournalDB.purchase_invoice.Where(i => i.accounting_journal == null && i.id_company == _setting.company_ID
                                                                                       && i.status == Status.Documents_General.Approved
                                                                                       && i.is_head == true)
                                                                                       .ToList();

            foreach (purchase_invoice purchase_invoice in purchase_invoiceList)
            {
                entity.Brillo.Accounting.Expense_Calc AccountingLogic = new entity.Brillo.Accounting.Expense_Calc();
                accounting_journal accounting_journal = AccountingLogic.Start(AccountingJournalDB, purchase_invoice);

                accounting_journal.IsSelected = false;

                Accounting_journalList.Add(accounting_journal);
                accounting_journalViewSource.View.Refresh();
                accounting_journalViewSource.View.MoveCurrentToLast();
            }

            //CollectionViewSource accounting_cycleViewSource = (CollectionViewSource)FindResource("accounting_cycleViewSource");
            //accounting_cycleViewSource.Source = Accounting_journalList.GroupBy(x => x.accounting_cycle).ToList();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            foreach (accounting_journal item in Accounting_journalList)
            {
                if (item.IsSelected)
                {
                    if (item.accounting_journal_detail.Sum(x=>x.credit)!=item.accounting_journal_detail.Sum(x=>x.debit))
                    {
                        toolbar.msgWarning("Balance is not equal For the jouranl:-" + item.code);
                        
                    }
                    else
                    {
                        AccountingJournalDB.accounting_journal.Add(item);
                    }
                  
                }

            }

            if (AccountingJournalDB.Approve())
            {
                toolbar.msgSaved(AccountingJournalDB.NumberOfRecords);   
            }
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
