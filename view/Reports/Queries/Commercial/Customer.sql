select contacts.code as Code,
 contacts.name as Name ,
  contacts.gov_code as GovCode, 
contacts.address as Address,
 contacts.telephone as Telephone,
contact_tag.name as Tag,
app_contract.name as Contract,
credit_limit
from contacts
inner join app_company on contacts.id_company= app_company.id_company
left join contact_tag_detail on contacts.id_contact= contact_tag_detail.id_contact 
left join app_contract on contacts.id_contract=app_contract.id_contract 
left join contact_tag on contact_tag_detail.id_contact_tag_detail= contact_tag.id_tag
 where (is_customer = @IsCustomer or is_supplier = @IsSupplier) and is_employee = 0