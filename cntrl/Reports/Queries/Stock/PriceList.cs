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
pl.name as PriceList,
pi.value as Price,
currency.name as Currency

from items as item
left join item_price as pi on item.id_item = pi.id_item

left join item_price_list as pl on pi.id_price_list = pl.id_price_list

inner join app_currency as currency on pi.id_currency = currency.id_currency


order by item.name";
    }
}



 


 



