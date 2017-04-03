using entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DebeHaber
{
    public enum TransactionTypes { Sales = 1, Purchase = 2, SalesReturn = 3, PurchaseReturn = 4 }

    public enum States { Approved = 1, Annuled = 2 }

    public enum CostCenterTypes { Expense = 1, Merchendice = 2, FixedAsset = 3, Income = 4, Production = 5 }

    public enum PaymentTypes { Normal = 1, CreditNote = 2, VATWithHolding = 3 }

    public class Integration
    {
        public Integration()
        {
            Transactions = new List<Transaction>();
        }

        public string Key { get; set; }
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

        public States State { get; set; }
        public DateTime TransDate { get; set; }
        public string Company { get; set; }
        public string Gov_Code { get; set; }
        public string Branch { get; set; }

        public string Comment { get; set; }
        public string Currency { get; set; }

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
            Company = sales_invoice.contact.name;

            if (sales_invoice.status == Status.Documents_General.Approved)
            {
                State = States.Approved;
            }
            else
            {
                State = States.Annuled;
            }

            Branch = sales_invoice.app_branch != null ? sales_invoice.app_branch.name : "";
            PaymentCondition = sales_invoice.app_contract != null ? (sales_invoice.app_contract.app_contract_detail != null ? sales_invoice.app_contract.app_contract_detail.Max(x => x.interval) : 0) : 0;

            Gov_Code = sales_invoice.contact.gov_code;
            Comment = sales_invoice.comment;
            Currency = sales_invoice.app_currencyfx != null ? sales_invoice.app_currencyfx.app_currency != null ? sales_invoice.app_currencyfx.app_currency.code : "" : "";

            DocNumber = sales_invoice.number;
            if (string.IsNullOrEmpty(sales_invoice.code))
            {
                DocCode = sales_invoice.app_document_range != null ? sales_invoice.app_document_range.code : "";
            }
            else
            {
                DocCode = sales_invoice.code;
            }

            DocExpiry = (sales_invoice.app_document_range != null ? sales_invoice.app_document_range.expire_date != null ? sales_invoice.app_document_range.expire_date : DateTime.Now : DateTime.Now);
        }

        public void Fill_BySalesReturn(sales_return sales_return)
        {
            Type = TransactionTypes.SalesReturn;
            TransDate = sales_return.trans_date;
            Company = sales_return.contact.name;
            Branch = sales_return.app_branch != null ? sales_return.app_branch.name : "";

            if (sales_return.status == Status.Documents_General.Approved)
            {
                State = States.Approved;
            }
            else
            {
                State = States.Annuled;
            }

            Gov_Code = sales_return.contact.gov_code;
            Comment = sales_return.comment;
            Currency = sales_return.app_currencyfx != null ? sales_return.app_currencyfx.app_currency != null ? sales_return.app_currencyfx.app_currency.code : "" : "";

            DocNumber = sales_return.number;
            DocCode = sales_return.app_document_range != null ? sales_return.app_document_range.code : "";
            DocExpiry = (sales_return.app_document_range != null ? (DateTime)sales_return.app_document_range.expire_date : DateTime.Now);
        }

        public void Fill_ByPurchase(purchase_invoice purchase_invoice)
        {
            Type = TransactionTypes.Purchase;
            TransDate = purchase_invoice.trans_date;
            Company = purchase_invoice.contact.name;
            Gov_Code = purchase_invoice.contact.gov_code;
            Branch = purchase_invoice.app_branch != null ? purchase_invoice.app_branch.name : "";

            if (purchase_invoice.status == Status.Documents_General.Approved)
            {
                State = States.Approved;
            }
            else
            {
                State = States.Annuled;
            }

            Comment = purchase_invoice.comment;
            Currency = purchase_invoice.app_currencyfx != null ? purchase_invoice.app_currencyfx.app_currency != null ? purchase_invoice.app_currencyfx.app_currency.code : "" : "";
            PaymentCondition = purchase_invoice.app_contract != null ? (purchase_invoice.app_contract.app_contract_detail != null ? purchase_invoice.app_contract.app_contract_detail.Max(x => x.interval) : 0) : 0;
            DocNumber = purchase_invoice.number;
            DocCode = purchase_invoice.code;
        }

        public void Fill_ByPurchaseReturn(purchase_return purchase_return)
        {
            Type = TransactionTypes.SalesReturn;
            TransDate = purchase_return.trans_date;
            Company = purchase_return.contact.name;

            if (purchase_return.status == Status.Documents_General.Approved)
            {
                State = States.Approved;
            }
            else
            {
                State = States.Annuled;
            }

            Gov_Code = purchase_return.contact.gov_code;
            Branch = purchase_return.app_branch != null ? purchase_return.app_branch.name : "";
            Comment = purchase_return.comment;
            Currency = purchase_return.app_currencyfx != null ? purchase_return.app_currencyfx.app_currency != null ? purchase_return.app_currencyfx.app_currency.code : "" : "";

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
                    app_cost_center app_cost_center = db.app_cost_center.Where(x => x.is_product).FirstOrDefault();
                    if (app_cost_center != null)
                    {
                        CC.Name = app_cost_center.name;
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

    public class Payments
    {
        public PaymentTypes PaymentType { get; set; }
        public DateTime TransDate { get; set; }
        public string Parent { get; set; }
        public string Company { get; set; }
        public string Gov_Code { get; set; }

        public string DocNumber { get; set; }
        public string DocCode { get; set; }
        public DateTime? DocExpiry { get; set; }

        public string Account { get; set; }
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

            Parent = schedual.parent.sales_invoice != null ? schedual.parent.sales_invoice.number : (schedual.parent.purchase_invoice != null ? schedual.parent.purchase_invoice.number : "");
            Company = schedual.payment_detail.payment.contact != null ? schedual.payment_detail.payment.contact.name : "";
            Gov_Code = schedual.payment_detail.payment.contact != null ? schedual.payment_detail.payment.contact.gov_code : "";
            DocCode = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.code : "";
            DocExpiry = schedual.payment_detail.payment.app_document_range != null ? schedual.payment_detail.payment.app_document_range.expire_date : DateTime.Now;
            DocNumber = schedual.payment_detail.payment.number;

            Account = schedual.payment_detail.app_account != null ? schedual.payment_detail.app_account.name : "";
            Value = schedual.payment_detail.value;
            Currency = schedual.payment_detail.app_currencyfx.app_currency.code;

            TransDate = schedual.payment_detail.payment.trans_date;
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

        public virtual List<FixedAsset> FixedAssets { get; set; }
    }

    public class Production
    {
        public Production()
        {
            Production_Detail = new List<Production_Detail>();
        }

        public string branch { get; set; }
        public string name { get; set; }
        public DateTime trans_date { get; set; }

        public virtual ICollection<Production_Detail> Production_Detail { get; set; }
    }

    public class Production_Detail
    {
        public Production_Detail()
        {
            CCListInput = new List<CostCenter>();
            CCListOutput = new List<CostCenter>();
        }

        public CostCenter cost_center_input { get; set; }
        public CostCenter cost_center_output { get; set; }
        public decimal value { get; set; }
        public string comment { get; set; }
        public DateTime trans_date { get; set; }

        //Collection Property
        public virtual ICollection<CostCenter> CCListInput { get; set; }

        //Collection Property
        public virtual ICollection<CostCenter> CCListOutput { get; set; }

        public void Fill_ByExecution(production_order_detail Detail, db db)
        {
            //Loop through each Execution multiplying Quantity by Unit Cost.
            foreach (production_execution_detail exe in Detail.production_execution_detail.Where(x => x.is_accounted == false))
            {
                value += exe.quantity * exe.unit_cost;
            }

            comment = Detail.name;
            trans_date = Detail.trans_date;

            CostCenter CCOutput = new CostCenter();
            CostCenter CCInput = new CostCenter();

            if (Detail.item != null)
            {
                //Run code for Input First. Most Common.
                if (Detail.is_input)
                {
                    //Input Code.
                    if (Detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        CCInput.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                        CCInput.Type = CostCenterTypes.FixedAsset;
                        CCListInput.Add(CCInput);
                    }
                    else if (
                        Detail.item.id_item_type == item.item_type.Product ||
                        Detail.item.id_item_type == item.item_type.RawMaterial ||
                        Detail.item.id_item_type == item.item_type.Supplies
                        )
                    {
                        app_cost_center app_cost_center = db.app_cost_center.Where(x => x.is_product).FirstOrDefault();

                        if (app_cost_center != null)
                        {
                            CCInput.Name = app_cost_center.name;
                            CCInput.Type = CostCenterTypes.Merchendice;
                        }
                        else
                        {
                            CCInput.Name = "Mercaderia";
                            CCInput.Type = CostCenterTypes.Merchendice;
                        }

                        //Add CostCenter into Detail.
                        CCListInput.Add(CCInput);
                    }
                    else
                    {
                        if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                        { CCInput.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                        else
                        { CCInput.Name = Detail.name; }

                        CCInput.Type = CostCenterTypes.Expense;

                        //Add CostCenter into Detail.
                        CCListInput.Add(CCInput);
                    }

                    ///Get Output for this Input
                    ///Run same code again.
                    production_order_detail parent_order = Detail.parent;
                    if (parent_order.item.id_item_type == item.item_type.FixedAssets)
                    {
                        CCOutput.Name = db.item_asset.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_asset_group.name : "";
                        CCOutput.Type = CostCenterTypes.FixedAsset;
                        CCListOutput.Add(CCOutput);
                    }
                    else if (parent_order.item.id_item_type == item.item_type.Product || parent_order.item.id_item_type == item.item_type.RawMaterial || parent_order.item.id_item_type == item.item_type.Supplies)
                    {
                        app_cost_center app_cost_center = db.app_cost_center.Where(x => x.is_product).FirstOrDefault();

                        if (app_cost_center != null)
                        {
                            CCOutput.Name = app_cost_center.name;
                            CCOutput.Type = CostCenterTypes.Merchendice;
                        }
                        else
                        {
                            CCOutput.Name = "Mercaderia";
                            CCOutput.Type = CostCenterTypes.Merchendice;
                        }

                        //Add CostCenter into Detail.
                        CCListOutput.Add(CCOutput);
                    }
                    else
                    {
                        if (db.items.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                        { CCOutput.Name = db.items.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                        else
                        { CCOutput.Name = parent_order.name; }

                        CCOutput.Type = CostCenterTypes.Expense;

                        //Add CostCenter into Detail.
                        CCListOutput.Add(CCOutput);
                    }
                }
                else
                {
                    string ProjectName = "";

                    if (Detail.project_task != null || Detail.id_project_task != null)
                    {
                        ProjectName = db.project_task.Where(x => x.id_project_task == Detail.id_project_task).Select(x => x.project.name).FirstOrDefault();
                    }

                    if (string.IsNullOrEmpty(ProjectName))
                    {
                        //If linked with Project, show project value.
                        project_task task = Detail.project_task;
                        project project = task.project;
                        CCOutput.Name = task.project.name;
                        CCOutput.Type = CostCenterTypes.Production;
                    }
                    else if (Detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        //Output Code.
                        CCOutput.Name = db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_asset_group.name : "";
                        CCOutput.Type = CostCenterTypes.FixedAsset;
                        CCListOutput.Add(CCOutput);
                    }
                    else if (Detail.item.id_item_type == item.item_type.Product || Detail.item.id_item_type == item.item_type.RawMaterial || Detail.item.id_item_type == item.item_type.Supplies)
                    {
                        app_cost_center app_cost_center = db.app_cost_center.Where(x => x.is_product).FirstOrDefault();

                        if (app_cost_center != null)
                        {
                            CCOutput.Name = app_cost_center.name;
                            CCOutput.Type = CostCenterTypes.Merchendice;
                        }
                        else
                        {
                            CCOutput.Name = "Mercaderia";
                            CCOutput.Type = CostCenterTypes.Merchendice;
                        }

                        //Add CostCenter into Detail.
                        CCListOutput.Add(CCOutput);
                    }
                    else
                    {
                        if (db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                        { CCOutput.Name = db.items.Where(x => x.id_item == Detail.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                        else
                        { CCOutput.Name = Detail.name; }

                        CCOutput.Type = CostCenterTypes.Expense;

                        //Add CostCenter into Detail.
                        CCListOutput.Add(CCOutput);
                    }

                    ///Get Output for this Input
                    ///Run same code again.
                    production_order_detail parent_order = Detail.parent;
                    if (parent_order.item.id_item_type == item.item_type.FixedAssets)
                    {
                        CCInput.Name = db.item_asset.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_asset_group != null ? db.item_asset.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_asset_group.name : "";
                        CCInput.Type = CostCenterTypes.FixedAsset;
                        CCListInput.Add(CCInput);
                    }
                    else if (parent_order.item.id_item_type == item.item_type.Product || parent_order.item.id_item_type == item.item_type.RawMaterial || parent_order.item.id_item_type == item.item_type.Supplies)
                    {
                        app_cost_center app_cost_center = db.app_cost_center.Where(x => x.is_product).FirstOrDefault();

                        if (app_cost_center != null)
                        {
                            CCInput.Name = app_cost_center.name;
                            CCInput.Type = CostCenterTypes.Merchendice;
                        }
                        else
                        {
                            CCInput.Name = "Mercaderia";
                            CCInput.Type = CostCenterTypes.Merchendice;
                        }

                        //Add CostCenter into Detail.
                        CCListInput.Add(CCInput);
                    }
                    else
                    {
                        if (db.items.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault() != null)
                        { CCInput.Name = db.items.Where(x => x.id_item == parent_order.id_item).FirstOrDefault().item_tag_detail.FirstOrDefault().item_tag.name; }
                        else
                        { CCInput.Name = parent_order.name; }

                        CCInput.Type = CostCenterTypes.Expense;

                        //Add CostCenter into Detail.
                        CCListInput.Add(CCInput);
                    }
                }
            }
        }
    }
}