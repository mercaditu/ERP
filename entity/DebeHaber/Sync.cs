using System;
using System.Collections.Generic;
using System.Linq;
using entity;

namespace DebeHaber
{
    public enum TransactionTypes { Sales = 1, Purchase = 2, SalesReturn = 3, PurchaseReturn = 4}
    public enum CostCenterTypes { Expense = 1, Merchendice = 2, FixedAsset = 3, Income = 4 }
    public enum PaymentTypes { Normal = 1, CreditNote = 2, VATWithHolding = 3 }

    public class Transactions
    {
        public Transactions()
        {
            Commercial_Invoice = new List<Commercial_Invoice>();
            Payments = new List<Payments>();
            FixedAssetGroup = new List<FixedAssetGroup>();
        }

        public string HashIntegration { get; set; }

        public virtual ICollection<Commercial_Invoice> Commercial_Invoice { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual ICollection<FixedAssetGroup> FixedAssetGroup { get; set; }
    }

    public class Commercial_Invoice
    {
        public Commercial_Invoice()
        {
            CommercialInvoice_Detail = new List<CommercialInvoice_Detail>();
            Payments = new List<Payments>();
        }
        
        //Invoice Data
        public TransactionTypes Type { get; set; }
        public DateTime TransDate { get; set; }
        public string CompanyName {get;set;}
        public string Gov_Code { get; set; }
        public string BranchName { get; set; }

        public string Comment { get; set; }
        public string CurrencyName { get; set; }

        //Invoice Documents
        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }

        public int PaymentCondition { get; set; }

        //Collection Property
        public virtual ICollection<CommercialInvoice_Detail> CommercialInvoice_Detail { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }

        //Fill Methods
        public void Fill_BySales(sales_invoice sales_invoice)
        {
            Type = TransactionTypes.Sales;
            TransDate = sales_invoice.trans_date;
            CompanyName = sales_invoice.contact.name;
            BranchName = sales_invoice.app_branch != null ? sales_invoice.app_branch.name : "";
            PaymentCondition = sales_invoice.app_contract != null ? (sales_invoice.app_contract.app_contract_detail != null ? sales_invoice.app_contract.app_contract_detail.Max(x => x.interval) : 0) : 0;

            Gov_Code = sales_invoice.contact.gov_code;
            Comment = sales_invoice.comment;
            CurrencyName = sales_invoice.app_currencyfx != null ? sales_invoice.app_currencyfx.app_currency != null ? sales_invoice.app_currencyfx.app_currency.name : "" : "";

            DocNumber = sales_invoice.number;
            DocCode = sales_invoice.app_document_range != null ? sales_invoice.app_document_range.code : "";
            DocExpiry = (sales_invoice.app_document_range != null ? (DateTime)sales_invoice.app_document_range.expire_date : DateTime.Now);
        }

        public void Fill_BySalesReturn(sales_return sales_return)
        {
            Type = TransactionTypes.SalesReturn;
            TransDate = sales_return.trans_date;
            CompanyName = sales_return.contact.name;
            BranchName = sales_return.app_branch != null ? sales_return.app_branch.name : "";

            Gov_Code = sales_return.contact.gov_code;
            Comment = sales_return.comment;
            CurrencyName = sales_return.app_currencyfx != null ? sales_return.app_currencyfx.app_currency != null ? sales_return.app_currencyfx.app_currency.name : "" : "";

            DocNumber = sales_return.number;
            DocCode = sales_return.app_document_range != null ? sales_return.app_document_range.code : "";
            DocExpiry = (sales_return.app_document_range != null ? (DateTime)sales_return.app_document_range.expire_date : DateTime.Now);
        }

        public void Fill_ByPurchase(purchase_invoice purchase_invoice)
        {
            Type = TransactionTypes.Purchase;
            TransDate = purchase_invoice.trans_date;
            CompanyName = purchase_invoice.contact.name;
            Gov_Code = purchase_invoice.contact.gov_code;
            BranchName = purchase_invoice.app_branch != null ? purchase_invoice.app_branch.name : "";

            Comment = purchase_invoice.comment;
            CurrencyName = purchase_invoice.app_currencyfx != null ? purchase_invoice.app_currencyfx.app_currency != null ? purchase_invoice.app_currencyfx.app_currency.name : "" : "";
            PaymentCondition = purchase_invoice.app_contract != null ? (purchase_invoice.app_contract.app_contract_detail != null ? purchase_invoice.app_contract.app_contract_detail.Max(x => x.interval) : 0) : 0;
            DocNumber = purchase_invoice.number;
            DocCode = purchase_invoice.code;
        }

