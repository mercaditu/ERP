using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Printing;
using System.Windows.Media;

namespace entity.Brillo.Logic
{
    public class Document
    {
        /// <summary>
        /// Print Sales Document
        /// </summary>
        /// <param name="app_document"></param>
        /// <param name="sales_invoice"></param>
        public void Document_PrintInvoice(app_document app_document, sales_invoice sales_invoice)
        {
            if (app_document != null)
            {
                if (sales_invoice.app_document_range.printer_name != null)
                {
                    DocumentViewr MainWindow = new DocumentViewr();
                    MainWindow.loadSalesInvoiceReport(sales_invoice.id_sales_invoice);
                }
                else
                {
                    DocumentViewr MainWindow = new DocumentViewr();
                    MainWindow.loadSalesInvoiceReport(sales_invoice.id_sales_invoice);

                    Window window = new Window
                    {
                        Title = "Report",
                        Content = MainWindow
                    };
                    window.ShowDialog();
                }
            }
            else
            {
                DocumentViewr MainWindow = new DocumentViewr();
                MainWindow.loadSalesInvoiceReport(sales_invoice.id_sales_invoice);

                Window window = new Window
                {
                    Title = "Report",
                    Content = MainWindow
                };

                window.ShowDialog();
            }
        }


        public void Document_PrintPaymentReceipt(payment payment)
        {
            DocumentViewr MainWindow = new DocumentViewr();
            MainWindow.loadPaymentRecieptReport(payment.id_payment);

            Window window = new Window
            {
                Title = "Report",
                Content = MainWindow
            };

            window.ShowDialog();

        }

        //packinglist
        public void Document_PrintPackingList(int document_id, sales_packing sales_packing)
        {
            app_document app_document;
            using (db db = new db())
            {
                app_document = db.app_document.Where(x => x.id_document == document_id).FirstOrDefault();
            }

            if (app_document != null)
            {
                if (app_document.style_reciept == true)
                {
                    string header = app_document.reciept_header;
                    string Mainheader = "";
                    foreach (sales_packing_detail sales_packing_detail in sales_packing.sales_packing_detail)
                    {
                        Mainheader += header.Replace("<<item>>", sales_packing_detail.item.name);
                        Mainheader += header.Replace("<<debit>>", sales_packing_detail.quantity.ToString());
                        Mainheader += header.Replace("<<Credit>>", "0");
                        Mainheader += header.Replace("<<Transcation>>", sales_packing.trans_date.ToString());
                        Mainheader += "\n";
                    }

                    string footer = app_document.reciept_footer;
                    string Mainfooter = "";
                    foreach (sales_packing_detail sales_packing_detail in sales_packing.sales_packing_detail)
                    {
                        Mainfooter += footer.Replace("<<item>>", sales_packing_detail.item.name);
                        Mainfooter += footer.Replace("<<debit>>", sales_packing_detail.quantity.ToString());
                        Mainfooter += footer.Replace("<<Credit>>", "0");
                        Mainfooter += footer.Replace("<<Transcation>>", sales_packing.trans_date.ToString());
                        Mainfooter += "\n";
                    }

                    string body = app_document.reciept_body;
                    string Mainbody = "";
                    foreach (sales_packing_detail sales_packing_detail in sales_packing.sales_packing_detail)
                    {

                        Mainbody += body.Replace("<<item>>", sales_packing_detail.item.name);
                        Mainbody += body.Replace("<<debit>>", sales_packing_detail.quantity.ToString());
                        Mainbody += body.Replace("<<Credit>>", "0");
                        Mainbody += body.Replace("<<Transcation>>", sales_packing.trans_date.ToString());
                        Mainbody += "\n";
                    }


                    PrintDialog pd = new PrintDialog();
                    FlowDocument doc = new FlowDocument(new Paragraph(new Run(Mainheader + "\n\n" + Mainbody + "\n\n" + Mainfooter)));
                    doc.Name = "FlowDoc";
                    IDocumentPaginatorSource idpSource = doc;
                    pd.PrintDocument(idpSource.DocumentPaginator, Mainheader + "\n\n" + Mainbody + "\n\n" + Mainfooter);

                }
                else if (app_document.style_printer == true)
                {
                    if (sales_packing.app_document_range.printer_name != null)
                    {
                        DocumentViewr MainWindow = new DocumentViewr();
                        MainWindow.loadSalesPackingList(sales_packing.id_sales_packing);
                    }
                    else
                    {
                        DocumentViewr MainWindow = new DocumentViewr();
                        MainWindow.loadSalesPackingList(sales_packing.id_sales_packing);

                        Window window = new Window
                        {
                            Title = "Report",
                            Content = MainWindow
                        };

                        window.ShowDialog();
                    }
                }
            }
        }


