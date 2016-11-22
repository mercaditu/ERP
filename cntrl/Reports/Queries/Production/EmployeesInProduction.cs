namespace cntrl.Reports.Queries.Production
{
	public static class EmployeesInProduction
	{
		public static string query = @" select c.name as Employee,
										pt.item_description,
										p.name as Project,
											 start_date,
										end_date,
										  htc.name as Coefficient,
										sum(time_to_sec(timediff(end_date,start_date)) / 3600)  as Hours,
										sum(quantity)  as ComputeHours
   
									from production_execution_detail as ped 
									
									inner join hr_time_coefficient as htc on ped.id_time_coefficient = htc.id_time_coefficient 
									left join contacts as c
									on c.id_contact  = ped.id_contact 
									left join  project_task as pt
									 on pt.id_project_task = ped.id_project_task 
									left join projects as p
									on  p.id_project = pt.id_project
									where ped.id_contact is not null
									and ped.id_company = @CompanyID
									and ped.trans_date between @StartDate and @EndDate
									group by ped.id_contact
									order by c.name";
	}
}



 