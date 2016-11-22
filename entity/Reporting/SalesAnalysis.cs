using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
    public class SalesAnalysis
    {
        public decimal TotalCredits { get; set; }
        public decimal RetailPrice { get; set; }
        public string Tag { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public string Branch { get; set; }
        public decimal Stock { get; set; }
        public decimal Sales { get; set; }
        public decimal Cost { get; set; }
       
    }
}
