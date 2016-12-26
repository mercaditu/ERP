using Cognitivo.Configs;
using entity;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Cognitivo.Menu
{
    public partial class StartUp : Page
    {

        bool _one;
        public bool one_Active
        {
            get { return _one; }
            set { _one = value; }
        }

        bool _two;
        public bool two_Active
        {
            get { return _two; }
            set { _two = value; }
        }

        bool _three;
        public bool three_Active
        {
            get { return _three; }
            set { _three = value; }
        }

        public StartUp()
        {
            InitializeComponent();

            //Check connectionString on Thread
            Task task_connString = Task.Factory.StartNew(() => connString_Check(null, null));
            //Check connectionString every 10 Seconds
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(connString_Check);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
            dispatcherTimer.Start();

            entity.Properties.Settings.Default.user_Name = "";
            entity.Properties.Settings.Default.company_Name = "";
            entity.Properties.Settings.Default.Save();

            Task task_dataBase = Task.Factory.StartNew(() => check_Database());
        }

        private void connString_Check(object sender, EventArgs e)
        {
            try
            {
                string hostname = (string)SQLQuery_ReturnScalar("SELECT @@hostname;", true);
                string version_compile_os = (string)SQLQuery_ReturnScalar("SELECT @@version_compile_os;", true);
                string version_comment = (string)SQLQuery_ReturnScalar("SELECT @@version_comment;", true);

                Dispatcher.BeginInvoke((Action)(() => { lbl_serverAddress.Content = hostname; }));
                Dispatcher.BeginInvoke((Action)(() => { lbl_serverOS.Content = version_compile_os; }));
                Dispatcher.BeginInvoke((Action)(() => { lbl_database_Version.Content = version_comment; }));

                MySqlConnectionStringBuilder connString = new MySqlConnectionStringBuilder(Properties.Settings.Default.MySQLconnString);
                Dispatcher.BeginInvoke((Action)(() => { lbl_database_Name.Content = connString.Database; }));

                string str_update = (string)SQLQuery_ReturnScalar(
                    " SELECT VARIABLE_VALUE" +
                    " FROM INFORMATION_SCHEMA.GLOBAL_STATUS " +
                    " WHERE VARIABLE_NAME LIKE 'Uptime'", true);

                int int_update = int.Parse(str_update);
                int_update = int_update * -1;
                DateTime uptime_date = DateTime.Now.AddSeconds((int)int_update);
                Dispatcher.BeginInvoke((Action)(() => { lbl_uptime.Content = uptime_date.ToLongDateString(); }));

                _one = true;
            }
            catch
            {
                _one = false;
            }

        }

        private void check_Database()
        {
            try
            {
                //MySqlConnectionStringBuilder connString = new MySqlConnectionStringBuilder(Cognitivo.Properties.Settings.Default.MySQLconnString);
                //Dispatcher.BeginInvoke((Action)(() => { SQLQuery_ReturnScalar("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '" + connString.Database + "';", true); }));
                _two = false;
            }
            catch
            {
                _two = true;
            }
        }

        private void list_Companies()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)(() => { SQLQuery_ReturnScalar("SELECT * FROM app_company;", false); }));
                _three = true;
            }
            catch
            {
                _three = false;
            }
        }

        private void connStringBuilder_Click(object sender, RoutedEventArgs e)
        {
            frameConfig.Navigate(new ConnectionBuilder());
        }

        private void createDB_Click(object sender, EventArgs e)
        {
            frameConfig.Navigate(new Menu.WaitPage());
            Task task_createDB = Task.Factory.StartNew(() => createDB(null, null));
        }

        private void createDB(object sender, EventArgs e)
        {
            using (db db = new db())
            {
                bool db_exists = db.Database.Exists();
                if (!db_exists)
                {
                    try
                    {
                        db.SaveChanges();
                        db.Database.CreateIfNotExists();
                        // db.contacts.Count();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            Dispatcher.BeginInvoke((Action)(() => frameConfig.Content = null));
        }

        private void createCompany_Click(object sender, RoutedEventArgs e)
        {

            frameConfig.Navigate(new MainSetup());
        }

        private object SQLQuery_ReturnScalar(string strSQL, bool generic)
        {
            try
            {
                MySqlConnection SQLConn;
                if (generic)
                {
                    //Generic ConnectionString without Database
                    MySqlConnectionStringBuilder specific_connString = new MySqlConnectionStringBuilder(Cognitivo.Properties.Settings.Default.MySQLconnString);
                    MySqlConnectionStringBuilder generic_connString = new MySqlConnectionStringBuilder();
                    generic_connString.Server = specific_connString.Server;
                    generic_connString.UserID = specific_connString.UserID;
                    generic_connString.Password = specific_connString.Password;
                    generic_connString.ConnectionTimeout = 1;
                    SQLConn = new MySqlConnection(generic_connString.ToString());
                }
                else
                {
                    //Specific ConnectionString with Database
                    SQLConn = new MySqlConnection(Cognitivo.Properties.Settings.Default.MySQLconnString);
                }

                MySqlCommand SQLCmd = new MySqlCommand();
                object eSQL;

                SQLConn.Open();
                SQLCmd = new MySqlCommand(strSQL, SQLConn);
                eSQL = (string)SQLCmd.ExecuteScalar();
                SQLConn.Close();

                return eSQL;
            }
            catch
            {
                return null;
            }

        }

        private void migrate_Click(object sender, RoutedEventArgs e)
        {
            frameConfig.Navigate(new Setup.Migration.MigrationAssistant());
        }

        private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow myWindow = App.Current.MainWindow as MainWindow;
            Frame myFrame = myWindow.mainFrame;
            myFrame.Navigate(new mainLogIn());
        }



        private void migratesql_Click(object sender, RoutedEventArgs e)
        {
            frameConfig.Navigate(new Setup.Migration.Cogent.MigrationGUI());
        }

        private void btnGenerateParentChildRel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string PASsWORD = Microsoft.VisualBasic.Interaction.InputBox("Password", "Cognitivo");
            if (PASsWORD == "DOCOMO")
            {
                Task thread_SecondaryData = Task.Factory.StartNew(() => GenerateParentChildRel_Thread());
            }

        }

        private void GenerateParentChildRel_Thread()
        {
            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = true; }));

            ProductMovementDB ProductMovementDB = new ProductMovementDB();
            ProductMovementDB.Generate_ProductMovement();

            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = false; }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => { progBarunitcost.IsIndeterminate = true; }));


            UpdateUnitCost();

            Dispatcher.BeginInvoke((Action)(() => { progBarunitcost.IsIndeterminate = false; }));

        }
        public void UpdateUnitCost()
        {
            using (db db = new db())
            {
                List<item> items = db.items.ToList();
                foreach (item item in items)
                {
                    if (item.item_product.FirstOrDefault() != null)
                    {
                        item_product item_product = item.item_product.FirstOrDefault();

                        /// Check for movement that have credit and no parents (Purchase or Inventory). Also that has value in Item Movement Value.
                        item_movement item_movement = db.item_movement
                            .Where(x =>
                                x.id_item_product == item_product.id_item_product &&
                                x.credit > 0 &&
                                x.parent == null &&
                                x.item_movement_value.Sum(y => y.unit_value) > 0).OrderByDescending(x => x.trans_date).FirstOrDefault();

                        if (item_movement != null)
                        {
                            item.unit_cost = item_movement.item_movement_value.Sum(x => x.unit_value);
                        }
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
