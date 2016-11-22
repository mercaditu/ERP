using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
  public  class StockFlowDimension
    {

        public int Branch { get; set; }
        public string Location { get; set; }
        public int id_movement { get; set; }
        public string Tag { get; set; }
        public string ItemCode { get; set; }
        public string Item { get; set; }
        public decimal debit { get; set; }
        public decimal credit { get; set; }
        public decimal Cost { get; set; }
        public string Dimension { get; set; }
        public decimal value { get; set; }
        public DateTime trans_date { get; set; }

        public string UserName { get; set; }
    }
}
