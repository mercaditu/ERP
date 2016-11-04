using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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

        public async void Generate_ProductMovement()
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
                    ).AsNoTracking().ToList();

            foreach (purchase_invoice purchase in purchaseLIST.OrderBy(y => y.trans_date))
            {
                using (PurchaseInvoiceDB PurchaseDB = new PurchaseInvoiceDB())
                {
                    purchase.IsSelected = true;
                    PurchaseDB.Insert_Items_2_Movement(purchase);
                    PurchaseDB.SaveChanges();
                }
            }

            using (ImpexDB ImpexDB = new ImpexDB())
            {
                List<impex> impexLIST = ImpexDB.impex.Local.Where(x => x.status == Status.Documents_General.Approved).ToList();

                foreach (impex impex in impexLIST)
                {
                    impex.IsSelected = true;
                    impex.status = Status.Documents_General.Pending;
                    ImpexDB.ApproveImport();
                }
            }

            DateTime StartDate = DateTime.Now.AddMonths(-7);
            DateTime EndDate = DateTime.Now;

            List<item_inventory> item_inventoryList = base.item_inventory.Where(x => x.id_company == CurrentSession.Id_Company && x.status == Status.Documents.Issued).AsNoTracking().ToList();
            List<item_transfer> item_transferList = base.item_transfer.Where(x => x.id_company == CurrentSession.Id_Company).AsNoTracking().ToList();

            foreach (DateTime day in EachDay(StartDate, EndDate))
            {
                if (item_inventoryList.Any(x => x.trans_date.Date == day.Date))
                {
                    ///Inventory
                    using (InventoryDB InventoryDB = new InventoryDB())
                    {
                        List<item_inventory> item_inventoryLIST = await InventoryDB.item_inventory.Where(x =>
                                x.id_company == CurrentSession.Id_Company &&
                                x.status == Status.Documents.Issued).OrderBy(y => y.trans_date).ToListAsync();

                        foreach (item_inventory inventory in item_inventoryLIST.Where(x => x.trans_date == day.Date))
                        {
                            if (inventory.status == Status.Documents.Issued)
                            {
                                inventory.status = Status.Documents.Pending;
                                inventory.IsSelected = true;
                            }

                            InventoryDB.Approve();
                        }
                    }
                }

                if (item_transferList.Any(x => x.trans_date.Date == day.Date))
                {
                    ///Transfers & Movement
                    using (ProductTransferDB ProductTransferDB = new ProductTransferDB())
                    {
                        List<item_transfer> item_transferLIST = await ProductTransferDB.item_transfer.Where(x => 
                            x.id_company == CurrentSession.Id_Company &&
                            x.status == Status.Transfer.Approved).OrderBy(y => y.trans_date).ToListAsync();

                        foreach (item_transfer transfer in item_transferLIST.Where(x => x.trans_date == day.Date))
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
            }


            List<sales_invoice> sales_invoiceLIST = base.sales_invoice.Where(x => x.id_company == CurrentSession.Id_Company && x.status == Status.Documents_General.Approved).ToList();
            int Count = sales_invoiceLIST.Count() / 100;

            for (int i = 0; i < Count; i++)
            {
                foreach (sales_invoice sales in sales_invoiceLIST.OrderBy(y => y.trans_date))
                {
                    ///Sales
                    using (SalesInvoiceDB SalesInvoiceDB = new SalesInvoiceDB())
                    {
                        sales_invoice sales_invoice = SalesInvoiceDB.sales_invoice.Find(sales.id_sales_invoice);
                        SalesInvoiceDB.Insert_Items_2_Movement(sales_invoice);

                        SalesInvoiceDB.SaveChanges();
                    }
                }
            }
        }
    }
}
