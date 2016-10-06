using entity;
using System;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Reporting.Views
{
	/// <summary>
	/// Interaction logic for SalesByItem.xaml
	/// </summary>
	public partial class SalesByItem : Page
	{
		public SalesByItem()
		{
			InitializeComponent();

			Fill(null, null);
		}

		public void Fill(object sender, EventArgs e)
		{
			string sql = @"
							SELECT 
								branch.name as Branch,
								tag.name as TagName,
								item.code as ItemCode,
								item.name as ItemName,
								sd.quantity AS Quantity, 
								s.number as Invoice,s.trans_date as InvoiceDate,
								round((sd.unit_cost * vatco.coef),4) as Cost,
								round(sd.quantity * sd.unit_cost * vatco.coef,4) as TotalCost,
								round(sd.unit_price * vatco.coef,4) as Retail,
								round((sd.unit_price * vatco.coef) * sd.quantity ,4)as TotalRetail,
								round((sd.discount * vatco.coef) / ((sd.discount * vatco.coef) + (sd.unit_price * vatco.coef)),4) as Discountper,
								round(sd.unit_price * vatco.coef,4) - round(sd.unit_cost * vatco.coef,4) as Profit,
								(((sd.quantity * sd.unit_price * vatco.coef) - (sd.quantity * sd.unit_cost * vatco.coef)) / (sd.quantity * sd.unit_price * vatco.coef)) as Margin

							FROM  sales_invoice s INNER JOIN
									 app_branch as branch on s.id_branch = branch.id_branch
									 inner join 
									 sales_invoice_detail sd ON s.id_sales_invoice = sd.id_sales_invoice LEFT OUTER JOIN 
									 items as item ON item.id_item = sd.id_item 
									 left join item_tag_detail as td on item.id_item = td.id_item
									 left join item_tag as tag on td.id_tag = tag.id_tag
									 LEFT OUTER JOIN 
										 (SELECT app_vat_group.id_vat_group, sum(app_vat.coefficient) as vat, sum(app_vat.coefficient) + 1 AS coef
										FROM  app_vat_group LEFT OUTER JOIN 
												 app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group LEFT OUTER JOIN 
												 app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
										GROUP BY app_vat_group.id_vat_group) vatco ON vatco.id_vat_group = sd.id_vat_group
									   where {0} (s.status = 2) and (s.trans_date >= {1}) AND (s.trans_date <= {2})  and s.id_company= {3}  
							group by  item.id_item,s.id_sales_invoice 
							order by item.id_item";

			//fill data
			DataTable dt = new DataTable();

            string SqlWhere = "";

			if (ReportPanel.Branch != null)
			{
                SqlWhere = string.Format(SqlWhere, "s.id_branch = " + ReportPanel.Branch.id_branch);
            }

            if (ReportPanel.ProductID > 0)
            {
                SqlWhere = string.Format(SqlWhere, "sd.id_item = " + ReportPanel.ProductID);
            }

            sql = string.Format(sql, SqlWhere, ReportPanel.StartDate, ReportPanel.EndDate, CurrentSession.Id_Company);

            reportViewer.Reset();
			Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
			reportDataSource1.Name = "SalesByItem";
			reportDataSource1.Value = dt;
			reportViewer.LocalReport.DataSources.Add(reportDataSource1);
			reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.SalesByItem.rdlc";

			reportViewer.Refresh();
			reportViewer.RefreshReport();
		}
	}
}
