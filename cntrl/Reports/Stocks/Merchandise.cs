namespace cntrl.Reports.Stock
{
    public static class Merchandise

    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
select branch.name as BranchName,
				inv.comment as TransComment,
				item.code as ItemCode,
				item.name as ItemName,
				inv.credit as Credit,
				inv.debit as Debit,
				UnitCost,
				(UnitCost* inv.credit) as TotalCost,
				inv.trans_date as TransDate,
	(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = item.id_item order by item_tag_detail.is_default limit 0,1) as Tag

			  from(
			  select item_movement.*, imvd.unit_value
			  from item_movement
			 left join item_movement_value_rel as imvr on im.id_movement_value_rel=imvr.id_movement_value_rel
             left join item_movement_value_detail as imvd on imvr.id_movement_value_rel=imvd.id_movement_value_rel
			  where item_movement.id_company = @CompanyID and item_movement.trans_date between '@StartDate' and '@EndDate'
			  and (
					item_movement.id_purchase_invoice_detail > 0 or
					item_movement.id_execution_detail > 0 or
					item_movement.id_inventory_detail > 0 or
					item_movement.id_transfer_detail > 0)

			  group by item_movement.id_movement
				) as inv

			  inner join item_product as prod on inv.id_item_product = prod.id_item_product
			  inner join items as item on prod.id_item = item.id_item
			  inner join app_location as loc on inv.id_location = loc.id_location
			  inner join app_branch as branch on loc.id_branch = branch.id_branch

			  group by inv.id_movement
			  order by inv.trans_date";
    }
}