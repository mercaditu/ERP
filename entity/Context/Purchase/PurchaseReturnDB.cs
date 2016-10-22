using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq; 
using System.Threading.Tasks;

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
            purchase_return.app_branch = app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault();
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
            foreach (purchase_return purchase_return in base.purchase_return.Local.Where(x => x.IsSelected == true))
            {
                if (purchase_return.Error == null)
                {
                    if (purchase_return.id_purchase_return == 0)
                    {
                        SaveChanges();
                    }

                    if (purchase_return.status != Status.Documents_General.Approved)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.insert_Schedual(purchase_return);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(this, purchase_return);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }


                        if (purchase_return.purchase_invoice != null)
                        {


                            payment payment = new payment();
                            payment.id_contact = purchase_return.id_contact;
                            payment.status = Status.Documents_General.Approved;

                            payment_detail payment_detailreturn = new payment_detail();
                            // payment_detailreturn.id_account = payment_quick.payment_detail.id_account;
                            payment_detailreturn.id_currencyfx = purchase_return.id_currencyfx;
                            if (base.payment_type.Where(x => x.payment_behavior == entity.payment_type.payment_behaviours.CreditNote).FirstOrDefault() != null)
                            {
                                payment_detailreturn.id_payment_type = base.payment_type.Where(x => x.payment_behavior == entity.payment_type.payment_behaviours.CreditNote).FirstOrDefault().id_payment_type;
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Please add crditnote payment type...");
                                return;
                            }


                            payment_detailreturn.id_sales_return = purchase_return.id_purchase_return;

                            payment_detailreturn.value = purchase_return.GrandTotal;

                            payment_schedual payment_schedualReturn = new payment_schedual();
                            payment_schedualReturn.debit = purchase_return.GrandTotal;
                            payment_schedualReturn.credit =0 ;
                            payment_schedualReturn.id_currencyfx = purchase_return.id_currencyfx;
                            payment_schedualReturn.purchase_return = purchase_return;
                            payment_schedualReturn.trans_date = purchase_return.trans_date;
                            payment_schedualReturn.expire_date = purchase_return.trans_date;
                            payment_schedualReturn.status = entity.Status.Documents_General.Approved;
                            payment_schedualReturn.id_contact = purchase_return.id_contact;
                            payment_schedualReturn.can_calculate = true;
                            payment_schedualReturn.parent = purchase_return.purchase_invoice.payment_schedual.FirstOrDefault();

                            payment_detailreturn.payment_schedual.Add(payment_schedualReturn);
                            payment.payment_detail.Add(payment_detailreturn);
                            base.payments.Add(payment);
                        }
                        purchase_return.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }
                }
                else if (purchase_return.Error != null)
                {
                    purchase_return.HasErrors = true;
                }
            }
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
