using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.Finance
{
    public static class PendingPayables
    {
        public static string query = @"select
								contact.code as Code,
								contact.gov_code as GovID,
								contact.name as Contact,
								si.number as Number,
								si.comment as Comment,
								cond.name as Conditions,
								contract.name as Contract,
								DATE_FORMAT(schedual.expire_date,'%d %b %y') as ExpiryDate,
								curr.name as CurrencyName,
								fx.buy_value as Rate,
								schedual.credit as Value, 
								schedual.DebitChild as Paid, 
								(schedual.credit - schedual.DebitChild) as Balance,
								schedual.trans_date as TransDate

								from(
								select
									parent.*,
									( select if(sum(debit) is null, 0, sum(debit)) 
									from payment_schedual as child where child.parent_id_payment_schedual = parent.id_payment_schedual
									) as DebitChild
								from payment_schedual as parent
								where parent.id_company = {0} and parent.trans_date <= '{1}'
								group by parent.id_payment_schedual

								) as schedual

								inner join contacts as contact on schedual.id_contact = contact.id_contact
								inner join app_currencyfx as fx on schedual.id_currencyfx = fx.id_currencyfx
								inner join app_currency as curr on fx.id_currency = curr.id_currency
								inner join purchase_invoice as si on schedual.id_purchase_invoice = si.id_purchase_invoice
								left join app_contract as contract on si.id_contract = contract.id_contract
								left join app_condition as cond on contract.id_condition = cond.id_condition
								where(schedual.credit - schedual.DebitChild) > 0
								group by schedual.id_payment_schedual
								order by schedual.expire_date";
    }
}