        //Sales order
        public void Document_PrintOrder(int document_id, sales_order sales_order, bool OverRide_AutoPrint)
        {
            try
            {
                if (sales_order.app_document_range.printer_name != null && OverRide_AutoPrint == false)
                {
                    DocumentViewr MainWindow = new DocumentViewr();
                    MainWindow.loadSalesOrderReport(sales_order.id_sales_order);
                }
                else // true
                {
                    NotSupportedException ex = new NotSupportedException();
                    throw ex;
                }
            }
            catch
            {
                DocumentViewr MainWindow = new DocumentViewr();
                MainWindow.loadSalesOrderReport(sales_order.id_sales_order);

                Window window = new Window
                {
                    Title = "Report",
                    Content = MainWindow
                };
                window.ShowDialog();
            }
        }

        //Purchase Order
        public void Document_PrintPurchaseOrder(int document_id, purchase_order purchase_order)
        {
            app_document app_document;
            using (db db = new db())
            {
                app_document = db.app_document.Where(x => x.id_document == document_id).FirstOrDefault();
            }
            if (app_document != null)
            {
                if (purchase_order.app_document_range.printer_name != null)
                {
                    DocumentViewr MainWindow = new DocumentViewr();
                    MainWindow.loadPurchaseOrderReport(purchase_order.id_purchase_order);
                }
                else
                {
                    DocumentViewr MainWindow = new DocumentViewr();
                    MainWindow.loadPurchaseOrderReport(purchase_order.id_purchase_order);
                    Window window = new Window
                    {
                        Title = "Report",
                        Content = MainWindow
                    };

                    window.ShowDialog();
                }
            }
        }

        //Sales Budget
        public void Document_PrintSalesBudget(int? document_id, sales_budget sales_budget)
        {
            if (sales_budget.app_document_range != null)
            {
                if (sales_budget.app_document_range.printer_name != null)
                {
                    DocumentViewr MainWindow = new DocumentViewr();
                    MainWindow.loadSalesBudgetReport(sales_budget.id_sales_budget);
                }
                else
                {
                    DocumentViewr MainWindow = new DocumentViewr();
                    MainWindow.loadSalesBudgetReport(sales_budget.id_sales_budget);
                    Window window = new Window
                    {
                        Title = "Document",
                        Content = MainWindow
                    };
                    window.ShowDialog();
                }
            }
            else
            {
                DocumentViewr MainWindow = new DocumentViewr();
                MainWindow.loadSalesBudgetReport(sales_budget.id_sales_budget);
                Window window = new Window
                {
                    Title = "Document",
                    Content = MainWindow
                };
                window.ShowDialog();
            }
        }

