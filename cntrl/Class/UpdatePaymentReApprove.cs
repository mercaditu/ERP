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

        public void Check_ContractChanges(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);


            List<payment_schedual> oldSchedual = OriginalSalesInvoice.payment_schedual.ToList();
            List<payment> oldpayment = new List<payment>();
            foreach (payment_schedual payment_schedual in oldSchedual)
            {
                oldpayment.Add(payment_schedual.payment_detail.payment);
            }

            //add new schedual
            List<payment_schedual> payment_schedualList;
            Payment _Payment = new Payment();
            payment_schedualList = _Payment.insert_Schedual(Local_SalesInvoice);


            db.payment_schedual.RemoveRange(oldSchedual);
            db.payments.RemoveRange(oldpayment);
            foreach (payment payment in oldpayment)
            {
                foreach (payment_detail payment_detail in payment.payment_detail)
                {
                    db.app_account.Remove(payment_detail.app_account);
                    db.payment_detail.Remove(payment_detail);
                }
            }
            db.payment_schedual.AddRange(payment_schedualList);

        }

        public void ValueUP(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

            decimal Value = 0;

            Value = Local_SalesInvoice.GrandTotal - OriginalSalesInvoice.GrandTotal;

            payment_schedual payment_schedual = OriginalSalesInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
            if (payment_schedual != null)
            {
                payment_schedual.debit = payment_schedual.debit + Value;

            }

        }
        public void ValueDown(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            decimal Value = 0;

            Value = OriginalSalesInvoice.GrandTotal - Local_SalesInvoice.GrandTotal;

            Decimal Balance = OriginalSalesInvoice.payment_schedual.Where(x => x.credit > 0).Sum(x => x.credit);
            if (Balance >= Value)
            {

                payment_schedual payment_schedual = OriginalSalesInvoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                if (payment_schedual != null)
                {
                    payment_schedual.debit = payment_schedual.debit - Value;

                }

            }
            else
            {

                List<payment_schedual> oldSchedual = OriginalSalesInvoice.payment_schedual.ToList();
                List<payment> oldpayment = new List<payment>();
                foreach (payment_schedual payment_schedual in oldSchedual)
                {
                    oldpayment.Add(payment_schedual.payment_detail.payment);
                }
                db.payments.RemoveRange(oldpayment);


            }




        }
        public void CurrencyChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

            List<payment_schedual> oldSchedual = OriginalSalesInvoice.payment_schedual.ToList();
            List<payment> oldpayment = new List<payment>();
            foreach (payment_schedual payment_schedual in oldSchedual)
            {
                oldpayment.Add(payment_schedual.payment_detail.payment);
            }
            //

            //add new schedual
            List<payment_schedual> payment_schedualList;
            Payment _Payment = new Payment();
            payment_schedualList = _Payment.insert_Schedual(Local_SalesInvoice);


            db.payment_schedual.RemoveRange(oldSchedual);
            db.payments.RemoveRange(oldpayment);
            db.payment_schedual.AddRange(payment_schedualList);

        }

        public void DateChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);


            List<payment_schedual> oldSchedual = OriginalSalesInvoice.payment_schedual.ToList();
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
}
