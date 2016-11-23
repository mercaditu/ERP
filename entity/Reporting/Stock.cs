using System;

namespace entity.Reporting
{
  public class Stock
    {
        public int MovementID { get; set; }

        public string Branch { get; set; }
        public string Location { get; set; }
        public string Tag { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public string Comment { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string CostDetail { get; set; }
        public decimal Cost { get; set; }
        public string Dimension { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string LotNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int ParentID { get; set; }

        public string UserName { get; set; }
    }
}
