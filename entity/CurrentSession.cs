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

        public static db db { get; set; }

        public void Start(string UserName, string Password)
        {
            //Set the User
            User = db.security_user.Where(user => user.name == UserName
                                               && user.password == Password
                                               && user.id_company == Properties.Settings.Default.company_ID)
                                   .FirstOrDefault();
            //Set the Company
            Company = User.app_company;
        }
    }
}
