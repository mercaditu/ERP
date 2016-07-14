using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using entity.Brillo;

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
            payment.id_range = GetDefault.Return_RangeID(entity.App.Names.PaymentUtility);
            if (app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault() != null)
            {
                payment.app_document_range = app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault();
            }

            payment.IsSelected = true;

            return payment;
        }

        /// <summary>
        /// Creates a new Payment Detail, and Adds it into Payment
        /// </summary>
        /// <param name="payment">Payment (Header) to automatically relate</param>
        /// <returns>Payment Detail Entity</returns>

        public payment_detail NewPaymentDetail(ref payment payment)
        {
            payment_detail payment_detail = new entity.payment_detail();
            payment_detail.State = EntityState.Added;
            payment_detail.id_payment_type = payment_type.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_payment_type;
            payment_detail.app_currencyfx = Brillo.Currency.get_DefaultFX(this);
            payment.payment_detail.Add(payment_detail);

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

                //Creates Balanced Payment Schedual and Account Detail (if necesary).
                MakePayment(_payment_schedual, payment, PrintRequire);
            }
        }

        public void MakePayment(payment_schedual Parent_Schedual, payment payment, bool RequirePrint)
        {
            foreach (payment_detail payment_detail in payment.payment_detail.Where(x=>x.IsSelected))
            {
                Parent_Schedual = base.payment_schedual.Where(x => x.id_payment_schedual == payment_detail.id_payment_schedual).FirstOrDefault();
                ///Creates counter balanced in payment schedual.
                ///Use this to Balance pending payments.
                payment_schedual balance_payment_schedual = new payment_schedual();

                //Assigns appCurrencyFX ID & Entity
                if (payment_detail.id_currencyfx == 0)
                {
                    payment_detail.id_currencyfx = app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
                    payment_detail.app_currencyfx = app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault();
                }

                ///Assigns appCurrencyFX Entity which is needed for Printing.
                if (payment_detail.id_currencyfx > 0 && payment_detail.app_currencyfx == null)
                {
                    payment_detail.app_currencyfx = app_currencyfx.Where(x => x.id_currencyfx == payment_detail.id_currencyfx && x.is_active).FirstOrDefault();
                }

                ///If by chance Payment Type is Blank, will get Default Payment Type.
                if (payment_detail.id_payment_type == 0)
                {
                    payment_detail.id_payment_type = payment_type.Where(x => x.is_default).FirstOrDefault().id_payment_type;
                }

                ///Checks if Account ID is set.
                if (payment_detail.id_account == 0 || payment_detail.id_account == null)
                {
                    payment_detail.id_account = CurrentSession.Id_Account;
                }

                ///Logic for Value in Balance Payment Schedual.
                if (Parent_Schedual.id_purchase_invoice > 0 || Parent_Schedual.id_purchase_order > 0 || Parent_Schedual.id_sales_return > 0)
                {
                    ///If PaymentDetail Value is Negative.
                    ///
                    if (payment_detail.app_currencyfx.id_currency != Parent_Schedual.app_currencyfx.id_currency)
                    {
                        balance_payment_schedual.debit = Math.Abs(Currency.convert_Values(payment_detail.value, payment_detail.id_currencyfx, Parent_Schedual.id_currencyfx, App.Modules.Purchase));
                    }
                    else
                    {
                        balance_payment_schedual.debit = Math.Abs(payment_detail.value);
                    }
                }
                else
                {
                    ///If PaymentDetail Value is Positive.
                    if (payment_detail.app_currencyfx.id_currency!=Parent_Schedual.app_currencyfx.id_currency)
                    {

                        balance_payment_schedual.credit = Currency.convert_Values(payment_detail.value, payment_detail.id_currencyfx, Parent_Schedual.id_currencyfx, App.Modules.Sales);
                    }
                    else
                    {
                        balance_payment_schedual.credit = payment_detail.value;
                    }
                 
                   
                }

                balance_payment_schedual.parent = Parent_Schedual;
                balance_payment_schedual.status = Status.Documents_General.Approved;
                balance_payment_schedual.id_contact = Parent_Schedual.id_contact;
                balance_payment_schedual.id_currencyfx = Parent_Schedual.id_currencyfx;
                balance_payment_schedual.trans_date = payment_detail.trans_date;
                balance_payment_schedual.expire_date = Parent_Schedual.expire_date;

                string ModuleName = string.Empty;

                ///
                if (Parent_Schedual.id_purchase_invoice != null)
                {
                    balance_payment_schedual.id_purchase_invoice = Parent_Schedual.id_purchase_invoice;
                    ModuleName = "PurchaseInvoice";
                }

                ///

                if (payment_detail.payment_schedual.FirstOrDefault() != null)
                {
                    balance_payment_schedual.id_purchase_order = payment_detail.payment_schedual.FirstOrDefault().id_purchase_order;
                    ModuleName = "PurchaseOrder";
                }

                ///
                if (Parent_Schedual.id_purchase_return != null)
                {
                    balance_payment_schedual.id_purchase_return = Parent_Schedual.id_purchase_return;
                    ModuleName = "PurchaseReturn";
                }

                ///
                if (Parent_Schedual.id_sales_invoice != null)
                {
                    balance_payment_schedual.id_sales_invoice = Parent_Schedual.id_sales_invoice;
                    ModuleName = "SalesInvoice";
                }

                ///
                if (Parent_Schedual.id_sales_order != null)
                {
                    balance_payment_schedual.id_sales_order = Parent_Schedual.id_sales_order;
                    ModuleName = "SalesOrder";
                }

                ///
                if (payment_detail.id_sales_return != null)
                {
                    balance_payment_schedual.id_sales_return = Parent_Schedual.id_sales_return;
                    ModuleName = "SalesReturn";
                }

                //Add Balance Payment Schedual into Context. 
                payment_detail.payment_schedual.Add(balance_payment_schedual);


                ///Code to specify Accounts.
                ///
                if (payment_type.Where(x => x.id_payment_type == payment_detail.id_payment_type).FirstOrDefault().payment_behavior == entity.payment_type.payment_behaviours.Normal)
                {
                    ///Creates new Account Detail for each Payment Detail.
                    app_account_detail app_account_detail = new app_account_detail();

                    app_account_detail.id_account = (int)payment_detail.id_account;
                    app_account_detail.id_currencyfx = payment_detail.id_currencyfx;
                    app_account_detail.id_payment_type = payment_detail.id_payment_type;
                    app_account_detail.id_payment_detail = payment_detail.id_payment_detail;
                    app_account_detail.trans_date = payment_detail.trans_date;

                    ///Gets the Session ID necesary for cashier movement.
                    if (base.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault() != null)
                    {
                        app_account_detail.id_session = base.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).FirstOrDefault().id_session;
                    }

                    //Logic for Account Detail based on Payment Detail Logic.
                    if (Parent_Schedual.id_purchase_invoice > 0 || Parent_Schedual.id_purchase_order > 0 || Parent_Schedual.id_sales_return > 0)
                    {
                        ///If PaymentDetail Value is Negative.
                        app_account_detail.debit = Math.Abs(Convert.ToDecimal(payment_detail.value));
                    }
                    else
                    {
                        ///If PaymentDetail Value is Positive.
                        app_account_detail.credit = Convert.ToDecimal(payment_detail.value);
                    }

                    ///Comment with Module Name and Contact.
                    ///Insert AccountDetail into Context.
                    ///
                    string number = "";
                    if (Parent_Schedual.id_purchase_invoice > 0 || Parent_Schedual.id_purchase_order > 0 || Parent_Schedual.id_sales_return > 0)
                    {

                        if (Parent_Schedual.purchase_invoice != null)
                        {
                            number = Parent_Schedual.purchase_invoice.number;
                        }

                    }
                    else
                    {
                        if (Parent_Schedual.sales_invoice != null)
                        {
                            number = Parent_Schedual.sales_invoice.number;
                        }
                    }
                    app_account_detail.comment = Brillo.Localize.StringText(ModuleName) + " " + number + " | " + Parent_Schedual.contact.name;
                    app_account_detail.tran_type = app_account_detail.tran_types.Transaction;
                    base.app_account_detail.Add(app_account_detail);
                }
                //pankeel
            }

            payment.status = Status.Documents_General.Approved;
            base.SaveChanges();

            if (RequirePrint)
            {
                entity.Brillo.Document.Start.Automatic(payment, payment.app_document_range);
            }

        }

        public void Rearrange_Payment()
        {
            List<payment_schedual> payment_schedualList = base.payment_schedual.ToList();

            foreach (payment_schedual parent in payment_schedualList)
            {
                foreach (payment_schedual child in parent.child)
                {
                    if (child.payment_detail != null)
                    {
                        int id_currency = parent.app_currencyfx.id_currency;
                        DateTime timestamp = child.payment_detail.trans_date;

                        if (base.app_currencyfx.Where(x => x.type == entity.app_currencyfx.CurrencyFXTypes.Transaction &&
                                                             x.id_currency == id_currency && x.timestamp <= timestamp)
                                                            .OrderByDescending(x => x.timestamp).FirstOrDefault() != null)
                        {
                            app_currencyfx app_currencyfx = base.app_currencyfx.Where(x => x.type == entity.app_currencyfx.CurrencyFXTypes.Transaction &&
                                                             x.id_currency == id_currency && x.timestamp <= timestamp)
                                                            .OrderByDescending(x => x.timestamp).FirstOrDefault();

                            if (child.debit > 0)
                            {
                                child.debit = Currency.convert_Values(child.payment_detail.value, child.payment_detail.id_currencyfx, app_currencyfx.id_currencyfx, App.Modules.Purchase);
                            }

                            if (child.credit > 0)
                            {
                                child.credit = Currency.convert_Values(child.payment_detail.value, child.payment_detail.id_currencyfx, app_currencyfx.id_currencyfx, App.Modules.Sales);
                            }
                        }
                    }

                }
            }
            base.SaveChanges();
        }
    }
}
