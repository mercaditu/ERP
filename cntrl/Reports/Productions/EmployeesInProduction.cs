namespace cntrl.Reports.Production
{
    public static class EmployeesInProduction
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
select c.name as Employee,
										pt.item_description as Task,
										p.name as Project,
                                        pod.name as ProductionOrder,
										start_date as StartDate,
										end_date as EndDate,
										  htc.name as Coefficient,
										sum(time_to_sec(timediff(end_date,start_date)) / 3600)  as Hours,
										(sum(time_to_sec(timediff(end_date,start_date)) / 3600) * htc.coefficient)  as ComputeHours,
                                        (sum(time_to_sec(timediff(end_date,start_date)) / 3600)-(sum(time_to_sec(timediff(end_date,start_date)) / 3600) * htc.coefficient)) as diff,
                                        (1-( (sum(time_to_sec(timediff(end_date,start_date)) / 3600)-(sum(time_to_sec(timediff(end_date,start_date)) / 3600) * htc.coefficient))/sum(time_to_sec(timediff(end_date,start_date)) / 3600))) as diffPer
									from production_execution_detail as ped

									inner join hr_time_coefficient as htc on ped.id_time_coefficient = htc.id_time_coefficient
									inner join production_order_detail as pod on ped.id_order_detail = pod.id_order_detail
                                    left join contacts as c
									on c.id_contact  = ped.id_contact

									left join  project_task as pt
									 on pt.id_project_task = ped.id_project_task
									left join projects as p
									on  p.id_project = pt.id_project
									where ped.id_contact is not null
								 	and ped.id_company = @CompanyID
									and ped.trans_date between '@StartDate' and '@EndDate'
									group by ped.id_execution_detail
									order by c.name";
    }
}