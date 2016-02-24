using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
    public partial class AccountingChartDB : BaseDB
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
            foreach (accounting_chart accounting_chart in base.accounting_chart.Local)
            {
                if (accounting_chart.IsSelected)
                {
                    if (accounting_chart.State == EntityState.Added)
                    {
                        accounting_chart.timestamp = DateTime.Now;
                        accounting_chart.State = EntityState.Unchanged;
                        Entry(accounting_chart).State = EntityState.Added;
                    }
                    else if (accounting_chart.State == EntityState.Modified)
                    {
                        accounting_chart.timestamp = DateTime.Now;
                        accounting_chart.State = EntityState.Unchanged;
                        Entry(accounting_chart).State = EntityState.Modified;
                    }
                    else if (accounting_chart.State == EntityState.Deleted)
                    {
                        accounting_chart.timestamp = DateTime.Now;
                        accounting_chart.State = EntityState.Unchanged;
                        base.accounting_chart.Remove(accounting_chart);
                    }
                }
                else if (accounting_chart.State > 0)
                {
                    if (accounting_chart.State != EntityState.Unchanged)
                    {
                        Entry(accounting_chart).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
