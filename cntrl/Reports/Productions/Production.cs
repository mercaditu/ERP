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
	                            CASE
                              WHEN pod.status=1 THEN '" + entity.Brillo.Localize.StringText("Pending") + @"'
                              WHEN pod.status=2 THEN '" + entity.Brillo.Localize.StringText("Approved") + @"'
                              WHEN pod.status=3 THEN '" + entity.Brillo.Localize.StringText("InProcess") + @"'
                              WHEN pod.status=4 THEN '" + entity.Brillo.Localize.StringText("Executed") + @"'
                              WHEN pod.status=5 THEN '" + entity.Brillo.Localize.StringText("QA_Check") + @"'
                              WHEN pod.status=6 THEN '" + entity.Brillo.Localize.StringText("QA_Rejected") + @"'
                              WHEN pod.status=7 THEN '" + entity.Brillo.Localize.StringText("Anulled") + @"'
                                END
                                    as Status,
CASE
      WHEN i.id_item_type=1 THEN '" + entity.Brillo.Localize.StringText("Product") + @"'
      WHEN i.id_item_type=2 THEN  '" + entity.Brillo.Localize.StringText("RawMaterial") + @"'
      WHEN i.id_item_type=3 THEN  '" + entity.Brillo.Localize.StringText("Service") + @"'
        WHEN i.id_item_type=4 THEN  '" + entity.Brillo.Localize.StringText("FixedAssets") + @"'
        WHEN i.id_item_type=5 THEN  '" + entity.Brillo.Localize.StringText("Task") + @"'
        WHEN i.id_item_type=6 THEN  '" + entity.Brillo.Localize.StringText("Supplies") + @"'
   WHEN i.id_item_type=7 THEN  '" + entity.Brillo.Localize.StringText("ServiceContract") + @"'
    END as Type,
                                        pod.is_input as Input,
                                        pod.code as Code,
                                        pod.name as Item,
                                        pod.quantity as QuantityOrdered,
                                        ped.quantity as QuantityExecuted,
                                        ped.unit_cost as CostExecuted,
                                        pt.unit_cost_est as CostEstimated,
                                        pod.start_date_est as StartDate,
                                        pod.end_date_est as EndDate,
                                        ped.quantity * icf.value * (select ROUND(EXP(SUM(LOG(`value`))),4) as value from production_execution_dimension where id_execution_detail = ped.id_execution_detail) as ConversionQuantity,
                                        (select ROUND(EXP(SUM(LOG(`value`))),4) as value from production_execution_dimension where id_execution_detail = ped.id_execution_detail)  * ped.quantity as Factor,

                                        sum(time_to_sec(timediff(ped.end_date, ped.start_date)) / 3600) as Hours,
                                        sum((time_to_sec(timediff(ped.end_date, ped.start_date)) / 3600)* htc.coefficient) as ComputeHours,
                                        sum(pod.quantity - ((time_to_sec(timediff(ped.end_date, ped.start_date)) / 3600)* htc.coefficient)) as diff,
                                        sum(((time_to_sec(timediff(ped.end_date, ped.start_date)) / 3600)) / pod.quantity) as diffPer,
                                        pod.completed as Completed, 
                                        pod.completed * 100 as Percentage,
                                        sum((((time_to_sec(timediff(ped.end_date, ped.start_date)) / 3600) * htc.coefficient))/pod.completed) as CompletedHours,
                                        (select GROUP_CONCAT(ROUND(value, 2) SEPARATOR ' x ') from production_execution_dimension where id_execution_detail = ped.id_execution_detail) value,
                                        am.name as Measurement

                                        from production_order as po

                                        left join projects as p on po.id_project = p.id_project
                                        left join contacts as c on p.id_contact = c.id_contact
                                        inner join production_line as l on po.id_production_line = l.id_production_line
                                        inner join production_order_detail as pod on po.id_production_order = pod.id_production_order
                                        inner join items as i on pod.id_item = i.id_item
                                        left join item_product as ip on i.id_item = ip.id_item
                                        left join item_conversion_factor as icf on ip.id_item_product = icf.id_item_product

                                        left join production_execution_detail as ped on pod.id_order_detail = ped.id_order_detail

                                        left join production_execution_dimension as pd on pd.id_execution_detail = ped.id_execution_detail
                                        left join app_dimension as ad on ad.id_dimension = pd.id_dimension
                                        left join app_measurement as am on am.id_measurement = pd.id_measurement
                                        left join project_task pt on pt.id_project_task=pod.id_project_task
                                        left join hr_time_coefficient as htc on ped.id_time_coefficient = htc.id_time_coefficient
                                        where po.id_company = @CompanyID and pod.trans_date between '@StartDate' and '@EndDate'
                                        group by ped.id_execution_detail;"
;
    }
}