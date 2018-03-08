namespace cntrl.Reports.Item
{
    public static class Item
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
                i.name as Name from items as i
                   where i.id_company = @CompanyID";
    }
}