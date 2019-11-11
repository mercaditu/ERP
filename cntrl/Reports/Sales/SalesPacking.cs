namespace cntrl.Reports.Sales
{
    public static class SalesPacking
    {
        public static string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	    CASE
      WHEN sales_packing.status = 1 THEN '" + entity.Brillo.Localize.StringText("Pending") + @"'
      WHEN sales_packing.status=2 THEN '" + entity.Brillo.Localize.StringText("Approved") + @"'
      WHEN sales_packing.status=3 THEN '" + entity.Brillo.Localize.StringText("Anulled") + @"'
      WHEN sales_packing.status=4 THEN '" + entity.Brillo.Localize.StringText("Rejected") + @"'
        END
      as Status,
sales_packing_detail.id_sales_packing_detail as DetailID,
												sales_packing.number as Number,
												Date(sales_packing.trans_date) as Date,
												sales_packing.timestamp as TimeStamp,
												app_branch.name as Branch,
												app_terminal.name as Terminal,
												contacts.name as Customer,
												contacts.gov_code as GovCode,
												contacts.code as CustomerCode,
												contacts.address as Address,
												sales_packing.comment as Comment,
												items.code as Code,
												items.name as Items,
                                                expire_date as ExpireDate,
                                                batch_code as BatchCode,
                                                verified_quantity as VerifiedQuantity,
												quantity as Quantity,
                                                sales_packing.id_user as User,
                                                ( SELECT contact_tag.name FROM contact_tag left join contact_tag_detail on contact_tag.id_tag = contact_tag_detail.id_tag
                                                  left join contacts on contact_tag_detail.id_contact = contacts.id_contact limit 0,1) as Contact_Tag
                                                
												from sales_packing_detail
												inner join sales_packing on sales_packing_detail.id_sales_packing=sales_packing.id_sales_packing
												inner join contacts on sales_packing.id_contact = contacts.id_contact
												inner join items on sales_packing_detail.id_item = items.id_item
												left join app_terminal on sales_packing.id_terminal = app_terminal.id_terminal
												inner join app_branch on app_branch.id_branch=sales_packing.id_branch
											  where sales_packing.trans_date between '@StartDate' and '@EndDate' and sales_packing.id_company = @CompanyID and sales_packing_detail.user_verified=true
                                              order by sales_packing.trans_date";
    }
}