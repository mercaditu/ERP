using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo.Logic
{
    public static class ProductionExexustionStock
    {
        db db = new db();
        entity.Brillo.Logic.Stock Stock = new Stock();
        public List<item_movement> CalculateStock(production_execution production_execution, List<item_movement> item_movementList)
        {
            foreach (production_execution_detail detail in production_execution.production_execution_detail
                    .Where(x => x.item.id_item_type == item.item_type.Product
                             || x.item.id_item_type == item.item_type.RawMaterial))
            {
                item_product item_product = Stock.FindNFix_ItemProduct(detail.item);

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
                    item_movementList.Add(Stock.debit_Movement(entity.Status.Stock.InStock,
                                            App.Names.ProductionExecustion,
                                            detail.id_production_execution,
                                            item_product.id_item_product,
                                            (int)production_execution.production_line.id_location,
                                            detail.quantity,
                                            production_execution.trans_date,
                                            Stock.comment_Generator(App.Names.ProductionExecustion, production_execution.id_production_execution.ToString(), "")
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
                            item_movement = Stock.debit_Movement(entity.Status.Stock.InStock,
                                                    App.Names.ProductionExecustion,
                                                    (int)detail.id_production_execution,
                                                    item_product.id_item_product,
                                                    (int)production_execution.production_line.id_location,
                                                    movement_debit_quantity,
                                                    production_execution.trans_date,
                                                    Stock.comment_Generator(App.Names.ProductionExecustion,
                                                    production_execution.id_production_execution.ToString(), ""));
                        }
                        else
                        {
                            //If input is false, then we should CREDIT Stock.
                            item_movement = Stock.credit_Movement(entity.Status.Stock.InStock,
                                                    App.Names.ProductionExecustion,
                                                    (int)detail.id_production_execution,
                                                    item_product.id_item_product,
                                                    (int)production_execution.production_line.id_location,
                                                    qty_ExexustionDetail,
                                                    production_execution.trans_date,
                                                    Stock.comment_Generator(App.Names.ProductionExecustion,
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
            }
            return item_movementList;
        }
    }
}
