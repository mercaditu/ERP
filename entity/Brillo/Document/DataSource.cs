using entity.Class;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace entity.Brillo.Document
{
    public class DataSource
    {
        private ReportDataSource reportDataSource = new ReportDataSource();

        public ReportDataSource Create(object Document)
        {
            string BaseName = Document.GetType().BaseType.ToString();
            string AppName = Document.GetType().ToString();

            if (AppName == typeof(sales_invoice).ToString() || BaseName == typeof(sales_invoice).ToString())
            {
                sales_invoice sales_invoice = (sales_invoice)Document;
                return SalesInvoice(sales_invoice);
            }
            else if (AppName == typeof(sales_order).ToString() || BaseName == typeof(sales_order).ToString())
            {
                sales_order sales_order = (sales_order)Document;
                return SalesOrder(sales_order);
            }
            else if (AppName == typeof(sales_budget).ToString() || BaseName == typeof(sales_budget).ToString())
            {
                sales_budget sales_budget = (sales_budget)Document;
                return SalesBudget(sales_budget);
            }
            else if (AppName == typeof(sales_packing).ToString() || BaseName == typeof(sales_packing).ToString())
            {
                sales_packing sales_packing = (sales_packing)Document;
                return Sales_PackingList(sales_packing);
            }
            else if (AppName == typeof(sales_return).ToString() || BaseName == typeof(sales_return).ToString())
            {
                sales_return sales_return = (sales_return)Document;
                return SalesReturn(sales_return);
            }
            else if (AppName == typeof(purchase_order).ToString() || BaseName == typeof(purchase_order).ToString())
            {
                purchase_order purchase_order = (purchase_order)Document;
                return PurchaseOrder(purchase_order);
            }
            else if (AppName == typeof(purchase_invoice).ToString() || BaseName == typeof(purchase_invoice).ToString())
            {
                purchase_invoice purchase_invoice = (purchase_invoice)Document;
                return PurchaseInvoice(purchase_invoice);
            }
            else if (AppName == typeof(purchase_return).ToString() || BaseName == typeof(purchase_return).ToString())
            {
                purchase_return purchase_return = (purchase_return)Document;
                return PurchaseReturn(purchase_return);
            }
            else if (AppName == typeof(payment_promissory_note).ToString() || BaseName == typeof(payment_promissory_note).ToString())
            {
                payment_promissory_note payment_promissory_note = (payment_promissory_note)Document;
                return PromissoryNote(payment_promissory_note);
            }
            else if (AppName == typeof(payment_detail).ToString() || BaseName == typeof(payment_detail).ToString())
            {
                payment_detail payment_detail = (payment_detail)Document;
                return PaymentDetail_Print(payment_detail);
            }
            else if (AppName == typeof(purchase_tender_contact).ToString() || BaseName == typeof(purchase_tender_contact).ToString())
            {
                purchase_tender_contact purchase_tender_contact = (purchase_tender_contact)Document;
                return PurchaseTender(purchase_tender_contact);
            }
            else if (AppName == typeof(item_transfer).ToString() || BaseName == typeof(item_transfer).ToString())
            {
                item_transfer item_transfer = (item_transfer)Document;
                return ItemTransfer(item_transfer);
            }
            else if (AppName == typeof(List<payment_schedual>).ToString() || BaseName == typeof(List<payment_schedual>).ToString())
            {
                List<payment_schedual> SchedualList = (List<payment_schedual>)Document;
                return PaymentSchedual(SchedualList);
            }
            else if (AppName == typeof(payment).ToString() || BaseName == typeof(payment).ToString())
            {
                payment payment = (payment)Document;
                return Payment(payment);
            }
            else if (AppName == typeof(payment_approve).ToString() || BaseName == typeof(payment_approve).ToString())
            {
                payment_approve payment_approve = (payment_approve)Document;
                return PaymentApprove(payment_approve);
            }
            else if (AppName == typeof(item_inventory).ToString() || BaseName == typeof(item_inventory).ToString())
            {
                item_inventory item_inventory = (item_inventory)Document;
                return Inventory(item_inventory);
            }
            else if (AppName == typeof(project).ToString() || BaseName == typeof(project).ToString())
            {
                project project = (project)Document;
                return Project(project);
            }
            else if (AppName == typeof(item_request).ToString() || BaseName == typeof(item_request).ToString())
            {
                item_request item_request = (item_request)Document;
                return ItemRequest(item_request);
            }
            else if (AppName == typeof(production_execution_detail).ToString() || BaseName == typeof(production_execution_detail).ToString())
            {
                production_execution_detail production_execution_detail = (production_execution_detail)Document;
                return ProductionExecutionDetail(production_execution_detail);
            }

            return null;
        }

        public ReportDataSource SalesBudget(sales_budget sales_budget)
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
                    currencyfx_rate = g.sales_budget != null ? g.sales_budget.app_currencyfx != null ? g.sales_budget.app_currencyfx.sell_value : 0 : 0,
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
                    BatchCode = g.batch_code,
                    ExpirationDate = g.expire_date,
                    terminale_name = g.sales_budget != null ? (g.sales_budget.app_terminal != null ? g.sales_budget.app_terminal.name != null ? g.sales_budget.app_terminal.name : "" : "") : "",
                    code = g.sales_budget != null ? g.sales_budget.code != null ? g.sales_budget.code : "" : "",
                    contact_name = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.name != null ? g.sales_budget.contact.name : "" : "" : "",
                    sales_rep_name = g.sales_budget != null ? g.sales_budget.sales_rep != null ? g.sales_budget.sales_rep.name != null ? g.sales_budget.sales_rep.name : "" : "" : "",
                    trans_date = g.sales_budget != null ? g.sales_budget.trans_date != null ? g.sales_budget.trans_date.ToShortDateString() : "" : "",
                    id_vat_group = g.id_vat_group,
                    gov_id = g.sales_budget != null ? g.sales_budget.contact != null ? g.sales_budget.contact.gov_code != null ? g.sales_budget.contact.gov_code : "" : "" : "",
                    contract = g.sales_budget != null ? g.sales_budget.app_contract != null ? g.sales_budget.app_contract.name != null ? g.sales_budget.app_contract.name : "" : "" : "",
                    condition = g.sales_budget != null ? g.sales_budget.app_condition != null ? g.sales_budget.app_condition.name != null ? g.sales_budget.app_condition.name : "" : "" : "",
                    number = g.sales_budget != null ? g.sales_budget.number != null ? g.sales_budget.number : "" : "",
                    comment = g.sales_budget != null ? g.sales_budget.comment != null ? g.sales_budget.comment : "" : "",
                    security_user_name = g.sales_budget != null ? g.sales_budget.security_user != null ? g.sales_budget.security_user.name != null ? g.sales_budget.security_user.name : "" : "" : "",
                    AmountWords = g.sales_budget != null ? g.sales_budget.app_currencyfx != null ? g.sales_budget.app_currencyfx.app_currency != null ? g.sales_budget.app_currencyfx.app_currency.has_rounding ?

                    // Text -> Words
                    NumToWords.IntToText(Convert.ToInt64(g.sales_budget != null ? g.sales_budget.GrandTotal : 0))
                    :
                    NumToWords.DecimalToText((Convert.ToDecimal(g.sales_budget != null ? g.sales_budget.GrandTotal : 0))) : "" : "" : "",

                    HasRounding = g.sales_budget != null ? g.sales_budget.app_currencyfx != null ? g.sales_budget.app_currencyfx.app_currency != null ? g.sales_budget.app_currencyfx.app_currency.has_rounding : false : false : false,
                }).ToList();

            return reportDataSource;
        }

        public ReportDataSource SalesOrder(sales_order sales_order)
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
                    BatchCode = g.batch_code,
                    ExpirationDate = g.expire_date,
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

                    HasRounding = g.sales_order != null ? g.sales_order.app_currencyfx != null ? g.sales_order.app_currencyfx.app_currency != null ? g.sales_order.app_currencyfx.app_currency.has_rounding : false : false : false,
                    unit_price_discount = g.discount,
                }).ToList();

            return reportDataSource;
        }

        public ReportDataSource SalesInvoice(sales_invoice sales_invoice)
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
                id_sales_invoice = g.sales_invoice != null ? g.sales_invoice.id_sales_invoice : 0,
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
                BatchCode = g.batch_code,
                ExpirationDate = g.expire_date,
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

                HasRounding = g.sales_invoice != null ? g.sales_invoice.app_currencyfx != null ? g.sales_invoice.app_currencyfx.app_currency != null ? g.sales_invoice.app_currencyfx.app_currency.has_rounding : false : false : false,
                unit_price_discount = g.discount,
            }).ToList();

            return reportDataSource;
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
                    if (_sales_packing_relation.sales_packing_detail.sales_packing.number != null)
                    {
                        if (!PackingList.Contains(_sales_packing_relation.sales_packing_detail.sales_packing.number))
                        {
                            PackingList = PackingList + ", " + _sales_packing_relation.sales_packing_detail.sales_packing.number;
                        }
                    }

                }
                if (PackingList != "")
                {
                    return PackingList.Remove(0, 1);
                }

            }

            return PackingList;
        }

        public ReportDataSource Sales_PackingList(sales_packing sales_packing)
        {
            reportDataSource.Name = "DataSet1";
            List<sales_packing_detail> sales_packing_detail = sales_packing.sales_packing_detail.ToList();
            reportDataSource.Value = sales_packing_detail
                          .Where(x => x.user_verified)
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
                              quantity = g.verified_quantity,
                              BatchCode = g.batch_code,
                              ExpirationDate = g.expire_date,
                              ItemMeasurement = g.item != null ? g.item.app_measurement != null ? g.item.app_measurement.name : "" : "",
                              VolumeMeasurement = g.measurement_volume != null ? g.measurement_volume.name : "",
                              WeightMeasurement = g.measurement_weight != null ? g.measurement_weight.name : "",
                              number = g.sales_packing != null ? g.sales_packing.number : "",
                              sales_invoice_number = g.sales_packing != null ? g.sales_packing_relation != null ? GetInvoice(g.sales_packing_relation.ToList()) : "" : "",
                              packing_type = g.sales_packing != null ? g.sales_packing.packing_type.ToString() : "",
                              eta = g.sales_packing.eta != null ? g.sales_packing.eta.ToString() : "",
                              etd = g.sales_packing.etd != null ? g.sales_packing.etd.ToString() : "",
                              driver = g.sales_packing.driver != null ? g.sales_packing.driver : "",
                              licence = g.sales_packing.licence_no != null ? g.sales_packing.licence_no : "",
                              distance = g.sales_packing.avg_distance != null ? g.sales_packing.avg_distance : "",
                              FixedAsset = g.sales_packing.item_asset != null ? g.sales_packing.item_asset.item.name : "",
                              FixedAssetBrand = g.sales_packing.item_asset != null ? g.sales_packing.item_asset.item.item_brand != null ? g.sales_packing.item_asset.item.item_brand.name : "" : "",
                          }).ToList();

            return reportDataSource;
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
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<sales_return_detail> sales_return_detail = sales_return.sales_return_detail.ToList();

            if (sales_return_detail.Count < sales_return.app_document_range.app_document.line_limit)
            {
                for (int i = sales_return_detail.Count; i < sales_return.app_document_range.app_document.line_limit; i++)
                {
                    sales_return_detail _sales_return_detail = new entity.sales_return_detail();
                    sales_return_detail.Add(_sales_return_detail);
                }
            }

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

                              HasRounding = g.sales_return != null ? g.sales_return.app_currencyfx != null ? g.sales_return.app_currencyfx.app_currency != null ? g.sales_return.app_currencyfx.app_currency.has_rounding : false : false : false,
                              unit_price_discount = g.discount,
                          }).ToList();

            return reportDataSource;
        }

        /// <summary>
        /// NOT FINSIHED
        /// </summary>
        /// <param name="purchase_tender"></param>
        /// <returns></returns>
        public ReportDataSource PurchaseTender(purchase_tender_contact purchase_tender_contact)
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
                    Measurement = g.purchase_tender_item != null ? g.purchase_tender_item.item != null ? g.purchase_tender_item.item.app_measurement != null ? g.purchase_tender_item.item.app_measurement.name : "" : "" : "",
                    AmountWords = g.purchase_tender_contact != null ? g.purchase_tender_contact.app_currencyfx != null ? g.purchase_tender_contact.app_currencyfx.app_currency != null ? g.purchase_tender_contact.app_currencyfx.app_currency.has_rounding ?

                 // Text -> Words
                 NumToWords.IntToText(Convert.ToInt32(g.purchase_tender_contact != null ? g.purchase_tender_contact.GrandTotal : 0))
                 :
                 NumToWords.DecimalToText((Convert.ToDecimal(g.purchase_tender_contact != null ? g.purchase_tender_contact.GrandTotal : 0))) : "" : "" : "",

                    HasRounding = g.purchase_tender_contact != null ? g.purchase_tender_contact.app_currencyfx != null ? g.purchase_tender_contact.app_currencyfx.app_currency != null ? g.purchase_tender_contact.app_currencyfx.app_currency.has_rounding : false : false : false
                }).ToList();
            return reportDataSource;
        }

        public ReportDataSource PurchaseOrder(purchase_order purchase_order)
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
                    contact_name = g.purchase_order != null ? g.purchase_order.contact.name : "",
                    add1 = g.purchase_order != null ? g.purchase_order.contact.address : "",
                    telephone = g.purchase_order != null ? g.purchase_order.contact.telephone : "",
                    email = g.purchase_order != null ? g.purchase_order.contact.email : "",
                    company_name = g.app_company != null ? g.app_company.name : "",
                    CompanyAddress = g.app_company != null ? g.app_company.address : "",
                    BranchAddress = g.purchase_order != null ? g.purchase_order.app_branch != null ? g.purchase_order.app_branch.address : "" : "",

                    Bank = g.purchase_order != null ? g.purchase_order.contact != null ? g.purchase_order.contact.app_bank != null ? g.purchase_order.contact.app_bank.name : "" : "" : "",
                    Comment = g.purchase_order != null ? g.purchase_order.comment : "",
                    RefContact = g.purchase_order != null ? g.purchase_order.contact_ref != null ? g.purchase_order.contact_ref.name : "" : "",

                    SupplierCode = g.item != null ? g.item.supplier_code : "",
                    SKU = g.item != null ? g.item.sku : "",
                    item_code = g.item != null ? g.item.code : "",
                    item_description = g.item_description,
                    SupplierName = g.item != null ? g.item.supplier_name : "",
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
                    Measurement = g.item != null ? g.item.app_measurement != null ? g.item.app_measurement.name : "" : "",
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

                    HasRounding = g.purchase_order != null ? g.purchase_order.app_currencyfx != null ? g.purchase_order.app_currencyfx.app_currency != null ? g.purchase_order.app_currencyfx.app_currency.has_rounding : false : false : false
                }).ToList();

            return reportDataSource;
        }

        public ReportDataSource PurchaseInvoice(purchase_invoice purchase_invoice)
        {
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<purchase_invoice_detail> purchase_invoice_detail = purchase_invoice.purchase_invoice_detail.ToList();

            reportDataSource.Value = purchase_invoice_detail
                .Select(g => new
                {
                    CompanyName = g.app_company != null ? g.app_company.name : "",
                    TerminalName = g.purchase_invoice != null ? g.purchase_invoice.app_terminal != null ? g.purchase_invoice.app_terminal.name : "" : "",
                    BranchName = g.purchase_invoice != null ? g.purchase_invoice.app_branch != null ? g.purchase_invoice.app_branch.name : "" : "",
                    UserName = g.purchase_invoice != null ? g.purchase_invoice.security_user != null ? g.purchase_invoice.security_user.name : "" : "",

                    SupplierName = g.purchase_invoice != null ? g.purchase_invoice.contact.name : "",
                    SupplierGovCode = g.purchase_invoice.contact.gov_code,
                    SupplierAddress = g.purchase_invoice != null ? g.purchase_invoice.contact.address : "",
                    SupplierTelephone = g.purchase_invoice != null ? g.purchase_invoice.contact.telephone : "",
                    SupplierEmail = g.purchase_invoice != null ? g.purchase_invoice.contact.email : "",

                    ItemCode = g.item != null ? g.item.code : "",
                    ItemName = g.item_description,
                    ItemBrand = g.item != null ? g.item.item_brand != null ? g.item.item_brand.name : "" : "",

                    CostCenterName = g.app_cost_center != null ? g.app_cost_center.name : "",
                    Quantity = g.quantity,
                    //Use VAT Group ID to seperate the data into columns as per Paraguayan Law.
                    VATGroup_ID = g.id_vat_group,
                    VATGroup_Name = g.app_vat_group != null ? g.app_vat_group.name : "",

                    UnitCost = g.unit_cost,
                    UnitCost_VAT = g.UnitCost_Vat,
                    SubTotal = g.SubTotal,
                    SubTotal_VAT = g.SubTotal_Vat,

                    ProjectName = g.purchase_invoice != null ? g.purchase_invoice.project != null ? g.purchase_invoice.project.name : "" : "",

                    Condition = g.purchase_invoice != null ? g.purchase_invoice.app_condition.name : "",
                    Contract = g.purchase_invoice != null ? g.purchase_invoice.app_contract.name : "",
                    Currency = g.purchase_invoice != null ? g.purchase_invoice.app_currencyfx != null ? g.purchase_invoice.app_currencyfx.app_currency.name : "" : "",
                    PurchaseNumber = g.purchase_invoice.number,
                    PurchaseCode = g.purchase_invoice.code,
                    PurchaseDate = g.purchase_invoice.trans_date,

                    AmountWords = g.purchase_invoice != null ? g.purchase_invoice.app_currencyfx != null ? g.purchase_invoice.app_currencyfx.app_currency != null ? g.purchase_invoice.app_currencyfx.app_currency.has_rounding ?

                 // Text -> Words
                 NumToWords.IntToText(Convert.ToInt32(g.purchase_invoice != null ? g.purchase_invoice.GrandTotal : 0))
                 :
                 NumToWords.DecimalToText((Convert.ToDecimal(g.purchase_invoice != null ? g.purchase_invoice.GrandTotal : 0))) : "" : "" : "",

                    HasRounding = g.purchase_invoice != null ? g.purchase_invoice.app_currencyfx != null ? g.purchase_invoice.app_currencyfx.app_currency != null ? g.purchase_invoice.app_currencyfx.app_currency.has_rounding : false : false : false
                }).ToList();

            return reportDataSource;
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
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<item_transfer_detail> item_transfer_detail = item_transfer.item_transfer_detail.ToList();

            reportDataSource.Value = item_transfer_detail
                .Select(g => new
                {
                    ProjectName = g.item_transfer != null ? g.item_transfer.project != null ? g.item_transfer.project.name : "" : "",
                    ProjectTaskName = g.project_task != null ? g.project_task.item_description : "",
                    ProjectTaskCode = g.project_task != null ? g.project_task.code : "",

                    DepartmentName = g.item_transfer != null ? g.item_transfer.app_department != null ? g.item_transfer.app_department.name : "" : "",
                    UserName = g.security_user != null ? g.security_user.name : "",
                    RequstedUserName = g.item_transfer != null ? g.item_transfer.user_requested != null ? g.item_transfer.user_requested.name : "" : "",
                    RequstedUserCode = g.item_transfer != null ? g.item_transfer.user_requested != null ? g.item_transfer.user_requested.code : "" : "",
                    EmployeeName = g.item_transfer != null ? g.item_transfer.employee != null ? g.item_transfer.employee.name : "" : "",
                    EmployeeCode = g.item_transfer != null ? g.item_transfer.employee != null ? g.item_transfer.employee.code : "" : "",

                    transfer_number = g.item_transfer.number,
                    location_origin_name = g.item_transfer != null ? g.item_transfer.app_location_origin != null ? g.item_transfer.app_location_origin.name : "" : "",
                    location_destination_name = g.item_transfer != null ? g.item_transfer.app_location_destination != null ? g.item_transfer.app_location_destination.name : "" : "",
                    item_code = g.item_product.item.code,
                    quantity_origin = g.quantity_origin,
                    quantity_destination = g.quantity_destination,
                    QuantityInStock = g.Quantity_InStock,
                    Measurement = g.item_product != null ? g.item_product.item != null ? g.item_product.item.app_measurement != null ? g.item_product.item.app_measurement.name : "" : "" : "",
                    item_name = g.item_product != null ? g.item_product.item.name : "",
                    trans_date = g.item_transfer.trans_date,
                    timestamp = g.timestamp,
                    status = g.status,
                    comment = g.item_transfer.comment,
                    Asset = g.item_transfer.item_asset != null ? g.item_transfer.item_asset.item != null ? g.item_transfer.item_asset.item.name : "" : "",
                    eta = g.item_transfer.eta != null ? g.item_transfer.eta.ToString() : "",
                    etd = g.item_transfer.etd != null ? g.item_transfer.etd.ToString() : "",
                    driver = g.item_transfer.driver != null ? g.item_transfer.driver.ToString() : "",
                    licence = g.item_transfer.licence_no != null ? g.item_transfer.licence_no.ToString() : "",
                    distance = g.item_transfer.avg_distance != null ? g.item_transfer.avg_distance.ToString() : ""
                }).ToList();

            return reportDataSource;
        }

        public ReportDataSource ItemRequest(item_request item_request)
        {
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<item_request_detail> item_request_detail = item_request.item_request_detail.ToList();

            reportDataSource.Value = item_request_detail
                .Select(g => new
                {
                    ProjectName = g.item_request != null ? g.item_request.project != null ? g.item_request.project.name : "" : "",
                    ProjectTaskName = g.project_task != null ? g.project_task.item_description : "",
                    ProjectTaskCode = g.project_task != null ? g.project_task.code : "",

                    DepartmentName = g.item_request != null ? g.item_request.app_department != null ? g.item_request.app_department.name : "" : "",
                    UserName = g.security_user != null ? g.security_user.name : "",
                    RequstedUserName = g.item_request != null ? g.item_request.request_user != null ? g.item_request.request_user.name : "" : "",
                    RequstedUserCode = g.item_request != null ? g.item_request.request_user != null ? g.item_request.request_user.code : "" : "",

                    item_code = g.item != null ? g.item.code : "",
                    item_name = g.item != null ? g.item.name : "",
                    quantity = g.quantity,
                    Available = g.Balance,
                    DimensionString = g.DimensionString,
                    Measurement = g.item != null ? g.item.app_measurement != null ? g.item.app_measurement.name : "" : "",
                    request_date = g.item_request.request_date,
                    trans_date = g.item_request.timestamp
                }).ToList();

            return reportDataSource;
        }
        public ReportDataSource ProductionExecutionDetail(production_execution_detail production_execution_detail)
        {
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<production_execution_detail> Listproduction_execution_detail = new List<entity.production_execution_detail>();
            Listproduction_execution_detail.Add(production_execution_detail);
            reportDataSource.Value = Listproduction_execution_detail
                .Select(g => new
                {
                    ProjectName = g.project_task != null ? g.project_task.project != null ? g.project_task.project.name : "" : "",
                    Number = g.production_order_detail != null?g.production_order_detail.production_order != null ? g.production_order_detail.production_order.work_number : "":"",
                    Name = g.name,
                    Line = g.production_order_detail != null ? g.production_order_detail.production_order != null ? g.production_order_detail.production_order.production_line != null ? g.production_order_detail.production_order.production_line.name : "" : "" : "",
                    StartDate = g.start_date != null ? g.start_date.ToString() : "",
                    EndDate = g.end_date != null ? g.end_date.ToString() : "",
                    item_input = g.parent != null ? g.parent.item != null ? g.parent.item.name : "" : "",
                    item_input_quantity = g.quantity ,
                    ParentDimension = g.parent != null ? g.parent.DimensionString:"",
                    item_code = g.item != null ? g.item.code : "",
                    item_name = g.item != null ? g.item.name : "",
                    Dimension = g.DimensionString,
                    trans_date = g.trans_date,
                    EmpName=g.contact!=null?g.contact.name:"",

                    Hours =g.hours

                }).ToList();

            return reportDataSource;
        }

        public ReportDataSource ItemMovementLabel(item_movement item_movement)
        {
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<item_movement> item_movementList = new List<entity.item_movement>();
            item_movementList.Add(item_movement);

            reportDataSource.Value = item_movementList
                .Select(g => new
                {
                    item_code = g.item_product != null ? g.item_product.item != null ? g.item_product.item.code : "" : "",
                    item_name = g.item_product != null ? g.item_product.item != null ? g.item_product.item.name : "" : "",
                    lot_number = g.code,
                    trans_date = g.trans_date,
                    exp_date = g.expire_date,
                    quantity = g.credit - g.debit,
                    barcode = g.barcode
                }).ToList();

            return reportDataSource;
        }

        public List<ReportDataSource> Impex(impex impex)
        {
            List<ReportDataSource> ReportDataSourceList = new List<ReportDataSource>();
            ReportDataSource reportDataSourceCost = new ReportDataSource();
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            reportDataSourceCost.Name = "DataSet2";
            if (impex != null && impex.impex_expense.FirstOrDefault() != null)
            {
                if (impex.impex_expense.FirstOrDefault() != null && impex.impex_expense.FirstOrDefault().purchase_invoice != null)
                {
                    Brillo.ImportCostReport ImportCostReport = new Brillo.ImportCostReport();
                    ImportCostReport.GetExpensesForAllIncoterm(impex);
                    List<Impex_ItemDetail> impex_expenseList = ImportCostReport.Impex_ItemDetailLIST;

                    reportDataSource.Value = ImportCostReport.Impex_ItemDetailLIST
                        .Select(g => new
                        {
                            number = g.number,
                            incoterm = g.incoterm,
                            item = g.item,
                            code = g.item_code,
                            quantity = g.quantity,
                            unit_cost = g.unit_cost,
                            unit_Importcost = g.unit_Importcost,
                            cost = g.cost,
                            prorated_cost = g.prorated_cost,
                            sub_total = g.sub_total,
                        }).ToList();
                    reportDataSourceCost.Value = ImportCostReport.CostDetailLIST.GroupBy(x => x.CostName)
                       .Select(g => new
                       {
                           Cost = g.Sum(x => x.Cost),
                           Costfx = g.Sum(x => x.Costfx),
                           CostName = g.Key
                       }).ToList();
                    ReportDataSourceList.Add(reportDataSource);
                    ReportDataSourceList.Add(reportDataSourceCost);
                }
                else
                {
                    Brillo.ImportCostReport ImportCostReport = new Brillo.ImportCostReport();
                    reportDataSource.Value = ImportCostReport.Impex_ItemDetailLIST
                     .Select(g => new
                     {
                         number = g.number,
                         incoterm = g.incoterm,
                         item = g.item,
                         code = g.item_code,
                         quantity = g.quantity,
                         unit_cost = g.unit_cost,
                         unit_Importcost = g.unit_Importcost,
                         cost = g.cost,
                         prorated_cost = g.prorated_cost,
                         sub_total = g.sub_total,
                     }).ToList();
                    reportDataSourceCost.Value = ImportCostReport.CostDetailLIST.GroupBy(x => x.CostName)
                       .Select(g => new
                       {
                           Cost = g.Sum(x => x.Cost),
                           Costfx = g.Sum(x => x.Costfx),
                           CostName = g.Key
                       }).ToList();
                    ReportDataSourceList.Add(reportDataSource);
                    ReportDataSourceList.Add(reportDataSourceCost);
                }
            }

            return ReportDataSourceList;
        }

        public ReportDataSource PaymentDetail_Print(payment_detail payment_detail)
        {
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc

            List<payment_detail> payment_detailList = new List<payment_detail>();
            payment_detailList.Add(payment_detail);

            reportDataSource.Value = payment_detailList.Select(g => new
            {
                Payee = g.payment != null ? g.payment.contact != null ? g.payment.contact.name : "" : "",
                Address = g.payment != null ? g.payment.contact != null ? g.payment.contact.address : "" : "",
                Phone = g.payment != null ? g.payment.contact != null ? g.payment.contact.telephone : "" : "",
                Email = g.payment != null ? g.payment.contact != null ? g.payment.contact.email : "" : "",
                Bank = g.payment != null ? g.payment.contact != null ? g.payment.contact.app_bank != null ? g.payment.contact.app_bank.name : "" : "" : "",
                ChequeDate = g.payment != null ? g.payment.trans_date.ToLongDateString() : "",
                PaymentDate = g.trans_date,
                Memo = g.comment,
                AmountNumber = Convert.ToInt32(g != null ? g.value : 0),
                AmountWords = g != null ? g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.has_rounding ?
                    //Text -> Words
                    NumToWords.IntToText(Convert.ToInt32(g != null ? g.value : 0))
                    :
                    NumToWords.DecimalToText((Convert.ToDecimal(g != null ? g.value : 0))) : "" : "" : "",
            }
                );
            return reportDataSource;
        }

        public ReportDataSource PromissoryNote(payment_promissory_note payment_promissory_note)
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

        public ReportDataSource PaymentSchedual(List<payment_schedual> SchedualList)
        {
            /// Pankeel we need to change some things here.
            /// 1) Create query based on Payment Schedual.

            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc

            //List<payment_detail> DetailList = new List<payment_detail>();

            //DetailList.Add(SchedualList.ToList());

            reportDataSource.Value = SchedualList
                            .Select(g => new
                            {
                                payment_type = g.payment_detail != null ? g.payment_detail.payment_type != null ? g.payment_detail.payment_type.name : "" : "",
                                comments = g.payment_detail != null ? string.IsNullOrEmpty(g.payment_detail.comment) ? "" : g.payment_detail.comment : "",
                                company_name = g.app_company != null ? g.app_company.name : "",
                                amount = g.payment_detail != null ? g.payment_detail.value : 0,
                                contact_name = g.contact != null ? g.contact.name : "Not Ref",
                                gov_id = g.contact != null ? g.contact.gov_code : "",
                                trans_date = g.trans_date,
                                currency_name = g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.name : "" : "",
                                currency_rate = g.app_currencyfx != null ? g.app_currencyfx.sell_value : 0,
                                number = g.number,
                                PurchaseNumber = g.purchase_invoice != null ? g.purchase_invoice.number : "",
                                BankAccount = g.payment_detail != null ? g.payment_detail.app_account != null ? g.payment_detail.app_account.name : "" : "",

                                //// Text -> Words
                                //NumToWords.IntToText(Convert.ToInt32(g != null ? g.payment.GrandTotal : 0))
                                //:
                                //NumToWords.DecimalToText((Convert.ToDecimal(g != null ? g.payment.GrandTotal : 0))) : "" : "",

                                //            HasRounding = g != null ? g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.has_rounding : false : false : false
                            }).ToList();

            return reportDataSource;
        }

        public ReportDataSource Payment(payment payment)
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
                                BankAccount = g.app_account != null ? g.app_account.name : "",
                                AmountWords = g != null ? g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.has_rounding ?

                    // Text -> Words
                    NumToWords.IntToText(Convert.ToInt32(g != null ? g.payment.payment_detail.Sum(x=>x.value) : 0))
                    :
                    NumToWords.DecimalToText((Convert.ToDecimal(g != null ? g.payment.payment_detail.Sum(x => x.value) : 0))) : "" : "" : "",

                                HasRounding = g != null ? g.app_currencyfx != null ? g.app_currencyfx.app_currency != null ? g.app_currencyfx.app_currency.has_rounding : false : false : false
                            }).ToList();

            return reportDataSource;
        }

        public ReportDataSource PaymentApprove(payment_approve payment_approve)
        {
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<payment_approve_detail> payment_approve_detail = payment_approve.payment_approve_detail.ToList();
            reportDataSource.Value = payment_approve_detail
                            .Select(g => new
                            {
                                id_company = g.id_company,
                                payment_type = g.payment_type != null ? g.payment_type.name : "",
                                comments = g.comment,
                                company_name = g.app_company != null ? g.app_company.name : "",
                                amount = g.value,
                                contact_name = g.payment_approve.contact != null ? g.payment_approve.contact.name : "Not Ref",
                                gov_id = g.payment_approve.contact != null ? g.payment_approve.contact.gov_code : "",
                                payment_name = g.payment_type != null ? g.payment_type.name : "",
                                trans_date = g.payment_approve != null ? g.payment_approve.trans_date : DateTime.Now,
                                currency_name = g.app_currency != null ? g.app_currency != null ? g.app_currency.name : "" : "",
                                number = g.payment_approve != null ? g.payment_approve.number : "Not Ref",
                                //SalesNumber = g.payment_schedual != null ? g.payment_schedual.sales_invoice != null ? g.payment_schedual.sales_invoice.number : "" : "",
                                PurchaseNumber = g.payment_schedual != null ? g.payment_schedual.purchase_invoice != null ? g.payment_schedual.purchase_invoice.number : "" : "",
                                BankAccount = g.app_account != null ? g.app_account.name : "",
                                AmountWords = g != null ? g.app_currency != null ? g.app_currency.has_rounding ?

                    // Text -> Words
                    NumToWords.IntToText(Convert.ToInt32(g != null ? g.payment_approve.GrandTotal : 0))
                    :
                    NumToWords.DecimalToText((Convert.ToDecimal(g != null ? g.payment_approve.GrandTotal : 0))) : "" : "",

                                HasRounding = g != null ? g.app_currency != null ? g.app_currency.has_rounding : false : false
                            }).ToList();

            return reportDataSource;
        }

        public ReportDataSource Project(project project)
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

        public ReportDataSource Inventory(item_inventory item_inventory)
        {
            reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
            List<item_inventory_detail> item_inventory_detail = item_inventory.item_inventory_detail.ToList();

            reportDataSource.Value = item_inventory_detail
                .Select(g => new
                {
                    date = g.item_inventory.trans_date,
                    branch = g.item_inventory.app_branch.name,
                    comment = g.item_inventory.comment,
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