using System;

namespace entity.Reporting
{
    public class InventoryValue
    {
        public DateTime Date { get; set; }
        public string Branch { get; set; }
        public string Location { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
      
        public decimal Balance { get; set; }
        public decimal UnitCost { get; set; }
        public string BatchCode { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}