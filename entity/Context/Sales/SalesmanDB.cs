using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
  public partial  class SalesmanDB : BaseDB
    {

        public override int SaveChanges()
        {
            validate_Salesman();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Salesman();
            return base.SaveChangesAsync();
        }

        private void validate_Salesman()
        {
            foreach (sales_rep sales_rep in base.sales_rep.Local)
            {
                if (sales_rep.IsSelected && sales_rep.Error == null)
                {
                    if (sales_rep.State == EntityState.Added)
                    {
                        sales_rep.timestamp = DateTime.Now;
                        sales_rep.State = EntityState.Unchanged;
                        Entry(sales_rep).State = EntityState.Added;
                    }
                    else if (sales_rep.State == EntityState.Modified)
                    {
                        sales_rep.timestamp = DateTime.Now;
                        sales_rep.State = EntityState.Unchanged;
                        Entry(sales_rep).State = EntityState.Modified;
                    }
                    else if (sales_rep.State == EntityState.Deleted)
                    {
                        sales_rep.timestamp = DateTime.Now;
                        sales_rep.is_head = false;
                        sales_rep.State = EntityState.Deleted;
                        base.sales_rep.Remove(sales_rep);
                    }
                }
                else if (sales_rep.State > 0)
                {
                    if (sales_rep.State != EntityState.Unchanged)
                    {
                        Entry(sales_rep).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
