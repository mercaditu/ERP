using System;
using System.Windows;
using entity;
using System.Linq;
using System.Collections.Generic;
using entity.Brillo.Logic;
namespace cntrl.Class
{
    public class UpdatePaymentReApprove
    {

        public bool Start(db db, int ID, entity.App.Names Application)
        {
            return true;
        }

        public void Update_ContractChanges(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);


                List<payment_schedual> oldSchedual = Local_SalesInvoice.payment_schedual.ToList();
                List<payment> oldpayment = new List<payment>();
                foreach (payment_schedual payment_schedual in oldSchedual)
                {
                    if (payment_schedual.payment_detail != null)
                    {
                        oldpayment.Add(payment_schedual.payment_detail.payment);
                    }

                }

                //add new schedual
                List<payment_schedual> payment_schedualList;
                Payment _Payment = new Payment();
                payment_schedualList = _Payment.insert_Schedual(Local_SalesInvoice);




                foreach (payment payment in oldpayment)
                {
                    foreach (payment_detail payment_detail in payment.payment_detail)
                    {
                        if (payment_detail.app_account != null)
                        {
                            db.app_account_detail.RemoveRange(payment_detail.app_account.app_account_detail);
                        }

                    }
                    db.payment_detail.RemoveRange(payment.payment_detail);
                }
                db.payments.RemoveRange(oldpayment);
                db.payment_schedual.RemoveRange(oldSchedual);
                db.payment_schedual.AddRange(payment_schedualList);
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice Local_purchase_invoice = db.purchase_invoice.Find(ID);


                List<payment_schedual> oldSchedual = Local_purchase_invoice.payment_schedual.ToList();
                List<payment> oldpayment = new List<payment>();
                foreach (payment_schedual payment_schedual in oldSchedual)
                {
                    if (payment_schedual.payment_detail != null)
                    {
                        oldpayment.Add(payment_schedual.payment_detail.payment);
                    }

                }

                //add new schedual
                List<payment_schedual> payment_schedualList;
                Payment _Payment = new Payment();
                payment_schedualList = _Payment.insert_Schedual(Local_purchase_invoice);




