using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace entity
{
    public partial class ItemDB : BaseDB
    {
        public item New()
        {
            item item = new item();
            item.State = System.Data.Entity.EntityState.Added;
            item.IsSelected = true;
            item.unit_cost = 0;

            if (item.State > 0)
            {
                using (db db = new db())
                {
                    if (db.app_vat_group.Where(x => x.is_default == true && x.id_company == Properties.Settings.Default.company_ID).FirstOrDefault() != null)
                        item.id_vat_group = db.app_vat_group.Where(x => x.is_default == true && x.id_company == Properties.Settings.Default.company_ID).FirstOrDefault().id_vat_group;
                    else
                        item.id_vat_group = 0;
                }
            }
            return item;
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
                    NumberOfRecords += 1;
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
