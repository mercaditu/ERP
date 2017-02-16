using System;

namespace entity.Reporting
{
    public class ProductionOrder
    {
        public string Project { get; set; }
        public string ProjectCode { get; set; }
        public string Contact { get; set; }
        public string Line { get; set; }
        public string Production { get; set; }
        public string Number { get; set; }
        public DateTime TransDate { get; set; }
        public string CostCenter { get; set; }
        public int OrderID { get; set; }
        public int ParentID { get; set; }
        public int Status { get; set; }
        public bool Input { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityExecuted { get; set; }
        public decimal CostExecuted { get; set; }
        public decimal CostEstimated { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}