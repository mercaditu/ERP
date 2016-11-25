using System;

namespace entity.Reporting
{
    public class ContactSubscription
    {
        public string Customer { get; set; }
        public string ContactRole { get; set; }
        public int ContactID { get; set; }
        public int ParentID { get; set; }
        public string GovCode { get; set; }
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string GeoLevel1 { get; set; }
        public string GeoLevel2 { get; set; }
        public string GeoLevel3 { get; set; }
        public string GeoLevel4 { get; set; }
        public string GeoLevel5 { get; set; }
        public string ItemCode { get; set; }
        public string Items { get; set; }
        public string Tag { get; set; }
        public string Contract { get; set; }
        public string Vat { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BillCycle { get; set; }
        public short BillOn { get; set; }
   

    }
}
