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
    public partial class Viewer : Form
    {
        public Viewer()
        {
            InitializeComponent();
        }
        public string connectionstring { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {
         //   QueryBuilder();
        }
        private void QueryBuilderInventory()
        {



            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "InventoryCost"; // Name of the DataSet we set in .rdlc
            DataTable dt = exeDT("call INVENTORY");
            reportDataSource.Value = dt;
            reportViewer1.LocalReport.ReportPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../../")) +"Inventory\\InventoryCost.rdlc"; // Path of the rdlc file
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.RefreshReport();
        }
        private void QueryBuilderSales()
        {



            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "CostOfGoolsSold"; // Name of the DataSet we set in .rdlc
            DataTable dt = exeDT("call SALES");
            reportDataSource.Value = dt;
            reportViewer1.LocalReport.ReportPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../../")) + "Sales\\CostOfGoodsSold.rdlc"; // Path of the rdlc file
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.RefreshReport();
        }
        public DataTable exeDT(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(Cognitivo.Properties.Settings.Default.MySQLconnString);
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