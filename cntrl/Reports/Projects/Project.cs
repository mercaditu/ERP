namespace cntrl.Reports.Project
{
    public static class Project
    {
        public static string query = @"
set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';

select proj.name as ProjectName,
task.id_project_task,
task.parent_id_project_task as ParentTask,
item.name as  Item,
item.code as ItemCode,
item.id_item_type,
CASE
      WHEN item.id_item_type=1 THEN '" + entity.Brillo.Localize.StringText("Product") + @"'
      WHEN item.id_item_type=2 THEN  '" + entity.Brillo.Localize.StringText("RawMaterial") + @"'
      WHEN item.id_item_type=3 THEN  '" + entity.Brillo.Localize.StringText("Service") + @"'
      WHEN item.id_item_type=4 THEN  '" + entity.Brillo.Localize.StringText("FixedAssets") + @"'
      WHEN item.id_item_type=5 THEN  '" + entity.Brillo.Localize.StringText("Task") + @"'
      WHEN item.id_item_type=6 THEN  '" + entity.Brillo.Localize.StringText("Supplies") + @"'
      WHEN item.id_item_type=7 THEN  '" + entity.Brillo.Localize.StringText("ServiceContract") + @"'
END as ItemType,
task.code as TaskCode,
task.item_description as Task,
task.status,
contacts.name as Contact,
contacts.code as ContactCode,
contacts.gov_code as GovermentId,
task.quantity_est as QuantityEst,

(SELECT GROUP_CONCAT(ROUND(value,2) SEPARATOR ' x ')
 from project_task_dimension where id_project_task = task.id_project_task) as Dimension,

ROUND(task.quantity_est,2) * item_conversion_factor.value * (select ROUND(EXP(SUM(LOG(`value`))),2) as value from project_task_dimension where id_project_task = task.id_project_task) as ConversionQuantity,
round((select ROUND(EXP(SUM(LOG(`value`/1000))),2 ) as value from project_task_dimension where id_project_task = task.id_project_task) * ROUND(task.quantity_est,2),2) as Factor,
sum(exe.Quantity) as QuantityReal,max(exe.start_date) as ExecStartDate,max(end_date) as ExecEndDate,
	sum(time_to_sec(timediff(end_date,start_date)) / 3600)  as Hours,
										(sum(time_to_sec(timediff(end_date,start_date)) / 3600) * htc.coefficient)  as ComputeHours,
                                        (sum(time_to_sec(timediff(end_date,start_date)) / 3600)-(sum(time_to_sec(timediff(end_date,start_date)) / 3600) * htc.coefficient)) as diff,
(1-( (sum(time_to_sec(timediff(end_date,start_date)) / 3600)-(sum(time_to_sec(timediff(end_date,start_date)) / 3600) * htc.coefficient))/sum(time_to_sec(timediff(end_date,start_date)) / 3600))) * 100 as diffPer,
(((sum(time_to_sec(timediff(end_date,start_date)) / 3600) * htc.coefficient))/task.completed) as CompletedHours,
task.unit_cost_est as CostEst,
sum(exe.unit_cost) as CostReal,
task.start_date_est as StartDate,
task.end_date_est as EndDate,
sum(sbd.quantity * sbd.unit_price) as TotalBudgeted,
sum(sid.quantity * sid.unit_price) as TotalInvoiced,
sum(ps.debit) as TotalPaid,
sum(sbd.quantity * sbd.unit_price)-sum(ps.debit) as Balance,
task.quantity_est-(if(TIMEDIFF( task.end_date_est, task.start_date_est )is null,0,TIMEDIFF( task.end_date_est, task.start_date_est ))) as QuantityAdditional,
if(task.importance > 0, task.importance, null) as AveragePercentage,
if(task.completed > 0, task.completed, null) as Percentage,
(select value from item_price inner join item_price_list on item_price.id_price_list = item_price_list.id_price_list where item_price.id_item = item.id_item and item_price_list.is_default limit 1) as Price
 ,executionemployee.name as EmployeeName

from project_task as task

inner join projects  as proj on proj.id_project = task.id_project
inner join contacts   on proj.id_contact = contacts.id_contact

inner join items as item on task.id_item = item.id_item
left join  item_product on item_product.id_item=item.id_item
left join item_conversion_factor on item_conversion_factor.id_item_product = item_product.id_item_product
left join  production_execution_detail as exe on task.id_project_task = exe.id_project_task
 left join  contacts as executionemployee   on exe.id_contact = executionemployee.id_contact
left join hr_time_coefficient as htc on exe.id_time_coefficient = htc.id_time_coefficient
left join sales_budget as sb on proj.id_project = sb.id_project
left join sales_budget_detail as sbd on sb.id_sales_budget = sbd.id_sales_budget
left join sales_invoice as si  on proj.id_project = si.id_project
left join sales_invoice_detail as sid on si.id_sales_invoice = sid.id_sales_invoice
left join payment_schedual as ps on ps.id_sales_invoice = si.id_sales_invoice

where proj.id_company = @CompanyID and proj.id_project = @ProjectID

group by task.id_project_task ";
    }
}