using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Linq;
using entity;
using System;

namespace entity.Brillo.Document
{
    public class DataSource
    {
        ReportDataSource reportDataSource = new ReportDataSource();

        public ReportDataSource Create(object Document)
        {
            if (Document.GetType().BaseType == typeof(sales_invoice) || Document.GetType() == typeof(sales_invoice))
            {
                sales_invoice sales_invoice = (sales_invoice)Document;
                return SalesInvoice(sales_invoice);
            }
            else if (Document.GetType().BaseType == typeof(sales_order) || Document.GetType() == typeof(sales_order))
            {
                sales_order sales_order = (sales_order)Document;
                return SalesOrder(sales_order);
            }
            else if (Document.GetType().BaseType == typeof(sales_budget) || Document.GetType() == typeof(sales_budget))
            {
                sales_budget sales_budget = (sales_budget)Document;
                return SalesBudget(sales_budget);
            }
            else if (Document.GetType().BaseType == typeof(sales_packing) || Document.GetType() == typeof(sales_packing))
            {
                sales_packing sales_packing = (sales_packing)Document;
                return Sales_PackingList(sales_packing);
            }
            else if (Document.GetType().BaseType == typeof(sales_return) || Document.GetType() == typeof(sales_return))
            {
                sales_return sales_return = (sales_return)Document;
                return SalesReturn(sales_return);
            }
            else if (Document.GetType().BaseType == typeof(purchase_order) || Document.GetType() == typeof(purchase_order))
            {
                purchase_order purchase_order = (purchase_order)Document;
                return PurchaseOrder(purchase_order);
            }
            else if (Document.GetType().BaseType == typeof(purchase_return) || Document.GetType() == typeof(purchase_return))
            {
                purchase_return purchase_return = (purchase_return)Document;
                return PurchaseReturn(purchase_return);
            }
            else if (Document.GetType().BaseType == typeof(payment_promissory_note) || Document.GetType() == typeof(payment_promissory_note))
            {
                payment_promissory_note payment_promissory_note = (payment_promissory_note)Document;
                return PromissoryNote(payment_promissory_note);
            }
            else if (Document.GetType().BaseType == typeof(purchase_tender_contact) || Document.GetType() == typeof(purchase_tender_contact))
            {
                purchase_tender_contact purchase_tender_contact = (purchase_tender_contact)Document;
                return PurchaseTender(purchase_tender_contact);
            }
            else if (Document.GetType() == typeof(item_transfer) || Document.GetType().BaseType == typeof(item_transfer))
            {
                item_transfer item_transfer = (item_transfer)Document;
                return ItemTransfer(item_transfer);
            }
            else if (Document.GetType() == typeof(payment) || Document.GetType() == typeof(payment))
            {
                payment payment = (payment)Document;
                return Payment(payment);
            }
            else if (Document.GetType().BaseType == typeof(project) || Document.GetType() == typeof(project))
            {
                project project = (project)Document;
                return Project(project);
            }
            else if (Document.GetType() == typeof(item_inventory) || Document.GetType().BaseType == typeof(item_inventory))
            {
                item_inventory item_inventory = (item_inventory)Document;
                return Inventory(item_inventory);
            }

            return null;
        }

        public ReportDataSource SalesBudget(sales_budget sales_budget)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<sales_budget_detail> sales_budget_detail = sales_budget.sales_budget_detail.ToList();

