using System;
using System.Windows;
using entity;
using System.Linq;
using System.Collections.Generic;
using entity.Brillo.Logic;
namespace cntrl.Class
{
    public class PaymentReApprove
    {

        public bool Start(db db, int ID, entity.App.Names Application)
        {
            return true;



        }
        public bool ContractChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            if (Oldsales_invoice.id_contract != sales_invoice.id_contract)
            {
                List<payment_schedual> oldSchedual = Oldsales_invoice.payment_schedual.ToList();
                List<payment> oldpayment = new List<payment>();
                foreach (payment_schedual payment_schedual in oldSchedual)
                {
                    oldpayment.Add(payment_schedual.payment_detail.payment);
                }
                //

                //add new schedual
                List<payment_schedual> payment_schedualList;
                Payment _Payment = new Payment();
                payment_schedualList = _Payment.insert_Schedual(sales_invoice);

                if (MessageBox.Show("abc") == MessageBoxResult.Yes)
                {
                    db.payment_schedual.RemoveRange(oldSchedual);
                    db.payments.RemoveRange(oldpayment);
                    db.payment_schedual.AddRange(payment_schedualList);
                    return true;
                }


            }
            return false;
        }
        public bool ValueUP(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            decimal Value = 0;
            if (Oldsales_invoice.GrandTotal < sales_invoice.GrandTotal)
            {
                Value = sales_invoice.GrandTotal - Oldsales_invoice.GrandTotal;
                if (MessageBox.Show("abc") == MessageBoxResult.Yes)
                {
                    payment_schedual payment_schedual = Oldsales_invoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                    if (payment_schedual != null)
                    {
                        payment_schedual.debit = payment_schedual.debit + Value;
                        return true;
                    }

                }

            }
            return false;
        }
        public bool ValueDown(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            decimal Value = 0;
            if (Oldsales_invoice.GrandTotal > sales_invoice.GrandTotal)
            {
                Value = Oldsales_invoice.GrandTotal - sales_invoice.GrandTotal;

                Decimal Balance = Oldsales_invoice.payment_schedual.Where(x => x.credit > 0).Sum(x => x.credit);
                if (Balance >= Value)
                {
                    if (MessageBox.Show("abc") == MessageBoxResult.Yes)
                    {
                        payment_schedual payment_schedual = Oldsales_invoice.payment_schedual.Where(x => x.debit > 0).LastOrDefault();
                        if (payment_schedual != null)
                        {
                            payment_schedual.debit = payment_schedual.debit - Value;
                            return true;
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("abc") == MessageBoxResult.Yes)
                    {
                        List<payment_schedual> oldSchedual = Oldsales_invoice.payment_schedual.ToList();
                        List<payment> oldpayment = new List<payment>();
                        foreach (payment_schedual payment_schedual in oldSchedual)
                        {
                            oldpayment.Add(payment_schedual.payment_detail.payment);
                        }
                        db.payments.RemoveRange(oldpayment);
                        return true;
                    }
                }


            }
            return false;
        }
        public bool CurrencyChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            if (Oldsales_invoice.id_currencyfx != sales_invoice.id_currencyfx)
            {
                List<payment_schedual> oldSchedual = Oldsales_invoice.payment_schedual.ToList();
                List<payment> oldpayment = new List<payment>();
                foreach (payment_schedual payment_schedual in oldSchedual)
                {
                    oldpayment.Add(payment_schedual.payment_detail.payment);
                }
                //

                //add new schedual
                List<payment_schedual> payment_schedualList;
                Payment _Payment = new Payment();
                payment_schedualList = _Payment.insert_Schedual(sales_invoice);

                if (MessageBox.Show("abc") == MessageBoxResult.Yes)
                {
                    db.payment_schedual.RemoveRange(oldSchedual);
                    db.payments.RemoveRange(oldpayment);
                    db.payment_schedual.AddRange(payment_schedualList);
                    return true;
                }


            }
            return false;
        }

        public bool DateChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            if (Oldsales_invoice.trans_date != sales_invoice.trans_date)
            {
                if (MessageBox.Show("abc") == MessageBoxResult.Yes)
                {
                    List<payment_schedual> oldSchedual = Oldsales_invoice.payment_schedual.ToList();
                    List<payment> oldpayment = new List<payment>();
                    List<app_contract_detail> app_contract_details = sales_invoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                    int i = 0;
                    foreach (app_contract_detail app_contract_detail in app_contract_details)
                    {
                        i = i + 1;
                        payment_schedual payment_schedual = oldSchedual.Skip(i - 1).Take(1).FirstOrDefault();
                        payment_schedual.expire_date = sales_invoice.trans_date.AddDays(app_contract_detail.interval);

                        return true;
                    }
                }

            }
            return false;
        }
    }
}
