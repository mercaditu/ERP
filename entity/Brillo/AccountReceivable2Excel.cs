using ClosedXML.Excel;
using entity.Controller.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace entity.Brillo
{
    public class AccountReceivable2Excel
    {
        PaymentDB db = new PaymentDB();
        //PaymentDB.payment_schedual
        //           .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
        //                && (x.id_sales_invoice > 0 || x.id_sales_order > 0)
        //                && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0
        //                && (x.contact.name.Contains(query) || x.contact.gov_code.Contains(query) || x.contact.code.Contains(query)))

        //                .Include(x => x.sales_invoice)
        //                .Include(x => x.contact)
        //                .OrderBy(x => x.expire_date)
        //                .Load();
        public bool Read(string Path)
        {
            List<ExcelList> DisplayList = new List<ExcelList>();
            if (string.IsNullOrEmpty(Path) == false)
            {
                XLWorkbook workbook = new XLWorkbook(Path);
                foreach (var ws in workbook.Worksheets)
                {
                    //Ignore 1st Row due to Header
                    foreach (IXLRow row in ws.RowsUsed().Where(x => x.RowNumber() > 1))
                    {
                       
                        string gov_code = Convert.ToString(row.Cell(2).Value).ToLower();
                        string name = Convert.ToString(row.Cell(1).Value).ToLower();
                        contact customer = db.contacts.Where(x => x.gov_code.ToLower() == gov_code || x.name.ToLower() == name).FirstOrDefault();
                        if (customer != null)
                        {
                            foreach (IXLColumn col in ws.ColumnsUsed().Where(x => x.ColumnNumber() > 3))
                            {

                                if (row.Cell(col.ColumnNumber()).Value.ToString() != "")
                                {
                                    int colnumber = col.ColumnNumber();
                                    List<payment_schedual> paymentSchedualList = db.payment_schedual
                                     .Where(x => x.expire_date.Month == colnumber -3)
                                     .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
                                          && (x.id_sales_invoice > 0 || x.id_sales_order > 0)
                                          && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0
                                          && (x.contact.gov_code == customer.gov_code))
                                           .Include(x => x.sales_invoice)
                                           .Include(x => x.sales_order)
                                           .Include(x => x.contact)
                                           .OrderBy(x => x.expire_date)
                                           .ToList();
                                    if (paymentSchedualList.Count() > 0)
                                    {
                                        ExcelList exceldata = new ExcelList();
                                        exceldata.name = customer.name;
                                        exceldata.taxId = customer.gov_code;
                                        exceldata.month = ws.FirstRowUsed().Cell(colnumber).Value.ToString();
                                        exceldata.amount = row.Cell(col.ColumnNumber()).GetValue<int>();
                                        exceldata.customer = customer;
                                        exceldata.paymentSchedualList = paymentSchedualList;
                                        exceldata.status = ExcelList.Status.Found;
                                        DisplayList.Add(exceldata);

                                       
                                    }
                                    else
                                    {
                                        ExcelList exceldata = new ExcelList();
                                        exceldata.name = customer.name;
                                        exceldata.taxId = customer.gov_code;
                                        exceldata.month = ws.FirstRowUsed().Cell(colnumber).Value.ToString();
                                        exceldata.amount = row.Cell(col.ColumnNumber()).GetValue<int>();
                                        exceldata.customer = customer;
                                        exceldata.paymentSchedualList = paymentSchedualList;
                                        exceldata.status = ExcelList.Status.PaymentNotFound;
                                        DisplayList.Add(exceldata);

                                    }
                                }
                               
                            }
                            
                        }
                        else
                        {
                            ExcelList exceldata = new ExcelList();
                            exceldata.name = name;
                            exceldata.taxId = gov_code;
                            exceldata.status = ExcelList.Status.NotFound;
                            DisplayList.Add(exceldata);
                        }

                       

                    }

                  


                }

                ExcelPaymentViewer excelPaymentViewer = new ExcelPaymentViewer();
                excelPaymentViewer.paymnetlist = DisplayList;
                Window window = new Window
                {
                    Title = "Report",
                    Content = excelPaymentViewer
                };

               bool? isPay= window.ShowDialog();

                if (isPay == true)
                {
                    foreach (ExcelList item in DisplayList.Where(x=>x.status==ExcelList.Status.Found).ToList())
                    {
                        payment payment = new payment();
                        payment.contact = item.customer;
                        payment_detail payment_detail = new payment_detail();
                        payment_detail.value = item.amount;
                        payment_detail.payment = payment;
                        payment.payment_detail.Add(payment_detail);
                        db.MakePayment(item.paymentSchedualList, payment, true, false, false);
                        foreach (payment_schedual payment_schedual in item.paymentSchedualList)
                        {
                            if (payment_schedual.id_sales_order != null && payment_schedual.sales_order != null)
                            {
                                sales_order sales_order = payment_schedual.sales_order;
                                sales_invoice sales_invoice = new sales_invoice()
                                {
                                    barcode = sales_order.barcode,
                                    code = sales_order.code,
                                    trans_date = DateTime.Now,
                                    comment = sales_order.comment,
                                    id_condition = sales_order.id_condition,
                                    id_contact = sales_order.id_contact,
                                    contact = sales_order.contact,
                                    id_contract = sales_order.id_contract,
                                    id_currencyfx = sales_order.id_currencyfx,
                                    id_project = sales_order.id_project,
                                    id_sales_rep = sales_order.id_sales_rep,
                                    id_weather = sales_order.id_weather,
                                    is_impex = sales_order.is_impex,
                                    sales_order = sales_order
                                };

                                foreach (sales_order_detail sales_order_detail in sales_order.sales_order_detail)
                                {
                                    sales_invoice_detail sales_invoice_detail = new sales_invoice_detail()
                                    {
                                        comment = sales_order_detail.comment,
                                        discount = sales_order_detail.discount,
                                        id_item = sales_order_detail.id_item,
                                        item_description = sales_order_detail.item_description,
                                        id_location = sales_order_detail.id_location,
                                        id_project_task = sales_order_detail.id_project_task,
                                        id_sales_order_detail = sales_order_detail.id_sales_order_detail,
                                        id_vat_group = sales_order_detail.id_vat_group,
                                        quantity = sales_order_detail.quantity - (sales_order_detail.sales_invoice_detail != null ? sales_order_detail.sales_invoice_detail.Sum(x => x.quantity) : 0),
                                        unit_cost = sales_order_detail.unit_cost,
                                        unit_price = sales_order_detail.unit_price,
                                        movement_id = sales_order_detail.movement_id
                                    };

                                    //If both are null, then we can just ignore the whole code.
                                    if (sales_order_detail.expire_date != null || !string.IsNullOrEmpty(sales_order_detail.batch_code))
                                    {
                                        sales_invoice_detail.expire_date = sales_order_detail.expire_date;
                                        sales_invoice_detail.batch_code = sales_order_detail.batch_code;
                                    }

                                    sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                                }

                                db.sales_invoice.Add(sales_invoice);
                                crm_opportunity crm_opportunity = sales_order.crm_opportunity;
                                crm_opportunity.sales_invoice.Add(sales_invoice);
                                db.SaveChanges();
                            }
                        }

                    }
                }
                
                
            }

            return false;
        }



    }

    public class ExcelList
    {
       public enum Status
        {
            Found,
            NotFound,
            PaymentNotFound
        }
        public string name { get; set; }
        public string taxId { get; set; }
        public string month { get; set; }
        public decimal amount { get; set; }
        public Status status { get; set; }
        public contact customer { get; set; }

        public List<payment_schedual> paymentSchedualList { get; set; }

    }

}