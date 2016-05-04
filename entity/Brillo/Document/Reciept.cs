

namespace entity.Brillo.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Printing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class Reciept
    {

        public string ItemMovement(item_transfer i)
        {
            string Header = string.Empty;
            string Detail = string.Empty;
            string Footer = string.Empty;

            string CompanyName = i.app_company.name;
            string TransNumber = i.number;
            DateTime TransDate = i.trans_date;
            string BranchName = i.app_location_origin.app_branch.name;

            string UserGiven = i.user_given.name_full;
            string DepartmentName = i.app_department.name;
            string ProjectName = i.project.name;
            string ProjectCode = i.project.code;

            Header =
                CompanyName + "\n"
                + "Registro de PND. Transaccion: " + TransNumber + "\n"
                + "Fecha y Hora: " + TransDate.ToString() + "\n"
                + "Local Expendido: " + BranchName + "\n"
                + "\n"
                + "Entrega: " + UserGiven + "\n"
                + "Sector: " + DepartmentName + "\n"
                + "Project: " + ProjectCode + " - " + ProjectName + "\n"
                + "-------------------------------"
                + "\n";

            foreach (item_transfer_detail d in i.item_transfer_detail)
            {
                Detail = "ACTIV. : " + d.project_task.parent.item_description + "\n";
                //foreach (project_task project_task in d.project_task.child)
                //{
                string ItemName = d.project_task.items.name;
                string ItemCode = d.project_task.code;
                decimal? Qty = d.project_task.quantity_est;
                string TaskName = d.project_task.item_description;

                Detail = Detail +
                    ""
                    + "Descripcion, Cantiad, Codigo" + "\n"
                    + "-------------------------------" + "\n"
                    + ItemName + "\n"
                    + Qty.ToString() + "\t" + ItemCode + "\t" + TaskName + "\n";
                //}

            }

            Footer = "-------------------------------";
            Footer += "RETIRADO: " + i.user_requested.name_full + "\n";
            Footer += "APRORADO: " + i.user_given.name_full + "\n";
            Footer += "-------------------------------";

            string Text = Header + Detail + Footer;
            return Text;
        }

        public string SalesReturn(sales_return sales_return)
        {
            string Header = string.Empty;
            string Detail = string.Empty;
            string Footer = string.Empty;
            string CompanyName = string.Empty;
            app_company app_company = null;
            if (sales_return.app_company != null)
            {
                CompanyName = sales_return.app_company.name;
            }
            else
            {
                using (db db = new db())
                {
                    if (db.app_company.Where(x => x.id_company == sales_return.id_company).FirstOrDefault() != null)
                    {
                        app_company = db.app_company.Where(x => x.id_company == sales_return.id_company).FirstOrDefault();
                        CompanyName = app_company.name;
                    }



                }

            }
            string UserGiven = "";
            if (sales_return.security_user != null)
            {
                UserGiven = sales_return.security_user.name;
            }
            else
            {
                using (db db = new db())
                {
                    if (db.security_user.Where(x => x.id_user == sales_return.id_user).FirstOrDefault() != null)
                    {
                        security_user security_user = db.security_user.Where(x => x.id_user == sales_return.id_user).FirstOrDefault();
                        UserGiven = security_user.name;
                    }
                }
            }

            string TransNumber = sales_return.number;
            DateTime TransDate = sales_return.trans_date;
            string BranchName = sales_return.app_branch.name;

            Header =
                CompanyName + "\n"
                + "RUC:" + app_company.gov_code + "\n"
                + app_company.address + "\n"
                + "***" + app_company.alias + "***" + "\n"
                + "Timbrado: " + sales_return.app_document_range.code + " Vto: " + sales_return.app_document_range.expire_date
                + "\n"
                + "--------------------------------"
                + "Descripcion, Cantiad, Precio" + "\n"
                + "--------------------------------" + "\n"
                + "\n";

            foreach (sales_return_detail d in sales_return.sales_return_detail)
            {
                string ItemName = d.item.name;
                string ItemCode = d.item.code;
                decimal? Qty = d.quantity;
                string TaskName = d.item_description;
                decimal? UnitPrice_Vat = Math.Round(d.UnitPrice_Vat, 2);

                Detail = Detail
                    + ItemName + "\n"
                    + Qty.ToString() + "\t" + ItemCode + "\t" + UnitPrice_Vat + "\n";
            }


            Footer = "--------------------------------" + "\n";
            Footer += "Total " + sales_return.app_currencyfx.app_currency.name + ": " + sales_return.GrandTotal + "\n";
            Footer += "Fecha & Hora: " + sales_return.trans_date + "\n";
            Footer += "Numero de Factura: " + sales_return.number + "\n";
            Footer += "-------------------------------" + "\n";

            if (sales_return != null)
            {
                List<sales_return_detail> sales_return_detail = sales_return.sales_return_detail.ToList();
                if (sales_return_detail.Count > 0)
                {

                    using (db db = new db())
                    {
                        var listvat = sales_return_detail
                          .Join(db.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                              , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
                              .GroupBy(a => new { a.name, a.id_vat, a.ad })
                      .Select(g => new
                      {
                          vatname = g.Key.ad.app_vat_group.name,
                          id_vat = g.Key.id_vat,
                          name = g.Key.name,
                          value = g.Sum(a => a.value * a.ad.quantity)
                      }).ToList();
                        var VAtList = listvat.GroupBy(x => x.id_vat).Select(g => new
                        {
                            vatname = g.Max(y => y.vatname),
                            id_vat = g.Max(y => y.id_vat),
                            name = g.Max(y => y.name),
                            value = g.Sum(a => a.value)
                        }).ToList();
                        foreach (dynamic item in VAtList)
                        {
                            Footer += item.vatname + "   : " + Math.Round(item.value, 2) + "\n";
                        }
                        Footer += "Total IVA: " + sales_return.app_currencyfx.app_currency.name + " " + Math.Round(VAtList.Sum(x => x.value), 2) + "\n";
                    }
                }
            }

            Footer += "-------------------------------";
            Footer += "Cliente   : " + sales_return.contact.name + "\n";
            Footer += "Documento : " + sales_return.contact.gov_code + "\n";
            Footer += "Condicion : " + sales_return.app_condition.name + "\n";
            Footer += "-------------------------------";
            Footer += "Sucursal    : " + sales_return.app_branch.name + " Terminal: " + sales_return.app_terminal.name + "\n";
            Footer += "Cajero/a    : " + UserGiven;

            string Text = Header + Detail + Footer;
            return Text;
        }

        public string SalesInvoice(sales_invoice sales_invoice)
        {
            string Header = string.Empty;
            string Detail = string.Empty;
            string Footer = string.Empty;
            string CompanyName = string.Empty;
            app_company app_company=null;
            if (sales_invoice.app_company != null)
            {
                CompanyName = sales_invoice.app_company.name;
            }
            else
            {
                using (db db = new db())
                {
                    if (db.app_company.Where(x => x.id_company == sales_invoice.id_company).FirstOrDefault() != null)
                    {
                         app_company = db.app_company.Where(x => x.id_company == sales_invoice.id_company).FirstOrDefault();
                        CompanyName = app_company.name;
                    }



                }

            }
            string UserGiven = "";
            if (sales_invoice.security_user != null)
            {
                UserGiven = sales_invoice.security_user.name;
            }
            else
            {
                using (db db = new db())
                {
                    if (db.security_user.Where(x => x.id_user == sales_invoice.id_user).FirstOrDefault() != null)
                    {
                        security_user security_user = db.security_user.Where(x => x.id_user == sales_invoice.id_user).FirstOrDefault();
                        UserGiven = security_user.name;
                    }
                }
            }

            string TransNumber = sales_invoice.number;
            DateTime TransDate = sales_invoice.trans_date;
            string BranchName = sales_invoice.app_branch.name;

            Header =
                CompanyName + "\n"
                + "RUC:" + app_company.gov_code + "\n"
                + app_company.address + "\n"
                + "***" + app_company.alias + "***" + "\n"
                + "Timbrado: " + sales_invoice.app_document_range.code + " Vto: " + sales_invoice.app_document_range.expire_date
                + "\n"
                + "--------------------------------"
                + "Descripcion, Cantiad, Precio" + "\n"
                + "--------------------------------" + "\n"
                + "\n";

            foreach (sales_invoice_detail d in sales_invoice.sales_invoice_detail)
            {
                string ItemName = d.item.name;
                string ItemCode = d.item.code;
                decimal? Qty = d.quantity;
                string TaskName = d.item_description;
                decimal? UnitPrice_Vat = Math.Round(d.UnitPrice_Vat,2);

                Detail = Detail
                    + ItemName + "\n"
                    + Qty.ToString() + "\t" + ItemCode + "\t" + UnitPrice_Vat + "\n";
            }


            Footer = "--------------------------------" + "\n"; 
            Footer += "Total " + sales_invoice.app_currencyfx.app_currency.name + ": " + sales_invoice.GrandTotal + "\n";
            Footer += "Total Descuento  :" + sales_invoice.sales_invoice_detail.Sum(x => x.Discount_SubTotal_Vat);
            Footer += "Fecha & Hora     : " + sales_invoice.trans_date + "\n";
            Footer += "Numero de Factura: " + sales_invoice.number + "\n";
            Footer += "-------------------------------" + "\n";

            if (sales_invoice != null)
            {
                List<sales_invoice_detail> sales_invoice_detail = sales_invoice.sales_invoice_detail.ToList();
                if (sales_invoice_detail.Count > 0)
                {

                    using (db db = new db())
                    {
                        var listvat = sales_invoice_detail
                          .Join(db.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                              , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
                              .GroupBy(a => new { a.name, a.id_vat, a.ad })
                      .Select(g => new
                      {
                          vatname = g.Key.ad.app_vat_group.name,
                          id_vat = g.Key.id_vat,
                          name = g.Key.name,
                          value = g.Sum(a => a.value * a.ad.quantity)
                      }).ToList();
                        var VAtList = listvat.GroupBy(x => x.id_vat).Select(g => new
                        {
                            vatname = g.Max(y => y.vatname),
                            id_vat = g.Max(y => y.id_vat),
                            name = g.Max(y => y.name),
                            value = g.Sum(a => a.value)
                        }).ToList();
                        foreach (dynamic item in VAtList)
                        {
                            Footer += item.vatname + "   : " + Math.Round(item.value,2) + "\n";
                        }
                        Footer += "Total IVA: " + sales_invoice.app_currencyfx.app_currency.name + " " + Math.Round(VAtList.Sum(x=>x.value),2) + "\n";
                    }
                }
            }
           
            Footer += "-------------------------------";
            Footer += "Cliente   : " + sales_invoice.contact.name + "\n";
            Footer += "Documento : " + sales_invoice.contact.gov_code + "\n";
            Footer += "Condicion : " + sales_invoice.app_condition.name + "\n";
            Footer += "-------------------------------";
            Footer += "Sucursal    : " + sales_invoice.app_branch.name + " Terminal: " + sales_invoice.app_terminal.name + "\n";
            Footer += "Vendedor/a    : " + sales_invoice.sales_rep != null ? sales_invoice.sales_rep.name : UserGiven;

            string Text = Header + Detail + Footer;
            return Text;
        }
        
        public string Payment(payment payment)
        {
            string Header = string.Empty;
            string Detail = string.Empty;
            string Footer = string.Empty;

            string CompanyName = string.Empty;
            app_company app_company = null;

            if (payment.app_company != null)
            {
                CompanyName = payment.app_company.name;
            }
            else
            {
                using (db db = new db())
                {
                    if (db.app_company.Where(x => x.id_company == payment.id_company).FirstOrDefault() != null)
                    {
                        app_company = db.app_company.Where(x => x.id_company == payment.id_company).FirstOrDefault();
                        CompanyName = app_company.name;
                    }
                }
            }

            string UserName = "";

            if (payment.security_user != null)
            {
                UserName = payment.security_user.name;
            }
            else
            {
                using (db db = new db())
                {
                    if (db.security_user.Where(x => x.id_user == payment.id_user).FirstOrDefault() != null)
                    {
                        security_user security_user = db.security_user.Where(x => x.id_user == payment.id_user).FirstOrDefault();
                        UserName = security_user.name;
                    }
                }
            }

            string TransNumber = payment.number;
            DateTime TransDate = payment.trans_date;
            string BranchName = payment.app_branch.name;

            Header =
                CompanyName + "\n"
                + "RUC:" + app_company.gov_code + "\n"
                + app_company.address + "\n"
                + "***" + app_company.alias + "***" + "\n"
                + "Timbrado: " + payment.app_document_range.code + " Vto: " + payment.app_document_range.expire_date
                + "\n"
                + "--------------------------------"
                + "Cuenta, Valor, Moneda" + "\n"
                + "--------------------------------" + "\n"
                + "\n";

            foreach (payment_detail d in payment.payment_detail)
            {
                string AccountName = d.app_account.name;
                
                decimal? value = d.value;
                string currency = d.app_currencyfx.app_currency.name;

                Detail = Detail
                    + AccountName + "\n"
                    + value.ToString() + "\t" + currency +  "\n";
            }

            Footer = "--------------------------------" + "\n";

            string Text = Header + Detail + Footer;
            return Text;
        }


        public void Document_Print(int document_id, object obj)
        {
            app_document app_document;
            string PrinterName;
            string Content = "";


            using (db db = new db())
            {
                app_document = db.app_document.Where(x => x.id_document == document_id).FirstOrDefault();
                PrinterName = app_document.app_document_range.FirstOrDefault().printer_name;
                if (app_document.id_application == App.Names.Movement)
                {
                    item_transfer item_transfer = (item_transfer)obj;
                    Content = ItemMovement(item_transfer);
                }
                else if (app_document.id_application == App.Names.SalesReturn)
                {
                    sales_return sales_return = (sales_return)obj;
                    Content = SalesReturn(sales_return);
                }
                else if (app_document.id_application == App.Names.SalesInvoice)
                {
                    sales_invoice sales_invoice = (sales_invoice)obj;
                    Content = SalesInvoice(sales_invoice);
                }
                else if (app_document.id_application == App.Names.PaymentUtility)
                {
                    payment payment = (payment)obj;
                    Content = Payment(payment);
                }
            }

            if (Content != "")
            {


                if (app_document != null && PrinterName != string.Empty)
                {
                    if (app_document.style_reciept == true)
                    {
                        Reciept Reciept = new Reciept();
                        PrintDialog pd = new PrintDialog();

                        FlowDocument document = new FlowDocument(new Paragraph(new Run(Content)));
                        document.Name = "ItemMovement";
                        document.FontFamily = new FontFamily("Courier New");
                        document.FontSize = 11.0;
                        document.FontStretch = FontStretches.Normal;
                        document.FontWeight = FontWeights.Normal;

                        document.PagePadding = new Thickness(20);

                        document.PageHeight = double.NaN;
                        document.PageWidth = double.NaN;
                        //document.

                        //Specify minimum page sizes. Origintally 283, but was too small.
                        document.MinPageWidth = 283;
                        //Specify maximum page sizes.
                        document.MaxPageWidth = 300;

                        IDocumentPaginatorSource idpSource = document;
                        pd.PrintQueue = new PrintQueue(new PrintServer(), PrinterName);
                        pd.PrintDocument(idpSource.DocumentPaginator, Content);
                    }
                }
            }
        }
    }
}
