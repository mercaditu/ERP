using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo.Accounting
{
    class Payment_Calc
    {
        //public class Payment_Calc
        //{
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
    // }
}