                foreach (payment payment in oldpayment)
                {
                    foreach (payment_detail payment_detail in payment.payment_detail)
                    {
                        if (payment_detail.app_account!=null)
                        {
                            db.app_account_detail.RemoveRange(payment_detail.app_account.app_account_detail);
                        }
                

                    }
                    db.payment_detail.RemoveRange(payment.payment_detail);
                }
                db.payments.RemoveRange(oldpayment);
                db.payment_schedual.RemoveRange(oldSchedual);
                db.payment_schedual.AddRange(payment_schedualList);
            }





        }

        public void Update_ValueUP(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();


                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

                    decimal Value = 0;

                    Value = Local_SalesInvoice.GrandTotal - OriginalSalesInvoice.payment_schedual.Sum(x => x.debit);

                    payment_schedual payment_schedual = Local_SalesInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                    if (payment_schedual != null)
                    {
                        payment_schedual.debit = payment_schedual.debit + Value;

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

                    decimal Value = 0;

                    Value = Local_PurchaseInvoice.GrandTotal - OriginalPurchaseInvoice.payment_schedual.Sum(x => x.credit);

                    payment_schedual payment_schedual = Local_PurchaseInvoice.payment_schedual.Where(x => x.credit > 0).LastOrDefault();
                    if (payment_schedual != null)
                    {
                        payment_schedual.credit = payment_schedual.credit + Value;

                    }
                }
            }
        }
        public void Update_ValueDown(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();


                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
                    decimal Value = 0;

                    Value = OriginalSalesInvoice.payment_schedual.Sum(x => x.debit) - Local_SalesInvoice.GrandTotal;

                    Decimal Balance = OriginalSalesInvoice.payment_schedual.Sum(x => x.AccountReceivableBalance);
                    if (Balance >= Value)
                    {

                        payment_schedual payment_schedual = Local_SalesInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            payment_schedual.debit = payment_schedual.debit - Value;

                        }

                    }
                    else
                    {
                        payment_schedual payment_schedual = Local_SalesInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            payment_schedual.credit = payment_schedual.credit - Value;

                        }
                        List<payment_schedual> oldSchedual = Local_SalesInvoice.payment_schedual.ToList();
                        List<payment> oldpayment = new List<payment>();
                        foreach (payment_schedual _payment_schedual in oldSchedual)
                        {
                            if (_payment_schedual.payment_detail != null)
                            {
                                oldpayment.Add(_payment_schedual.payment_detail.payment);

                            }

                        }
                        db.payments.RemoveRange(oldpayment);


                    }

                }
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice OriginalPurcahseInvoice;

                using (db temp = new db())
                {
                    OriginalPurcahseInvoice = temp.purchase_invoice.Where(x => x.id_purchase_invoice == ID).FirstOrDefault();


                    purchase_invoice Local_purchase_invoice = db.purchase_invoice.Find(ID);
                    decimal Value = 0;

                    Value = OriginalPurcahseInvoice.payment_schedual.Sum(x => x.credit) - Local_purchase_invoice.GrandTotal;

                    Decimal Balance = OriginalPurcahseInvoice.payment_schedual.Sum(x=>x.AccountPayableBalance);
                    if (Balance >= Value)
                    {

                        payment_schedual payment_schedual = Local_purchase_invoice.payment_schedual.Where(x => x.credit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            payment_schedual.credit = payment_schedual.credit - Value;

                        }

                    }
                    else
                    {
                        payment_schedual payment_schedual = Local_purchase_invoice.payment_schedual.Where(x => x.credit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            payment_schedual.credit = payment_schedual.credit - Value;

                        }
                        List<payment_schedual> oldSchedual = Local_purchase_invoice.payment_schedual.ToList();
                        List<payment> oldpayment = new List<payment>();
                        foreach (payment_schedual _payment_schedual in oldSchedual)
                        {
                            if (_payment_schedual.payment_detail != null)
                            {
                                oldpayment.Add(_payment_schedual.payment_detail.payment);

                            }

                        }
                        db.payments.RemoveRange(oldpayment);


                    }

                }
            }
        }
        public void Update_CurrencyChange(db db, int ID, entity.App.Names Application)
        {

            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);


                List<payment_schedual> oldSchedual = Local_SalesInvoice.payment_schedual.ToList();
                List<payment> oldpayment = new List<payment>();
                foreach (payment_schedual payment_schedual in oldSchedual)
                {
                    if (payment_schedual.payment_detail != null)
                    {
                        oldpayment.Add(payment_schedual.payment_detail.payment);
                    }

                }

                //add new schedual
                List<payment_schedual> payment_schedualList;
                Payment _Payment = new Payment();
                payment_schedualList = _Payment.insert_Schedual(Local_SalesInvoice);




                foreach (payment payment in oldpayment)
                {
                    foreach (payment_detail payment_detail in payment.payment_detail)
                    {
                        if (payment_detail.app_account!=null)
                        {
                            db.app_account_detail.RemoveRange(payment_detail.app_account.app_account_detail);

                        }

                    }
                    db.payment_detail.RemoveRange(payment.payment_detail);
                }
                db.payments.RemoveRange(oldpayment);
                db.payment_schedual.RemoveRange(oldSchedual);
                db.payment_schedual.AddRange(payment_schedualList);
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice Local_PurchaseInvoice = db.purchase_invoice.Find(ID);


                List<payment_schedual> oldSchedual = Local_PurchaseInvoice.payment_schedual.ToList();
                List<payment> oldpayment = new List<payment>();
                foreach (payment_schedual payment_schedual in oldSchedual)
                {
                    if (payment_schedual.payment_detail != null)
                    {
                        oldpayment.Add(payment_schedual.payment_detail.payment);
                    }

                }

                //add new schedual
                List<payment_schedual> payment_schedualList;
                Payment _Payment = new Payment();
                payment_schedualList = _Payment.insert_Schedual(Local_PurchaseInvoice);




                foreach (payment payment in oldpayment)
                {
                    foreach (payment_detail payment_detail in payment.payment_detail)
                    {
                        if (payment_detail.app_account != null)
                        {
                            db.app_account_detail.RemoveRange(payment_detail.app_account.app_account_detail);
                        }

                    }
                    db.payment_detail.RemoveRange(payment.payment_detail);
                }
                db.payments.RemoveRange(oldpayment);
                db.payment_schedual.RemoveRange(oldSchedual);
                db.payment_schedual.AddRange(payment_schedualList);
            }
        }

        public void Update_DateChange(db db, int ID, entity.App.Names Application)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice OriginalSalesInvoice;

                using (db temp = new db())
                {
                    OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();


                    sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);


                    List<payment_schedual> oldSchedual = Local_SalesInvoice.payment_schedual.ToList();
                    List<payment> oldpayment = new List<payment>();
                    List<app_contract_detail> app_contract_details = Local_SalesInvoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                    int i = 0;
                    foreach (app_contract_detail app_contract_detail in app_contract_details)
                    {
                        i = i + 1;
                        payment_schedual payment_schedual = oldSchedual.Skip(i - 1).Take(1).FirstOrDefault();
                        payment_schedual.expire_date = Local_SalesInvoice.trans_date.AddDays(app_contract_detail.interval);


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


                    List<payment_schedual> oldSchedual = Local_PurchaseInvoice.payment_schedual.ToList();
                    List<payment> oldpayment = new List<payment>();
                    List<app_contract_detail> app_contract_details = Local_PurchaseInvoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                    int i = 0;
                    foreach (app_contract_detail app_contract_detail in app_contract_details)
                    {
                        i = i + 1;
                        payment_schedual payment_schedual = oldSchedual.Skip(i - 1).Take(1).FirstOrDefault();
                        payment_schedual.expire_date = Local_PurchaseInvoice.trans_date.AddDays(app_contract_detail.interval);


                    }
                }
            }
        }


    }
}
