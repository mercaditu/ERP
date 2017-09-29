namespace cntrl.Reports.Stock
{
    public static class StockAnalysis

    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
		select
		b.name as Brand,
		bc.name as Branch,
		c.name as Supplier,
		c.lead_time as LeadTime,
		i.code as Code,
		i.name as Items,
		ip.can_expire as CanExpire,
		ip.stock_max as MaxStock,
		ip.stock_min as SafetyStock,
		im.credit as Credit,
		im.debit as Debit,
		im.id_movement,
		im.trans_date as Date,
		if(im.id_sales_invoice_detail > 0, debit, 0) as Sales,
		RetailPrice.value as Price,
		RetailPrice.Currency as Currency
		from item_movement as im
		inner join app_location as loc on im.id_location = loc.id_location
		inner join app_branch as bc on bc.id_branch = loc.id_branch
		inner join item_product as ip on im.id_item_product = ip.id_item_product
		inner join items as i on ip.id_item = i.id_item
		left join (
		select price.value, price.id_item, curr.name as Currency from item_price as price
		inner join item_price_list as plist on price.id_price_list = plist.id_price_list
		inner join app_currency as curr on price.id_currency = curr.id_currency
		where curr.is_priority
		) as RetailPrice on i.id_item = RetailPrice.id_item
		left join item_brand as b on i.id_brand = b.id_brand
		left join contacts as c on b.id_contact = c.id_contact
		where im.trans_date < '@EndDate'";
    }
}