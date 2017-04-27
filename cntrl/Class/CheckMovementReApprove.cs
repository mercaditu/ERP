using entity;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace cntrl.Class
{
    public class Approve_Check
    {
        public enum Messagess
        {
            QuantityIncreased,
            QuantityDecreased,
            UnitPriceChanged,
            ItemDeleted,
            DateChanged
        }

        public Approve_Check()
        {
            //Movement
           // Approve_Check CheckMovementReApprove = new Approve_Check();

            //Check for Quantity Up
            //if (QuantityUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //{
            //    new UpdateMovementReApprove().QuantityUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //}

            ////Check for Quantity Down
            //if (QuantityDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //{
            //    new UpdateMovementReApprove().QuantityDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //}

            ////Checks for Date Changes
            //if (DateChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //{
            //    new UpdateMovementReApprove().DateChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //}

            ////Checks for New Detail Insertions
            //if (CreateDetail(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //{
            //    new UpdateMovementReApprove().NewMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //}

            ////Check if Item has been Removed
            //if (RemovedDetail(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //{
            //    new UpdateMovementReApprove().DeleteMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ID"></param>
        /// <param name="Application"></param>
        /// <returns></returns>
        public bool CheckPriceChange(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);

                foreach (var detail in sales_invoice.sales_invoice_detail)
                {
                    decimal CurrentPrice = db.Entry(detail).Property(u => u.unit_price).CurrentValue;
                    decimal OriginalPrice = db.Entry(detail).Property(u => u.unit_price).OriginalValue;
                    decimal Difference = OriginalPrice - CurrentPrice;
                    //Current - Original if it's 
                    if (Difference != 0)
                    {
                        return true;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = db.purchase_invoice.Find(ID);

                foreach (var detail in purchase_invoice.purchase_invoice_detail)
                {
                    decimal CurrentPrice = db.Entry(detail).Property(u => u.quantity).CurrentValue;
                    decimal OriginalPrice = db.Entry(detail).Property(u => u.quantity).OriginalValue;
                    decimal Difference = OriginalPrice - CurrentPrice;
                    //Current - Original if it's 
                    if (Difference != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">Context</param>
        /// <param name="ID">Transaction ID of Header</param>
        /// <param name="Application">Application Name</param>
        /// <returns></returns>
        public bool QuantityUP(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);

                foreach (var detail in sales_invoice.sales_invoice_detail)
                {
                    decimal CurrentQuantity = db.Entry(detail).Property(u => u.quantity).CurrentValue;
                    decimal OriginalQuantity = db.Entry(detail).Property(u => u.quantity).OriginalValue;
                    decimal Difference = OriginalQuantity - CurrentQuantity;
                    //Current - Original if it's 
                    if (Difference > 0)
                    {
                        return true;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = db.purchase_invoice.Find(ID);

                foreach (var detail in purchase_invoice.purchase_invoice_detail)
                {
                    decimal CurrentQuantity = db.Entry(detail).Property(u => u.quantity).CurrentValue;
                    decimal OriginalQuantity = db.Entry(detail).Property(u => u.quantity).OriginalValue;
                    decimal Difference = OriginalQuantity - CurrentQuantity;
                    //Current - Original if it's 
                    if (Difference > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks for Quantity Down (Decrease) in Sales or Purchases. If Quantity has been decreased, Returns True. Else False.
        /// </summary>
        /// <param name="db">Context</param>
        /// <param name="ID">Transaction ID of Header</param>
        /// <param name="Application">Application Name</param>
        /// <returns></returns>
        public bool QuantityDown(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);

                foreach (var detail in sales_invoice.sales_invoice_detail)
                {
                    decimal CurrentQuantity = db.Entry(detail).Property(u => u.quantity).CurrentValue;
                    decimal OriginalQuantity = db.Entry(detail).Property(u => u.quantity).OriginalValue;
                    decimal Difference = OriginalQuantity - CurrentQuantity;
                    //Current - Original if it's 
                    if (Difference < 0)
                    {
                        return true;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = db.purchase_invoice.Find(ID);

                foreach (var detail in purchase_invoice.purchase_invoice_detail)
                {
                    decimal CurrentQuantity = db.Entry(detail).Property(u => u.quantity).CurrentValue;
                    decimal OriginalQuantity = db.Entry(detail).Property(u => u.quantity).OriginalValue;
                    decimal Difference = OriginalQuantity - CurrentQuantity;
                    //Current - Original if it's 
                    if (Difference < 0)
                    {
                        return true;
                    }
                }
            }

            //Return False if not true.
            return false;
        }

        /// <summary>
        /// Checks for changes in Date affecting Stock Movement due to Sales Or Purchases. IF Change of Date, then Returns True else False
        /// </summary>
        /// <param name="db">Context</param>
        /// <param name="ID">Transaction ID of Header</param>
        /// <param name="Application">Application Name</param>
        /// <returns></returns>
        public bool DateChange(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);
                DateTime CurrentDate = db.Entry(sales_invoice).Property(u => u.trans_date).CurrentValue;
                DateTime OriginalDate = db.Entry(sales_invoice).Property(u => u.trans_date).OriginalValue;

                if (CurrentDate.Date != OriginalDate.Date)
                {
                    return true;
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = db.purchase_invoice.Find(ID);
                DateTime CurrentDate = db.Entry(purchase_invoice).Property(u => u.trans_date).CurrentValue;
                DateTime OriginalDate = db.Entry(purchase_invoice).Property(u => u.trans_date).OriginalValue;

                if (CurrentDate.Date != OriginalDate.Date)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ID"></param>
        /// <param name="Application"></param>
        /// <returns></returns>
        public bool CreateDetail(db db, int ID, App.Names Application)
        {
            DbChangeTracker Tracker = db.ChangeTracker;
            var entries = Tracker.Entries<sales_invoice_detail>().Where(x => x.Entity.id_sales_invoice == ID);

            foreach (var entity in entries)
            {
                if (entity.State == System.Data.Entity.EntityState.Added)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">Context</param>
        /// <param name="ID">Transaction ID of Header</param>
        /// <param name="Application">Application Name</param>
        /// <returns></returns>
        public bool RemovedDetail(db db, int ID, App.Names Application)
        {
            DbChangeTracker Tracker = db.ChangeTracker;
            var entries = Tracker.Entries<sales_invoice_detail>().Where(x => x.Entity.id_sales_invoice == ID);

            foreach (var entity in entries)
            {
                if (entity.State == System.Data.Entity.EntityState.Deleted)
                {
                    return true;
                }
            }

            return false;
        }
    }
}