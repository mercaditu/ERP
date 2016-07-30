using System;
using System.Collections.Generic;
using System.Linq;
using WPFLocalizeExtension.Extensions;
using System.Data.Entity;

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

                foreach (sales_invoice_detail detail in sales_invoice.sales_invoice_detail)
                {

                    if (detail.item.is_autorecepie)
                    {
                        item_product item_product = FindNFix_ItemProduct(detail.item);
                        if (detail.item.item_recepie.FirstOrDefault() != null)
                        {
                            foreach (item_recepie_detail item_recepie_detail in detail.item.item_recepie.FirstOrDefault().item_recepie_detail)
                            {

                                item_product item_productSub = FindNFix_ItemProduct(item_recepie_detail.item);
                                if (item_productSub != null)
                                {

                                    if (detail.id_location == null)
                                    {
                                        detail.id_location = FindNFix_Location(item_productSub, detail.app_location, sales_invoice.app_branch);
                                        detail.app_location = db.app_location.Where(x => x.id_location == detail.id_location).FirstOrDefault();
                                    }

                                    sales_invoice.app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == sales_invoice.id_currencyfx).FirstOrDefault();
                                    List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == detail.app_location.id_location
                                           && x.id_item_product == item_productSub.id_item_product
                                           && x.status == entity.Status.Stock.InStock
                                           && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                                    item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, entity.Status.Stock.InStock,
                                                             App.Names.SalesInvoice,
                                                             detail.id_sales_invoice,
                                                             (int)detail.id_sales_invoice_detail,
                                                             sales_invoice.app_currencyfx,
                                                             item_productSub,
                                                             detail.app_location,
                                                             item_recepie_detail.quantity,
                                                             sales_invoice.trans_date,
                                                             comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name)
                                                             ));
                                }
                            }
                        }

                    }
                    else
                    {
                        item_product item_product = FindNFix_ItemProduct(detail.item);
                        if (item_product != null)
                        {
                            if (detail.id_location == null)
                            {
                                detail.id_location = FindNFix_Location(item_product, detail.app_location, sales_invoice.app_branch);
                                detail.app_location = db.app_location.Where(x => x.id_location == detail.id_location).FirstOrDefault();
                            }

                            sales_invoice.app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == sales_invoice.id_currencyfx).FirstOrDefault();
                            List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == detail.app_location.id_location
                                                                              && x.id_item_product == item_product.id_item_product
                                                                              && x.status == entity.Status.Stock.InStock
                                                                              && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                            item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, entity.Status.Stock.InStock,
                                                     App.Names.SalesInvoice,
                                                     detail.id_sales_invoice,
                                                     (int)detail.id_sales_invoice_detail,
                                                     sales_invoice.app_currencyfx,
                                                     item_product,
                                                     detail.app_location,
                                                     detail.quantity,
                                                     sales_invoice.trans_date,
                                                     comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name)
                                                     ));
                        }
                    }
                }
                //Return List so we can save into context.
                return item_movementList;
            }

            //SALES RETURN
            else if (obj_entity.GetType().BaseType == typeof(sales_return) || obj_entity.GetType() == typeof(sales_return))
            {
                sales_return sales_return = (sales_return)obj_entity;
                foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail.Where(x => x.item.item_product.Count() > 0))
                {
                    item_product item_product = FindNFix_ItemProduct(sales_return_detail.item);
                    sales_return_detail.id_location = FindNFix_Location(item_product, sales_return_detail.app_location, sales_return.app_branch);
                    sales_return_detail.app_location = db.app_location.Where(x => x.id_location == sales_return_detail.id_location).FirstOrDefault();
                    item_movementList.Add(
                        CreditOnly_Movement(entity.Status.Stock.InStock,
                                             App.Names.SalesReturn,
                                             sales_return_detail.id_sales_return,
                                             sales_return_detail.id_sales_return_detail,
                                             sales_return.app_currencyfx,
                                             item_product,
                                             sales_return_detail.app_location,
                                             sales_return_detail.quantity,
                                             sales_return.trans_date,
                                             sales_return_detail.unit_cost,
                                             comment_Generator(App.Names.SalesReturn, sales_return.number, sales_return.contact.name),
                                             null
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
                    purchase_return_detail.app_location = db.app_location.Where(x => x.id_location == purchase_return_detail.id_location).FirstOrDefault();

                    List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == purchase_return_detail.app_location.id_location
                                                                     && x.id_item_product == item_product.id_item_product
                                                                     && x.status == entity.Status.Stock.InStock
                                                                     && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();
                    item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, entity.Status.Stock.InStock,
                                             App.Names.PurchaseReturn,
                                             purchase_return_detail.id_purchase_return,
                                             purchase_return_detail.id_purchase_return_detail,
                                             purchase_return.app_currencyfx,
                                             item_product,
                                             purchase_return_detail.app_location,
                                             purchase_return_detail.quantity,
                                             purchase_return.trans_date,
                                             comment_Generator(App.Names.PurchaseReturn, purchase_return.number, purchase_return.contact.name)
                                             ));
                }
                //Return List so we can save into context.
                return item_movementList;
            }

            //PURCHASE INVOICE
            else if (obj_entity.GetType().BaseType == typeof(purchase_invoice) || obj_entity.GetType() == typeof(purchase_invoice))
            {
                purchase_invoice purchase_invoice = (purchase_invoice)obj_entity;
                List<purchase_invoice_detail> Listpurchase_invoice_detail = purchase_invoice.purchase_invoice_detail.Where(x => x.id_item > 0).ToList();

                foreach (purchase_invoice_detail purchase_invoice_detail in Listpurchase_invoice_detail.Where(x => x.item.item_product.Count() > 0))
                {
                    item_product item_product = FindNFix_ItemProduct(purchase_invoice_detail.item);
                    purchase_invoice_detail.id_location = FindNFix_Location(item_product, purchase_invoice_detail.app_location, purchase_invoice.app_branch);
                    purchase_invoice_detail.app_location = db.app_location.Where(x => x.id_location == purchase_invoice_detail.id_location).FirstOrDefault();

                    if (purchase_invoice.app_currencyfx == null)
                    {
                        purchase_invoice.app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == purchase_invoice.id_currencyfx).FirstOrDefault();
                    }

                    List<item_movement_dimension> item_movement_dimensionLIST = null;
                    if (purchase_invoice_detail.purchase_invoice_dimension.Count > 0)
                    {
                        item_movement_dimensionLIST = new List<item_movement_dimension>();
                        foreach (purchase_invoice_dimension purchase_invoice_dimension in purchase_invoice_detail.purchase_invoice_dimension)
                        {
                            item_movement_dimension item_movement_dimension = new entity.item_movement_dimension();
                            item_movement_dimension.id_dimension = purchase_invoice_dimension.id_dimension;
                            item_movement_dimension.value = purchase_invoice_dimension.value;
                            // item_movement_dimension.id_measurement = purchase_invoice_dimension.id_measurement;
                            item_movement_dimensionLIST.Add(item_movement_dimension);
                        }
                    }

                    //Improve Comment. More standarized.
                    item_movementList.Add(
                        CreditOnly_Movement(
                            entity.Status.Stock.InStock,
                            App.Names.PurchaseInvoice,
                            purchase_invoice_detail.id_purchase_invoice,
                            purchase_invoice_detail.id_purchase_invoice_detail,
                            purchase_invoice.app_currencyfx,
                            item_product,
                            purchase_invoice_detail.app_location,
                            purchase_invoice_detail.quantity,
                            purchase_invoice.trans_date,
                            purchase_invoice_detail.unit_cost,
                            comment_Generator(App.Names.PurchaseInvoice, purchase_invoice.number != null ? purchase_invoice.number : "", purchase_invoice.contact.name), item_movement_dimensionLIST
                            ));
                }
                //Return List so we can save into context.
                return item_movementList;
            }

                ///SALES PACKING
            else if (obj_entity.GetType().BaseType == typeof(sales_packing) || obj_entity.GetType() == typeof(sales_packing))
            {
                sales_packing sales_packing = (sales_packing)obj_entity;

                foreach (sales_packing_detail packing_detail in sales_packing.sales_packing_detail
                    .Where(x => x.item.item_product != null))
                {
                    item_product item_product = FindNFix_ItemProduct(packing_detail.item);
                    if (packing_detail.id_location != null && packing_detail.app_location != null)
                    {
                        packing_detail.id_location = FindNFix_Location(item_product, packing_detail.app_location, sales_packing.app_branch);
                        packing_detail.app_location = db.app_location.Where(x => x.id_location == packing_detail.id_location).FirstOrDefault();

                    }


                    List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == packing_detail.app_location.id_location
                                                                      && x.id_item_product == item_product.id_item_product
                                                                      && x.status == entity.Status.Stock.InStock
                                                                      && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                    item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, entity.Status.Stock.InStock,
                                             App.Names.PackingList,
                                             packing_detail.id_sales_packing,
                                             packing_detail.id_sales_packing_detail,
                                             null,
                                             item_product,
                                             packing_detail.app_location,
                                             packing_detail.quantity,
                                             sales_packing.trans_date,
                                             comment_Generator(App.Names.PackingList, sales_packing.number, sales_packing.contact.name)
                                             ));
                }
                //Return List so we can save into context.
                return item_movementList;
            }
            //PRODUCTION EXECUTION
            else if (obj_entity.GetType().BaseType == typeof(production_execution_detail) || obj_entity.GetType() == typeof(production_execution_detail))
            {
                List<item_movement> item_movementINPUT = new List<item_movement>();
                List<item_movement> item_movementOUTPUT = new List<item_movement>();

                production_execution_detail detail = (production_execution_detail)obj_entity;

                if (detail.item.id_item_type == item.item_type.Product || detail.item.id_item_type == item.item_type.RawMaterial || detail.item.id_item_type == item.item_type.Supplies)
                {
                    item_product item_product = FindNFix_ItemProduct(detail.item);

                    //INPUT (DISCOUNT)
                    if (detail.is_input)
                    {
                        if (detail.quantity > 0)
                        {
                            List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == detail.production_execution.production_line.id_location
                                                                && x.id_item_product == item_product.id_item_product
                                                                && x.status == entity.Status.Stock.InStock
                                                                && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                            item_movementINPUT.AddRange(
                                DebitOnly_MovementLIST(db, Items_InStockLIST, entity.Status.Stock.InStock,
                                                    App.Names.ProductionExecution,
                                                    detail.id_production_execution,
                                                    detail.id_execution_detail,
                                                    Currency.get_Default(db).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                                                    item_product,
                                                    detail.production_execution.production_line.app_location,
                                                    detail.quantity,
                                                    DateTime.Now,
                                                    comment_Generator
                                                    (App.Names.ProductionExecution,
                                                    (detail.production_execution.production_order != null ? detail.production_execution.production_order.work_number : ""),
                                                    "")));
                            detail.unit_cost = item_movementINPUT.Sum(x => x.item_movement_value.Sum(y => y.unit_value));
                        }
                    }
                }

                if (detail.item.id_item_type == item.item_type.Product || detail.item.id_item_type == item.item_type.RawMaterial || detail.item.id_item_type == item.item_type.Supplies)
                {
                    item_product item_product = FindNFix_ItemProduct(detail.item);

                    //OUTPUT. CREDIT
                    if (detail.is_input == false)
                    {
                        if (detail.quantity > 0)
                        {
                            item_movementINPUT = db.item_movement.Where(x => x.production_execution_detail.id_production_execution == detail.parent.id_production_execution).ToList(); //detail.parent.id_production_execution

                            if (item_movementINPUT.Count() > 0)
                            {
                                bool CostDimension = false;
                                decimal InputDimension = 1;
                                decimal OutPutDimension = 1;

                                decimal Cost = Convert.ToDecimal(item_movementINPUT.Sum(x => x.item_movement_value.Sum(y => y.unit_value)));

                                foreach (item_movement_dimension item_movement_dimension in item_movementINPUT.FirstOrDefault().item_movement_dimension)
                                {
                                    CostDimension = true;
                                    InputDimension *= item_movement_dimension.value;
                                }

                                foreach (production_execution_dimension production_execution_dimension in detail.production_execution_dimension)
                                {
                                    CostDimension = true;
                                    OutPutDimension *= production_execution_dimension.value;
                                }

                                if (CostDimension)
                                {
                                    decimal Percentage = ((OutPutDimension * 100) / InputDimension) / 100;
                                    Cost = Cost * Percentage;
                                }

                                item_movementOUTPUT.Add(
                                        CreditOnly_Movement(entity.Status.Stock.InStock,
                                                        App.Names.ProductionExecution,
                                                        detail.id_production_execution,
                                                        detail.id_execution_detail,
                                                        Currency.get_Default(db).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                                                        item_product,
                                                        detail.production_execution.production_line.app_location,
                                                        detail.quantity,
                                                        detail.production_execution.trans_date,
                                                        Cost,
                                                        comment_Generator
                                                        (App.Names.ProductionExecution,
                                                        (detail.production_execution.production_order != null ? detail.production_execution.production_order.work_number : ""),
                                                        ""),
                                                        null)
                                                    );

                                detail.unit_cost = Cost;   
                            }
                        }
                    }
                }
                item_movementList.AddRange(item_movementINPUT);
                item_movementList.AddRange(item_movementOUTPUT);
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
        public List<item_movement> revert_Stock(db db, App.Names Application_ID, object Transcation)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            if (Application_ID == App.Names.Transfer)
            {
                item_transfer item_transfer = Transcation as item_transfer;
                foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail)
                {
                    // item_movement.transaction_id = TransactionID;
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_transfer_detail == item_transfer_detail.id_transfer)
                                                                     .ToList());
                }

            }
            else if (Application_ID == App.Names.ProductionExecution)
            {
                production_execution production_execution = Transcation as production_execution;
                foreach (production_execution_detail production_execution_detail in production_execution.production_execution_detail)
                {
                    // item_movement.transaction_id = TransactionID;
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_execution_detail == production_execution_detail.id_execution_detail)
                                                                     .ToList());
                }

            }
            else if (Application_ID == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = Transcation as purchase_invoice;
                foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice.purchase_invoice_detail)
                {
                    // item_movement.transaction_id = TransactionID;
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail)
                                                                     .ToList());
                }


            }
            else if (Application_ID == App.Names.PurchaseReturn)
            {
                purchase_return purchase_return = Transcation as purchase_return;
                foreach (purchase_return_detail purchase_return_detail in purchase_return.purchase_return_detail)
                {
                    // item_movement.transaction_id = TransactionID;
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_purchase_return_detail == purchase_return_detail.id_purchase_return_detail)
                                                                     .ToList());
                }


            }
            else if (Application_ID == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = Transcation as sales_invoice;
                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
                {
                    // item_movement.transaction_id = TransactionID;
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail)
                                                                     .ToList());
                }

            }
            else if (Application_ID == App.Names.SalesReturn)
            {
                sales_return sales_return = Transcation as sales_return;
                foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail)
                {
                    // item_movement.transaction_id = TransactionID;
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_sales_return_detail == sales_return_detail.id_sales_return_detail)
                                                                     .ToList());
                }

            }
            else if (Application_ID == App.Names.PackingList)
            {
                sales_packing sales_packing = Transcation as sales_packing;
                foreach (sales_packing_detail sales_packing_detail in sales_packing.sales_packing_detail)
                {
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_sales_packing_detail == sales_packing_detail.id_sales_packing_detail).ToList());
                }
            }

            if (item_movementList != null)
            {
                return item_movementList;
            }

            return null;
        }

        public List<item_movement> DebitOnly_MovementLIST(db db, List<item_movement> Items_InStockLIST, entity.Status.Stock Status, App.Names ApplicationID, int TransactionID, int TransactionDetailID,
                                                       app_currencyfx app_currencyfx, item_product item_product, app_location app_location,
                                                       decimal Quantity, DateTime TransDate, string Comment)
        {
            List<item_movement> Final_ItemMovementLIST = new List<item_movement>();

            int id_location = app_location.id_location;
            int id_item_product = item_product.id_item_product;
            int id_movement = 0;

            if (item_product.cogs_type == item_product.COGS_Types.LIFO && Items_InStockLIST != null)
            {
                Items_InStockLIST = Items_InStockLIST.OrderByDescending(x => x.trans_date).ToList();
            }
            else if (Items_InStockLIST != null)
            {
                Items_InStockLIST = Items_InStockLIST.OrderBy(x => x.trans_date).ToList();
            }

            decimal qty_SalesDetail = Quantity;

            ///Will create new Item Movements 
            ///if split from Parents is needed.
            foreach (item_movement parent_Movement in Items_InStockLIST)
            {
                if (qty_SalesDetail > 0)
                {
                    item_movement item_movement = new item_movement();

                    //THIS BLOCK OF CODE IS WRONG.
                    decimal movement_debit_quantity = qty_SalesDetail;

                    //If Parent Movement is lesser than Quantity, then only take total value of Parent.
                    if (parent_Movement.credit <= qty_SalesDetail)
                    {
                        movement_debit_quantity = parent_Movement.credit;
                    }

                    item_movement.comment = Comment;
                    item_movement.id_item_product = item_product.id_item_product;
                    item_movement.debit = movement_debit_quantity;
                    item_movement.credit = 0;
                    item_movement.status = Status;
                    item_movement.id_location = app_location.id_location;
                    item_movement._parent = parent_Movement;

                    if (ApplicationID == App.Names.Transfer)
                    {
                        item_movement.id_transfer_detail = TransactionDetailID;
                    }
                    else if (ApplicationID == App.Names.ProductionExecution)
                    {
                        item_movement.id_execution_detail = TransactionDetailID;

                        if (db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault() != null && db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault().movement_id != null)
                        {
                            id_movement = (int)db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault().movement_id;
                            item_movement._parent = db.item_movement.Where(x => x.id_movement == id_movement).FirstOrDefault();
                        }
                    }
                    else if (ApplicationID == App.Names.PurchaseInvoice)
                    {
                        item_movement.id_purchase_invoice_detail = TransactionDetailID;
                    }
                    else if (ApplicationID == App.Names.PurchaseReturn)
                    {
                        item_movement.id_purchase_return_detail = TransactionDetailID;
                    }
                    else if (ApplicationID == App.Names.SalesInvoice)
                    {
                        item_movement.id_sales_invoice_detail = TransactionDetailID;
                    }
                    else if (ApplicationID == App.Names.SalesReturn)
                    {
                        item_movement.id_sales_return_detail = TransactionDetailID;
                    }
                    else if (ApplicationID == App.Names.PackingList)
                    {
                        item_movement.id_sales_packing_detail = TransactionDetailID;
                    }

                    item_movement.trans_date = TransDate;

                    decimal Dimension_Cost = 0;
                    if (ApplicationID == App.Names.ProductionExecution)
                    {

                        if (db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault() != null)
                        {
                            production_execution_detail production_execution_detail = db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault();
                            if (production_execution_detail.production_execution.production_order.types == production_order.ProductionOrderTypes.Fraction)
                            {
                                if (parent_Movement.item_movement_dimension != null && item_movement.item_movement_dimension != null)
                                {
                                    decimal ParentDimesion = 0;
                                    decimal ChildDimesion = 0;

                                    foreach (item_movement_dimension item_movement_dimension in parent_Movement.item_movement_dimension)
                                    {
                                        ParentDimesion = ParentDimesion * item_movement_dimension.value;
                                    }
                                    foreach (item_movement_dimension item_movement_dimension in item_movement.item_movement_dimension)
                                    {
                                        ChildDimesion = ChildDimesion * item_movement_dimension.value;
                                    }


                                    if (ParentDimesion > 0 && ChildDimesion > 0)
                                    {
                                        Dimension_Cost = parent_Movement.GetValue_ByCurrency(app_currencyfx.app_currency);
                                        decimal ChildPaticipantion = (ChildDimesion / ParentDimesion);
                                        Dimension_Cost = Dimension_Cost * ChildPaticipantion;
                                    }

                                }

                            }

                        }

                    }

                    //Logic for Value
                    if (app_currencyfx != null)
                    {
                        item_movement_value item_movement_value = new item_movement_value();
                        if (Dimension_Cost > 0)
                        {
                            item_movement_value.unit_value = Dimension_Cost;
                        }
                        else
                        {
                            item_movement_value.unit_value = parent_Movement.GetValue_ByCurrency(app_currencyfx.app_currency);
                        }

                        item_movement_value.id_currencyfx = app_currencyfx.id_currencyfx;
                        item_movement_value.comment = Brillo.Localize.StringText("DirectCost");
                        item_movement.item_movement_value.Add(item_movement_value);
                    }

                    foreach (item_movement_dimension item_movement_dimension in parent_Movement.item_movement_dimension)
                    {
                        item_movement_dimension _item_movement_dimension = new item_movement_dimension();
                        _item_movement_dimension.id_dimension = item_movement_dimension.id_dimension;
                        _item_movement_dimension.value = item_movement_dimension.value;
                        item_movement.item_movement_dimension.Add(_item_movement_dimension);
                    }

                    //Adding into List
                    Final_ItemMovementLIST.Add(item_movement);
                    qty_SalesDetail = qty_SalesDetail - parent_Movement.credit;
                }
            }

            ///In case Parent does not exist, will enter this code.
            if (qty_SalesDetail > 0)
            {
                id_movement = 0;
                item_movement item_movement = new item_movement();
                //Adding into List if Movement List for this Location is empty.
                item_movement.comment = Comment;
                item_movement.id_item_product = item_product.id_item_product;
                item_movement.debit = qty_SalesDetail;
                item_movement.credit = 0;
                item_movement.status = Status;
                item_movement.id_location = app_location.id_location;
                item_movement._parent = null;
                if (ApplicationID == App.Names.Transfer)
                {
                    item_movement.id_transfer_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.ProductionExecution)
                {
                    item_movement.id_execution_detail = TransactionDetailID;

                    if (db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault() != null)
                    {
                        production_execution_detail production_execution_detail = db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault();
                        foreach (production_execution_dimension production_execution_dimension in production_execution_detail.production_execution_dimension)
                        {
                            item_movement_dimension _item_movement_dimension = new item_movement_dimension();
                            _item_movement_dimension.id_dimension = production_execution_dimension.id_dimension;
                            _item_movement_dimension.value = production_execution_dimension.value;
                            item_movement.item_movement_dimension.Add(_item_movement_dimension);
                        }
                    }
                }
                else if (ApplicationID == App.Names.PurchaseInvoice)
                {
                    item_movement.id_purchase_invoice_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.PurchaseReturn)
                {
                    item_movement.id_purchase_return_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.SalesInvoice)
                {
                    item_movement.id_sales_invoice_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.SalesReturn)
                {
                    item_movement.id_sales_return_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.PackingList)
                {
                    item_movement.id_sales_packing_detail = TransactionDetailID;
                }

                // item_movement.transaction_id = TransactionID;
                item_movement.trans_date = TransDate;



                if (app_currencyfx != null)
                {
                    //Logic for Value in case Parent does not Exist, we will take from 

                    item_movement_value item_movement_value = new item_movement_value();
                    if (item_product.item.unit_cost != null)
                    {
                        item_movement_value.unit_value = (decimal)item_product.item.unit_cost;
                    }

                    item_movement_value.id_currencyfx = app_currencyfx.id_currencyfx;
                    item_movement_value.comment = Brillo.Localize.StringText("DirectCost");
                    item_movement.item_movement_value.Add(item_movement_value);
                    //Adding into List

                }


                Final_ItemMovementLIST.Add(item_movement);
            }
            //}
            return Final_ItemMovementLIST;
        }

        public item_movement CreditOnly_Movement(entity.Status.Stock Status, App.Names ApplicationID, int TransactionID, int TransactionDetailID,
                                              app_currencyfx app_currencyfx, item_product item_product, app_location app_location,
                                              decimal Quantity, DateTime TransDate, decimal Cost, string Comment, List<item_movement_dimension> DimensionList)
        {
            int id_movement = 0;
            if (Quantity > 0)
            {
                item_movement item_movement = new item_movement();
                item_movement.comment = Comment;
                item_movement.id_item_product = item_product.id_item_product;
                item_movement.debit = 0;
                item_movement.credit = Quantity;
                item_movement.status = Status;
                item_movement.id_location = app_location.id_location;

                if (ApplicationID == App.Names.Transfer)
                {
                    item_movement.id_transfer_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.ProductionExecution)
                {
                    item_movement.id_execution_detail = TransactionDetailID;
                    using (db db = new db())
                    {
                        if (db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault() != null && db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault().movement_id != null)
                        {
                            id_movement = (int)db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault().movement_id;

                        }

                        if (DimensionList == null)
                        {
                            if (db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault() != null)
                            {
                                production_execution_detail production_execution_detail = db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).FirstOrDefault();
                                foreach (production_execution_dimension production_execution_dimension in production_execution_detail.production_execution_dimension)
                                {
                                    item_movement_dimension _item_movement_dimension = new item_movement_dimension();
                                    _item_movement_dimension.id_dimension = production_execution_dimension.id_dimension;
                                    _item_movement_dimension.value = production_execution_dimension.value;
                                    item_movement.item_movement_dimension.Add(_item_movement_dimension);
                                }
                            }
                        }

                    }
                }
                else if (ApplicationID == App.Names.PurchaseInvoice)
                {
                    item_movement.id_purchase_invoice_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.PurchaseReturn)
                {
                    item_movement.id_purchase_return_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.SalesInvoice)
                {
                    item_movement.id_sales_invoice_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.SalesReturn)
                {
                    item_movement.id_sales_return_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.PackingList)
                {
                    item_movement.id_sales_packing_detail = TransactionDetailID;
                }
                // item_movement.transaction_id = TransactionID;
                item_movement.trans_date = TransDate;

                //Logic for Value in case Parent does not Exist, we will take from 
                item_movement_value item_movement_value = new item_movement_value();

                if (Cost != 0)
                {
                    ///Bring Cost from Transaction.

                    using (db db = new db())
                    {
                        int ID_CurrencyFX_Default = Currency.get_Default(db).app_currencyfx.Where(x => x.is_active).FirstOrDefault().id_currencyfx;
                        decimal DefaultCurrency_Cost = Currency.convert_Values(Cost, app_currencyfx.id_currencyfx, ID_CurrencyFX_Default, null);

                        item_movement_value.unit_value = DefaultCurrency_Cost;
                        item_movement_value.id_currencyfx = ID_CurrencyFX_Default;
                        item_movement_value.comment = Brillo.Localize.StringText("DirectCost");

                        //Adding Value into Movement
                        item_movement.item_movement_value.Add(item_movement_value);
                    }
                }
                else
                {
                    ///If cost is 0, then take hand written cost from Items Table.
                    ///In most cases, item_unit_cost will be zero, but in case it is not, we can use it as a reference.
                    ///Also we are assuming item.unit_cost is in default currency. But if this transaction is in a different currency
                    ///we can have mis-guided information.

                    if (item_product.item.unit_cost != null)
                    {
                        item_movement_value.unit_value = (decimal)item_product.item.unit_cost;
                        item_movement_value.id_currencyfx = app_currencyfx.id_currencyfx;
                        item_movement_value.comment = Brillo.Localize.StringText("DirectCost");

                        //Adding Value into Movement
                        item_movement.item_movement_value.Add(item_movement_value);
                    }
                }

                if (DimensionList != null)
                {
                    foreach (item_movement_dimension item_movement_dimension in DimensionList)
                    {
                        item_movement.item_movement_dimension.Add(item_movement_dimension);
                    }
                }
                return item_movement;
            }
            return null;
        }

        #region Helpers

        public string comment_Generator(App.Names AppName, string TransNumber, string ContactName)
        {
            string strAPP = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + AppName.ToString());
            return string.Format(strAPP + " {0} | {1}", TransNumber, ContactName);
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

        #endregion
    }
}
