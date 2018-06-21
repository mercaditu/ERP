
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
                i.description as Description,
                branch.name as Branch,
                sum(im.credit - im.debit) as InStock
                from items as i
                inner join item_product as ip on i.id_item = ip.id_item
                left join item_movement as im on ip.id_item_product = im.id_item_product
                left join app_location as location on im.id_location = location.id_location
                left join app_branch as branch on location.id_branch = branch.id_branch
                where i.id_company = @CompanyID";
    }
}
