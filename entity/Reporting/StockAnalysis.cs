using System;

namespace entity.Reporting
{
    public class StockAnalysis
    {
        public string Brand { get; set; }
        public string Branch { get; set; }
        public string Supplier { get; set; }
        public int LeadTime { get; set; }
        public string Code { get; set; }
        public string Items { get; set; }
        public bool CanExpire { get; set; }

        public decimal MaxStock { get; set; }
        public decimal SafetyStock { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
}