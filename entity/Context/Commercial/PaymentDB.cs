using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace entity
{
    public partial class PaymentDB : BaseDB
    {
        /// <summary>
        /// Creates new Payment (Header)
        /// </summary>
        /// <returns>Payment Entity</returns>
        public payment New()
        {
            payment payment = new entity.payment();
            payment.status = Status.Documents_General.Pending;
            payment.State = EntityState.Added;

            if (app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PaymentUtility).FirstOrDefault() != null)
            {
                payment.app_document_range = app_document_range.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.app_document.id_application == entity.App.Names.PaymentUtility).FirstOrDefault();
            }
            
            return payment;
        }

        /// <summary>
        /// Creates a new Payment Detail
        /// </summary>
        /// <param name="payment">Payment (Header) to automatically relate</param>
        /// <returns>Payment Detail Entity</returns>
        public payment_detail NewPaymentDetail(ref payment payment)
        {
            payment_detail payment_detail = new entity.payment_detail();
            payment_detail.State = EntityState.Added;
            payment_detail.payment = payment;

            return payment_detail;
        }

        #region Save
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
        #endregion

        public void Approve(int id_payment_schedual, bool PrintRequire)
        {
            foreach (payment payment in payments.Local.Where(x => x.status != Status.Documents_General.Approved && x.IsSelected))
            {
                if (payment.id_payment == 0)
                {
                    SaveChanges();
                }

                //entity.Brillo.Logic.AccountReceivable AccountReceivable = new entity.Brillo.Logic.AccountReceivable();
                payment_schedual _payment_schedual = payment_schedual.Where(x => x.id_payment_schedual == id_payment_schedual).FirstOrDefault();

                MakePayment(_payment_schedual, payment, PrintRequire);
            }
        }




        public void MakePayment(payment_schedual payment_schedual, payment payment, bool PrintRequire)
        {
            foreach (payment_detail payment_detail in payment.payment_detail)
            {
                ///Creates counter balanced in payment schedual.
                ///Use this to Balance pending payments.
                payment_schedual balance_payment_schedual = new payment_schedual();

                if (payment_detail.id_currencyfx == 0)
                {
                    payment_detail.id_currencyfx = app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
                    payment_detail.app_currencyfx = app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault();
                }
                
                if (payment_detail.id_currencyfx > 0 && payment_detail.app_currencyfx == null)
                {
                    payment_detail.app_currencyfx = app_currencyfx.Where(x => x.id_currencyfx == payment_detail.id_currencyfx && x.is_active).FirstOrDefault();
                }

                if (payment_detail.id_payment_type == 0)
                {
                    payment_detail.id_payment_type = payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
                }

                if (payment_detail.id_account == 0 || payment_detail.id_account == null)
                {
                    payment_detail.id_account = CurrentSession.Id_Account;
                }

                if (payment_detail.value < 0)
                {
                    ///If PaymentDetail Value is Negative.
                    balance_payment_schedual.debit = Math.Abs(Convert.ToDecimal(payment_detail.value));
                }
                else
                {
                    ///If PaymentDetail Value is Positive.
                    balance_payment_schedual.credit = Convert.ToDecimal(payment_detail.value);
                }

                balance_payment_schedual.parent = payment_schedual;
                balance_payment_schedual.status = Status.Documents_General.Approved;
                balance_payment_schedual.id_contact = payment_schedual.id_contact;
                balance_payment_schedual.id_currencyfx = payment_schedual.id_currencyfx;
                balance_payment_schedual.trans_date = payment_detail.trans_date;
                balance_payment_schedual.expire_date = payment_schedual.expire_date;

                string ModuleName = string.Empty;

                ///
                if (payment_schedual.id_purchase_invoice != 0)
                {
                    balance_payment_schedual.id_purchase_invoice = payment_schedual.id_purchase_invoice;
                    ModuleName = "PurchaseInvoice";
                }

                ///
                if (payment_schedual.id_purchase_order != 0)
                {
                    balance_payment_schedual.id_purchase_order = payment_schedual.id_purchase_order;
                    ModuleName = "PurchaseOrder";
                }

                ///
                if (payment_schedual.id_purchase_return != 0)
                {
                    balance_payment_schedual.id_purchase_return = payment_schedual.id_purchase_return;
                    ModuleName = "PurchaseReturn";
                }

                ///
                if (payment_schedual.id_sales_invoice != 0)
                {
                    balance_payment_schedual.id_sales_invoice = payment_schedual.id_sales_invoice;
                    ModuleName = "SalesInvoice";
                }

                ///
                if (payment_schedual.id_sales_order != 0)
                {
                    balance_payment_schedual.id_sales_order = payment_schedual.id_sales_order;
                    ModuleName = "SalesOrder";
                }
                
                ///
                if (payment_detail.id_sales_return != 0)
                {
                    balance_payment_schedual.id_sales_return = payment_schedual.id_sales_return;
                    ModuleName = "SalesReturn";
                }

                //Add Balance Payment Schedual into Context. 
                payment_detail.payment_schedual.Add(balance_payment_schedual);


                ///Code to specify Accounts.
                ///
                if (payment_type.Where(x => x.id_payment_type == payment_detail.id_payment_type).FirstOrDefault().payment_behavior == entity.payment_type.payment_behaviours.Normal)
                {
                    app_account_detail app_account_detail = new app_account_detail();
                    if (base.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault() != null)
                    {
                        app_account_detail.id_session = base.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault().id_session;
                    }

                    app_account_detail.id_account = (int)payment_detail.id_account;
                    app_account_detail.id_currencyfx = payment_detail.id_currencyfx;
                    app_account_detail.id_payment_type = payment_detail.id_payment_type;
                    app_account_detail.id_payment_detail = payment_detail.id_payment_detail;
                    app_account_detail.trans_date = payment_detail.trans_date;
                    app_account_detail.debit = 0;
                    app_account_detail.credit = Convert.ToDecimal(payment_detail.value);

                    app_account_detail.comment = Brillo.Localize.StringText(ModuleName) + " " + payment_schedual.sales_invoice.number + " | " + payment_schedual.contact.name;
                    base.app_account_detail.Add(app_account_detail);
                }

               
            }

            payment.status = Status.Documents_General.Approved;
            base.SaveChanges();



            if (PrintRequire)
            {
                
          
            entity.Brillo.Document.Start.Automatic(payment, payment.app_document_range);

            }

        }
    }
}
