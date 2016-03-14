using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace entity
{
    public partial class ItemDB : BaseDB
    {
        public void New(item item)
        {
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
        }

        public override int SaveChanges()
        {
            validate_Item();
            //item a = (item)GetClone(base.items.FirstOrDefault(), typeof(item));
            //a.is_head = false;
            //var source = base.items.FirstOrDefault();
            //var clone = new item();
            //base.items.Add(clone);

            //var sourceValues = base.Entry(source).CurrentValues;
            //base.Entry(clone).CurrentValues.SetValues(sourceValues);

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
