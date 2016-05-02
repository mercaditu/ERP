using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace entity
{
    public partial class PaymentDB : BaseDB
    {
        public payment New()
        {
            payment payment = new entity.payment();
            payment.id_company = CurrentSession.Id_Company;
            payment.id_user = CurrentSession.Id_User;
            payment.is_head = true;
            if (app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PaymentUtility).FirstOrDefault() != null)
            {
                payment.app_document_range = app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PaymentUtility).FirstOrDefault();
            }
            payment.status = Status.Documents_General.Pending;
            return payment;
        }

        public override int SaveChanges()
        {
            validate_Contact();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Contact();
            return base.SaveChangesAsync();
        }

        private void validate_Contact()
        {
            foreach (payment payment in base.payments.Local)
            {
                if (payment.IsSelected)
                {
                    if (payment.State == EntityState.Added)
                    {
                        payment.timestamp = DateTime.Now;
                        payment.State = EntityState.Unchanged;
                        Entry(payment).State = EntityState.Added;
                    }
                    else if (payment.State == EntityState.Modified)
                    {
                        payment.timestamp = DateTime.Now;
                        payment.State = EntityState.Unchanged;
                        Entry(payment).State = EntityState.Modified;
                    }
                    else if (payment.State == EntityState.Deleted)
                    {
                        payment.timestamp = DateTime.Now;
                        payment.State = EntityState.Unchanged;
                        payments.Remove(payment);
                    }
                }
                else if (payment.State > 0)
                {
                    if (payment.State != EntityState.Unchanged)

                        Entry(payment).State = EntityState.Unchanged;
                }
            }

        }

        public void Approve(int id_payment_schedual)
        {
            foreach (payment payment in payments.Local.Where(x =>
                                               x.status != Status.Documents_General.Approved
                                                       && x.IsSelected))
            {

                if (payment.id_payment == 0)
                {
                    SaveChanges();
                }

                entity.Brillo.Logic.AccountReceivable AccountReceivable = new entity.Brillo.Logic.AccountReceivable();
                payment_schedual _payment_schedual = payment_schedual.Where(x => x.id_payment_schedual == id_payment_schedual).FirstOrDefault();
              
                 ReceivePayment(_payment_schedual, payment);
            }
        }




        public void ReceivePayment(payment_schedual payment_schedual, payment payment)
        {

            foreach (payment_detail payment_detail in payment.payment_detail)
            {
                if ( payment_detail.id_currencyfx==0 ||payment_detail.id_currencyfx==null )
                {
                    payment_detail.id_currencyfx = app_currencyfx.Where(x=>x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
                }

                if (payment_detail.id_payment_type == 0 || payment_detail.id_payment_type == null)
                {
                    payment_detail.id_payment_type = payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
                }

                if (payment_detail.id_account == 0 || payment_detail.id_account == null)
                {
                    payment_detail.id_account =CurrentSession.Id_Account;
                }
                payment_schedual _payment_schedual = new payment_schedual();

                if (payment_detail.value<0)
                {
                    _payment_schedual.debit = Math.Abs(Convert.ToDecimal(payment_detail.value));
                }
                else
                {
                    _payment_schedual.credit = Convert.ToDecimal(payment_detail.value);
                }
                _payment_schedual.credit = Convert.ToDecimal(payment_detail.value);
                _payment_schedual.parent = payment_schedual;
                _payment_schedual.expire_date = payment_schedual.expire_date;
                _payment_schedual.status = payment_schedual.status;
                _payment_schedual.id_contact = payment_schedual.id_contact;
                _payment_schedual.id_currencyfx = payment_schedual.id_currencyfx;
                if (payment_schedual.id_purchase_invoice == 0)
                {
                    _payment_schedual.id_purchase_invoice = null;
                }
                else
                {
                    payment_schedual.id_purchase_invoice = payment_schedual.id_purchase_invoice;
                }
                if (payment_schedual.id_purchase_order == 0)
                {
                    _payment_schedual.id_purchase_order = null;
                }
                else
                {
                    payment_schedual.id_purchase_order = payment_schedual.id_purchase_order;
                }
                if (payment_schedual.id_purchase_return == 0)
                {
                    _payment_schedual.id_purchase_return = null;
                }
                else
                {
                    payment_schedual.id_purchase_return = payment_schedual.id_purchase_return;
                }
                if (payment_schedual.id_sales_invoice == 0)
                {
                    _payment_schedual.id_sales_invoice = null;
                }
                else
                {
                    payment_schedual.id_sales_invoice = payment_schedual.id_sales_invoice;
                }
                if (payment_schedual.id_sales_order == 0)
                {
                    _payment_schedual.id_sales_order = null;
                }
                else
                {
                    payment_schedual.id_sales_order = payment_schedual.id_sales_order;
                }
                
                
                if (payment_detail.id_sales_return==0)
                {
                    _payment_schedual.id_sales_return = null;
                }
                else
                {
                    payment_schedual.id_sales_return = payment_schedual.id_sales_return;
                }
                _payment_schedual.trans_date = payment_detail.trans_date;
                payment_detail.payment_schedual.Add(_payment_schedual);

                if (payment_type.Where(x => x.id_payment_type == payment_detail.id_payment_type).FirstOrDefault().payment_behavior == entity.payment_type.payment_behaviours.Normal)
                {
                    app_account_detail app_account_detail = new app_account_detail();
                    app_account_detail.id_account =(int)payment_detail.id_account;
                    app_account_detail.id_currencyfx = payment_schedual.id_currencyfx;
                    app_account_detail.id_payment_type = payment_detail.id_payment_type;
                    app_account_detail.trans_date = payment_detail.trans_date;
                    app_account_detail.debit = 0;
                    app_account_detail.credit = Convert.ToDecimal(payment_detail.value);
                    base.app_account_detail.Add(app_account_detail);
                }

               
            }

            payment.status = Status.Documents_General.Approved;
            base.SaveChanges();



            entity.Brillo.Document.Start.Automatic(payment, payment.app_document_range);



        }
    }
}
