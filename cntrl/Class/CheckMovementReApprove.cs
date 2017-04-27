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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ID"></param>
        /// <param name="Application"></param>
        /// <returns></returns>
        public string CheckValueChange(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;
                string movmessage = "";

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                        if (sales_invoice_detail.unit_price != Oldsales_invoice_detail.unit_price)
                        {
                            foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                            {
                                item_movement_value item_movement_value = item_movement.item_movement_value.FirstOrDefault();
                                if (item_movement_value != null)
                                {
                                    movmessage += item_movement_value.unit_value + "-->" + sales_invoice_detail.unit_price;
                                }
                            }
                        }
                    }
                }

                if (movmessage != "")
                {
                    String Message = "You Have Changed The Date So Following Changes Required..\n";
                    Message += "This Movement Will be Changed..\n" + movmessage;
                    return movmessage;
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;
                string movmessage = "";
                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurcahseInvoice = db.purchase_invoice.Find(ID);

                    foreach (purchase_invoice_detail purchase_invoice_detail in Local_PurcahseInvoice.purchase_invoice_detail)
                    {
                        purchase_invoice_detail Oldpurchase_invoice_detail = OriginalPurchaseInvoice.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                        if (purchase_invoice_detail.unit_cost != Oldpurchase_invoice_detail.unit_cost)
                        {
                            foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                            {
                                item_movement_value item_movement_value = item_movement.item_movement_value.FirstOrDefault();
                                if (item_movement_value != null)
                                {
                                    movmessage += item_movement_value.unit_value + "-->" + purchase_invoice_detail.unit_cost;
                                }
                            }
                        }
                    }
                }
                if (movmessage != "")
                {
                    String Message = "You Have Changed The Date So Following Changes Required..\n";
                    Message += "This Movement Will be Changed..\n" + movmessage;
                    return movmessage;
                }
            }

            return "";
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