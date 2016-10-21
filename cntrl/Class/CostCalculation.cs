using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
namespace cntrl.Class
{
    public class CostCalculation
    {
        public List<CostList> CalculateOrderCost(List<production_order_detail> Listproduction_order_detail)
        {
            db db = new db();
            List<CostList> costLists = new List<Class.CostList>();
            foreach (production_order_detail production_order_detail in Listproduction_order_detail)
            {
                entity.Brillo.Stock stock = new entity.Brillo.Stock();
                CostList CostList = new Class.CostList();
                CostList.parent_id_order_detail = production_order_detail.parent.id_order_detail;
                CostList.Name = production_order_detail.item.name;
                CostList.Name = production_order_detail.item.name;
                CostList.Quantity = production_order_detail.quantity;
                if (production_order_detail.item.item_product.FirstOrDefault() != null)
                {
                    int id_item_product = production_order_detail.item.item_product.FirstOrDefault().id_item_product;
                    item_movement item_movement = db.item_movement
                      .Where(x => x.id_item_product == id_item_product && x.credit > 0)
                      .OrderByDescending(y => y.trans_date)
                      .FirstOrDefault();
                    if (item_movement != null)
                    {
                        CostList.Cost = item_movement.item_movement_value.Sum(x => x.unit_value);
                    }
                    else
                    {
                        CostList.Cost = production_order_detail.item.unit_cost != null ? (decimal)production_order_detail.item.unit_cost : 0;
                    }
                }
                else
                {
                    CostList.Cost = (decimal)production_order_detail.item.unit_cost;
                }
                CostList.SubTotal = CostList.Quantity * CostList.Cost;
                costLists.Add(CostList);
            }


            return costLists;
        }
        public List<OutputList> CalculateOutputOrder(List<production_order_detail> Listproduction_order_detail)
        {

            db db = new db();
            List<OutputList> OutputLists = new List<Class.OutputList>();
            foreach (production_order_detail production_order_detail in Listproduction_order_detail)
            {

                OutputList OutputList = new Class.OutputList();
                OutputList.id_order_detail = production_order_detail.id_order_detail;
                OutputList.Name = production_order_detail.item.name;
                OutputList.Code = production_order_detail.item.code;
                OutputLists.Add(OutputList);
            }


            return OutputLists;
        }
    }

    public class CostList
    {
        public int parent_id_order_detail { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class OutputList
    {

        public int id_order_detail { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }
}
