namespace cntrl.Reports.Stock
{
    public static class ItemReceipe
	{
        public static string query = @"

  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
            select * from ((select item_recepie.id_company,id_recepie,items.name,1 as Quantiy from item_recepie 
inner join items on items.id_item=item_recepie.id_item)
union
(select item_recepie_detail.id_company,item_recepie_detail.item_recepie_id_recepie,items.name,quantity as Quantiy from item_recepie_detail 
inner join items on items.id_item=item_recepie_detail.id_item)) as recepie
 where recepie.id_company=@CompanyID
order by id_recepie";
    }
}