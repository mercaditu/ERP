using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Controller.Product
{
    public class ItemController : Base
    {
        public async void Load(bool FilterByProduct, bool FilterByRawMaterial, 
            bool FilterBySupplies, bool FilterByTask, bool FilterByService, bool FilterByServiceContract)
        {
            var predicate = PredicateBuilder.True<item>();
            predicate = (x => (x.id_company == CurrentSession.Id_Company || x.id_company == null));

            var predicateOR = PredicateBuilder.False<item>();

            if (FilterByProduct)
            {
                predicateOR = predicateOR.Or(x => x.id_item_type == item.item_type.Product);
            }

            if (FilterByRawMaterial)
            {
                predicateOR = predicateOR.Or(x => x.id_item_type == item.item_type.RawMaterial);
            }

            if (FilterBySupplies)
            {
                predicateOR = predicateOR.Or(x => x.id_item_type == item.item_type.Supplies);
            }

            if (FilterByTask)
            {
                predicateOR = predicateOR.Or(x => x.id_item_type == item.item_type.Task);
            }

            if (FilterByService)
            {
                predicateOR = predicateOR.Or(x => x.id_item_type == item.item_type.Service);
            }

            if (FilterByServiceContract)
            {
                predicateOR = predicateOR.Or(x => x.id_item_type == item.item_type.ServiceContract);
            }

            predicate = predicate.And
            (
                predicateOR
            );

            await db.items.Where(predicate).OrderBy(x => x.name).LoadAsync();
            
            await db.app_measurement
                  .Where(a => a.is_active && a.id_company == CurrentSession.Id_Company)
                  .OrderBy(a => a.name).LoadAsync();
            await db.app_dimension
                  .Where(a => a.id_company == CurrentSession.Id_Company)
                  .OrderBy(a => a.name).LoadAsync();

            await db.app_property.OrderBy(a => a.name).LoadAsync();
            await db.item_tag
                  .Where(x => x.id_company == CurrentSession.Id_Company && x.is_active)
                  .OrderBy(x => x.name).LoadAsync();
            await db.item_template
                  .Where(x => x.id_company == CurrentSession.Id_Company && x.is_active)
                  .OrderBy(x => x.name).LoadAsync();
            await db.hr_talent
                  .Where(a => a.is_active && a.id_company == CurrentSession.Id_Company)
                  .OrderBy(a => a.name).LoadAsync();
            await db.item_brand
                  .Where(a => a.id_company == CurrentSession.Id_Company)
                  .OrderBy(a => a.name).LoadAsync();
        }

        public item Create()
        {
            item item = new item()
            {
                State = EntityState.Added,
                IsSelected = true,
                unit_cost = 0
            };

            if (db.app_vat_group.Where(x => x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
            {
                item.id_vat_group = db.app_vat_group.Where(x => x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
            }
            else
            {
                item.id_vat_group = 0;
            }

            item_price_list price_list = CurrentSession.PriceLists.Where(x => x.is_default).FirstOrDefault();
            if (price_list != null && CurrentSession.Currency_Default != null)
            {
                item.item_price.Add(new item_price()
                {
                    id_currency = CurrentSession.Currency_Default.id_currency,
                    id_price_list = price_list.id_price_list
                });
            }

            return item;
        }

        public bool Edit(item item)
        {
            item.IsSelected = true;
            item.State = EntityState.Modified;
            db.Entry(item).State = EntityState.Modified;
            return true;
        }

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (item item in db.items.Local)
            {
                if (item.IsSelected && item.Error == null)
                {
                    if (item.State == EntityState.Added)
                    {
                        item.timestamp = DateTime.Now;
                        item.State = EntityState.Unchanged;
                        db.Entry(item).State = EntityState.Added;
                        item.IsSelected = false;
                    }
                    else if (item.State == EntityState.Modified)
                    {
                        item.timestamp = DateTime.Now;
                        item.State = EntityState.Unchanged;
                        db.Entry(item).State = EntityState.Modified;
                        item.IsSelected = false;
                    }
                    NumberOfRecords += 1;
                }
                if (item.State > 0)
                {
                    if (item.State != EntityState.Unchanged)
                    {
                        db.Entry(item).State = EntityState.Unchanged;
                    }
                }
            }
            return true;
        }
    }
}
