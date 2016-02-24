using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class IncotermDB : BaseDB
    {

        public override int SaveChanges()
        {
            validate_Incoterm();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Incoterm();
            return base.SaveChangesAsync();
        }


        private void validate_Incoterm()
        {
            foreach (impex_incoterm impex_incoterm in base.impex_incoterm.Local)
            {
                if (impex_incoterm.IsSelected)
                {
                    if (impex_incoterm.State == EntityState.Added)
                    {
                        impex_incoterm.timestamp = DateTime.Now;
                        impex_incoterm.State = EntityState.Unchanged;
                        Entry(impex_incoterm).State = EntityState.Added;
                    }
                    else if (impex_incoterm.State == EntityState.Modified)
                    {
                        impex_incoterm.timestamp = DateTime.Now;
                        impex_incoterm.State = EntityState.Unchanged;
                        Entry(impex_incoterm).State = EntityState.Modified;
                    }
                    else if (impex_incoterm.State == EntityState.Deleted)
                    {
                        impex_incoterm.timestamp = DateTime.Now;
                        impex_incoterm.State = EntityState.Unchanged;
                        base.impex_incoterm.Remove(impex_incoterm);
                    }
                }
                else if (impex_incoterm.State > 0)
                {
                    if (impex_incoterm.State != EntityState.Unchanged)
                    {
                        Entry(impex_incoterm).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}