namespace cntrl.Reports.Sales
{
    public static class SalesAnalysis

    {
        public static string query = @"  select  
			(select max(value)
			from item_price as price
			inner join app_currency as curr on price.id_currency = price.id_currency
			inner join item_price_list as price_list on price.id_price_list = price_list.id_price_list
			where (curr.is_priority or price_list.is_default) and price.id_item = sid.id_item
			) as RetailPrice,extract(Year from trans_date) as Year, extract(Month from trans_date) as Month, i.code as Code, i.name as Item, 
	(SELECT sum(val.unit_value) FROM item_movement_value as val WHERE val.id_movement in (select id_movement from item_movement where item_movement.trans_date<= Concat(extract(Year from trans_date),'-',extract(Month from trans_date),'-1')and item_movement.id_item_product=ip.id_item_product)) AS Cost,
				b.name as Branch,sid.quantity as Sales,(select sum(credit-debit) from item_movement where item_movement.trans_date<= Concat(extract(Year from trans_date),'-',extract(Month from trans_date),'-1') and item_movement.id_item_product=ip.id_item_product) as Stock,Round(avg(sid.unit_price),2) as RetailPrice,
                (select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag
            from sales_invoice_detail sid 	
inner join sales_invoice as si on sid.id_sales_invoice=si.id_sales_invoice
				inner join items as i on sid.id_item = i.id_item
                	inner join item_product as ip on i.id_item = ip.id_item
				inner join app_branch as b on si.id_branch = b.id_branch
				where si.trans_date between '@StartDate' and '@EndDate' and sid.id_company = @CompanyID
				group by extract(Month from trans_date), si.id_branch, sid.id_item";
    }
}
