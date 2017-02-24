namespace cntrl.Reports.Production
{
    public static class Production
    {
        public static string query = @" SELECT
										p.name as Project,
										p.code as ProjectCode,
										c.name as Contact,
										l.name as Line,
										po.name as Production,
										po.work_number as Number,
										po.trans_date as Date,
										po.project_cost_center as CostCenter,
										pod.id_order_detail as OrderID,
										pod.parent_id_order_detail as ParentID,
										pod.status as Status,
										pod.is_input as Input,
										pod.code as Code,
										pod.name as Item,
										pod.quantity as QuantityOrdered,
										ped.quantity as QuantityExecuted,
										ped.unit_cost as CostExecuted,
                                        pt.unit_cost_est as CostEstimated,
										pod.start_date_est as StartDate,
										pod.end_date_est as EndDate,
                                        time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600  as Hours,
										(time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600) * htc.coefficient  as ComputeHours,
                                        (time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600)-(time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600) * htc.coefficient as diff,
(1-( (time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600)-((time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600) * htc.coefficient))/time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600) * 100 as diffPer,
pod.completed as Completed,pod.completed*100 as Percentage,
(((time_to_sec(timediff(ped.end_date,ped.start_date)) / 3600) * htc.coefficient) *100)/pod.completed as CompletedHours
										from production_order as po

										left join projects as p on po.id_project = p.id_project
										left join contacts as c on p.id_contact = c.id_contact
										inner join production_line as l on po.id_production_line = l.id_production_line
										inner join production_order_detail as pod on po.id_production_order = pod.id_production_order
                                        inner join project_task pt on pt.id_project_task=pod.id_project_task
										left join production_execution_detail as ped on pod.id_order_detail = ped.id_order_detail
                                        left join hr_time_coefficient as htc on ped.id_time_coefficient = htc.id_time_coefficient
										where po.id_company = @CompanyID and pod.trans_date between '@StartDate' and '@EndDate'";
    }
}