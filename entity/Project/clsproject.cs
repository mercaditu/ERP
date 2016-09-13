using System;
using System.Collections.Generic;
using System.Linq;

namespace entity.Project
{
    public class clsproject
    {
        dbContext _entity = new dbContext();
        public decimal getsumquantity(int project_task,ICollection<project_task> child)
        {
             return Convert.ToDecimal(child.Sum(x => x.quantity_est));
        }
        public decimal getsumquantityProduction(int production_order, ICollection<production_order_detail> child)
        {
            return Convert.ToDecimal(child.Sum(x => x.quantity));
        }
        public decimal getsumunitcost(int project_task, ICollection<project_task> child)
        {
            return Convert.ToDecimal(child.Sum(x => x.unit_cost_est));
        }
    }
}
