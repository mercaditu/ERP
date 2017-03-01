using System;

namespace Reports
{
    public class PaymentsReceivable
    {
        public int Refrence { get; set; }
        public string Customer { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string GovCode { get; set; }
        public string Branch { get; set; }
        public string Conditions { get; set; }
        public string Contract { get; set; }
        public string SalesNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Salesman { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime Payment { get; set; }
        public decimal Paid { get; set; }
        public string PaymentType { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateDiff { get; set; }
    }
}