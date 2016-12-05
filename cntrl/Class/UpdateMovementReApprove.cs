using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
using entity.Brillo;
using cntrl.PanelAdv;
using System.Windows;

namespace cntrl.Class
{
    public class UpdateMovementReApprove
    {
        //public void Start(db db, int ID, entity.App.Names Application)
        //{


        //    //sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
        //    //sales_invoice sales_invoice = db.sales_invoice.Find(ID);
        //    //ValueChange(db, ID, Application);
        //    //QuantityUP(db, ID, Application);
        //    //QuantityDown(db, ID, Application);
        //    //DateChange(db, ID, Application);
        //    //NewMovement(db, ID, Application);

        //    //List<item_movement> item_movementList = new List<item_movement>();
        //    //foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
        //    //{
        //    //    item_movementList.AddRange(sales_invoice_detail.item_movement);
        //    //}
        //    //List<item_movement> item_movementListOld = new List<item_movement>();
        //    //foreach (sales_invoice_detail sales_invoice_detail in Oldsales_invoice.sales_invoice_detail)
        //    //{
        //    //    item_movementListOld.AddRange(sales_invoice_detail.item_movement);
        //    //}
        //    //ActionPanel _ActionPanel = new ActionPanel();
        //    //_ActionPanel.item_movementList = item_movementList;
        //    //_ActionPanel.item_movementOldList = item_movementListOld;



        //    //Window window = new Window
        //    //{
        //    //    Title = "ReApprove",
        //    //    Content = _ActionPanel
        //    //};

        //    //window.ShowDialog();

        //}
        public void ValueChange(db db, int ID, entity.App.Names Application)
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
                        purchase_invoice_detail Oldsales_invoice_detail = OriginalPurchaseInvoice.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                        if (Oldsales_invoice_detail != null)
                        {
                            if (purchase_invoice_detail.unit_cost != Oldsales_invoice_detail.unit_cost)
                            {
                                foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                                {
                                    item_movement_value item_movement_value = item_movement.item_movement_value.FirstOrDefault();
                                    if (item_movement_value != null)
                                    {
                                        item_movement_value.unit_value = purchase_invoice_detail.unit_cost;
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }
        public void QuantityUP(db db, int ID, entity.App.Names Application)
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
                            if (sales_invoice_detail.quantity > Oldsales_invoice_detail.quantity)
                            {
                                decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                                foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                                {

                                    if (item_movement.parent == null)
                                    {
                                        item_movement.debit = sales_invoice_detail.quantity;
                                    }
                                    else
                                    {
                                        if ((item_movement.parent.credit - item_movement.parent.child.Sum(x => x.debit)) > Diff)
                                        {
                                            item_movement.debit = sales_invoice_detail.quantity;
                                        }
                                        else
                                        {
                                            item_movement.debit = sales_invoice_detail.quantity;
                                            Stock stock = new Stock();
                                            List<StockList> Items_InStockLIST = stock.List(sales_invoice_detail.app_location.id_branch, (int)sales_invoice_detail.id_location, sales_invoice_detail.item.item_product.FirstOrDefault().id_item_product);
                                            foreach (StockList StockList in Items_InStockLIST)
                                            {
                                                if (StockList.QtyBalance > item_movement.debit)
                                                {
                                                    item_movement.parent = db.item_movement.Where(x => x.id_movement == StockList.MovementID).FirstOrDefault();
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
                            if (purchase_invoice_detail.quantity > Oldpurchase_invoice_detail.quantity)
                            {
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

        }
        public void QuantityDown(db db, int ID, entity.App.Names Application)
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
                            if (sales_invoice_detail.quantity < Oldsales_invoice_detail.quantity)
                            {
                                decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                                foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                                {

                                    if (item_movement.child.Count() == 0)
                                    {
                                        item_movement.debit = sales_invoice_detail.quantity;
                                    }
                                    else
                                    {
                                        if ((item_movement.child.Sum(x => x.credit)) < sales_invoice_detail.quantity)
                                        {
                                            item_movement.credit = sales_invoice_detail.quantity;

                                        }
                                        else
                                        {
                                            Decimal qunatity = sales_invoice_detail.quantity;
                                            item_movement.debit = qunatity;

                                            foreach (item_movement item in item_movement.child)
                                            {

                                                if (item.credit < qunatity)
                                                {
                                                    qunatity = qunatity - item.credit;
                                                    item.parent = item_movement;
                                                }
                                                else
                                                {
                                                    int id_item_product = sales_invoice_detail.item.item_product.FirstOrDefault().id_item_product;
                                                    List<item_movement> item_movementList = db.item_movement.Where(x => x.id_item_product == id_item_product && x.id_movement != item_movement.id_movement).ToList();
                                                    foreach (item_movement _item_movement in item_movementList)
                                                    {
                                                        if (item_movement.avlquantity > item.credit)
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
                            if (purchase_invoice_detail.quantity < Oldpurchase_invoice_detail.quantity)
                            {
                                decimal Diff = purchase_invoice_detail.quantity - Oldpurchase_invoice_detail.quantity;
                                foreach (item_movement item_movement in purchase_invoice_detail.item_movement)
                                {

                                    if (item_movement.child.Count() == 0)
                                    {
                                        item_movement.credit = purchase_invoice_detail.quantity;
                                    }
                                    else
                                    {
                                        if ((item_movement.child.Sum(x => x.debit)) < purchase_invoice_detail.quantity)
                                        {
                                            item_movement.credit = purchase_invoice_detail.quantity;

                                        }
                                        else
                                        {
                                            Decimal qunatity = purchase_invoice_detail.quantity;
                                            item_movement.credit = qunatity;

                                            foreach (item_movement item in item_movement.child)
                                            {

                                                if (item.debit < qunatity)
                                                {
                                                    qunatity = qunatity - item.debit;
                                                    item.parent = item_movement;
                                                }
                                                else
                                                {
                                                    int id_item_product = purchase_invoice_detail.item.item_product.FirstOrDefault().id_item_product;
                                                    List<item_movement> item_movementList = db.item_movement.Where(x => x.id_item_product == id_item_product && x.id_movement != item_movement.id_movement).ToList();
                                                    foreach (item_movement _item_movement in item_movementList)
                                                    {
                                                        if (item_movement.avlquantity > item.debit)
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
            }
        }



        public void DateChange(db db, int ID, entity.App.Names Application)
        {

            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();


                sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                if (OriginalSalesInvoice.trans_date != Local_SalesInvoice.trans_date)
                {
                    foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                    {
                        foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                        {
                            item_movement.trans_date = Local_SalesInvoice.trans_date;
                        }
                    }

                }
            }
        }


        public void NewMovement(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                {
                    sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                    if (Oldsales_invoice_detail == null)
                    {
                        Stock stock = new Stock();
                        entity.Brillo.Logic.Stock Stock = new entity.Brillo.Logic.Stock();
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
    }
}

