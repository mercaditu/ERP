using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace entity
{
    public partial class DocumentViewr : UserControl
    {
        db db = new db();

        public DocumentViewr()
        {
            InitializeComponent();
        }



        public void loadSalesPackingList(int id)
        {
            try
            {
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                sales_packing sales_packing = db.sales_packing.Where(x => x.id_sales_packing == id).FirstOrDefault();
                List<sales_packing_detail> sales_packing_detail = db.sales_packing_detail.Where(x => x.id_sales_packing == sales_packing.id_sales_packing).ToList();

                reportDataSource.Value = sales_packing_detail
                              .Select(g => new
                              {
                                  contact_name = g.sales_packing.contact.name,
                                  customer_address = g.sales_packing.contact.address,
                                  customer_telephone = g.sales_packing.contact.telephone,
                                  customer_email = g.sales_packing.contact.email,
                                  company_Name = g.app_company.name,
                                  sales_terminal = g.sales_packing.app_terminal.name,
                                  branch_Name = g.sales_packing.app_branch.name,
                                  security_user_name = g.sales_packing.security_user.name,
                                  trans_date = g.sales_packing.trans_date,
                                  sales_order_Comment = g.sales_packing.comment,
                                  item_code = g.item.code,
                                  item_description = g.item.name,
                                  item_brand = g.item.item_brand != null ? g.item.item_brand.name : "",
                                  quantity = g.quantity,

                              }).ToList();

                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = path + "\\CogntivoERP";
                string SubFolder = "";
                SubFolder = "\\TemplateFiles";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_PackingList.rdlc", path + SubFolder + "\\Sales_PackingList.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_PackingList.rdlc", path + SubFolder + "\\Sales_PackingList.rdlc");

                }
                else if (!File.Exists(path + SubFolder + "\\Sales_PackingList.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_PackingList.rdlc", path + SubFolder + "\\Sales_PackingList.rdlc");
                }

                if (!sales_packing.app_document_range.use_default_printer)
                {
                    
                    reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_PackingList.rdlc"; // Path of the rdlc file
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer.RefreshReport();

                }
                else
                {
                    try
                    {
                        if (sales_packing.app_document_range.printer_name != null)
                        {
                            LocalReport report = new LocalReport();
                            PrintInvoice PrintInvoice = new PrintInvoice();
                            report.ReportPath = path + SubFolder + "\\Sales_PackingList.rdlc"; // Path of the rdlc file
                            report.DataSources.Add(reportDataSource);
                            PrintInvoice.Export(report);
                            PrintInvoice.Print(sales_packing.app_document_range.printer_name);
                        }
                        else
                        {
                            NotSupportedException ex = new NotSupportedException();
                            throw ex;
                        }
                    }
                    catch
                    {
                        reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_PackingList.rdlc"; // Path of the rdlc file
                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer.RefreshReport();
                        Window window = new Window
                        {
                            Title = "Report",
                            Content = this
                        };

                        window.ShowDialog();
                    }
                }



            }
            catch { }

        }


        public void loadSalesOrderReport(int id)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";
            ReportDataSource reportDataSource = new ReportDataSource();
            try
            {
              

               
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                sales_order sales_order = db.sales_order.Where(x => x.id_sales_order == id).FirstOrDefault();
                List<sales_order_detail> sales_order_detail = db.sales_order_detail.Where(x => x.id_sales_order == sales_order.id_sales_order).ToList();

                reportDataSource.Value = sales_order_detail
                              .Select(g => new
                              {
                                  sales_budget_number = g.sales_order.sales_budget != null ? g.sales_order.sales_budget.number : "",
                                  contact_name = g.sales_order.contact.name,
                                  customer_address = g.sales_order.contact.address,
                                  customer_telephone = g.sales_order.contact.telephone,
                                  customer_email = g.sales_order.contact.email,
                                  company_Name = g.app_company.name,
                                  sales_order_terminal = g.sales_order.app_terminal.name,
                                  branch_Name = g.sales_order.app_branch.name,
                                  order_Code = g.sales_order.code,
                                  delivery_Date = g.sales_order.delivery_date,
                                  sales_number = g.sales_order.number,
                                  order_Total = g.sales_order.GrandTotal,
                                  project_Name = g.sales_order.project != null ? g.sales_order.project.name : "",
                                  sales_order_representative = g.sales_order.sales_rep != null ? g.sales_order.sales_rep.name : "",
                                  security_user_name = g.sales_order.security_user.name,
                                  trans_date = g.sales_order.trans_date,
                                  sales_order_contract = g.sales_order.app_contract.name,
                                  sales_order_condition = g.sales_order.app_condition.name,
                                  DeliveryDate = g.sales_order.delivery_date,
                                  sales_order_Comment = g.sales_order.comment,
                                  item_code = g.item.code,
                                  item_description = g.item.name,
                                  item_brand = g.item.item_brand != null ? g.item.item_brand.name : "",
                                  quantity = g.quantity,
                                  sub_Total = g.SubTotal,
                                  sub_Total_vat = g.SubTotal_Vat,
                                  unit_cost = g.unit_cost,
                                  unit_price = g.unit_price,
                                  unit_price_vat = g.UnitPrice_Vat,
                              }).ToList();

              
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Order.rdlc", path + SubFolder + "\\Sales_Order.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Order.rdlc", path + SubFolder + "\\Sales_Order.rdlc");

                }
                else if (!File.Exists(path + SubFolder + "\\Sales_Order.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Order.rdlc", path + SubFolder + "\\Sales_Order.rdlc");
                }

              


                    if (!sales_order.app_document_range.use_default_printer)
                    {
                        reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Order.rdlc"; // Path of the rdlc file
                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer.RefreshReport();
                        Window window = new Window
                        {
                            Title = "Report",
                            Content = this
                        };

                        window.ShowDialog();
                    }
                    else
                    {
                        try
                        {
                            if (sales_order.app_document_range.printer_name != null)
                            {
                                LocalReport report = new LocalReport();
                                PrintInvoice PrintInvoice = new PrintInvoice();
                                report.ReportPath = path + SubFolder + "\\Sales_Order.rdlc"; // Path of the rdlc file
                                report.DataSources.Add(reportDataSource);
                                PrintInvoice.Export(report);
                                PrintInvoice.Print(sales_order.app_document_range.printer_name);
                            }
                            else
                            {
                                NotSupportedException ex = new NotSupportedException();
                                throw ex;
                            }
                        }
                        catch
                        {
                            reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Order.rdlc"; // Path of the rdlc file
                            reportViewer.LocalReport.DataSources.Add(reportDataSource);
                            reportViewer.RefreshReport();
                            Window window = new Window
                            {
                                Title = "Report",
                                Content = this
                            };

                            window.ShowDialog();
                        }
                    }
                



            }
            catch
            {
                reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Order.rdlc"; // Path of the rdlc file
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();
                Window window = new Window
                {
                    Title = "Report",
                    Content = this
                };

                window.ShowDialog();
            }

        }

        public void loadSalesInvoiceReport(int id)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";
            ReportDataSource reportDataSource = new ReportDataSource();
            try
            {

               
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                //find the all sales invoice Detail
                List<sales_invoice_detail> sales_invoice_detail = db.sales_invoice_detail.Where(x => x.id_sales_invoice == id).ToList();
                reportDataSource.Value = sales_invoice_detail
                              .Select(g => new
                              {
                                  //id_sales_invoice = g.id_sales_invoice,
                                  //id_sales_invoice_detail = g.id_sales_invoice_detail,
                                  sales_invoice = g.id_sales_invoice,
                                  id_company = g.id_company,
                                  add1 = g.sales_invoice.contact.address,
                                  telephone = g.sales_invoice.contact.telephone,
                                  email = g.sales_invoice.contact.email,
                                  company_name = g.app_company.name,
                                  item_code = g.item.code,
                                  item_description = g.item.name,
                                  Description = g.item.item_brand != null ? g.item.item_brand.name : "",
                                  quantity = g.quantity,
                                  sub_Total = g.SubTotal,
                                  sub_Total_vat = g.SubTotal_Vat,
                                  unit_cost = g.unit_cost,
                                  unit_price = g.unit_price,
                                  unit_price_vat = g.UnitPrice_Vat,
                                  terminal_name = g.sales_invoice.app_terminal.name,
                                  code = g.sales_invoice.code,
                                  customer_contact_name = g.sales_invoice.contact.name,
                                  customer_code = g.sales_invoice.contact.code,
                                  customer_alias = g.sales_invoice.contact.alias,
                                  project_name = g.sales_invoice.project != null ? g.sales_invoice.project.name : "",
                                  sales_invoice_rep_name = g.sales_invoice.sales_rep != null ? g.sales_invoice.sales_rep.name : "",
                                  trans_date = g.sales_invoice.trans_date,
                                  id_vat_group = g.id_vat_group,
                                  gov_id = g.sales_invoice.contact.gov_code,
                                  sales_invoice_contract = g.sales_invoice.app_contract.name,
                                  sales_invoice_condition = g.sales_invoice.app_contract.app_condition.name,
                                  sales_number = g.sales_invoice.number,
                                  sales_invoice_Comment = g.sales_invoice.comment
                              }).ToList();

                //copy report to the template folder
                 path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = path + "\\CogntivoERP";
                SubFolder = "";
                SubFolder = "\\TemplateFiles";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Sales_Invoice.rdlc", path + SubFolder + "\\Sales_Invoice.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Sales_Invoice.rdlc", path + SubFolder + "\\Sales_Invoice.rdlc");
                }
                else if (!File.Exists(path + SubFolder + "\\Sales_Invoice.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Sales_Invoice.rdlc", path + SubFolder + "\\Sales_Invoice.rdlc");
                }


                //print report as per the use_default_printer
                if (!sales_invoice_detail.FirstOrDefault().sales_invoice.app_document_range.use_default_printer)
                {
                    reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Invoice.rdlc"; // Path of the rdlc file
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer.RefreshReport();
                }
                else
                {
                    try
                    {
                        if (sales_invoice_detail.FirstOrDefault().sales_invoice.app_document_range.printer_name != null)
                        {
                            LocalReport report = new LocalReport();
                            PrintInvoice PrintInvoice = new PrintInvoice();
                            report.ReportPath = path + SubFolder + "\\Sales_Invoice.rdlc"; // Path of the rdlc file
                            report.DataSources.Add(reportDataSource);
                            PrintInvoice.Export(report);
                            PrintInvoice.Print(sales_invoice_detail.FirstOrDefault().sales_invoice.app_document_range.printer_name);
                        }
                        else
                        {
                            NotSupportedException ex = new NotSupportedException();
                            throw ex;
                        }
                    }
                    catch
                    {
                        reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Invoice.rdlc"; // Path of the rdlc file
                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer.RefreshReport();
                        Window window = new Window
                        {
                            Title = "Report",
                            Content = this
                        };

                        window.ShowDialog();
                    }

                }



            }
            catch
            {
                

                reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Invoice.rdlc"; // Path of the rdlc file
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();
                Window window = new Window
                {
                    Title = "Report",
                    Content = this
                };

                window.ShowDialog();
            }
        }

        public void loadSalesBudgetReport(int id)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";
            ReportDataSource reportDataSource = new ReportDataSource();
            try
            {
              
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                sales_budget sales_budget = db.sales_budget.Where(x => x.id_sales_budget == id).FirstOrDefault();
                List<sales_budget_detail> sales_budget_detail = db.sales_budget_detail.Where(x => x.id_sales_budget == sales_budget.id_sales_budget).ToList();

                reportDataSource.Value = sales_budget_detail
                              .Select(g => new
                              {
                                  id_sales_budget = g.id_sales_budget,
                                  id_sales_budget_detail = g.id_sales_budget_detail,
                                  sales_budget = g.id_sales_budget_detail,
                                  id_company = g.id_company,
                                  add1 = g.sales_budget.contact.address,
                                  telephone = g.sales_budget.contact.telephone,
                                  email = g.sales_budget.contact.email,
                                  company_name = g.app_company.name,
                                  item_code = g.item.code,
                                  item_description = g.item.name,
                                  item_brand = g.item.item_brand != null ? g.item.item_brand.name : "",
                                  quantity = g.quantity,
                                  sub_Total = g.SubTotal,
                                  sub_Total_vat = g.SubTotal_Vat,
                                  unit_cost = g.unit_cost,
                                  unit_price = g.unit_cost,
                                  unit_price_vat = g.UnitPrice_Vat,
                                  terminale_name = g.sales_budget.app_terminal.name,
                                  code = g.sales_budget.code,
                                  contact_name = g.sales_budget.contact.name,
                                  //project_name = g.sales_budget.project != null ? g.sales_budget.project.name : "",
                                  sales_rep_name = g.sales_budget.sales_rep != null ? g.sales_budget.sales_rep.name : "",
                                  trans_date = g.sales_budget.trans_date,
                                  id_vat_group = g.id_vat_group,
                                  gov_id = g.sales_budget.contact.gov_code,
                                  contract = g.sales_budget.app_contract.name,
                                  condition = g.sales_budget.app_contract.app_condition.name,
                                  Number = g.sales_budget.number,
                                  comment = g.sales_budget.comment
                              }).ToList();

              
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Budget.rdlc", path + SubFolder + "\\Sales_Budget.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Budget.rdlc", path + SubFolder + "\\Sales_Budget.rdlc");

                }
                else if (!File.Exists(path + SubFolder + "\\Sales_Budget.rdlc"))
                {
                    File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Budget.rdlc", path + SubFolder + "\\Sales_Budget.rdlc");
                }
                if (!sales_budget.app_document_range.use_default_printer)
                {
                    reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Budget.rdlc"; // Path of the rdlc file
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer.RefreshReport();
                }
                else
                {
                    try
                    {
                        if (sales_budget.app_document_range.printer_name != null)
                        {
                            LocalReport report = new LocalReport();
                            PrintInvoice PrintInvoice = new PrintInvoice();
                            report.ReportPath = path + SubFolder + "\\Sales_Budget.rdlc"; // Path of the rdlc file
                            report.DataSources.Add(reportDataSource);
                            PrintInvoice.Export(report);
                            PrintInvoice.Print(sales_budget.app_document_range.printer_name);

                        }
                        else
                        {
                            NotSupportedException ex = new NotSupportedException();
                            throw ex;
                        }
                    }
                    catch
                    {
                        reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Budget.rdlc"; // Path of the rdlc file
                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer.RefreshReport();
                        Window window = new Window
                        {
                            Title = "Report",
                            Content = this
                        };

                        window.ShowDialog();
                    }
                }
            }
            catch
            {
                reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Budget.rdlc"; // Path of the rdlc file
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();
                Window window = new Window
                {
                    Title = "Report",
                    Content = this
                };

                window.ShowDialog();
            }
        }

        public void loadSalesReturnReport(int id)
        {
            try
            {
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                sales_return sales_return = db.sales_return.Where(x => x.id_sales_return == id).FirstOrDefault();
                List<sales_return_detail> sales_return_detail = db.sales_return_detail.Where(x => x.id_sales_return == sales_return.id_sales_return).ToList();

                reportDataSource.Value = sales_return_detail
                              .Select(g => new
                              {
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
                              }).ToList();

                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = path + "\\CogntivoERP";
                string SubFolder = "";
                SubFolder = "\\TemplateFiles";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Return.rdlc", path + SubFolder + "\\Sales_Return.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Return.rdlc", path + SubFolder + "\\Sales_Return.rdlc");

                }
                else if (!File.Exists(path + SubFolder + "\\Sales_Return.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Sales_Return.rdlc", path + SubFolder + "\\Sales_Return.rdlc");
                }
                if (!sales_return.app_document_range.use_default_printer)
                {
                    reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Return.rdlc"; // Path of the rdlc file
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer.RefreshReport();
                }
                else
                {
                    try
                    {
                        if (sales_return.app_document_range.printer_name != null)
                        {
                            LocalReport report = new LocalReport();
                            PrintInvoice PrintInvoice = new PrintInvoice();
                            report.ReportPath = path + SubFolder + "\\Sales_Return.rdlc"; // Path of the rdlc file
                            report.DataSources.Add(reportDataSource);
                            PrintInvoice.Export(report);
                            PrintInvoice.Print(sales_return.app_document_range.printer_name);
                        }
                        else
                        {
                            NotSupportedException ex = new NotSupportedException();
                            throw ex;
                        }
                    }
                    catch
                    {
                        reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Sales_Return.rdlc"; // Path of the rdlc file
                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer.RefreshReport();
                        Window window = new Window
                        {
                            Title = "Report",
                            Content = this
                        };

                        window.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void loadPurchaseOrderReport(int id)
        {
            try
            {

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                purchase_order purchase_order = db.purchase_order.Where(x => x.id_purchase_order == id).FirstOrDefault();
                List<purchase_order_detail> purchase_order_detail = db.purchase_order_detail.Where(x => x.id_purchase_order == purchase_order.id_purchase_order).ToList();

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
                                  //project_name = g.sales_budget.project != null ? g.sales_budget.project.name : "",
                                  //sales_rep_name = g.purchase_order.sales_rep != null ? g.purchase_order.sales_rep.name : "",
                                  trans_date = g.purchase_order.trans_date,
                                  id_vat_group = g.id_vat_group,
                                  gov_id = g.purchase_order.contact.gov_code,
                                  //contract = g.sales_return.app_contract.name,
                                  //condition = g.sales_return.app_contract.app_condition.name,
                                  Number = g.purchase_order.number,
                                  //Comment = g.purchase_order.comment
                              }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void loadPaymentRecieptReport(int id)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";
            ReportDataSource reportDataSource = new ReportDataSource();
            try
            {


                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                //find the all sales invoice Detail
                List<payment_detail> payment_detail = db.payment_detail.Where(x => x.id_payment == id).ToList();
                reportDataSource.Value = payment_detail
                              .Select(g => new
                              {
                                  id_company = g.id_company,
                                  company_name = g.app_company.name,
                                  amount = g.value,
                                  contact_name = g.payment.contact.name,
                                  payment_name = g.payment_type.name,
                                  trans_date = g.trans_date,
                                  currency_name=g.app_currencyfx.app_currency.name
                              }).ToList();

                //copy report to the template folder

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "payment.rdlc", path + SubFolder + "\\payment.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "payment.rdlc", path + SubFolder + "\\payment.rdlc");
                }
                else if (!File.Exists(path + SubFolder + "\\payment.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "payment.rdlc", path + SubFolder + "\\payment.rdlc");
                }


                //print report as per the use_default_printer

                reportViewer.LocalReport.ReportPath = path + SubFolder + "\\payment.rdlc"; // Path of the rdlc file
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();
            }
            catch
            { }
        }


        public void loadCarnetcontactReport(int id)
        {
            try
            {
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                contact contact = db.contacts.Where(x => x.id_contact == id).FirstOrDefault();
                if ( contact.child.Count >0)
                {
                    List<contact> contact_detail = contact.child.ToList();
                    reportDataSource.Value = contact_detail
                       .Select(g => new
                       {
                           id_contact = g.id_contact,
                           contacts_name = g.parent.name,
                           date_birth = g.parent.date_birth,
                           gove_code = g.parent.gov_code,
                           trans_date = g.parent.timestamp,
                           contacts_code = g.parent.code,
                           Product_code=g.parent.contact_subscription.FirstOrDefault().item.name,
                           name = g.name
                       }).ToList();
                }
                else
                {
                    List<contact> contact_detail = new List<entity.contact>();
                    contact_detail.Add(contact);
                    reportDataSource.Value = contact_detail
                       .Select(g => new
                       {
                           id_contact = g.id_contact,
                           contacts_name = g.name,
                           date_birth = g.date_birth,
                           gove_code = g.gov_code,
                           trans_date = g.timestamp,
                           contacts_code = g.code,
                           Product_code = g.contact_subscription.FirstOrDefault().item.name,
                           name = ""
                       }).ToList();
                }

                if (contact.child.Count==0)
                {
                   
                      
                }
                else
                {
                  
                }
           

                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = path + "\\CogntivoERP";
                string SubFolder = "";
                SubFolder = "\\TemplateFiles";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact.rdlc", path + SubFolder + "\\Carnet_Contact.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact.rdlc", path + SubFolder + "\\Carnet_Contact.rdlc");

                }
                else if (!File.Exists(path + SubFolder + "\\Carnet_Contact.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact.rdlc", path + SubFolder + "\\Carnet_Contact.rdlc");
                }

                reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Carnet_Contact.rdlc"; // Path of the rdlc file
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer.RefreshReport();
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


