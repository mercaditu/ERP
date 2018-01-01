namespace cntrl.Reports.Stock
{
    public static class InventoryValue
    {
        public static string query = @"
                                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select *                                
                                from (
                                select  
                                l.id_location as LocationID
                                ,im.trans_date as Date
                                ,item.code as Code
								,item.name as Item
                                , l.name as Location
                                , l.id_branch as BranchID
                                ,branch.name as Branch
                                , max(im.id_movement) as MovementID
                                , ip.id_item as ItemID
                                , ip.id_item_product as ProductID
                                , ip.can_expire
                                , (im.credit - sum(IFNULL(child.debit,0))) as Balance
                                , (select sum(imvd.unit_value) from item_movement_value_detail as imvd where imvd.id_movement_value_rel=im.id_movement_value_rel) as UnitCost
                                , im.code as BatchCode
                                , im.expire_date as ExpiryDate
                                from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
	                            inner join items as item on ip.id_item = item.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as branch on l.id_branch = branch.id_branch
                                left join item_movement_value_rel as imvr on im.id_movement_value_rel = imvr.id_movement_value_rel
                                where im.id_company = @CompanyID and im.trans_date <= '@EndDate'
                                group by im.id_movement
                                order by im.expire_date) as movement 
                                where Balance > 0";


  
						
    }
}