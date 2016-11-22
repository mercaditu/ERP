using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
    public class InventoryValue
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Credit { get; set; }
        public decimal DebitChild { get; set; }
        public decimal Balance { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal RetailPrice { get; set; }
    }
}