        public void Fill_ByPurchaseReturn(purchase_return purchase_return)
        {
            Type = TransactionTypes.SalesReturn;
            TransDate = purchase_return.trans_date;
            CompanyName = purchase_return.contact.name;

            Gov_Code = purchase_return.contact.gov_code;
            BranchName = purchase_return.app_branch != null ? purchase_return.app_branch.name : "";
            Comment = purchase_return.comment;
            CurrencyName = purchase_return.app_currencyfx != null ? purchase_return.app_currencyfx.app_currency != null ? purchase_return.app_currencyfx.app_currency.name : "" : "";

            DocNumber = purchase_return.number;
            DocCode = purchase_return.app_document_range != null ? purchase_return.app_document_range.code : "";
            DocExpiry = (purchase_return.app_document_range != null ? (DateTime)purchase_return.app_document_range.expire_date : DateTime.Now);
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
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CostCenter = new CostCenter();

            // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
            if (Detail.item.id_item_type == item.item_type.FixedAssets)
            {
                CostCenter.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                CostCenter.Type = CostCenterTypes.FixedAsset;

                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
            // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
            else if (Detail.item.id_item_type == item.item_type.Service || Detail.item.id_item_type == item.item_type.Task || Detail.item.id_item_type == item.item_type.ServiceContract)
            {
                if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                { CostCenter.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                else
                { CostCenter.Name = Detail.item_description; }

                CostCenter.Type = CostCenterTypes.Income;

                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
            // Finally if all else fails, assume Item being sold is Merchendice.
            else
            {
                if (db.app_cost_center.Where(x => x.is_product).FirstOrDefault() != null)
                {
                    CostCenter.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                    CostCenter.Type = CostCenterTypes.Merchendice;
                }
                else
                {
                    CostCenter.Name = "Mercaderia";
                    CostCenter.Type = CostCenterTypes.Merchendice;
                }
                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
        }

        public void Fill_BySalesReturn(sales_return_detail Detail, db db)
        {
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CostCenter = new CostCenter();

            // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
            if (Detail.item.id_item_type == item.item_type.FixedAssets)
            {
                CostCenter.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                CostCenter.Type = CostCenterTypes.FixedAsset;

                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
            // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
            else if (Detail.item.id_item_type == item.item_type.Service || Detail.item.id_item_type == item.item_type.Task || Detail.item.id_item_type == item.item_type.ServiceContract)
            {
                if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                { CostCenter.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                else
                { CostCenter.Name = Detail.item_description; }

                CostCenter.Type = CostCenterTypes.Income;

                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
            // Finally if all else fails, assume Item being sold is Merchendice.
            else
            {
                if (db.app_cost_center.Where(x => x.is_product).FirstOrDefault() != null)
                {
                    CostCenter.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                    CostCenter.Type = CostCenterTypes.Merchendice;
                }
                else
                {
                    CostCenter.Name = "Mercaderia";
                    CostCenter.Type = CostCenterTypes.Merchendice;
                }
                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
        }

        public void Fill_ByPurchase(purchase_invoice_detail Detail, db db)
        {
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CostCenter = new CostCenter();

            //Check if Purchase has Item. If not its an expense.
            if (Detail.item != null)
            {
                // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
                if (Detail.item.id_item_type == item.item_type.FixedAssets)
                {
                    CostCenter.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                    CostCenter.Type = CostCenterTypes.FixedAsset;

                    //Add CostCenter into Detail.
                    Detail.costce.Add(CostCenter);
                }
                // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
                else if (Detail.item.id_item_type == item.item_type.Service || Detail.item.id_item_type == item.item_type.Task || Detail.item.id_item_type == item.item_type.ServiceContract)
                {
                    if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                    { CostCenter.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                    else
                    { CostCenter.Name = Detail.item_description; }

                    CostCenter.Type = CostCenterTypes.Income;

                    //Add CostCenter into Detail.
                    CostCenter.Add(CostCenter);
                }
                // Finally if all else fails, assume Item being sold is Merchendice.
                else
                {
                    if (db.app_cost_center.Where(x => x.is_product).FirstOrDefault() != null)
                    {
                        CostCenter.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                        CostCenter.Type = CostCenterTypes.Merchendice;
                    }
                    else
                    {
                        CostCenter.Name = "Mercaderia";
                        CostCenter.Type = CostCenterTypes.Merchendice;
                    }

                    //Add CostCenter into Detail.
                    CostCenter.Add(CostCenter);
                }
            }
            else
            {
                CostCenter.Name = db.app_cost_center.Where(x => x.is_administrative).FirstOrDefault().name;
                CostCenter.Type = CostCenterTypes.Expense;

                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
        }

        public void Fill_ByPurchaseReturn(purchase_return_detail Detail, db db)
        {
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CostCenter = new CostCenter();

            //Check if Purchase has Item. If not its an expense.
            if (Detail.item != null)
            {
                // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
                if (Detail.item.id_item_type == item.item_type.FixedAssets)
                {
                    CostCenter.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                    CostCenter.Type = CostCenterTypes.FixedAsset;

                    //Add CostCenter into Detail.
                    CostCenter.Add(CostCenter);
                }
                // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
                else if (Detail.item.id_item_type == item.item_type.Service || Detail.item.id_item_type == item.item_type.Task || Detail.item.id_item_type == item.item_type.ServiceContract)
                {
                    if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                    { CostCenter.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                    else
                    { CostCenter.Name = Detail.item_description; }

                    CostCenter.Type = CostCenterTypes.Income;

                    //Add CostCenter into Detail.
                    CostCenter.Add(CostCenter);
                }
                // Finally if all else fails, assume Item being sold is Merchendice.
                else
                {
                    if (db.app_cost_center.Where(x => x.is_product).FirstOrDefault() != null)
                    {
                        CostCenter.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                        CostCenter.Type = CostCenterTypes.Merchendice;
                    }
                    else
                    {
                        CostCenter.Name = "Mercaderia";
                        CostCenter.Type = CostCenterTypes.Merchendice;
                    }

                    //Add CostCenter into Detail.
                    CostCenter.Add(CostCenter);
                }
            }
            else
            {
                CostCenter.Name = db.app_cost_center.Where(x => x.is_administrative).FirstOrDefault().name;
                CostCenter.Type = CostCenterTypes.Expense;

                //Add CostCenter into Detail.
                CostCenter.Add(CostCenter);
            }
        }
    }

    public class CostCenter
    {
        public CostCenterTypes Type { get; set; }
        public string Name { get; set; }

        public CommercialInvoice_Detail CommercialInvoice_Detail { get; set; }
    }

    public class Payments
    {
        public PaymentTypes PaymentType { get; set; }
        public DateTime TransDate { get; set; }
        public string Parent { get; set; }
        public string CompanyName { get; set; }
        public string Gov_Code { get; set; }

        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }

        public string  Account { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }

        public void FillPayments(payment_schedual schedual)
        {
            PaymentType = PaymentTypes.Normal;

            if (schedual.payment_detail.payment_type.payment_behavior == payment_type.payment_behaviours.CreditNote)
            {
                PaymentType = PaymentTypes.CreditNote;
            }
            else if (schedual.payment_detail.payment_type.payment_behavior == payment_type.payment_behaviours.WithHoldingVAT)
            {
                PaymentType = PaymentTypes.VATWithHolding;
            }

            Parent = schedual.parent.sales_invoice != null ? schedual.parent.sales_invoice.number : (schedual.parent.purchase_invoice != null ? schedual.parent.purchase_invoice.number : "") ;
            CompanyName = schedual.payment_detail.payment.contact != null ? schedual.payment_detail.payment.contact.name : "";
            Gov_Code = schedual.payment_detail.payment.contact != null ? schedual.payment_detail.payment.contact.gov_code : "";
            DocCode = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.code : "";
            DocExpiry = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.expire_date : DateTime.Now;
            DocNumber = schedual.payment_detail.payment.number;

            Account = schedual.payment_detail.app_account != null ? schedual.payment_detail.app_account.name : "";
            Value = schedual.payment_detail.value;
            Currency = schedual.payment_detail.app_currencyfx.app_currency.name;

            TransDate = schedual.payment_detail.payment.trans_date;
            Account = schedual.payment_detail.app_account.name;
        }
    }

    public class FixedAsset
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal CurrentCost { get; set; }

        public string CurrencyName { get; set; }
        public virtual FixedAssetGroup FixedAssetGroup { get; set; }
    }

    public class FixedAssetGroup
    {
        public FixedAssetGroup()
        {
            FixedAssets = new List<FixedAsset>();
        }

        public string Name { get; set; }
        public decimal LifespanYears { get; set; }

        public virtual List<FixedAsset> FixedAssets {get;set;}
    }

    public class Vat
    {
        public int Type { get; set; }
        public decimal Coef { get; set; }
        public decimal ValueWVAT { get; set; }
        public string CostCenter { get; set; }
        public string CostCenterType { get; set; }
    }
}
