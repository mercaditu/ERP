select sales_invoice.number as Number,Date(sales_invoice.trans_date) as SalesDate,app_branch.name as Branch , 
contacts.name as Customer,contacts.gov_code as CustGovCode,contacts.code as CustCode , contacts.address as Address,
app_currency.name as Currency, app_currencyfx.buy_value,
items.code as ItemCode, items.name as ItemName,
(select Name from item_tag_detail inner join item_tag
 on item_tag_detail.id_tag=Item_tag.id_tag where item_tag_detail.id_item=items.id_item order by item_tag_detail.is_default limit 0,1 ) as Tag, 
app_contract.name  as Contract,app_condition.name as SalesCondition,
quantity as Quantity, sales_invoice_detail.unit_cost as UnitCost,sales_invoice_detail.unit_price as UnitPrice,
round(( sales_invoice_detail.unit_price * vatco.coef),4) as UnitPriceVat,  
round((sales_invoice_detail.quantity * sales_invoice_detail.unit_price),4) as SubTotal,
 round((sales_invoice_detail.quantity * sales_invoice_detail.unit_price * vatco.coef),4) as SubTotalVat,
 (sales_invoice_detail.discount) as DiscountUnitPrice,
round((sales_invoice_detail.discount*vatco.coef),4) as DiscountUnitPriceVat,  
round((sales_invoice_detail.quantity * (sales_invoice_detail.discount)),4) as DiscountSubTotal,
 round((sales_invoice_detail.quantity * (sales_invoice_detail.discount*vatco.coef)),4) as DiscountSubTotalVat,
((sales_invoice_detail.quantity * sales_invoice_detail.unit_price * vatco.coef) - (sales_invoice_detail.quantity * sales_invoice_detail.unit_cost * vatco.coef)) / (sales_invoice_detail.quantity * sales_invoice_detail.unit_price * vatco.coef) as Margin,
	((sales_invoice_detail.quantity * sales_invoice_detail.unit_price * vatco.coef) - (sales_invoice_detail.quantity * sales_invoice_detail.unit_cost * vatco.coef)) / (sales_invoice_detail.quantity * sales_invoice_detail.unit_cost * vatco.coef) as MarkUp,
	 ((sales_invoice_detail.quantity * sales_invoice_detail.unit_price * vatco.coef) - (sales_invoice_detail.quantity * sales_invoice_detail.unit_cost * vatco.coef)) as Profit
from sales_invoice_detail  
inner join sales_invoice 
on sales_invoice_detail.id_sales_invoice=sales_invoice.id_sales_invoice 
inner join contacts on sales_invoice.id_contact = contacts.id_contact  
inner join items on sales_invoice_detail.id_item = items.id_item 
         LEFT OUTER JOIN 
             (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient) + 1 AS coef 
				FROM  app_vat_group  
					LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group  
					LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat  
				GROUP BY app_vat_group.id_vat_group)  
                vatco ON vatco.id_vat_group = sales_invoice_detail.id_vat_group 
                inner join app_branch on app_branch.id_branch=sales_invoice.id_branch
                inner join app_currencyfx on app_currencyfx.id_currencyfx=sales_invoice.id_currencyfx
                inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
                inner join app_contract on app_contract.id_contract=sales_invoice.id_contract
                inner join app_condition on app_condition.id_condition=sales_invoice.id_condition
   where sales_invoice.trans_date between @StartDate and @EndDate and sales_invoice.id_company = @CompanyID
   order by sales_invoice.trans_date