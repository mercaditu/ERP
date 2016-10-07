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
            item.State = EntityState.Added;
            item.IsSelected = true;
            item.unit_cost = 0;

            using (db db = new db())
            {
                if (db.app_vat_group.Where(x => x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                    item.id_vat_group = db.app_vat_group.Where(x => x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
                else
                    item.id_vat_group = 0;
            }

            item.item_price.Add(New_ItemPrice(item));

            return item;
        }

        public item_price New_ItemPrice(item item)
        {
            item_price item_price = new item_price();
            Brillo.General general = new Brillo.General();
            item_price.id_currency = general.Get_Currency(CurrentSession.Id_Company);
            item_price.id_price_list = general.get_price_list(CurrentSession.Id_Company);
            return item_price;
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
                        item.IsSelected = false;
                    }
                    else if (item.State == EntityState.Modified)
                    {
                        item.timestamp = DateTime.Now;
                        item.State = EntityState.Unchanged;
                        Entry(item).State = EntityState.Modified;
                        item.IsSelected = false;
                    }
                    NumberOfRecords += 1;
                }
                if (item.State > 0)
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
