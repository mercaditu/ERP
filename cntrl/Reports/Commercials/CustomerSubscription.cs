using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Commercials
{
    public static class CustomerSubscription
    {
        public static string query = @"SELECT c.id_contact as ContactID,c.parent_id_contact as ParentID,	c.name as Customer,contact_role.name as ContactRole,
												c.gov_code as GovCode,
												c.code as CustomerCode, 
												c.address as Address,
												if(c.gender = 0, 'Male', 'Female') as Gender,
                                                (select name from app_geography where id_geography=c.id_geography) as GeoLevel1,
												(select name from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=c.id_geography)) as GeoLevel2,

												(select name from app_geography where id_geography=
												 (select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=c.id_geography))) as GeoLevel3,
												(select name from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												 (select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=c.id_geography)))) as GeoLevel4,
												(select name from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												 (select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=c.id_geography))))) as GeoLevel5,
                                                	i.code as Code, 
												i.name as Items,
                                                (select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag, 
                                                vatco.Vat ,
                                                unit_price as Price,start_date as StartDate,end_date as EndDate,
                                                bill_cycle as BillCycle,bill_on as BillOn FROM 
 contact_subscription cs
inner join contacts c on c.id_contact=cs.id_contact
left join contact_role on contact_role.id_contact_role=c.id_contact_role
left join app_geography on app_geography.id_geography=c.id_geography
left join app_contract contract on contract.id_contract=cs.id_contract
 LEFT OUTER JOIN 
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient) + 1 AS coef ,app_vat_group.name as VAT
																FROM  app_vat_group  
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group  
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat  
																GROUP BY app_vat_group.id_vat_group)
				 
																vatco ON vatco.id_vat_group = cs.id_vat_group 
inner join items i on i.id_item=cs.id_item
";
    }
}
