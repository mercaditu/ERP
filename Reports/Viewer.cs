﻿using Microsoft.Reporting.WinForms;
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
     public  enum Forms
	{
	       Inventory,
         Sales
	}
        public string connectionstring { get; set; }
        public Forms Form { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Form==Viewer.Forms.Inventory)
            {
                QueryBuilderInventory();
            }
            else
            {
                QueryBuilderSales();
            }
         
        }
        private void QueryBuilderInventory()
        {
           



            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "InventoryCost"; // Name of the DataSet we set in .rdlc
            DataTable dt = exeDT("call INVENTORY");
            reportDataSource.Value = dt;
     
           // reportViewer1.LocalReport.ReportPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../../../")) + "Reports\\Inventory\\InventoryCost.rdlc"; // Path of the rdlc file
            reportViewer1.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Inventory\\InventoryCost.rdlc";
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.RefreshReport();
        }
        private void QueryBuilderSales()
        {



            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CostOfGoolsSold"; // Name of the DataSet we set in .rdlc
            String query = "SELECT " +
    " I.NAME AS ITEM, " +
    " I.CODE, " +
    " B.NAME AS BRANCH," +
    " SUM(SD.quantity) AS QUANTITY," +
    " SUM(IMV.UNIT_VALUE) AS COST," +
    " ROUND(SUM(SD.quantity) * SUM(IMV.UNIT_VALUE),4) AS SUBTOTAL" +
" FROM sales_invoice_detail AS SD" +
" INNER JOIN item_movement AS IM" +
" ON IM.ID_SALES_INVOICE_DETAIL = SD.ID_SALES_INVOICE_DETAIL" +
" INNER JOIN item_movement_value AS IMV" +
" ON IM.ID_MOVEMENT = IMV.ID_MOVEMENT" +
" LEFT JOIN ITEMS AS I" +
" ON I.ID_ITEM = SD.ID_ITEM" +
" LEFT JOIN APP_LOCATION AS L" +
" ON L.ID_LOCATION = IM.ID_LOCATION" +
" LEFT JOIN APP_BRANCH AS B" +
" ON B.ID_BRANCH = L.ID_BRANCH" +
" GROUP BY I.CODE,B.CODE" +
" ORDER BY I.NAME";
            DataTable dt = exeDT(query);
            reportDataSource.Value = dt;
           // reportViewer1.LocalReport.ReportPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../../../")) + "Reports\\Sales\\CostOfGoodsSold.rdlc"; // Path of the rdlc file
            reportViewer1.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\SAles\\CostOfGoodsSold.rdlc";
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
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
            catch
            {
                MessageBox.Show("Unable to Connect to Database. Please Check your credentials.");
            }
            return dt;
        }
    }
}