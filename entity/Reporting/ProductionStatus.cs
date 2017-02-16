using System;

namespace entity.Reporting
{
    public class ProductionStatus
    {
        public string ProductionOrder { get; set; }
        public int id_production_order { get; set; }
        public int id_order_detail { get; set; }
        public int Parent { get; set; }
        public string TaskName { get; set; }
        public string TaskCode { get; set; }
        public decimal EstQuantity { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Coeficient { get; set; }
        public decimal Hours { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}