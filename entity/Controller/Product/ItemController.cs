using entity.Brillo;
using System;
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

                //If Error or Not Selected, This will Ignore Updates into DB allowing code to run without errors.
                if (item.State > 0)
                {
                    if (item.State != EntityState.Unchanged)
                    {
                        db.Entry(item).State = EntityState.Unchanged;

                        foreach (item_price price in item.item_price)
                        {
                            if (price.State != EntityState.Unchanged)
                            {
                                db.Entry(price).State = EntityState.Unchanged;
                            }
                        }

                        foreach (item_product product in item.item_product)
                        {
                            if (product.State != EntityState.Unchanged)
                            {
                                db.Entry(product).State = EntityState.Unchanged;
                            }
                        }

                        foreach (item_asset asset in item.item_asset)
                        {
                            if (asset.State != EntityState.Unchanged)
                            {
                                db.Entry(asset).State = EntityState.Unchanged;
                            }
                        }

                        foreach (item_dimension dimension in item.item_dimension)
                        {
                            if (dimension.State != EntityState.Unchanged)
                            {
                                db.Entry(dimension).State = EntityState.Unchanged;
                            }
                        }

                        foreach (item_property property in item.item_property)
                        {
                            if (property.State != EntityState.Unchanged)
                            {
                                db.Entry(property).State = EntityState.Unchanged;
                            }
                        }

                        foreach (item_tag_detail tag_detail in item.item_tag_detail)
                        {
                            if (tag_detail.State != EntityState.Unchanged)
                            {
                                db.Entry(tag_detail).State = EntityState.Unchanged;
                            }
                        }
                    }
                }
            }

            db.SaveChanges();
            return true;
        }
    }
}
