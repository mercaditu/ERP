using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
    public class TransferSummary
    {

        public DateTime Date { get; set; }
        public string Movement { get; set; }
        public string Transfer { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; }
        public string RequestedName { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity_D { get; set; }
        public decimal Quantity_O { get; set; }
    }
     
}
