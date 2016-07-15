using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.DebeHaber
{
    public enum TransactionTypes { Sales = 1, Purchase = 1}
    public enum CostCenterTypes { Expense = 1, Merchendice = 2, FixedAsset = 3, Income = 4 }
    public enum PaymentTypes { Normal = 1, CreditNote = 2, VATWithHolding = 3 }

    public class Methods
    {
        public string Currency2ISOCode(string CurrencyName)
        {
            string ISOCode = string.Empty;

            if (CurrencyName.Contains("Guaran"))
            {
                ISOCode = "PYG";
            }
            else if (CurrencyName.Contains("Dol") || CurrencyName.Contains("lar"))
            {
                ISOCode = "USD";
            }
            else if (CurrencyName.Contains("Euro"))
            {
                ISOCode = "EUR";
            }
            else if (CurrencyName.Contains("Argen"))
            {
                ISOCode = "ARP";
            }
            else if (CurrencyName.Contains("Bra"))
            {
                ISOCode = "BRL";
            }

            return ISOCode;
        }
    }

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
        public DateTime TransDate { get; set; }
        public string Gov_Code { get; set; }
        public string Comment { get; set; }
        public string CurrencyName { get; set; }

        //Invoice Documents
        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }

        //Collection Property
        public virtual ICollection<CommercialInvoice_Detail> CommercialInvoice_Detail { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual ICollection<Commercial_Return> Commercial_Return { get; set; }

        //Fill Methods
        public void Fill_BySales(sales_invoice sales_invoice)
        {
            this.Type = entity.DebeHaber.TransactionTypes.Sales;
            this.TransDate = sales_invoice.trans_date;
            this.Gov_Code = sales_invoice.contact.gov_code;
            this.Comment = sales_invoice.comment;
            this.CurrencyName = sales_invoice.app_currencyfx.app_currency.name;

            this.DocNumber = sales_invoice.number;
            this.DocCode = sales_invoice.app_document_range != null ? sales_invoice.app_document_range.code : "";
            this.DocExpiry = (sales_invoice.app_document_range != null ? (DateTime)sales_invoice.app_document_range.expire_date : DateTime.Now);
        }

        public void Fill_ByPurchase(purchase_invoice purchase_invoice)
        {
            this.Type = entity.DebeHaber.TransactionTypes.Sales;
            this.TransDate = purchase_invoice.trans_date;
            this.Gov_Code = purchase_invoice.contact.gov_code;
            this.Comment = purchase_invoice.comment;
            this.CurrencyName = purchase_invoice.app_currencyfx.app_currency.name;

            this.DocNumber = purchase_invoice.number;
            this.DocCode = purchase_invoice.code;
            //No Expiry Date for Purchase Code.
        }
    }

    public class CommercialInvoice_Detail
    {
        public CommercialInvoice_Detail()
        {
            CostCenter = new List<CostCenter>();
        }

        public decimal VAT_Coeficient { get; set; }
        public decimal UnitValue_WithVAT { get; set; }
        public string Comment { get; set; }

        //Nav Property
        public virtual Commercial_Invoice Commercial_Invoice { get; set; }
        //Collection Property
        public virtual ICollection<CostCenter> CostCenter { get; set; }

        //Methods
        public void Fill_BySales(sales_invoice_detail Detail, db db)
        {
            this.VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            this.UnitValue_WithVAT = Detail.SubTotal_Vat;
            this.Comment = Detail.item_description;

            entity.DebeHaber.CostCenter CostCenter = new entity.DebeHaber.CostCenter();

            // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
            if (Detail.item.id_item_type == entity.item.item_type.FixedAssets)
            {
                CostCenter.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                CostCenter.Type = entity.DebeHaber.CostCenterTypes.FixedAsset;

                //Add CostCenter into Detail.
                this.CostCenter.Add(CostCenter);
            }
            // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
            else if (Detail.item.id_item_type == entity.item.item_type.Service || Detail.item.id_item_type == entity.item.item_type.Task || Detail.item.id_item_type == entity.item.item_type.ServiceContract)
            {
                if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                { CostCenter.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                else
                { CostCenter.Name = Detail.item_description; }

                CostCenter.Type = entity.DebeHaber.CostCenterTypes.Income;

                //Add CostCenter into Detail.
                this.CostCenter.Add(CostCenter);
            }
            // Finally if all else fails, assume Item being sold is Merchendice.
            else
            {
                CostCenter.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                CostCenter.Type = entity.DebeHaber.CostCenterTypes.Merchendice;

                //Add CostCenter into Detail.
                this.CostCenter.Add(CostCenter);
            }
        }
    }

    public class CostCenter
    {
        public CostCenterTypes Type { get; set; }
        public string Name { get; set; }

        public CommercialInvoice_Detail CommercialInvoice_Detail { get; set; }
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
        public PaymentTypes PaymentType { get; set; }
        public DateTime TransDate { get; set; }
        public string Parent { get; set; }
        public string Gov_Code { get; set; }

        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }

        public string  Account { get; set; }
        public decimal Value { get; set; }

        public void FillPayments(entity.payment_schedual schedual)
        {
            this.PaymentType = entity.DebeHaber.PaymentTypes.Normal;

            if (schedual.payment_detail.payment_type.payment_behavior == entity.payment_type.payment_behaviours.CreditNote)
            {
                this.PaymentType = entity.DebeHaber.PaymentTypes.CreditNote;
            }
            else if (schedual.payment_detail.payment_type.payment_behavior == entity.payment_type.payment_behaviours.WithHoldingVAT)
            {
                this.PaymentType = entity.DebeHaber.PaymentTypes.VATWithHolding;
            }

            this.Parent = schedual.parent.sales_invoice.number;
            this.Gov_Code = schedual.payment_detail.payment.contact != null ? schedual.payment_detail.payment.contact.gov_code : "";
            this.DocCode = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.code : "";
            this.DocExpiry = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.expire_date : DateTime.Now;
            this.DocNumber = schedual.payment_detail.payment.number;

            this.Account = schedual.payment_detail.app_account != null ? schedual.payment_detail.app_account.name : "";
            this.Value = schedual.payment_detail.value;

            this.TransDate = schedual.payment_detail.payment.trans_date;
            this.Account = schedual.payment_detail.app_account.name;
            this.Value = schedual.debit;
        }
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
