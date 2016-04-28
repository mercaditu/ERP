using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognitivo.Class

{
    class clsTransferAmount
    {
        public int id_payment_type { get; set; }
        public decimal amount { get; set; }
        public int id_currencyfx { get; set; }
        public String Currencyfxname { get; set; }
        public String PaymentTypeName { get; set; }
        public decimal amountCounted { get; set; }
    }
}
