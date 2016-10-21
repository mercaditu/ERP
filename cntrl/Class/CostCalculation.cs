using System.Collections.Generic;
using System.Linq;
using entity;
using System.ComponentModel;

namespace cntrl.Class
{
    public class CostCalculation
    {
        public List<CostList> CalculateOrderCost(List<production_order_detail> Listproduction_order_detail)
        {
            db db = new db();
            List<CostList> costlists = new List<Class.CostList>();
            foreach (production_order_detail production_order_detail in Listproduction_order_detail)
            {
                entity.Brillo.Stock stock = new entity.Brillo.Stock();
                CostList CostList = new CostList();
           
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
                costlists.Add(CostList);
            }
            return costlists;

           
        }
        public List<OutputList> CalculateOutputOrder(List<production_order_detail> Listproduction_order_detail, List<production_order_detail> Listinputproduction_order_detail)
        {
            db db = new db();
            List<OutputList> OutputLists = new List<OutputList>();
            foreach (production_order_detail production_order_detail in Listproduction_order_detail)
            {
                OutputList OutputList = new OutputList();
                OutputList.id_order_detail = production_order_detail.id_order_detail;
                OutputList.Name = production_order_detail.item.name;
                OutputList.Code = production_order_detail.item.code;
                List<CostList> costlists = CalculateOrderCost(Listinputproduction_order_detail.Where(x => x.parent.id_order_detail == OutputList.id_order_detail).ToList());
                foreach (CostList CostList in costlists)
                {
                    OutputList.Costs.Add(CostList);
                }
              
                OutputLists.Add(OutputList);
            }
            return OutputLists;
        }
    }

    public class CostList
    {
    
        public string Name { get; set; }

        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }

        public decimal SubTotal { get; set; }
       public OutputList OutputList { get; set; }
    }

    public class OutputList:INotifyPropertyChanged
    {
        public OutputList()
        {
            Costs = new List<CostList>();
        }
        public int id_order_detail { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal GrandTotal
        {
            get
            {
                _GrandTotal = Costs.Sum(x => x.Cost);

                //calc_credit(_GrandTotal);
                return _GrandTotal;
            }
            set
            {
                _GrandTotal = value;
                RaisePropertyChanged("GrandTotal");
            }
        }
        decimal _GrandTotal;
        public ICollection<CostList> Costs { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
