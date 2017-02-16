using System;

namespace entity.Reporting
{
    public class CostBreakDown
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime TransDate { get; set; }
        public string Tag { get; set; }
        public string Comment { get; set; }
        public int MovID { get; set; }
        public decimal UnitValue { get; set; }
        public string Concept { get; set; }
        public decimal Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}