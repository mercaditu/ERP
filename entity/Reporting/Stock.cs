using System;

namespace entity.Reporting
{
  public  class Stock
    {
        public string Tag { get; set; }
        public string ItemCode { get; set; }
        public string Item { get; set; }
        public string Comment { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Cost { get; set; }
        public string Dimension { get; set; }
        public decimal value { get; set; }
        public DateTime trans_date { get; set; }

        public string UserName { get; set; }
    }
}
