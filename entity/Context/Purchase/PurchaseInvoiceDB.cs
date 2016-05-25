using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class PurchaseInvoiceDB : BaseDB
    {
  
        public purchase_invoice New()
        {
            purchase_invoice purchase_invoice = new purchase_invoice();

            //purchase_invoice.id_range = Brillo.GetDefault.Range(App.Names.PurchaseInvoice);
            purchase_invoice.status = Status.Documents_General.Pending;
            purchase_invoice.trans_date = DateTime.Now;

            purchase_invoice.State = EntityState.Added;
            purchase_invoice.IsSelected = true;
            base.Entry(purchase_invoice).State = EntityState.Added;
            return purchase_invoice;
        }

        public override int SaveChanges()
        {
            validate_Invoice();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Invoice();
            return base.SaveChangesAsync();
        }

        private void validate_Invoice()
        {
            NumberOfRecords = 0;

            foreach (purchase_invoice purchase_invoice in base.purchase_invoice.Local)
            {
                if (purchase_invoice.IsSelected && purchase_invoice.Error == null)
                {
                    if (purchase_invoice.State == EntityState.Added)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        Entry(purchase_invoice).State = EntityState.Added;
                    }
                    else if (purchase_invoice.State == EntityState.Modified)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        Entry(purchase_invoice).State = EntityState.Modified;
                    }
                    else if (purchase_invoice.State == EntityState.Deleted)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        base.purchase_invoice.Remove(purchase_invoice);
                    }
                    NumberOfRecords += 1;
                }
                else if (purchase_invoice.State > 0)
                {
                    if (purchase_invoice.State != EntityState.Unchanged)
                    {
                        Entry(purchase_invoice).State = EntityState.Unchanged;
                    }
                }
            }
        }
        
        public void Approve()
        {
            foreach (purchase_invoice invoice in base.purchase_invoice.Local.Where(x => x.IsSelected == true))
            {
                if (invoice.Error == null)
                {
                    if (invoice.id_purchase_invoice == 0)
                    {
                        SaveChanges();
                    }

                    if (invoice.status != Status.Documents_General.Approved)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        ///Insert Payment Schedual Logic
                        payment_schedualList = _Payment.insert_Schedual(invoice);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        ///Insert Stock Logic
                        item_movementList = _Stock.insert_Stock(this, invoice);

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

                        invoice.status = Status.Documents_General.Approved;

                        SaveChanges();
                    }
                }
                else if (invoice.Error != null)
                {
                    invoice.HasErrors = true;
                }
            }
        }

        public void Anull()
        {
            foreach (purchase_invoice purchase_invoice in base.purchase_invoice.Local)
            {
                if (purchase_invoice.IsSelected && purchase_invoice.Error == null)
                {
                    if (purchase_invoice.accounting_journal == null || 
                        purchase_invoice.purchase_return.Count() == 0)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.revert_Schedual(purchase_invoice);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.revert_Stock(this, App.Names.PurchaseInvoice, purchase_invoice.id_purchase_invoice);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            base.payment_schedual.RemoveRange(payment_schedualList);
                        }
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            base.item_movement.RemoveRange(item_movementList);
                        }

                        purchase_invoice.status = Status.Documents_General.Annulled;
                        SaveChanges();
                    }
                }
            }
        }
    }
}
