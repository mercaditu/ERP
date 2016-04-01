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
            else if (Document.GetType().BaseType == typeof(purchase_return))
            {
                purchase_return purchase_return = (purchase_return)Document;
                return PurchaseReturn(purchase_return);
            }
            else if (Document.GetType().BaseType == typeof(purchase_tender))
            {
                purchase_tender purchase_tender = (purchase_tender)Document;
                return PurchaseTender(purchase_tender);
            }
            else if (Document.GetType().BaseType == typeof(item_transfer))
            {
                item_transfer item_transfer = (item_transfer)Document;
                return ItemTransfer(item_transfer);
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
                        geo_name = g.sales_budget.contact.app_geography != null ? g.sales_budget.contact.app_geography.name : "",
                        id_sales_budget = g.id_sales_budget,
                        id_sales_budget_detail = g.id_sales_budget_detail,
                        sales_budget = g.id_sales_budget_detail,
                        id_company = g.id_company,
                        add1 = g.sales_budget != null ? g.sales_budget.contact.address : "",
                        telephone = g.sales_budget != null ? g.sales_budget.contact.telephone : "",
                        email = g.sales_budget != null ? g.sales_budget.contact.email : "",
                        company_name = g.sales_budget != null ? g.sales_budget.contact.name : "",
                        currency = g.sales_budget != null ? g.sales_budget.app_currencyfx.app_currency.name : "",
                        currencyfx_rate = g.sales_budget != null ? g.sales_budget.app_currencyfx.sell_value : 0,
                        item_code = g.item != null ? g.item.code : "",
                        item_description = g.item != null ? g.item.name : "",
                        item_long_description = g.item != null ? g.item.description : "",
                        item_brand = g.item != null ? (g.item.item_brand != null ? g.item.item_brand.name : "") : "",
                        quantity = g.quantity,
                        sub_Total = g.SubTotal,
                        sub_Total_vat = g.SubTotal_Vat,
                        unit_cost = g.unit_cost,
                        unit_price = g.unit_cost,
                        unit_price_vat = g.UnitPrice_Vat,
                        terminale_name = g.sales_budget != null ? (g.sales_budget.app_terminal != null ? g.sales_budget.app_terminal.name : "") : "",
                        code = g.sales_budget != null ? g.sales_budget.code : "",
                        contact_name = g.sales_budget != null ? g.sales_budget.contact.name : "",
                        sales_rep_name = g.sales_budget.sales_rep != null ? g.sales_budget.sales_rep.name : "",
                        trans_date = g.sales_budget != null ? g.sales_budget.trans_date.ToShortDateString() : "",
                        id_vat_group = g.id_vat_group,
                        gov_id = g.sales_budget != null ? g.sales_budget.contact.gov_code : "",
                        contract = g.sales_budget != null ? g.sales_budget.app_contract.name : "",
                        condition = g.sales_budget != null ? g.sales_budget.app_condition.name : "",
                        Number = g.sales_budget != null ? g.sales_budget.number : "",
                        comment = g.sales_budget != null ? g.sales_budget.comment : "",
                        security_user_name = g.sales_budget != null ? g.sales_budget.security_user.name : "",
                        AmountWords = g.sales_budget != null ? g.sales_budget.app_currencyfx.app_currency.has_rounding ?

                        // Text -> Words
                        NumToWords.IntToText(Convert.ToInt32(g.sales_budget != null ? g.sales_budget.GrandTotal : 0)) 
                        :
                        NumToWords.DecimalToText((Convert.ToDecimal(g.sales_budget != null ? g.sales_budget.GrandTotal : 0))) : "",
                        
                        HasRounding = g.sales_budget != null ? g.sales_budget.app_currencyfx.app_currency.has_rounding : false
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
                        geo_name = g.sales_order.contact.app_geography != null ? g.sales_order.contact.app_geography.name : "",
                        sales_budget_number = g.sales_budget_detail != null ? g.sales_budget_detail.sales_budget.number : "",
                        contact_name = g.sales_order.contact.name,
                        customer_address = g.sales_order.contact.address,
                        customer_telephone = g.sales_order.contact.telephone,
                        customer_email = g.sales_order.contact.email,
                        company_Name = g.app_company.name,
                        customer_govid = g.sales_order.contact.gov_code,
                        sales_order_terminal = g.sales_order.app_terminal.name,
                        branch_Name = g.sales_order.app_branch.name,
                        order_Code = g.sales_order.code,
                        delivery_Date = g.sales_order.delivery_date,
                        sales_number = g.sales_order.number,
                        order_Total = g.sales_order.GrandTotal,
                        currency = g.sales_order.app_currencyfx.app_currency.name,
                        currencyfx_rate = g.sales_order.app_currencyfx.sell_value,
                        project_Name = g.sales_order.project != null ? g.sales_order.project.name : "",
                        sales_order_representative = g.sales_order.sales_rep != null ? g.sales_order.sales_rep.name : "",
                        security_user_name = g.sales_order.security_user.name,
                        trans_date = g.sales_order.trans_date,
                        sales_order_contract = g.sales_order.app_contract.name,
                        sales_order_condition = g.sales_order.app_condition.name,
                        DeliveryDate = g.sales_order.delivery_date,
                        sales_order_Comment = g.sales_order.comment,
                        vat_group_name = g.app_vat_group.name,
                        id_vat_group = g.id_vat_group,
                        item_code = g.item.code,
                        item_description = g.item.name,
                        item_brand = g.item.item_brand != null ? g.item.item_brand.name : "",
                        quantity = g.quantity,
                        sub_Total = g.SubTotal,
                        sub_Total_vat = g.SubTotal_Vat,
                        unit_cost = g.unit_cost,
                        unit_price = g.unit_price,
                        unit_price_vat = g.UnitPrice_Vat,
                        AmountWords = g.sales_order != null ? g.sales_order.app_currencyfx.app_currency.has_rounding ?
                        NumToWords.IntToText(Convert.ToInt32(g.sales_order != null ? g.sales_order.GrandTotal : 0))
                        :
                        NumToWords.DecimalToText((Convert.ToDecimal(g.sales_order != null ? g.sales_order.GrandTotal : 0))) : "",
                        HasRounding = g.sales_order != null ? g.sales_order.app_currencyfx.app_currency.has_rounding : false
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
                    add1 = g.sales_invoice != null ? g.sales_invoice.contact.address : "",
                    telephone = g.sales_invoice != null ? g.sales_invoice.contact.telephone : "",
                    email = g.sales_invoice != null ? g.sales_invoice.contact.email : "",
                    company_name = g.sales_invoice != null ? g.sales_invoice.app_company != null ? g.sales_invoice.app_company.name : "" : "",
                    item_code = g.item != null ? g.item.code : "",
                    item_description = g.item != null ? g.item.name : "",
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
                    terminal_name = g.sales_invoice != null ? g.sales_invoice.app_terminal.name : "",
                    code = g.sales_invoice != null ? g.sales_invoice.code : "",
                    customer_contact_name =g.sales_invoice != null ? g.sales_invoice.contact.name:"",
                    customer_code = g.sales_invoice != null ? g.sales_invoice.contact.code:"",
                    customer_alias = g.sales_invoice != null ? g.sales_invoice.contact.alias:"",
                    project_name = g.sales_invoice != null ? g.sales_invoice.project != null ? g.sales_invoice.project.name : "" : "",
                    sales_invoice_rep_name = g.sales_invoice != null ? g.sales_invoice.sales_rep != null ? g.sales_invoice.sales_rep.name : "" : "",
                    trans_date = g.sales_invoice != null ? g.sales_invoice.trans_date.ToString():"",
                    id_vat_group = g.id_vat_group,
                    gov_id = g.sales_invoice != null ? g.sales_invoice.contact.gov_code : "",
                    sales_invoice_contract = g.sales_invoice != null ? g.sales_invoice.app_contract.name : "",
                    sales_invoice_condition = g.sales_invoice != null ? g.sales_invoice.app_contract.app_condition.name : "",
                    sales_number = g.sales_invoice != null ? g.sales_invoice.number : "",
                    sales_invoice_Comment = g.sales_invoice != null ? g.sales_invoice.comment : "",
                    packingList = g.sales_invoice != null ? g.sales_packing_relation != null ? GetPacking(g.sales_packing_relation.ToList()) : "" : "",
                    sales_order = g.sales_invoice != null ? g.sales_order_detail != null ? g.sales_order_detail.sales_order.number : "" : "",
                    AmountWords = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding ?
                        NumToWords.IntToText(Convert.ToInt32(g.sales_invoice != null ? g.sales_invoice.GrandTotal : 0))
                        :
                        NumToWords.DecimalToText((Convert.ToDecimal(g.sales_invoice != null ? g.sales_invoice.GrandTotal : 0))) : "",
                    HasRounding = g.sales_invoice != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding : false
                }).ToList();
              
                return reportDataSource;
            }
        }

        private string GetPacking(List<sales_packing_relation> sales_packing_relation)
        {
            string PackingList = "";
            foreach (sales_packing_relation _sales_packing_relation in sales_packing_relation)
            {
                if (PackingList.Contains(_sales_packing_relation.sales_packing_detail.sales_packing.number))
                {
                    PackingList = PackingList + ", " + _sales_packing_relation.sales_packing_detail.sales_packing.number;
                }
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
                                  contact_name = g.sales_packing.contact.name,
                                  customer_govid = g.sales_packing.contact.gov_code,
                                  customer_address = g.sales_packing.contact.address,
                                  customer_telephone = g.sales_packing.contact.telephone,
                                  customer_email = g.sales_packing.contact.email,
                                  company_Name = g.app_company.name,
                                  company_govid = g.app_company.gov_code,
                                  company_address = g.app_company.address,
                                  sales_terminal = g.sales_packing.app_terminal.name,
                                  branch_Name = g.sales_packing.app_branch.name,
                                  security_user_name = g.sales_packing.security_user.name,
                                  trans_date = g.sales_packing.trans_date,
                                  sales_order_Comment = g.sales_packing.comment,
                                  item_code = g.item.code,
                                  item_description = g.item.name,
                                  item_brand = g.item.item_brand != null ? g.item.item_brand.name : "",
                                  quantity = g.quantity,
                                  sales_invoice_number = g.sales_packing != null ? g.sales_packing_relation != null ? GetInvoice(g.sales_packing_relation.ToList()) : "" : ""
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
                                  add1 = g.sales_return.contact.address,
                                  telephone = g.sales_return.contact.telephone,
                                  email = g.sales_return.contact.email,
                                  company_name = g.app_company.name,
                                  item_code = g.item.code,
                                  item_description = g.item.name,
                                  Description = g.item.item_brand != null ? g.item.item_brand.name : "",
                                  quantity = g.quantity,
                                  sub_Total = g.SubTotal,
                                  sub_Total_vat = g.SubTotal_Vat,
                                  unit_cost = g.unit_cost,
                                  unit_price = g.unit_cost,
                                  unit_price_vat = g.UnitPrice_Vat,
                                  terminale_name = g.sales_return.app_terminal.name,
                                  code = g.sales_return.code,
                                  contact_name = g.sales_return.contact.name,
                                  trans_date = g.sales_return.trans_date,
                                  id_vat_group = g.id_vat_group,
                                  gov_id = g.sales_return.contact.gov_code,
                                  Number = g.sales_return.number,
                                  AmountWords = g.sales_return != null ? g.sales_return.app_currencyfx.app_currency.has_rounding ?
                        NumToWords.IntToText(Convert.ToInt32(g.sales_return != null ? g.sales_return.GrandTotal : 0))
                        :
                        NumToWords.DecimalToText((Convert.ToDecimal(g.sales_return != null ? g.sales_return.GrandTotal : 0))) : "",
                                  HasRounding = g.sales_return != null ? g.sales_return.app_currencyfx.app_currency.has_rounding : false
                              }).ToList();

                return reportDataSource;
            }
        }

        /// <summary>
        /// NOT FINSIHED
        /// </summary>
        /// <param name="purchase_tender"></param>
        /// <returns></returns>
        public ReportDataSource PurchaseTender(purchase_tender purchase_tender)
        {
            using (db db = new db())
            {
                //reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                //List<sales_budget_detail> sales_budget_detail = db.sales_budget_detail.Where(x => x.id_sales_budget == sales_budget.id_sales_budget).ToList();

                //reportDataSource.Value = sales_budget_detail
                //    .Select(g => new
                //    {
                //        id_sales_budget = g.id_sales_budget,
                //        id_sales_budget_detail = g.id_sales_budget_detail,
                //        sales_budget = g.id_sales_budget_detail,
                //        id_company = g.id_company,
                //        add1 = g.sales_budget.contact.address,
                //        telephone = g.sales_budget.contact.telephone,
                //        email = g.sales_budget.contact.email,
                //        company_name = g.app_company.name,
                //        item_code = g.item.code,
                //        item_description = g.item.name,
                //        item_brand = g.item.item_brand != null ? g.item.item_brand.name : "",
                //        quantity = g.quantity,
                //        sub_Total = g.SubTotal,
                //        sub_Total_vat = g.SubTotal_Vat,
                //        unit_cost = g.unit_cost,
                //        unit_price = g.unit_cost,
                //        unit_price_vat = g.UnitPrice_Vat,
                //        terminale_name = g.sales_budget.app_terminal.name,
                //        code = g.sales_budget.code,
                //        contact_name = g.sales_budget.contact.name,
                //        sales_rep_name = g.sales_budget.sales_rep != null ? g.sales_budget.sales_rep.name : "",
                //        trans_date = g.sales_budget.trans_date,
                //        id_vat_group = g.id_vat_group,
                //        gov_id = g.sales_budget.contact.gov_code,
                //        contract = g.sales_budget.app_contract.name,
                //        condition = g.sales_budget.app_contract.app_condition.name,
                //        Number = g.sales_budget.number,
                //        comment = g.sales_budget.comment
                //    }).ToList();

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
                        add1 = g.purchase_order.contact.address,
                        telephone = g.purchase_order.contact.telephone,
                        email = g.purchase_order.contact.email,
                        company_name = g.app_company.name,
                        item_code = g.item.code,
                        item_description = g.item.name,
                        Description = g.item.item_brand != null ? g.item.item_brand.name : "",
                        quantity = g.quantity,
                        sub_Total = g.SubTotal,
                        sub_Total_vat = g.SubTotal_Vat,
                        unit_cost = g.unit_cost,
                        unit_price = g.unit_cost,
                        unit_price_vat = g.UnitCost_Vat,
                        terminale_name = g.purchase_order.app_terminal.name,
                        code = g.purchase_order.code,
                        contact_name = g.purchase_order.contact.name,
                        trans_date = g.purchase_order.trans_date,
                        id_vat_group = g.id_vat_group,
                        gov_id = g.purchase_order.contact.gov_code,
                        Number = g.purchase_order.number,
                        AmountWords = g.purchase_order != null ? g.purchase_order.app_currencyfx.app_currency.has_rounding ?
                        NumToWords.IntToText(Convert.ToInt32(g.purchase_order != null ? g.purchase_order.GrandTotal : 0))
                        :
                        NumToWords.DecimalToText((Convert.ToDecimal(g.purchase_order != null ? g.purchase_order.GrandTotal : 0))) : "",
                        HasRounding = g.purchase_order != null ? g.purchase_order.app_currencyfx.app_currency.has_rounding : false
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
            //using (db db = new db())
            //{
            //    List<sales_invoice_detail> sales_invoice_detail = db.sales_invoice_detail.Where(x => x.id_sales_invoice == sales_invoice.id_sales_invoice).ToList();
            //    reportDataSource.Value = sales_invoice_detail.Select(g => new
            //    {
            //        sales_invoice = g.id_sales_invoice,
            //        id_company = g.id_company,
            //        add1 = g.sales_invoice.contact.address,
            //        telephone = g.sales_invoice.contact.telephone,
            //        email = g.sales_invoice.contact.email,
            //        company_name = g.app_company.name,
            //        item_code = g.item.code,
            //        item_description = g.item.name,
            //        Description = g.item.item_brand != null ? g.item.item_brand.name : "",
            //        quantity = g.quantity,
            //        sub_Total = g.SubTotal,
            //        sub_Total_vat = g.SubTotal_Vat,
            //        unit_cost = g.unit_cost,
            //        unit_price = g.unit_price,
            //        unit_price_vat = g.UnitPrice_Vat,
            //        terminal_name = g.sales_invoice.app_terminal.name,
            //        code = g.sales_invoice.code,
            //        customer_contact_name = g.sales_invoice.contact.name,
            //        customer_code = g.sales_invoice.contact.code,
            //        customer_alias = g.sales_invoice.contact.alias,
            //        project_name = g.sales_invoice.project != null ? g.sales_invoice.project.name : "",
            //        sales_invoice_rep_name = g.sales_invoice.sales_rep != null ? g.sales_invoice.sales_rep.name : "",
            //        trans_date = g.sales_invoice.trans_date,
            //        id_vat_group = g.id_vat_group,
            //        gov_id = g.sales_invoice.contact.gov_code,
            //        sales_invoice_contract = g.sales_invoice.app_contract.name,
            //        sales_invoice_condition = g.sales_invoice.app_contract.app_condition.name,
            //        sales_number = g.sales_invoice.number,
            //        sales_invoice_Comment = g.sales_invoice.comment
            //    }).ToList();

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
                        transfer_number=g.item_transfer.number,
                        location_origin_name =g.item_transfer.app_location_origin.name,
                        location_destination_name = g.item_transfer.app_location_destination.name,
                        item_code =g.item_product.item.code,
                        quantity_origin =g.quantity_origin,
                        item_name =g.item_product.item.name,
                        trans_date =g.item_transfer.trans_date,
                        comment=g.item_transfer.comment
                    }).ToList();

                return reportDataSource;
            }
        }
    }
}