        //Sales return
        public void Document_PrintSalesReturn(int document_id, sales_return sales_return)
        {
            app_document app_document;
            using (db db = new db())
            {
                app_document = db.app_document.Where(x => x.id_document == document_id).FirstOrDefault();
            }

            if (app_document != null)
            {
                if (app_document.style_reciept == true)
                {
                    String header = app_document.reciept_header;
                    string Mainheader = "";
                    foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail)
                    {
                        Mainheader += header.Replace("<<item>>", sales_return_detail.item.name);
                        Mainheader += header.Replace("<<Location>>", sales_return_detail.app_location.name);
                        Mainheader += header.Replace("<<debit>>", sales_return_detail.quantity.ToString());
                        Mainheader += header.Replace("<<Credit>>", "0");
                        Mainheader += header.Replace("<<Transcation>>", sales_return.trans_date.ToString());
                        Mainheader += "\n";
                    }

                    String footer = app_document.reciept_footer;
                    string Mainfooter = "";
                    foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail)
                    {

                        Mainfooter += footer.Replace("<<item>>", sales_return_detail.item.name);
                        Mainfooter += footer.Replace("<<Location>>", sales_return_detail.app_location.name);
                        Mainfooter += footer.Replace("<<debit>>", sales_return_detail.quantity.ToString());
                        Mainfooter += footer.Replace("<<Credit>>", "0");
                        Mainfooter += footer.Replace("<<Transcation>>", sales_return.trans_date.ToString());
                        Mainfooter += "\n";
                    }

                    String body = app_document.reciept_body;
                    string Mainbody = "";
                    foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail)
                    {

                        Mainbody += body.Replace("<<item>>", sales_return_detail.item.name);
                        Mainbody += body.Replace("<<Location>>", sales_return_detail.app_location.name);
                        Mainbody += body.Replace("<<debit>>", sales_return_detail.quantity.ToString());
                        Mainbody += body.Replace("<<Credit>>", "0");
                        Mainbody += body.Replace("<<Transcation>>", sales_return.trans_date.ToString());
                        Mainbody += "\n";
                    }


                    PrintDialog pd = new PrintDialog();
                    FlowDocument doc = new FlowDocument(new Paragraph(new Run(Mainheader + "\n\n" + Mainbody + "\n\n" + Mainfooter)));
                    doc.Name = "FlowDoc";
                    IDocumentPaginatorSource idpSource = doc;
                    pd.PrintDocument(idpSource.DocumentPaginator, Mainheader + "\n\n" + Mainbody + "\n\n" + Mainfooter);

                }
                else if (app_document.style_printer == true)
                {

                    if (sales_return.app_document_range.printer_name != null)
                    {
                        DocumentViewr MainWindow = new DocumentViewr();
                        MainWindow.loadSalesReturnReport(sales_return.id_sales_return);
                    }
                    else
                    {
                        DocumentViewr MainWindow = new DocumentViewr();
                        MainWindow.loadSalesReturnReport(sales_return.id_sales_return);
                        Window window = new Window
                        {
                            Title = "Report",
                            Content = MainWindow
                        };

                        window.ShowDialog();
                    }
                }
            }
        }

        public void Document_PrintItemRequest(int document_id, item_transfer item_transfer)
        {
            app_document app_document;
            string PrinterName;

            using (db db = new db())
            {
                app_document = db.app_document.Where(x => x.id_document == document_id).FirstOrDefault();
                PrinterName = app_document.app_document_range.FirstOrDefault().printer_name;
            }

            if (app_document != null && PrinterName != string.Empty)
            {
                if (app_document.style_reciept == true)
                {
                    Reciept Reciept = new Reciept();
                    PrintDialog pd = new PrintDialog();

                    FlowDocument document = new FlowDocument(new Paragraph(new Run(Reciept.ItemMovement(item_transfer))));
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
                    pd.PrintDocument(idpSource.DocumentPaginator, Reciept.ItemMovement(item_transfer));
                }
            }
        }


        public void Document_PrintCarnetContact(contact contact)
        {

            DocumentViewr MainWindow = new DocumentViewr();
            MainWindow.loadCarnetcontactReport(contact.id_contact);
            Window window = new Window
            {
                Title = "Report",
                Content = MainWindow
            };

            window.ShowDialog();
        }
    }
}
