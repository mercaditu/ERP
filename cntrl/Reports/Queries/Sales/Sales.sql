select 
sales_invoice.status as Status,
sales_invoice.number as Number,
sales_invoice.is_impex as Exports,
Date(sales_invoice.trans_date) as Date,
sales_invoice.timestamp as TimeStamp,
app_branch.name as Branch,
app_terminal.name as Terminal,
app_contract.name  as Contract,
app_condition.name as Conditions,
contacts.name as Customer,
contacts.gov_code as GovCode,
contacts.code as CustomerCode, 
contacts.address as Address,
app_currency.name as Currency, 
app_currencyfx.buy_value as Rate,
sales_rep.name as SalesRep,
sales_invoice.comment as Comment,
items.code as Code, 
items.name as Items,
vatco.Vat,
projects.name as Project,

(select name from item_tag_detail inner join item_tag on item_tag_detail.id_tag = item_tag.id_tag where item_tag_detail.id_item = items.id_item order by item_tag_detail.is_default limit 0,1) as Tag, 
quantity as Quantity, 
sales_invoice_detail.unit_cost as UnitCost,
sales_invoice_detail.unit_price as UnitPrice,
round(( sales_invoice_detail.unit_price * vatco.coef),4) as UnitPriceVat,  
round((sales_invoice_detail.quantity * sales_invoice_detail.unit_price),4) as SubTotal,
round((sales_invoice_detail.quantity * sales_invoice_detail.unit_price * vatco.coef),4) as SubTotalVat,
round(sales_invoice_detail.discount, 4) as Discount,
round((sales_invoice_detail.quantity * (sales_invoice_detail.discount * vatco.coef)),4) as Discount,

(sales_invoice_detail.unit_price - sales_invoice_detail.unit_cost) / (sales_invoice_detail.unit_price) as Margin,
(sales_invoice_detail.unit_price - sales_invoice_detail.unit_cost) / (sales_invoice_detail.unit_cost) as MarkUp,
(sales_invoice_detail.unit_price - sales_invoice_detail.unit_cost) as Profit

from sales_invoice_detail
inner join sales_invoice on sales_invoice_detail.id_sales_invoice=sales_invoice.id_sales_invoice 
left join sales_rep on sales_invoice.id_sales_rep = sales_rep.id_sales_rep
inner join contacts on sales_invoice.id_contact = contacts.id_contact  
inner join items on sales_invoice_detail.id_item = items.id_item
left join app_terminal on sales_invoice.id_terminal = app_terminal.id_terminal
	 LEFT OUTER JOIN 
			 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient) + 1 AS coef ,app_vat_group.name as VAT
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
				left join projects on projects.id_project=sales_invoice.id_project
   where sales_invoice.trans_date between @StartDate and @EndDate and sales_invoice.id_company = @CompanyID

   order by sales_invoice.trans_date