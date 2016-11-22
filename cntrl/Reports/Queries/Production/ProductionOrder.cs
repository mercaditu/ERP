	 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cntrl.Reports.Queries.Production
{
    public static class ProductionOrder
    {
        public static string query = @" 	SELECT p.name as Project,
											 l.name as Line,
											  po.name as ProductiontName,
											   po.work_number as Number, 
											   po.trans_date as TransDate,
												po.project_cost_center as CostCenter,
											pod.id_order_detail as OrderID,
											pod.parent_id_order_detail as ParentID,
												pod.status as ProductionStatus,
												 pod.is_input as Input,
												  pod.code as Code,
												   pod.name as Item, 
												   pod.quantity as Quantity,
											pod.start_date_est as StartDate,
											 pod.end_date_est as EndDate
    
											from production_order as po
    
											left join projects as p on po.id_project = p.id_project
											inner join production_line as l on po.id_production_line = l.id_production_line
											inner join production_order_detail as pod on po.id_production_order = pod.id_production_order
											where po.id_company =@CompanyID  and pod.trans_date >= @start_date
									  and pod.trans_date <= @end_date";
    }
}



 
