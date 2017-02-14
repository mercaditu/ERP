using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
    public class PaymentsReceivable
    {
        public string Customer { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Code{ get; set; }
        public string GovCode { get; set; }
        public string Branch { get; set; }
        public string Condition { get; set; }
        public string Contract { get; set; }
        public string Number { get; set; }
        public DateTime SalesDate { get; set; }
        public decimal Salesman { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Decimal Debit { get; set; }
        public Decimal Credit { get; set; }
        public string number { get; set; }
        public DateTime Payment { get; set; }
        public Decimal Paid { get; set; }
        public string PaymentType { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateDiff { get; set; }
      

    }
}
