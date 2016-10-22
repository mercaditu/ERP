using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class PurchaseOrderDB : BaseDB
    {
        public purchase_order New()
        {
            purchase_order purchase_order = new purchase_order();
            purchase_order.State = EntityState.Added;
            purchase_order.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.PurchaseOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();

            purchase_order.status = Status.Documents_General.Pending;
            purchase_order.trans_date = DateTime.Now;
            purchase_order.app_branch = app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault();
            purchase_order.IsSelected = true;
            
            return purchase_order;
        }

        public override int SaveChanges()
        {
            validate_Order();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Order();
            return base.SaveChangesAsync();
        }

        private void validate_Order()
        {
            NumberOfRecords = 0;

            foreach (purchase_order purchase_order in base.purchase_order.Local)
            {
                if (purchase_order.IsSelected && purchase_order.Error == null)
                {
                    if (purchase_order.State == EntityState.Added)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        Entry(purchase_order).State = EntityState.Added;
                    }
                    else if (purchase_order.State == EntityState.Modified)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        Entry(purchase_order).State = EntityState.Modified;
                    }
                    else if (purchase_order.State == EntityState.Deleted)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        base.purchase_order.Remove(purchase_order);
                    }

                    NumberOfRecords += 1;

                }
                else if (purchase_order.State > 0)
                {
                    if (purchase_order.State != EntityState.Unchanged)
                    {
                        Entry(purchase_order).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public bool Approve()
        {
            NumberOfRecords = 0;

            foreach (purchase_order purchase_order in base.purchase_order.Local.Where(x => x.IsSelected == true))
            {
                if (purchase_order.Error == null)
                {
                    if (purchase_order.id_purchase_order == 0)
                    {
                        SaveChanges();
                    }

                    if (purchase_order.status != Status.Documents_General.Approved)
                    {
                        if (purchase_order.number == null && purchase_order.id_range != null)
                        {
                            //Brillo.Logic.Document _Document = new Brillo.Logic.Document();
                            Brillo.Logic.Range.branch_Code = base.app_branch.Where(x => x.id_branch == purchase_order.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = base.app_terminal.Where(x => x.id_terminal == purchase_order.id_terminal).FirstOrDefault().code;
                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == purchase_order.id_range).FirstOrDefault();
                            purchase_order.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            purchase_order.RaisePropertyChanged("number");
                            purchase_order.is_issued = true;
                        }
                        else
                        {
                            purchase_order.is_issued = false;
                        }

                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.insert_Schedual(purchase_order);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(this, purchase_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

                        purchase_order.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }

                    if (purchase_order.is_issued)
                    {
                        app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == purchase_order.id_range).FirstOrDefault();
                        Brillo.Document.Start.Automatic(purchase_order, app_document_range);
                    }

                    NumberOfRecords += 1;
                    purchase_order.IsSelected = false;

                }
                else if (purchase_order.Error != null)
                {
                    purchase_order.HasErrors = true;
                }
            }

            return true;
        }

        public bool Anull()
        {
            NumberOfRecords = 0;
            foreach (purchase_order purchase_order in base.purchase_order.Local)
            {
                if (purchase_order.IsSelected && purchase_order.Error == null)
                {
                    if (purchase_order.purchase_invoice.Count() == 0)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.revert_Schedual(purchase_order);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.revert_Stock(this, App.Names.PurchaseOrder, purchase_order.id_purchase_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            base.payment_schedual.RemoveRange(payment_schedualList);
                        }

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            base.item_movement.RemoveRange(item_movementList);
                        }

                        purchase_order.status = Status.Documents_General.Annulled;
                        SaveChanges();

                        //Clean Up
                        purchase_order.IsSelected = false;
                        NumberOfRecords += 1;
                    }
                }
            }
            return true;
        }
    }
}
