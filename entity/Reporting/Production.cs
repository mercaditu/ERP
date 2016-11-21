using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
  public  class Production
    {
        public string Project { get; set; }
        public string Line { get; set; }
        public string ProductiontName { get; set; }
        public string Number { get; set; }
        public DateTime TransDate { get; set; }
        public string CostCenter { get; set; }
        public int OrderID { get; set; }
        public int ParentID { get; set; }
        public int ProductionStatus { get; set; }
        public bool Input { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
