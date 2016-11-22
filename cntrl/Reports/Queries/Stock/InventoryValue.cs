namespace cntrl.Reports.Queries.Stock
{
    public static class InventoryValue

    {
        public static string query = @" 	

select branch.name as BranchName,
                                item.code as ItemCode, 
                                item.name as ItemName,
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
                                select item_movement.*, sum(val.unit_value) as UnitCost,
                                (select if(sum(debit) is null, 0, sum(debit)) 
                                    from item_movement as mov 
                                    where mov.parent_id_movement = item_movement.id_movement
                                    and mov.trans_date <= '{0}'
                                    ) as DebitChild

                                from item_movement 
                                left outer join item_movement_value as val on item_movement.id_movement = val.id_movement
                                where item_movement.id_company = {1} and item_movement.trans_date <= '{0}'
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



 


 


