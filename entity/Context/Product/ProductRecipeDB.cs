using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace entity
{
    public partial class ProductRecipeDB : BaseDB
    {
        public item_recepie New()
        {
            item_recepie item_recepie = new item_recepie();
            item_recepie.State = EntityState.Added;
            item_recepie.IsSelected = true;

            return item_recepie;
        }
        public override int SaveChanges()
        {
            validate_ProductRecipe();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_ProductRecipe();
            return base.SaveChangesAsync();
        }

        private void validate_ProductRecipe()
        {
            foreach (item_recepie item_recepie in base.item_recepie.Local)
            {
                if (item_recepie.IsSelected)
                    // && item_transfer.Error == null)
                {
                    if (item_recepie.State == EntityState.Added)
                    {
                        item_recepie.timestamp = DateTime.Now;
                        item_recepie.State = EntityState.Unchanged;
                        Entry(item_recepie).State = EntityState.Added;
                    }
                    else if (item_recepie.State == EntityState.Modified)
                    {
                        item_recepie.timestamp = DateTime.Now;
                        item_recepie.State = EntityState.Unchanged;
                        Entry(item_recepie).State = EntityState.Modified;
                    }
                    else if (item_recepie.State == EntityState.Deleted)
                    {
                        item_recepie.timestamp = DateTime.Now;
                        item_recepie.State = EntityState.Unchanged;
                        base.item_recepie.Remove(item_recepie);
                    }
                }
                else if (item_recepie.State > 0)
                {
                    if (item_recepie.State != EntityState.Unchanged)
                    {
                        Entry(item_recepie).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
