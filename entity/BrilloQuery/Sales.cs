using System;
using System.Collections.Generic;
using System.Data;

namespace entity.BrilloQuery
{
    public class ReturnInvoice_Integration
    {
        public int ReturnDetailID { get; set; }
        public int InvoiceID { get; set; }
        public decimal SubTotalVAT { get; set; }
    }

    public class Sales
    {
        public List<ReturnInvoice_Integration> Get_ReturnInvoice_Integration(int ReturnID)
        {
            string query = @"
						    SET sql_mode = '';
							select
							(srd.quantity * srd.unit_price * (vatco.vat + 1)) as SubTotalVAT,
							si.id_sales_invoice as InvoiceID,
							srd.id_sales_return_detail as ReturnDetailID

							from sales_return_detail as srd

							LEFT OUTER JOIN
							(SELECT app_vat_group.id_vat_group, sum(app_vat.coefficient) as vat, sum(app_vat.coefficient) + 1 AS coef
							    FROM app_vat_group
									LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
									LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
								GROUP BY app_vat_group.id_vat_group)
								vatco ON vatco.id_vat_group = srd.id_vat_group

							  left join sales_invoice_detail as sid on srd.sales_invoice_detail_id_sales_invoice_detail = sid.id_sales_invoice_detail
							  left join sales_invoice as si on sid.id_sales_invoice = si.id_sales_invoice
							  where srd.id_sales_return = {0}
							  group by si.id_sales_invoice";

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