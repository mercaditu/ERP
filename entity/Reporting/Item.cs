
namespace entity.Reporting
{
    public class Item
    {
        public string Tag { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Sku { get; set; }
        public string Variation { get; set; }
        public string Description { get; set; }
        public decimal StockMax { get; set; }
        public decimal StockMin { get; set; }
        public bool CanExpire { get; set; }

        public string Branch { get; set; }
        public decimal InStock { get; set; }
        public decimal Velocity { get; set; }
    }
}