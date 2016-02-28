using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public static class CurrentSession
    {
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
                                branch.id_branch == Properties.Settings.Default.branch_ID)
                                .FirstOrDefault() != null)
                {
                    //Set Branch
                    Branch = Company.app_branch.Where(branch =>
                                branch.id_branch == Properties.Settings.Default.branch_ID)
                                .FirstOrDefault();
                }
            }
        }
    }
}
