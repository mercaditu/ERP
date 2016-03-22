using System;
using System.Collections.Generic;
using System.Linq;
using WPFLocalizeExtension.Extensions;

namespace entity.Brillo.Logic
{
    public class Stock
    {
        /// <summary>
        /// Will take the Entity, check child rows, validate, and insert the necesary transactions into Item_Movement table.
        /// </summary>
        /// <param name="obj_entity">Pass Table Entity such as Sales Invoice or Purchase Order</param>
        /// <returns></returns>
        public List<item_movement> insert_Stock(db db, object obj_entity)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            //SALES INVOICE
            if (obj_entity.GetType().BaseType == typeof(sales_invoice) || obj_entity.GetType() == typeof(sales_invoice))
            {
                sales_invoice sales_invoice = (sales_invoice)obj_entity;

                foreach (sales_invoice_detail detail in sales_invoice.sales_invoice_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {
                    item_product item_product = FindNFix_ItemProduct(detail.item);
                    detail.id_location = FindNFix_Location(item_product, detail.app_location, sales_invoice.app_branch);

                    //Add Logic for removing Reserved Stock 
                    if (item_product != null && detail.sales_order_detail != null)
                    {
                        //Adding into List
                        item_movementList.Add(Debit_Movement(entity.Status.Stock.Reserved,
                                            App.Names.SalesInvoice,
                                            detail.id_sales_invoice,
                                            item_product,
                                            detail.app_location,
                                            detail.quantity,
                                            sales_invoice.trans_date,
                                            comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name)
                                            ));
                    }

                    item_movementList = Debit_MovementLIST(entity.Status.Stock.InStock,
                                             App.Names.SalesInvoice,
                                             detail.id_sales_invoice,
                                             sales_invoice.app_currencyfx,
                                             item_product,
                                             detail.app_location,
                                             detail.quantity,
                                             sales_invoice.trans_date,
                                             comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name),
                                             detail.unit_price
                                             );
                }
                //Return List so we can save into context.
                return item_movementList;
            }

            //SALES RETURN
            else if (obj_entity.GetType().BaseType == typeof(sales_return) || obj_entity.GetType() == typeof(sales_return))
            {
                sales_return sales_return = (sales_return)obj_entity;
                foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {
                    item_product item_product = FindNFix_ItemProduct(sales_return_detail.item);
                    sales_return_detail.id_location = FindNFix_Location(item_product, sales_return_detail.app_location, sales_return.app_branch);

                    item_movementList.Add( Credit_Movement(entity.Status.Stock.InStock,
                                             App.Names.SalesReturn,
                                             sales_return_detail.id_sales_return,
                                             sales_return.id_currencyfx,
                                             item_product,
                                             (int)sales_return_detail.id_location,
                                             sales_return_detail.quantity,
                                             sales_return.trans_date,
                                             comment_Generator(App.Names.SalesReturn, sales_return.number, sales_return.contact.name),
                                             sales_return_detail.unit_price
                                             ));
                }
                //Return List so we can save into context.
                return item_movementList;

            }

