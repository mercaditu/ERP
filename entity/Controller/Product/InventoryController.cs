using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Controller.Product
{
    public class InventoryController : Base
    {
        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (item_inventory item_inventory in db.item_inventory.Local.Where(x => x.IsSelected))
            {


                if (item_inventory.State == EntityState.Added)
                {
                    //If Value Counted in Null, we undsertand that this has not been counted and will be removed from context.
                    if (item_inventory.item_inventory_detail.Where(x => x.value_counted == null).Count() > 0)
                    {
                        List<item_inventory_detail> null_detail = item_inventory.item_inventory_detail.Where(x => x.value_counted == null).ToList();
                        db.item_inventory_detail.RemoveRange(null_detail);
                    }
                    item_inventory.timestamp = DateTime.Now;
                    item_inventory.State = EntityState.Unchanged;
                    db.Entry(item_inventory).State = EntityState.Added;
                    item_inventory.IsSelected = false;
                }
                else if (item_inventory.State == EntityState.Modified)
                {
                    item_inventory.timestamp = DateTime.Now;
                    item_inventory.State = EntityState.Unchanged;
                    db.Entry(item_inventory).State = EntityState.Modified;
                    item_inventory.IsSelected = false;
                }
                NumberOfRecords += 1;



            }

            foreach (var error in db.GetValidationErrors())
            {
                db.Entry(error.Entry.Entity).State = EntityState.Detached;
            }

            if (db.GetValidationErrors().Count() > 0)
            {
                return false;
            }
            else
            {
                db.SaveChanges();
                return true;
            }
        }
        public async void Load()
        {
            await db.item_inventory.Where(a => a.id_company == CurrentSession.Id_Company).Include(x => x.item_inventory_detail).OrderByDescending(x => x.trans_date).LoadAsync();

            await db.app_branch
            .Where(a => a.is_active == true
                && a.can_stock == true
                && a.id_company == CurrentSession.Id_Company).Include(x => x.app_location)
            .OrderBy(a => a.name).LoadAsync();
        }

        public bool Approve()
        {
            NumberOfRecords = 0;
            db.SaveChanges();

            foreach (item_inventory item_inventory in db.item_inventory.Local.Where(x => x.status != Status.Documents.Issued && x.IsSelected))
            {



                List<item_movement> item_movementLIST = new List<item_movement>();

                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                item_movementLIST = _Stock.Inventory_Approve(db, item_inventory);

                if (item_movementLIST.Count() > 0)
                {
                    db.item_movement.AddRange(item_movementLIST);
                }

                db.item_movement.AddRange(item_movementLIST);

                item_inventory.status = Status.Documents.Issued;

                db.SaveChanges();
            }

            return true;
        }
    }
}
