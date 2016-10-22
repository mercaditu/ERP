using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public static class CurrentSession
    {
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
            Alphonso_255, //Full
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
                _Id_Company = value;
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
            set { _Id_Branch = value; }
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
            set { _Id_Terminal = value; }
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
            set { _Id_Account = value; }
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
        public static int NumberOfSeats { get; set; }

        public static List<security_crud> Security_CurdList { get; set; }
        public static List<security_role_privilage> Security_role_privilageList { get; set; }

        public static security_user User { get; set; }
        public static security_role UserRole { get; set; }

        public static void Start(security_user Sec_User, security_role Role)
        {
            Security_CurdList = new List<security_crud>();
            Security_role_privilageList = new List<security_role_privilage>();

            if (Sec_User != null)
            {
                User = Sec_User;
                Id_User = User.id_user;
                UserRole = Role;

                Properties.Settings.Default.user_Name = User.name_full;
                Properties.Settings.Default.Save();

                //Setting Security, once CurrentSession Data is set.
                Load_Security();

                //Basic Data like Salesman, Contracts, VAT, Currencies, etc to speed up Window Load.
                Load_BasicData();

                Brillo.Activation Activation = new Brillo.Activation();
                Version = Activation.VersionDecrypt();
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

        public static void Load_BasicData()
        {
            Task taskAuth = Task.Factory.StartNew(() => Thread_Data());
        }

        private static void Thread_Data()
        {
            using (db cntx = new db())
            {
                cntx.Configuration.LazyLoadingEnabled = false;
                cntx.Configuration.AutoDetectChangesEnabled = false;

                //Default Currency
                Currency_Default = cntx.app_currency.Where(x => x.is_priority && x.id_company == Id_Company).FirstOrDefault();
                if (Currency_Default != null)
                {
                    CurrencyFX_Default = Currency_Default.app_currencyfx.Where(x => x.is_active).FirstOrDefault();
                }

                SalesRep = cntx.sales_rep.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Contract = cntx.app_contract.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Condition = cntx.app_condition.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                VAT_Group = cntx.app_vat_group.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                VAT_GroupDetail = cntx.app_vat_group_details.Where(x => x.id_company == Id_Company).ToList();
                VAT = cntx.app_vat.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Branch = cntx.app_branch.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Terminal = cntx.app_terminal.Where(x => x.id_company == Id_Company && x.is_active).ToList();
                Currency = cntx.app_currency.Where(x => x.id_company == Id_Company && x.is_active).ToList();

            }
        }

        private static List<sales_rep> SalesRep { get; set; }
        private static List<app_contract> Contract { get; set; }
        private static List<app_condition> Condition { get; set; }
        private static List<app_vat_group> VAT_Group { get; set; }
        private static List<app_vat_group_details> VAT_GroupDetail { get; set; }
        private static List<app_vat> VAT { get; set; }
        private static List<app_branch> Branch { get; set; }
        private static List<app_terminal> Terminal { get; set; }
        private static List<app_currency> Currency { get; set; }
        private static List<item_price_list> PriceList { get; set; }

        public static app_currency Currency_Default { get; set; }
        public static app_currencyfx CurrencyFX_Default { get; set; }


        public static List<sales_rep> Get_SalesRep()
        {
            return SalesRep;
        }

        public static List<app_contract> Get_Contract()
        {
            return Contract;
        }

        public static List<app_condition> Get_Condition()
        {
            return Condition;
        }

        public static List<app_vat_group> Get_VAT_Group()
        {
            return VAT_Group;
        }

        public static List<app_vat_group_details> Get_VAT_GroupDetails()
        {
            return VAT_GroupDetail;
        }

        public static List<app_vat> Get_VAT()
        {
            return VAT;
        }

        public static List<app_branch> Get_Branch()
        {
            return Branch;
        }

        public static List<app_terminal> Get_Terminal()
        {
            return Terminal;
        }

        public static List<app_currency> Get_Currency()
        {
            return Currency;
        }
    }
}
