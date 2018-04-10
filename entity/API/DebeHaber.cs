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
        public string CustomerTaxID { get; set; }
        public string CustomerName { get; set; }
        public string SupplierTaxID { get; set; }
        public string SupplierName { get; set; }
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
        public string Comment { get; set; }
        public DateTime? CodeExpiry { get; set; }
        public int PaymentCondition { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public ICollection<InvoiceDetail> Details { get; set; }

        public void LoadSales(sales_invoice data)
        {
            Type = InvoiceTypes.Sales;
            CustomerName = data.contact.name;
            CustomerTaxID = data.contact.gov_code;
            SupplierName = data.app_company.name;
            SupplierTaxID = data.app_company.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;

            Details = new List<InvoiceDetail>();
            foreach (sales_invoice_detail sales_invoice_detail in data.sales_invoice_detail)
            {
                foreach (var VatDetail in sales_invoice_detail.app_vat_group.app_vat_group_details)
                {
                    ItemTypes DetailType = ItemTypes.RevenueByService;
                    string Name = "Service";
                    if (sales_invoice_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        DetailType = ItemTypes.Fixedasset;
                        Name = "Fixedasset";
                    }
                    else if (sales_invoice_detail.item.id_item_type == item.item_type.Product
                        || sales_invoice_detail.item.id_item_type == item.item_type.RawMaterial
                        || sales_invoice_detail.item.id_item_type == item.item_type.Supplies)
                    {
                        DetailType = ItemTypes.RevenueByProduct;
                        Name = "Product";
                    }

                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = sales_invoice_detail.unit_cost;
                    Detail.Value = sales_invoice_detail.UnitPrice_Vat;
                    Detail.VATPercentage = Convert.ToInt32(sales_invoice_detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient) * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }
        }

        public void LoadPurchase(purchase_invoice data)
        {
            Type = InvoiceTypes.Purchase;
            SupplierName = data.contact.name;
            SupplierTaxID = data.contact.gov_code;
            CustomerName = data.app_company.name;
            CustomerTaxID = data.app_company.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;

            Details = new List<InvoiceDetail>();
            foreach (purchase_invoice_detail purchase_invoice_detail in data.purchase_invoice_detail)
            {
                foreach (var VatDetail in purchase_invoice_detail.app_vat_group.app_vat_group_details)
                {
                    ItemTypes DetailType = ItemTypes.RevenueByService;
                    string Name = "Service";
                    if (purchase_invoice_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        DetailType = ItemTypes.Fixedasset;
                        Name = "Fixedasset";
                    }
                    else if (purchase_invoice_detail.item.id_item_type == item.item_type.Product
                        || purchase_invoice_detail.item.id_item_type == item.item_type.RawMaterial
                        || purchase_invoice_detail.item.id_item_type == item.item_type.Supplies)
                    {
                        DetailType = ItemTypes.RevenueByProduct;
                        Name = "Product";
                    }

                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = purchase_invoice_detail.unit_cost;
                    Detail.Value = purchase_invoice_detail.UnitCost_Vat;
                    Detail.VATPercentage = Convert.ToInt32(purchase_invoice_detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient) * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }
        }

        public void LoadSalesReturn(sales_return data)
        {
            Type = InvoiceTypes.SalesReturn;
            SupplierName = data.contact.name;
            SupplierTaxID = data.contact.gov_code;
            CustomerName = data.app_company.name;
            CustomerTaxID = data.app_company.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;

            Details = new List<InvoiceDetail>();
            foreach (sales_return_detail sales_return_detail in data.sales_return_detail)
            {
                foreach (var VatDetail in sales_return_detail.app_vat_group.app_vat_group_details)
                {
                    ItemTypes DetailType = ItemTypes.RevenueByService;
                    string Name = "Service";
                    if (sales_return_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        DetailType = ItemTypes.Fixedasset;
                        Name = "Fixedasset";
                    }
                    else if (sales_return_detail.item.id_item_type == item.item_type.Product
                        || sales_return_detail.item.id_item_type == item.item_type.RawMaterial
                        || sales_return_detail.item.id_item_type == item.item_type.Supplies)
                    {
                        DetailType = ItemTypes.RevenueByProduct;
                        Name = "Product";
                    }

                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = sales_return_detail.unit_cost;
                    Detail.Value = sales_return_detail.UnitPrice_Vat;
                    Detail.VATPercentage = Convert.ToInt32(sales_return_detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient) * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }
        }

        public void LoadPurchaseReturn(purchase_return data)
        {
            Type = InvoiceTypes.PurchaseReturn;
            CustomerName = data.contact.name;
            CustomerTaxID = data.contact.gov_code;
            SupplierName = data.app_company.name;
            SupplierTaxID = data.app_company.gov_code;
            Date = data.trans_date;
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = data.app_currencyfx.app_currency.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;

            Details = new List<InvoiceDetail>();
            foreach (purchase_return_detail purchase_return_detail in data.purchase_return_detail)
            {
                foreach (var VatDetail in purchase_return_detail.app_vat_group.app_vat_group_details)
                {
                    ItemTypes DetailType = ItemTypes.RevenueByService;
                    string Name = "Service";
                    if (purchase_return_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        DetailType = ItemTypes.Fixedasset;
                        Name = "Fixedasset";
                    }
                    else if (purchase_return_detail.item.id_item_type == item.item_type.Product
                        || purchase_return_detail.item.id_item_type == item.item_type.RawMaterial
                        || purchase_return_detail.item.id_item_type == item.item_type.Supplies)
                    {
                        DetailType = ItemTypes.RevenueByProduct;
                        Name = "Product";
                    }

                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = purchase_return_detail.unit_cost;
                    Detail.Value = purchase_return_detail.UnitCost_Vat;
                    Detail.VATPercentage = Convert.ToInt32(purchase_return_detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient) * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }
        }
    }

    public class InvoiceDetail
    {
        public ItemTypes Type { get; set; }
        public Int32 VATPercentage { get; set; }
        public decimal Value { get; set; }
        public decimal Cost { get; set; }
        public string Name { get; set; }
    }

    public class AccountMovements
    {
        public string Account { get; set; }
        public DateTime Date { get; set; }
        public string ReferenceInvoice { get; set; }
        public int? ReferenceInvoiceID { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public void LoadPaymentsMade(payment_detail data, string InvoiceNumber, int InvoiceID)
        {
            Account = data.app_account.name;
            Date = data.trans_date;
            CurrencyCode = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault().code;
            CurrencyRate = data.app_currencyfx.buy_value > 0 ? data.app_currencyfx.buy_value : data.app_currencyfx.sell_value;
            Debit = data.value;
            Credit = 0;

            ReferenceInvoice = InvoiceNumber;
            ReferenceInvoiceID = 0;
        }

        public void LoadPaymentsRecieved(payment_detail data, string InvoiceNumber, int InvoiceID)
        {
            Account = data.app_account.name;
            Date = data.trans_date;
            CurrencyCode = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault().code;
            CurrencyRate = data.app_currencyfx.buy_value > 0 ? data.app_currencyfx.buy_value : data.app_currencyfx.sell_value;
            Debit = 0;
            Credit = data.value;

            ReferenceInvoice = InvoiceNumber;
            ReferenceInvoiceID = 0;
        }

        //Make another API for MoneyTransfers
        public void LoadTransfers(app_account_detail data)
        {
            Account = data.app_account.name;
            Date = data.trans_date;
            CurrencyCode = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault().code;
            CurrencyRate = data.app_currencyfx.buy_value > 0 ? data.app_currencyfx.buy_value : data.app_currencyfx.sell_value;
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