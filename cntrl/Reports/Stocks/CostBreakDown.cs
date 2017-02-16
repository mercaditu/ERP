namespace cntrl.Reports.Stock
{
    public static class CostBreakDown
    {
        public static string query = @"
            Select i.code as Code,
            i.name as Name,
            im.trans_date as TransDate,
            im.comment as Comment,
            imv.id_movement as MovID,
            imv.unit_value as UnitValue,
            imv.comment as Concept,
            im.credit as Quantity,
            (select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag,
            (select sum(unit_value) from item_movement_value where item_movement_value.id_movement = imv.id_movement) as SubTotal
                        from item_movement_value as imv
                        inner join item_movement as im on imv.id_movement = im.id_movement
                        inner join item_product as p on im.id_item_product = p.id_item_product
                        inner join items as i on p.id_item = i.id_item
                        where (im.id_purchase_invoice_detail is not null or im.id_execution_detail is not null)
                        and im.id_comapny=@CompanyID and im.trans_date between '@StartDate' and '@EndDate'
                        order by im.trans_date";
    }
}