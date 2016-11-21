using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
    public class EmployeeInProduction
    {
        public string Employee { get; set; }
        public string item_description { get; set; }
        public string Project { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Coefficient { get; set; }
        public decimal Hours { get; set; }
        public decimal ComputeHours { get; set; }
    }
}
