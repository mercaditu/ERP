namespace cntrl.Reports.Sales
{
    public static class SalesAnalysis

    {
        public static string query = @"

  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	(select max(value)
	from item_price as price
	inner join app_currency as curr on price.id_currency = price.id_currency
	inner join item_price_list as price_list on price.id_price_list = price_list.id_price_list
	where (curr.is_priority or price_list.is_default) and price.id_item = i.id_item
	) as RetailPrice,
		(SELECT sum(val.unit_value) FROM item_movement_value as val WHERE val.id_movement = item_movement.id_movement) AS Cost,
	(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag,

	extract(Year from trans_date) as Year,
	extract(Month from trans_date) as Month,
	i.code as Code, i.name as Item,
	b.name as Branch,
	PurchaseIn, TransferIn, InventoryIn, ProductionIn,
	SalesOut, TransferOut, InventoryOut, ProductionOut,
	PurchaseIn + TransferIn + InventoryIn + ProductionIn as TotalIn,
	SalesOut + TransferOut + InventoryOut + ProductionOut as TotalOut

		from
		(
		select
			im.id_item_product, im.trans_date, im.credit, im.debit, im.id_movement, im.id_location,
			sum(if(im.id_purchase_invoice_detail  > 0, credit, 0)) as PurchaseIn,
			sum(if(im.id_transfer_detail  > 0, credit, 0)) as TransferIn,
			sum(if(im.id_inventory_detail  > 0, credit, 0)) as InventoryIn,
			sum(if(im.id_execution_detail  > 0, credit, 0)) as ProductionIn,

			sum(if(im.id_sales_invoice_detail  > 0, debit, 0)) as SalesOut,
			sum(if(im.id_transfer_detail  > 0, debit, 0)) as TransferOut,
			sum(if(im.id_inventory_detail > 0, debit, 0)) as InventoryOut,
			sum(if(im.id_execution_detail  > 0, debit, 0)) as ProductionOut
		from item_movement as im
		where im.trans_date between '@StartDate' and '@EndDate' and im.id_company = @CompanyID
		group by extract(Year_Month from im.trans_date), im.id_location, im.id_item_product
		)

		as item_movement
		inner join item_product as ip on item_movement.id_item_product = ip.id_item_product
	inner join items as i on ip.id_item = i.id_item
	inner join app_location as l on item_movement.id_location = l.id_location
	inner join app_branch as b on l.id_branch = b.id_branch
";
    }
}