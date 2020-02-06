namespace cntrl.Reports.Finance
{
    public static class ContactBalance
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
select
										 if(
										 fx.is_reverse,
										 sum((sch.debit* fx.sell_value) - (sch.credit* fx.sell_value)),
										 sum((sch.debit / fx.sell_value) - (sch.credit / fx.sell_value))
										 ) as Balance
										 from payment_schedual as sch
										 inner join app_currencyfx as fx on sch.id_currencyfx = fx.id_currencyfx
										 where id_sales_invoice > 0 and sch.can_calculate = 1 and id_contact = @ContactID";
    }
}