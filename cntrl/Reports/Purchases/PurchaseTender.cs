using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Purchases
{
   public static class PurchaseTender
    {
        public static string query = @" select pt.id_purchase_tender, 
pt.status,
pt.name as TenderName, 
pt.number AS TenderNumber,
projects.name AS ProjectName,
app_currency.name as Currency,
 c.name as Supplier,
condi.name as PurchaseCondition, 
contract.name as Contract,
 pti.item_description as ItemDescription,
ptd.unit_cost as Cost,
 pti.quantity as Quantity, 
pod.quantity as Ordered

 from purchase_tender pt
 inner join purchase_tender_contact as ptc on pt.id_purchase_tender = ptc.id_purchase_tender
	inner join app_currencyfx on app_currencyfx.id_currencyfx=purchase_tender_contact.id_currencyfx
inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency

 inner join contacts as c on ptc.id_contact = c.id_contact
 inner join app_contract as contract on ptc.id_contract = contract.id_contract
 inner join app_condition as condi on contract.id_condition = condi.id_condition

 inner join purchase_tender_detail as ptd on ptd.id_purchase_tender_contact = ptc.id_purchase_tender_contact
 left join purchase_order_detail as pod on ptd.id_purchase_tender_detail = pod.id_purchase_tender_detail
 inner join purchase_tender_item as pti on ptd.id_purchase_tender_item = pti.id_purchase_tender_item
 left join items as i on pti.id_item = i.id_item
 left outer join projects on pt.id_project = projects.id_project
 where pt.trans_date between '@StartDate' and '@EndDate' and pt.id_company = @CompanyID";
    }
}
