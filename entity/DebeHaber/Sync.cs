using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.DebeHaber
{
    public enum TransactionTypes { Sales = 1, Purchase = 1}
    public enum CostCenterTypes { Expense = 1, Merchendice = 2, FixedAsset = 3 }

    public class Commercial_Invoice
    {
        public Commercial_Invoice()
        {
            CommercialInvoice_Detail = new List<CommercialInvoice_Detail>();
            Commercial_Return = new List<Commercial_Return>();
            Payments = new List<Payments>();
        }
        
        //Invoice Data
        public TransactionTypes Type { get; set; }
        public DateTime Date { get; set; }
        public string Gov_Code { get; set; }

        //Invoice Documents
        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }

        //Currency
        public string Currency { get; set; }

        public virtual ICollection<CommercialInvoice_Detail> CommercialInvoice_Detail { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual ICollection<Commercial_Return> Commercial_Return { get; set; }
    }

    public class CommercialInvoice_Detail
    { 
        public CostCenter CostCenter { get; set; }

        public decimal VAT_Coeficient { get; set; }
        public decimal Value { get; set; }
        public string Comment { get; set; }
        public virtual Commercial_Invoice Commercial_Invoice { get; set; }
    }

    public class CostCenter
    {
        public int Reference_ID { get; set; }
        public CostCenterTypes Type { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CommercialInvoice_Detail> CommercialInvoice_Detail { get; set; }
    }

    public class Commercial_Return
    {
        //Return Data
        public TransactionTypes Type { get; set; }
        public DateTime Date { get; set; }
        public string Gov_Code { get; set; }

        //Invoice Documents
        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }
    }

    public class Payments
    {
        public int Type { get; set; }
        public DateTime Date { get; set; }
        public string Parent { get; set; }
        public string Gov_Code { get; set; }

        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }

        public string  Account { get; set; }
        public decimal Value { get; set; }
     
    }

    public class Vat
    {
        public int Type { get; set; }
        public decimal Coef { get; set; }
        public decimal ValueWVAT { get; set; }
        public string CostCenter { get; set; }
        public string CostCenterType { get; set; }
    }

    public class CreditNote
    {
        public CreditNote()
        {
            Commercial_Invoice = new List<Commercial_Invoice>();
           
        }
         public virtual ICollection<Commercial_Invoice> Commercial_Invoice { get; set; }
     
    }
}
