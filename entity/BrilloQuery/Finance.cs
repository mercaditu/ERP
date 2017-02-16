using System;
using System.Data;

namespace entity.BrilloQuery
{
    public class Finance
    {
        public decimal? SpecialFXBalance_ByCustomer(decimal SpecialFXRate, int CustomerID)
        {
            string query = @"
						 select
                         (
                        if(fx.is_reverse,
                         ((cont.credit_limit * fx.sell_value) / {0}),
                         ((cont.credit_limit / fx.sell_value) * {0})) -
                        if(fx.is_reverse,
                         sum(((sch.debit * fx.sell_value) / {0}) - ((sch.credit * fx.sell_value)) / {0}),
                         sum(((sch.debit / fx.sell_value) * {0}) - ((sch.credit / fx.sell_value)) * {0}))
                         )
                        as Balance from payment_schedual as sch
                         inner join contacts as cont on sch.id_contact = cont.id_contact
                         inner join app_currencyfx as fx on sch.id_currencyfx = fx.id_currencyfx
                         where id_sales_invoice > 0 and sch.can_calculate = 1 and sch.id_contact = {1}";

            query = string.Format(query, SpecialFXRate, CustomerID);

            DataTable dt = QueryExecutor.DT(query);

            if (dt.Rows.Count > 0)
            {
                return Convert.ToDecimal(dt.Rows[0][0]);
            }
            else
            {
                return 0;
            }
        }
    }
}