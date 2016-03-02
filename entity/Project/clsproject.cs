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
        public decimal getsumunitcost(int project_task, ICollection<project_task> child)
        {
            return Convert.ToDecimal(child.Sum(x => x.unit_cost_est));
        }

        public decimal getsumAccounting_chart(accounting_chart child)
        {
            return   Convert.ToDecimal(child.accounting_journal_detail.Sum(x => x.credit) - child.accounting_journal_detail.Sum(x => x.debit));
        }
    }
}
