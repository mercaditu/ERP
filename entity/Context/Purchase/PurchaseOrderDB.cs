using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class PurchaseOrderDB : BaseDB
    {
        public purchase_order New(int DaysOffSet)
        {
            purchase_order purchase_order = new purchase_order();
            purchase_order.State = EntityState.Added;
            purchase_order.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.PurchaseOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
            purchase_order.id_condition = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_condition;
            purchase_order.id_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_contract;
            purchase_order.status = Status.Documents_General.Pending;
            purchase_order.trans_date = DateTime.Now.AddDays(DaysOffSet);
            purchase_order.app_branch = app_branch.Find(CurrentSession.Id_Branch);
            purchase_order.app_terminal = app_terminal.Find(CurrentSession.Id_Terminal);
            purchase_order.IsSelected = true;
            base.Entry(purchase_order).State = EntityState.Added;
            return purchase_order;
        }

        public override int SaveChanges()
        {
            validate_Order();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Order();
            return base.SaveChangesAsync();
        }

        private void validate_Order()
        {
            NumberOfRecords = 0;

            foreach (purchase_order purchase_order in base.purchase_order.Local)
            {
                if (purchase_order.IsSelected && purchase_order.Error == null)
                {
                    if (purchase_order.contact.id_contract == 0)
                    {
                        purchase_order.contact.id_contract = purchase_order.id_contract;
                    }

                    if (purchase_order.contact.id_currency == 0)
                    {
                        purchase_order.contact.id_currency = purchase_order.app_currencyfx.id_currency;
                    }

                    if (purchase_order.contact.id_cost_center == 0 && purchase_order.purchase_order_detail.FirstOrDefault() != null)
                    {
                        purchase_order.contact.id_cost_center = purchase_order.purchase_order_detail.FirstOrDefault().id_cost_center;
                    }

                    if (purchase_order.State == EntityState.Added)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        Entry(purchase_order).State = EntityState.Added;
                    }
                    else if (purchase_order.State == EntityState.Modified)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        Entry(purchase_order).State = EntityState.Modified;
                    }
                    else if (purchase_order.State == EntityState.Deleted)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        base.purchase_order.Remove(purchase_order);
                    }

                    NumberOfRecords += 1;
                }
                else if (purchase_order.State > 0)
                {
                    if (purchase_order.State != EntityState.Unchanged)
                    {
                        Entry(purchase_order).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public bool Approve()
        {
            NumberOfRecords = 0;

            foreach (purchase_order purchase_order in base.purchase_order.Local.Where(x => x.IsSelected == true))
            {
                if (purchase_order.Error == null)
                {
                    if (purchase_order.id_purchase_order == 0)
                    {
                        SaveChanges();
                    }
                    purchase_order.app_condition = app_condition.Find(purchase_order.id_condition);
                    purchase_order.app_contract = app_contract.Find(purchase_order.id_contract);
                    purchase_order.app_currencyfx = app_currencyfx.Find(purchase_order.id_currencyfx);
                    if (purchase_order.status != Status.Documents_General.Approved)
                    {
                        if (purchase_order.number == null && purchase_order.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == purchase_order.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == purchase_order.id_terminal).FirstOrDefault().code;

                            app_document_range app_document_range = base.app_document_range.Find(purchase_order.id_range);
                            purchase_order.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            purchase_order.RaisePropertyChanged("number");
                            purchase_order.is_issued = true;
                        }
                        else
                        {
                            purchase_order.is_issued = false;
                        }

                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.insert_Schedual(purchase_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }

                        purchase_order.status = Status.Documents_General.Approved;

                        foreach (purchase_order_detail purchase_order_detail in purchase_order.purchase_order_detail)
                        {
                            if (purchase_order_detail.item.id_item_type == item.item_type.ServiceContract)
                            {
                                production_service_account production_service_account = new entity.production_service_account();
                                production_service_account.id_contact = purchase_order_detail.purchase_order.id_contact;
                                production_service_account.id_item = (int)purchase_order_detail.id_item;
                                production_service_account.id_purchase_order_detail = purchase_order_detail.id_purchase_order_detail;
                                production_service_account.unit_cost = purchase_order_detail.unit_cost;
                                production_service_account.debit = 0;
                                production_service_account.credit = purchase_order_detail.quantity;
                                production_service_account.exp_date = purchase_order_detail.expire_date;
                                base.production_service_account.Add(production_service_account);
                            }
                        }

                        SaveChanges();
                    }

                    if (purchase_order.is_issued)
                    {
                        app_document_range app_document_range = base.app_document_range.Find(purchase_order.id_range);
                        Brillo.Document.Start.Automatic(purchase_order, app_document_range);
                    }

                    NumberOfRecords += 1;
                    purchase_order.IsSelected = false;
                }
                else if (purchase_order.Error != null)
                {
                    purchase_order.HasErrors = true;
                }
            }

            return true;
        }

        public bool Anull()
        {
            NumberOfRecords = 0;
            foreach (purchase_order purchase_order in base.purchase_order.Local)
            {
                if (purchase_order.IsSelected && purchase_order.Error == null)
                {
                    if (purchase_order.purchase_invoice.Count() == 0)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.revert_Schedual(purchase_order);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.revert_Stock(this, App.Names.PurchaseOrder, purchase_order.id_purchase_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            base.payment_schedual.RemoveRange(payment_schedualList);
                        }

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            base.item_movement.RemoveRange(item_movementList);
                        }

                        purchase_order.status = Status.Documents_General.Annulled;
                        SaveChanges();

                        //Clean Up
                        purchase_order.IsSelected = false;
                        NumberOfRecords += 1;
                    }
                }
            }
            return true;
        }
    }
}