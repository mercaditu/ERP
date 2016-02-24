using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace entity
{
    public partial class ItemDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Item();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Item();
            return base.SaveChangesAsync();
        }

        private void validate_Item()
        {
            foreach (item item in base.items.Local)
            {
                if (item.IsSelected && item.Error == null)
                {
                    if (item.State == EntityState.Added)
                    {
                        item.timestamp = DateTime.Now;
                        item.State = EntityState.Unchanged;
                        Entry(item).State = EntityState.Added;
                    }
                    else if (item.State == EntityState.Modified)
                    {
                        item.timestamp = DateTime.Now;
                        item.State = EntityState.Unchanged;
                        Entry(item).State = EntityState.Modified;
                    }
                    else if (item.State == EntityState.Deleted)
                    {
                        item.timestamp = DateTime.Now;
                        item.State = EntityState.Unchanged;
                        base.items.Remove(item);
                    }
                }
                else if (item.State > 0)
                {
                    if (item.State != EntityState.Unchanged)
                    {
                        Entry(item).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
