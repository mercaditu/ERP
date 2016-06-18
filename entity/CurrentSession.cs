using System.Collections.Generic;
using System.Linq;

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
                    //Set Branch
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
                    //Set Terminal
                    Id_Terminal = db.app_terminal.Where(terminal =>
                                    terminal.id_branch == Id_Branch &&
                                    terminal.id_terminal == Properties.Settings.Default.terminal_ID)
                                    .FirstOrDefault().id_terminal;
                }


                //Setting Security, once CurrentSession Data is set.
                Refresh_Security();
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
    }
}
