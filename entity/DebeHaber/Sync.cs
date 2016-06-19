using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.DebeHaber
{
    public enum TransactionTypes { Sales, Purchase}
    public enum CostCenterTypes { Expense, Merchendice, FixedAsset }

    public class Commercial_Invoice
    {
        public int Reference_ID { get; set; }
        public TransactionTypes Type { get; set; }
        public string CurrencyISO_Code { get; set; }

        public string Contact_GovCode { get; set; }

        public string BranchName { get; set; }
        public string BranchCode { get; set; }

        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime InvoiceCode_ExpDate { get; set; }
        public decimal GrandTotal { get; set; }
        public int PaymentCondition { get; set; }

        public string Comment { get; set; }

        public virtual ICollection<CommercialInvoice_Detail> CommercialInvoice_Detail { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
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
        public int Reference_ID { get; set; }
        public TransactionTypes Type { get; set; }
        public string CurrencyISO_Code { get; set; }

        public string Contact_GovCode { get; set; }

        public string BranchName { get; set; }
        public string BranchCode { get; set; }

        public DateTime ReturnDate { get; set; }
        public decimal ReturnNumber { get; set; }
        public string ReturnCode { get; set; }
        public DateTime ReturnCodeDate { get; set; }
        public decimal GrandTotal { get; set; }
        public int PaymentCondition { get; set; }

        public string Comment { get; set; }
    }

    public class Payments
    {

    }
}
