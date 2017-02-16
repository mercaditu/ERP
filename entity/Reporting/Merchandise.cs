using System;

namespace entity.Reporting
{
    public class Merchandise
    {
        public string BranchName { get; set; }
        public string TransComment { get; set; }
        public string Tag { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime TransDate { get; set; }
    }
}