using System;

namespace entity.Reporting
{
    public class PriceList
    {
        public string Code { get; set; }
        public string Items { get; set; }
        public string PriceLists { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Tag { get; set; }
        public decimal Cost { get; set; }
        public DateTime LastSold { get; set; }
        public decimal InStock { get; set; }
    }
}
