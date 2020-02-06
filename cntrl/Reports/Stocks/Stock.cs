namespace cntrl.Reports.Stock
{
    internal class Stock
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
        select
            l.name as Location, branch.name as Branch,
            (select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag,
            i.name as Item, i.code as Code,

            im.id_movement as MovementID,
            im.comment as Comment, im.debit as Debit, im.credit as Credit,
           (select max(imvd.comment) from item_movement_value_detail as imvd where im.id_movement_value_rel=imvd.id_movement_value_rel) as CostDetail,
            imvr.total_value as Cost,

            -- dim.name as Dimension, imd.value as Value,

            im.trans_date as Date, im.code as LotNumber, im.expire_date as ExpiryDate, im.timestamp as Timestamp, im.parent_id_movement as ParentID,
            u.name as User

            from item_movement as im

            inner join app_location as l on im.id_location = l.id_location
            inner join app_branch as branch on l.id_branch = branch.id_branch
         left join item_movement_value_rel as imvr on im.id_movement_value_rel=imvr.id_movement_value_rel
         --    left join item_movement_value_detail as imvd on imvr.id_movement_value_rel=imvd.id_movement_value_rel
           -- left join item_movement_dimension as imd on im.id_movement = imd.id_movement
           -- left join app_dimension as dim on imd.id_dimension = dim.id_dimension
            inner join item_product as ip on im.id_item_product = ip.id_item_product
            inner join items as i on ip.id_item = i.id_item
            inner join security_user as u on im.id_user = u.id_user
            where im.id_company = @CompanyID and im.trans_date between '@StartDate' and '@EndDate'
            order by im.trans_date";
    }
}