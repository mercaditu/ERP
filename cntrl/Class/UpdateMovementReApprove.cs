using entity;
using entity.Brillo;
using System.Collections.Generic;
using System.Linq;

namespace cntrl.Class
{
    public class UpdateMovementReApprove
    {
        /// <summary>
        /// Changes in Value of Item Movement Value.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ID"></param>
        /// <param name="Application"></param>
        public void CheckPriceChange(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);
                    foreach (purchase_invoice_detail purchase_invoice_detail in Local_PurchaseInvoice.purchase_invoice_detail)
                    {
                        foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                        {
                            //Fix child Unit Values.
                            item_movement.Update_ChildVales(purchase_invoice_detail.unit_cost);
                        }
                    }
                }
            }
        }
        
        public void QuantityUP(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice RemoteCopy;

                using (db temp = new db())
                {
                    RemoteCopy = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
                    sales_invoice LocalCopy = db.sales_invoice.Find(ID);

                    foreach (sales_invoice_detail LocalDetail in LocalCopy.sales_invoice_detail)
                    {
                        sales_invoice_detail RemoteDetail = RemoteCopy.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == LocalDetail.id_sales_invoice_detail).FirstOrDefault();
                        if (RemoteDetail != null)
                        {
                            //find the diffrence between actual detail and old detail. If Positive, then Qty is Up. If Negative then Qty is Down.
                            decimal Diff = LocalDetail.quantity - RemoteDetail.quantity;
                            List<item_movement> item_movementlist = LocalDetail.item_movement.ToList();
                            foreach (item_movement item_movement in item_movementlist)
                            {
                                //if parent is null then change the quantity
                                if (item_movement.parent == null)
                                {
                                    item_movement.debit = LocalDetail.quantity;
                                }
                                else
                                {
                                    if (Diff > 0)
                                    {
                                        //if   parent balance qunatity is greater then the diffrence then change the quantity
                                        if ((item_movement.parent.credit - item_movement.parent.child.Sum(x => x.debit)) > Diff)
                                        {
                                            item_movement.debit = LocalDetail.quantity;
                                        }
                                        else
                                        {
                                            //where is new item movement to hold the difference??
                                            //if diffrance is greater then the find the new parent
                                            Stock stock = new Stock();
                                            entity.Brillo.Logic.Stock Stock = new entity.Brillo.Logic.Stock();
                                            List<StockList> Items_InStockLIST = stock.List(LocalDetail.app_location.id_branch, (int)LocalDetail.id_location, LocalDetail.item.item_product.FirstOrDefault().id_item_product);

                                            db.item_movement.AddRange(Stock.DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                                     App.Names.SalesInvoice,
                                                                                    LocalDetail.id_sales_invoice,
                                                                                    LocalDetail.id_sales_invoice_detail,
                                                                                    LocalCopy.id_currencyfx,
                                                                                    LocalDetail.item.item_product.FirstOrDefault(),
                                                                                    (int)LocalDetail.id_location,
                                                                                    Diff,
                                                                                    LocalCopy.trans_date,
                                                                                    "Sales Invoice Fix"
                                                                                    ));
                                            Diff = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_purchase_invoice = db.purchase_invoice.Find(ID);
                    foreach (purchase_invoice_detail purchase_invoice_detail in Local_purchase_invoice.purchase_invoice_detail)
                    {
                        purchase_invoice_detail Oldpurchase_invoice_detail = OriginalPurchaseInvoice.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                        if (Oldpurchase_invoice_detail != null)
                        {
                            //change the quantity from the detail quantity
                            decimal Diff = purchase_invoice_detail.quantity - Oldpurchase_invoice_detail.quantity;
                            foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                            {
                                item_movement.credit = purchase_invoice_detail.quantity;
                            }
                        }
                    }
                }
            }
        }

        public void QuantityDown(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                        if (Oldsales_invoice_detail != null)
                        {
                            //find the diffrence between actual detail and old detail
                            decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;

                            List<item_movement> MovementList = sales_invoice_detail.item_movement.ToList();

                            foreach (item_movement item_movement in MovementList)
                            {
                                if (item_movement.debit >= sales_invoice_detail.quantity)
                                {
                                    item_movement.debit = sales_invoice_detail.quantity;
                                }
                                else
                                {
                                    Diff = item_movement.debit - Diff;
                                    db.item_movement.Remove(item_movement);
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
                        purchase_invoice_detail Oldpurchase_invoice_detail = OriginalPurchaseInvoice.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                        if (Oldpurchase_invoice_detail != null)
                        {
                            decimal Diff = Oldpurchase_invoice_detail.quantity - purchase_invoice_detail.quantity;

                            if (Diff > 0)
                            {
                                List<item_movement> MovementList = purchase_invoice_detail.item_movement.ToList();

                                foreach (item_movement item_movement in MovementList)
                                {
                                    if ((item_movement.credit - item_movement.child.Sum(x => x.debit)) > Diff)
                                    {
                                        item_movement.credit = purchase_invoice_detail.quantity;
                                    }
                                    else
                                    {
                                        foreach (var item in item_movement.child)
                                        {
                                            List<item_movement> item_movementList = db.item_movement.Where(x => x.id_item_product == item_movement.id_item_product && x.id_movement != item_movement.id_movement && x.credit > 0).ToList();
                                            foreach (item_movement _item_movement in item_movementList)
                                            {
                                                if (_item_movement.avlquantity > item.credit)
                                                {
                                                    item.parent = _item_movement;
                                                }
                                                else
                                                {
                                                    item.parent = null;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DateChange(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

                foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                {
                    foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                    {
                        item_movement.trans_date = Local_SalesInvoice.trans_date;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice Local_purchase_invoice = db.purchase_invoice.Find(ID);

                foreach (purchase_invoice_detail purchase_invoice_detail in Local_purchase_invoice.purchase_invoice_detail)
                {
                    foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                    {
                        item_movement.trans_date = Local_purchase_invoice.trans_date;
                    }
                }
            }
        }

        //public void BranchChange(db db, int ID, App.Names Application)
        //{
        //    if (Application == App.Names.SalesInvoice)
        //    {
        //        sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

        //        foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
        //        {
        //            foreach (item_movement item_movement in sales_invoice_detail.item_movement)
        //            {
        //                item_movement.trans_date = Local_SalesInvoice.trans_date;
        //            }
        //        }
        //    }
        //    else if (Application == App.Names.PurchaseInvoice)
        //    {
        //        purchase_invoice Local_purchase_invoice = db.purchase_invoice.Find(ID);

        //        foreach (purchase_invoice_detail purchase_invoice_detail in Local_purchase_invoice.purchase_invoice_detail)
        //        {
        //            foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
        //            {
        //                item_movement.trans_date = Local_purchase_invoice.trans_date;
        //            }
        //        }
        //    }
        //}

        //add new item movemt for the new detail inserted
        public void NewMovement(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                        if (Oldsales_invoice_detail == null)
                        {
                            Stock stock = new Stock();
                            entity.Brillo.Logic.Stock Stock = new entity.Brillo.Logic.Stock();
                            if (sales_invoice_detail.id_location == null)
                            {
                                sales_invoice_detail.id_location = Stock.FindNFix_Location(sales_invoice_detail.item.item_product.FirstOrDefault(), sales_invoice_detail.app_location, Local_SalesInvoice.app_branch);
                                sales_invoice_detail.app_location = db.app_location.Find(sales_invoice_detail.id_location);
                            }
                            else
                            {
                                sales_invoice_detail.app_location = db.app_location.Find(sales_invoice_detail.id_location);
                            }

                            List<StockList> Items_InStockLIST = stock.List(sales_invoice_detail.app_location.id_branch, (int)sales_invoice_detail.id_location, sales_invoice_detail.item.item_product.FirstOrDefault().id_item_product);

                            db.item_movement.AddRange(Stock.DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                     App.Names.SalesInvoice,
                                                                    sales_invoice_detail.id_sales_invoice,
                                                                    sales_invoice_detail.id_sales_invoice_detail,
                                                                    Local_SalesInvoice.id_currencyfx,
                                                                    sales_invoice_detail.item.item_product.FirstOrDefault(),
                                                                    (int)sales_invoice_detail.id_location,
                                                                    sales_invoice_detail.quantity,
                                                                    Local_SalesInvoice.trans_date,
                                                                 "Sales Invoice Fix"
                                                                    ));
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
                        purchase_invoice_detail Oldpurchase_invoice_detail = OriginalPurchaseInvoice.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                        if (Oldpurchase_invoice_detail == null)
                        {
                            purchase_invoice_detail.id_location = CurrentSession.Locations.Where(x => x.id_branch == purchase_invoice_detail.purchase_invoice.id_branch).FirstOrDefault().id_location; //FindNFix_Location(item_product, purchase_invoice_detail.app_location, purchase_invoice.app_branch);

                            List<item_movement_dimension> item_movement_dimensionLIST = null;
                            if (purchase_invoice_detail.purchase_invoice_dimension.Count > 0)
                            {
                                item_movement_dimensionLIST = new List<item_movement_dimension>();
                                foreach (purchase_invoice_dimension purchase_invoice_dimension in purchase_invoice_detail.purchase_invoice_dimension)
                                {
                                    item_movement_dimension item_movement_dimension = new item_movement_dimension();
                                    item_movement_dimension.id_dimension = purchase_invoice_dimension.id_dimension;
                                    item_movement_dimension.value = purchase_invoice_dimension.value;
                                    item_movement_dimensionLIST.Add(item_movement_dimension);
                                }
                            }
                            entity.Brillo.Logic.Stock Stock = new entity.Brillo.Logic.Stock();
                            db.item_movement.Add(
                  Stock.CreditOnly_Movement(
                       Status.Stock.InStock,
                       App.Names.PurchaseInvoice,
                       purchase_invoice_detail.id_purchase_invoice,
                       purchase_invoice_detail.id_purchase_invoice_detail,
                       Local_PurchaseInvoice.id_currencyfx,
                       purchase_invoice_detail.item.item_product.FirstOrDefault().id_item_product,
                       (int)purchase_invoice_detail.id_location,
                       purchase_invoice_detail.quantity,
                       Local_PurchaseInvoice.trans_date,
                       purchase_invoice_detail.unit_cost,
                        "Purcahse Invoice Fix", item_movement_dimensionLIST,
                       purchase_invoice_detail.expire_date, purchase_invoice_detail.batch_code));
                        }
                    }
                }
            }
        }

        public void DeleteMovement(db db, int ID, App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                        Oldsales_invoice_detail.IsSelected = true;
                    }
                    foreach (sales_invoice_detail sales_invoice_detail in OriginalSalesInvoice.sales_invoice_detail.Where(x => x.IsSelected == false))
                    {
                        db.item_movement.RemoveRange(db.item_movement.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail));
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                        Oldsales_invoice_detail.IsSelected = true;
                    }
                    foreach (sales_invoice_detail sales_invoice_detail in OriginalSalesInvoice.sales_invoice_detail.Where(x => x.IsSelected == false))
                    {
                        List<item_movement> ListItemMovement = db.item_movement.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).ToList();
                        foreach (item_movement item_movement in ListItemMovement)
                        {
                            if (item_movement.child.Count() == 0)
                            {
                                db.item_movement.Remove(item_movement);
                            }
                            else
                            {
                                foreach (var item in item_movement.child)
                                {
                                    List<item_movement> item_movementList = db.item_movement.Where(x => x.id_item_product == item_movement.id_item_product && x.id_movement != item_movement.id_movement && x.credit > 0).ToList();
                                    foreach (item_movement _item_movement in item_movementList)
                                    {
                                        if (_item_movement.avlquantity > item.credit)
                                        {
                                            item.parent = _item_movement;
                                        }
                                        else
                                        {
                                            item.parent = null;
                                        }
                                    }
                                }
                                db.item_movement.Remove(item_movement);
                            }
                        }
                    }
                }
            }
        }
    }
}