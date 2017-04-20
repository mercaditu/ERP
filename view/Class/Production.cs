using System;
using System.Collections.Generic;
using System.Data;

namespace Cognitivo.Class
{
    public class Logistics
    {
        public entity.Status.Production Status { get; set; }
        public int ItemID { get; set; }

        public entity.item.item_type Type { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemMask { get; set; }
        public decimal Quantity { get; set; }

        public decimal Availability { get; set; }

        //how much has been already requested
        public decimal Requested { get; set; }
    }

    internal class Production
    {
        public DataTable Get_Production(int ProductionOrderID, DateTime StartDate, DateTime EndDate)
        {
            string query = @" 
                                  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                 SELECT p.name as Project, l.name as Line, po.name as ProjectName, po.work_number as Number, po.trans_date as TransDate, po.project_cost_center as CostCenter,
                                pod.id_order_detail as OrderID,
                                pod.parent_id_order_detail as ParentID,

                                 pod.is_input as Input, pod.code as Code, pod.name as Item, pod.quantity as Quantity,
                                pod.start_date_est as StartDate, pod.end_date_est as EndDate

                                ,  case pod.status when 1 then 'Pending' when 2 then 'Approved' when 3 then 'InProcess' when 4 then 'Executed' when 5 then 'Rejected' when 6 then 'Management_Approved' end as status

                                from production_order as po

                                left join projects as p on po.id_project = p.id_project

                                inner join production_line as l on po.id_production_line = l.id_production_line

                                inner join production_order_detail as pod on po.id_production_order = pod.id_production_order

                                where (po.id_company = {0} and po.trans_date >= '{1}' and po.trans_date <= '{2}')";

            query = string.Format(query, ProductionOrderID, StartDate.ToString("yyyy-MM-dd 23:59:59"), EndDate.ToString("yyyy-MM-dd 23:59:59"));
            return Generate.DataTable(query);
        }

        public List<Logistics> Return_OrderLogistics(int ProductionOrderID)
        {
            string query = @"
                                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select
	                            pod.status as ProductionStatus,
                                pod.id_item,
	                            pod.id_item ItemID, i.id_item_type as Type, i.code as ItemCode, i.name as ItemName, pod.name as Mask,
	                            sum(pod.quantity) as Quantity,
	                            (select sum(credit - debit) from item_movement where id_item_product = ip.id_item_product) as Availability,
                                (select sum(quantity) from item_request_detail where id_order_detail = pod.id_order_detail) as Requested

	                            from production_order_detail as pod

	                            left join items as i on pod.id_item = i.id_item
	                            left join item_product as ip on i.id_item = ip.id_item
								where pod.id_production_order = {0} and is_input = 1
								group by pod.id_item";

            query = string.Format(query, ProductionOrderID);
            return GenerateLOG(Generate.DataTable(query));
        }

        private List<Logistics> GenerateLOG(DataTable dt)
        {
            List<Logistics> LogisticsList = new List<Logistics>();
            foreach (DataRow DataRow in dt.Rows)
            {
                Logistics Logistics = new Logistics();
                if (!(DataRow["ItemID"] is DBNull))
                {
                    Logistics.ItemID = Convert.ToInt16(DataRow["ItemID"]);
                }
               
                Logistics.ItemCode = DataRow["ItemCode"].ToString();
                Logistics.ItemName = DataRow["ItemName"].ToString();

                Logistics.Quantity = !DataRow.IsNull("Quantity") ? Convert.ToDecimal(DataRow["Quantity"]) : 0;
                Logistics.Availability = !DataRow.IsNull("Availability") ? Convert.ToDecimal(DataRow["Availability"]) : 0;
                Logistics.Requested = !DataRow.IsNull("Requested") ? Convert.ToDecimal(DataRow["Requested"]) : 0;

                int ItemType = !DataRow.IsNull("Type") ? Convert.ToInt16(DataRow["Type"]) : 0;

                if (ItemType == 1)
                {
                    Logistics.Type = entity.item.item_type.Product;
                }
                else if (ItemType == 2)
                {
                    Logistics.Type = entity.item.item_type.RawMaterial;
                }
                else if (ItemType == 3)
                {
                    Logistics.Type = entity.item.item_type.Service;
                }
                else if (ItemType == 4)
                {
                    Logistics.Type = entity.item.item_type.FixedAssets;
                }
                else if (ItemType == 5)
                {
                    Logistics.Type = entity.item.item_type.Task;
                }
                else if (ItemType == 6)
                {
                    Logistics.Type = entity.item.item_type.Supplies;
                }
                else
                {
                    Logistics.Type = entity.item.item_type.ServiceContract;
                }

                int ProductionStatus = !DataRow.IsNull("ProductionStatus") ? Convert.ToInt16(DataRow["ProductionStatus"]) : 0;

                if (ProductionStatus == 2)
                {
                    Logistics.Status = entity.Status.Production.Approved;
                }
                else if (ProductionStatus == 7)
                {
                    Logistics.Status = entity.Status.Production.Anull;
                }
                else if (ProductionStatus == 4)
                {
                    Logistics.Status = entity.Status.Production.Executed;
                }
                else if (ProductionStatus == 3)
                {
                    Logistics.Status = entity.Status.Production.InProcess;
                }
                else //(ProductionStatus == 1)
                {
                    Logistics.Status = entity.Status.Production.Pending;
                }

                LogisticsList.Add(Logistics);
            }
            return LogisticsList;
        }
    }
}