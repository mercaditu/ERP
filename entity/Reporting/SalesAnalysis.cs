namespace entity.Reporting
{
    public class SalesAnalysis
    {
        //public decimal TotalCredits { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal Cost { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public string Tag { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public string Branch { get; set; }
        
        //public decimal Stock { get; set; }
        public decimal PurchaseIn { get; set; }
        public decimal TransferIn { get; set; }
        public decimal InventoryIn { get; set; }
        public decimal ProductionIn { get; set; }
        public decimal SalesOut { get; set; }
        public decimal TransferOut { get; set; }
        public decimal InventoryOut { get; set; }
        public decimal ProductionOut { get; set; }

        public decimal TotalOut { get; set; }
        public decimal TotalIn { get; set; }

    }
}
