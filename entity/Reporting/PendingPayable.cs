using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
    public class PendingPayable
    {
        public string Code { get; set; }
        public string GovID { get; set; }
        public string Contact { get; set; }
        public string Number { get; set; }
        public string Comment { get; set; }
        public string Conditions { get; set; }
        public string Contract { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CurrencyName { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public DateTime TransDate { get; set; }
      
      
    }
}
