using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.API
{
    public class DebeHaber
    {
        
    }

    public enum InvoiceTypes { Purchase = 1, PurchaseReturn = 3, Sales = 4, SalesReturn = 5 }
    public enum ItemTypes { Inventory = 1, Expense = 2, RevenueByService = 3, RevenueByProduct = 4, Fixedasset = 5 }
    public class Invoice
    {
        public InvoiceTypes Type { get; set; }
        public string Taxpayer { get; set; }
        public string TaxpayerID { get; set; }
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public DateTime? CodeExpiry { get; set; }
        public int PaymentCondition { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public ICollection<InvoiceDetail> Details { get; set; }

        public void LoadSales(sales_invoice data)
        {
            Type = InvoiceTypes.Sales;
            Taxpayer = data.contact.name;
            TaxpayerID = data.contact.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.buy_value;
        }

        public void LoadPurchase(purchase_invoice data)
        {
            Type = InvoiceTypes.Purchase;
            Taxpayer = data.contact.name;
            TaxpayerID = data.contact.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.sell_value;
        }

        public void LoadSalesReturn(sales_return data)
        {
            Type = InvoiceTypes.SalesReturn;
            Taxpayer = data.contact.name;
            TaxpayerID = data.contact.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.buy_value;
        }

        public void LoadPurchaseReturn(purchase_invoice data)
        {
            Type = InvoiceTypes.PurchaseReturn;
            Taxpayer = data.contact.name;
            TaxpayerID = data.contact.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.sell_value;
        }
    }

    public class InvoiceDetail
    {
        public ItemTypes Type { get; set; }
        public decimal VATPercentage { get; set; }
        public decimal Value { get; set; }
        public decimal Cost { get; set; }
    }

    public class AccountMovements
    {
        public string Account { get; set; }
        public DateTime Date { get; set; }
        public string ReferenceInvoice { get; set; }
        public int? ReferenceInvoiceID { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}