using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq; 
using System.Threading.Tasks;

namespace entity
{
    public partial class PurchaseReturnDB : BaseDB
    {
        public purchase_return New()
        {
            purchase_return purchase_return = new purchase_return();

            purchase_return.id_range = Brillo.GetDefault.Range(App.Names.PurchaseInvoice);
            purchase_return.status = Status.Documents_General.Pending;
            purchase_return.trans_date = DateTime.Now;

            purchase_return.State = EntityState.Added;
            purchase_return.IsSelected = true;

            return purchase_return;
        }

        public override int SaveChanges()
        {
            validate_Return();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Return();
            return base.SaveChangesAsync();
        }


        private void validate_Return()
        {
            foreach (purchase_return purchase_return in base.purchase_return.Local)
            {
                if (purchase_return.IsSelected && purchase_return.Error == null)
                {
                    if (purchase_return.State == EntityState.Added)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        Entry(purchase_return).State = EntityState.Added;
                    }
                    else if (purchase_return.State == EntityState.Modified)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        Entry(purchase_return).State = EntityState.Modified;
                    }
                    else if (purchase_return.State == EntityState.Deleted)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        base.purchase_return.Remove(purchase_return);
                    }
                }
                else if (purchase_return.State > 0)
                {
                    if (purchase_return.State != EntityState.Unchanged)
                    {
                        Entry(purchase_return).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve()
        {
            foreach (purchase_return purchase_return in base.purchase_return.Local.Where(x => x.IsSelected == true))
            {
                if (purchase_return.Error == null)
                {
                    if (purchase_return.id_purchase_return == 0)
                    {
                        SaveChanges();
                    }

                    if (purchase_return.status != Status.Documents_General.Approved)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.insert_Schedual(purchase_return);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(this, purchase_return);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

                        purchase_return.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }
                }
                else if (purchase_return.Error != null)
                {
                    purchase_return.HasErrors = true;
                }
            }
        }

        public void Anull()
        {
            foreach (purchase_return purchase_return in base.purchase_return.Local)
            {
                if (purchase_return.IsSelected && purchase_return.Error == null)
                {
                    List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                    Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                    payment_schedualList = _Payment.revert_Schedual(purchase_return);

                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    List<item_movement> item_movementList = new List<item_movement>();
                    item_movementList = _Stock.revert_Stock(this, App.Names.PurchaseReturn, purchase_return.id_purchase_return);

                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        base.payment_schedual.AddRange(payment_schedualList);
                    }
                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        base.item_movement.RemoveRange(item_movementList);
                    }
                }
            }
        }
    }
}
