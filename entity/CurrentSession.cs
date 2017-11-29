using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace entity
{
    public static class CurrentSession
    {

        #region PropertyChanged

        // INotifyPropertyChanged implementation
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        #endregion PropertyChanged

        #region Properties

        public static bool ConnectionLost
        {
            get
            {
                return _ConnLost;
            }
            set
            {
                _ConnLost = value;
                Properties.Settings.Default.ConnectionLost = _ConnLost;
                Properties.Settings.Default.Save();
            }
        }
        private static bool _ConnLost;

        public enum Versions
        {
            Lite,
            Basic,
            Medium,
            Full
        }

        public enum VersionsKey
        {
            Himayuddin_51, //Lite
            Bathua_102,    //Basic
            Mankurad_153,  //Medium
            Alphonso_255,  //Full
            Gulabkhas_306, //PrintingPress
            Chausa_357     //EventManagement
        }

        public static int Id_Company
        {
            get
            {
                if (_Id_Company == 0)
                {
                    _Id_Company = Properties.Settings.Default.company_ID;
                }
                return _Id_Company;
            }
            set
            {
                if (_Id_Company != value)
                {
                    _Id_Company = value;

                    using (db db = new db())
                    {
                        app_company app_company = db.app_company.Where(x => x.id_company == value).FirstOrDefault();
                        if (app_company != null)
                        {
                            Properties.Settings.Default.company_ID = app_company.id_company;
                            Properties.Settings.Default.company_Name = app_company.name;
                            Properties.Settings.Default.Save();
                        }
                    }
                }
            }
        }

        private static int _Id_Company;

        public static int Id_User { get; set; }

        public static int Id_Branch
        {
            get
            {
                if (_Id_Branch == 0)
                {
                    _Id_Branch = Properties.Settings.Default.branch_ID;
                }
                return _Id_Branch;
            }
            set
            {
                if (_Id_Branch != value)
                {
                    _Id_Branch = value;
                    using (db db = new db())
                    {
                        app_branch app_branch = db.app_branch.Where(x => x.id_branch == value).FirstOrDefault();
                        if (app_branch != null)
                        {
                            Properties.Settings.Default.branch_ID = app_branch.id_branch;
                            Properties.Settings.Default.branch_Name = app_branch.name;
                            Properties.Settings.Default.Save();
                        }
                    }
                }
            }
        }

        private static int _Id_Branch;

        public static int Id_Terminal
        {
            get
            {
                if (_Id_Terminal == 0)
                {
                    _Id_Terminal = Properties.Settings.Default.terminal_ID;
                };
                return _Id_Terminal;
            }
            set
            {
                _Id_Terminal = value;

                using (db db = new db())
                {
                    app_terminal app_terminal = db.app_terminal.Where(x => x.id_terminal == value).FirstOrDefault();
                    if (app_terminal != null)
                    {
                        Properties.Settings.Default.terminal_ID = app_terminal.id_terminal;
                        Properties.Settings.Default.terminal_Name = app_terminal.name;
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }

        private static int _Id_Terminal;

        public static int Id_Account
        {
            get
            {
                if (_Id_Account == 0)
                {
                    _Id_Account = Properties.Settings.Default.account_ID;
                };
                return _Id_Account;
            }
            set
            {
                _Id_Account = value;
                using (db db = new db())
                {
                    app_account app_account = db.app_account.Where(x => x.id_account == value).FirstOrDefault();
                    if (app_account != null)
                    {
                        Properties.Settings.Default.account_ID = app_account.id_account;
                        Properties.Settings.Default.account_Name = app_account.name;
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }

        private static int _Id_Account;

        public static string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set { _ConnectionString = value; }
        }

        private static string _ConnectionString;

        public static Versions Version { get; set; }

        public static List<security_crud> Security_CurdList { get; set; }
        public static List<app_field> AppField { get; set; }
        public static List<security_role_privilage> Security_role_privilageList { get; set; }

        public static security_user User { get; set; }
        public static security_role UserRole { get; set; }

        #endregion Properties

        public static void Start(security_user Sec_User, security_role Role)
        {
            UserRole = Role;

            Version = Versions.Full;




            Security_CurdList = new List<security_crud>();
            Security_role_privilageList = new List<security_role_privilage>();

            if (Sec_User != null)
            {
                User = Sec_User;
                Id_User = User.id_user;
                UserRole = Role;

                if (Id_Branch == 0)
                {
                    using (db db = new db())
                    {
                        Id_Branch = db.app_branch.Where(x => x.id_company == _Id_Company && x.is_active).Select(y => y.id_branch).FirstOrDefault();
                    }

                    Properties.Settings.Default.branch_ID = Id_Branch;
                    Properties.Settings.Default.Save();
                }

                Properties.Settings.Default.user_Name = User.name_full;
                Properties.Settings.Default.Save();

                //Setting Security, once CurrentSession Data is set.
                Load_Security();

                //Basic Data like Salesman, Contracts, VAT, Currencies, etc to speed up Window Load.
                Load_BasicData(null, null);
                //Load Basic Data into Timer.
                Timer myTimer = new Timer();
                myTimer.Elapsed += new ElapsedEventHandler(Load_BasicData);
                myTimer.Interval = 300000;
                myTimer.Start();
            }
        }

        public static void Load_Security()
        {
            Task taskAuth = Task.Factory.StartNew(() => Thread_Security());
        }

        private static void Thread_Security()
        {
            Security_CurdList.Clear();
            Security_role_privilageList.Clear();

            using (db cntx = new db())
            {
                cntx.Configuration.LazyLoadingEnabled = false;
                cntx.Configuration.AutoDetectChangesEnabled = false;

                //Curd
                Security_CurdList = cntx.security_curd.Where(x => x.id_role == User.id_role).ToList();

                //Privilage
                Security_role_privilageList = cntx.security_role_privilage.Where(x => x.id_role == User.id_role).Include(x => x.security_privilage).ToList();
                Security_role_privilageList = Security_role_privilageList.Where(x => x.security_privilage != null).ToList();
                try
                {
                    Allow_UpdateSalesDetail = !(Security_role_privilageList
                        .Where(x => x.security_privilage.name == Privilage.Privilages.CanUserNotUpdatePrice &&
                        x.has_privilage).FirstOrDefault() != null ? true : false);

                    Allow_BarCodeSearchOnly = Security_role_privilageList
                        .Where(x => x.security_privilage.name == Privilage.Privilages.ItemBarcodeSearchOnly &&
                        x.has_privilage).FirstOrDefault() != null ? true : false;

                    Include_OutOfStock = Security_role_privilageList
                        .Where(x => x.security_privilage.name == Privilage.Privilages.Include_OutOfStock &&
                        x.has_privilage).FirstOrDefault() != null ? true : false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void Load_BasicData(object sender, ElapsedEventArgs e)
        {
            Task taskAuth = Task.Factory.StartNew(() => Thread_Data());
        }

        private static bool IsLoaded = false;

        private static void Thread_Data()
        {
            using (db db = new db())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                //Default Currency
                Currencies = db.app_currency.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();
                Currency_Default = Currencies.Where(x => x.is_priority && x.id_company == Id_Company).FirstOrDefault();
                CurrencyFX_ActiveRates = db.app_currencyfx.Where(x => x.id_company == Id_Company && x.is_active).ToList();

                SalesReps = db.sales_rep.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();
                Contracts = db.app_contract.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();
                Conditions = db.app_condition.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();
                PriceLists = db.item_price_list.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();

                Branches = db.app_branch.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();
                Locations = db.app_location.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();
                Terminals = db.app_terminal.Where(x => x.id_company == Id_Company && x.is_active).OrderBy(x => x.name).ToList();
                AppField = db.app_field.Where(x => x.id_company == Id_Company).ToList();

                var app_notifications = db.app_notification.Where(x => x.is_read == false && x.id_company == Id_Company &&
                    ((x.notified_user.id_user == CurrentSession.Id_User && x.notified_department == null) || x.notified_department.id_department == UserRole.id_department)).ToList();

                NotificationCounts = new List<NotificationCount>();

                foreach (var app_notification in app_notifications)
                {
                    NotificationCount notificationCount = new NotificationCount
                    {
                        Count = app_notifications.Where(x => x.id_application == app_notification.id_application).Count(),
                        Name = app_notification.id_application
                    };

                    NotificationCounts.Add(notificationCount);
                }

                if (IsLoaded == false)
                {
                    VAT_Groups = db.app_vat_group.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                    VAT_GroupDetails = db.app_vat_group_details.Include("app_vat").Where(x => x.id_company == Id_Company).ToList();
                    VATs = db.app_vat.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                }
            }
        }

        public static string GetBranchCode(int id_branch)
        {
            if (Branches.Count() > 0)
            {
                app_branch app_branch = Branches.Where(x => x.id_branch == id_branch).FirstOrDefault();
                if (app_branch != null)
                {
                    return app_branch.code;
                }

            }
            return "";
        }
        public static string GetTerminalCode(int id_terminal)
        {
            if (Terminals.Count() > 0)
            {
                app_terminal app_terminal = Terminals.Where(x => x.id_terminal == id_terminal).FirstOrDefault();
                if (app_terminal != null)
                {
                    return app_terminal.code;
                }

            }
            return "";
        }

        public static T FindParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDepObj = child;
            do
            {
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
                T parent = parentDepObj as T;
                if (parent != null) return parent;
            }
            while (parentDepObj != null);
            return null;
        }

        public static List<sales_rep> SalesReps { get; set; }
        public static List<app_contract> Contracts { get; set; }
        public static List<app_condition> Conditions { get; set; }
        public static List<app_vat_group> VAT_Groups { get; set; }
        public static List<app_vat_group_details> VAT_GroupDetails { get; set; }
        public static List<app_vat> VATs { get; set; }
        public static List<app_branch> Branches { get; set; }
        public static List<app_location> Locations { get; set; }
        public static List<app_terminal> Terminals { get; set; }
        public static List<app_currency> Currencies { get; set; }
        public static List<item_price_list> PriceLists { get; set; }

        public static List<NotificationCount> NotificationCounts { get; set; }

        public static app_currency Currency_Default { get; set; }
        public static List<app_currencyfx> CurrencyFX_ActiveRates { get; set; }

        public static bool Allow_UpdateSalesDetail { get; set; }
        public static bool Allow_BarCodeSearchOnly { get; set; }
        public static bool Include_OutOfStock { get; set; }

        public static string ApplicationFile_Path
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\";
            }
        }

        public static MySql.Data.MySqlClient.MySqlConnectionStringBuilder MySQLConnString { get; set; }

        public static app_currencyfx Get_Currency_Default_Rate()
        {
            return CurrencyFX_ActiveRates.Where(x => x.id_currency == Currency_Default.id_currency).FirstOrDefault();
        }
    }

    public class NotificationCount
    {
        public App.Names Name { get; set; }
        public int Count { get; set; }
    }
}