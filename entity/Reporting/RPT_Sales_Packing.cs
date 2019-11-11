using System;

namespace entity.Reporting
{
    public class RPT_Sales_Packing
    {
        public int Status { get; set; }
        public int DetailID { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Branch { get; set; }
        public string Terminal { get; set; }
        public string Customer { get; set; }
        public string GovCode { get; set; }
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public string Code { get; set; }
        public string Items { get; set; }
        public decimal Quantity { get; set; }
        public decimal VerifiedQuantity { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string BatchCode { get; set; }
    }
}