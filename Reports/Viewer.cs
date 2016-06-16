using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using entity;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;

namespace Reports
{
    public partial class Viewer : UserControl
    {
        public Viewer()
        {
            InitializeComponent();
        }
        public enum Reports
        {
            SalesByProduct,
            SalesByDate,
            SalesByTag,
            InventoryFIFO,
            CostOfGoodsSold


        }

        public string connectionstring { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public Reports Report { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region Finance

        #endregion

        #region Sales

        public void SalesByProduct()
        {
            String query = 
            "                select " +
            "	i.code as Code, i.name as Item, sum(sd.quantity) as Quantity," +
            "    round(sum(sd.quantity *sd.unit_price * vatco.coef),4) as Total," +
            "    round(sum(sd.quantity *sd.discount * vatco.coef),4) as Discount" +
            " from sales_invoice as s" +
            " inner join sales_invoice_detail as sd" +
            " on s.id_sales_invoice = sd.id_sales_invoice" +
            " left join items as i" +
            " on i.id_item = sd.id_item" +
            " LEFT JOIN" +
            "	(SELECT app_vat_group.id_vat_group,SUM(app_vat.coefficient) + 1 as coef" +
            "    FROM app_vat_group " +
            "	LEFT JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group" +
            "	LEFT JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat GROUP BY app_vat_group.id_vat_group) as vatco" +
            " ON vatco.id_vat_group = sd.id_vat_group";

            if (start_date != null || end_date != null)
            {
                query = query + " WHERE s.status = 2";
            }

            if (start_date != null)
            {
                query = query + " AND s.trans_date >= '" + start_date.ToString("yyyy-MM-dd") + "'";
            }

            if (end_date != null)
            {
                query = query + " AND s.trans_date <= '" + end_date.ToString("yyyy-MM-dd") + "'";
            }

            query = query + " GROUP BY i.id_item";

            DataTable dt = exeDT(query);

               string ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales\\SalesByProduct.rdlc";
          
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "SalesByProduct";
            reportDataSource.Value = dt;

            RunReport(ReportPath, reportDataSource);
        }

        public void SalesByDate()
        {
            String query =
                " select " +
    " s.trans_date, contact.name, s.number, sum(sd.quantity) as quantity," +
   "  round(sum(sd.quantity * sd.unit_price * vatco.coef),4) as total," +
    " (sum(discount)*-1) as discount" +
" from sales_invoice as s" +
" inner join contacts as contact" +
" on s.id_contact = contact.id_contact" +
" inner join sales_invoice_detail as sd" +
" on s.id_sales_invoice = sd.id_sales_invoice" +
" left join items as i" +
" on i.id_item = sd.id_item" +
" LEFT JOIN" +
" 	(SELECT app_vat_group.id_vat_group, (SUM(app_vat.coefficient) + 1) as coef" +
"     FROM app_vat_group " +
" 	LEFT JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group" +
" 	LEFT JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat GROUP BY app_vat_group.id_vat_group) as vatco" +
" ON vatco.id_vat_group = sd.id_vat_group";

            if (start_date != null || end_date != null)
            {
                query = query + " WHERE s.status = 2";
            }

            if (start_date != null)
            {
                query = query + " and s.trans_date >= '" + start_date.ToString("yyyy-MM-dd") + "'";
            }

            if (end_date != null)
            {
                query = query + " and s.trans_date <= '" + end_date.ToString("yyyy-MM-dd") + "'";
            }

            query = query + " GROUP BY s.id_sales_invoice";

            DataTable dt = exeDT(query);

            string ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales\\SalesByDate.rdlc";
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "SalesByDate";
            reportDataSource.Value = dt;

            RunReport(ReportPath, reportDataSource);
        }

        public void SalesByTag()
        {
            String query = 
                "select " +
	" it.name,sum(sd.quantity) as Quantity," +
    " sum(sd.quantity * sd.unit_price) as SubTotal," +
    " sum(sd.quantity * discount) as Discount" +
" from sales_invoice as s" +
" inner join sales_invoice_detail as sd" +
" on s.id_sales_invoice = sd.id_sales_invoice" +
" left join items as i" +
" on i.id_item = sd.id_item" +
" left join item_tag_detail as itd" +
" on itd.id_item = i.id_item" +
" left join item_tag as it" +
" on it.id_tag = itd.id_tag";

            if (start_date != null || end_date != null)
            {
                query = query + " WHERE s.status = 2";
            }

            if (start_date != null)
            {
                query = query + " and s.trans_date >= '" + start_date.ToString("yyyy-MM-dd") + "'";
            }

            if (end_date != null)
            {
                query = query + " and s.trans_date <= '" + end_date.ToString("yyyy-MM-dd") + "'";
            }

            query = query + " GROUP BY it.id_tag";

            DataTable dt = exeDT(query);

            string ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales\\SalesByTag.rdlc";
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = dt;

            RunReport(ReportPath, reportDataSource);
        }

        public void PendingDocuments_Sales()
        {
            String query = "SELECT" +
	                        " c.code, " +
                            " c.name," +
                            " c.alias," +
                            " c.address," +
                            " c.telephone," +
                            " ps.trans_date as fecha," +
                            " if(ps.id_sales_invoice is not null,'Venta'," +
	                        " 	if(ps.id_sales_return is not null,'Nota de credito','Nothing')) as detalle," +
                            " s.number, " +
	                        " case when ps.id_sales_invoice is not null then ps.debit " +
                            " when ps.id_sales_return is not null then ps.credit" +
                            " END as monto," +
                            " ps.debit - ifnull((SELECT sum(ifnull(credit,0)) as credit " +
                            " FROM payment_schedual as ps2" +
                            " WHERE ps2.id_sales_invoice = s.id_sales_invoice " +
                            " and ps2.parent_id_payment_schedual = ps.id_payment_schedual),0) as pendiente" +
                            " FROM sales_invoice as s  " +
                            " INNER JOIN payment_schedual as ps" +
                            " ON ps.id_sales_invoice = s.id_sales_invoice and ps.debit != 0" +
                            " LEFT JOIN sales_return as sr" +
                            " ON sr.id_sales_return = ps.id_sales_return" +
                            " LEFT JOIN contacts as c" +
                            " ON c.id_contact = s.id_contact" +
                            " WHERE s.status = 2" +
                            " having pendiente != 0" +
                            " order by c.name, ps.trans_date,s.number";

            if (start_date != null || end_date != null)
            {
                query = query + " WHERE ";
            }

            if (start_date != null)
            {
                query = query + " SI.trans_date >= '" + start_date.ToString("yyyy-MM-dd") + "'";
            }

            if (end_date != null)
            {
                if (start_date != null)
                {
                    query = query + " AND";
                }

                query = query + " SI.trans_date <= '" + end_date.ToString("yyyy-MM-dd") + "'";
            }

            query = query + " GROUP BY I.CODE,B.CODE ORDER BY I.NAME";

            DataTable dt = exeDT(query);

            string ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales\\CostOfGoodsSold.rdlc";
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CostOfGoodsSold";
            reportDataSource.Value = dt;

            RunReport(ReportPath, reportDataSource);
        }

        public void CostOfGoodsSold()
        {
            String query = "SELECT " +
                            " I.NAME AS ITEM, " +
                            " I.CODE, " +
                            " B.NAME AS BRANCH," +
                            " SUM(SD.quantity) AS QUANTITY," +
                            " IMV.UNIT_VALUE AS COST," +
                            " (SUM(SD.quantity) * IMV.UNIT_VALUE) AS SUBTOTAL " +
                                " FROM sales_invoice_detail AS SD" +
                                " INNER JOIN item_movement AS IM" +
                                " ON IM.ID_SALES_INVOICE_DETAIL = SD.ID_SALES_INVOICE_DETAIL" +
                                " inner join sales_invoice as SI on SD.id_sales_invoice = SI.id_sales_invoice" +
                                " INNER JOIN item_movement_value AS IMV" +
                                " ON IM.ID_MOVEMENT = IMV.ID_MOVEMENT" +
                                " LEFT JOIN ITEMS AS I" +
                                " ON I.ID_ITEM = SD.ID_ITEM" +
                                " LEFT JOIN APP_LOCATION AS L" +
                                " ON L.ID_LOCATION = IM.ID_LOCATION" +
                                " LEFT JOIN APP_BRANCH AS B" +
                                " ON B.ID_BRANCH = L.ID_BRANCH";

            if (start_date != null || end_date != null)
            {
                query = query + " WHERE ";
            }

            if (start_date != null)
            {
                query = query + " SI.trans_date >= '" + start_date.ToString("yyyy-MM-dd") + "'";
            }

            if (end_date != null)
            {
                if (start_date != null)
                {
                    query = query + " AND";
                }

                query = query + " SI.trans_date <= '" + end_date.ToString("yyyy-MM-dd") + "'";
            }

            query = query + " GROUP BY I.CODE,B.CODE ORDER BY I.NAME";

            DataTable dt = exeDT(query);

            string ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales\\CostOfGoodsSold.rdlc";
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CostOfGoodsSold";
            reportDataSource.Value = dt;

            RunReport(ReportPath, reportDataSource);
        }

        public void PendingDocuments()
        {

        }

        #endregion

        #region Inventory

        public void InventoryFIFO()
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "InventoryCost"; // Name of the DataSet we set in .rdlc
            DataTable dt = exeDT("call INVENTORY('" + end_date.ToString("yyyy-MM-dd") + "')");
            reportDataSource.Value = dt;
     
            string ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Inventory\\InventoryCost.rdlc";

            RunReport(ReportPath, reportDataSource);
        }

        #endregion

        private void RunReport(string ReportPath, ReportDataSource reportDataSource)
        {
            reportViewer1.Reset();
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.ReportPath = ReportPath;
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.RefreshReport();
        }

        public DataTable exeDT(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(connectionstring);
                sqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, sqlConn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to Database. Please Check your credentials: " + ex.Message);
            }
            return dt;
        }
    }
}