            //PURCHASE RETURN
            else if (obj_entity.GetType().BaseType == typeof(purchase_return) || obj_entity.GetType() == typeof(purchase_return))
            {
                purchase_return purchase_return = (purchase_return)obj_entity;
                List<purchase_return_detail> Listpurchase_return_detail = purchase_return.purchase_return_detail.Where(x => x.id_item > 0).ToList();
                foreach (purchase_return_detail purchase_return_detail in Listpurchase_return_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {

                    item_product item_product = FindNFix_ItemProduct(purchase_return_detail.item);
                    purchase_return_detail.id_location = FindNFix_Location(item_product, purchase_return_detail.app_location, purchase_return.app_branch);

                    

                    item_movementList = Debit_MovementLIST(entity.Status.Stock.InStock,
                                             App.Names.PurchaseReturn,
                                             purchase_return_detail.id_purchase_return,
                                             purchase_return.app_currencyfx,
                                             item_product,
                                             purchase_return_detail.app_location,
                                             purchase_return_detail.quantity,
                                             purchase_return.trans_date,
                                             comment_Generator(App.Names.PurchaseReturn, purchase_return.number, purchase_return.contact.name),
                                             purchase_return_detail.unit_cost
                                             );
                }
                //Return List so we can save into context.
                return item_movementList;
            }

            //SALES ORDER
            else if (obj_entity.GetType().BaseType == typeof(sales_order) || obj_entity.GetType() == typeof(sales_order))
            {
                sales_order sales_order = (sales_order)obj_entity;
                foreach (sales_order_detail sales_order_detail in sales_order.sales_order_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {

                    item_product item_product = FindNFix_ItemProduct(sales_order_detail.item);
                    sales_order_detail.id_location = FindNFix_Location(item_product, sales_order_detail.app_location, sales_order.app_branch);

                    //Add Logic for removing Reserved Stock 
                    if (item_product != null && sales_order_detail.sales_budget_detail != null)
                    {
                        //Adding into List
                        item_movementList.Add(Debit_Movement(entity.Status.Stock.Reserved,
                                            App.Names.SalesOrder,
                                            sales_order_detail.id_sales_order,
                                            item_product,
                                            sales_order_detail.app_location,
                                            sales_order_detail.quantity,
                                            sales_order.trans_date,
                                            comment_Generator(App.Names.SalesOrder, sales_order.number, sales_order.contact.name)
                                            ));
                    }

                    item_movementList = Debit_MovementLIST(entity.Status.Stock.InStock,
                                             App.Names.SalesOrder,
                                            sales_order_detail.id_sales_order,
                                              sales_order.app_currencyfx,
                                             item_product,
                                             sales_order_detail.app_location,
                                             sales_order_detail.quantity,
                                              sales_order.trans_date,
                                             comment_Generator(App.Names.SalesOrder, sales_order.number, sales_order.contact.name),
                                             sales_order_detail.unit_price
                                             );
                }

                //Return List so we can save into context.
            }

            //PURCHASE INVOICE
            else if (obj_entity.GetType().BaseType == typeof(purchase_invoice) || obj_entity.GetType() == typeof(purchase_invoice))
            {
                purchase_invoice purchase_invoice = (purchase_invoice)obj_entity;
                List<purchase_invoice_detail> Listpurchase_invoice_detail = purchase_invoice.purchase_invoice_detail.Where(x => x.id_item > 0).ToList();
                foreach (purchase_invoice_detail purchase_invoice_detail in Listpurchase_invoice_detail
                        .Where(x => x.item.item_product.Count() > 0))
                {

                    item_product item_product = FindNFix_ItemProduct(purchase_invoice_detail.item);
                    purchase_invoice_detail.id_location = FindNFix_Location(item_product, purchase_invoice_detail.app_location, purchase_invoice.app_branch);

                    if (purchase_invoice_detail.purchase_order_detail != null)
                    {
                        item_movementList = Debit_MovementLIST(entity.Status.Stock.OnTheWay,
                                             App.Names.PurchaseInvoice,
                                             purchase_invoice_detail.id_purchase_invoice,
                                             purchase_invoice.app_currencyfx,
                                             item_product,
                                             purchase_invoice_detail.app_location,
                                             purchase_invoice_detail.quantity,
                                              purchase_invoice.trans_date,
                                             comment_Generator(App.Names.PurchaseInvoice, purchase_invoice.number, purchase_invoice.contact.name),
                                             purchase_invoice_detail.unit_cost
                                             );

                        //Adding into List
                      // item_movementList.Add(mov_OnTheWay);
                    }

                    //Improve Comment. More standarized.
                    item_movementList.Add(Credit_Movement(entity.Status.Stock.InStock,
                                              App.Names.PurchaseInvoice,
                                              purchase_invoice_detail.id_purchase_invoice,
                                              purchase_invoice.id_currencyfx,
                                              item_product,
                                              (int)purchase_invoice_detail.id_location,
                                              purchase_invoice_detail.quantity,
                                              purchase_invoice.trans_date,
                                              comment_Generator(App.Names.PurchaseInvoice, purchase_invoice.number, purchase_invoice.contact.name),
                                              purchase_invoice_detail.unit_cost
                                              ));
                }
                //Return List so we can save into context.
                return item_movementList;
            }

            //PURCHASE ORDER
            else if (obj_entity.GetType().BaseType == typeof(purchase_order) || obj_entity.GetType() == typeof(purchase_order))
            {
                purchase_order purchase_order = (purchase_order)obj_entity;
                List<purchase_order_detail> Listpurchase_order_detail = purchase_order.purchase_order_detail.Where(x => x.id_item > 0).ToList();
                foreach (purchase_order_detail purchase_order_detail in Listpurchase_order_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {
                    item_product item_product = FindNFix_ItemProduct(purchase_order_detail.item);
                    purchase_order_detail.id_location = FindNFix_Location(item_product, purchase_order_detail.app_location, purchase_order.app_branch);

                    if (item_product != null)
                    {
                        item_movementList.Add(Credit_Movement(entity.Status.Stock.InStock,
                                                App.Names.PurchaseOrder,
                                                purchase_order_detail.id_purchase_order,
                                                purchase_order.id_currencyfx,
                                                item_product,
                                                (int)purchase_order_detail.id_location,
                                                purchase_order_detail.quantity,
                                                purchase_order.trans_date,
                                                comment_Generator(App.Names.PurchaseOrder, purchase_order.number, purchase_order.contact.name),
                                                purchase_order_detail.unit_cost
                                                ));
                    }
                }
                //Return List so we can save into context.
                return item_movementList;
            }
            //production Execustion
            if (obj_entity.GetType().BaseType == typeof(production_execution) || obj_entity.GetType() == typeof(production_execution))
            {
                production_execution production_execution = (production_execution)obj_entity;

                foreach (production_execution_detail detail in production_execution.production_execution_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {
                    item_product item_product = FindNFix_ItemProduct(detail.item);

                    List<item_movement> _item_movementList;
                    _item_movementList = db.item_movement.Where(x => x.id_location == production_execution.production_line.id_location
                                                                  && x.id_item_product == item_product.id_item_product
                                                                  && x.status == entity.Status.Stock.InStock
                                                                  && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                    if (item_product.cogs_type == item_product.COGS_Types.LIFO && _item_movementList != null)
                    {
                        _item_movementList = _item_movementList.OrderBy(x => x.trans_date).ToList();
                    }
                    else if (_item_movementList != null)
                    {
                        _item_movementList = _item_movementList.OrderByDescending(x => x.trans_date).ToList();
                    }

                    decimal qty_ExexustionDetail = detail.quantity;
                    foreach (item_movement object_Movement in _item_movementList)
                    {


                        if (qty_ExexustionDetail > 0)
                        {
                            item_movement item_movement = new item_movement();



                            if (detail.is_input)
                            {
                                decimal movement_debit_quantity = qty_ExexustionDetail;
                                if (object_Movement.credit <= qty_ExexustionDetail)
                                {
                                    movement_debit_quantity = object_Movement.credit;
                                }
                                else
                                {
                                    movement_debit_quantity = qty_ExexustionDetail;
                                }

                                //If input is true, then we should DEBIT Stock.
                                item_movement = Debit_Movement(entity.Status.Stock.InStock,
                                                        App.Names.ProductionExecution,
                                                        (int)detail.id_production_execution,
                                                        item_product,
                                                        production_execution.production_line.app_location,
                                                        movement_debit_quantity,
                                                        production_execution.trans_date,
                                                        comment_Generator(App.Names.ProductionExecution,
                                                        production_execution.id_production_execution.ToString(), ""));
                            }
                            else
                            {
                                //If input is false, then we should CREDIT Stock.
                                item_movement = credit_Movement(entity.Status.Stock.InStock,
                                                        App.Names.ProductionExecution,
                                                        (int)detail.id_production_execution,
                                                        item_product.id_item_product,
                                                        (int)production_execution.production_line.id_location,
                                                        qty_ExexustionDetail,
                                                        production_execution.trans_date,
                                                        comment_Generator(App.Names.ProductionExecution,
                                                        production_execution.id_production_execution.ToString(), ""));
                            }

                            item_movement._parent = object_Movement;

                            //Logic for Value
                            item_movement_value item_movement_value = new item_movement_value();
                            item_movement_value.unit_value = object_Movement.item_movement_value.Sum(i => i.unit_value);
                            item_movement_value.id_currencyfx = object_Movement.item_movement_value.FirstOrDefault().id_currencyfx;
                            item_movement_value.comment = item_movement.comment;
                            item_movement.item_movement_value.Add(item_movement_value);
                            //Adding into List
                            item_movementList.Add(item_movement);
                            qty_ExexustionDetail = qty_ExexustionDetail - object_Movement.credit;

                        }
                    }
                    if (qty_ExexustionDetail > 0)
                    {
                        //Adding into List if _item_movementList is empty.
                        item_movementList.Add(Debit_Movement(entity.Status.Stock.InStock,
                                                App.Names.ProductionExecution,
                                                detail.id_production_execution,
                                                item_product,
                                                production_execution.production_line.app_location,
                                                qty_ExexustionDetail,
                                                production_execution.trans_date,
                                                comment_Generator(App.Names.ProductionExecution, production_execution.id_production_execution.ToString(), "")
                                            ));
                    }
                    // }

                }
                //Return List so we can save into context.
                return item_movementList;
            }

            return null;
        }

        /// <summary>
        /// Will Delete all records found matching Application ID and Transaction ID from Database.
        /// </summary>
        /// <param name="Application_ID">ID of Source Application. OfType app.Applications</param>
        /// <param name="Transaction_ID">ID of Source Transaction from DB.</param>
        /// <returns></returns>
        public List<item_movement> revert_Stock(db db, App.Names Application_ID, int Transaction_ID)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            //using (db db = new db())
            //{
            item_movementList = db.item_movement
                                        .Where(x => x.id_application == Application_ID
                                            && x.transaction_id == Transaction_ID).ToList();
            if (item_movementList != null)
            {
                return item_movementList;
            }
            // }

            return null;
        }

        public string comment_Generator(App.Names AppName, string TransNumber, string ContactName)
        {
            string strAPP = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + AppName.ToString());
            return string.Format(strAPP + " {0} / {1}", TransNumber, ContactName);
        }


        public List<item_movement> Debit_MovementLIST( entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
                                                       app_currencyfx app_currencyfx, item_product item_product, app_location app_location,
                                                       decimal Quantity, DateTime TransDate,
                                                       string Comment, decimal unit_price)
                    {

                        List<item_movement> Items_InStockLIST = new List<item_movement>();
                        List<item_movement> Final_ItemMovementLIST = new List<item_movement>();

                        using (db db = new db())
                        {
                            Items_InStockLIST = db.item_movement.Where(x => x.id_location == app_location.id_location
                                                                                  && x.id_item_product == item_product.id_item_product
                                                                                  && x.status == entity.Status.Stock.InStock
                                                                                  && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                            if (item_product.cogs_type == item_product.COGS_Types.LIFO && Items_InStockLIST != null)
                            {
                                Items_InStockLIST = Items_InStockLIST.OrderBy(x => x.trans_date).ToList();
                            }
                            else if (Items_InStockLIST != null)
                            {
                                Items_InStockLIST = Items_InStockLIST.OrderByDescending(x => x.trans_date).ToList();
                            }

                            decimal qty_SalesDetail = Quantity;
                
                            ///Will create new Item Movements 
                            ///if split from Parents is needed.
                            foreach (item_movement parent_Movement in Items_InStockLIST)
                            {
                                if (qty_SalesDetail > 0)
                                {
                                    item_movement item_movement = new item_movement();

                                    decimal movement_debit_quantity = qty_SalesDetail;

                                    //If Parent Movement is lesser than Quantity, then only take total value of Parent.
                                    if (parent_Movement.credit <= qty_SalesDetail)
                                    {
                                        movement_debit_quantity = parent_Movement.credit;
                                    }

                                    item_movement.comment = Comment;
                                    item_movement.id_item_product = item_product.id_item_product;
                                    item_movement.debit = Quantity;
                                    item_movement.credit = 0;
                                    item_movement.status = Status;
                                    item_movement.id_location = app_location.id_location;
                                    item_movement._parent = null;
                                    item_movement.id_application = ApplicationID;
                                    item_movement.transaction_id = TransactionID;
                                    item_movement.trans_date = TransDate;

                                    item_movement._parent = parent_Movement;

                                    //Logic for Value
                                    item_movement_value item_movement_value = new item_movement_value();

                                    item_movement_value.unit_value = parent_Movement.GetValue_ByCurrency(app_currencyfx.app_currency);
                                    item_movement_value.id_currencyfx = app_currencyfx.id_currencyfx;
                                    item_movement_value.comment = Brillo.Localize.StringText("DirectCost");
                                    item_movement.item_movement_value.Add(item_movement_value);

                                    //Adding into List
                                    Final_ItemMovementLIST.Add(item_movement);
                                    qty_SalesDetail = qty_SalesDetail - parent_Movement.credit;
                                }
                            }

                            ///In case Parent does not exist, will enter this code.
                            if (qty_SalesDetail > 0)
                            {
                                item_movement item_movement = new item_movement();
                                //Adding into List if Movement List for this Location is empty.
                                item_movement.comment = Comment;
                                item_movement.id_item_product = item_product.id_item_product;
                                item_movement.debit = Quantity;
                                item_movement.credit = 0;
                                item_movement.status = Status;
                                item_movement.id_location = app_location.id_location;
                                item_movement._parent = null;
                                item_movement.id_application = ApplicationID;
                                item_movement.transaction_id = TransactionID;
                                item_movement.trans_date = TransDate;

                                item_movement._parent = null;

                                //Logic for Value in case Parent does not Exist, we will take from 
                                item_movement_value item_movement_value = new item_movement_value();
                                item_movement_value.unit_value = (decimal)item_product.item.unit_cost;
                                item_movement_value.id_currencyfx = app_currencyfx.id_currencyfx;
                                item_movement_value.comment = Brillo.Localize.StringText("DirectCost");
                                item_movement.item_movement_value.Add(item_movement_value);
                                //Adding into List
                                Final_ItemMovementLIST.Add(item_movement);
                            }
                        }
                        return Final_ItemMovementLIST;
                    }

     
        public item_movement Credit_Movement( entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
                                              int CurrencyFXID, item_product item_product, int LocationID,
                                              decimal Quantity, DateTime TransDate, string Comment, decimal unit_price)
        {
            if (Quantity > 0)
            {
                item_movement item_movement = new item_movement();
                //Adding into List if Movement List for this Location is empty.
                item_movement.comment = Comment;
                item_movement.id_item_product = item_product.id_item_product;
                item_movement.debit = 0;
                item_movement.credit = Quantity;
                item_movement.status = Status;
                item_movement.id_location = LocationID;
                item_movement._parent = null;
                item_movement.id_application = ApplicationID;
                item_movement.transaction_id = TransactionID;
                item_movement.trans_date = TransDate;

                item_movement._parent = null;

                //Logic for Value in case Parent does not Exist, we will take from 
                item_movement_value item_movement_value = new item_movement_value();
                item_movement_value.unit_value = (decimal)item_product.item.unit_cost;
                item_movement_value.id_currencyfx = CurrencyFXID;
                item_movement_value.comment = Brillo.Localize.StringText("DirectCost");
                item_movement.item_movement_value.Add(item_movement_value);

                return item_movement;
            }

            return null;
        }

        public List<item_movement> DebitCredit_MovementList( entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
                                              int CurrencyFXID, item_product item_product, int LocationID,
                                              decimal Quantity, DateTime TransDate, string Comment, decimal unit_price)
        {
            List<item_movement> Final_ItemMovementLIST = new List<item_movement>();

            item_movement credit_movement = new item_movement();
            credit_movement = Credit_Movement(Status, ApplicationID, TransactionID, CurrencyFXID,
                                              item_product, LocationID, Quantity, TransDate,
                                              Comment, unit_price);

            //Bring Debit Function form above. IT should handle child and parent values.
            item_movement debit_movement = new item_movement();
            if (Quantity > 0)
            {
                item_movement item_movement = new item_movement();
                //Adding into List if Movement List for this Location is empty.
                item_movement.comment = Comment;
                item_movement.id_item_product = item_product.id_item_product;
                item_movement.debit = Quantity;
                item_movement.credit = 0;
                item_movement.status = Status;
                item_movement.id_location = LocationID;
                item_movement._parent = null;
                item_movement.id_application = ApplicationID;
                item_movement.transaction_id = TransactionID;
                item_movement.trans_date = TransDate;

                item_movement._parent = credit_movement;

                //Logic for Value in case Parent does not Exist, we will take from 
                item_movement_value item_movement_value = new item_movement_value();
                //logic to check fx rate of parent.
                item_movement_value.unit_value = credit_movement.item_movement_value.Sum(x => x.unit_value); 
                item_movement_value.id_currencyfx = credit_movement.item_movement_value.Max(x => x.id_currencyfx);
                item_movement_value.comment = Brillo.Localize.StringText("DirectCost");
                item_movement.item_movement_value.Add(item_movement_value);
                //Adding into List
                Final_ItemMovementLIST.Add(item_movement);
            }

            credit_movement._child.Add(debit_movement);
            Final_ItemMovementLIST.Add(credit_movement);

            return Final_ItemMovementLIST;
        }

        public item_movement Debit_Movement( entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
                                             item_product item_product, app_location app_location, decimal Quantity,
                                             DateTime TransDate, string Comment)
        {
            item_movement item_movement = new item_movement();
            item_movement.comment = Comment;
            item_movement.id_item_product = item_product.id_item_product;
            item_movement.debit = Quantity;
            item_movement.credit = 0;
            item_movement.status = Status;
            item_movement.id_location = app_location.id_location;
            item_movement.id_application = ApplicationID;
            item_movement.transaction_id = TransactionID;
            item_movement.trans_date = TransDate;
            return item_movement;
        }


        public item_movement credit_Movement(
          entity.Status.Stock Status,
          App.Names ApplicationID,
          int TransactionID,
          int Item_ProductID,
          int LocationID,
          decimal Quantity,
          DateTime TransDate,
          string Comment)
        {
            item_movement item_movement = new item_movement();
            item_movement.comment = Comment;
            item_movement.id_item_product = Item_ProductID;
            item_movement.debit = 0;
            item_movement.credit = Quantity;
            item_movement.status = Status;
            item_movement.id_location = LocationID;
            item_movement.id_application = ApplicationID;
            item_movement.transaction_id = TransactionID;
            item_movement.trans_date = TransDate;
            return item_movement;
        }

       
        public item_product FindNFix_ItemProduct(item item)
        {
            if (item.item_product == null)
            {
                using (db db = new db())
                {
                    item_product item_product = new item_product();
                    item.item_product.Add(item_product);
                    db.items.Attach(item);
                    db.SaveChangesAsync();
                    return item_product;
                }
            }
            return item.item_product.FirstOrDefault();
        }

        public int FindNFix_Location(item_product item_product, app_location app_location, app_branch app_branch)
        {
            if (app_location == null && app_branch != null)
            {
                Location Location = new Location();
                return Location.get_Location(item_product, app_branch);
            }

            return app_location.id_location;
        }
    }
}
