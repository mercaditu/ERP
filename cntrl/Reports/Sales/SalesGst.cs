namespace cntrl.Reports.Sales
{
    public static class SalesGst
    {
        public static string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
Date(si.trans_date) as Date,
                                              (select
	    CASE
      WHEN si.status=1 THEN 'Pending'
      WHEN si.status=2 THEN 'Approved'
      WHEN si.status=3 THEN 'Anulled'
      WHEN si.status=4 THEN 'Rejected'
        END)
      as Status,
      si.number as Number,
v.name as Vat, 
c.name as Customer,
												c.gov_code as GovCode,
												c.code as CustomerCode,
												c.address as Address,
v.coefficient,
i.name as Item, 
i.code,
sid.quantity,
sid.unit_price,
(sid.quantity * sid.unit_price) as SubTotal,
(sid.quantity * sid.unit_price * (v.coefficient * vgd.percentage)) as TotalVAT,
(sid.quantity * sid.unit_price * (v.coefficient * vgd.percentage + 1)) as SubTotalVAT,
(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = i.id_item order by item_tag_detail.is_default limit 0,1) as Tag
from app_vat as v
inner join app_vat_group_details as vgd on 
v.id_vat=vgd.id_vat
inner join app_vat_group as avg
on vgd.id_vat_group=avg.id_vat_group
inner join sales_invoice_detail as sid
on avg.id_vat_group=sid.id_vat_group
inner join sales_invoice as si
on sid.id_sales_invoice=si.id_sales_invoice
inner join contacts as c
on si.id_contact=c.id_contact
inner join items as i
on sid.id_item=i.id_item
where  si.trans_date between '@StartDate' and '@EndDate' and si.id_company = @CompanyID and si.status = 2
order by si.trans_date";
    }
}