                reportDataSource.Value = sales_budget_detail
                    .Select(g => new
                    {
                        geo_name = g.sales_budget.contact.app_geography != null ? g.sales_budget.contact.app_geography.name != null ? g.sales_budget.contact.app_geography.name : "" : "",
                        id_sales_budget = g.id_sales_budget,
                        id_sales_budget_detail = g.id_sales_budget_detail,
                        sales_budget = g.id_sales_budget_detail,
                        id_company = g.id_company,
                        add1 = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.address != null ? g.sales_budget.contact.address : "" : "" : "",
                        telephone = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.telephone != null ? g.sales_budget.contact.telephone : "" : "" : "",
                        email = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.email != null ? g.sales_budget.contact.email : "" : "" : "",
                        company_name = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.name != null ? g.sales_budget.contact.name : "" : "" : "",
                        currency = g.sales_budget != null ? g.sales_budget.app_currencyfx != null ? g.sales_budget.app_currencyfx.app_currency != null ? g.sales_budget.app_currencyfx.app_currency.name != null ? g.sales_budget.app_currencyfx.app_currency.name : "" : "" : "" : "",
                        currencyfx_rate = g.sales_budget != null ? g.sales_budget.app_currencyfx != null ? g.sales_budget.app_currencyfx.sell_value != null ? g.sales_budget.app_currencyfx.sell_value : 0 : 0 : 0,
                        item_code = g.item != null ? g.item.code != null ? g.item.code : "" : "",
                        item_description = g.item != null ? g.item.name != null ? g.item.name : "" : "",
                        item_long_description = g.item != null ? g.item.description != null ? g.item.description : "" : "",
                        item_brand = g.item != null ? (g.item.item_brand != null ? g.item.item_brand.name != null ? g.item.item_brand.name : "" : "") : "",
                        quantity = g.quantity,
                        sub_Total = g.SubTotal,
                        sub_Total_vat = g.SubTotal_Vat,
                        unit_cost = g.unit_cost,
                        unit_price = g.unit_cost,
                        unit_price_vat = g.UnitPrice_Vat,
                        unit_price_discount = g.discount,

                        terminale_name = g.sales_budget != null ? (g.sales_budget.app_terminal != null ? g.sales_budget.app_terminal.name != null ? g.sales_budget.app_terminal.name : "" : "") : "",
                        code = g.sales_budget != null ? g.sales_budget.code != null ? g.sales_budget.code : "" : "",
                        contact_name = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.name != null ? g.sales_budget.contact.name : "" : "" : "",
                        sales_rep_name = g.sales_budget != null ? g.sales_budget.sales_rep != null ? g.sales_budget.sales_rep.name != null ? g.sales_budget.sales_rep.name : "" : "" : "",
                        trans_date = g.sales_budget != null ? g.sales_budget.trans_date != null ? g.sales_budget.trans_date.ToShortDateString() : "" : "",
                        id_vat_group = g.id_vat_group,
                        gov_id = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.gov_code != null ? g.sales_budget.contact.gov_code : "" : "" : "",
                        contract = g.sales_budget != null ? g.sales_budget.app_contract != null ? g.sales_budget.app_contract.name != null ? g.sales_budget.app_contract.name : "" : "" : "",
                        condition = g.sales_budget != null ? g.sales_budget.app_condition != null ? g.sales_budget.app_condition.name != null ? g.sales_budget.app_condition.name : "" : "" : "",
                        Number = g.sales_budget != null ? g.sales_budget.number != null ? g.sales_budget.number : "" : "",
                        comment = g.sales_budget != null ? g.sales_budget.comment != null ? g.sales_budget.comment : "" : "",
                        security_user_name = g.sales_budget != null ? g.sales_budget.security_user != null ? g.sales_budget.security_user.name != null ? g.sales_budget.security_user.name : "" : "" : "",
                        AmountWords = g.sales_budget != null ? g.sales_budget.app_currencyfx != null ? g.sales_budget.app_currencyfx.app_currency != null ? g.sales_budget.app_currencyfx.app_currency.has_rounding ?

                        // Text -> Words
                        NumToWords.IntToText(Convert.ToInt64(g.sales_budget != null ? g.sales_budget.GrandTotal : 0))
                        :
                        NumToWords.DecimalToText((Convert.ToDecimal(g.sales_budget != null ? g.sales_budget.GrandTotal : 0))) : "" : "" : "",

                        HasRounding = g.sales_budget != null ? g.sales_budget.app_currencyfx != null ? g.sales_budget.app_currencyfx.app_currency != null ? g.sales_budget.app_currencyfx.app_currency.has_rounding != null ? g.sales_budget.app_currencyfx.app_currency.has_rounding : false : false : false : false,
                        //unit_price_discount = g.discount != null ? g.discount : 0,

                    }).ToList();

