using System;

namespace entity.Reporting
{
    public class PurchaseTender
    {
        public int Status { get; set; }
        public string Tender { get; set; }
        public string Number { get; set; }
        public string Project { get; set; }
        public string Supplier { get; set; }
        public string Condition { get; set; }
        public string Contract { get; set; }
        public string Items { get; set; }
        public decimal Cost { get; set; }
        public decimal Quantity { get; set; }
        public decimal Ordered { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
    }
}
