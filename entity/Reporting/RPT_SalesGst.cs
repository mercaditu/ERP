using System;

namespace entity.Reporting
{
    public class RPT_SalesGst
    {
        public int Status { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string Vat { get; set; }
        public string Customer { get; set; }
        public string GovCode { get; set; }
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public decimal coefficient { get; set; }
        public string Item { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public decimal unit_price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal SubTotalVAT { get; set; }
        public string Tag { get; set; }

       
    }
}