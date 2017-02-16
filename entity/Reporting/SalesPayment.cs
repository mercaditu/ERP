using System;

namespace entity.Reporting
{
    public class SalesPayment
    {
        public int SchedualID { get; set; }
        public string Contact { get; set; }
        public string SalesPerson { get; set; }
        public string Code { get; set; }
        public string GovID { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Contract { get; set; }
        public string Conditions { get; set; }
        public string Currency { get; set; }
        public decimal SalesRate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string SalesReturn { get; set; }
        public string Payment { get; set; }
        public string Type { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentRate { get; set; }
        public string PaymentCurrency { get; set; }
    }
}