using System;

namespace entity.Reporting
{
   public class RPT_Project
    {
        public string ProjectName { get; set; }
        public int id_project_task { get; set; }
        public int ParentTask { get; set; }
        public string Item { get; set; }
        public string ItemCode { get; set; }
        public string TaskCode { get; set; }
        public string Task { get; set; }
        public string Contact { get; set; }
        public string ContactCode { get; set; }
        public string GovermentId { get; set; }
        public decimal QuantityEst { get; set; }
        public decimal QuantityReal { get; set; }
        public decimal CostEst { get; set; }
        public decimal CostReal { get; set; }
        public decimal StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TotalBudgeted { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }

    }
}
