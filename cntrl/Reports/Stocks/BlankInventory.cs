
namespace cntrl.Reports.Stock
{
    public class BlankInventory
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
        select
 
           
                i.code as Code,
                i.sku as Sku,
                i.name as Name,
                i.variation as Variation,
                i.description as Description
                from items as i
                inner join item_product as ip on i.id_item = ip.id_item
                where i.id_company = @CompanyID";
    }
}
