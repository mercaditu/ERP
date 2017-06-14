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

            //PRODUCTION EXECUTION
            if (obj_entity.GetType().BaseType == typeof(production_execution_detail) || obj_entity.GetType() == typeof(production_execution_detail))
            {
                List<item_movement> item_movementINPUT = new List<item_movement>();
                List<item_movement> item_movementOUTPUT = new List<item_movement>();

                production_execution_detail production_execution_detail = (production_execution_detail)obj_entity;

                if (production_execution_detail.item.id_item_type == item.item_type.Product ||
                    production_execution_detail.item.id_item_type == item.item_type.RawMaterial ||
                    production_execution_detail.item.id_item_type == item.item_type.Supplies)
                {
                    item_product item_product = FindNFix_ItemProduct(production_execution_detail.item);

                    //INPUT (DISCOUNT)
                    if (production_execution_detail.is_input)
                    {
                        if (production_execution_detail.quantity > 0)
                        {
                            List<StockList> Items_InStockLIST = null;

                            //If Detail has an associated Id Movement. Use it, else List FIFO.
                            if (production_execution_detail.movement_id != null && production_execution_detail.movement_id > 0)
                            {
                                Brillo.Stock stockBrillo = new Brillo.Stock();
                                Items_InStockLIST = stockBrillo.ScalarMovement((long)production_execution_detail.movement_id);
                            }
                            else
                            {
                                Brillo.Stock stockBrillo = new Brillo.Stock();
                                app_location app_location = db.app_location.Find(production_execution_detail.production_order_detail.production_order.production_line.id_location);
                                Items_InStockLIST = stockBrillo.List(app_location.id_branch, app_location.id_location, item_product.id_item_product);
                            }

                            item_movementINPUT.AddRange(
                                DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                    App.Names.ProductionExecution,
                                                    production_execution_detail.production_order_detail.id_production_order,
                                                    production_execution_detail.id_execution_detail,
                                                    CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                                    item_product,
                                                    production_execution_detail.production_order_detail.production_order.production_line.id_location,
                                                    production_execution_detail.quantity,
                                                    DateTime.Now,
                                                    comment_Generator
                                                    (App.Names.ProductionExecution,
                                                    (production_execution_detail.production_order_detail.production_order != null ? production_execution_detail.production_order_detail.production_order.work_number : ""),
                                                    "")));

                            if (item_movementINPUT.Count() > 0)
                            {
                                item_movementList.AddRange(item_movementINPUT);
                                production_execution_detail.unit_cost = item_movementINPUT.FirstOrDefault().item_movement_value.Sum(y => y.unit_value);
                            }
                        }
                    }
                }

                if (production_execution_detail.item.id_item_type == item.item_type.Product ||
                    production_execution_detail.item.id_item_type == item.item_type.RawMaterial ||
                    production_execution_detail.item.id_item_type == item.item_type.Supplies)
                {
                    item_product item_product = FindNFix_ItemProduct(production_execution_detail.item);
                    decimal Cost = 0;

                    //OUTPUT. CREDIT Stock.
                    if (production_execution_detail.production_order_detail.is_input == false)
                    {
                        if (production_execution_detail.quantity > 0)
                        {
                            decimal PercentOfTotal = 0M;
                            List<item_movement_dimension> OutputMovementDimensionLIST = new List<item_movement_dimension>();

                            if (production_execution_detail.production_order_detail.production_order.type == production_order.ProductionOrderTypes.Fraction)
                            {
                                if (production_execution_detail.production_order_detail.parent != null)
                                {
                                    production_order_detail _parent_order_detail = production_execution_detail.production_order_detail.parent;
                                    production_execution_detail _parent_execution_detail = _parent_order_detail.production_execution_detail.FirstOrDefault();

                                    item_movementINPUT = db.item_movement.Where(x => x.id_execution_detail == _parent_execution_detail.id_execution_detail).ToList();
                                }

                                if (item_movementINPUT.Count() > 0)
                                {
                                    if (item_movementINPUT.FirstOrDefault().item_movement_dimension.Count > 0)
                                    {
                                        bool CostDimension = false;
                                        decimal InputDimension = 1;
                                        decimal OutPutDimension = 1;

                                        foreach (item_movement_dimension item_movement_dimension in item_movementINPUT.FirstOrDefault().item_movement_dimension)
                                        {
                                            CostDimension = true;
                                            InputDimension *= item_movement_dimension.value;
                                        }

                                        foreach (production_execution_dimension production_execution_dimension in production_execution_detail.production_execution_dimension)
                                        {
                                            item_movement_dimension item_movement_dimension = new item_movement_dimension();
                                            item_movement_dimension.id_dimension = production_execution_dimension.id_dimension;
                                            item_movement_dimension.value = production_execution_dimension.value;
                                            OutputMovementDimensionLIST.Add(item_movement_dimension);

                                            CostDimension = true;
                                            OutPutDimension *= production_execution_dimension.value;
                                        }

                                        if (CostDimension)
                                        {
                                            PercentOfTotal = (OutPutDimension / InputDimension);
                                        }
                                    }
                                }
                                else
                                {
                                    PercentOfTotal = 1;
                                }
                            }
                            //Else for Normal Production.
                            else
                            {
                                PercentOfTotal = 1;
                            }

                            production_order_detail parent_order_detail = production_execution_detail.production_order_detail.parent;
                            if (parent_order_detail != null)
                            {
                                Cost = parent_order_detail.production_execution_detail.Sum(x => x.unit_cost) * PercentOfTotal;
                            }
                            else if (production_execution_detail.production_order_detail.child.Count() > 0)
                            {
                                if (production_execution_detail.quantity > 0)
                                {
                                    foreach (production_order_detail child_order in production_execution_detail.production_order_detail.child)
                                    {
                                        foreach (production_execution_detail child_exe in child_order.production_execution_detail)
                                        {
                                            Cost += (child_exe.quantity * child_exe.unit_cost * PercentOfTotal) / production_execution_detail.quantity;
                                        }
                                    }
                                }
                            }

                            //In case of wrong configuration.
                            production_execution_detail.unit_cost = Cost;

                            item_movementOUTPUT.Add(
                                    CreditOnly_Movement(Status.Stock.InStock,
                                                    App.Names.ProductionExecution,
                                                    production_execution_detail.production_order_detail.id_production_order,
                                                    production_execution_detail.id_execution_detail,
                                                    CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                                    item_product.id_item_product,
                                                    production_execution_detail.production_order_detail.production_order.production_line.id_location,
                                                    production_execution_detail.quantity,
                                                    production_execution_detail.trans_date,
                                                    Cost,
                                                    comment_Generator
                                                    (App.Names.ProductionExecution,
                                                    (production_execution_detail.production_order_detail.production_order != null ? production_execution_detail.production_order_detail.production_order.work_number : ""),
                                                    ""),
                                                    OutputMovementDimensionLIST, null, null)
                                                );
                            item_movementList.AddRange(item_movementOUTPUT);
                        }
                    }
                }
                //Return List so we can save into context.
                return item_movementList;
            }
            return null;
        }

        public List<item_movement> SalesPacking_Approve(db db, sales_packing sales_packing)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            //Just bring Sales Packing that has Item Product and No relation to Sales Invoice. This will help discount stock only for thse
            //that are not linked to Sales Invoice. If linked with Sales Invoice, the stock will get discounted there forcefully.
            foreach (sales_packing_detail packing_detail in
                sales_packing.sales_packing_detail
                .Where(x => x.item.item_product.Count() > 0 
                && x.sales_packing_relation.Count() == 0 
                && x.user_verified))
            {
                item_product item_product = FindNFix_ItemProduct(packing_detail.item);

                int LocationID = 0;
                if (packing_detail.id_location == null)
                {
                    LocationID = FindNFix_Location(item_product, packing_detail.app_location, sales_packing.app_branch);
                    packing_detail.app_location = db.app_location.Find(LocationID);
                }
                else
                {
                    packing_detail.app_location = db.app_location.Find(packing_detail.id_location);
                    LocationID = (int)packing_detail.id_location;
                }

                List<StockList> Items_InStockLIST = null;
                if (packing_detail.id_movement != null && packing_detail.id_movement > 0)
                {
                    Brillo.Stock stockBrillo = new Brillo.Stock();
                    Items_InStockLIST = stockBrillo.ScalarMovement((long)packing_detail.id_movement);
                }
                else
                {
                    Brillo.Stock stock = new Brillo.Stock();
                    Items_InStockLIST = stock.List(packing_detail.app_location.id_branch, LocationID, item_product.id_item_product);
                }

                item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                         App.Names.PackingList,
                                         packing_detail.id_sales_packing,
                                         packing_detail.id_sales_packing_detail,
                                         CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                         item_product,
                                         LocationID,
                                         (int)packing_detail.verified_quantity,
                                         sales_packing.trans_date,
                                         comment_Generator(App.Names.PackingList, sales_packing.number, sales_packing.contact.name)
                                         ));
            }
            //Return List so we can save into context.
            return item_movementList;
        }

        public List<item_movement> PurchasePacking_Approve(db db, purchase_packing purchase_packing)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            foreach (purchase_packing_detail packing_detail in purchase_packing.purchase_packing_detail
                .Where(x => x.item.item_product.Count() > 0 && x.verified_by != null))
            {
                item_product item_product = FindNFix_ItemProduct(packing_detail.item);

                int LocationID = 0;
                if (packing_detail.id_location == null)
                {
                    LocationID = FindNFix_Location(item_product, packing_detail.app_location, purchase_packing.app_branch);
                    packing_detail.app_location = db.app_location.Find(LocationID);
                }
                else
                {
                    packing_detail.app_location = db.app_location.Find(packing_detail.id_location);
                    LocationID = (int)packing_detail.id_location;
                }

                item_movementList.Add(
                        CreditOnly_Movement(
                            Status.Stock.InStock,
                            App.Names.PurchasePacking,
                            packing_detail.id_purchase_packing,
                            packing_detail.id_purchase_packing_detail,
                            CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                            packing_detail.item.item_product.Select(x => x.id_item_product).FirstOrDefault(),
                            LocationID,
                            (decimal)packing_detail.verified_quantity,
                            purchase_packing.trans_date,
                            packing_detail.purchase_order_detail.unit_cost,
                            comment_Generator(App.Names.PurchasePacking, purchase_packing.number ?? "", purchase_packing.contact.name), null,
                            packing_detail.expire_date, packing_detail.batch_code
                    ));
            }
            //Return List so we can save into context.
            return item_movementList;
        }

        public List<item_movement> PurchaseInvoice_Approve(db db, purchase_invoice purchase_invoice)
        {
            List<item_movement> item_movementList = new List<item_movement>();
            List<purchase_invoice_detail> Detail_Product_List = new List<purchase_invoice_detail>();

            if (purchase_invoice != null)
            {
                //If Purchase Invoice has Packing Relation, then skip the code. This is why if Count() = 0, then continue.
                if (db.purchase_packing_relation.Where(x => x.id_purchase_invoice == purchase_invoice.id_purchase_invoice).Count() == 0)
                {
                    //Only insert Details that are Products, RawMaterials or Supplies
                    foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice.purchase_invoice_detail
                                                                .Where(x => x.item != null && (
                                                                    x.item.id_item_type == item.item_type.Product ||
                                                                    x.item.id_item_type == item.item_type.RawMaterial ||
                                                                    x.item.id_item_type == item.item_type.Supplies)))
                    {
                        Detail_Product_List.Add(purchase_invoice_detail);
                    }
                }
            }

            //Always insert Location into Default Location.
            int LocationID = CurrentSession.Locations.Where(x => x.id_branch == purchase_invoice.id_branch && x.is_default).Any()
                ?
                CurrentSession.Locations.Where(x => x.id_branch == purchase_invoice.id_branch && x.is_default).Select(x => x.id_location).FirstOrDefault()
                :
                CurrentSession.Locations.Where(x => x.id_branch == purchase_invoice.id_branch).Select(x => x.id_location).FirstOrDefault(); //FindNFix_Location(item_product, purchase_invoice_detail.app_location, purchase_invoice.app_branch);

            foreach (purchase_invoice_detail purchase_invoice_detail in Detail_Product_List)
            {
                purchase_invoice_detail.id_location = LocationID;

                List<item_movement_dimension> item_movement_dimensionLIST = null;
                if (purchase_invoice_detail.purchase_invoice_dimension.Count > 0)
                {
                    item_movement_dimensionLIST = new List<item_movement_dimension>();
                    foreach (purchase_invoice_dimension purchase_invoice_dimension in purchase_invoice_detail.purchase_invoice_dimension)
                    {
                        item_movement_dimension item_movement_dimension = new item_movement_dimension()
                        {
                            id_dimension = purchase_invoice_dimension.id_dimension,
                            value = purchase_invoice_dimension.value
                        };
                        item_movement_dimensionLIST.Add(item_movement_dimension);
                    }
                }

                //Improve Comment. More standarized.
                if (purchase_invoice_detail.item.item_product.FirstOrDefault() != null)
                {
                    item_movementList.Add(
                   CreditOnly_Movement(
                        Status.Stock.InStock,
                        App.Names.PurchaseInvoice,
                        purchase_invoice_detail.id_purchase_invoice,
                        purchase_invoice_detail.id_purchase_invoice_detail,
                        purchase_invoice.id_currencyfx,
                        purchase_invoice_detail.item.item_product.FirstOrDefault().id_item_product,
                        (int)purchase_invoice_detail.id_location,
                        purchase_invoice_detail.quantity,
                        purchase_invoice.trans_date,
                        purchase_invoice_detail.unit_cost,
                        comment_Generator(App.Names.PurchaseInvoice, purchase_invoice.number ?? "", purchase_invoice.contact.name), item_movement_dimensionLIST,
                        purchase_invoice_detail.expire_date, purchase_invoice_detail.batch_code
                ));
                }
            }
            //Return List so we can save into context.
            return item_movementList;
        }

        public List<item_movement> PurchaseReturn_Approve(db db, purchase_return purchase_return)
        {
            List<item_movement> item_movementList = new List<item_movement>();
            List<purchase_return_detail> Invoice_WithProducts = new List<purchase_return_detail>();

            if (purchase_return != null)
            {
                if (purchase_return.purchase_return_detail.Count() > 0)
                {
                    if (purchase_return.purchase_return_detail.Where(x => x.item != null).Count() > 0)
                    {
                        //Due to lack of special field, use Is Read to know if we should discount item or not.
                        Invoice_WithProducts.AddRange(purchase_return.purchase_return_detail
                            .Where(x => x.item != null && x.is_read == true).ToList());
                    }
                }
            }
            else
            {
                return item_movementList;
            }

            //PURCHASE RETURN
            foreach (purchase_return_detail purchase_return_detail in Invoice_WithProducts)
            {
                item_product item_product = FindNFix_ItemProduct(purchase_return_detail.item);
                if (item_product != null)
                {
                    purchase_return_detail.id_location = FindNFix_Location(item_product, purchase_return_detail.app_location, purchase_return.app_branch);
                    purchase_return_detail.app_location = db.app_location.Find(purchase_return_detail.id_location);
                }

                if (purchase_return.id_purchase_invoice > 0)
                {
                    Brillo.Stock stock = new Brillo.Stock();
                    List<StockList> Items_InStockLIST = new List<StockList>();

                    foreach (item_movement _item_movement in purchase_return_detail.purchase_invoice_detail.item_movement)
                    {
                        StockList Stock = new StockList();
                        Stock.MovementID = Convert.ToInt32(_item_movement.id_movement);
                        Stock.TranDate = Convert.ToDateTime(_item_movement.trans_date);
                        Stock.QtyBalance = Convert.ToDecimal(_item_movement.credit);
                        Stock.Cost = Convert.ToDecimal(_item_movement.item_movement_value.Sum(x => x.unit_value));
                        Stock.LocationID = _item_movement.id_location;
                        Stock.ExpirationDate = _item_movement.expire_date;
                        Stock.code = _item_movement.code;

                        Items_InStockLIST.Add(Stock);
                    }

                    item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                App.Names.PurchaseReturn,
                                                purchase_return_detail.id_purchase_return,
                                                purchase_return_detail.id_purchase_return_detail,
                                                purchase_return.id_currencyfx,
                                                item_product,
                                                (int)purchase_return_detail.id_location,
                                                purchase_return_detail.quantity,
                                                purchase_return.trans_date,
                                                comment_Generator(App.Names.PurchaseReturn, purchase_return.number, purchase_return.contact.name)
                                                ));
                }
                else
                {
                    List<StockList> Items_InStockLIST = null;
                    //Get specific Movement ID
                    if (purchase_return_detail.movement_id != null && purchase_return_detail.movement_id > 0)
                    {
                        Brillo.Stock stockBrillo = new Brillo.Stock();
                        Items_InStockLIST = stockBrillo.ScalarMovement((long)purchase_return_detail.movement_id);
                    }
                    else
                    { // Get all Movements with Balance.
                        Brillo.Stock stock = new Brillo.Stock();
                        Items_InStockLIST = stock.List(purchase_return_detail.app_location.id_branch, (int)purchase_return_detail.id_location, item_product.id_item_product);
                    }

                    item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                App.Names.PurchaseReturn,
                                                purchase_return_detail.id_purchase_return,
                                                purchase_return_detail.id_purchase_return_detail,
                                                purchase_return.id_currencyfx,
                                                item_product,
                                                (int)purchase_return_detail.id_location,
                                                purchase_return_detail.quantity,
                                                purchase_return.trans_date,
                                                comment_Generator(App.Names.PurchaseReturn, purchase_return.number, purchase_return.contact.name)
                                                ));
                }
            }
            //Return List so we can save into context.
            return item_movementList;
        }

        public List<item_movement> SalesInvoice_Approve(db db, ref sales_invoice sales_invoice)
        {
            List<item_movement> item_movementList = new List<item_movement>();
            List<sales_invoice_detail> Invoice_WithProducts = new List<sales_invoice_detail>();

            if (sales_invoice != null)
            {
                if (sales_invoice.sales_invoice_detail.Count() > 0)
                {
                    if (sales_invoice.sales_invoice_detail.Where(x => x.item.item_product.Count() > 0 && x.sales_packing_relation.Count() == 0).Count() > 0)
                    {
                        Invoice_WithProducts.AddRange(sales_invoice.sales_invoice_detail.Where(x => x.item.item_product.Count() > 0 && x.sales_packing_relation.Count() == 0).ToList());
                    }
                    else
                    {
                        //Link with Packing if necesary
                        foreach (sales_invoice_detail detail in sales_invoice.sales_invoice_detail
                            .Where(x => x.item.item_product.Count() > 0 && x.sales_packing_relation.Count() > 0))
                        {
                            List<sales_packing_relation> PackingList = detail.sales_packing_relation.ToList();
                            foreach (sales_packing_relation sales_packing_relation in PackingList)
                            {
                                sales_packing_detail sales_packing_detail = sales_packing_relation.sales_packing_detail;
                                if (sales_packing_detail != null)
                                {
                                    if (sales_packing_detail.item_movement != null)
                                    {
                                        List<item_movement> MovementList = sales_packing_detail.item_movement.ToList();
                                        foreach (item_movement mov in MovementList)
                                        {
                                            mov.id_sales_invoice_detail = detail.id_sales_invoice_detail;
                                            detail.batch_code = mov.code;
                                            detail.expire_date = mov.expire_date;
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
                return item_movementList;
            }

            foreach (sales_invoice_detail detail in Invoice_WithProducts)
            {
                if (detail.item.is_autorecepie)
                {
                    if (detail.item.item_recepie.FirstOrDefault() != null)
                    {
                        foreach (item_recepie_detail item_recepie_detail in detail.item.item_recepie.FirstOrDefault().item_recepie_detail)
                        {
                            item_product item_productSub = FindNFix_ItemProduct(item_recepie_detail.item);
                            int LocationID = 0;
                            if (item_productSub != null)
                            {
                                if (detail.id_location == null)
                                {
                                    LocationID = FindNFix_Location(item_productSub, detail.app_location, sales_invoice.app_branch);
                                    //detail.app_location = db.app_location.Find(detail.id_location);
                                }
                                else
                                {
                                    detail.app_location = db.app_location.Find(detail.id_location);
                                    LocationID = (int)detail.id_location;
                                }

                                List<StockList> Items_InStockLIST = null;

                                if (detail.movement_id != null && detail.movement_id > 0)
                                {
                                    Brillo.Stock stockBrillo = new Brillo.Stock();
                                    Items_InStockLIST = stockBrillo.ScalarMovement((long)detail.movement_id);
                                }
                                else
                                {
                                    Brillo.Stock stock = new Brillo.Stock();
                                    Items_InStockLIST = stock.List(detail.sales_invoice.id_branch, LocationID, item_productSub.id_item_product);
                                }

                                item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                            App.Names.SalesInvoice,
                                                            detail.id_sales_invoice,
                                                            detail.id_sales_invoice_detail,
                                                            sales_invoice.id_currencyfx,
                                                            item_productSub,
                                                            LocationID,
                                                            item_recepie_detail.quantity,
                                                            sales_invoice.trans_date,
                                                            comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact.name)
                                                            ));
                            }
                        }
                    }
                }
                else
                //If NOT Auto Recpie
                {
                    int LocationID = 0;
                    item_product item_product = detail.item.item_product.FirstOrDefault();
                    if (item_product != null)
                    {
                        if (detail.id_location == null)
                        {
                            LocationID = FindNFix_Location(item_product, detail.app_location, sales_invoice.app_branch);
                            // detail.app_location = db.app_location.Find(detail.id_location);
                        }
                        else
                        {
                            detail.app_location = db.app_location.Find(detail.id_location);
                            LocationID = (int)detail.id_location;
                        }

                        List<StockList> Items_InStockLIST = null;
                        decimal quantity = 0;
                        if (detail.movement_id != null && detail.movement_id > 0)
                        {
                            Brillo.Stock stockBrillo = new Brillo.Stock();
                            Items_InStockLIST = stockBrillo.ScalarMovement((long)detail.movement_id);
                         
                            if (detail.quantity> Items_InStockLIST.FirstOrDefault().QtyBalance)
                            {
                                quantity = Items_InStockLIST.FirstOrDefault().QtyBalance;
                            }
                            else
                            {
                                quantity = detail.quantity;
                            }
                        }
                        else
                        {
                            quantity = detail.quantity;
                            Brillo.Stock stock = new Brillo.Stock();
                            Items_InStockLIST = stock.List(detail.sales_invoice.id_branch, detail.id_location, item_product.id_item_product);
                        }

                        if (quantity>0)
                        {
                            item_movementList.AddRange(DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                   App.Names.SalesInvoice,
                                                   detail.id_sales_invoice,
                                                   detail.id_sales_invoice_detail,
                                                   sales_invoice.id_currencyfx,
                                                   item_product,
                                                   LocationID,
                                                   quantity,
                                                   sales_invoice.trans_date,
                                                   comment_Generator(App.Names.SalesInvoice, sales_invoice.number, sales_invoice.contact != null ? sales_invoice.contact.name : "")
                                                   ));
                        }
                       
                    }
                }
            }

            //Return List so we can save into context.
            return item_movementList;
        }

        public List<item_movement> SalesReturn_Approve(db db, sales_return sales_return)
        {
            List<item_movement> item_movementList = new List<item_movement>();
            List<sales_return_detail> Invoice_WithProducts = new List<sales_return_detail>();

            if (sales_return != null)
            {
                if (sales_return.sales_return_detail.Count() > 0)
                {
                    if (sales_return.sales_return_detail.Where(x => x.id_item > 0).Count() > 0)
                    {
                        //Due to lack of special field, use Is Read to know if we should discount item or not.
                        Invoice_WithProducts.AddRange(sales_return.sales_return_detail.Where(x => x.item.item_product.Count() > 0 && x.is_read == true).ToList());
                    }
                }
            }
            else
            {
                return item_movementList;
            }

            //SALES RETURN
            foreach (sales_return_detail sales_return_detail in Invoice_WithProducts)
            {
                item_product item_product = sales_return_detail.item.item_product.FirstOrDefault();

                sales_return_detail.id_location = 
                    CurrentSession.Locations
                    .Where(x => x.id_branch == sales_return.id_branch && x.is_default)
                    .FirstOrDefault()
                    .id_location;

                decimal unit_cost = 0;
                if (sales_return_detail.sales_invoice_detail != null)
                {
                    unit_cost = sales_return_detail.sales_invoice_detail.unit_cost;
                }

                item_movementList.Add(
                    CreditOnly_Movement(Status.Stock.InStock,
                                            App.Names.SalesReturn,
                                            sales_return_detail.id_sales_return,
                                            sales_return_detail.id_sales_return_detail,
                                            sales_return.id_currencyfx,
                                            item_product.id_item_product,
                                            (int)sales_return_detail.id_location,
                                            sales_return_detail.quantity,
                                            sales_return.trans_date,
                                             unit_cost,
                                            comment_Generator(App.Names.SalesReturn, sales_return.number, sales_return.contact.name),
                                            null, sales_return_detail.expire_date, sales_return_detail.batch_code
                                            ));
            }
            //Return List so we can save into context.
            return item_movementList;
        }

        public List<item_movement> Inventory_Approve(db db, item_inventory item_inventory)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            foreach (item_inventory_detail item_inventory_detail in item_inventory.item_inventory_detail
                .Where(x => x.item_product != null && Convert.ToDecimal(x.value_counted) != x.value_system))
            {
                ///If Inventory has Dimension Count.
                if (item_inventory_detail.item_inventory_dimension.Count() > 0)
                {
                    List<item_movement_dimension> item_movement_dimensionLIST = null;
                    if (item_inventory_detail.item_inventory_dimension.Count > 0)
                    {
                        item_movement_dimensionLIST = new List<item_movement_dimension>();
                        foreach (item_inventory_dimension item_inventory_dimension in item_inventory_detail.item_inventory_dimension)
                        {
                            item_movement_dimension item_movement_dimension = new item_movement_dimension()
                            {
                                id_dimension = item_inventory_dimension.id_dimension,
                                value = item_inventory_dimension.value
                            };
                            item_movement_dimensionLIST.Add(item_movement_dimension);
                        }
                    }

                    if (item_inventory_detail.value_counted > 0)
                    {
                        item_movementList.Add(
                            CreditOnly_Movement(
                                Status.Stock.InStock,
                                App.Names.Inventory,
                                item_inventory_detail.id_inventory,
                                item_inventory_detail.id_inventory_detail,
                                CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                item_inventory_detail.id_item_product,
                                item_inventory_detail.id_location,
                                (decimal)item_inventory_detail.value_counted,
                                item_inventory_detail.item_inventory.trans_date,
                                item_inventory_detail.unit_value,
                                comment_Generator(App.Names.Inventory, Localize.Text<string>("Inventory"), item_inventory_detail.comment), item_movement_dimensionLIST
                                , null, null
                                ));
                    }
                }
                ///If Inventory has MovementID (used with Batch Code)
                else if (item_inventory_detail.movement_id > 0)
                {
                    item_movement item_movement = db.item_movement.Find(item_inventory_detail.movement_id);
                    if (item_movement != null)
                    {
                        //Positive Delta creates an Increase Stock
                        if (item_inventory_detail.Delta > 0)
                        {
                            //Increase Delta ontop of Credit.
                            item_movement.credit += item_inventory_detail.Delta;
                            item_movement.comment += " | " + Localize.StringText("Inventory") + ": " + "Increased by " + String.Format("{0:0.00}", item_inventory_detail.Delta) + " | " + item_inventory_detail.comment;
                            item_movement.timestamp = DateTime.Now;
                        }
                        //Negative Delta creates an Decrease Stock
                        else if (item_inventory_detail.Delta < 0)
                        {
                            item_movement im = new item_movement()
                            {
                                id_item_product = item_inventory_detail.id_item_product,
                                id_location = item_inventory_detail.id_location,
                                credit = 0,
                                debit = Math.Abs(item_inventory_detail.Delta),
                                status = Status.Stock.InStock,
                                code = item_movement.code,
                                expire_date = item_movement.expire_date,
                                id_inventory_detail = item_inventory_detail.id_inventory_detail,
                                trans_date = item_inventory_detail.item_inventory.trans_date,
                                timestamp = DateTime.Now,
                                comment = Localize.StringText("Inventory") + ": " + item_inventory_detail.comment,
                                parent = item_movement
                            };

                            item_movementList.Add(im);
                        }
                        else
                        {
                            //Ignore if Delta is 0. Means no change.
                        }
                    }
                }
                ///If Inventory is Generic
                else
                {
                    if (item_inventory_detail.Delta > 0)
                    {
                        item_movementList.Add(
                            CreditOnly_Movement(
                                Status.Stock.InStock,
                                App.Names.Inventory,
                                item_inventory_detail.id_inventory,
                                item_inventory_detail.id_inventory_detail,
                                CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                item_inventory_detail.id_item_product,
                                item_inventory_detail.id_location,
                                item_inventory_detail.Delta,
                                item_inventory_detail.item_inventory.trans_date,
                                item_inventory_detail.unit_value,
                                comment_Generator(App.Names.Inventory, Localize.Text<string>("Inventory"), item_inventory_detail.comment), null,
                                null, null
                                ));
                    }
                    else if (item_inventory_detail.Delta < 0)
                    {
                        List<StockList> Items_InStockLIST = null;
                        if (item_inventory_detail.movement_id != null && item_inventory_detail.movement_id > 0)
                        {
                            Brillo.Stock stockBrillo = new Brillo.Stock();
                            Items_InStockLIST = stockBrillo.ScalarMovement((long)item_inventory_detail.movement_id);
                        }
                        else
                        {
                            Brillo.Stock stock = new Brillo.Stock();
                            Items_InStockLIST = stock.List(item_inventory_detail.app_location.id_branch, item_inventory_detail.id_location, item_inventory_detail.id_item_product);
                        }

                        item_movementList.AddRange(
                            DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                App.Names.Inventory,
                                item_inventory_detail.id_inventory,
                                item_inventory_detail.id_inventory_detail,
                                CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                item_inventory_detail.item_product,
                                item_inventory_detail.id_location,
                                Math.Abs(item_inventory_detail.Delta),
                                item_inventory_detail.item_inventory.trans_date,
                                comment_Generator(App.Names.Inventory, Localize.Text<string>("Inventory"), item_inventory_detail.comment)
                                ));
                    }
                }
            }
            //Return List so we can save into context.
            return item_movementList;
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
                foreach (item_transfer_detail item_transfer_detail in (Transcation as item_transfer).item_transfer_detail)
                {
                    // item_movement.transaction_id = TransactionID;
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_transfer_detail == item_transfer_detail.id_transfer).ToList());
                }
            }
            else if (Application_ID == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = Transcation as purchase_invoice;
                foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice.purchase_invoice_detail)
                {
                    item_movement item_movement = db.item_movement.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                    if (item_movement != null)
                    {
                        List<item_movement> ChildMovementList = db.item_movement.Where(x => x.parent.id_movement == item_movement.id_movement).ToList();

                        //If a child row has not been created. just delete the entire range.
                        if (ChildMovementList == null || ChildMovementList.Count() == 0)
                        {
                            item_movementList.Add(item_movement);
                        }
                        else //if child row has been created, but is lesser than credit.
                        {
                            if (item_movement.credit > ChildMovementList.Sum(x => x.debit))
                            {
                                item_movement.credit = ChildMovementList.Sum(x => x.debit);
                            }
                        }
                    }
                }
            }
            else if (Application_ID == App.Names.PurchaseReturn)
            {
                purchase_return purchase_return = Transcation as purchase_return;
                foreach (purchase_return_detail purchase_return_detail in purchase_return.purchase_return_detail)
                {
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_purchase_return_detail == purchase_return_detail.id_purchase_return_detail).ToList());
                }
            }
            else if (Application_ID == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = Transcation as sales_invoice;
                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
                {
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).ToList());
                }
            }
            else if (Application_ID == App.Names.SalesReturn)
            {
                sales_return sales_return = Transcation as sales_return;
                foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail)
                {
                    item_movementList.AddRange(db.item_movement.Where(x => x.id_sales_return_detail == sales_return_detail.id_sales_return_detail).ToList());
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

            if (item_movementList.Count() > 0)
            {
                return item_movementList;
            }

            return null;
        }

        public List<item_movement> DebitOnly_MovementLIST(db db, List<StockList> Items_InStockLIST, Status.Stock Status, App.Names ApplicationID, int TransactionID, int TransactionDetailID,
                                                       int CurrencyFXID, item_product item_product, int LocationID,
                                                       decimal Quantity, DateTime TransDate, string Comment)
        {
            List<item_movement> Final_ItemMovementLIST = new List<item_movement>();

            int id_item_product = item_product.id_item_product;
            int id_movement = 0;
            decimal Unitcost = 0;

            if (item_product.cogs_type == item_product.COGS_Types.LIFO && Items_InStockLIST != null)
            {
                Items_InStockLIST = Items_InStockLIST.OrderByDescending(x => x.TranDate).ToList();
            }
            else if (Items_InStockLIST != null)
            {
                Items_InStockLIST = Items_InStockLIST.OrderBy(x => x.TranDate).ToList();
            }

            ///Will create new Item Movements
            ///if split from Parents is needed.
            foreach (StockList parent_Movement in Items_InStockLIST)
            {
                List<item_movement_dimension> parent_movement_dimension = db.item_movement_dimension.Where(x => x.id_movement == parent_Movement.MovementID).ToList();
                if (Quantity > 0)
                {
                    item_movement item_movement = new item_movement();

                    //THIS BLOCK OF CODE IS WRONG.
                    decimal movement_debit_quantity = Quantity;

                    //If Parent Movement is lesser than Quantity, then only take total value of Parent.
                    if (parent_Movement.QtyBalance <= Quantity)
                    {
                        movement_debit_quantity = parent_Movement.QtyBalance;
                    }

                    if (parent_Movement.LocationID > 0)
                    {
                        item_movement.id_location = parent_Movement.LocationID;
                    }

                    item_movement.comment = Comment;
                    item_movement.id_item_product = item_product.id_item_product;
                    item_movement.debit = movement_debit_quantity;
                    item_movement.credit = 0;
                    item_movement.status = Status;

                    //Batch Information is carried forward to chil
                    item_movement.expire_date = parent_Movement.ExpirationDate;
                    item_movement.code = parent_Movement.code;

                    item_movement.parent = db.item_movement.Find(parent_Movement.MovementID);
                    item_movement.barcode = item_movement.parent != null ? item_movement.parent.barcode : Barcode.RandomGenerator();

                    if (ApplicationID == App.Names.Transfer)
                    {
                        item_movement.id_transfer_detail = TransactionDetailID;
                    }
                    else if (ApplicationID == App.Names.ProductionExecution)
                    {
                        item_movement.id_execution_detail = TransactionDetailID;

                        if (db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).Select(y => y.movement_id).FirstOrDefault() != null)
                        {
                            int MovementID = (int)db.production_execution_detail.Where(x => x.id_execution_detail == TransactionDetailID).Select(y => y.movement_id).FirstOrDefault();
                            if (MovementID > 0)
                            {
                                id_movement = MovementID;
                                item_movement.parent = db.item_movement.Find(id_movement);
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
                    else if (ApplicationID == App.Names.Inventory)
                    {
                        item_movement.id_inventory_detail = TransactionDetailID;
                        decimal Unit_Value = db.item_inventory_detail.Where(x => x.id_inventory_detail == TransactionDetailID).Select(x => x.unit_value).FirstOrDefault();
                        if (Unit_Value > 0)
                        {
                            Unitcost = Unit_Value;
                        }

                        item_inventory_detail detail = db.item_inventory_detail.Find(TransactionDetailID);

                        if (detail != null)
                        {
                            if (detail.item_product != null)
                            {
                                if (detail.item_product.can_expire)
                                {
                                    item_movement.expire_date = detail.expire_date;
                                    item_movement.code = detail.batch_code;
                                }
                            }
                        }
                    }

                    item_movement.trans_date = TransDate;
                  
                    if (ApplicationID == App.Names.ProductionExecution)
                    {
                        if (db.production_execution_detail.Find(TransactionDetailID) != null)
                        {
                            production_execution_detail production_execution_detail = db.production_execution_detail.Find(TransactionDetailID);
                            if (production_execution_detail.production_order_detail.production_order.type == production_order.ProductionOrderTypes.Fraction)
                            {
                                if (parent_movement_dimension != null && item_movement.item_movement_dimension != null)
                                {
                                    decimal ParentDimesion = 1;
                                    decimal ChildDimesion = 1;

                                    foreach (item_movement_dimension item_movement_dimension in parent_movement_dimension)
                                    {
                                        ParentDimesion = ParentDimesion * item_movement_dimension.value;
                                    }
                                    foreach (item_movement_dimension item_movement_dimension in item_movement.item_movement_dimension)
                                    {
                                        ChildDimesion = ChildDimesion * item_movement_dimension.value;
                                    }

                                    if (ParentDimesion > 0 && ChildDimesion > 0)
                                    {
                                        Unitcost = parent_Movement.Cost;
                                        decimal ChildPaticipantion = (ParentDimesion / ChildDimesion);
                                        Unitcost = Unitcost * ChildPaticipantion;
                                    }
                                }
                            }
                        }
                    }

                    //Logic for Value
                    if (CurrencyFXID > 0)
                    {
                        item_movement_value item_movement_value = new item_movement_value();
                        if (Unitcost > 0)
                        {
                            item_movement_value.unit_value = Unitcost;
                        }
                        else
                        {
                            item_movement_value.unit_value = parent_Movement.Cost;
                        }

                        item_movement_value.id_currencyfx = CurrencyFXID;
                        item_movement_value.comment = Localize.StringText("DirectCost");
                        item_movement.item_movement_value.Add(item_movement_value);
                    }


                    foreach (item_movement_dimension item_movement_dimension in parent_movement_dimension)
                    {
                        item_movement_dimension _item_movement_dimension = new item_movement_dimension()
                        {
                            id_dimension = item_movement_dimension.id_dimension,
                            value = item_movement_dimension.value
                        };

                        item_movement.item_movement_dimension.Add(_item_movement_dimension);
                    }

                    //Adding into List
                    Final_ItemMovementLIST.Add(item_movement);
                    Quantity -= parent_Movement.QtyBalance;
                }
            }

            ///In case Parent does not exist, will enter this code.
            if (Quantity > 0)
            {
                id_movement = 0;
                item_movement item_movement = new item_movement()
                {
                    comment = Comment,
                    id_item_product = item_product.id_item_product,
                    debit = Quantity,
                    credit = 0,
                    status = Status,
                    //Check for Better Code.
                    id_location = LocationID,
                    parent = null
                };

                if (ApplicationID == App.Names.Transfer)
                {
                    item_movement.id_transfer_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.ProductionExecution)
                {
                    item_movement.id_execution_detail = TransactionDetailID;

                    production_execution_detail production_execution_detail = db.production_execution_detail.Find(TransactionDetailID);
                    if (production_execution_detail != null)
                    {
                        foreach (production_execution_dimension production_execution_dimension in production_execution_detail.production_execution_dimension)
                        {
                            item_movement_dimension _item_movement_dimension = new item_movement_dimension()
                            {
                                id_dimension = production_execution_dimension.id_dimension,
                                value = production_execution_dimension.value
                            };

                            item_movement.item_movement_dimension.Add(_item_movement_dimension);
                        }
                    }
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
                else if (ApplicationID == App.Names.Inventory)
                {
                    item_movement.id_inventory_detail = TransactionDetailID;
                }

                // item_movement.transaction_id = TransactionID;
                item_movement.trans_date = TransDate;

                if (CurrencyFXID > 0)
                {
                    //Logic for Value in case Parent does not Exist, we will take from

                    item_movement_value item_movement_value = new item_movement_value();

                    if (Unitcost > 0)
                    {
                        item_movement_value.unit_value = Unitcost;
                    }
                    else
                    {
                        if (item_product.item.unit_cost != null)
                        {
                            item_movement_value.unit_value = (decimal)item_product.item.unit_cost;
                        }
                    }

                    item_movement_value.id_currencyfx = CurrencyFXID;
                    item_movement_value.comment = Localize.StringText("DirectCost");
                    item_movement.item_movement_value.Add(item_movement_value);
                }

                Final_ItemMovementLIST.Add(item_movement);
            }
            return Final_ItemMovementLIST;
        }

        public item_movement CreditOnly_Movement(Status.Stock Status, App.Names ApplicationID, int TransactionID, int TransactionDetailID,
                                              int CurrencyFXID, int ProductID, int LocationID,
                                              decimal Quantity, DateTime TransDate, decimal Cost, string Comment, List<item_movement_dimension> DimensionList,
                                              DateTime? ExpiryDate, string Code)
        {
            int id_movement = 0;
            if (Quantity > 0)
            {
                item_movement item_movement = new item_movement()
                {
                    comment = Comment,
                    id_item_product = ProductID,
                    debit = 0,
                    credit = Quantity,
                    status = Status,
                    id_location = LocationID
                };

                //Product Expiry Date...
                if (ExpiryDate != null)
                {
                    item_movement.expire_date = ExpiryDate;
                }

                //Lote Number Code...
                if (Code != null)
                {
                    item_movement.code = Code;
                }

                if (ApplicationID == App.Names.Transfer)
                {
                    item_movement.id_transfer_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.ProductionExecution)
                {
                    item_movement.id_execution_detail = TransactionDetailID;

                    using (db db = new db())
                    {
                        production_execution_detail production_execution_detail = db.production_execution_detail.Find(TransactionDetailID);

                        if (production_execution_detail != null && production_execution_detail.movement_id != null)
                        {
                            id_movement = (int)production_execution_detail.movement_id;
                        }

                        if (DimensionList == null)
                        {
                            if (production_execution_detail != null)
                            {
                                foreach (production_execution_dimension production_execution_dimension in production_execution_detail.production_execution_dimension)
                                {
                                    item_movement_dimension _item_movement_dimension = new item_movement_dimension()
                                    {
                                        id_dimension = production_execution_dimension.id_dimension,
                                        value = production_execution_dimension.value
                                    };

                                    item_movement.item_movement_dimension.Add(_item_movement_dimension);
                                }
                            }
                        }
                    }
                }
                else if (ApplicationID == App.Names.PurchaseInvoice)
                {
                    item_movement.id_purchase_invoice_detail = TransactionDetailID;

                    purchase_invoice_detail detail = null;

                    using (db db = new db())
                    {
                        detail = db.purchase_invoice_detail.Find(TransactionDetailID);

                        if (detail != null)
                        {
                            if (detail.item != null)
                            {
                                if (detail.item.item_product.FirstOrDefault() != null)
                                {
                                    if (detail.item.item_product.FirstOrDefault().can_expire)
                                    {
                                        item_movement.expire_date = detail.expire_date;
                                        item_movement.code = detail.batch_code;
                                    }
                                }
                            }
                        }
                    }
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
                else if (ApplicationID == App.Names.PurchasePacking)
                {
                    item_movement.id_purchase_packing_detail = TransactionDetailID;
                }
                else if (ApplicationID == App.Names.Inventory)
                {
                    item_movement.id_inventory_detail = TransactionDetailID;
                    item_inventory_detail detail = null;

                    using (db db = new db())
                    {
                        detail = db.item_inventory_detail.Find(TransactionDetailID);

                        if (detail != null)
                        {
                            if (detail.item_product != null)
                            {
                                if (detail.item_product.can_expire)
                                {
                                    item_movement.expire_date = detail.expire_date;
                                    item_movement.code = detail.batch_code;
                                }
                            }
                        }
                    }
                }

                item_movement.trans_date = TransDate;

                //Logic for Value in case Parent does not Exist, we will take from
                item_movement_value item_movement_value = new item_movement_value();

                if (Cost != 0)
                {
                    ///Bring Cost from Transaction.
                    int ID_CurrencyFX_Default = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                    decimal DefaultCurrency_Cost = Currency.convert_Values(Cost, CurrencyFXID, ID_CurrencyFX_Default, null);

                    item_movement_value.unit_value = DefaultCurrency_Cost;
                    item_movement_value.id_currencyfx = ID_CurrencyFX_Default;
                    item_movement_value.comment = Localize.StringText("DirectCost");

                    //Adding Value into Movement
                    item_movement.item_movement_value.Add(item_movement_value);
                }
                else
                {
                    ///If cost is 0, then take hand written cost from Items Table.
                    ///In most cases, item_unit_cost will be zero, but in case it is not, we can use it as a reference.
                    ///Also we are assuming item.unit_cost is in default currency. But if this transaction is in a different currency
                    ///we can have mis-guided information.

                    if (ProductID > 0)
                    {
                        using (db db = new db())
                        {
                            int i = 0;
                            i = db.item_product.Where(x => x.id_item_product == ProductID).Select(x => x.id_item).FirstOrDefault();
                            if (i > 0)
                            {
                                if (db.items.Where(x => x.id_item == i).Select(x => x.unit_cost).FirstOrDefault() != null)
                                {
                                    item_movement_value.unit_value = (decimal)db.items.Where(x => x.id_item == i).Select(x => x.unit_cost).FirstOrDefault();
                                }
                                else
                                {
                                    item_movement_value.unit_value = 0;
                                }
                            }
                        }

                        item_movement_value.id_currencyfx = CurrencyFXID;
                        item_movement_value.comment = Localize.StringText("DirectCost");

                        //Adding Value into Movement
                        item_movement.item_movement_value.Add(item_movement_value);
                    }
                }

                //Generate a barcode for datetime tick to prevent duplication.
                item_movement.barcode = Barcode.RandomGenerator();

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
            if (item.item_product.Count == 0)
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

        #endregion Helpers
    }
}