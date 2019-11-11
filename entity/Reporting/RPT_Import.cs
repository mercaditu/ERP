using System;

namespace entity.Reporting
{
    public class RPT_Import
    {
        public int Status { get; set; }
        public string Number { get; set; }
        public DateTime ETD { get; set; }
        public DateTime ETA { get; set; }
        public string PurchaseInvoice { get; set; }
        public Decimal Value { get; set; }
        public string Expense { get; set; }
      
    }
}