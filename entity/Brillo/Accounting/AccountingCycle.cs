using System;

namespace entity.Brillo.Accounting
{
    public class AccountingCycle
    {
        public accounting_cycle Generate_Cycle(DateTime TransDate)
        {
            int Year = TransDate.Year;
            accounting_cycle accounting_cycle_new = new accounting_cycle();
            accounting_cycle_new.name = "P. " + Year;
            accounting_cycle_new.start_date = new DateTime(Year, 1, 1);
            accounting_cycle_new.end_date = new DateTime(Year, 12, 31);
            
            using (db db = new db())
            {
                db.accounting_cycle.Add(accounting_cycle_new);
                db.SaveChangesAsync();
            }

            return accounting_cycle_new;
        }
    }
}
