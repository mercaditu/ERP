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
                    //    if (item_inventory_detail.value_counted != 0)
                    //    {
                    //        validate_item_inventory_detail(item_inventory_detail);
                    //    }
                    //    else
                    //    {
                    //         Entry(item_inventory_detail).State = EntityState.Unchanged;
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
                if(item_inventory.id_inventory == 0)
                {
                    SaveChanges();
                }

                List<item_movement> item_movementLIST = new List<item_movement>();

                foreach (item_inventory_detail item_inventory_detail in item_inventory.item_inventory_detail)
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
                        item_movement.id_inventory = item_inventory_detail.id_inventory_detail;
                        item_movement.timestamp = DateTime.Now;
                        item_movementLIST.Add(item_movement);
                    }
                }

                base.item_movement.AddRange(item_movementLIST);

                item_inventory.status = Status.Documents.Issued;
                SaveChanges();
            }
        }
    }
}