                return reportDataSource;
            }
        }

        public ReportDataSource SalesOrder(sales_order sales_order)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<sales_order_detail> sales_order_detail = sales_order.sales_order_detail.ToList();

                reportDataSource.Value = sales_order_detail
                    .Select(g => new
                    {

                        geo_name = g.sales_order != null ? g.sales_order.contact != null ? g.sales_order.contact.app_geography != null ? g.sales_order.contact.app_geography.name : "" : "" : "",
                        sales_budget_number = g.sales_budget_detail != null ? g.sales_budget_detail.sales_budget.number : "",
                        contact_name = g.sales_order != null ? g.sales_order.contact.name : "",
                        customer_address = g.sales_order != null ? g.sales_order.contact.address != null ? g.sales_order.contact.address : "" : "",
                        customer_telephone = g.sales_order != null ? g.sales_order.contact.telephone : "",
                        customer_email = g.sales_order != null ? g.sales_order.contact.email != null ? g.sales_order.contact.email : "" : "",
                        company_Name = g.sales_order != null ? g.sales_order.app_company != null ? g.sales_order.app_company.name : "" : "",
                        customer_govid = g.sales_order != null ? g.sales_order.contact.gov_code : "",
                        sales_order_terminal = g.sales_order != null ? g.sales_order.app_terminal != null ? g.sales_order.app_terminal.name : "" : "",
                        branch_Name = g.sales_order != null ? g.sales_order.app_branch != null ? g.sales_order.app_branch.name : "" : "",
                        order_Code = g.sales_order != null ? g.sales_order.code != null ? g.sales_order.code : "" : "",
                        delivery_Date = g.sales_order != null ? g.sales_order.delivery_date != null ? g.sales_order.delivery_date : DateTime.Now : DateTime.Now,
                        sales_number = g.sales_order != null ? g.sales_order.number : "",
                        order_Total = g.sales_order != null ? g.sales_order.GrandTotal : 0M,
                        currency = g.sales_order != null ? g.sales_order.app_currencyfx != null ? g.sales_order.app_currencyfx.app_currency != null ? g.sales_order.app_currencyfx.app_currency.name : "" : "" : "",
                        currencyfx_rate = g.sales_order != null ? g.sales_order.app_currencyfx != null ? g.sales_order.app_currencyfx.sell_value : 0M : 0M,
                        project_Name = g.sales_order != null ? g.sales_order.project != null ? g.sales_order.project.name : "" : "",
                        sales_order_representative = g.sales_order != null ? g.sales_order.sales_rep != null ? g.sales_order.sales_rep.name : "" : "",
                        security_user_name = g.sales_order != null ? g.sales_order.security_user != null ? g.sales_order.security_user.name : "" : "",
                        trans_date = g.sales_order != null ? g.sales_order.trans_date : DateTime.Now,
                        sales_order_contract = g.sales_order != null ? g.sales_order.app_contract != null ? g.sales_order.app_contract.name : "" : "",
                        sales_order_condition = g.sales_order != null ? g.sales_order.app_condition != null ? g.sales_order.app_condition.name : "" : "",
                        DeliveryDate = g.sales_order != null ? g.sales_order.delivery_date : DateTime.Now,
                        sales_order_Comment = g.sales_order != null ? g.sales_order.comment != null ? g.sales_order.comment : "" : "",
                        vat_group_name = g.app_vat_group != null ? g.app_vat_group.name : "",
                        contract = g.sales_order != null ? g.sales_order.app_contract != null ? g.sales_order.app_contract.name != null ? g.sales_order.app_contract.name : "" : "" : "",
                        condition = g.sales_order != null ? g.sales_order.app_condition != null ? g.sales_order.app_condition.name != null ? g.sales_order.app_condition.name : "" : "" : "",
                        item_code = g.item != null ? g.item.code : "",
                        item_description = g.item != null ? g.item.name : "",
                        item_brand = g.item != null ? g.item.item_brand != null ? g.item.item_brand.name : "" : "",

                        quantity = g.quantity,
                        sub_Total = g.SubTotal,
                        sub_Total_vat = g.SubTotal_Vat,
                        unit_cost = g.unit_cost,
                        unit_price = g.unit_price,
                        unit_price_vat = g.UnitPrice_Vat,
                        sub_total_vat_discount = g.Discount_SubTotal_Vat,
                        AmountWords = g.sales_order != null ? g.sales_order.app_currencyfx != null ? g.sales_order.app_currencyfx.app_currency != null ? g.sales_order.app_currencyfx.app_currency.has_rounding ?

                        // Text -> Words
                        NumToWords.IntToText(Convert.ToInt64(g.sales_order != null ? g.sales_order.GrandTotal : 0))
                        :
                        NumToWords.DecimalToText((Convert.ToDecimal(g.sales_order != null ? g.sales_order.GrandTotal : 0))) : "" : "" : "",

                        HasRounding = g.sales_order != null ? g.sales_order.app_currencyfx != null ? g.sales_order.app_currencyfx.app_currency != null ? g.sales_order.app_currencyfx.app_currency.has_rounding != null ? g.sales_order.app_currencyfx.app_currency.has_rounding : false : false : false : false,
                        unit_price_discount = g.discount != null ? g.discount : 0,

                    }).ToList();

                return reportDataSource;
            }
        }

        public ReportDataSource SalesInvoice(sales_invoice sales_invoice)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1";
                List<sales_invoice_detail> sales_invoice_detail = sales_invoice.sales_invoice_detail.ToList();
                if (sales_invoice_detail.Count < sales_invoice.app_document_range.app_document.line_limit)
                {
                    for (int i = sales_invoice_detail.Count; i < sales_invoice.app_document_range.app_document.line_limit; i++)
                    {
                        sales_invoice_detail _sales_invoice_detail = new entity.sales_invoice_detail();
                        sales_invoice_detail.Add(_sales_invoice_detail);
                    }
                }

                reportDataSource.Value = sales_invoice_detail.Select(g => new
                {
                    geo_name = g.sales_invoice != null ? g.sales_invoice.contact.app_geography != null ? g.sales_invoice.contact.app_geography.name : "" : "",
                    sales_invoice = g.sales_invoice != null ? g.sales_invoice.id_sales_invoice : 0,
                    id_company = g.id_company,
                    add1 = g.sales_invoice != null ? g.sales_invoice.contact.address != null ? g.sales_invoice.contact.address : "" : "",
                    telephone = g.sales_invoice != null ? g.sales_invoice.contact.telephone != null ? g.sales_invoice.contact.telephone : "" : "",
                    email = g.sales_invoice != null ? g.sales_invoice.contact.email != null ? g.sales_invoice.contact.email : "" : "",
                    company_name = g.sales_invoice != null ? g.sales_invoice.app_company != null ? g.sales_invoice.app_company.name : "" : "",
                    item_code = g.item != null ? g.item.code : "",
                    item_name = g.item != null ? g.item.name : "",
                    item_description = g.item_description,
                    Description = g.item != null ? g.item.item_brand != null ? g.item.item_brand.name : "" : "",
                    currency = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.app_currency.name : "",
                    currencyfx_rate = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.sell_value : 0,
                    quantity = g.quantity,
                    sub_Total = g.SubTotal,
                    sub_Total_vat = g.SubTotal_Vat,
                    sub_Total_Vat_Discount = g.Discount_SubTotal_Vat,
                    unit_cost = g.unit_cost,
                    unit_price = g.unit_price,
                    unit_price_vat = g.UnitPrice_Vat,
                    terminal_name = g.sales_invoice != null ? g.sales_invoice.app_terminal != null ? g.sales_invoice.app_terminal.name : "" : "",
                    code = g.sales_invoice != null ? g.sales_invoice.code != null ? g.sales_invoice.code : "" : "",
                    customer_contact_name = g.sales_invoice != null ? g.sales_invoice.contact.name : "",
                    customer_code = g.sales_invoice != null ? g.sales_invoice.contact.code : "",
                    customer_alias = g.sales_invoice != null ? g.sales_invoice.contact.alias : "",
                    project_name = g.sales_invoice != null ? g.sales_invoice.project != null ? g.sales_invoice.project.name : "" : "",
                    sales_invoice_rep_name = g.sales_invoice != null ? g.sales_invoice.sales_rep != null ? g.sales_invoice.sales_rep.name : "" : "",
                    trans_date = g.sales_invoice != null ? g.sales_invoice.trans_date.ToString() : "",
                    id_vat_group = g.id_vat_group,
                    gov_id = g.sales_invoice != null ? g.sales_invoice.contact.gov_code : "",
                    sales_invoice_contract = g.sales_invoice != null ? g.sales_invoice.app_contract.name : "",
                    sales_invoice_condition = g.sales_invoice != null ? g.sales_invoice.app_contract.app_condition.name : "",
                    sales_number = g.sales_invoice != null ? g.sales_invoice.number : "",
                    sales_barcode = g.sales_invoice != null ? GetBarcode(g.sales_invoice.number) : "",
                    sales_invoice_Comment = g.sales_invoice != null ? g.sales_invoice.comment : "",
                    packingList = g.sales_invoice != null ? g.sales_packing_relation != null ? GetPacking(g.sales_packing_relation.ToList()) : "" : "",
                    sales_order = g.sales_invoice != null ? g.sales_order_detail != null ? g.sales_order_detail.sales_order.number : "" : "",
                    AmountWords = g.sales_invoice != null ? g.sales_invoice.app_currencyfx != null ? g.sales_invoice.app_currencyfx.app_currency != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding ?

                    // Text -> Words
                    NumToWords.IntToText(Convert.ToInt64(g.sales_invoice != null ? g.sales_invoice.GrandTotal : 0))
                    :
                    NumToWords.DecimalToText((Convert.ToDecimal(g.sales_invoice != null ? g.sales_invoice.GrandTotal : 0))) : "" : "" : "",

                    HasRounding = g.sales_invoice != null ? g.sales_invoice.app_currencyfx != null ? g.sales_invoice.app_currencyfx.app_currency != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding : false : false : false : false,
                    unit_price_discount = g.discount != null ? g.discount : 0,
                }).ToList();

                return reportDataSource;
            }
        }
        private string GetBarcode(string number)
        {
            entity.Class.clsBarcode clsbarcode = new Class.clsBarcode();
           return clsbarcode.ConvertToBarcode(number);
        }
        private string GetPacking(List<sales_packing_relation> sales_packing_relation)
        {
            string PackingList = "";
            if (sales_packing_relation.Count > 0)
            {
                foreach (sales_packing_relation _sales_packing_relation in sales_packing_relation)
                {
                    if (!PackingList.Contains(_sales_packing_relation.sales_packing_detail.sales_packing.number))
                    {
                        PackingList = PackingList + ", " + _sales_packing_relation.sales_packing_detail.sales_packing.number;
                    }
                }
                return PackingList.Remove(0, 1);
            }

            return PackingList;
        }


        public ReportDataSource Sales_PackingList(sales_packing sales_packing)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1";
                List<sales_packing_detail> sales_packing_detail = sales_packing.sales_packing_detail.ToList();
                reportDataSource.Value = sales_packing_detail
                              .Select(g => new
                              {
                                  contact_name = g.sales_packing.contact != null ? g.sales_packing.contact.name != null ? g.sales_packing.contact.name : "" : "",
                                  customer_govid = g.sales_packing.contact != null ? g.sales_packing.contact.gov_code != null ? g.sales_packing.contact.gov_code : "" : "",
                                  customer_address = g.sales_packing.contact != null ? g.sales_packing.contact.address != null ? g.sales_packing.contact.address : "" : "",
                                  customer_telephone = g.sales_packing.contact != null ? g.sales_packing.contact.telephone != null ? g.sales_packing.contact.telephone : "" : "",
                                  customer_email = g.sales_packing.contact != null ? g.sales_packing.contact.email != null ? g.sales_packing.contact.email : "" : "",
                                  company_Name = g.app_company != null ? g.app_company.name != null ? g.app_company.name : "" : "",
                                  company_govid = g.app_company != null ? g.app_company.gov_code != null ? g.app_company.gov_code : "" : "",
                                  company_address = g.app_company != null ? g.app_company.address != null ? g.app_company.address : "" : "",
                                  sales_terminal = g.sales_packing != null ? g.sales_packing.app_terminal != null ? g.sales_packing.app_terminal.name != null ? g.sales_packing.app_terminal.name : "" : "" : "",
                                  branch_Name = g.sales_packing != null ? g.sales_packing.app_branch != null ? g.sales_packing.app_branch.name != null ? g.sales_packing.app_branch.name : "" : "" : "",
                                  security_user_name = g.sales_packing != null ? g.sales_packing.security_user != null ? g.sales_packing.security_user.name != null ? g.sales_packing.security_user.name : "" : "" : "",
                                  trans_date = g.sales_packing.trans_date != null ? g.sales_packing.trans_date : DateTime.Now,
                                  sales_order_Comment = g.sales_packing.comment != null ? g.sales_packing.comment : "",
                                  item_code = g.item != null ? g.item.code != null ? g.item.code : "" : "",
                                  item_description = g.item != null ? g.item.name != null ? g.item.name : "" : "",
                                  item_brand = g.item != null ? g.item.item_brand != null ? g.item.item_brand.name != null ? g.item.item_brand.name : "" : "" : "",
                                  quantity = g.quantity != null ? g.quantity : 0,
                                  number = g.sales_packing != null ? g.sales_packing.number : "",
                                  sales_invoice_number = g.sales_packing != null ? g.sales_packing_relation != null ? GetInvoice(g.sales_packing_relation.ToList()) : "" : "",
                                  packing_type = g.sales_packing != null ? g.sales_packing.packing_type.ToString() : ""
                              }).ToList();

                return reportDataSource;
            }
        }
        private string GetInvoice(List<sales_packing_relation> sales_packing_relation)
        {
            string PackingList = "";
            foreach (sales_packing_relation _sales_packing_relation in sales_packing_relation)
            {
                if (PackingList.Contains(_sales_packing_relation.sales_invoice_detail.sales_invoice.number))
                {
                    PackingList = PackingList + ", " + _sales_packing_relation.sales_invoice_detail.sales_invoice.number;
                }
            }
            return PackingList;
        }
        public ReportDataSource SalesReturn(sales_return sales_return)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<sales_return_detail> sales_return_detail = sales_return.sales_return_detail.ToList();

                reportDataSource.Value = sales_return_detail
                              .Select(g => new
                              {
                                  geo_name = g.sales_return.contact.app_geography != null ? g.sales_return.contact.app_geography.name : "",
                                  id_sales_return = g.id_sales_return,
                                  id_sales_return_detail = g.id_sales_return_detail,
                                  sales_return = g.id_sales_return_detail,
                                  id_company = g.id_company,
                                  add1 = g.sales_return.contact.address != null ? g.sales_return.contact.address : "",
                                  telephone = g.sales_return.contact.telephone != null ? g.sales_return.contact.telephone : "",
                                  email = g.sales_return.contact.email != null ? g.sales_return.contact.email : "",
                                  company_name = g.app_company != null ? g.app_company.name : "",
                                  item_code = g.item.code,
                                  item_description = g.item.name,
                                  Description = g.item.item_brand != null ? g.item.item_brand.name : "",
                                  quantity = g.quantity,
                                  sub_Total = g.SubTotal,
                                  sub_Total_vat = g.SubTotal_Vat,
                                  unit_cost = g.unit_cost,
                                  unit_price = g.unit_cost,
                                  unit_price_vat = g.UnitPrice_Vat,
                                  sales_invoice_number = g.sales_invoice_detail != null ? g.sales_invoice_detail.sales_invoice != null ? g.sales_invoice_detail.sales_invoice.number : "" : "",
                                  salesman = g.sales_invoice_detail != null ? g.sales_invoice_detail.sales_invoice != null ? g.sales_invoice_detail.sales_invoice.sales_rep != null ? g.sales_invoice_detail.sales_invoice.sales_rep.name : "" : "" : "",
                                  terminale_name = g.sales_return.app_terminal != null ? g.sales_return.app_terminal.name : "",
                                  code = g.sales_return.code,
                                  contact_name = g.sales_return.contact.name,
                                  trans_date = g.sales_return.trans_date,
                                  id_vat_group = g.id_vat_group,
                                  gov_id = g.sales_return.contact.gov_code,
                                  AmountWords = g.sales_return != null ? g.sales_return.app_currencyfx != null ? g.sales_return.app_currencyfx.app_currency != null ? g.sales_return.app_currencyfx.app_currency.has_rounding ?

                     // Text -> Words
                     NumToWords.IntToText(Convert.ToInt32(g.sales_return != null ? g.sales_return.GrandTotal : 0))
                     :
                     NumToWords.DecimalToText((Convert.ToDecimal(g.sales_return != null ? g.sales_return.GrandTotal : 0))) : "" : "" : "",

                                  HasRounding = g.sales_return != null ? g.sales_return.app_currencyfx != null ? g.sales_return.app_currencyfx.app_currency != null ? g.sales_return.app_currencyfx.app_currency.has_rounding != null ? g.sales_return.app_currencyfx.app_currency.has_rounding : false : false : false : false,
                                  unit_price_discount = g.discount != null ? g.discount : 0,
                              }).ToList();

                return reportDataSource;
            }
        }

        /// <summary>
        /// NOT FINSIHED
        /// </summary>
        /// <param name="purchase_tender"></param>
        /// <returns></returns>
        public ReportDataSource PurchaseTender(purchase_tender_contact purchase_tender_contact)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<purchase_tender_detail> purchase_tender_detail = purchase_tender_contact.purchase_tender_detail.ToList();

                reportDataSource.Value = purchase_tender_detail
                    .Select(g => new
                    {
                        add1 = g.purchase_tender_contact != null ? g.purchase_tender_contact.contact != null ? g.purchase_tender_contact.contact.address : "" : "",
                        telephone = g.purchase_tender_contact != null ? g.purchase_tender_contact.contact != null ? g.purchase_tender_contact.contact.telephone : "" : "",
                        email = g.purchase_tender_contact != null ? g.purchase_tender_contact.contact != null ? g.purchase_tender_contact.contact.email : "" : "",
                        company_name = g.app_company != null ? g.app_company.name : "",
                        item_code = g.purchase_tender_item != null ? g.purchase_tender_item.item != null ? g.purchase_tender_item.item.code : "" : "",
                        item_description = g.purchase_tender_item != null ? g.purchase_tender_item.item != null ? g.purchase_tender_item.item.name : "" : "",
                        Brand = g.purchase_tender_item != null ? g.purchase_tender_item.item != null ? g.purchase_tender_item.item.item_brand != null ? g.purchase_tender_item.item.item_brand.name : "" : "" : "",
                        quantity = g.quantity,
                        sub_Total = g.SubTotal,
                        sub_Total_vat = g.SubTotal_Vat,
                        unit_cost = g.unit_cost,
                        unit_price = g.unit_cost,
                        unit_price_vat = g.UnitCost_Vat,
                        branch_name = g.purchase_tender_contact != null ? g.purchase_tender_contact.purchase_tender != null ? g.purchase_tender_contact.purchase_tender.app_branch != null ? g.purchase_tender_contact.purchase_tender.app_branch.name : "" : "" : "",
                        terminal_name = g.purchase_tender_contact != null ? g.purchase_tender_contact.purchase_tender != null ? g.purchase_tender_contact.purchase_tender.app_terminal != null ? g.purchase_tender_contact.purchase_tender.app_terminal.name : "" : "" : "",
                        Condition = g.purchase_tender_contact != null ? g.purchase_tender_contact.app_condition != null ? g.purchase_tender_contact.app_condition.name : "" : "",
                        Contract = g.purchase_tender_contact != null ? g.purchase_tender_contact.app_contract != null ? g.purchase_tender_contact.app_contract.name : "" : "",
                        Currency = g.purchase_tender_contact != null ? g.purchase_tender_contact.app_currencyfx != null ? g.purchase_tender_contact.app_currencyfx.app_currency != null ? g.purchase_tender_contact.app_currencyfx.app_currency.name : "" : "" : "",
                        code = g.purchase_tender_contact != null ? g.purchase_tender_contact.purchase_tender != null ? g.purchase_tender_contact.purchase_tender.code.ToString() : "" : "",
                        contact_name = g.purchase_tender_contact != null ? g.purchase_tender_contact.contact != null ? g.purchase_tender_contact.contact.name : "" : "",
                        trans_date = g.purchase_tender_contact != null ? g.purchase_tender_contact.purchase_tender != null ? g.purchase_tender_contact.purchase_tender.trans_date : DateTime.Now : DateTime.Now,
                        id_vat_group = g.id_vat_group,
                        gov_id = g.purchase_tender_contact != null ? g.purchase_tender_contact.contact != null ? g.purchase_tender_contact.contact.gov_code : "" : "",
                        Number = g.purchase_tender_contact != null ? g.purchase_tender_contact.purchase_tender != null ? g.purchase_tender_contact.purchase_tender.number != null ? g.purchase_tender_contact.purchase_tender.number.ToString() : "" : "" : "",
                        DimensionString = g.DimensionString,
                        AmountWords = g.purchase_tender_contact != null ? g.purchase_tender_contact.app_currencyfx != null ? g.purchase_tender_contact.app_currencyfx.app_currency != null ? g.purchase_tender_contact.app_currencyfx.app_currency.has_rounding ?

                     // Text -> Words
                     NumToWords.IntToText(Convert.ToInt32(g.purchase_tender_contact != null ? g.purchase_tender_contact.GrandTotal : 0))
                     :
                     NumToWords.DecimalToText((Convert.ToDecimal(g.purchase_tender_contact != null ? g.purchase_tender_contact.GrandTotal : 0))) : "" : "" : "",

                        HasRounding = g.purchase_tender_contact != null ? g.purchase_tender_contact.app_currencyfx != null ? g.purchase_tender_contact.app_currencyfx.app_currency != null ? g.purchase_tender_contact.app_currencyfx.app_currency.has_rounding != null ? g.purchase_tender_contact.app_currencyfx.app_currency.has_rounding : false : false : false : false
                    }).ToList();
                return reportDataSource;
            }
        }

        public ReportDataSource PurchaseOrder(purchase_order purchase_order)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<purchase_order_detail> purchase_order_detail = purchase_order.purchase_order_detail.ToList();

                reportDataSource.Value = purchase_order_detail
                    .Select(g => new
                    {
                        id_sales_return = g.id_purchase_order,
                        id_sales_return_detail = g.id_purchase_order,
                        sales_return = g.id_purchase_order_detail,
                        id_company = g.id_company,
                        add1 = g.purchase_order != null ? g.purchase_order.contact.address : "",
                        telephone = g.purchase_order != null ? g.purchase_order.contact.telephone : "",
                        email = g.purchase_order != null ? g.purchase_order.contact.email : "",
                        company_name = g.app_company != null ? g.app_company.name : "",
                        item_code = g.item != null ? g.item.code : "",
                        item_description = g.item_description,
                        Brand = g.item != null ? g.item.item_brand != null ? g.item.item_brand.name : "" : "",
                        quantity = g.quantity,
                        sub_Total = g.SubTotal,
                        sub_Total_vat = g.SubTotal_Vat,
                        unit_cost = g.unit_cost,
                        unit_price = g.unit_cost,
                        unit_price_vat = g.UnitCost_Vat,
                        terminal_name = g.purchase_order != null ? g.purchase_order.app_terminal.name : "",
                        Condition = g.purchase_order != null ? g.purchase_order.app_condition.name : "",
                        Contract = g.purchase_order != null ? g.purchase_order.app_contract.name : "",
                        Currency = g.purchase_order != null ? g.purchase_order.app_currencyfx.app_currency.name : "",
                        code = g.purchase_order.code,
                        contact_name = g.purchase_order != null ? g.purchase_order.contact.name : "",
                        trans_date = g.purchase_order.trans_date,
                        id_vat_group = g.id_vat_group,
                        vat_group_name = g.app_vat_group != null ? g.app_vat_group.name : "",
                        gov_id = g.purchase_order.contact.gov_code,
                        Number = g.purchase_order.number,
                        AmountWords = g.purchase_order != null ? g.purchase_order.app_currencyfx != null ? g.purchase_order.app_currencyfx.app_currency != null ? g.purchase_order.app_currencyfx.app_currency.has_rounding ?

                     // Text -> Words
                     NumToWords.IntToText(Convert.ToInt32(g.purchase_order != null ? g.purchase_order.GrandTotal : 0))
                     :
                     NumToWords.DecimalToText((Convert.ToDecimal(g.purchase_order != null ? g.purchase_order.GrandTotal : 0))) : "" : "" : "",

                        HasRounding = g.purchase_order != null ? g.purchase_order.app_currencyfx != null ? g.purchase_order.app_currencyfx.app_currency != null ? g.purchase_order.app_currencyfx.app_currency.has_rounding != null ? g.purchase_order.app_currencyfx.app_currency.has_rounding : false : false : false : false
                    }).ToList();

                return reportDataSource;
            }
        }

        /// <summary>
        /// NOT FINSIHED
        /// </summary>
        /// <param name="purchase_return"></param>
        /// <returns></returns>
        public ReportDataSource PurchaseReturn(purchase_return purchase_return)
        {
            return reportDataSource;
        }

        public ReportDataSource ItemTransfer(item_transfer item_transfer)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<item_transfer_detail> item_transfer_detail = item_transfer.item_transfer_detail.ToList();

                reportDataSource.Value = item_transfer_detail
                    .Select(g => new
                    {
                        transfer_number = g.item_transfer.number,
                        location_origin_name = g.item_transfer.app_location_origin.name,
                        location_destination_name = g.item_transfer.app_location_destination.name,
                        item_code = g.item_product.item.code,
                        quantity_origin = g.quantity_origin,
                        item_name = g.item_product.item.name,
                        trans_date = g.item_transfer.trans_date,
                        comment = g.item_transfer.comment
                    }).ToList();

                return reportDataSource;
            }
        }

        public ReportDataSource PromissoryNote(payment_promissory_note payment_promissory_note)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                reportDataSource.Value = payment_promissory_note.payment_schedual
                              .Select(g => new
                              {
                                  Customer = g.contact != null ? g.contact.name : "",
                                  CustomerGovID = g.contact != null ? g.contact.gov_code : "",
                                  CustomerAddress = g.contact != null ? g.contact.address : "",
                                  CustomerTelephone = g.contact != null ? g.contact.telephone : "",
                                  Value = g.payment_promissory_note.value,
                                  Currency = g.app_currencyfx.app_currency.name,
                                  TransDate = g.payment_promissory_note.trans_date,
                                  ExpiryDate = g.payment_promissory_note.expiry_date,
                                  Relation = g.contact != null ? GetRelation(g.contact.child.ToList()) : "",
                                  SalesInvoiceNumber = g.sales_invoice != null ? g.sales_invoice.number : "",
                                  PurchaseInvoiceNumber = g.purchase_invoice != null ? g.purchase_invoice.number : "",
                                  Number = g.payment_promissory_note.note_number,

                                  AmountWords = g != null ? g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.has_rounding ?
                                      // Text -> Words
                                        NumToWords.IntToText(Convert.ToInt32(g != null ? g.payment_promissory_note.value : 0))
                                        :
                                        NumToWords.DecimalToText((Convert.ToDecimal(g != null ? g.payment_promissory_note.value : 0))) : "" : "" : "",

                                  CompanyName = g.app_company.name,
                              }).ToList();

                return reportDataSource;
            }
        }

        public ReportDataSource Payment(payment payment)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<payment_detail> payment_detail = payment.payment_detail.ToList();
                reportDataSource.Value = payment_detail
                              .Select(g => new
                              {
                                  id_company = g.id_company,
                                  payment_type = g.payment_type != null ? g.payment_type.name : "",
                                  comments = g.comment,
                                  company_name = g.app_company != null ? g.app_company.name : "",
                                  amount = g.value,
                                  contact_name = g.payment.contact != null ? g.payment.contact.name : "Not Ref",
                                  gov_id = g.payment.contact != null ? g.payment.contact.gov_code : "",
                                  payment_name = g.payment_type != null ? g.payment_type.name : "",
                                  trans_date = g.payment != null ? g.payment.trans_date : DateTime.Now,
                                  currency_name = g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.name : "" : "",
                                  currency_rate = g.app_currencyfx != null ? g.app_currencyfx.sell_value : 0,
                                  number = g.payment != null ? g.payment.number : "Not Ref",
                                  SalesNumber = g.payment_schedual.FirstOrDefault() != null ? g.payment_schedual.FirstOrDefault().sales_invoice != null ? g.payment_schedual.FirstOrDefault().sales_invoice.number : "" : "",
                                  AmountWords = g != null ? g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.has_rounding ?

                     // Text -> Words
                     NumToWords.IntToText(Convert.ToInt32(g != null ? g.payment.GrandTotal : 0))
                     :
                     NumToWords.DecimalToText((Convert.ToDecimal(g != null ? g.payment.GrandTotal : 0))) : "" : "" : "",

                                  HasRounding = g != null ? g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.has_rounding != null ? g.app_currencyfx.app_currency.has_rounding : false : false : false : false

                              }).ToList();

                return reportDataSource;
            }
        }
        public ReportDataSource Project(project project)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<project_task> project_task = project.project_task.ToList();
                reportDataSource.Value = project_task.Select(g => new
                              {
                                  project_code = g.project.code,
                                  project_est_end_date = g.project.est_end_date,
                                  project_est_start_date = g.project.est_start_date,
                                  task_trans_date = g.trans_date,
                                  task_start_date_est = g.start_date_est,
                                  task_end_date_est = g.end_date_est,
                                  number = g.number,
                                  id_company = g.id_company,
                                  company_name = g.app_company != null ? g.app_company.name : "",
                                  contact_name = g.project.contact != null ? g.project.contact.name : "",
                                  contact_address = g.project.contact != null ? g.project.contact.address != null ? g.project.contact.address : "" : "",
                                  contact_email = g.project.contact != null ? g.project.contact.email != null ? g.project.contact.email : "" : "",
                                  contact_phone = g.project.contact != null ? g.project.contact.telephone != null ? g.project.contact.telephone : "" : "",
                                  gov_id = g.project.contact != null ? g.project.contact.gov_code : "",
                                  TagList = g.project.project_tag_detail != null ? GetTag(g.project.project_tag_detail.ToList()) : "",

                              }).ToList();

                return reportDataSource;
            }
        }

        public ReportDataSource Inventory(item_inventory item_inventory)
        {
            using (db db = new db())
            {
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<item_inventory_detail> item_inventory_detail = item_inventory.item_inventory_detail.ToList();

                reportDataSource.Value = item_inventory_detail
                    .Select(g => new
                    {
                        date=g.item_inventory.trans_date,
                        branch=g.item_inventory.app_branch.name,
                        comment=g.item_inventory.comment,
                        id_inventory_detail = g.id_inventory_detail,
                        id_company = g.id_company,
                        item_code = g.item_product != null ? g.item_product.item != null ? g.item_product.item.code != null ? g.item_product.item.code : "" : "" : "",
                        item_description = g.item_product != null ? g.item_product.item != null ? g.item_product.item.name != null ? g.item_product.item.name : "" : "" : "",
                        item_long_description = g.item_product != null ? g.item_product.item != null ? g.item_product.item.description != null ? g.item_product.item.description : "" : "" : "",
                        quantity_system = g.value_system,
                        quantity_counted = g.value_counted,
                        unit_cost = g.unit_value,
                        location = g.app_location != null ? g.app_location.name : "",

                    }).ToList();

                return reportDataSource;
            }
        }
        private string GetTag(List<project_tag_detail> project_tag_detail)
        {
            string TagList = "";
            if (project_tag_detail.Count > 0)
            {
                foreach (project_tag_detail _project_tag_detail in project_tag_detail)
                {
                    if (!TagList.Contains(_project_tag_detail.project_tag.name))
                    {
                        TagList = TagList + ", " + _project_tag_detail.project_tag.name;
                    }
                }
                return TagList.Remove(0, 1);
            }

            return TagList;
        }
        private string GetRelation(List<contact> contact)
        {
            string ContactList = "";
            if (contact.Count > 0)
            {
                foreach (contact _contact in contact)
                {
                    if (!ContactList.Contains(_contact.name))
                    {
                        ContactList = ContactList + ", " + _contact.name + "role:" + _contact.contact_role.name;
                    }
                }
                return ContactList.Remove(0, 1);
            }

            return ContactList;
        }


    }
}
