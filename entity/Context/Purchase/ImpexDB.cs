using System;
using System.Threading.Tasks;
using System.Data.Entity;

namespace entity
{
    public partial class ImpexDB : BaseDB
    {

        public override int SaveChanges()
        {
            validate_Contact();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Contact();
            return base.SaveChangesAsync();
        }

        private void validate_Contact()
        {
            foreach (impex impex in base.impex.Local)
            {
                if (impex.IsSelected )
                {
                    if (impex.State == EntityState.Added)
                    {
                        impex.timestamp = DateTime.Now;
                        impex.State = EntityState.Unchanged;
                        Entry(impex).State = EntityState.Added;
                    }
                    else if (impex.State == EntityState.Modified)
                    {
                        impex.timestamp = DateTime.Now;
                        impex.State = EntityState.Unchanged;
                        Entry(impex).State = EntityState.Modified;
                    }
                    else if (impex.State == EntityState.Deleted)
                    {
                        impex.timestamp = DateTime.Now;
                        impex.State = EntityState.Unchanged;
                        //impex.Remove(impex);
                    }
                }
                else if (impex.State > 0)
                {
                    if (impex.State != EntityState.Unchanged)
                    {
                        Entry(impex).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
