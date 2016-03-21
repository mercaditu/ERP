using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Brillo.Accounting
{
    public class Income_Calc
    {
        public accounting_journal Start(AccountingJournalDB AccountingJournalDB, sales_invoice sales_invoice)
        {
            if (sales_invoice != null
                  && sales_invoice.accounting_journal == null
                  && sales_invoice.status == entity.Status.Documents_General.Approved)
            {
                accounting_cycle accounting_cycledb = AccountingJournalDB.accounting_cycle
                    .Where(i => 
                    i.start_date <= sales_invoice.trans_date ||
                    i.end_date >= sales_invoice.trans_date && 
                    i.id_company == Properties.Settings.Default.company_ID)
                    .FirstOrDefault();

                if (accounting_cycledb == null)
                {
                    AccountingCycle AccCycle = new AccountingCycle();
                    int id_cycle=AccCycle.Generate_Cycle(sales_invoice.trans_date).id_cycle;
                    accounting_cycledb = AccountingJournalDB.accounting_cycle.Where(x => x.id_cycle == id_cycle).FirstOrDefault();
                }

                //Check which Contract Detail has 0 Days == Cash
              return  calc(AccountingJournalDB, sales_invoice, accounting_cycledb);
            }
            return null;
        }

        private accounting_journal calc(AccountingJournalDB context, sales_invoice sales_invoice, accounting_cycle accounting_cycle)
        {
            //List<accounting_journal> ListAccountingJournal = new List<accounting_journal>();

            accounting_journal accounting_journal = new accounting_journal();
            if (sales_invoice.accounting_journal == null)
            {
               // accounting_journal accounting_journal = new accounting_journal();
                accounting_journal.id_cycle = accounting_cycle.id_cycle;
                accounting_journal.comment = sales_invoice.comment;
                accounting_journal.trans_date = sales_invoice.trans_date;
                accounting_journal.State = EntityState.Added;

                List<accounting_journal_detail> accounting_journal_detailList = new List<accounting_journal_detail>();

                //List<sales_invoice_detail> _sales_invoice_detail = sales_invoice.sales_invoice_detail.ToList();
                Asset.Inventory Inventory = new Asset.Inventory();
                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail.ToList())
                {
                    accounting_chart INV_Chart = null;
                    
                    if (sales_invoice_detail.item != null)
                    {
                        if (sales_invoice_detail.item.item_tag_detail != null)
                        {
                            foreach (item_tag_detail item_tag_detail in sales_invoice_detail.item.item_tag_detail.ToList())
                            {
                                item_tag item_tag = item_tag_detail.item_tag;

                                if (Inventory.find_Chart(context, item_tag) != null)
                                {
                                    INV_Chart = Inventory.find_Chart(context, item_tag);
                                }
                            }
                        }
                    }
                    else
                    {
                        INV_Chart = Inventory.find_Chart(context, null);
                    }

                    if (INV_Chart != null)
                    {
                        accounting_journal_detail INV_accounting_journal = new accounting_journal_detail();
                        INV_accounting_journal.accounting_chart = INV_Chart;
                        INV_accounting_journal.id_currencyfx = sales_invoice_detail.sales_invoice.id_currencyfx;
                        INV_accounting_journal.credit = sales_invoice_detail.SubTotal;
                        INV_accounting_journal.trans_date = sales_invoice.trans_date;
                        accounting_journal_detailList.Add(INV_accounting_journal);
                    }
                }

                Asset.ValueAddedTax VAT = new Asset.ValueAddedTax();
                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail.ToList())
                {
                    foreach (app_vat_group_details app_vat_group in sales_invoice_detail.app_vat_group.app_vat_group_details)
                    {
                        accounting_chart VAT_Chart = VAT.find_Chart(context, app_vat_group.app_vat);
                        if (VAT_Chart != null)
                        {
                            accounting_journal_detail INV_accounting_journal = new accounting_journal_detail();
                            INV_accounting_journal.trans_date = sales_invoice.trans_date;
                            INV_accounting_journal.accounting_chart = VAT_Chart;
                            INV_accounting_journal.credit = Vat.calculate_Vat(sales_invoice_detail.unit_price, app_vat_group.app_vat.coefficient);
                            INV_accounting_journal.id_currencyfx = sales_invoice.app_currencyfx.id_currencyfx;
                            accounting_journal_detailList.Add(INV_accounting_journal);
                        }
                    }
                }


                app_contract_detail app_contract_detail;
                app_contract_detail = context.app_contract_detail.Where(e => e.interval == 0).FirstOrDefault();

                if (app_contract_detail.app_contract.id_contract != sales_invoice.app_contract.id_contract)
                {
                    Asset.AccountsReceivable AccountsReceivable = new Asset.AccountsReceivable();
                    accounting_chart AR_Chart = AccountsReceivable.find_Chart(context, sales_invoice.contact);
                    if (AR_Chart != null)
                    {
                        accounting_journal_detail AR_accounting_journal_detail = new accounting_journal_detail();
                        AR_accounting_journal_detail.accounting_chart = AR_Chart;
                        AR_accounting_journal_detail.trans_date = sales_invoice.trans_date;
                        AR_accounting_journal_detail.debit = sales_invoice.GrandTotal;
                        AR_accounting_journal_detail.id_currencyfx = sales_invoice.app_currencyfx.id_currencyfx;
                        accounting_journal_detailList.Add(AR_accounting_journal_detail);
                    }
                }
                else
                {
                    //Cash Payments
                    List<payment_schedual> payment_schedualLIST = context.payment_schedual.Where(x => x.id_sales_invoice == sales_invoice.id_sales_invoice).ToList();
                    foreach (payment_schedual schedual in payment_schedualLIST)
                    {
                        Asset.Cash CashAccount = new Asset.Cash();

                        if (schedual!=null)
                        {
                            if (schedual.payment_detail!=null)
                            {
                                accounting_chart AR_Chart = CashAccount.find_Chart(context, schedual.payment_detail.app_account);
                                if (AR_Chart != null)
                                {
                                    accounting_journal_detail PAYaccounting_journal_detail = new accounting_journal_detail();
                                    PAYaccounting_journal_detail.accounting_chart = AR_Chart;
                                    PAYaccounting_journal_detail.trans_date = schedual.trans_date;
                                    PAYaccounting_journal_detail.debit = schedual.credit;
                                    PAYaccounting_journal_detail.id_currencyfx = schedual.app_currencyfx.id_currencyfx;
                                    accounting_journal_detailList.Add(PAYaccounting_journal_detail);
                                }
                                }
                           
                            }
                           
                        }
                }
                foreach (accounting_journal_detail accounting_journal_detail in accounting_journal_detailList)
                {
                    int id_chart = accounting_journal_detail.accounting_chart.id_chart;
                    if (accounting_journal.accounting_journal_detail.Where(x => x.id_chart == id_chart).Count() == 0)
                    {
                        accounting_journal_detail PAYaccounting_journal_detail = new accounting_journal_detail();
                        PAYaccounting_journal_detail.id_chart = accounting_journal_detail.accounting_chart.id_chart;
                        PAYaccounting_journal_detail.accounting_chart = accounting_journal_detail.accounting_chart;
                        PAYaccounting_journal_detail.trans_date = accounting_journal_detail.trans_date;
                        PAYaccounting_journal_detail.credit = accounting_journal_detail.credit;
                        PAYaccounting_journal_detail.debit = accounting_journal_detail.debit;
                        PAYaccounting_journal_detail.id_currencyfx = accounting_journal_detail.id_currencyfx;
                        accounting_journal.accounting_journal_detail.Add(PAYaccounting_journal_detail);
                        
                    }
                    else
                    {
                        accounting_journal_detail PAYaccounting_journal_detail = accounting_journal.accounting_journal_detail.Where(x => x.id_chart == id_chart).FirstOrDefault();
                        PAYaccounting_journal_detail.credit += accounting_journal_detail.credit;
                        PAYaccounting_journal_detail.debit += accounting_journal_detail.debit;
                    }
                }
                
                accounting_journal.sales_invoice.Add(sales_invoice);
            }
            return accounting_journal;
        }
    }
}
