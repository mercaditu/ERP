namespace cntrl.Reports.Project
{
    public static class ProjectFinance
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
                            c.name as Contact,
							c.code as ContactCode,
							c.gov_code as GovermentId,
                            p.name as ProjectName,
                            p.code as ProjectCode,
                        (    select
                            sum(sbd.quantity * sbd.unit_price) 
                            from sales_budget_detail as sbd
                            join sales_budget as sb on sb.id_sales_budget = sbd.id_sales_budget
                            LEFT OUTER JOIN
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient) + 1 AS coef ,app_vat_group.name as VAT
																FROM  app_vat_group
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
																GROUP BY app_vat_group.id_vat_group)

																vatco ON vatco.id_vat_group = sbd.id_vat_group
                            where p.id_project = sb.id_project
                        ) as TotalBudgeted,
                        (    select
                            sum(sid.quantity * sid.unit_price) 
                            from sales_invoice_detail as sid 
                            join sales_invoice as si on si.id_sales_invoice = sid.id_sales_invoice
                             LEFT OUTER JOIN
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient) + 1 AS coef ,app_vat_group.name as VAT
																FROM  app_vat_group
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
																GROUP BY app_vat_group.id_vat_group)

																vatco ON vatco.id_vat_group = sid.id_vat_group
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