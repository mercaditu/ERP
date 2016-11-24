using System;

namespace entity.Reporting
{
    public class PurchaseTender
    {
        public int Status { get; set; }
        public string TenderName { get; set; }
        public string TenderNumber { get; set; }
        public string ProjectName { get; set; }
        public string Supplier { get; set; }
        public string PurchaseCondition { get; set; }
        public string Contract { get; set; }
        public string ItemDescription { get; set; }
        public decimal Cost { get; set; }
        public decimal Quantity { get; set; }
        public decimal Ordered { get; set; }
      
    }
}
