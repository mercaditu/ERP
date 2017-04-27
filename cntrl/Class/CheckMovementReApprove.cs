using entity;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace cntrl.Class
{
    public class CheckMovementReApprove
    {
        public enum Messagess
        {
            QuantityIncreased,
            QuantityDecreased,
            UnitPriceChanged,
            ItemDeleted,
            DateChanged
        }

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
        public bool CheckQuantityUP(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                using (db temp = new db())
                {
                    sales_invoice OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                        if (Oldsales_invoice_detail != null)
                        {
                            if (sales_invoice_detail.quantity > Oldsales_invoice_detail.quantity)
                            {
                                decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                                if (Diff > 0)
                                {
                                    foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                                    {
                                        return true;
                                        //movmessage += item_movement.debit + "-->" + sales_invoice_detail.quantity;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                using (db temp = new db())
                {
                    purchase_invoice OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();
                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);

                    foreach (purchase_invoice_detail purchase_invoice_detail in Local_PurchaseInvoice.purchase_invoice_detail)
                    {
                        purchase_invoice_detail Oldpurchase_invoice_detail = OriginalPurchaseInvoice.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                        if (Oldpurchase_invoice_detail != null)
                        {
                            if (purchase_invoice_detail.quantity > Oldpurchase_invoice_detail.quantity)
                            {
                                decimal Diff = purchase_invoice_detail.quantity - Oldpurchase_invoice_detail.quantity;
                                if (Diff > 0)
                                {
                                    foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                                    {
                                        return true;
                                        //movmessage += item_movement.credit + "-->" + purchase_invoice_detail.quantity;
                                    }
                                }
                            }
                        }
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
        public bool CheckQuantityDown(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                using (db temp = new db())
                {
                    sales_invoice OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();

                        if (Oldsales_invoice_detail != null)
                        {
                            if (sales_invoice_detail.quantity < Oldsales_invoice_detail.quantity)
                            {
                                decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                                if (Diff < 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);
                    foreach (purchase_invoice_detail purchase_invoice_detail in Local_PurchaseInvoice.purchase_invoice_detail)
                    {
                        purchase_invoice_detail Oldid_purchase_invoice_detail = OriginalPurchaseInvoice.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();

                        if (Oldid_purchase_invoice_detail != null)
                        {
                            if (purchase_invoice_detail.quantity < Oldid_purchase_invoice_detail.quantity)
                            {
                                decimal Diff = purchase_invoice_detail.quantity - Oldid_purchase_invoice_detail.quantity;
                                if (Diff < 0)
                                {
                                    foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
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
        public bool CheckDateChange(db db, int ID, App.Names Application)
        {
            //string movmessage = "";
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    if (OriginalSalesInvoice.trans_date != Local_SalesInvoice.trans_date)
                    {
                        return true;
                        //movmessage += OriginalSalesInvoice.trans_date + "-->" + Local_SalesInvoice.trans_date;
                    }

                    //if (movmessage != "")
                    //{
                    //    String Message = "You Have Changed The Date So Following Changes Required..\n";
                    //    Message += "This Movement Will be Changed..\n" + movmessage;
                    //    return Message;
                    //}
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);
                    if (OriginalPurchaseInvoice.trans_date != Local_PurchaseInvoice.trans_date)
                    {
                        return true;
                        //movmessage += OriginalPurchaseInvoice.trans_date + "-->" + Local_PurchaseInvoice.trans_date;
                    }

                    //if (movmessage != "")
                    //{
                    //    String Message = "You Have Changed The Date So Following Changes Required..\n";
                    //    Message += "This Movement Will be Changed..\n" + movmessage;
                    //    return Message;
                    //}
                }
            }

            return false;
        }


        public bool CheckNewMovement(db db, int ID, entity.App.Names Application)
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
        public bool CheckDeleteMovement(db db, int ID, entity.App.Names Application)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">Context</param>
        /// <param name="ID">Transaction ID of Header</param>
        /// <param name="Application">Application Name</param>
        /// <returns></returns>
        public bool CheckDateMovement(db db, int ID, App.Names Application)
        {
            using (db temp = new db())
            {
                sales_invoice Oldsales_invoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);

                if (sales_invoice.id_branch != Oldsales_invoice.id_branch)
                {
                    return true;
                }
            }

            return false;
        }
    }
}