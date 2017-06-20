namespace cntrl.Reports.Finance
{
    public static class Payments
    {
        public static string query = @"
                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
						SELECT 
                        c.name as Contact,
                        c.code as Code,
                        c.gov_code as GovernmentID,
                        c.address as Address,
                        c.telephone as Telephone,
                        c.email as Email,
                        rep.name as Collector,
                        si.number as SalesNumber,
                        si.trans_date as SalesDate,
                        srep.name as Salesman,
                        pi.number as PurchaseNumber,
                        pi.trans_date as PurchaseDate,
                        pt.name as PaymentType,
                        p.number as Number,
                        acc.name as Account,
                        acc.code as AccountNumber,
                        pd.payment_type_number as PaymentTypeNumber,
                        p.trans_date as PaymentDate,
                        pd.trans_date as PaymentDate,
                        pd.value as Value,
                        curr.code as Currency,
                        fx.buy_value as BuyRate,
                        pd.comment as Comment,
                        sum(sch.debit) as Debit,
                        sum(sch.credit) as Credit
                         FROM payment_detail as pd
                        inner join payments as p on pd.id_payment = p.id_payment
                        inner join payment_type as pt on pd.id_payment_type = pt.id_payment_type
                        inner join contacts as c on p.id_contact = c.id_contact
                        inner join app_currencyfx as fx on pd.id_currencyfx = fx.id_currencyfx
                        inner join app_currency as curr on fx.id_currency = curr.id_currency
                        inner join app_account as acc on pd.id_account = acc.id_account
                        inner join app_branch as branch on p.id_branch = branch.id_branch
                        left join sales_rep as rep on p.id_sales_rep = rep.id_sales_rep
                        left join payment_schedual as sch on pd.id_payment_detail = sch.id_payment_detail
                        left join sales_invoice as si on sch.id_sales_invoice = si.id_sales_invoice
                        left join purchase_invoice as pi on sch.id_purchase_invoice = pi.id_purchase_invoice
                        left join sales_rep as srep on si.id_sales_rep = rep.id_sales_rep
                        where p.id_company = {0} and pd.trans_date between @StartDate and @EndDate
                        group by pd.id_payment_detail";
    }
}