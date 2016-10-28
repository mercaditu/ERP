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
		public decimal Requested { get; set; }
	}

	class Production
	{
		public List<Logistics> Return_OrderLogistics(int ProductionOrderID)
		{
			string query = @"select 
								pod.status as ProductionStatus, 
								pod.id_item ItemID, i.id_item_type as Type, i.code as ItemCode, i.name as ItemName, pod.name as Mask,
								sum(pod.quantity) as Quantity, 
								(select sum(credit - debit) as Available 
									from item_movement where id_item_product = ip.id_item_product) as Availability
								,ird.quantity as Requested
								from production_order_detail as pod
								left join item_request_detail as ird on pod.id_item = ird.id_item
								inner join items as i on pod.id_item = i.id_item
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
				Logistics.ItemID = Convert.ToInt16(DataRow["ItemID"]);
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
				else if(ItemType == 5)
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
