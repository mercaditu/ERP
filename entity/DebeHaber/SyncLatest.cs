using entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DebeHaber.SyncLatest
{
    public enum TransactionTypes { Sales = 1, Purchase = 2, SalesReturn = 3, PurchaseReturn = 4 }

    public enum States { Approved = 1, Annuled = 2 }

    public enum CostCenterTypes { Expense = 1, Merchendice = 2, FixedAsset = 3, Income = 4, Production = 5 }

    public enum PaymentTypes { Normal = 1, CreditNote = 2, VATWithHolding = 3 }
    
    public enum PaymentModes { Recievable = 1, Payable = 2 }

    public class Integration
    {
        public Integration()
        {
            Transactions = new List<Transaction>();
        }

        public string Key { get; set; }
        public string GovCode { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        public Transaction()
        {
            Commercial_Invoices = new List<Commercial_Invoice>();
            Payments = new List<Payments>();
            FixedAssetGroups = new List<FixedAssetGroup>();
            Production = new List<Production>();
        }

        public virtual ICollection<Commercial_Invoice> Commercial_Invoices { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual ICollection<FixedAssetGroup> FixedAssetGroups { get; set; }
        public virtual ICollection<Production> Production { get; set; }
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
     
        public string customerTaxID { get; set; }
        public string customerName { get; set; }

        public string supplierTaxID { get; set; }
        public string supplierName { get; set; }

        public string currencyCode { get; set; }
        public int paymentCondition { get; set; }

        public string account { get; set; }
        public DateTime date { get; set; }

        //Invoice Documents
        public string number { get; set; }

        public string code { get; set; }

        public string comment { get; set; }

        public DateTime? code_expiry { get; set; }

        

        //Collection Property
        public virtual ICollection<CommercialInvoice_Detail> CommercialInvoice_Detail { get; set; }

        public virtual ICollection<Payments> Payments { get; set; }

        //Fill Methods
        public void Fill_BySales(sales_invoice sales_invoice)
        {
            Type = TransactionTypes.Sales;
            date = sales_invoice.trans_date;
          


           
            paymentCondition = sales_invoice.app_contract != null ? (sales_invoice.app_contract.app_contract_detail != null ? sales_invoice.app_contract.app_contract_detail.Max(x => x.interval) : 0) : 0;

            customerTaxID = sales_invoice.contact.gov_code;
            customerName = sales_invoice.contact.name;
            comment = sales_invoice.comment;
            currencyCode = sales_invoice.app_currencyfx != null ? sales_invoice.app_currencyfx.app_currency != null ? sales_invoice.app_currencyfx.app_currency.code : "" : "";

            number = sales_invoice.number;
            if (string.IsNullOrEmpty(sales_invoice.code))
            {
                code = sales_invoice.app_document_range != null ? sales_invoice.app_document_range.code : "";
            }
            else
            {
                code = sales_invoice.code;
            }

            code_expiry = (sales_invoice.app_document_range != null ? sales_invoice.app_document_range.expire_date != null ? sales_invoice.app_document_range.expire_date : DateTime.Now : DateTime.Now);
        }

        //public void Fill_BySalesReturn(sales_return sales_return)
        //{
        //    Type = TransactionTypes.SalesReturn;
        //    TransDate = sales_return.trans_date;
        //    Company = sales_return.contact.name;
        //    Branch = sales_return.app_branch != null ? sales_return.app_branch.name : "";

        //    if (sales_return.status == Status.Documents_General.Approved)
        //    {
        //        State = States.Approved;
        //    }
        //    else
        //    {
        //        State = States.Annuled;
        //    }

        //    Gov_Code = sales_return.contact.gov_code;
        //    Comment = sales_return.comment;
        //    Currency = sales_return.app_currencyfx != null ? sales_return.app_currencyfx.app_currency != null ? sales_return.app_currencyfx.app_currency.code : "" : "";

        //    DocNumber = sales_return.number;
        //    DocCode = sales_return.app_document_range != null ? sales_return.app_document_range.code : "";
        //    DocExpiry = (sales_return.app_document_range != null ? (DateTime)sales_return.app_document_range.expire_date : DateTime.Now);
        //}

        //public void Fill_ByPurchase(purchase_invoice purchase_invoice)
        //{
        //    Type = TransactionTypes.Purchase;
        //    TransDate = purchase_invoice.trans_date;
        //    Company = purchase_invoice.contact.name;
        //    Gov_Code = purchase_invoice.contact.gov_code;
        //    Branch = purchase_invoice.app_branch != null ? purchase_invoice.app_branch.name : "";

        //    if (purchase_invoice.status == Status.Documents_General.Approved)
        //    {
        //        State = States.Approved;
        //    }
        //    else
        //    {
        //        State = States.Annuled;
        //    }

        //    Comment = purchase_invoice.comment;
        //    Currency = purchase_invoice.app_currencyfx != null ? purchase_invoice.app_currencyfx.app_currency != null ? purchase_invoice.app_currencyfx.app_currency.code : "" : "";
        //    PaymentCondition = purchase_invoice.app_contract != null ? (purchase_invoice.app_contract.app_contract_detail != null ? purchase_invoice.app_contract.app_contract_detail.Max(x => x.interval) : 0) : 0;
        //    DocNumber = purchase_invoice.number;
        //    DocCode = purchase_invoice.code;
        //}

        //public void Fill_ByPurchaseReturn(purchase_return purchase_return)
        //{
        //    Type = TransactionTypes.PurchaseReturn;
        //    TransDate = purchase_return.trans_date;
        //    Company = purchase_return.contact.name;

        //    if (purchase_return.status == Status.Documents_General.Approved)
        //    {
        //        State = States.Approved;
        //    }
        //    else
        //    {
        //        State = States.Annuled;
        //    }

        //    Gov_Code = purchase_return.contact.gov_code;
        //    Branch = purchase_return.app_branch != null ? purchase_return.app_branch.name : "";
        //    Comment = purchase_return.comment;
        //    Currency = purchase_return.app_currencyfx != null ? purchase_return.app_currencyfx.app_currency != null ? purchase_return.app_currencyfx.app_currency.code : "" : "";

        //    DocNumber = purchase_return.number;
        //    DocCode = purchase_return.app_document_range != null ? purchase_return.app_document_range.code : "";
        //    DocExpiry = (purchase_return.app_document_range != null ? (DateTime)purchase_return.app_document_range.expire_date : DateTime.Now);
        //}
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

        #region Methods

        public void Fill_BySales(sales_invoice_detail Detail, db db)
        {
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CC = new CostCenter();

            // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
            if (Detail.item.id_item_type == item.item_type.FixedAssets)
            {
                CC.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                CC.Type = CostCenterTypes.FixedAsset;

                //Add CostCenter into Detail.
                CostCenter.Add(CC);
            }
            // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
            else if (Detail.item.id_item_type == item.item_type.Service || Detail.item.id_item_type == item.item_type.Task || Detail.item.id_item_type == item.item_type.ServiceContract)
            {
                if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                { CC.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                else
                { CC.Name = Detail.item_description; }

                CC.Type = CostCenterTypes.Income;

                //Add CostCenter into Detail.
                CostCenter.Add(CC);
            }
            // Finally if all else fails, assume Item being sold is Merchendice.
            else
            {
                if (db.app_cost_center.Where(x => x.is_product).FirstOrDefault() != null)
                {
                    CC.Name = db.app_cost_center.Where(x => x.is_product).Select(x => x.name).FirstOrDefault();
                    CC.Type = CostCenterTypes.Merchendice;
                }
                else
                {
                    CC.Name = "Mercaderia";
                    CC.Type = CostCenterTypes.Merchendice;
                }
                //Add CostCenter into Detail.
                CostCenter.Add(CC);
            }
        }

        public void Fill_BySalesReturn(sales_return_detail Detail, db db)
        {
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CC = new CostCenter();

            // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
            if (Detail.item.id_item_type == item.item_type.FixedAssets)
            {
                CC.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                CC.Type = CostCenterTypes.FixedAsset;

                //Add CostCenter into Detail.
                CostCenter.Add(CC);
            }
            // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
            else if (Detail.item.id_item_type == item.item_type.Service || Detail.item.id_item_type == item.item_type.Task || Detail.item.id_item_type == item.item_type.ServiceContract)
            {
                if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                { CC.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                else
                { CC.Name = Detail.item_description; }

                CC.Type = CostCenterTypes.Income;

                //Add CostCenter into Detail.
                CostCenter.Add(CC);
            }
            // Finally if all else fails, assume Item being sold is Merchendice.
            else
            {
                if (db.app_cost_center.Where(x => x.is_product).FirstOrDefault() != null)
                {
                    CC.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                    CC.Type = CostCenterTypes.Merchendice;
                }
                else
                {
                    CC.Name = "Mercaderia";
                    CC.Type = CostCenterTypes.Merchendice;
                }
                //Add CostCenter into Detail.
                CostCenter.Add(CC);
            }
        }

        public void Fill_ByPurchase(purchase_invoice_detail Detail, db db)
        {
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CC = new CostCenter();

            if (Detail.app_cost_center != null)
            {
                CC.Name = Detail.app_cost_center.name;
                
                //Verify Type and Assign.
                if (Detail.app_cost_center.is_fixedasset)
                {
                    CC.Type = CostCenterTypes.FixedAsset;
                }
                else if (Detail.app_cost_center.is_product)
                {
                    CC.Type = CostCenterTypes.Merchendice;
                }
                else
                {
                    CC.Type = CostCenterTypes.Expense;
                }
            }
            else
            {
                CC.Name = "Gasto No Controlado";
                CC.Type = CostCenterTypes.Expense;
            }

            //Add CostCenter into Detail.
            CostCenter.Add(CC);
        }

        public void Fill_ByPurchaseReturn(purchase_return_detail Detail, db db)
        {
            VAT_Coeficient = Detail.app_vat_group.app_vat_group_details.Sum(x => x.app_vat.coefficient);
            UnitValue_WithVAT = Detail.SubTotal_Vat;
            Comment = Detail.item_description;

            CostCenter CC = new CostCenter();

            //Check if Purchase has Item. If not its an expense.
            if (Detail.item != null)
            {
                // If Item being sold is FixedAsset, get Cost Center will be the GroupName.
                if (Detail.item.id_item_type == item.item_type.FixedAssets)
                {
                    CC.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                    CC.Type = CostCenterTypes.FixedAsset;

                    //Add CostCenter into Detail.
                    CostCenter.Add(CC);
                }
                // If Item being sold is a Service, Contract, or Task. Take it as Direct Revenue.
                else if (Detail.item.id_item_type == item.item_type.Service || Detail.item.id_item_type == item.item_type.Task || Detail.item.id_item_type == item.item_type.ServiceContract)
                {
                    if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                    { CC.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                    else
                    { CC.Name = Detail.item_description; }

                    CC.Type = CostCenterTypes.Income;

                    //Add CostCenter into Detail.
                    CostCenter.Add(CC);
                }
                // Finally if all else fails, assume Item being sold is Merchendice.
                else
                {
                    if (db.app_cost_center.Where(x => x.is_product).FirstOrDefault() != null)
                    {
                        CC.Name = db.app_cost_center.Where(x => x.is_product).FirstOrDefault().name;
                        CC.Type = CostCenterTypes.Merchendice;
                    }
                    else
                    {
                        CC.Name = "Mercaderia";
                        CC.Type = CostCenterTypes.Merchendice;
                    }

                    //Add CostCenter into Detail.
                    CostCenter.Add(CC);
                }
            }
            else
            {
                CC.Name = db.app_cost_center.Where(x => x.is_administrative).FirstOrDefault().name;
                CC.Type = CostCenterTypes.Expense;

                //Add CostCenter into Detail.
                CostCenter.Add(CC);
            }
        }

        #endregion Methods
    }

    public class CostCenter
    {
        public CostCenterTypes Type { get; set; }
        public string Name { get; set; }

        public CommercialInvoice_Detail CommercialInvoice_Detail { get; set; }
        public Production_Detail Production_Detail { get; set; }
    }

   


}