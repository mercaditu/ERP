using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public static class Session
    {
        public static app_company Company { get; set; }
        public static app_branch Branch { get; set; }
        public static app_terminal Terminal { get; set; }

        public static security_user User { get; set; }
    }
}
