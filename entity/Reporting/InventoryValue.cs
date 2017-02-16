using System;

namespace entity.Reporting
{
    public class InventoryValue
    {
        public DateTime Date { get; set; }
        public string Branch { get; set; }
        public string Tag { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public decimal Credit { get; set; }
        public decimal DebitChild { get; set; }
        public decimal Balance { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal RetailPrice { get; set; }
    }
}