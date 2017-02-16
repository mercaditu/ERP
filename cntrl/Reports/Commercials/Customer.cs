namespace cntrl.Reports.Commercial
{
    public static class Customer
    {
        public static string query = @"
						select
						contacts.is_customer as Customer,
						contacts.is_supplier as Supplier,
						contacts.id_contact as ContactID,
						contacts.parent_id_contact as ParentID,
						contacts.is_active as Active,
						contacts.code as Code,
						contacts.name as Name,
						contacts.alias as Alias,
						contacts.gov_code as GovCode,
						contacts.address as Address,
						contacts.telephone as Telephone,
						contacts.email as Email,
						contact_tag.name as Tag,
						app_contract.name as Contract,
						contacts.credit_limit as CreditLimit,
						(select sum(credit - debit) from payment_schedual where id_contact = contacts.id_contact && can_calculate = 1) as AccountPayable,
						(select sum(debit - credit) from payment_schedual where id_contact = contacts.id_contact && can_calculate = 1) as AccountRecievable,
						contacts.date_birth as DateOfBirth,
						TIMESTAMPDIFF(YEAR,contacts.date_birth,CURDATE()) AS Age,
						geo.name as Geography,
						rep.name as SalesRep,
						bank.name as Bank,
						price.name as PriceList,
						role.name as ContactRole,
						curr.name as Currency,
						cc.name as CostCenter,
						if(contacts.gender is null, null, if(contacts.gender = 0, 'Male', 'Female')) as Gender

						from contacts

						left join contact_tag_detail on contacts.id_contact= contact_tag_detail.id_contact
						left join app_contract on contacts.id_contract=app_contract.id_contract
						left join contact_tag on contact_tag_detail.id_contact_tag_detail= contact_tag.id_tag
						left join app_geography as geo on contacts.id_geography = geo.id_geography
						left join sales_rep as rep on contacts.id_sales_rep = rep.id_sales_rep
						left join app_bank as bank on contacts.id_bank = bank.id_bank
						left join item_price_list as price on contacts.id_price_list = price.id_price_list
						left join app_cost_center as cc on contacts.id_cost_center = cc.id_cost_center
						left join app_currency as curr on contacts.id_currency = curr.id_currency
						left join contact_role as role on contacts.id_contact_role = role.id_contact_role
						where is_employee = 0 and (contacts.id_company = @CompanyID or contacts.id_company is null)
						order by contacts.name";
    }
}