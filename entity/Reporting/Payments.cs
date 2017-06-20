using System;

namespace entity.Reporting
{
    public class Payments
    {
        public string Contact { get; set; }
        public string Code { get; set; }
        public string GovernmentID { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Collector { get; set; }
        public string SalesNumber { get; set; }
        public string SalesDate { get; set; }
        public string PurchaseNumber { get; set; }
        public string PurchaseDate { get; set; }
        public string Salesman { get; set; }
        public string PaymentType { get; set; }
        public string Number { get; set; }
        public string Account { get; set; }
        public string AccountNumber { get; set; }
        public string PaymentTypeNumber { get; set; }
        public string Date { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Value { get; set; }

        public decimal Difference { get; set; }
        public decimal ItemCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Tag { get; set; }
        public string Comment { get; set; }
        public string Branch { get; set; }
        public string Location { get; set; }
        public string Batch { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}