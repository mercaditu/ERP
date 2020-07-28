using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace entity
{
    public partial class PaymentDB : BaseDB
    {
        /// <summary>
        /// Creates an instance of Payment entity with corresponding data needed.
        /// </summary>
        /// <param name="Is_PaymentRecievable">Specify if Payment being created is meant to Recieve (Sales) then True., Payable (Purchase) then False.</param>
        /// <returns>Payment Instance ready for use.</returns>
        public payment New(bool Is_PaymentRecievable)
        {
            payment payment = new payment();
            payment.status = Status.Documents_General.Pending;
            payment.State = EntityState.Added;

            if (Is_PaymentRecievable)
            {
                payment.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.PointOfSale, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
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
            payment_detail payment_detail = new payment_detail();
            payment_detail.State = EntityState.Added;
            payment_detail.id_payment_type = payment_type.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).Select(y => y.id_payment_type).FirstOrDefault();
            payment_detail.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
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

        #endregion Save

        public void Approve(List<payment_schedual> payment_schedualList, bool IsRecievable, bool is_print, bool clean_balance)
        {
            foreach (payment payment in payments.Local.Where(x => x.status != Status.Documents_General.Approved && x.IsSelected))
            {
                if (payment.id_payment == 0)
                {
                    SaveChanges();
                }

                //entity.Brillo.Logic.AccountReceivable AccountReceivable = new entity.Brillo.Logic.AccountReceivable();

                //Creates Balanced Payment Schedual and Account Detail (if necesary).
                MakePayment(payment_schedualList, payment, IsRecievable, is_print, clean_balance);
            }
        }

        public async void MakePayment(List<payment_schedual> payment_schedualList, payment payment, bool IsRecievable, bool is_print, bool clean_balance)
        {
            string ModuleName = string.Empty;
            string number = string.Empty;
            foreach (payment_detail payment_detail in payment.payment_detail.ToList())
            {
                ///Creates counter balanced in payment schedual.
                ///Use this to Balance pending payments.
                List<payment_schedual> schedualList = new List<payment_schedual>();
                payment_schedual Parent_Schedual = new payment_schedual();

                //Assigns appCurrencyFX ID & Entity

                if (payment_detail.id_currencyfx == 0)
                {
                    payment_detail.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                }

                ///Assigns appCurrencyFX Entity which is needed for Printing.
                if (payment_detail.id_currencyfx > 0 && payment_detail.app_currencyfx == null)
                {
                    payment_detail.app_currencyfx = await app_currencyfx.FindAsync(payment_detail.id_currencyfx);
                }

                ///If by chance Payment Type is Blank, will get Default Payment Type.
                if (payment_detail.id_payment_type == 0)
                {
                    payment_detail.id_payment_type = await payment_type.Where(x => x.is_default).Select(x => x.id_payment_type).FirstOrDefaultAsync();
                }

                if (payment_detail.payment.id_contact > 0 || payment_detail.payment.contact == null)
                {
                    payment_detail.payment.contact = await contacts.FindAsync(payment_detail.payment.id_contact);
                }

                ///Checks if Account ID is set.
                if (payment_detail.id_account == 0 || payment_detail.id_account == null)
                {
                    payment_detail.id_account = CurrentSession.Id_Account;
                    payment_detail.app_account = await app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefaultAsync();
                }
                decimal ChildBalance = 0;
                ///If PaymentDetail Value is Negative.
                if (IsRecievable == false)
                {
                    ChildBalance = Currency.convert_Values(payment_detail.value, payment_detail.id_currencyfx, payment_detail.Default_id_currencyfx, App.Modules.Sales);
                    foreach (payment_schedual parent in payment_schedualList.Where(x => x.AccountPayableBalance > 0))
                    {

                        if (ChildBalance > 0)
                        {
                            payment_schedual child_schedual = new payment_schedual();
                            if (Math.Round(ChildBalance, 2) >= Math.Round(parent.AccountPayableBalance))
                            {
                                child_schedual.debit = parent.AccountPayableBalance;
                                if (base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault() != null)
                                {
                                    child_schedual.parent = base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault();
                                    Parent_Schedual = child_schedual.parent;
                                }

                                ChildBalance -= parent.AccountPayableBalance;
                            }
                            else
                            {
                                if (clean_balance)
                                {
                                    child_schedual.debit = parent.AccountPayableBalance;
                                    if (base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault() != null)
                                    {
                                        child_schedual.parent = base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault();
                                        Parent_Schedual = child_schedual.parent;
                                    }
                                    ChildBalance = 0;
                                }
                                else
                                {
                                    child_schedual.debit = ChildBalance;
                                    if (base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault() != null)
                                    {
                                        child_schedual.parent = base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault();
                                        Parent_Schedual = child_schedual.parent;
                                    }
                                    ChildBalance -= ChildBalance;
                                }


                            }


                            if (Parent_Schedual.purchase_invoice != null)
                            {
                                number = Parent_Schedual.purchase_invoice.number;
                            }
                            if (Parent_Schedual.id_purchase_invoice != null)
                            {
                                child_schedual.id_purchase_invoice = Parent_Schedual.id_purchase_invoice;
                                ModuleName = "PurchaseInvoice";
                            }
                            if (Parent_Schedual.id_purchase_order != null)
                            {
                                child_schedual.id_purchase_order = Parent_Schedual.id_purchase_order;
                                ModuleName = "PurchaseOrder";
                            }

                            /////
                            //if (payment_detail.payment_schedual.FirstOrDefault() != null)
                            //{
                            //    child_schedual.id_purchase_order = payment_detail.payment_schedual.Select(x => x.id_purchase_order).FirstOrDefault();
                            //    ModuleName = "PurchaseOrder";
                            //}

                            ///
                            if (payment_detail.id_purchase_return != null)
                            {
                                child_schedual.id_purchase_return = payment_detail.id_purchase_return;
                                ModuleName += " & Return";
                            }
                            schedualList.Add(child_schedual);
                        }

                    }
                }
                else
                {
                    ///If PaymentDetail Value is Positive.
                    ChildBalance = Currency.convert_Values(payment_detail.value, payment_detail.id_currencyfx, payment_detail.Default_id_currencyfx, App.Modules.Sales);
                    foreach (payment_schedual parent in payment_schedualList.Where(x => x.AccountReceivableBalance > 0))
                    {

                        if (ChildBalance > 0)
                        {
                            payment_schedual child_schedual = new payment_schedual();
                            //Payment Detail is greater or equal to Schedual
                            if (ChildBalance >= parent.AccountReceivableBalance)
                            {
                                child_schedual.credit = parent.AccountReceivableBalance;
                                child_schedual.parent = base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault();
                                Parent_Schedual = child_schedual.parent;
                                ChildBalance -= parent.AccountReceivableBalance;
                            }
                            else
                            { //Schedual is greater than Payment Detail.
                                if (clean_balance)
                                {
                                    child_schedual.credit = parent.AccountReceivableBalance;
                                    child_schedual.parent = base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault();
                                    Parent_Schedual = child_schedual.parent;
                                    ChildBalance = 0;
                                }
                                else
                                {
                                    child_schedual.credit = ChildBalance;
                                    child_schedual.parent = base.payment_schedual.Where(x => x.id_payment_schedual == parent.id_payment_schedual).FirstOrDefault();
                                    Parent_Schedual = child_schedual.parent;
                                    ChildBalance -= ChildBalance;
                                }
                            }

                            ///
                            if (Parent_Schedual.sales_invoice != null)
                            {
                                number = Parent_Schedual.sales_invoice.number;
                            }


                            if (Parent_Schedual.id_sales_invoice != null)
                            {
                                child_schedual.id_sales_invoice = Parent_Schedual.id_sales_invoice;
                                ModuleName = "SalesInvoice";
                            }

                            ///
                            if (payment_detail.id_sales_return != null)
                            {
                                child_schedual.id_sales_return = payment_detail.id_sales_return;
                                ModuleName += " & Return";
                            }
                            ///
                            if (Parent_Schedual.id_sales_order != null)
                            {
                                child_schedual.id_sales_order = Parent_Schedual.id_sales_order;
                                ModuleName = "SalesOrder";
                            }
                            schedualList.Add(child_schedual);
                        }

                    }
                    //End Mode IF
                }

                ///Code to specify Accounts.
                ///
                payment_type _payment_type = await payment_type.FindAsync(payment_detail.id_payment_type);
                if (_payment_type.payment_behavior == entity.payment_type.payment_behaviours.Normal)
                {
                    ///Creates new Account Detail for each Payment Detail.
                    app_account_detail app_account_detail = new app_account_detail();

                    app_account_detail.id_account = (int)payment_detail.id_account;
                    app_account_detail.app_account = payment_detail.app_account;
                    app_account_detail.id_currencyfx = payment_detail.id_currencyfx;
                    app_account_detail.id_payment_type = payment_detail.id_payment_type;
                    app_account_detail.payment_detail = payment_detail;
                    app_account_detail.trans_date = payment_detail.trans_date;

                    if (_payment_type.is_direct)
                    {
                        app_account_detail.status = Status.Documents_General.Approved;
                    }
                    else
                    {
                        app_account_detail.status = Status.Documents_General.Pending;
                    }

                    if (payment_detail.id_range > 0)
                    {
                        app_document_range detail_document_range = await base.app_document_range.FindAsync(payment_detail.id_range);
                        payment_detail.payment_type_number = Brillo.Logic.Range.calc_Range(detail_document_range, true);
                        payment.RaisePropertyChanged("payment_type_number");
                    }

                    ///Gets the Session ID necesary for cashier movement.
                    int id_account_session = await base.app_account_session.Where(x => x.id_account == payment_detail.id_account && x.is_active).Select(y => y.id_session).FirstOrDefaultAsync();

                    if (id_account_session > 0)
                    {
                        app_account_detail.id_session = id_account_session;
                    }

                    //Logic for Account Detail based on Payment Detail Logic.
                    if (IsRecievable == false)
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
                    string customerName = "";
                    if (Parent_Schedual.contact != null)
                    {
                        customerName = Parent_Schedual.contact.name;
                    }
                    
                  
                    app_account_detail.comment = Localize.StringText(ModuleName) + " " + number + " | " + customerName;
                    app_account_detail.tran_type = app_account_detail.tran_types.Transaction;
                    base.app_account_detail.Add(app_account_detail);
                }

                ChildBalance = Currency.convert_Values(ChildBalance, payment_detail.Default_id_currencyfx, payment_detail.id_currencyfx, App.Modules.Sales);
                if (ChildBalance > 0)
                {
                    payment_detail.value = payment_detail.value;
                }


                ///Logic for Value in Balance Payment Schedual.
                foreach (payment_schedual payment_schedual in schedualList)
                {

                    //Create a Parent Schedual Object.

                    payment_schedual _Parent_Schedual = payment_schedual.parent;
                    if (_Parent_Schedual != null)
                    {
                        payment_schedual.status = Status.Documents_General.Approved;
                        payment_schedual.id_contact = _Parent_Schedual.id_contact;
                        payment_schedual.id_currencyfx = _Parent_Schedual.id_currencyfx;
                        payment_schedual.trans_date = payment_detail.trans_date;
                        payment_schedual.expire_date = _Parent_Schedual.expire_date;
                        payment_detail.payment_schedual.Add(payment_schedual);
                    }
                    else
                    {
                        MessageBox.Show("Check Payments, No Parent Found.");
                    }
                }
                //pankeel
            }

            payment.status = Status.Documents_General.Approved;
            app_document_range app_document_range = await base.app_document_range.FindAsync(payment.id_range);
            if (app_document_range != null)
            {
                payment.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                payment.RaisePropertyChanged("number");
            }

            try
            {
                base.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (is_print)
            {
                Brillo.Document.Start.Automatic(payment, app_document_range);
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

                        app_currencyfx app_currencyfx = base.app_currencyfx.Where(x => x.type == app_currencyfx.CurrencyFXTypes.Transaction &&
                                                             x.id_currency == id_currency && x.timestamp <= timestamp)
                                                            .OrderByDescending(x => x.timestamp).FirstOrDefault();
                        if (app_currencyfx != null)
                        {
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

        public void Anull()
        {
            List<payment> PaymentList = base.payments.Local.Where(x => x.IsSelected).ToList();
            foreach (payment payment in PaymentList)
            {
                if (payment.is_accounted == true && CurrentSession.UserRole.is_master == false)
                {
                    //If Payment is Accounted and user is not Master, ignore this record.
                    continue;
                }

                //Remove Account Detail Information
                foreach (payment_detail payment_detail in payment.payment_detail)
                {
                    base.app_account_detail.RemoveRange(payment_detail.app_account_detail);
                    base.payment_schedual.RemoveRange(payment_detail.payment_schedual);
                    payment.status = Status.Documents_General.Annulled;
                }
            }

            SaveChanges();
        }
    }
}