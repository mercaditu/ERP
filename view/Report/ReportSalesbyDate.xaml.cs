using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.SqlClient;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Cognitivo.Report
{
    public partial class ReportSalesbyDate : Page
    {
        db db = new db();
        string _connString = string.Empty;

        public ReportSalesbyDate()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            db.app_location.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            cbxTerminal.ItemsSource = db.app_location.Local;

            Cognitivo.Properties.Settings Settings = new Properties.Settings();
            _connString = Settings.MySQLconnString;

            DataTable dt = exeDT(sql());
            dgvreport.ItemsSource = dt.DefaultView;
        }

        public DataTable exeDT(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(_connString);
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

        private string sql()
        {

            string sql = string.Empty;

            sql =  " select ";
            sql += " (select name from app_terminal where id_terminal=(select id_terminal from sales_invoice where sales_invoice.id_sales_invoice=sales_invoice_detail.id_sales_invoice)) as Terminal, ";
            sql += " (select number from sales_invoice where sales_invoice.id_sales_invoice=sales_invoice_detail.id_sales_invoice) as number, ";
            sql += " (select code from items where id_item=sales_invoice_detail.id_item) as code, ";
            sql += " (select name from items where id_item=sales_invoice_detail.id_item) as Description, ";
            sql += " sum(quantity) as qty,sum(unit_price) as price, ";
            sql += " (sum(item_movement.credit)-sum(item_movement.debit))as profit, ";
            sql += "  sum(discount) as discount";
            sql += " from sales_invoice_detail left outer join item_movement on item_movement.id_sales_invoice_detail=sales_invoice_detail.id_sales_invoice_detail ";



            if (dtpTrans_Date.SelectedDate != null)
            {
                sql += " where trans_date = '" + dtpTrans_Date.SelectedDate + "'";
            }
            else
            {
                sql += " where 1=1";
            }

            if (cbxTerminal.SelectedValue != null)
            {
                sql += " and (select id_terminal from sales_invoice where id_sales_invoice=sales_invoice_detail.id_sales_invoice) = " + cbxTerminal.SelectedValue;
            }

            sql += " group by trans_date";

            return sql;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = exeDT(sql());
            dgvreport.ItemsSource = dt.DefaultView;
            cbxTerminal.SelectedValue = null;
        }
    }
}
