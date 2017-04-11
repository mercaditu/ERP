namespace cntrl.Reports.Project
{
    public static class ProjectFinance
    {
        public static string query = @"select
                            c.name as Contact,
							c.code as ContactCode,
							c.gov_code as GovermentId,
                            p.name as ProjectName,
                            p.code as ProjectCode,
                        (    select
                            sum(sbd.quantity * sbd.unit_price) 
                            from sales_budget_detail as sbd
                            join sales_budget as sb on sb.id_sales_budget = sbd.id_sales_budget
                            where p.id_project = sb.id_project
                        ) as TotalBudgeted,
                        (    select
                            sum(sid.quantity * sid.unit_price) 
                            from sales_invoice_detail as sid 
                            join sales_invoice as si on si.id_sales_invoice = sid.id_sales_invoice
                            where p.id_project = si.id_project
                        ) as TotalInvoiced,
                        (    select
                            sum(ps.credit)
                            from payment_schedual as ps 
							join sales_invoice as si on ps.id_sales_invoice = si.id_sales_invoice
                            where p.id_project = si.id_project
                        ) as TotalPaid
                            
                            from projects as p
                            inner join contacts as c on p.id_contact = c.id_contact
                            where p.id_company=@CompanyID and p.id_project=@ProjectID";
    }
}