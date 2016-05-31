using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Brillo.Accounting
{
    public class Expense_Calc
    {
        public accounting_journal Start(AccountingJournalDB AccountingJournalDB, purchase_invoice purchase_invoice)
        {
            ///PURCHASE
            if (purchase_invoice != null
                && purchase_invoice.accounting_journal == null
                && purchase_invoice.status == entity.Status.Documents_General.Approved)
            {
                DateTime TransDate = purchase_invoice.trans_date;

                accounting_cycle accounting_cycledb = AccountingJournalDB.accounting_cycle
                    .Where(i => i.start_date <= TransDate
                             && i.end_date >= TransDate
                             && i.id_company == Properties.Settings.Default.company_ID)
                    .FirstOrDefault();

                if (accounting_cycledb == null)
                {
                    AccountingCycle AccCycle = new AccountingCycle();
                    int CylceID = AccCycle.Generate_Cycle(TransDate).id_cycle;
                    accounting_cycledb = AccountingJournalDB.accounting_cycle.Where(x => x.id_cycle == CylceID).FirstOrDefault();
                }

                return Calculate_PurchaseInvoice(AccountingJournalDB, purchase_invoice, accounting_cycledb);
            }
            return null;
        }

        private accounting_journal Calculate_PurchaseInvoice(AccountingJournalDB AccountingJournalDB, purchase_invoice purchase_invoice, accounting_cycle accounting_cycle)
        {
            accounting_journal accounting_journal = new accounting_journal();
            if (purchase_invoice.accounting_journal == null)
            {
                accounting_journal.id_cycle = accounting_cycle.id_cycle;
                accounting_journal.trans_date = purchase_invoice.trans_date;
                accounting_journal.IsSelected = true;
                accounting_journal.State = EntityState.Added;
                accounting_journal.comment = purchase_invoice.comment;

                List<accounting_journal_detail> accounting_journal_detailList = new List<accounting_journal_detail>();
                foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice.purchase_invoice_detail.ToList())
                {
                    if (purchase_invoice_detail.app_cost_center.is_product && purchase_invoice_detail.item != null)
                    {
                        List<item_tag_detail> item_tag_detailLIST = purchase_invoice_detail.item.item_tag_detail.ToList();
                        if (item_tag_detailLIST != null)
                        {
                            Asset.Inventory Inventory = new Asset.Inventory();

                            accounting_chart INV_Chart = null;
                            foreach (item_tag_detail item_tag_detail in purchase_invoice_detail.item.item_tag_detail.ToList())
                            {
                                item_tag item_tag = item_tag_detail.item_tag;
                                INV_Chart = Inventory.find_Chart(AccountingJournalDB, item_tag);
                            }

                            if (INV_Chart != null)
                            {
                                accounting_journal_detail INV_accounting_journal = new accounting_journal_detail();
                                INV_accounting_journal.accounting_chart = INV_Chart;
                                INV_accounting_journal.trans_date = purchase_invoice.trans_date;
                                INV_accounting_journal.debit = Math.Round(purchase_invoice_detail.SubTotal,2);
                                INV_accounting_journal.id_currencyfx = purchase_invoice.app_currencyfx.id_currencyfx;
                                accounting_journal_detailList.Add(INV_accounting_journal);
                            }
                        }
                    }
                    else if (purchase_invoice_detail.app_cost_center.is_fixedasset)
                    {
                        //Ignore
                    }
                    else
                    {
                        Expense.AdministrationExpense AdministrationExpense = new Expense.AdministrationExpense();

                        app_cost_center app_cost_center = purchase_invoice_detail.app_cost_center;
                        accounting_chart Exp_Chart = AdministrationExpense.find_Chart(AccountingJournalDB, app_cost_center);
                        if (Exp_Chart != null)
                        {
                            accounting_journal_detail INV_accounting_journal = new accounting_journal_detail();
                            INV_accounting_journal.accounting_chart = Exp_Chart;
                            INV_accounting_journal.trans_date = purchase_invoice.trans_date;
                            INV_accounting_journal.debit =Math.Round( purchase_invoice_detail.SubTotal,2);
                            INV_accounting_journal.id_currencyfx = purchase_invoice.app_currencyfx.id_currencyfx;
                            accounting_journal_detailList.Add(INV_accounting_journal);
                        }
                    }
                }

                Liability.ValueAddedTax VAT = new Liability.ValueAddedTax();
                foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice.purchase_invoice_detail.ToList())
                {
                    foreach (app_vat_group_details app_vat_group in purchase_invoice_detail.app_vat_group.app_vat_group_details)
                    {
                        accounting_chart VAT_Chart = VAT.find_Chart(AccountingJournalDB, app_vat_group.app_vat);
                        if (VAT_Chart != null)
                        {
                            accounting_journal_detail INV_accounting_journal = new accounting_journal_detail();
                            INV_accounting_journal.accounting_chart = VAT_Chart;
                            INV_accounting_journal.trans_date = purchase_invoice.trans_date;
                            INV_accounting_journal.debit = Math.Round(Vat.calculate_Vat((purchase_invoice_detail.unit_cost * purchase_invoice_detail.quantity), app_vat_group.app_vat.coefficient),2);
                            INV_accounting_journal.id_currencyfx = purchase_invoice.app_currencyfx.id_currencyfx;
                            accounting_journal_detailList.Add(INV_accounting_journal);
                        }
                    }
                }

                List<payment_schedual> payment_schedualLIST = AccountingJournalDB.payment_schedual.Where(x => x.id_purchase_invoice == purchase_invoice.id_purchase_invoice).ToList();

                if (payment_schedualLIST != null)
                {
                    ///For Loop for each Payment Schedual.
                    foreach (payment_schedual payment_schedual in payment_schedualLIST)
                    {
                        ///Example: 1000$ Invoice.
                        ///Example: 600$ Paid & 400$ NotPaid.

                        ///Payment Done -> Ex. 600$
                        if (payment_schedual.payment_detail != null && payment_schedual.debit > 0)
                        {
                            Asset.Cash CashAccount = new Asset.Cash();
                            accounting_chart AR_Chart = CashAccount.find_Chart(AccountingJournalDB, payment_schedual.payment_detail.app_account);

                            if (AR_Chart != null)
                            {
                                accounting_journal_detail PAYaccounting_journal_detail = new accounting_journal_detail();
                                PAYaccounting_journal_detail.accounting_chart = AR_Chart;
                                PAYaccounting_journal_detail.trans_date = payment_schedual.trans_date;
                                PAYaccounting_journal_detail.credit = payment_schedual.debit;
                                PAYaccounting_journal_detail.id_currencyfx = payment_schedual.app_currencyfx.id_currencyfx;
                                accounting_journal_detailList.Add(PAYaccounting_journal_detail);
                            }
                        }
                        ///Payment Left -> Ex. 400$
                        else if (payment_schedual.payment_detail == null && payment_schedual.credit > 0)
                        {
                            //Credit Payment
                            Asset.AccountsReceivable AccountsReceivable = new Asset.AccountsReceivable();
                            accounting_chart AR_Chart = AccountsReceivable.find_Chart(AccountingJournalDB, purchase_invoice.contact);

                            if (AR_Chart != null)
                            {
                                accounting_journal_detail AR_accounting_journal_detail = new accounting_journal_detail();
                                AR_accounting_journal_detail.accounting_chart = AR_Chart;
                                AR_accounting_journal_detail.trans_date = purchase_invoice.trans_date;
                                AR_accounting_journal_detail.credit = payment_schedual.credit;
                                AR_accounting_journal_detail.id_currencyfx = purchase_invoice.app_currencyfx.id_currencyfx;
                                accounting_journal_detailList.Add(AR_accounting_journal_detail);
                            }
                        }
                    }
                }

                ///Clean up Duplicate Accounts.
                ///If Duplicate, will sum into first of the same chart it Finds.
                foreach (accounting_journal_detail accounting_journal_detail in accounting_journal_detailList)
                {
                    int id_chart=accounting_journal_detail.accounting_chart.id_chart;
                    if (accounting_journal.accounting_journal_detail.Where(x => x.id_chart == id_chart).Count()==0)
                    {
                        accounting_journal_detail PAYaccounting_journal_detail = new accounting_journal_detail();
                        PAYaccounting_journal_detail.id_chart = accounting_journal_detail.accounting_chart.id_chart;
                        PAYaccounting_journal_detail.accounting_chart = accounting_journal_detail.accounting_chart;
                        PAYaccounting_journal_detail.trans_date = accounting_journal_detail.trans_date;
                        PAYaccounting_journal_detail.credit = Math.Round(accounting_journal_detail.credit,2);
                        PAYaccounting_journal_detail.debit = Math.Round(accounting_journal_detail.debit,2);
                        PAYaccounting_journal_detail.id_currencyfx = accounting_journal_detail.id_currencyfx;
                        accounting_journal.accounting_journal_detail.Add(PAYaccounting_journal_detail);
                    }
                    else
                    {
                        accounting_journal_detail PAYaccounting_journal_detail = accounting_journal.accounting_journal_detail.Where(x => x.id_chart == id_chart).FirstOrDefault();
                        PAYaccounting_journal_detail.credit += Math.Round(accounting_journal_detail.credit,2);
                        PAYaccounting_journal_detail.debit += Math.Round(accounting_journal_detail.debit,2);
                    }
                }
                accounting_journal.purchase_invoice.Add(purchase_invoice);
            }

            return accounting_journal;
        }
    }

    public class Payment_Calc
    {
        public accounting_journal Start(AccountingJournalDB db, payment payment)
        {
            ///PURCHASE
            if (payment != null
                && payment.accounting_journal == null
                && payment.status == entity.Status.Documents_General.Approved)
            {
                accounting_cycle accounting_cycle;
                accounting_cycle accounting_cycledb = db.accounting_cycle.Where(i => i.start_date <= payment.trans_date || i.end_date >= payment.trans_date).FirstOrDefault();

                if (accounting_cycledb == null)
                {
                    AccountingCycle AccCycle = new AccountingCycle();
                    accounting_cycle = AccCycle.Generate_Cycle(payment.trans_date);
                }
                else
                {
                    accounting_cycle = accounting_cycledb;
                }

                return Calculate_Payment(db, payment, accounting_cycle);
            }
            return null;
        }

        private accounting_journal Calculate_Payment(AccountingJournalDB db, payment payment, accounting_cycle accounting_cycle)
        {
            return null;
        }
    }
}
