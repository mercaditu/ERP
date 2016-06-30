create definer=`root`@`%` procedure `budgetstatus`(in startdate datetime,
												  in enddate datetime,
                                                  in idcompany int(11))
begin
select 
	cont.name,
    cont.gov_code,
    s.number,
    s.status,
    sum(sd.quantity) as quantity,
    round(sum(sd.unit_price * vatco.coef),4) as price,
    round(sum(sd.unit_price * vatco.coef * sd.quantity),4) as total
from
sales_budget as s
inner join sales_budget_detail as sd
on sd.id_sales_budget = s.id_sales_budget
left join contacts as cont
on cont.id_contact = s.id_contact
left join app_condition as cond
on cond.id_condition = s.id_condition
left join
	(select app_vat_group.id_vat_group,sum(app_vat.coefficient) + 1 as coef
    from app_vat_group 
	left join app_vat_group_details 
    on app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
	left join app_vat 
    on app_vat_group_details.id_vat = app_vat.id_vat 
    group by app_vat_group.id_vat_group) as vatco
on vatco.id_vat_group = sd.id_vat_group
where s.id_company = idcompany
and s.trans_date >= startdate
and s.trans_date <=enddate
group by 
s.id_sales_budget;
end