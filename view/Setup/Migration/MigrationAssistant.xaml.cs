using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cognitivo.Setup.Migration
{
    public partial class MigrationAssistant : Page
    {
        public entity.db dbContext { get; set; }

        public bool SalesInvoice;
        public bool SalesReturn;
        public bool PurchaseInvoice;

        public DateTime StartDate;

        public int id_company { get; set; }
        public int id_user { get; set; }
        public int id_branch { get; set; }
        public int id_terminal { get; set; }

        public bool _cogent_State { get; set; }
        public string _connString { get; set; }
        public string _DataBase { get; set; }
        public bool _IsIndeterminate { get; set; }
        public int _customer_Current { get; set; }
        public int _customer_Max { get; set; }
        public int _supplier_Current { get; set; }
        public int _supplier_Max { get; set; }
        public int _product_Current { get; set; }
        public int _product_Max { get; set; }
        public int _sales_Current { get; set; }
        public int _sales_Max { get; set; }
        public int _salesReturn_Current { get; set; }
        public int _salesReturn_Max { get; set; }
        public int _purchase_Current { get; set; }
        public int _purchase_Max { get; set; }

        public MigrationAssistant()
        {
            InitializeComponent();
            dbContext = new db();
            id_company = CurrentSession.Id_Company;

            //Sets the DatePicker to the first day of current year.
            int year = DateTime.Now.Year;
            dtpStartDate.SelectedDate = new DateTime(year, 1, 1);

            if (CurrentSession.Id_User == 0)
            {
                if (dbContext.security_user.Where(i => i.id_company == id_company).FirstOrDefault() != null)
                {
                    id_user = dbContext.security_user.Where(i => i.id_company == id_company).FirstOrDefault().id_user;
                    CurrentSession.Id_User = id_user;
                }
            }

            _cogent_State = false;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_cogent_State == false)
            {
                popConnBuilder.IsOpen = true;
                return;
            }

            SalesInvoice = (bool)chbxSalesInvoice.IsChecked;
            SalesReturn = (bool)chbxSalesReturns.IsChecked;
            PurchaseInvoice = (bool)chbxPurchaseInvoice.IsChecked;

            StartDate = (DateTime)dtpStartDate.SelectedDate;

            Task basic_Task = Task.Factory.StartNew(() => start());
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = tbxAddress.Text;
            builder.UserID = tbxUser.Text;
            builder.Password = tbxPassword.Password;
            builder.IntegratedSecurity = true;
            _connString = builder.ToString();

            string sql = " SELECT [name] "
                        + " FROM master.dbo.sysdatabases "
                        + " WHERE dbid > 4 ";
            DataTable dt = exeDT(sql);
            cbxDataBaseList.ItemsSource = dt.DefaultView;
        }

        private void TestConn_Click(object sender, RoutedEventArgs e)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = tbxAddress.Text;
            builder.IntegratedSecurity = true;
            //builder.UserID = tbxUser.Text;
            //builder.Password = tbxPassword.Password;
            builder.InitialCatalog = cbxDataBaseList.Text;
            _connString = builder.ToString();
            _DataBase = cbxDataBaseList.Text;
            DataTable dt = exeDT("SELECT * FROM EMPRESA");
            if (dt.Rows.Count > 0)
            {
                _cogent_State = true;
                popConnBuilder.IsOpen = false;
            }
            else
            {
                _cogent_State = false;
                MessageBox.Show("Failure, please check your credentials or that your database is a RIA");
                popConnBuilder.IsOpen = true;
            }
        }

        public DataTable exeDT(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection sqlConn = new SqlConnection(_connString);
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                cmd.CommandTimeout = 200;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            popConnBuilder.IsOpen = true;
        }

        private void btnClear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dbContext db = new dbContext();
            //string sql = " select ' truncate table ' + table_name "
            //           + " from information_schema.tables where TAble_schema='"+ "astilleronew" +"'";

            db.Truncates();
        }
    }

    public static class DbContextExtension
    {
        public static int Truncates(this dbContext db, params string[] tables)
        {
            List<string> target = new List<string>();
            int result = 0;

            if (tables == null || tables.Length == 0)
            {
                target = db.GetTableList();
            }
            else
            {
                target.AddRange(tables);
            }

            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var table in target)
                {
                    db.db.Database.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS=0");
                    result += db.db.Database.ExecuteSqlCommand(string.Format("DELETE FROM  {0}", table));
                    db.db.Database.ExecuteSqlCommand(string.Format("ALTER TABLE {0} AUTO_INCREMENT = 1", table));
                    db.db.Database.ExecuteSqlCommand("SET FOREIGN_KEY_CHECKS=1");
                }

                scope.Complete();
            }

            return result;
        }

        public static List<string> GetTableList(this dbContext db)
        {
            var type = db.db.GetType();

            return type.GetProperties()
                .Where(x => x.PropertyType.Name == "DbSet`1")
                .Select(x => x.Name).ToList();
        }
    }
}