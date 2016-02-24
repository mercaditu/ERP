using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using entity;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
namespace Cognitivo.Setup.Migration.Cogent
{
    /// <summary>
    /// Interaction logic for MysqlMigrationAssitant.xaml
    /// </summary>
    public partial class MigrationGUI : Page
    {
        public string _connString { get; set; }
        public bool _cogent_State { get; set; }
        public int _project_Current { get; set; }
        public int _project_Max { get; set; }
        public int _Exec_Current { get; set; }
        public int _Exec_Max { get; set; }

        public int _item_Current { get; set; }
        public int _item_Max { get; set; }
        public int _contact_Current { get; set; }
        public int _contact_Max { get; set; }
        public int _emp_Current { get; set; }
        public int _emp_Max { get; set; }

        public MigrationGUI()
        {
            InitializeComponent();
        }
        public DataTable exeDTMysql(string sql)
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
            catch
            {
                MessageBox.Show("Unable to Connect to Database. Please Check your credentials.");
            }
            return dt;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = tbxAddress.Text;
            builder.UserID = tbxUser.Text;
            builder.Password = tbxPassword.Password;
            builder.ConnectionTimeout = 1500;
            _connString = builder.ToString();

            string sql = " SHOW DATABASES; ";

            DataTable dt = exeDTMysql(sql);
            cbxDataBaseList.ItemsSource = dt.DefaultView;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        { //Check Connection.
            if (_cogent_State == false)
            {
                popConnBuilder.IsOpen = true;
                return;
            }
            Task basic_Task = Task.Factory.StartNew(() => start());

        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            popConnBuilder.IsOpen = true;

        }

        private void TestConn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = tbxAddress.Text;
            builder.UserID = tbxUser.Text;
            builder.Password = tbxPassword.Password;
            builder.Database = cbxDataBaseList.Text;
            _connString = builder.ToString();

            DataTable dt = exeDTMysql("SELECT * FROM item_sku");
            if (dt.Rows.Count >= 0)
            {
                _cogent_State = true;
                popConnBuilder.IsOpen = false;
            }
            else
            {
                _cogent_State = false;
                MessageBox.Show("Failure, please check your credentials or that your database is a Cogent");
                popConnBuilder.IsOpen = true;
            }
        }

        public void start()
        {
            Task basics = Task.Factory.StartNew(() => startTask());
            basics.Wait();
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
