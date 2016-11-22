select
po.id_production_order,
po.name as ProductionOrder,
pod.id_order_detail,
pod.parent_id_order_detail as Parent,
pod.name as TaskName,
pod.code as TaskCode,
pod.quantity as EstQuantity,
pod.start_date_est as OrderDate,
pe.start_date as StartDate,
 pe.end_date as EndDate,
htc.name as Coeficient,
time_to_sec(timediff(pe.end_date, pe.start_date)) / 3600 as Hours,
 pe.quantity as Quantity,
 pe.unit_cost as UnitCost, 
 (pe.unit_cost* pe.quantity) as TotalCost,
 c.name as Employee
 from production_order_detail as pod
 inner join production_order as po on pod.id_production_order = po.id_production_order
 left join production_execution_detail as pe on pe.id_order_detail = pod.id_order_detail
 left join contacts as c on pe.id_contact = c.id_contact
 left join hr_time_coefficient as htc on  pe.id_time_coefficient = htc.id_time_coefficient 
 where pod.id_company =@CompanyID and p.id_project=@ProjectID -- and pod.id_production_order = @OrderID
 order by pod.id_order_detail, pod.code