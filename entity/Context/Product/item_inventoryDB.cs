using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class InventoryDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_item_inventory();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_item_inventory();
            return base.SaveChangesAsync();
        }

        private void validate_item_inventory()
        {
            foreach (item_inventory item_inventory in base.item_inventory.Local)
            {
                if (item_inventory.IsSelected)
                {
                    //foreach (item_inventory_detail item_inventory_detail in item_inventory.item_inventory_detail)
                    //{
                    //    if (item_inventory_detail.value_counted != 0 && item_inventory_detail.IsSelected)
                    //    {
                    //        validate_item_inventory_detail(item_inventory_detail);
                    //    }
                    //    else
                    //    {
                    //        Entry(item_inventory_detail).State = EntityState.Unchanged;
                    //    }
                    //}
                    if (item_inventory.State == EntityState.Added)
                    {
                        item_inventory.timestamp = DateTime.Now;
                        item_inventory.State = EntityState.Unchanged;
                        Entry(item_inventory).State = EntityState.Added;
                    }
                    else if (item_inventory.State == EntityState.Modified)
                    {
                        item_inventory.timestamp = DateTime.Now;
                        item_inventory.State = EntityState.Unchanged;
                        Entry(item_inventory).State = EntityState.Modified;
                    }
                    else if (item_inventory.State == EntityState.Deleted)
                    {
                        item_inventory.timestamp = DateTime.Now;
                        item_inventory.State = EntityState.Unchanged;
                        base.item_inventory.Remove(item_inventory);
                    }

                }
                else if (item_inventory.State > 0)
                {
                    if (item_inventory.State != EntityState.Unchanged)
                    {
                        Entry(item_inventory).State = EntityState.Unchanged;
                    }
                }
            }
        }

        private void validate_item_inventory_detail(item_inventory_detail item_inventory_detail)
        {
            if (item_inventory_detail.State == EntityState.Added)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                Entry(item_inventory_detail).State = EntityState.Added;
            }
            else if (item_inventory_detail.State == EntityState.Modified)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                Entry(item_inventory_detail).State = EntityState.Modified;
            }
            else if (item_inventory_detail.State == EntityState.Deleted)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                base.item_inventory_detail.Remove(item_inventory_detail);
            }

            if (item_inventory_detail.State != EntityState.Unchanged)
            {
                Entry(item_inventory_detail).State = EntityState.Unchanged;
            }
        }

        public void Approve()
        {
            foreach (item_inventory item_inventory in base.item_inventory.Local.Where(x =>
                                                                                      x.status != Status.Documents.Issued
                                                                                   && x.IsSelected))
            {
                if (item_inventory.id_inventory == 0)
                {
                    SaveChanges();
                }

                List<item_movement> item_movementLIST = new List<item_movement>();

                foreach (item_inventory_detail item_inventory_detail in item_inventory.item_inventory_detail)
                {
                    if (item_inventory_detail.item_inventory_dimension.Count() > 0)
                    {
                        item_movement item_movement = new item_movement();
                        item_movement.id_item_product = item_inventory_detail.item_product.id_item_product;
                        item_movement.id_location = item_inventory_detail.app_location.id_location;
                        item_movement.comment = Brillo.Localize.Text<string>("Inventory") + ": " + item_inventory_detail.comment;
                        item_movement.status = Status.Stock.InStock;
                        item_movement.debit = 0;
                        item_movement.credit = item_inventory_detail.value_counted;
                        item_movement.trans_date = item_inventory.trans_date;
                        item_movement.id_application = App.Names.Inventory;
                        item_movement.id_inventory_detail = item_inventory_detail.id_inventory_detail;
                        item_movement.id_inventory = item_inventory_detail.id_inventory;
                        item_movement.timestamp = DateTime.Now;


                        if (item_inventory_detail.unit_value > 0 && item_inventory_detail.id_currencyfx > 0)
                        {
                            item_movement_value item_movement_value = new item_movement_value();
                            item_movement_value.unit_value = item_inventory_detail.unit_value;
                            item_movement_value.id_currencyfx = item_inventory_detail.id_currencyfx;
                            item_movement_value.comment = Brillo.Localize.Text<string>("Inventory") + ": " + item_inventory_detail.comment;
                            item_movement.item_movement_value.Add(item_movement_value);
                        }

                        foreach (item_inventory_dimension item_inventory_dimension in item_inventory_detail.item_inventory_dimension)
                        {
                            item_movement_dimension item_movement_dimension = new item_movement_dimension();
                            item_movement_dimension.value = item_inventory_dimension.value;
                            item_movement_dimension.id_dimension = item_inventory_dimension.id_dimension;
                            item_movement.item_movement_dimension.Add(item_movement_dimension);
                        }
                        item_movementLIST.Add(item_movement);

                    }
                    else
                    {
                        if (item_inventory_detail.value_counted != 0)
                        {


                            decimal delta = 0;
                            if (item_inventory_detail.value_system != item_inventory_detail.value_counted)
                            {
                                //Negative
                                delta = item_inventory_detail.value_counted - item_inventory_detail.value_system;
                            }

                            if (delta != 0)
                            {
                                item_movement item_movement = new item_movement();
                                item_movement.id_item_product = item_inventory_detail.item_product.id_item_product;
                                item_movement.id_location = item_inventory_detail.app_location.id_location;
                                item_movement.comment = Brillo.Localize.Text<string>("Inventory") + ": " + item_inventory_detail.comment;
                                item_movement.status = Status.Stock.InStock;
                                item_movement.credit = delta > 0 ? delta : 0;
                                item_movement.debit = delta < 0 ? Math.Abs(delta) : 0;
                                item_movement.trans_date = item_inventory.trans_date;
                                item_movement.id_application = App.Names.Inventory;
                                item_movement.id_inventory_detail = item_inventory_detail.id_inventory_detail;
                                item_movement.id_inventory = item_inventory_detail.id_inventory;
                                item_movement.timestamp = DateTime.Now;


                                if (item_inventory_detail.unit_value > 0 && item_inventory_detail.id_currencyfx > 0)
                                {
                                    item_movement_value item_movement_value = new item_movement_value();
                                    item_movement_value.unit_value = item_inventory_detail.unit_value;
                                    item_movement_value.id_currencyfx = item_inventory_detail.id_currencyfx;
                                    item_movement.item_movement_value.Add(item_movement_value);
                                }

                                item_movementLIST.Add(item_movement);
                            }
                        }
                    }
                }

                base.item_movement.AddRange(item_movementLIST);

                item_inventory.status = Status.Documents.Issued;
                SaveChanges();
            }
        }
    }
}



