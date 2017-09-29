namespace cntrl.Reports.Stock
{
    public static class ItemReceipe
	{
        public static string query = @"
                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
            select 
            Product.name as Product, Product.code as Code, 
            RawMaterial.name as RawMaterial, RawMaterial.code as RawMaterialCode,
            ird.quantity as Quantity,

			            (select  imvd.unit_value
				            from item_movement as mov
				            left join item_movement_value_rel as imvr on mov.id_movement_value_rel=imvr.id_movement_value_rel
             left join item_movement_value_detail as imvd on imvr.id_movement_value_rel=imvd.id_movement_value_rel
				            inner join item_product as prod on mov.id_item_product = prod.id_item_product
				            where (mov.id_purchase_invoice_detail > 0 or mov.id_inventory_detail > 0 or mov.id_execution_detail > 0) and prod.id_item = RawMaterial.id_item
				            group by mov.id_movement
				            order by mov.trans_date desc
				            limit 0,1
			            ) as Cost

            from item_recepie_detail as ird
            left join item_recepie as ir on ir.id_recepie = ird.item_recepie_id_recepie
            left join items as Product on ir.item_id_item = Product.id_item
            left join items as RawMaterial on ird.item_id_item = RawMaterial.id_item
            where ir.id_company=@CompanyID
            order by Product.name";
    }
}