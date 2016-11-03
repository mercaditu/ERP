using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WPFLocalizeExtension.Extensions;

namespace entity
{
    public partial class PurchaseReturnDB : BaseDB
    {
        public purchase_return New()
        {
            purchase_return purchase_return = new purchase_return();

            purchase_return.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.PurchaseReturn, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
            purchase_return.status = Status.Documents_General.Pending;
            purchase_return.trans_date = DateTime.Now;

            purchase_return.State = EntityState.Added;
            purchase_return.IsSelected = true;
            purchase_return.app_branch = app_branch.Find(CurrentSession.Id_Branch);
            return purchase_return;
        }

        public override int SaveChanges()
        {
            validate_Return();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Return();
            return base.SaveChangesAsync();
        }


        private void validate_Return()
        {
            foreach (purchase_return purchase_return in base.purchase_return.Local)
            {
                if (purchase_return.IsSelected && purchase_return.Error == null)
                {
                    if (purchase_return.State == EntityState.Added)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        Entry(purchase_return).State = EntityState.Added;
                    }
                    else if (purchase_return.State == EntityState.Modified)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        Entry(purchase_return).State = EntityState.Modified;
                    }
                    else if (purchase_return.State == EntityState.Deleted)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        base.purchase_return.Remove(purchase_return);
                    }
                }
                else if (purchase_return.State > 0)
                {
                    if (purchase_return.State != EntityState.Unchanged)
                    {
                        Entry(purchase_return).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve()
        {
            foreach(purchase_return purchase_return in base.purchase_return.Local.Where(x => x.status != Status.Documents_General.Approved))
            {
                if (purchase_return.status != Status.Documents_General.Approved &&
                    purchase_return.IsSelected &&
                    purchase_return.Error == null)
                {
                    if (purchase_return.id_purchase_return == 0)
                    {
                        SaveChanges();
                    }

                    purchase_return.app_condition = app_condition.Find(purchase_return.id_condition);
                    purchase_return.app_contract = app_contract.Find(purchase_return.id_contract);
                    purchase_return.app_currencyfx = app_currencyfx.Find(purchase_return.id_currencyfx);

                    if (purchase_return.status != Status.Documents_General.Approved)
                    {
                        if (purchase_return.number == null && purchase_return.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == purchase_return.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == purchase_return.id_terminal).FirstOrDefault().code;

                            app_document_range app_document_range = base.app_document_range.Find(purchase_return.id_range);

                            purchase_return.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            purchase_return.RaisePropertyChanged("number");
                            purchase_return.is_issued = true;

                            //Save values before printing.
                            SaveChanges();

                            Brillo.Document.Start.Automatic(purchase_return, app_document_range);
                        }
                        else
                        {
                            purchase_return.is_issued = false;
                        }

                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        payment_schedualList = _Payment.insert_Schedual(purchase_return);
                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.revert_Stock(this, App.Names.PurchaseReturn, purchase_return);
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

                        SaveChanges();

                        //Automatically Link Return & Purchase
                        Linked2Sales(purchase_return);

                        purchase_return.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }
                    else if (purchase_return.Error != null)
                    {
                        purchase_return.HasErrors = true;
                    }
                }
            }

        }

        private void Linked2Sales(purchase_return purchase_return)
        {
            payment_type payment_type = base.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.CreditNote).FirstOrDefault();

            payment payment = new payment();
            payment.id_contact = purchase_return.id_contact;
            payment.status = Status.Documents_General.Approved;

            BrilloQuery.Sales Sales = new BrilloQuery.Sales();
            List<BrilloQuery.ReturnInvoice_Integration> ReturnList = Sales.Get_ReturnInvoice_Integration(purchase_return.id_purchase_return);

            foreach (BrilloQuery.ReturnInvoice_Integration item in ReturnList)
            {
                if (item.InvoiceID > 0)
                {
                    //Sales Invoice Integrated.
                    purchase_invoice purchase_invoice = base.purchase_invoice.Find(item.InvoiceID);
                    decimal Return_GrandTotal_ByInvoice = ReturnList.Where(x => x.InvoiceID == item.InvoiceID).Sum(x => x.SubTotalVAT);

                    foreach (payment_schedual payment_schedual in purchase_invoice.payment_schedual.Where(x => x.AccountPayableBalance > 0))
                    {
                        if (payment_schedual.AccountPayableBalance > 0 && Return_GrandTotal_ByInvoice > 0)
                        {
                            decimal PaymentValue = payment_schedual.AccountPayableBalance < Return_GrandTotal_ByInvoice ? payment_schedual.AccountPayableBalance : Return_GrandTotal_ByInvoice;
                            Return_GrandTotal_ByInvoice -= PaymentValue;

                            payment_schedual Schedual = new payment_schedual();
                            Schedual.debit = 0;
                            Schedual.credit = PaymentValue;
                            Schedual.id_currencyfx = purchase_return.id_currencyfx;
                            Schedual.purchase_return = purchase_return;
                            Schedual.trans_date = purchase_return.trans_date;
                            Schedual.expire_date = purchase_return.trans_date;
                            Schedual.status = Status.Documents_General.Approved;
                            Schedual.id_contact = purchase_return.id_contact;
                            Schedual.can_calculate = true;
                            Schedual.parent = base.payment_schedual.Where(x => x.id_purchase_return == purchase_return.id_purchase_return).FirstOrDefault();

                            payment_detail payment_detail = new payment_detail();
                            payment_detail.id_currencyfx = purchase_return.id_currencyfx;
                            payment_detail.id_sales_return = purchase_return.id_purchase_return;
                            payment_detail.payment_type = payment_type != null ? payment_type : Fix_PaymentType();

                            payment_detail.value = PaymentValue;
                            payment_detail.payment_schedual.Add(Schedual);

                            payment.payment_detail.Add(payment_detail);
                        }
                    }
                }
            }

            base.payments.Add(payment);
        }

        private payment_type Fix_PaymentType()
        {
            //In case Payment type doesn not exist, this will create it and try to fix the error.
            payment_type payment_type = new payment_type();
            payment_type.payment_behavior = entity.payment_type.payment_behaviours.CreditNote;
            payment_type.name = LocExtension.GetLocalizedValue<string>("Cognitivo:local:PurchaseReturn");
            base.payment_type.Add(payment_type);

            return payment_type;
        }

        public void Anull()
        {
            foreach (purchase_return purchase_return in base.purchase_return.Local)
            {
                if (purchase_return.IsSelected && purchase_return.Error == null)
                {
                    List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                    Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                    payment_schedualList = _Payment.revert_Schedual(purchase_return);

                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    List<item_movement> item_movementList = new List<item_movement>();
                    item_movementList = _Stock.revert_Stock(this, App.Names.PurchaseReturn, purchase_return);

                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        base.payment_schedual.RemoveRange(payment_schedualList);
                    }
                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        base.item_movement.RemoveRange(item_movementList);
                    }
                    purchase_return.status = Status.Documents_General.Annulled;
                    base.SaveChanges();
                }
            }
        }
    }
}
