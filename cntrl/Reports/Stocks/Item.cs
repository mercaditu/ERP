
namespace cntrl.Reports.Stock
{
    public class Item
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
        select
 (select name 
 from item_tag_detail 
 inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag 
 where item_tag_detail.id_item = i.id_item 
 order by item_tag_detail.is_default limit 0,1) as Tag,
            CASE
               WHEN i.id_item_type = 1 THEN '" + entity.Brillo.Localize.StringText("Product") + @"'
               WHEN i.id_item_type = 2 THEN  '" + entity.Brillo.Localize.StringText("RawMaterial") + @"'
               WHEN i.id_item_type = 3 THEN  '" + entity.Brillo.Localize.StringText("Service") + @"'
               WHEN i.id_item_type = 4 THEN  '" + entity.Brillo.Localize.StringText("FixedAssets") + @"'
               WHEN i.id_item_type = 5 THEN  '" + entity.Brillo.Localize.StringText("Task") + @"'
               WHEN i.id_item_type = 6 THEN  '" + entity.Brillo.Localize.StringText("Supplies") + @"'
               WHEN i.id_item_type = 7 THEN  '" + entity.Brillo.Localize.StringText("ServiceContract") + @"'
                END as Type,
                i.code as Code,
                i.sku as Sku,
                i.name as Name,
                i.variation as Variation,
                i.description as Description,
                ip.stock_max as StockMax,
                ip.stock_min as StockMin,
                ip.can_expire as CanExpire,
                branch.name as Branch,
                sum(im.credit - im.debit) as InStock,
                (select sum(debit)
                from item_movement
                inner join app_location as l on item_movement.id_location = l.id_location
                inner join app_branch as b on l.id_branch = b.id_branch
                where item_movement.trans_date >= now()-interval 3 month
                and id_item_product = ip.id_item_product
                and b.id_branch = branch.id_branch
                and
                (
                item_movement.id_sales_invoice_detail is not null
                or
                item_movement.id_execution_detail is not null
                )) as Velocity
            from items as i
            left join item_product as ip on i.id_item = ip.id_item
            left join item_movement as im on ip.id_item_product = im.id_item_product
            left join app_location as location on im.id_location = location.id_location
            left join app_branch as branch on location.id_branch = branch.id_branch
            where i.id_company = @CompanyID
            group by i.id_item, branch.id_branch";
    }
}
