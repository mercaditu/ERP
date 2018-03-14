using entity;
using System;
using System.Collections.Generic;
using System.Data;
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
            Transactions = new List<DebeHaber.SyncLatest.Transaction>();
        }

        public string Key { get; set; }
        public string GovCode { get; set; }

        public virtual ICollection<DebeHaber.SyncLatest.Transaction> Transactions { get; set; }
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
            CommercialInvoice_Detail = new List<DebeHaber.SyncLatest.CommercialInvoice_Detail>();

        }

        public int id { get; set; }
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
        public virtual ICollection<DebeHaber.SyncLatest.CommercialInvoice_Detail> CommercialInvoice_Detail { get; set; }

        
        //Fill Methods
        public void Fill_BySales(DataRow sales_invoice)
        {
            Type = TransactionTypes.Sales;
            id = Convert.ToInt32(sales_invoice["id_sales_invoice"]);
            date = Convert.ToDateTime(sales_invoice["date"]);
            customerTaxID = sales_invoice["customerTaxID"].ToString();
            customerName = sales_invoice["customerName"].ToString();
            supplierTaxID = sales_invoice["supplierTaxID"].ToString();
            supplierName = sales_invoice["supplierName"].ToString();
            currencyCode = sales_invoice["currencyCode"].ToString();
            paymentCondition = Convert.ToInt32(sales_invoice["paymentCondition"]);
           
            number = Convert.ToString(sales_invoice["number"]);
            
            comment = Convert.ToString(sales_invoice["comment"]);
            


        }

      
    }

    public class CommercialInvoice_Detail
    {
        public CommercialInvoice_Detail()
        {
           
        }
        public int id { get; set; }

        public string chart { get; set; }
        public decimal value { get; set; }
        public string vat { get; set; }

        //Nav Property
        public virtual Commercial_Invoice Commercial_Invoice { get; set; }

        //Collection Property
       

        #region Methods

        public void Fill_BySales(DataRow Detail)
        {
            id = Convert.ToInt32(Detail["id"]);
            chart = Detail["chart"].ToString();
            value =Convert.ToDecimal(Detail["value"]);
            vat = Detail["vat"].ToString();


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
        public PaymentModes PaymentMode { get; set; }
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

            PaymentMode = schedual.parent.sales_invoice != null ? PaymentModes.Recievable : PaymentModes.Payable;
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
            trans_date = Detail.trans_date;
            comment = Detail.name;

            //Loop through each Execution multiplying Quantity by Unit Cost.
            foreach (production_execution_detail exe in Detail.production_execution_detail.Where(x => x.is_accounted == false))
            {
                value += exe.quantity * exe.unit_cost;
                trans_date = exe.trans_date;
            }

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


    public class Web_Data
    {
       

        public int id { get; set; }
        public int type { get; set; }
        public int customer_id { get; set; }
        public int supplier_id { get; set; }
        public int document_id { get; set; }
        public int currency_id { get; set; }
        public int rate { get; set; }
        public string payment_condition { get; set; }
        public int chart_account_id { get; set; }
        public string number { get; set; }
        public int ref_id { get; set; }





    }
}