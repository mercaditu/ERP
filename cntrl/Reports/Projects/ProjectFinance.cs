namespace cntrl.Reports.Project
{
    public static class ProjectFinance
    {
        public static string query = @"select
                            c.name as Contact,
							c.code as ContactCode,
							c.gov_code as GovermentId,
                            p.name as ProjectName,
                            sum(sbd.quantity * sbd.unit_price) as TotalBudgeted,
                            sum(sid.quantity * sid.unit_price) as TotalInvoiced,
                            sum(ps.debit) as TotalPaid,
                            sum(sbd.quantity * sbd.unit_price)-sum(ps.debit) as Balance

                            from projects as p
                            inner join contacts as c on p.id_contact = c.id_contact
                            left join sales_budget as sb on p.id_project = sb.id_project
                            left join sales_budget_detail as sbd on sb.id_sales_budget = sbd.id_sales_budget
                            left join sales_invoice as si  on p.id_project = si.id_project
                            left join sales_invoice_detail as sid on si.id_sales_invoice = sid.id_sales_invoice
                            left join payment_schedual as ps on ps.id_sales_invoice = si.id_sales_invoice

                            where p.id_company=@CompanyID and p.id_project=@ProjectID";
    }
}