namespace entity.Reporting
{
    public class StockByBranch
    {
        public int LocationID { get; set; }
        public string Location { get; set; }
        public string Tag { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public string Measurement { get; set; }
        public decimal Cost { get; set; }
        public string Brand { get; set; }
    }
}