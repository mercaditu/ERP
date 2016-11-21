using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Reporting
{
    public class Customer
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string GovCode { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Tag { get; set; }
        public string Contract { get; set; }
        public decimal credit_limit { get; set; }
    }
}
