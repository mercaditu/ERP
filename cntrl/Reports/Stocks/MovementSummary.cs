namespace cntrl.Reports.Stock
{
    public static class MovementSummary

    {
        public static string query = @"
    set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                               set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION'; 
select
                             it.trans_date as  Date,
                             Origin.name as OriginL,
                             Destination.name as DestinationL,
                              Origin.name as OriginB,
                             Destination.name as DestinationB,
                             it.number as Transfer, it.comment as Comment, u.name as UserName, r.name as RequestedName,
                             i.name as ItemName, i.code as ItemCode,
                             itd.quantity_destination as Quantity_D, itd.quantity_origin as Quantity_O,
                             (select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag,
                             p.name as Project
                             from item_transfer as it
                             inner join item_transfer_detail as itd on it.id_transfer = itd.id_transfer
                             inner join item_product as ip on itd.id_item_product = ip.id_item_product
                             inner join items as i on ip.id_item = i.id_item
                             inner join app_location as Origin on it.app_location_origin_id_location = Origin.id_location
                             inner join app_location as Destination on it.app_location_destination_id_location = Destination.id_location
                             left join app_branch as OriginB on it.app_branch_origin_id_branch = OriginB.id_branch
                             left join app_branch as DestinationB on it.app_branch_destination_id_branch = DestinationB.id_branch
                             left join projects as p on it.id_project = p.id_project
                             inner join security_user as u on it.id_user = u.id_user
                             left join security_user as r on it.user_requested_id_user = r.id_user
                             where  it.id_company=@CompanyID  and it.trans_date between '@StartDate' and '@EndDate' and it.transfer_type=0
                             order by it.trans_date";
    }
}