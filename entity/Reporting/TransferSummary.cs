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
        public string OriginL { get; set; }
        public string DestinationL { get; set; }
        public string OriginB { get; set; }
        public string DestinationB { get; set; }
        public string Project { get; set; }
        public string Tag { get; set; }
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
