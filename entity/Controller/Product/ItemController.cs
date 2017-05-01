using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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

            if (New_ItemPrice(item) != null)
            {
                item.item_price.Add(New_ItemPrice(item));
            }

            return item;
        }

        public item_price New_ItemPrice(item item)
        {

            if (CurrentSession.Currency_Default == null)
            {
                if (db.app_currency.Where(x => x.id_company == CurrentSession.Id_Company).Any())
                {
                    CurrentSession.Currency_Default = db.app_currency.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                }
            }

            if (CurrentSession.PriceLists.Where(x => x.is_default).FirstOrDefault() != null)
            {
                CurrentSession.PriceLists.FirstOrDefault().is_default = true;
            }

            item_price item_price = new item_price()
            {
                id_currency = CurrentSession.Currency_Default.id_currency,
                id_price_list = CurrentSession.PriceLists.Where(x => x.is_default).FirstOrDefault().id_price_list
            };

            return item_price;
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
            
            foreach (var error in db.GetValidationErrors())
            {
                item item = error.Entry.Entity as item;
                if (item != null)
                {
                    if (item.item_price.Count() > 0)
                    {
                        db.item_price.RemoveRange(item.item_price);
                    }
                }

                db.Entry(item).State = EntityState.Detached;
            }
           
            db.SaveChanges();
            foreach (item item in db.items.Local)
            {
                item.State = EntityState.Unchanged;
            }
            return true;
        }
    }
}
