  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.Stock
{
    public static class InventoryAnalysis

    {
        public static string query = @" 	
								 select extract(Year from trans_date) as Year, extract(Month from trans_date) as Month, i.code as Code, i.name as Item, 
																 b.name as Branch, sum(credit - debit) as Stock, 
																 sum(if(item_movement.id_sales_invoice_detail > 0, item_movement.debit, 0)) as Sales
																 from item_movement
																 inner join item_product as ip on item_movement.id_item_product = ip.id_item_product
																 inner join items as i on ip.id_item = i.id_item
																 inner join app_location as l on item_movement.id_location = l.id_location
																 inner join app_branch as b on l.id_branch = b.id_branch
																 where item_movement.trans_date between @StartDate and @EndDate and item_movement.id_company = @ComapnyID
																 group by extract(Year_Month from trans_date), l.id_branch, item_movement.id_item_product";
    }
}



 


 


