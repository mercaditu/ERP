using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public partial class AccountingCycleDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Cycle();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Cycle();
            return base.SaveChangesAsync();
        }


        private void validate_Cycle()
        {
            foreach (accounting_cycle accounting_cycle in base.accounting_cycle.Local)
            {
                if (accounting_cycle.IsSelected)
                {
                    if (accounting_cycle.State == EntityState.Added)
                    {
                        accounting_cycle.timestamp = DateTime.Now;
                        accounting_cycle.State = EntityState.Unchanged;
                        Entry(accounting_cycle).State = EntityState.Added;
                    }
                    else if (accounting_cycle.State == EntityState.Modified)
                    {
                        accounting_cycle.timestamp = DateTime.Now;
                        accounting_cycle.State = EntityState.Unchanged;
                        Entry(accounting_cycle).State = EntityState.Modified;
                    }
                    else if (accounting_cycle.State == EntityState.Deleted)
                    {
                        accounting_cycle.timestamp = DateTime.Now;
                        accounting_cycle.State = EntityState.Unchanged;
                        base.accounting_cycle.Remove(accounting_cycle);
                    }
                }
                else if (accounting_cycle.State > 0)
                {
                    if (accounting_cycle.State != EntityState.Unchanged)
                    {
                        Entry(accounting_cycle).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
