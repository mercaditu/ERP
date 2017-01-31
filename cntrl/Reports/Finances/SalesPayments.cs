
namespace cntrl.Reports.Finances
{
    public static class SalesPayments
    {
        public static string query = @"
            select 
            ps.id_payment_schedual as SchedualID,
            c.name as Contact, 
            c.code as Code, 
            c.telephone as Telephone,
            c.address as Address,
            c.gov_code as GovID, 
            si.number as Number,
            si.trans_date as Date,
            contract.name as Contract, 
            Conditions.name as Conditions, 
            curr.name as Currency, 
            fx.buy_value as CurrencyRate,
            sum(ps.debit) as Debit, 
            sum(ps.credit) as Credit, 
            sr.number as SalesReturn,
            p.number as Payment,
            pt.name as Type,
            ps.expire_date as ExpiryDate,
            pd.trans_date as PaymentDate,  
            fx2.buy_value as PaymentRate, 
            curr2.name as Currency

            from payment_schedual as ps

            inner join sales_invoice as si on ps.id_sales_invoice = si.id_sales_invoice
            inner join app_contract as contract on si.id_contract = contract.id_contract
            inner join app_condition as conditions on contract.id_condition = conditions.id_condition
            inner join contacts as c on ps.id_contact = c.id_contact
            inner join app_currencyfx as fx on si.id_currencyfx = fx.id_currencyfx
            inner join app_currency as curr on fx.id_currency = curr.id_currency

            left join payment_detail as pd on ps.id_payment_detail = pd.id_payment_detail
            left join payments as p on pd.id_payment = p.id_payment
            left join payment_type as pt on pd.id_payment_type = pt.id_payment_type
            left join sales_return as sr on pd.id_sales_return = sr.id_sales_return
            left join app_currencyfx as fx2 on pd.id_currencyfx = fx2.id_currencyfx
            left join app_currency as curr2 on fx2.id_currency = curr2.id_currency
            where si.trans_date between @StartDate and @EndDate
            group by ps.id_sales_invoice 
            order by ps.id_payment_schedual";
    }
}
