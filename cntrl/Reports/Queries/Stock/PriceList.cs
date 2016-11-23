  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.Stock
{
    public static class PriceList

    {
        public static string query = @" 
select item.code as ItemCode,
item.name as ItemName,
pl.name as PriceLists,
pi.value as Price,
currency.name as Currency,
	(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = item.id_item order by item_tag_detail.is_default limit 0,1) as Tag
from items as item 

left join item_price as pi on item.id_item = pi.id_item

left join item_price_list as pl on pi.id_price_list = pl.id_price_list

inner join app_currency as currency on pi.id_currency = currency.id_currency
where item.id_company = @CompanyID 
order by item.name";
    }
}



 


 



