using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.StockFlowDimension
{
    public static class Stock

    {
        public static string query = @" 
select branch.name as Branch

, loc.name as Location, imd.id_movement, item.code as  ItemCode, item.name as Item, im.debit, im.credit, sum(imv.unit_value) as Cost,
 dimension.name as Dimension,imd.value,im.trans_date,su.name_full  as UserName,
	(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = item.id_item order by item_tag_detail.is_default limit 0,1) as Tag
 from item_movement_dimension as imd
 inner join item_movement as im on imd.id_movement=im.id_movement
inner join app_dimension as dimension on imd.id_dimension=dimension.id_dimension
inner join app_location as loc on im.id_location=loc.id_location
inner join app_branch as branch on loc.id_branch=branch.id_branch
inner join item_product as ip on im.id_item_product=ip.id_item_product
inner join items as item on ip.id_item=item.id_item
inner join security_user as su on im.id_user=su.id_user
left outer join item_movement_value as imv on imd.id_movement=imv.id_movement
where im.id_company = @CompanyID and(im.trans_date >= @StartDate) AND(im.trans_date <= @EndDate)
 group by imd.id_movement_dimension
 order by id_movement
";
    }
}












