using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class ProductMovementDB : BaseDB
    {
        public override int SaveChanges()
        {
            //validate_ProductMovement();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            //validate_ProductMovement();
            return base.SaveChangesAsync();
        }

        private void validate_ProductMovement()
        {
            foreach (item_movement item_movement in base.item_movement.Local)
            {
                if (item_movement.IsSelected)
                {
                    if (item_movement.State == EntityState.Added)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        Entry(item_movement).State = EntityState.Added;
                    }
                    else if (item_movement.State == EntityState.Modified)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        Entry(item_movement).State = EntityState.Modified;
                    }
                    else if (item_movement.State == EntityState.Deleted)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        base.item_movement.Remove(item_movement);
                    }
                }
                else if (item_movement.State > 0)
                {
                    if (item_movement.State != EntityState.Unchanged)
                    {
                        Entry(item_movement).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public void Generate_ProductMovement()
        {
            ///Delete all Movements.
            if (base.item_movement.Where(x => x.id_company == CurrentSession.Id_Company).Count() > 0)
            {
                base.item_movement.RemoveRange(base.item_movement.Where(x => x.id_company == CurrentSession.Id_Company).ToList());
                base.SaveChanges();
            }

            ///Purchase
            List<purchase_invoice> purchaseLIST = purchase_invoice
                .Where(x =>
                    x.id_company == CurrentSession.Id_Company &&
                    x.status == Status.Documents_General.Approved
                    ).ToList();

            foreach (purchase_invoice purchase in purchaseLIST.OrderBy(y => y.trans_date))
            {
                using (PurchaseInvoiceDB PurchaseDB = new PurchaseInvoiceDB())
                {
                    PurchaseDB.Insert_Items_2_Movement(purchase);
                    PurchaseDB.SaveChanges();
                }
            }

            DateTime StartDate = DateTime.Now.AddMonths(-5);
            DateTime EndDate = DateTime.Now;

            foreach (DateTime day in EachDay(StartDate, EndDate))
            {
                ///Inventory
                using (InventoryDB InventoryDB = new InventoryDB())
                {
                    List<item_inventory> item_inventoryLIST = InventoryDB.item_inventory.Where(x =>
                            x.id_company == CurrentSession.Id_Company && x.trans_date.Day == day.Day && x.trans_date.Month == day.Month &&
                            x.status == Status.Documents.Issued).OrderBy(y => y.trans_date).ToList();
                    foreach (item_inventory inventory in item_inventoryLIST)
                    {
                        if (inventory.status == Status.Documents.Issued)
                        {
                            inventory.status = Status.Documents.Pending;
                            inventory.IsSelected = true;
                        }

                        foreach (item_inventory_detail item_inventory_detail in inventory.item_inventory_detail)
                        {
                            if (item_inventory_detail.value_counted > 0)
                            {
                                item_inventory_detail.IsSelected = true;
                            }
                        }
                        InventoryDB.Approve();
                    }
                }

                
                ///Transfers & Movement
                using (ProductTransferDB ProductTransferDB = new ProductTransferDB())
                {
                    List<item_transfer> item_transferLIST = ProductTransferDB.item_transfer.Where(x => x.id_company == CurrentSession.Id_Company 
                        && x.trans_date.Day == day.Day && x.trans_date.Month == day.Month
                        && x.status == Status.Transfer.Approved).ToList();
                    foreach (item_transfer transfer in item_transferLIST.OrderBy(y => y.trans_date))
                    {
                        transfer.IsSelected = true;
                        foreach (item_transfer_detail detail in transfer.item_transfer_detail)
                        {
                            detail.status = Status.Documents_General.Pending;
                            detail.IsSelected = true;
                        }

                        ProductTransferDB.SaveChanges();
                        ProductTransferDB.ApproveOrigin(transfer.app_branch_origin.id_branch, transfer.app_branch_destination.id_branch, false);
                        ProductTransferDB.ApproveDestination(transfer.app_branch_origin.id_branch, transfer.app_branch_destination.id_branch, false);

                        transfer.IsSelected = false;
                    }
                }
            }


            List<sales_invoice> sales_invoiceLIST = base.sales_invoice.Where(x => x.id_company == CurrentSession.Id_Company && x.status == Status.Documents_General.Approved).ToList();
            foreach (sales_invoice sales in sales_invoiceLIST.OrderBy(y => y.trans_date))
            {
                ///Sales
                using (SalesInvoiceDB SalesInvoiceDB = new SalesInvoiceDB())
                {
                    sales_invoice sales_invoice = SalesInvoiceDB.sales_invoice.Where(x => x.id_sales_invoice == sales.id_sales_invoice).FirstOrDefault();

                    SalesInvoiceDB.Insert_Items_2_Movement(sales_invoice);
                    SalesInvoiceDB.SaveChanges();
                }
            }
        }
    }
}
