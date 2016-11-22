using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.Stock
{
    public static class StockByBranch

    {
        public static string query = @" 
select loc.id_location as LocationID, loc.name as Location, item.code as ItemCode, 
                             item.name as ItemName, prod.id_item_product as ProductID, 
                             (sum(mov.credit) - sum(mov.debit)) as Quantity, 
                             measure.name as Measurement,
                             (SELECT sum(val.unit_value) FROM item_movement_value as val WHERE val.id_movement = MAX(mov.id_movement)) AS Cost
                             brand.name as Brand        
                             from item_movement as mov
                             inner join app_location as loc on mov.id_location = loc.id_location
                             inner join app_branch as branch on loc.id_branch = branch.id_branch
                             inner join item_product as prod on mov.id_item_product = prod.id_item_product 
                             inner join items as item on prod.id_item = item.id_item
                             left join item_brand as brand on brand.id_brand = item.id_brand
                             left join app_measurement as measure on item.id_measurement = measure.id_measurement 
                             where mov.id_company = {0} and branch.id_branch = {1} and mov.trans_date <= '{2}'
                             group by loc.id_location, prod.id_item_product
                             order by item.name";
    }
}



 


 




 