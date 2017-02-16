using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
    public partial class PromotionDB : BaseDB
    {
        public sales_promotion New()
        {
            sales_promotion sales_promotion = new sales_promotion();
            sales_promotion.State = System.Data.Entity.EntityState.Added;
            sales_promotion.IsSelected = true;

            return sales_promotion;
        }

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
            NumberOfRecords = 0;

            foreach (sales_promotion sales_promotion in base.sales_promotion.Local)
            {
                if (sales_promotion.IsSelected)
                {
                    if (sales_promotion.State == EntityState.Added)
                    {
                        sales_promotion.timestamp = DateTime.Now;
                        sales_promotion.State = EntityState.Unchanged;
                        Entry(sales_promotion).State = EntityState.Added;
                    }
                    else if (sales_promotion.State == EntityState.Modified)
                    {
                        sales_promotion.timestamp = DateTime.Now;
                        sales_promotion.State = EntityState.Unchanged;
                        Entry(sales_promotion).State = EntityState.Modified;
                    }
                    NumberOfRecords += 1;
                }
                else if (sales_promotion.State > 0)
                {
                    if (sales_promotion.State != EntityState.Unchanged)
                    {
                        Entry(sales_promotion).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}