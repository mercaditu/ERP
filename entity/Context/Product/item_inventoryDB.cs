using entity.Brillo.Document;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
            NumberOfRecords = 0;

            foreach (item_inventory item_inventory in base.item_inventory.Local)
            {
                if (item_inventory.IsSelected)
                {
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


                    NumberOfRecords += 1;

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
            NumberOfRecords = 0;

            if (item_inventory_detail.State == EntityState.Added)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                Entry(item_inventory_detail).State = EntityState.Added;

                NumberOfRecords += 1;
            }
            else if (item_inventory_detail.State == EntityState.Modified)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                Entry(item_inventory_detail).State = EntityState.Modified;

                NumberOfRecords += 1;
            }
            else if (item_inventory_detail.State == EntityState.Deleted)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                base.item_inventory_detail.Remove(item_inventory_detail);

                NumberOfRecords += 1;
            }

            if (item_inventory_detail.State != EntityState.Unchanged)
            {
                Entry(item_inventory_detail).State = EntityState.Unchanged;
            }
        }

        public bool Approve()
        {
            NumberOfRecords = 0;

            foreach (item_inventory item_inventory in base.item_inventory.Local.Where(x => x.status != Status.Documents.Issued && x.IsSelected))
            {
                //If Value Counted in Null, we undsertand that this has not been counted and will be removed from context.
                if (item_inventory.item_inventory_detail.Where(x => x.value_counted == null).Count() > 0)
                {
                    List<item_inventory_detail> null_detail = item_inventory.item_inventory_detail.Where(x => x.value_counted == null).ToList();
                    base.item_inventory_detail.RemoveRange(null_detail);
                }

                if (item_inventory.id_inventory == 0)
                {
                    SaveChanges();
                }

                List<item_movement> item_movementLIST = new List<item_movement>();
              
                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                item_movementLIST = _Stock.insert_Stock(this, item_inventory);

                if (item_movementLIST.Count() > 0)
                {
                    item_movement.AddRange(item_movementLIST);
                }

                base.item_movement.AddRange(item_movementLIST);

                item_inventory.status = Status.Documents.Issued;

                SaveChanges();
            }

            return true;
        }
    }
}



