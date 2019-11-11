using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class PurchasePackingListDB : BaseDB
    {
        public purchase_packing New()
        {
            purchase_packing purchase_packing = new purchase_packing();
            purchase_packing.State = EntityState.Added;
            purchase_packing.status = Status.Documents_General.Pending;

            app_document_range app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.PackingList, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
            if (app_document_range != null)
            {
                purchase_packing.id_range = app_document_range.id_range;
            }

            purchase_packing.IsSelected = true;
            purchase_packing.app_branch = app_branch.Find(CurrentSession.Id_Branch);
            return purchase_packing;
        }

        public override int SaveChanges()
        {
            validate_PackingList();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_PackingList();
            return base.SaveChangesAsync();
        }

        private void validate_PackingList()
        {
            foreach (purchase_packing purchase_packing in base.purchase_packing.Local)
            {
                if (purchase_packing.IsSelected && purchase_packing.Error == null)
                {
                    if (purchase_packing.State == EntityState.Added)
                    {
                        purchase_packing.timestamp = DateTime.Now;
                        purchase_packing.State = EntityState.Unchanged;
                        Entry(purchase_packing).State = EntityState.Added;
                    }
                    else if (purchase_packing.State == EntityState.Modified)
                    {
                        purchase_packing.timestamp = DateTime.Now;
                        purchase_packing.State = EntityState.Unchanged;
                        Entry(purchase_packing).State = EntityState.Modified;
                    }
                    else if (purchase_packing.State == EntityState.Deleted)
                    {
                        purchase_packing.timestamp = DateTime.Now;
                        purchase_packing.is_head = false;
                        purchase_packing.State = EntityState.Deleted;
                        Entry(purchase_packing).State = EntityState.Modified;
                    }
                }
                else if (purchase_packing.State > 0)
                {
                    if (purchase_packing.State != EntityState.Unchanged)
                    {
                        Entry(purchase_packing).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve()
        {
            NumberOfRecords = 0;

            foreach (purchase_packing purchase_packing in
                base.purchase_packing.Local
                .Where(x =>
                x.IsSelected &&
                x.Error == null
                ))
            {
                if (purchase_packing.id_purchase_packing == 0)
                {
                    try
                    {
                        SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                   
                }

                if (purchase_packing.status != Status.Documents_General.Approved)
                {
                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    List<item_movement> item_movementList = new List<item_movement>();
                    item_movementList = _Stock.PurchasePacking_Approve(this, purchase_packing);

                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        item_movement.AddRange(item_movementList);
                    }

                    if (purchase_packing.number == null && purchase_packing.id_range > 0)
                    {
                        Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == purchase_packing.id_branch).Select(x => x.code).FirstOrDefault();

                        if (purchase_packing.app_terminal != null)
                        { Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == purchase_packing.app_terminal.id_terminal).Select(x => x.code).FirstOrDefault(); }

                        app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == purchase_packing.id_range).FirstOrDefault();
                        purchase_packing.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                        purchase_packing.RaisePropertyChanged("number");
                    }

                    purchase_packing.status = Status.Documents_General.Approved;
                    try
                    {
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                    }
                  
                }

                NumberOfRecords += 1;
            }
        }

        public void Annull()
        {
            foreach (purchase_packing purchase_packing in base.purchase_packing.Local)
            {
                if (purchase_packing.IsSelected && purchase_packing.Error == null)
                {
                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    List<item_movement> item_movementList = new List<item_movement>();
                    item_movementList = _Stock.revert_Stock(this, App.Names.PurchasePacking, purchase_packing);

                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        base.item_movement.RemoveRange(item_movementList);
                    }

                    foreach (purchase_packing_detail purchase_packing_detail in purchase_packing.purchase_packing_detail)
                    {
                        purchase_packing_detail.id_purchase_order_detail = null;
                    }

                    purchase_packing.status = Status.Documents_General.Annulled;
                    SaveChanges();
                }
            }
        }
    }
}