using System;
using System.Collections.Generic;
using System.Data;

namespace entity.BrilloQuery
{
	public class Purchase
	{
		public List<ReturnInvoice_Integration> Get_ReturnInvoice_Integration(int ReturnID)
		{
			string query = @" 
							SET sql_mode = '';
							select 
							 (prd.quantity * prd.unit_cost * (vatco.vat + 1)) as SubTotalVAT,
							 pi.id_purchase_invoice as InvoiceID,
							 prd.id_purchase_return_detail as ReturnDetailID
							 
							from purchase_return_detail as prd
 
							LEFT OUTER JOIN 
							(SELECT app_vat_group.id_vat_group, sum(app_vat.coefficient) as vat, sum(app_vat.coefficient) + 1 AS coef
								FROM app_vat_group 
									LEFT OUTER JOIN 
									app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group 
									LEFT OUTER JOIN 
									app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
								GROUP BY app_vat_group.id_vat_group) 
								vatco ON vatco.id_vat_group = prd.id_vat_group

							 left join purchase_invoice_detail as pid on prd.id_purchase_invoice_detail = pid.id_purchase_invoice_detail
							 left join purchase_invoice as pi on pid.id_purchase_invoice = pi.id_purchase_invoice
							 where prd.id_purchase_return = {0}
							 group by pi.id_purchase_invoice";

			query = string.Format(query, ReturnID);

			DataTable dt = QueryExecutor.DT(query);

			List<ReturnInvoice_Integration> ReturnList = new List<ReturnInvoice_Integration>();

			foreach (DataRow DataRow in dt.Rows)
			{
				ReturnInvoice_Integration Return = new ReturnInvoice_Integration();
				Return.ReturnDetailID = Convert.ToInt16(DataRow["ReturnDetailID"]);
				Return.InvoiceID = DataRow.IsNull("InvoiceID") == false ? Convert.ToInt16(DataRow["InvoiceID"]) : 0;
				Return.SubTotalVAT = Convert.ToDecimal(DataRow["SubTotalVAT"]);

				ReturnList.Add(Return);
			}

			return ReturnList;
		}
	}
}