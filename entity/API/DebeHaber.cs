using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.API.DebeHaber
{
    public enum InvoiceTypes { Purchase = 1, Sales = 2 }
    public enum InvoiceSubTypes { Invoice = 1, Notes = 2 }

    public enum AccountTypes { AccountPayable = 1, AccountReceivable = 2, Sales = 3 }

    public enum BusineesCenter { RevenueByService = 1, Asset_Inventory = 2, FixedAsset = 3 }


    public class Impex
    {
        public InvoiceTypes Type { get; set; }
        public string Date { get; set; }
        public String Number { get; set; }
        public string SupplierTaxID { get; set; }
        public string SupplierName { get; set; }
        public string CustomerTaxID { get; set; }
        public string CustomerName { get; set; }
        public string Comment { get; set; }
        public bool IsImport { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public ICollection<Expenses> Expenses { get; set; }
        public ICollection<Invoice> Invoices { get; set; }

        public void loadImpex(impex data, app_company app_company)
        {
            Type = InvoiceTypes.Purchase;
            Date = data.real_landed_date!=null? data.real_landed_date.Value.ToUniversalTime().ToString("yyyy-MM-dd") : DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd"); 
            Number = data.number;
            SupplierName = data.contact.name;
            SupplierTaxID = data.contact.gov_code;
            CustomerName = app_company.name;
            CustomerTaxID = app_company.gov_code;
            IsImport = true; 

            Expenses = new List<Expenses>();
            foreach (impex_expense impex_expense in data.impex_expense)
            {

                Expenses expense = new Expenses();
                //epxense type check
                expense.Type = InvoiceTypes.Purchase;
                expense.Value = Convert.ToDecimal(impex_expense.value);
                expense.Name = impex_expense.impex_incoterm_condition.name;
                expense.CurrencyCode = impex_expense.app_currency != null ? impex_expense.app_currency.code : CurrentSession.Currency_Default.code;
                expense.CurrencyRate = impex_expense.currency_rate;
                Expenses.Add(expense);

            }
            //load expense
            Invoices = new List<Invoice>();
            foreach (impex_import impeximport in data.impex_import)
            {
                using (db db = new db())
                {
                    purchase_invoice purchase = db.purchase_invoice.Where(x => x.id_purchase_invoice == impeximport.id_purchase_invoice)
                                    .Include(x => x.contact)
                                    .Include(x => x.purchase_invoice_detail)
                                    .Include(x => x.app_currencyfx)
                                    .Include(x => x.app_company)
                                    .OrderBy(x => x.timestamp)
                                    .FirstOrDefault();

                    var Invoice = new Invoice();
                    Invoice.LoadPurchase(purchase, app_company);
                    Invoices.Add(Invoice);
                }
            }
            //load invoice with using
            //load purchase
        }
    }


    public class Invoice
    {
        public InvoiceTypes Type { get; set; }
        public InvoiceSubTypes SubType { get; set; }
        public int local_id { get; set; }
        public int cloud_id { get; set; }
        public string CustomerTaxID { get; set; }
        public string CustomerName { get; set; }
        public string SupplierTaxID { get; set; }
        public string SupplierName { get; set; }
        public string Date { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
        public string Comment { get; set; }
        public string CodeExpiry { get; set; }
        public int PaymentCondition { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public bool IsImpex { get; set; }
        public string Message { get; set; }
        public ICollection<InvoiceDetail> Details { get; set; }


        public void LoadSales(sales_invoice data, app_company app_company)
        {

            app_currency app_currency = null;
            if (data.app_currencyfx.app_currency != null)
            {
                app_currency = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault();
            }

            local_id = data.id_sales_invoice;
            Type = InvoiceTypes.Sales;
            SubType = InvoiceSubTypes.Invoice;
            CustomerName = data.contact.name;
            CustomerTaxID = data.contact.gov_code;
            SupplierName = app_company.name;
            SupplierTaxID = app_company.gov_code;
            Date = data.trans_date.Date.ToString("yyyy-MM-dd");
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date != null ? Convert.ToDateTime(data.app_document_range.expire_date).Date.ToString("yyyy-MM-dd") : null : null;
            PaymentCondition = data.app_contract != null ? data.app_contract.app_contract_detail.Max(x => x.interval) : 0;
            CurrencyCode = app_currency != null ? app_currency.code : CurrentSession.Currency_Default.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;


            Details = new List<InvoiceDetail>();
            foreach (sales_invoice_detail sales_invoice_detail in data.sales_invoice_detail)
            {

                app_vat_group app_vat_group = CurrentSession.VAT_Groups.Where(x => x.id_vat_group == sales_invoice_detail.id_vat_group).FirstOrDefault();
                if (app_vat_group == null)
                {
                    app_vat_group = sales_invoice_detail.app_vat_group;
                }
                foreach (var VatDetail in app_vat_group.app_vat_group_details)
                {
                    BusineesCenter DetailType = BusineesCenter.RevenueByService;
                    string Name = "Service";
                    if (sales_invoice_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        DetailType = BusineesCenter.FixedAsset;
                        Name = "Fixed Asset";
                    }
                    else if (sales_invoice_detail.item.id_item_type == item.item_type.Product
                        || sales_invoice_detail.item.id_item_type == item.item_type.RawMaterial
                        || sales_invoice_detail.item.id_item_type == item.item_type.Supplies)
                    {
                        DetailType = BusineesCenter.Asset_Inventory;
                        Name = "Product";
                    }

                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = sales_invoice_detail.unit_cost;
                    Detail.Value = (sales_invoice_detail.SubTotal_Vat) * (VatDetail.percentage);
                    Detail.VATPercentage = Convert.ToInt32(VatDetail.app_vat.coefficient * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }
        }

        public void LoadPurchase(purchase_invoice data, app_company app_company)
        {
            app_currency app_currency = null;
            if (data.app_currencyfx.app_currency != null)
            {
                app_currency = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault();
            }

            local_id = data.id_purchase_invoice;
            Type = InvoiceTypes.Purchase;
            SubType = InvoiceSubTypes.Invoice;
            SupplierName = data.contact.name;
            SupplierTaxID = data.contact.gov_code;
            CustomerName = app_company.name;
            CustomerTaxID = app_company.gov_code;
            Date = data.trans_date.ToUniversalTime().ToString("yyyy-MM-dd");
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date != null ? Convert.ToDateTime(data.app_document_range.expire_date).Date.ToString("yyyy-MM-dd") : null : null;
            PaymentCondition = data.app_contract != null ? data.app_contract.app_contract_detail.Max(x => x.interval) : 0;
            CurrencyCode = app_currency != null ? app_currency.code : CurrentSession.Currency_Default.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;
            IsImpex = data.is_impex;



            Details = new List<InvoiceDetail>();
            foreach (purchase_invoice_detail purchase_invoice_detail in data.purchase_invoice_detail)
            {
                app_vat_group app_vat_group = CurrentSession.VAT_Groups.Where(x => x.id_vat_group == purchase_invoice_detail.id_vat_group).FirstOrDefault();

                if (app_vat_group == null)
                {
                    app_vat_group = purchase_invoice_detail.app_vat_group;
                }

                foreach (var VatDetail in app_vat_group.app_vat_group_details)
                {
                    string Name = "";
                    BusineesCenter DetailType = BusineesCenter.RevenueByService;
                    using (db db = new db())
                    {
                        Name = db.app_cost_center.Find(purchase_invoice_detail.id_cost_center).name;
                    }


                    if (purchase_invoice_detail.item != null)
                    {
                        if (purchase_invoice_detail.item.id_item_type == item.item_type.FixedAssets)
                        {
                            DetailType = BusineesCenter.FixedAsset;
                        }
                        else if (purchase_invoice_detail.item.id_item_type == item.item_type.Product
                            || purchase_invoice_detail.item.id_item_type == item.item_type.RawMaterial
                            || purchase_invoice_detail.item.id_item_type == item.item_type.Supplies)
                        {
                            DetailType = BusineesCenter.Asset_Inventory;
                        }
                    }


                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = purchase_invoice_detail.unit_cost;
                    Detail.Value = (purchase_invoice_detail.SubTotal_Vat) * (VatDetail.percentage);
                    Detail.VATPercentage = Convert.ToInt32(app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient) * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }


        }

        public void LoadSalesReturn(sales_return data, app_company app_company)
        {
            app_currency app_currency = null;
            if (data.app_currencyfx.app_currency != null)
            {
                app_currency = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault();
            }
            local_id = data.id_sales_return;
            Type = InvoiceTypes.Sales;
            SubType = InvoiceSubTypes.Notes;
            CustomerName = data.contact.name;
            CustomerTaxID = data.contact.gov_code;
            SupplierName = app_company.name;
            SupplierTaxID = app_company.gov_code;
            Date = data.trans_date.Date.ToString("yyyy-MM-dd");
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date != null ? Convert.ToDateTime(data.app_document_range.expire_date).Date.ToString("yyyy-MM-dd") : null : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = app_currency != null ? app_currency.code : CurrentSession.Currency_Default.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;

            Details = new List<InvoiceDetail>();
            foreach (sales_return_detail sales_return_detail in data.sales_return_detail)
            {
                app_vat_group app_vat_group = CurrentSession.VAT_Groups.Where(x => x.id_vat_group == sales_return_detail.id_vat_group).FirstOrDefault();
                if (app_vat_group == null)
                {
                    app_vat_group = sales_return_detail.app_vat_group;
                }
                foreach (var VatDetail in app_vat_group.app_vat_group_details)
                {
                    BusineesCenter DetailType = BusineesCenter.RevenueByService;
                    string Name = "Service";
                    if (sales_return_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        DetailType = BusineesCenter.FixedAsset;
                        Name = "Fixedasset";
                    }
                    else if (sales_return_detail.item.id_item_type == item.item_type.Product
                        || sales_return_detail.item.id_item_type == item.item_type.RawMaterial
                        || sales_return_detail.item.id_item_type == item.item_type.Supplies)
                    {
                        DetailType = BusineesCenter.Asset_Inventory;
                        Name = "Product";
                    }

                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = sales_return_detail.unit_cost;
                    Detail.Value = (sales_return_detail.SubTotal_Vat) * (VatDetail.percentage);
                    Detail.VATPercentage = Convert.ToInt32(app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient) * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }
        }

        public void LoadPurchaseReturn(purchase_return data, app_company app_company)
        {
            app_currency app_currency = null;
            if (data.app_currencyfx.app_currency != null)
            {
                app_currency = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault();
            }
            local_id = data.id_purchase_return;
            Type = InvoiceTypes.Purchase;
            SubType = InvoiceSubTypes.Notes;
            SupplierName = data.contact.name;
            SupplierTaxID = data.contact.gov_code;
            CustomerName = app_company.name;
            CustomerTaxID = app_company.gov_code;
            Date = data.trans_date.Date.ToString("yyyy-MM-dd");
            Code = data.code;
            CodeExpiry = data.app_document_range != null ? data.app_document_range.expire_date != null ? Convert.ToDateTime(data.app_document_range.expire_date).Date.ToString("yyyy-MM-dd") : null : null;
            PaymentCondition = data.app_contract.app_contract_detail.Max(x => x.interval);
            CurrencyCode = app_currency != null ? app_currency.code : CurrentSession.Currency_Default.code;
            CurrencyRate = data.app_currencyfx.buy_value;
            Number = data.number;
            Comment = data.comment;

            Details = new List<InvoiceDetail>();
            foreach (purchase_return_detail purchase_return_detail in data.purchase_return_detail)
            {
                app_vat_group app_vat_group = CurrentSession.VAT_Groups.Where(x => x.id_vat_group == purchase_return_detail.id_vat_group).FirstOrDefault();
                if (app_vat_group == null)
                {
                    app_vat_group = purchase_return_detail.app_vat_group;
                }
                foreach (var VatDetail in app_vat_group.app_vat_group_details)
                {
                    string Name = "";
                    BusineesCenter DetailType = BusineesCenter.RevenueByService;
                    using (db db = new db())
                    {
                        Name = db.app_cost_center.Find(purchase_return_detail.id_cost_center).name;
                    }


                    if (purchase_return_detail.item != null)
                    {
                        if (purchase_return_detail.item.id_item_type == item.item_type.FixedAssets)
                        {
                            DetailType = BusineesCenter.FixedAsset;
                        }
                        else if (purchase_return_detail.item.id_item_type == item.item_type.Product
                            || purchase_return_detail.item.id_item_type == item.item_type.RawMaterial
                            || purchase_return_detail.item.id_item_type == item.item_type.Supplies)
                        {
                            DetailType = BusineesCenter.Asset_Inventory;
                        }
                    }

                    InvoiceDetail Detail = Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() != null ?
                        Details.Where(x => x.VATPercentage == VatDetail.app_vat.coefficient && x.Type == DetailType).FirstOrDefault() :
                        new InvoiceDetail();

                    Detail.Type = DetailType;
                    Detail.Cost = purchase_return_detail.unit_cost;
                    Detail.Value = (purchase_return_detail.SubTotal_Vat) * (VatDetail.percentage);
                    Detail.VATPercentage = Convert.ToInt32(app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient) * 100);
                    Detail.Name = Name;
                    Details.Add(Detail);
                }
            }
        }
    }

    public class InvoiceDetail
    {
        public BusineesCenter Type { get; set; }
        public Int32 VATPercentage { get; set; }
        public decimal Value { get; set; }
        public decimal Cost { get; set; }
        public string Name { get; set; }
    }

    public class Expenses
    {
        public InvoiceTypes Type { get; set; }
        public decimal Value { get; set; }
        public decimal Cost { get; set; }
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
    }

    public class AccountMovements
    {
        public AccountTypes Type { get; set; }
        public payment_type.payment_behaviours PaymentType { get; set; }
        public int local_id { get; set; }
        public int cloud_id { get; set; }
        public string CustomerTaxID { get; set; }
        public string CustomerName { get; set; }
        public string SupplierTaxID { get; set; }
        public string SupplierName { get; set; }
        public string AccountName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Date { get; set; }
        public string PaymentNumber { get; set; }
        public long? ReferenceInvoiceID { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Comment { get; set; }
        public string Message { get; set; }

        public void LoadPaymentsMade(payment_detail data, app_company app_company)
        {
            Type = AccountTypes.AccountPayable;
            PaymentType = data.payment_type.payment_behavior;
            CustomerName = app_company.name;
            CustomerTaxID = app_company.gov_code;
            SupplierName = data.payment.contact.name;
            SupplierTaxID = data.payment.contact.gov_code;

            InvoiceNumber = data.payment_schedual.First().purchase_invoice.number;
            Comment = data.comment;
            AccountName = data.app_account.name;
            Date = data.trans_date.Date.ToString("yyyy-MM-dd");
            CurrencyCode = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault().code;
            CurrencyRate = data.app_currencyfx.sell_value > 0 ? data.app_currencyfx.sell_value : data.app_currencyfx.buy_value;
            Debit = data.value;
            Credit = 0;

            //ReferenceInvoice = data.payment_detail.payment_schedual.First().purchase_invoice.number;
            ReferenceInvoiceID = (int)data.payment_schedual.First().id_purchase_invoice;
        }

        public void LoadPaymentsRecieved(payment_schedual data, app_company app_company)
        {
            sales_invoice sales_invoice = null;
            purchase_invoice purchase_invoice = null;
            purchase_return purchase_return = null;
            sales_return sales_return = null;

            string number = "";
            int id_company = 0;

            using (db db = new db())
            {
                contact contact = db.contacts.Find(data.id_contact);

                if (data.id_sales_invoice > 0)
                {
                    sales_invoice = db.sales_invoice.Find(data.id_sales_invoice);
                    number = sales_invoice.number;
                    InvoiceDate = sales_invoice.trans_date.Date.ToString("yyyy-MM-dd");
                    id_company = sales_invoice.id_company;
                    CustomerName = contact.name;
                    CustomerTaxID = contact.gov_code;
                    SupplierName = app_company.name;
                    SupplierTaxID = app_company.gov_code;

                }
                else if (data.id_purchase_invoice > 0)
                {
                    purchase_invoice = db.purchase_invoice.Find(data.id_purchase_invoice);
                    number = purchase_invoice.number;
                    InvoiceDate = purchase_invoice.trans_date.Date.ToString("yyyy-MM-dd");
                    id_company = purchase_invoice.id_company;
                    SupplierName = contact.name;
                    SupplierTaxID = contact.gov_code;
                    CustomerName = app_company.name;
                    CustomerTaxID = app_company.gov_code;
                }
                else if (data.id_purchase_return > 0)
                {
                    purchase_return = db.purchase_return.Find(data.id_purchase_return);
                    number = purchase_return.number;
                    InvoiceDate = purchase_return.trans_date.Date.ToString("yyyy-MM-dd");
                    id_company = purchase_return.id_company;
                    CustomerName = contact.name;
                    CustomerTaxID = contact.gov_code;
                    SupplierName = app_company.name;
                    SupplierTaxID = app_company.gov_code;
                }
                else if (data.id_sales_return > 0)
                {
                    sales_return = db.sales_return.Find(data.id_sales_return);
                    number = sales_return.number;
                    InvoiceDate = sales_return.trans_date.Date.ToString("yyyy-MM-dd");
                    id_company = sales_return.id_company;
                    SupplierName = contact.name;
                    SupplierTaxID = contact.gov_code;
                    CustomerName = app_company.name;
                    CustomerTaxID = app_company.gov_code;
                }

                PaymentType = db.payment_type.Find(data.payment_detail.id_payment_type).payment_behavior;
                local_id = data.id_payment_schedual;

                Type = data.credit > 0 ? AccountTypes.AccountReceivable : AccountTypes.AccountPayable;

                InvoiceNumber = number;
                Comment = data.payment_detail.comment;
                AccountName = data.payment_detail.app_account != null ? data.payment_detail.app_account.name : "Default Account";
                Date = data.trans_date != null ? data.trans_date.Date.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");
                app_currency currency = CurrentSession.Currencies.Where(x => x.id_currency == data.payment_detail.app_currencyfx.id_currency).FirstOrDefault();
                CurrencyCode = currency != null ? currency.code : CurrentSession.Currency_Default.code;
                CurrencyRate = data.credit > 0 ? data.payment_detail.app_currencyfx.buy_value : data.payment_detail.app_currencyfx.sell_value;
                //PaymentNumber = data.payment_detail.payment.number;

                Debit = data.debit; //: entity.Brillo.Currency.convert_Values(data.debit, data.id_currencyfx, data.payment_detail.id_currencyfx, App.Modules.Purchase);
                Credit = data.credit; // : entity.Brillo.Currency.convert_Values(data.credit, data.id_currencyfx, data.payment_detail.id_currencyfx, App.Modules.Sales);
            }
        }

        //Make another API for MoneyTransfers
        public void LoadTransfers(app_account_detail data, app_company app_company)
        {
            Type = AccountTypes.AccountPayable;
            local_id = data.id_account_detail;
            AccountName = data.app_account.name;
            CustomerName = app_company.name;
            CustomerTaxID = app_company.gov_code;
            Date = data.trans_date.Date.ToString("yyyy-MM-dd");
            app_currency currency = CurrentSession.Currencies.Where(x => x.id_currency == data.app_currencyfx.id_currency).FirstOrDefault();
            CurrencyCode = currency != null ? currency.code : CurrentSession.Currency_Default.code;
            CurrencyRate = data.credit > 0 ? data.app_currencyfx.buy_value : data.app_currencyfx.sell_value;
            Debit = data.debit;
            Credit = data.credit;
            Comment = data.comment;
        }
    }

    public class FixedAsset
    {
        public entity.item.item_type Type { get; set; }
        public int id { get; set; }
        public string ItemName { get; set; }
        public string TaxpayerTaxID { get; set; }
        public string TaxpayerName { get; set; }
        public string ItemCode { get; set; }
        public string ManufactureDate { get; set; }
        public string PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string CurrencyCode { get; set; }
        public string AssetGroup { get; set; }
        public decimal? LifeSpan { get; set; }

        public void LoadAsset(item_asset data, app_company app_company)
        {
            Type = item.item_type.FixedAssets;
            TaxpayerName = app_company.name;
            TaxpayerTaxID = app_company.gov_code;
            ItemName = data.item.name;
            ItemCode = data.item.code;
            id = data.id_item_asset;
            if (data.manufacture_date != null)
            {
                DateTime Manufacture_date = (DateTime)data.manufacture_date;
                ManufactureDate = Manufacture_date.ToString("yyyy-MM-dd");
            }

            if (data.purchase_date != null)
            {
                DateTime Purcahse_date = (DateTime)data.purchase_date;
                PurchaseDate = Purcahse_date.ToString("yyyy-MM-dd");
            }

            PurchaseValue = data.purchase_value != null ? (decimal)data.purchase_value : 0;
            CurrentValue = data.current_value != null ? (decimal)data.current_value : 0;
            CurrencyCode = data.app_currency != null ? data.app_currency.code : CurrentSession.Currency_Default.code;
            Quantity = data.quantity ?? 1;
            AssetGroup = data.item_asset_group != null ? data.item_asset_group.name : "";
            LifeSpan = data.item_asset_group != null ? data.item_asset_group.depreciation_rate : null;

        }


    }

    public class Production
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }

        //Input Data
        public BusineesCenter InputType { get; set; }
        public decimal InputCost { get; set; }

        //Output Data --> can be null
        public BusineesCenter? OutputType { get; set; }
        public decimal? OutputValue { get; set; }

        public void Load(production_execution_detail data)
        {
            Name = data.production_order_detail.production_order.name;
            Date = data.trans_date;

            if (data.item.id_item_type == item.item_type.Product ||
                data.item.id_item_type == item.item_type.RawMaterial ||
                data.item.id_item_type == item.item_type.Supplies)
            {
                InputType = BusineesCenter.Asset_Inventory;
            }
            else
            {
                InputType = BusineesCenter.RevenueByService;
            }

            InputCost = (data.unit_cost * data.quantity);

            item.item_type type = data.production_order_detail.item.id_item_type;

            if (type == item.item_type.Product ||
                type == item.item_type.RawMaterial ||
                type == item.item_type.Supplies)
            {
                OutputType = BusineesCenter.Asset_Inventory;
            }
        }
    }

    public class ResoponseAssetData
    {

        public int ref_id { get; set; }

        public int chart_id { get; set; }
        public int taxpayer_id { get; set; }
        public int currency_id { get; set; }
        public decimal rate { get; set; }
        public string serial { get; set; }
        public string name { get; set; }
        public decimal current_value { get; set; }
        public string purchase_date { get; set; }
        public decimal purchase_value { get; set; }
        public decimal? quantity { get; set; }
        public string sync_date { get; set; }
        public Chart chart { get; set; }

    }
    public class ResoponseData
    {

        public int ref_id { get; set; }



    }
    public class Chart
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal? asset_years { get; set; }
    }
}