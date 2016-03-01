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
                if (_Id_Company==0)
                {
                    _Id_Company = Properties.Settings.Default.company_ID;
                };
                return _Id_Company;
            }
            set { _Id_Company=value; }
        }
          static int _Id_Company;
          public static int Id_User
          {
              get
              {
                  if (_Id_User == 0)
                  {
                      _Id_User = Properties.Settings.Default.user_ID;
                  };
                  return _Id_User;
              }
              set { _Id_User = value; }
          }
          static int _Id_User;
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
        public static app_company Company { get; set; }
        public static app_branch Branch { get; set; }
        public static app_terminal Terminal { get; set; }

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
                //Set the Company
                Company = User.app_company;

                //Check if Branch Exists
                if (Company.app_branch.Where(branch => 
                                branch.id_company == Company.id_company &&
                                branch.id_branch == Properties.Settings.Default.branch_ID)
                                .FirstOrDefault() != null)
                {
                    //Set Branch
                    Branch = Company.app_branch.Where(branch =>
                                branch.id_company == Company.id_company &&
                                branch.id_branch == Properties.Settings.Default.branch_ID)
                                .FirstOrDefault();
                }

                //Check if Terminal Exists inside Branch
                if (Branch.app_terminal.Where(terminal =>
                                terminal.id_terminal == Properties.Settings.Default.terminal_ID)
                                .FirstOrDefault() != null)
                {
                    //Set Terminal
                    Terminal = Branch.app_terminal.Where(terminal =>
                                terminal.id_terminal == Properties.Settings.Default.terminal_ID)
                                .FirstOrDefault();
                }
            }
        }
    }
}
