using entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cntrl.Class
{
    public class CheckPaymentReApprove
    {
        public bool Start(db db, int ID, entity.App.Names Application)
        {
            return true;
        }

        public string Check_ContractChanges(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

                    if (Local_SalesInvoice.id_contract != OriginalSalesInvoice.id_contract)
                    {
                        String Message = "You Have Changed The Contract So Following Changes Required..\nThis Payment schedule deleted:";
                        List<payment> oldpayment = new List<payment>();
                        List<payment_schedual> oldSchedual = OriginalSalesInvoice.payment_schedual.ToList();
                        foreach (payment_schedual payment_schedual in oldSchedual)
                        {
                            Message += "\n " + payment_schedual.AccountPayableBalance;
                            if (payment_schedual.payment_detail != null)
                            {
                                oldpayment.Add(payment_schedual.payment_detail.payment);
                            }
                        }
                        Message += "\n Deleted Payment:";

                        foreach (payment payment in oldpayment)
                        {
                            Message += "\n " + payment.GrandTotalDetailValue;
                        }
                        Message += "Of Person " + Local_SalesInvoice.contact.name;
                        return Message;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_purchase_invoice = db.purchase_invoice.Find(ID);

                    if (Local_purchase_invoice.id_contract != OriginalPurchaseInvoice.id_contract)
                    {
                        String Message = "You Have Changed The Contract So Following Changes Required..\nThis Payment schedule deleted:";
                        List<payment> oldpayment = new List<payment>();
                        List<payment_schedual> oldSchedual = OriginalPurchaseInvoice.payment_schedual.ToList();
                        foreach (payment_schedual payment_schedual in oldSchedual)
                        {
                            Message += "\n " + payment_schedual.AccountPayableBalance;
                            if (payment_schedual.payment_detail != null)
                            {
                                oldpayment.Add(payment_schedual.payment_detail.payment);
                            }
                        }
                        Message += "\n Deleted Payment:";

                        foreach (payment payment in oldpayment)
                        {
                            Message += "\n " + payment.GrandTotalDetailValue;
                        }
                        Message += "Of Person " + Local_purchase_invoice.contact.name;
                        return Message;
                    }
                }
            }

            return "";
        }

        public string Check_ValueUP(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

                    if (OriginalSalesInvoice.GrandTotal < Local_SalesInvoice.GrandTotal)
                    {
                        decimal Value = 0;

                        Value = Local_SalesInvoice.GrandTotal - OriginalSalesInvoice.payment_schedual.Sum(x => x.debit);
                        String Message = "You Have Changed The Value of the invoice So Following Changes Required..\n";
                        payment_schedual payment_schedual = OriginalSalesInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            Message = "This Payment Schedual change to " + OriginalSalesInvoice.payment_schedual.Sum(x => x.debit) + "-->" + Local_SalesInvoice.GrandTotal;
                        }

                        return Message;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);

                    if (OriginalPurchaseInvoice.GrandTotal < Local_PurchaseInvoice.GrandTotal)
                    {
                        decimal Value = 0;

                        Value = Local_PurchaseInvoice.GrandTotal - OriginalPurchaseInvoice.payment_schedual.Sum(x => x.debit);
                        String Message = "You Have Changed The Value of the invoice So Following Changes Required..\n";
                        payment_schedual payment_schedual = OriginalPurchaseInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            Message = "This Payment Schedual change to " + OriginalPurchaseInvoice.payment_schedual.Sum(x => x.debit) + "-->" + Local_PurchaseInvoice.GrandTotal;
                        }

                        return Message;
                    }
                }
            }
            return "";
        }

        public string Check_ValueDown(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    if (OriginalSalesInvoice.GrandTotal > Local_SalesInvoice.GrandTotal)
                    {
                        decimal Value = 0;

                        Value = Local_SalesInvoice.GrandTotal - OriginalSalesInvoice.payment_schedual.Sum(x => x.debit);
                        String Message = "You Have Changed The Value of the invoice So Following Changes Required..\n";
                        payment_schedual payment_schedual = OriginalSalesInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            Message = "This Payment Schedual change to " + OriginalSalesInvoice.payment_schedual.Sum(x => x.debit) + "-->" + Local_SalesInvoice.GrandTotal;
                        }

                        return Message;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);
                    if (OriginalPurchaseInvoice.GrandTotal > Local_PurchaseInvoice.GrandTotal)
                    {
                        decimal Value = 0;

                        Value = Local_PurchaseInvoice.GrandTotal - OriginalPurchaseInvoice.payment_schedual.Sum(x => x.debit);
                        String Message = "You Have Changed The Value of the invoice So Following Changes Required..\n";
                        payment_schedual payment_schedual = OriginalPurchaseInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            Message = "This Payment Schedual change to " + OriginalPurchaseInvoice.payment_schedual.Sum(x => x.debit) + "-->" + Local_PurchaseInvoice.GrandTotal;
                        }

                        return Message;
                    }
                }
            }

            return "";
        }

        public string Check_CurrencyChange(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    if (OriginalSalesInvoice.id_currencyfx != Local_SalesInvoice.id_currencyfx)
                    {
                        String Message = "You Have Changed The Currency So Following Changes Required..\n";
                        List<payment_schedual> oldSchedual = OriginalSalesInvoice.payment_schedual.ToList();
                        Message += "This Schedule Will be Deleted..\n";
                        foreach (payment_schedual payment_schedual in oldSchedual)
                        {
                            Message += "\n " + payment_schedual.debit;
                        }
                        Message = "and New Schedule Inserted..\n";
                        return Message;
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);
                    if (OriginalPurchaseInvoice.id_currencyfx != Local_PurchaseInvoice.id_currencyfx)
                    {
                        String Message = "You Have Changed The Currency So Following Changes Required..\n";
                        List<payment_schedual> oldSchedual = OriginalPurchaseInvoice.payment_schedual.ToList();
                        Message += "This Schedule Will be Deleted..\n";
                        foreach (payment_schedual payment_schedual in oldSchedual)
                        {
                            Message += "\n " + payment_schedual.debit;
                        }
                        Message = "and New Schedule Inserted..\n";
                        return Message;
                    }
                }
            }

            return "";
        }

        public string Check_DateChange(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();

                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    String Message = "You Have Changed The Date So Following Changes Required..\n";
                    if (OriginalSalesInvoice.trans_date != Local_SalesInvoice.trans_date)
                    {
                        List<payment_schedual> oldSchedual = OriginalSalesInvoice.payment_schedual.ToList();
                        List<payment> oldpayment = new List<payment>();
                        List<app_contract_detail> app_contract_details = Local_SalesInvoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                        int i = 0;
                        Message += "This Schedule Will be Changed..\n";
                        foreach (app_contract_detail app_contract_detail in app_contract_details)
                        {
                            i = i + 1;
                            payment_schedual payment_schedual = oldSchedual.Skip(i - 1).Take(1).FirstOrDefault();
                            Message += payment_schedual.expire_date + "-->" + Local_SalesInvoice.trans_date.AddDays(app_contract_detail.interval);
                            return Message;
                        }
                    }
                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurchaseInvoice;

                using (db temp = new db())
                {
                    OriginalPurchaseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();

                    purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);
                    String Message = "You Have Changed The Date So Following Changes Required..\n";
                    if (OriginalPurchaseInvoice.trans_date != Local_PurchaseInvoice.trans_date)
                    {
                        List<payment_schedual> oldSchedual = OriginalPurchaseInvoice.payment_schedual.ToList();
                        List<payment> oldpayment = new List<payment>();
                        List<app_contract_detail> app_contract_details = Local_PurchaseInvoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                        int i = 0;
                        Message += "This Schedule Will be Changed..\n";
                        foreach (app_contract_detail app_contract_detail in app_contract_details)
                        {
                            i = i + 1;
                            payment_schedual payment_schedual = oldSchedual.Skip(i - 1).Take(1).FirstOrDefault();
                            Message += payment_schedual.expire_date + "-->" + Local_PurchaseInvoice.trans_date.AddDays(app_contract_detail.interval);
                            return Message;
                        }
                    }
                }
            }
            return "";
        }
    }
}