﻿namespace cntrl.Reports.Stock
{
    public static class InventoryValue
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
						select branch.name as Branch,
								item.code as Code,
								item.name as Item,
								inv.credit as Credit,
								inv.DebitChild,
								inv.credit - inv.DebitChild as Balance,
								UnitCost,
								(UnitCost * (inv.credit - if(inv.DebitChild is null, 0, inv.DebitChild))) as TotalCost,
								inv.trans_date as TransDate, (select max(value)
										from item_price as price
											inner join app_currency as curr on price.id_currency = price.id_currency
											inner join item_price_list as price_list on price.id_price_list = price_list.id_price_list
											where (curr.is_priority or price_list.is_default) and price.id_item = item.id_item
											) as RetailPrice,
						(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = item.id_item order by item_tag_detail.is_default limit 0,1) as Tag

								from (
								select item_movement.*,  imvd.unit_value as UnitCost,
								(select if(sum(debit) is null, 0, sum(debit))
									from item_movement as mov
									where mov.parent_id_movement = item_movement.id_movement

									) as DebitChild

								from item_movement
                                 left join item_movement_value_rel as imvr on item_movement.id_movement_value_rel=imvr.id_movement_value_rel
             left join item_movement_value_detail as imvd on imvr.id_movement_value_rel=imvd.id_movement_value_rel    
								
								where item_movement.id_company = @CompanyID and   item_movement.trans_date between '@StartDate' and '@EndDate'
								group by item_movement.id_movement
								) as inv

								inner join item_product as prod on inv.id_item_product = prod.id_item_product
								inner join items as item on prod.id_item = item.id_item
								inner join app_location as loc on inv.id_location = loc.id_location
								inner join app_branch as branch on loc.id_branch = branch.id_branch
								where inv.credit > 0
								group by inv.id_movement";
    }
}