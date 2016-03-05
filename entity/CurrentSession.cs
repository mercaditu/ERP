using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                _Id_Company=value; 
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

        public static int Id_terminal
        {
            get
            {
                if (_Id_terminal == 0)
                {
                    _Id_terminal = Properties.Settings.Default.terminal_ID;
                };
                return _Id_terminal;
            }
            set { _Id_terminal = value; }
        }
        static int _Id_terminal;

        public static security_user User { get; set; }

        private static db db = new db();

        public static void Start(string UserName, string Password)
        {
            //Set the User
            User = db.security_user.Where(user => user.name == UserName
                                               && user.password == Password
                                               && user.id_company == Properties.Settings.Default.company_ID)
                                   .FirstOrDefault();
            if (User != null)
            {
                //Set the User
                Id_User = User.id_user;

                entity.Properties.Settings.Default.user_Name = User.name_full;
                entity.Properties.Settings.Default.Save();

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
                    Id_terminal = db.app_terminal.Where(terminal =>
                                    terminal.id_branch == Id_Branch &&
                                    terminal.id_terminal == Properties.Settings.Default.terminal_ID)
                                    .FirstOrDefault().id_terminal;
                }
            }
        }
    }
}
