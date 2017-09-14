namespace cntrl.Reports.Sales
{
    public static class Sales
    {
        public static string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	    CASE
      WHEN sales_invoice.status=1 THEN '" + entity.Brillo.Localize.StringText("Pending") + @"'
      WHEN sales_invoice.status=2 THEN '" + entity.Brillo.Localize.StringText("Approved") + @"'
      WHEN sales_invoice.status=3 THEN '" + entity.Brillo.Localize.StringText("Anulled") + @"'
      WHEN sales_invoice.status=4 THEN '" + entity.Brillo.Localize.StringText("Rejected") + @"'
        END
      as Status,
	CASE
      WHEN sales_invoice.trans_type=0 THEN '" + entity.Brillo.Localize.StringText("Normal") + @"'
      WHEN sales_invoice.trans_type=1 THEN '" + entity.Brillo.Localize.StringText("Bonificacion") + @"'
      WHEN sales_invoice.trans_type=2 THEN '" + entity.Brillo.Localize.StringText("Change") + @"'
      WHEN sales_invoice.trans_type=3 THEN '" + entity.Brillo.Localize.StringText("Marketing") + @"'
      WHEN sales_invoice.trans_type=2 THEN '" + entity.Brillo.Localize.StringText("Sample") + @"'
      WHEN sales_invoice.trans_type=3 THEN '" + entity.Brillo.Localize.StringText("Other") + @"'
    END
 as Type,
sales_invoice_detail.id_sales_invoice_detail as DetailID,
												sales_invoice.number as Number,
												sales_invoice.is_impex as Exports,
												Date(sales_invoice.trans_date) as Date,
                                                -- sales_invoice.trans_date as Date,
												sales_invoice.timestamp as TimeStamp,
												app_branch.name as Branch,
												app_terminal.name as Terminal,
												app_contract.name  as Contract,
												app_condition.name as Conditions,
												contacts.name as Customer,
												contacts.gov_code as GovCode,
												contacts.code as CustomerCode,
												contacts.address as Address,
												if(contacts.gender = 0, 'Male', 'Female') as Gender,
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
												round((sales_invoice_detail.quantity * (sales_invoice_detail.discount * vatco.coef)),4) as DiscountVat,
												(sales_invoice_detail.unit_price - sales_invoice_detail.unit_cost) / (sales_invoice_detail.unit_price) as Margin,
												(sales_invoice_detail.unit_price - sales_invoice_detail.unit_cost) / (sales_invoice_detail.unit_cost) as MarkUp,
                                                (sales_invoice_detail.quantity * sales_invoice_detail.unit_cost) as SubTotalCost,
                                                (sales_invoice_detail.unit_price - sales_invoice_detail.unit_cost) as Profit,
												(select name from app_geography where id_geography=contacts.id_geography) as GeoLevel1,
												(select name from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=contacts.id_geography)) as GeoLevel2,

												(select name from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=contacts.id_geography))) as GeoLevel3,
												(select name from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=contacts.id_geography)))) as GeoLevel4,
												(select name from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=
												(select parent_id_geography from app_geography where id_geography=contacts.id_geography))))) as GeoLevel5,
                                                sales_invoice.id_user as User,
                                                ( SELECT contact_tag.name FROM contact_tag left join contact_tag_detail on contact_tag.id_tag = contact_tag_detail.id_tag
                                                  left join contacts on contact_tag_detail.id_contact = contacts.id_contact) as Contact_Tag
                                                

												from sales_invoice_detail
												inner join sales_invoice on sales_invoice_detail.id_sales_invoice=sales_invoice.id_sales_invoice
												left join sales_rep on sales_invoice.id_sales_rep = sales_rep.id_sales_rep
												inner join contacts on sales_invoice.id_contact = contacts.id_contact
												left join app_geography on app_geography.id_geography=contacts.id_geography
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
											  where sales_invoice.trans_date between '@StartDate' and '@EndDate' and sales_invoice.id_company = @CompanyID
                                              order by sales_invoice.trans_date";
    }
}