namespace cntrl.Reports.Finance
{
    public static class PaymentRecievables
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
										select
										if(ps.id_payment_schedual is null, pschild.parent_id_payment_schedual, ps.id_payment_schedual) as Refrence,
                                        contact.name as Customer,
                                        contact.telephone as Telephone,
                                        contact.address as Address,
                                        contact.code as Code,
                                        contact.gov_code as GovCode,
                                        branch.name as Branch,
                                        cond.name as Conditions,
                                        contract.name as Contract,
                                        si.number as SalesNumber,
                                        si.trans_date as Date,
                                        sr.name as Salesman,
                                        ps.trans_date as PaymentDate,
                                        Date(ps.expire_date) as ExpiryDate,
                                        ps.debit as Debit,
                                        pschild.credit as Credit,
                                        payment.number as PaymentNumber,
                                        payment.trans_date as Payment,
                                        pd.value as Paid,
                                        pt.name as PaymentType,
                                        c.name as Currency,
                                        cfx.buy_value as Rate,
                                        datediff(ps.expire_date, ifnull(payment.trans_date, Now())) as DateDiff

                                        from payment_schedual as ps
                                        left join payment_schedual as pschild on ps.id_payment_schedual = pschild.parent_id_payment_schedual

                                        right join sales_invoice as si on ps.id_sales_invoice = si.id_sales_invoice
                                        left join contacts as contact on si.id_contact = contact.id_contact
                                        left join app_branch as branch on si.id_branch = branch.id_branch
                                        left join app_contract as contract on si.id_contract = contract.id_contract
                                        left join app_condition as cond on si.id_condition = cond.id_condition
                                        left join payment_detail as pd on pschild.id_payment_detail = pd.id_payment_detail
                                        left join payments as payment on pd.id_payment = payment.id_payment
                                        left join payment_type as pt on pd.id_payment_type = pt.id_payment_type
                                        left join app_currencyfx as cfx on pd.id_currencyfx = cfx.id_currencyfx
                                        left join app_currency as c on cfx.id_currency = c.id_currency
                                        left join sales_rep as sr on si.id_sales_rep = sr.id_sales_rep

                                        where ps.id_sales_invoice > 0 and ps.parent_id_payment_schedual is null and ps.id_company = @CompanyID and ps.trans_date between '@StartDate' and '@EndDate'";
    }
}