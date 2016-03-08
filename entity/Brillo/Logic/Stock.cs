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
        public List<item_movement> insert_Stock(db db ,object obj_entity)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            //SALES INVOICE
            if (obj_entity.GetType().BaseType == typeof(sales_invoice) || obj_entity.GetType() == typeof(sales_invoice))
            {
                sales_invoice sales_invoice = (sales_invoice)obj_entity;

                foreach (sales_invoice_detail detail in sales_invoice.sales_invoice_detail
                    .Where(x => x.item.id_item_type == item.item_type.Product
                             || x.item.id_item_type == item.item_type.RawMaterial))
                {
                    item_product item_product = FindNFix_ItemProduct(detail.item);
                    detail.id_location = FindNFix_Location(item_product, detail.app_location, sales_invoice.app_branch);

                    //Add Logic for removing Reserved Stock 
                    if (item_product != null && detail.sales_order_detail != null)
                    {
                        //Adding into List
                        item_movementList.Add(debit_Movement(entity.Status.Stock.Reserved,
                                            App.Names.SalesInvoice,
                                            detail.id_sales_invoice,
                                            item_product.id_item_product,
                                            (int)detail.id_location,
                                            detail.quantity,
                                            sales_invoice.trans_date,
                                            comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name)
                                            ));
                    }

                    List<item_movement> _item_movementList;
                    //using (db db = new db())
                    //{
                        _item_movementList = db.item_movement.Where(x => x.id_location == detail.id_location
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
                    else
                    {
                        //Adding into List if _item_movementList is empty.
                        item_movementList.Add(debit_Movement(entity.Status.Stock.InStock,
                                                App.Names.SalesInvoice,
                                                detail.id_sales_invoice,
                                                item_product.id_item_product,
                                                (int)detail.id_location,
                                                detail.quantity,
                                                sales_invoice.trans_date,
                                                comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name)
                                            ));
                    }
                    if (_item_movementList.Count()==0)
                    {
                        item_movement item_movement = new item_movement();
                          //Adding into List if Movement List for this Location is empty.
                            item_movement = debit_Movement(entity.Status.Stock.InStock,
                                                    App.Names.SalesInvoice,
                                                    (int)detail.id_sales_invoice_detail,
                                                    item_product.id_item_product,
                                                    (int)detail.id_location,
                                                    detail.quantity,
                                                    sales_invoice.trans_date,
                                                    comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name));

                            item_movement._parent = null;

                            //Logic for Value
                            item_movement_value item_movement_value = new item_movement_value();
                            item_movement_value.unit_value =detail.unit_price;
                            item_movement_value.id_currencyfx = sales_invoice.id_currencyfx;
                            item_movement_value.comment = item_movement.comment;
                            item_movement.item_movement_value.Add(item_movement_value);
                            //Adding into List
                            item_movementList.Add(item_movement);
                    }

                    foreach (item_movement object_Movement in _item_movementList)
                    {
                        decimal qty_SalesDetail = detail.quantity;

                        if (qty_SalesDetail > 0)
                        {
                            item_movement item_movement = new item_movement();

                            decimal movement_debit_quantity = qty_SalesDetail;
                            if (object_Movement.credit <= qty_SalesDetail)
                            {
                                movement_debit_quantity = object_Movement.credit;
                            }

                            //Adding into List if Movement List for this Location is empty.
                            item_movement = debit_Movement(entity.Status.Stock.InStock,
                                                    App.Names.SalesInvoice,
                                                    (int)detail.id_sales_invoice_detail,
                                                    item_product.id_item_product,
                                                    (int)detail.id_location,
                                                    movement_debit_quantity,
                                                    sales_invoice.trans_date,
                                                    comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name));

                            item_movement._parent = object_Movement;

                            //Logic for Value
                            item_movement_value item_movement_value = new item_movement_value();
                            item_movement_value.unit_value = object_Movement.item_movement_value.Sum(i => i.unit_value);
                            item_movement_value.id_currencyfx = sales_invoice.id_currencyfx;
                            item_movement_value.comment = item_movement.comment;
                            item_movement.item_movement_value.Add(item_movement_value);
                            //Adding into List
                            item_movementList.Add(item_movement);
                            qty_SalesDetail = qty_SalesDetail - object_Movement.credit;
                        }
                    }
                   // }

                }
                //Return List so we can save into context.
                return item_movementList;
            }

            //SALES RETURN
            else if (obj_entity.GetType().BaseType == typeof(sales_return) || obj_entity.GetType() == typeof(sales_return))
            {
                sales_return sales_return = (sales_return)obj_entity;
                foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail
                    .Where(x => x.item.id_item_type == item.item_type.Product
                             || x.item.id_item_type == item.item_type.RawMaterial))
                {
                    item_product item_product = FindNFix_ItemProduct(sales_return_detail.item);
                    sales_return_detail.id_location = FindNFix_Location(item_product, sales_return_detail.app_location, sales_return.app_branch);

                    item_movement item_movement = credit_Movement(entity.Status.Stock.InStock,
                                                        App.Names.SalesReturn,
                                                        sales_return_detail.id_sales_return_detail,
                                                        item_product.id_item_product,
                                                        (int)sales_return_detail.id_location,
                                                        sales_return_detail.quantity,
                                                        sales_return.trans_date,
                                                        comment_Generator(App.Names.SalesReturn, sales_return.number, sales_return.contact.name));

                    //Logic for Value
                    item_movement_value item_movement_detail = new item_movement_value();
                    item_movement_detail.unit_value = sales_return_detail.unit_price;
                    item_movement_detail.id_currencyfx = sales_return.id_currencyfx;
                    item_movement_detail.comment = comment_Generator(App.Names.SalesReturn, sales_return.number, sales_return.contact.name);
                    item_movement.item_movement_value.Add(item_movement_detail);
                    //Adding into List
                    item_movementList.Add(item_movement);
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
                    .Where(x => x.item.id_item_type == item.item_type.Product
                             || x.item.id_item_type == item.item_type.RawMaterial))
                {

                    item_product item_product = FindNFix_ItemProduct(purchase_return_detail.item);
                    purchase_return_detail.id_location = FindNFix_Location(item_product, purchase_return_detail.app_location, purchase_return.app_branch);

                    item_movement item_movement = debit_Movement(entity.Status.Stock.InStock,
                                    App.Names.PurchaseReturn,
                                    purchase_return_detail.id_purchase_return_detail,
                                    item_product.id_item_product,
                                    (int)purchase_return_detail.id_location,
                                    purchase_return_detail.quantity,
                                    purchase_return.trans_date,
                                    comment_Generator(App.Names.PurchaseReturn, purchase_return.number, purchase_return.contact.name));

                    //Logic for Value
                    item_movement_value item_movement_detail = new item_movement_value();
                    item_movement_detail.unit_value = purchase_return_detail.unit_cost;
                    item_movement_detail.id_currencyfx = purchase_return.id_currencyfx;
                    item_movement_detail.comment = item_movement.comment;
                    item_movement.item_movement_value.Add(item_movement_detail);
                    //Adding into List
                    item_movementList.Add(item_movement);
                }
                //Return List so we can save into context.
                return item_movementList;
            }

            //SALES ORDER
            else if (obj_entity.GetType().BaseType == typeof(sales_order) || obj_entity.GetType() == typeof(sales_order))
            {
                sales_order sales_order = (sales_order)obj_entity;
                foreach (sales_order_detail sales_order_detail in sales_order.sales_order_detail
                    .Where(x => x.item.id_item_type == item.item_type.Product
                             || x.item.id_item_type == item.item_type.RawMaterial))
                {

                    item_product item_product = FindNFix_ItemProduct(sales_order_detail.item);
                    sales_order_detail.id_location = FindNFix_Location(item_product, sales_order_detail.app_location, sales_order.app_branch);

                    //Check if Stock Exists.
                    bool qtyExists = false;
                    using (StockDB _stockDB = new StockDB())
                    {
                        decimal qty = _stockDB.getStockCount((int)sales_order_detail.id_location, item_product.id_item_product);

                        if (qty > 0)
                            qtyExists = true;
                    }

                    if (qtyExists)
                    {
                        //If Exists, Discount from InStock
                        item_movement item_movement_debit = debit_Movement(entity.Status.Stock.InStock,
                                    App.Names.SalesOrder,
                                    sales_order_detail.id_sales_order_detail,
                                    item_product.id_item_product,
                                    (int)sales_order_detail.id_location,
                                    sales_order_detail.quantity,
                                    sales_order.trans_date,
                                    comment_Generator(App.Names.SalesOrder, sales_order.number, sales_order.contact.name));
                        item_movementList.Add(item_movement_debit);

                        ///...and Reserve
                        item_movement item_movement_credit = credit_Movement(entity.Status.Stock.Reserved,
                                    App.Names.SalesOrder,
                                    sales_order_detail.id_sales_order_detail,
                                    item_product.id_item_product,
                                    (int)sales_order_detail.id_location,
                                    sales_order_detail.quantity,
                                    sales_order.trans_date,
                                    comment_Generator(App.Names.SalesOrder, sales_order.number, sales_order.contact.name));
                        item_movementList.Add(item_movement_credit);
                        return item_movementList;
                    }
                    else
                    {
                        //If Not send order to Request.
                    }
                }

                //Return List so we can save into context.
            }

            //PURCHASE INVOICE
            else if (obj_entity.GetType().BaseType == typeof(purchase_invoice) || obj_entity.GetType() == typeof(purchase_invoice))
            {
                purchase_invoice purchase_invoice = (purchase_invoice)obj_entity;
                List<purchase_invoice_detail> Listpurchase_invoice_detail = purchase_invoice.purchase_invoice_detail.Where(x => x.id_item > 0).ToList();
                foreach (purchase_invoice_detail purchase_invoice_detail in Listpurchase_invoice_detail
                        .Where(x => x.item.id_item_type == item.item_type.Product
                                 || x.item.id_item_type == item.item_type.RawMaterial))
                {

                    item_product item_product = FindNFix_ItemProduct(purchase_invoice_detail.item);
                    purchase_invoice_detail.id_location = FindNFix_Location(item_product, purchase_invoice_detail.app_location, purchase_invoice.app_branch);
                    
                    if (purchase_invoice_detail.purchase_order_detail != null)
                    {
                        item_movement mov_OnTheWay = debit_Movement(entity.Status.Stock.OnTheWay,
                                                        App.Names.PurchaseInvoice,
                                                        purchase_invoice_detail.id_purchase_invoice_detail,
                                                        item_product.id_item_product,
                                                        (int)purchase_invoice_detail.id_location,
                                                        purchase_invoice_detail.quantity,
                                                        purchase_invoice.trans_date,
                                                        comment_Generator(App.Names.PurchaseInvoice, purchase_invoice.number, purchase_invoice.contact.name));
                        
                        //Adding into List
                        item_movementList.Add(mov_OnTheWay);
                    }

                    //Improve Comment. More standarized.
                    item_movement item_movement = credit_Movement(entity.Status.Stock.InStock,
                                                    App.Names.PurchaseInvoice,
                                                    purchase_invoice_detail.id_purchase_invoice_detail,
                                                    item_product.id_item_product,
                                                    (int)purchase_invoice_detail.id_location,
                                                    purchase_invoice_detail.quantity,
                                                    purchase_invoice.trans_date,
                                                    comment_Generator(App.Names.PurchaseInvoice, purchase_invoice.number, purchase_invoice.contact.name));

                    //Logic for Value
                    item_movement_value item_movement_detail = new item_movement_value();
                    item_movement_detail.unit_value = purchase_invoice_detail.unit_cost;
                    item_movement_detail.id_currencyfx = purchase_invoice.id_currencyfx;
                    item_movement_detail.comment = item_movement.comment;
                    item_movement.item_movement_value.Add(item_movement_detail);

                    //Adding into List
                    item_movementList.Add(item_movement);
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
                    .Where(x => x.item.id_item_type == item.item_type.Product
                             || x.item.id_item_type == item.item_type.RawMaterial))
                {
                    item_product item_product = FindNFix_ItemProduct(purchase_order_detail.item);
                    purchase_order_detail.id_location = FindNFix_Location(item_product, purchase_order_detail.app_location, purchase_order.app_branch);

                    if (item_product != null)
                    {
                        //Improve Comment. More standarized.
                        item_movement item_movement = credit_Movement(entity.Status.Stock.OnTheWay,
                                                        App.Names.PurchaseInvoice,
                                                        purchase_order_detail.id_purchase_order_detail,
                                                        item_product.id_item_product,
                                                        (int)purchase_order_detail.id_location,
                                                        purchase_order_detail.quantity,
                                                        purchase_order.trans_date,
                                                        comment_Generator(App.Names.PurchaseOrder, purchase_order.number, purchase_order.contact.name));
                        //Adding into List
                        item_movementList.Add(item_movement);
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
                    .Where(x => x.item.id_item_type == item.item_type.Product
                             || x.item.id_item_type == item.item_type.RawMaterial))
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
                    else
                    {
                        //Adding into List if _item_movementList is empty.
                        item_movementList.Add(debit_Movement(entity.Status.Stock.InStock,
                                                App.Names.ProductionExecustion,
                                                detail.id_production_execution,
                                                item_product.id_item_product,
                                                (int)production_execution.production_line.id_location,
                                                detail.quantity,
                                                production_execution.trans_date,
                                                comment_Generator(App.Names.ProductionExecustion, production_execution.id_production_execution.ToString(), "")
                                            ));
                    }

                    foreach (item_movement object_Movement in _item_movementList)
                    {
                        decimal qty_ExexustionDetail = detail.quantity;

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
                                item_movement = debit_Movement(entity.Status.Stock.InStock,
                                                        App.Names.ProductionExecustion,
                                                        (int)detail.id_production_execution,
                                                        item_product.id_item_product,
                                                        (int)production_execution.production_line.id_location,
                                                        movement_debit_quantity,
                                                        production_execution.trans_date,
                                                        comment_Generator(App.Names.ProductionExecustion, 
                                                        production_execution.id_production_execution.ToString(), ""));
                            }
                            else
                            {
                                //If input is false, then we should CREDIT Stock.
                                item_movement = credit_Movement(entity.Status.Stock.InStock,
                                                        App.Names.ProductionExecustion,
                                                        (int)detail.id_production_execution,
                                                        item_product.id_item_product,
                                                        (int)production_execution.production_line.id_location,
                                                        qty_ExexustionDetail,
                                                        production_execution.trans_date,
                                                        comment_Generator(App.Names.ProductionExecustion, 
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
        public List<item_movement> revert_Stock(db db,App.Names Application_ID, int Transaction_ID)
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

        private string comment_Generator(App.Names AppName, string TransNumber, string ContactName)
        {
            string strAPP = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + AppName.ToString());
            return string.Format(strAPP + " {0} / {1}", TransNumber, ContactName);
        }

        private item_movement credit_Movement(
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

        private item_movement debit_Movement(
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
            item_movement.debit = Quantity;
            item_movement.credit = 0;
            item_movement.status = Status;
            item_movement.id_location = LocationID;
            item_movement.id_application = ApplicationID;
            item_movement.transaction_id = TransactionID;
            item_movement.trans_date = TransDate;
            return item_movement;
        }

        private item_product FindNFix_ItemProduct(item item)
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

        private int FindNFix_Location(item_product item_product, app_location app_location, app_branch app_branch)
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
