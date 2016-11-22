  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.Stock
{
    public static class Merchandise

    {
        public static string query = @" select branch.name as BranchName,
                inv.comment as TransComment,
                item.code as ItemCode,
                item.name as ItemName,
                inv.credit as Credit,
				inv.debit as Debit,
                UnitCost,
                (UnitCost* inv.credit) as TotalCost,
                inv.trans_date as TransDate,
	(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = item.id_item order by item_tag_detail.is_default limit 0,1) as Tag

              from(
              select item_movement.*, sum(val.unit_value) as UnitCost
              from item_movement
              left outer join item_movement_value as val on item_movement.id_movement = val.id_movement
              where item_movement.id_company = {0} and item_movement.trans_date between '{1}' and '{2}' 
              and (
                    item_movement.id_purchase_invoice_detail > 0 or 
                    item_movement.id_execution_detail > 0 or 
                    item_movement.id_inventory_detail > 0 or
                    item_movement.id_transfer_detail > 0)

              group by item_movement.id_movement
                ) as inv

              inner join item_product as prod on inv.id_item_product = prod.id_item_product
              inner join items as item on prod.id_item = item.id_item
              inner join app_location as loc on inv.id_location = loc.id_location
              inner join app_branch as branch on loc.id_branch = branch.id_branch
              
              group by inv.id_movement
              order by inv.trans_date";
    }
}



 


 



