using System;

namespace Cognitivo.Class

{
    internal class clsTransferAmount
    {
        public int id_payment_type { get; set; }
        public String PaymentTypeName { get; set; }
        public int id_currencyfxdest { get; set; }
        public int? id_accountdest { get; set; }
        public int? id_accountorigin { get; set; }
        public string AccountDest { get; set; }
        public string AccountOrigin { get; set; }
        public String Currencyfxnamedest { get; set; }
        public int id_currencyfxorigin { get; set; }
        public String Currencyfxnameorigin { get; set; }
        public decimal amount { get; set; }
        public decimal amountCounted { get; set; }
    }
}