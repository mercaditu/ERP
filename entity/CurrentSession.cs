using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;


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

        #endregion

        #region Properties

        public enum Versions
        {
            Lite,           //     0 USD //   0 USD
            Basic,          // 3,000 USD // 350 USD
            Medium,         // 5,000 USD // 550 USD
            Full,           // 8,000 USD // 750 USD
            PrintingPress,
            EventManagement
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
        static int _Id_Company;

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
        static int _Id_Branch;

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
        static int _Id_Terminal;

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
        static int _Id_Account;

        public static string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set { _ConnectionString = value; }
        }
        static string _ConnectionString;

        public static Versions Version { get; set; }

        public static List<security_crud> Security_CurdList { get; set; }
        public static List<app_field> AppField { get; set; }
        public static List<security_role_privilage> Security_role_privilageList { get; set; }

        public static security_user User { get; set; }
        public static security_role UserRole { get; set; }

        public static bool IsDataLoading
        {
            get { return _IsDataLoading; }
            set { _IsDataLoading = value; NotifyStaticPropertyChanged("IsDataLoading"); }
        }
        private static bool _IsDataLoading = false;

        #endregion

        public static void Start(security_user Sec_User, security_role Role)
        {
            Security_CurdList = new List<security_crud>();
            Security_role_privilageList = new List<security_role_privilage>();

            if (Sec_User != null)
            {
                User = Sec_User;
                Id_User = User.id_user;
                UserRole = Role;
                Version = Role.Version;

                if (Id_Branch == 0)
                {
                    using (db db = new db())
                    {
                        Id_Branch = db.app_branch.Where(x => x.id_company == _Id_Company && x.is_active).FirstOrDefault().id_branch;
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
                myTimer.Interval = 60000;
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
                Security_role_privilageList = cntx.security_role_privilage.Where(x => x.id_role == User.id_role).ToList();
            }
        }

        public static void Load_BasicData(object sender, ElapsedEventArgs e)
        {
            Task taskAuth = Task.Factory.StartNew(() => Thread_Data());
            IsDataLoading = true;
        }

        static bool IsLoaded = false;

        private static void Thread_Data()
        {
            using (db db = new db())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                //Default Currency
                Currencies = db.app_currency.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Currency_Default = Currencies.Where(x => x.is_priority && x.id_company == Id_Company).FirstOrDefault();
                CurrencyFX_ActiveRates = db.app_currencyfx.Where(x => x.id_company == Id_Company && x.is_active).ToList();

                SalesReps = db.sales_rep.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Contracts = db.app_contract.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Conditions = db.app_condition.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                PriceLists = db.item_price_list.Where(x => x.id_company == Id_Company && x.is_active).ToList();

                Branches = db.app_branch.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Locations = db.app_location.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Terminals = db.app_terminal.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                AppField = db.app_field.Where(x => x.id_company == Id_Company).ToList();

                if (IsLoaded == false)
                {
                    VAT_Groups = db.app_vat_group.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                    VAT_GroupDetails = db.app_vat_group_details.Include("app_vat").Where(x => x.id_company == Id_Company).ToList();
                    VATs = db.app_vat.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                }

                IsDataLoading = false;
            }
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

        public static app_currency Currency_Default { get; set; }
        public static List<app_currencyfx> CurrencyFX_ActiveRates { get; set; }

        public static app_currencyfx Get_Currency_Default_Rate()
        {
            return CurrencyFX_ActiveRates.Where(x => x.id_currency == Currency_Default.id_currency).FirstOrDefault();
        }
    }
}
