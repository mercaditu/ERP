using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.Stock
{
    public static class SalesAnalysis

    {
        public static string query = @"  select
 (select sum(credit) from item_movement as imv
	where (imv.id_purchase_invoice_detail > 0 or imv.id_execution_detail > 0 or imv.id_inventory_detail > 0) and
    imv.id_item_product = item_movement.id_item_product) as TotalCredits,
(select max(value)
from item_price as price
    inner join app_currency as curr on price.id_currency = price.id_currency
    inner join item_price_list as price_list on price.id_price_list = price_list.id_price_list
    where (curr.is_priority or price_list.is_default) and price.id_item = i.id_item
    ) as RetailPrice,
 extract(Year from trans_date) as Year, extract(Month from trans_date) as Month, i.code as Code, i.name as Item, 
																 b.name as Branch, sum(credit - debit) as Stock, 
																 sum(if(item_movement.id_sales_invoice_detail > 0, item_movement.debit, 0)) as Sales,
                                                                  (SELECT sum(val.unit_value) FROM item_movement_value as val WHERE val.id_movement = item_movement.id_movement) AS Cost,
	(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag
																 from item_movement
																 inner join item_product as ip on item_movement.id_item_product = ip.id_item_product
																 inner join items as i on ip.id_item = i.id_item
																 inner join app_location as l on item_movement.id_location = l.id_location
																 inner join app_branch as b on l.id_branch = b.id_branch
																-- where item_movement.trans_date between @StartDate and @EndDate and item_movement.id_company = @ComapnyID
																 group by extract(Year_Month from trans_date), l.id_branch, item_movement.id_item_product";
    }
}
