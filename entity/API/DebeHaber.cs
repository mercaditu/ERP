using System;
using System.Collections.Generic;
using System.Linq;

namespace entity.API.DebeHaber
{
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

        public void LoadPaymentsMade(payment_detail data, string InvoiceNumber, int InvoiceID)
        {
            Account = data.app_account.name;
            Date = data.trans_date;
            Debit = data.value;
            Credit = 0;

            ReferenceInvoice = InvoiceNumber;
            ReferenceInvoiceID = 0;
        }

        public void LoadPaymentsRecieved(payment_detail data, string InvoiceNumber, int InvoiceID)
        {
            Account = data.app_account.name;
            Date = data.trans_date;
            Debit = 0;
            Credit = data.value;

            ReferenceInvoice = InvoiceNumber;
            ReferenceInvoiceID = 0;
        }

        public void LoadTransfers(app_account_detail data)
        {
            Account = data.app_account.name;
            Date = data.trans_date;
            Debit = data.debit;
            Credit = data.credit;
        }
    }

    public class Production
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        
        //Input Data
        public ItemTypes InputType { get; set; }
        public decimal InputCost { get; set; }
        
        //Output Data --> can be null
        public ItemTypes? OutputType { get; set; }
        public decimal? OutputValue { get; set; }

        public void Load(production_execution_detail data)
        {
            Name = data.production_order_detail.production_order.name;
            Date = data.trans_date;

            if (data.item.id_item_type == item.item_type.Product || 
                data.item.id_item_type == item.item_type.RawMaterial || 
                data.item.id_item_type == item.item_type.Supplies)
            {
                InputType = ItemTypes.Inventory;
            }
            else
            {
                InputType = ItemTypes.Expense;
            }

            InputCost = (data.unit_cost * data.quantity);

            item.item_type type = data.production_order_detail.item.id_item_type;

            if (type == item.item_type.Product ||
                type == item.item_type.RawMaterial ||
                type == item.item_type.Supplies)
            {
                OutputType = ItemTypes.Inventory;
            }
        }
    }
}