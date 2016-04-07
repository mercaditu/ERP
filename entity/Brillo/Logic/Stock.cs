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

                foreach (sales_invoice_detail detail in sales_invoice.sales_invoice_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {
                    item_product item_product = FindNFix_ItemProduct(detail.item);
                    detail.id_location = FindNFix_Location(item_product, detail.app_location, sales_invoice.app_branch);
                    detail.app_location = db.app_location.Where(x => x.id_location == detail.id_location).FirstOrDefault();
                    sales_invoice.app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == sales_invoice.id_currencyfx).FirstOrDefault();
                    List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == detail.app_location.id_location
                                                                      && x.id_item_product == item_product.id_item_product
                                                                      && x.status == entity.Status.Stock.InStock
                                                                      && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();
                    
                    item_movementList = DebitOnly_MovementLIST(Items_InStockLIST, entity.Status.Stock.InStock,
                                             App.Names.SalesInvoice,
                                             detail.id_sales_invoice,
                                             sales_invoice.app_currencyfx,
                                             item_product,
                                             detail.app_location,
                                             detail.quantity,
                                             sales_invoice.trans_date,
                                             comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name)
                                             );
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
                                             sales_return.app_currencyfx,
                                             item_product,
                                             sales_return_detail.app_location,
                                             sales_return_detail.quantity,
                                             sales_return.trans_date,
                                             sales_return_detail.unit_cost,
                                             comment_Generator(App.Names.SalesReturn, sales_return.number, sales_return.contact.name)
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
                    item_movementList = DebitOnly_MovementLIST(Items_InStockLIST, entity.Status.Stock.InStock,
                                             App.Names.PurchaseReturn,
                                             purchase_return_detail.id_purchase_return,
                                             purchase_return.app_currencyfx,
                                             item_product,
                                             purchase_return_detail.app_location,
                                             purchase_return_detail.quantity,
                                             purchase_return.trans_date,
                                             comment_Generator(App.Names.PurchaseReturn, purchase_return.number, purchase_return.contact.name)
                                             );
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

                    //Improve Comment. More standarized.
                    item_movementList.Add(
                        CreditOnly_Movement(
                            entity.Status.Stock.InStock,
                            App.Names.PurchaseInvoice,
                            purchase_invoice_detail.id_purchase_invoice,
                            purchase_invoice.app_currencyfx,
                            item_product,
                            purchase_invoice_detail.app_location,
                            purchase_invoice_detail.quantity,
                            purchase_invoice.trans_date,
                            purchase_invoice_detail.unit_cost,
                            comment_Generator(App.Names.PurchaseInvoice, purchase_invoice.number, purchase_invoice.contact.name)
                            ));
                }
                //Return List so we can save into context.
                return item_movementList;
            }
            //PRODUCTION EXECUTION
            if (obj_entity.GetType().BaseType == typeof(production_execution) || obj_entity.GetType() == typeof(production_execution))
            {
                List<item_movement> item_movementinput = new List<item_movement>();
                List<item_movement> item_movementoutput = new List<item_movement>();
                production_execution production_execution = (production_execution)obj_entity;

                foreach (production_execution_detail detail in production_execution.production_execution_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {
                    if (detail.item.id_item_type == item.item_type.Product || detail.item.id_item_type == item.item_type.RawMaterial || detail.item.id_item_type == item.item_type.Supplies)
                    {
                        item_product item_product = FindNFix_ItemProduct(detail.item);

                        if ((detail.is_input && (detail.State==EntityState.Added)) //|| 
                            //(!detail.is_input && production_execution.State == System.Data.Entity.EntityState.Deleted)
                            )
                        {

                            if (detail.quantity > 0)
                            {
                                List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == production_execution.production_line.id_location
                                                                    && x.id_item_product == item_product.id_item_product
                                                                    && x.status == entity.Status.Stock.InStock
                                                                    && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                                item_movementinput.AddRange(
                                    DebitOnly_MovementLIST(Items_InStockLIST, entity.Status.Stock.InStock,
                                                        App.Names.ProductionExecution,
                                               detail.id_production_execution,
                                            Currency.get_Default(db,CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                                                        item_product,
                                                        production_execution.production_line.app_location,
                                               detail.quantity,
                                                        production_execution.trans_date,
                                                        comment_Generator(App.Names.ProductionExecution,
                                                        production_execution.id_production_execution.ToString(), "")
                                               ));
                            }
                            else
                            {
                                item_movementinput.Add(CreditOnly_Movement(entity.Status.Stock.InStock,
                                           App.Names.ProductionExecution,
                                           detail.id_production_execution,
                                           Currency.get_Default(db, CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                                           item_product,
                                           production_execution.production_line.app_location,
                                           detail.quantity,
                                           production_execution.trans_date,
                                           0,
                                           comment_Generator(App.Names.ProductionExecution, production_execution.id_production_execution.ToString(), "")
                                       ));

                            }
                        }
                        //else if ( //(detail.is_input && production_execution.State == System.Data.Entity.EntityState.Deleted) || 
                        //            (detail.is_input == false && (detail.State == EntityState.Added)))
                        //{
                        //    if (detail.quantity > 0)
                        //    {
                        //        item_movementList.Add(
                        //                CreditOnly_Movement(entity.Status.Stock.InStock,
                        //                                App.Names.ProductionExecution,
                        //                                detail.id_production_execution,
                        //                                Currency.get_Default(db,CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                        //                                item_product,
                        //                                production_execution.production_line.app_location,
                        //                                detail.quantity,
                        //                                production_execution.trans_date,
                        //                                //Pankeel -> this needs to be fixed. I need the sum of all child.
                        //                                0,
                        //                                comment_Generator(App.Names.ProductionExecution, production_execution.id_production_execution.ToString(), "")
                        //                                )           
                        //                            );
                        //    }
                        //    else
                        //    {
                        //        List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == production_execution.production_line.id_location
                        //                                           && x.id_item_product == item_product.id_item_product
                        //                                           && x.status == entity.Status.Stock.InStock
                        //                                           && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();
                        //        item_movementList.AddRange(DebitOnly_MovementLIST(Items_InStockLIST, entity.Status.Stock.InStock,
                        //                                                        App.Names.ProductionExecution,
                        //                                                        detail.id_production_execution,
                        //                                                        Currency.get_Default(db, CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                        //                                                        item_product,
                        //                                                        production_execution.production_line.app_location,
                        //                                                        detail.quantity,
                        //                                                        production_execution.trans_date,
                        //                                                        comment_Generator(App.Names.ProductionExecution, production_execution.id_production_execution.ToString(), "")
                        //                                                      )
                        //                                  );
                        //    }
                        //}
                    }
                }

                foreach (production_execution_detail detail in production_execution.production_execution_detail
                    .Where(x => x.item.item_product.Count() > 0))
                {
                    if (detail.item.id_item_type == item.item_type.Product || detail.item.id_item_type == item.item_type.RawMaterial || detail.item.id_item_type == item.item_type.Supplies)
                    {
                        item_product item_product = FindNFix_ItemProduct(detail.item);

                        //if ((detail.is_input && (detail.State == EntityState.Added)) //|| 
                        //    //(!detail.is_input && production_execution.State == System.Data.Entity.EntityState.Deleted)
                        //    )
                        //{

                        //    if (detail.quantity > 0)
                        //    {
                        //        List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == production_execution.production_line.id_location
                        //                                            && x.id_item_product == item_product.id_item_product
                        //                                            && x.status == entity.Status.Stock.InStock
                        //                                            && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

                        //        item_movementList.AddRange(
                        //            DebitOnly_MovementLIST(Items_InStockLIST, entity.Status.Stock.InStock,
                        //                                App.Names.ProductionExecution,
                        //                       detail.id_production_execution,
                        //                    Currency.get_Default(db, CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                        //                                item_product,
                        //                                production_execution.production_line.app_location,
                        //                       detail.quantity,
                        //                                production_execution.trans_date,
                        //                                comment_Generator(App.Names.ProductionExecution,
                        //                                production_execution.id_production_execution.ToString(), "")
                        //                       ));
                        //    }
                        //    else
                        //    {
                        //        item_movementList.Add(CreditOnly_Movement(entity.Status.Stock.InStock,
                        //                   App.Names.ProductionExecution,
                        //                   detail.id_production_execution,
                        //                   Currency.get_Default(db, CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                        //                   item_product,
                        //                   production_execution.production_line.app_location,
                        //                   detail.quantity,
                        //                   production_execution.trans_date,
                        //                   0,
                        //                   comment_Generator(App.Names.ProductionExecution, production_execution.id_production_execution.ToString(), "")
                        //               ));

                        //    }
                        //}
                        //else 
                        if ( //(detail.is_input && production_execution.State == System.Data.Entity.EntityState.Deleted) || 
                                    (detail.is_input == false && (detail.State == EntityState.Added)))
                        {
                            if (detail.quantity > 0)
                            {
                                item_movementoutput.Add(
                                        CreditOnly_Movement(entity.Status.Stock.InStock,
                                                        App.Names.ProductionExecution,
                                                        detail.id_production_execution,
                                                        Currency.get_Default(db,CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                                                        item_product,
                                                        production_execution.production_line.app_location,
                                                        detail.quantity,
                                                        production_execution.trans_date,
                                                        //Pankeel -> this needs to be fixed. I need the sum of all child.
                                                        Convert.ToDecimal(item_movementinput.Sum(x=>x.item_movement_value.Sum(y=>y.unit_value))),
                                                        comment_Generator(App.Names.ProductionExecution, production_execution.id_production_execution.ToString(), "")
                                                        )           
                                                    );
                            }
                            else
                            {
                                List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == production_execution.production_line.id_location
                                                                   && x.id_item_product == item_product.id_item_product
                                                                   && x.status == entity.Status.Stock.InStock
                                                                   && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();
                                item_movementoutput.AddRange(DebitOnly_MovementLIST(Items_InStockLIST, entity.Status.Stock.InStock,
                                                                                App.Names.ProductionExecution,
                                                                                detail.id_production_execution,
                                                                                Currency.get_Default(db, CurrentSession.Id_Company).app_currencyfx.Where(x => x.is_active).FirstOrDefault(),
                                                                                item_product,
                                                                                production_execution.production_line.app_location,
                                                                                detail.quantity,
                                                                                production_execution.trans_date,
                                                                                comment_Generator(App.Names.ProductionExecution, production_execution.id_production_execution.ToString(), "")
                                                                              )
                                                          );
                            }
                        }
                    }
                }
                item_movementList.AddRange(item_movementinput);
                item_movementList.AddRange(item_movementoutput);
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

            item_movementList = db.item_movement.Where(x => x.id_application == Application_ID
                                                         && x.transaction_id == Transaction_ID)
                                                             .ToList();
            if (item_movementList != null)
            {
                return item_movementList;
            }

            return null;
        }

        public List<item_movement> DebitOnly_MovementLIST(List<item_movement> Items_InStockLIST, entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
                                                       app_currencyfx app_currencyfx, item_product item_product, app_location app_location,
                                                       decimal Quantity, DateTime TransDate, string Comment)
        {
            List<item_movement> Final_ItemMovementLIST = new List<item_movement>();

            int id_location = app_location.id_location;
            int id_item_product = item_product.id_item_product;

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
                item_movement.debit = qty_SalesDetail;
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
            //}
            return Final_ItemMovementLIST;
        }

        public item_movement CreditOnly_Movement(entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
                                              app_currencyfx app_currencyfx, item_product item_product, app_location app_location,
                                              decimal Quantity, DateTime TransDate, decimal Cost, string Comment)
        {
            if (Quantity > 0)
            {
                item_movement item_movement = new item_movement();
                item_movement.comment = Comment;
                item_movement.id_item_product = item_product.id_item_product;
                item_movement.debit = 0;
                item_movement.credit = Quantity;
                item_movement.status = Status;
                item_movement.id_location = app_location.id_location;
                item_movement.id_application = ApplicationID;
                item_movement.transaction_id = TransactionID;
                item_movement.trans_date = TransDate;

                //Logic for Value in case Parent does not Exist, we will take from 
                item_movement_value item_movement_value = new item_movement_value();
                if (Cost != 0)
                {
                    ///Bring Cost from Transaction.
                    item_movement_value.unit_value = Cost;
                }
                else
                {
                    ///If cost is 0, then take hand written cost from Items Table.
                    ///In most cases, item_unit_cost will be zero, but in case it is not, we can use it as a reference.
                    ///Also we are assuming item.unit_cost is in default currency. But if this transaction is in a different currency
                    ///we can have mis-guided information.
                    item_movement_value.unit_value = (decimal)item_product.item.unit_cost;
                }

                item_movement_value.id_currencyfx = app_currencyfx.id_currencyfx;
                item_movement_value.comment = Brillo.Localize.StringText("DirectCost");

                //Adding Value into Movement
                item_movement.item_movement_value.Add(item_movement_value);

                return item_movement;
            }

            return null;
        }

        public List<item_movement> DebitCredit_MovementList(db db, entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
                                              app_currencyfx app_currencyfx, item_product item_product, app_location app_location,
                                              decimal Quantity, DateTime TransDate, string Comment)
        {
            List<item_movement> Final_ItemMovementLIST = new List<item_movement>();

            //Bring Debit Function form above. IT should handle child and parent values.
            List<item_movement> Items_InStockLIST = db.item_movement.Where(x => x.id_location == app_location.id_location
                                                                    && x.id_item_product == item_product.id_item_product
                                                                    && x.status == entity.Status.Stock.InStock
                                                                    && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();

            List<item_movement> debit_movementLIST = new List<item_movement>();
            debit_movementLIST = DebitOnly_MovementLIST(Items_InStockLIST, Status, ApplicationID, TransactionID, app_currencyfx,
                                                    item_product, app_location, Quantity, TransDate, Comment);

            List<item_movement> credit_movementLIST = new List<item_movement>();
            foreach (item_movement debit_movement in debit_movementLIST)
            {
                item_movement credit_movement = new item_movement();
                credit_movement = 
                    CreditOnly_Movement(Status, ApplicationID, TransactionID, app_currencyfx,
                                              item_product, app_location, debit_movement.debit, TransDate, 
                                              // Pankeel -> Add cost of parent movement. Change currency to default or current currency selected.
                                              debit_movement.item_movement_value.Sum(x => x.unit_value),
                                              Comment);

                credit_movement._parent = debit_movement;
                credit_movementLIST.Add(credit_movement);
            }

            if (credit_movementLIST.Count > 0)
            {
                Final_ItemMovementLIST.AddRange(credit_movementLIST);
            }

            return Final_ItemMovementLIST;
        }

        //public item_movement Debit_Movement(entity.Status.Stock Status, App.Names ApplicationID, int TransactionID,
        //                                     item_product item_product, app_location app_location, decimal Quantity,
        //                                     DateTime TransDate, string Comment)
        //{
        //    item_movement item_movement = new item_movement();
        //    item_movement.comment = Comment;
        //    item_movement.id_item_product = item_product.id_item_product;
        //    item_movement.debit = Quantity;
        //    item_movement.credit = 0;
        //    item_movement.status = Status;
        //    item_movement.id_location = app_location.id_location;
        //    item_movement.id_application = ApplicationID;
        //    item_movement.transaction_id = TransactionID;
        //    item_movement.trans_date = TransDate;
        //    return item_movement;
        //}


        //public item_movement credit_Movement(
        //  entity.Status.Stock Status,
        //  App.Names ApplicationID,
        //  int TransactionID,
        //  int Item_ProductID,
        //  int LocationID,
        //  decimal Quantity,
        //  DateTime TransDate,
        //  string Comment)
        //{
        //    item_movement item_movement = new item_movement();
        //    item_movement.comment = Comment;
        //    item_movement.id_item_product = Item_ProductID;
        //    item_movement.debit = 0;
        //    item_movement.credit = Quantity;
        //    item_movement.status = Status;
        //    item_movement.id_location = LocationID;
        //    item_movement.id_application = ApplicationID;
        //    item_movement.transaction_id = TransactionID;
        //    item_movement.trans_date = TransDate;
        //    return item_movement;
        //}

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
