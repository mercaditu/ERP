using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public static class CurrentSession
    {

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
                };
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

        public static List<security_curd> Security_CurdList
        {
            get
            {
                return _Security_CurdList;
            }
            set { _Security_CurdList = value; }
        }
        static List<security_curd> _Security_CurdList = new List<security_curd>();

        public static List<security_role_privilage> Security_role_privilageList
        {
            get
            {

                return _Security_role_privilageList;
            }
            set { _Security_role_privilageList = value; }
        }
        static List<security_role_privilage> _Security_role_privilageList = new List<security_role_privilage>();

        public static db db = new db();

        public static security_user User { get; set; }

        public static void Start(string UserName, string Password)
        {
            Security_CurdList = new List<security_curd>();
            Security_role_privilageList = new List<security_role_privilage>();

            //Set the User
            User = db.security_user.Where(x => x.name == UserName
                                               && x.password == Password
                                               && x.id_company == Id_Company)
                                   .FirstOrDefault();

            if (User != null)
            {
                //Set the User
                Id_User = User.id_user;

                Properties.Settings.Default.user_Name = User.name_full;
                Properties.Settings.Default.Save();

                //Set the Company
                Id_Company = User.app_company.id_company;

                //Check if Branch Exists
                if (User.app_company.app_branch.Where(branch =>
                                branch.id_company == Id_Company &&
                                branch.id_branch == Properties.Settings.Default.branch_ID)
                                .FirstOrDefault() != null)
                {
                    Id_Branch = User.app_company.app_branch.Where(branch =>
                                branch.id_company == Id_Company &&
                                branch.id_branch == Properties.Settings.Default.branch_ID)
                                .FirstOrDefault().id_branch;
                }

                //Check if Terminal Exists inside Branch
                if (db.app_terminal.Where(terminal =>
                                terminal.id_branch == Id_Branch &&
                                terminal.id_terminal == Properties.Settings.Default.terminal_ID)
                                .FirstOrDefault() != null)
                {
                    Id_Terminal = db.app_terminal.Where(terminal =>
                                    terminal.id_branch == Id_Branch &&
                                    terminal.id_terminal == Properties.Settings.Default.terminal_ID)
                                    .FirstOrDefault().id_terminal;
                }


                //Setting Security, once CurrentSession Data is set.
                Refresh_Security();

                //Basic Data like Salesman, Contracts, VAT, Currencies, etc to speed up Window Load.
                Task taskAuth = Task.Factory.StartNew(() => Load_BasicData());
            }
        }

        public static void Refresh_Security()
        {
            _Security_CurdList.Clear();

                //Curd
                security_user security_user = db.security_user.Where(x => x.id_user == Id_User).FirstOrDefault();
                _Security_CurdList = db.security_curd.Where(x => x.id_role == security_user.id_role).ToList();

                //Privilage
                _Security_role_privilageList.Clear();
                _Security_role_privilageList = db.security_role_privilage.Where(x => x.id_role == security_user.id_role).ToList();
        }

        public static void Load_BasicData()
        {
            db.sales_rep.Where(x => x.id_company == Id_Company && x.is_active).ToList();
            db.app_contract.Where(x => x.id_company == Id_Company && x.is_active).ToList();
            db.app_condition.Where(x => x.id_company == Id_Company && x.is_active).ToList();
            db.app_vat_group.Where(x => x.id_company == Id_Company && x.is_active).ToList();
            db.app_branch.Where(x => x.id_company == Id_Company && x.is_active).ToList();
            db.app_terminal.Where(x => x.id_company == Id_Company && x.is_active).ToList();
            db.app_currency.Where(x => x.id_company == Id_Company && x.is_active).ToList();
        }

        public static List<sales_rep> Get_SalesRep()
        {
            return db.sales_rep.Local.OrderBy(x => x.name).ToList();
        }

        public static List<app_contract> Get_Contract()
        {
            return db.app_contract.Local.OrderBy(x => x.name).ToList();
        }

        public static List<app_condition> Get_Condition()
        {
            return db.app_condition.Local.OrderBy(x => x.name).ToList();
        }

        public static List<app_vat_group> Get_VAT_Group()
        {
            return db.app_vat_group.Local.OrderBy(x => x.name).ToList();
        }

        public static List<app_branch> Get_Branch()
        {
            return db.app_branch.Local.OrderBy(x => x.name).ToList();
        }

        public static List<app_terminal> Get_Terminal()
        {
            return db.app_terminal.Local.OrderBy(x => x.name).ToList();
        }

        public static List<app_currency> Get_Currency()
        {
            return db.app_currency.Local.OrderBy(x => x.name).ToList();
        }

        public static app_currency Get_DefaultCurrency()
        {
            return db.app_currency.Local.Where(x => x.is_priority).FirstOrDefault();
        }
    }
}
