namespace cntrl.Reports.Stock
{
	public static class PriceList
	{
		public static string query = @" 
			select item.code as Code,
			item.name as Items,
			pl.name as PriceLists,
			pi.value as Price,
			currency.name as Currency,
			(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = item.id_item order by item_tag_detail.is_default limit 0,1) as Tag,
			(select sum(val.unit_value) from item_movement as mov 
				inner join item_movement_value as val on mov.id_movement = val.id_movement 
				inner join item_product as prod on mov.id_item_product = prod.id_item_product
				where (mov.id_purchase_invoice_detail > 0 or mov.id_inventory_detail > 0 or mov.id_execution_detail > 0) and prod.id_item = item.id_item
				group by mov.id_movement
				order by mov.trans_date desc
				limit 0,1
			) as Cost,
			
			(select si.trans_date from sales_invoice as si 
				inner join sales_invoice_detail as sid on si.id_sales_invoice = sid.id_sales_invoice
				where sid.id_item = item.id_item
				order by si.trans_date desc
				limit 0,1) as LastSold,
				
			(select sum(credit - debit) from item_movement as mov 
				inner join item_product as prod on mov.id_item_product = prod.id_item_product
				where prod.id_item = item.id_item
			) as InStock
			
			from items as item 

			left join item_price as pi on item.id_item = pi.id_item
			left join item_price_list as pl on pi.id_price_list = pl.id_price_list
			left join app_currency as currency on pi.id_currency = currency.id_currency
			left join item_product as p on item.id_item = p.id_item            
			where pi.id_company = @CompanyID
			order by item.name";
	}
}



 


 



