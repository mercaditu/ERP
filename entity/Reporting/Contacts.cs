using System;

namespace entity.Reporting
{
    public class Contacts
    {
        public bool Customer { get; set; }
        public bool Supplier { get; set; }
        public int ContactID { get; set; }
        public int ParentID { get; set; }
        public bool Active { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string GovCode { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Tag { get; set; }
        public string Contract { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AccountPayable { get; set; }
        public decimal AccountRecievable { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Geography { get; set; }
        public string SalesRep { get; set; }
        public string Bank { get; set; }
        public string PriceList { get; set; }
        public string ContactRole { get; set; }
        public string Currency { get; set; }
        public string CostCenter { get; set; }
        public string Gender { get; set; }
    }
}