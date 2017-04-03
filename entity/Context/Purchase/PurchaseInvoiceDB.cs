using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class PurchaseInvoiceDB : BaseDB
    {
        public purchase_invoice New(int DaysOffSet)
        {
            purchase_invoice purchase_invoice = new purchase_invoice();

            purchase_invoice.status = Status.Documents_General.Pending;
            purchase_invoice.trans_date = DateTime.Now.AddDays(DaysOffSet);
            purchase_invoice.State = EntityState.Added;
            purchase_invoice.IsSelected = true;
            purchase_invoice.id_condition = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_condition).FirstOrDefault();
            purchase_invoice.id_contract = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_contract).FirstOrDefault();

            purchase_invoice.app_branch = app_branch.Find(CurrentSession.Id_Branch);
            base.Entry(purchase_invoice).State = EntityState.Added;
            return purchase_invoice;
        }

        public override int SaveChanges()
        {
            validate_Invoice();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Invoice();
            return base.SaveChangesAsync();
        }

        private void validate_Invoice()
        {
            NumberOfRecords = 0;

            foreach (purchase_invoice purchase_invoice in base.purchase_invoice.Local)
            {
                if (purchase_invoice.IsSelected && purchase_invoice.Error == null)
                {
                    //Data Loading Code. If data is not set, then Cognitivo ERP should try to fill up.
                    if (purchase_invoice.contact.id_contract == 0)
                    {
                        purchase_invoice.contact.id_contract = purchase_invoice.id_contract;
                    }

                    if (purchase_invoice.contact.id_currency == 0)
                    {
                        purchase_invoice.contact.id_currency = purchase_invoice.app_currencyfx.id_currency;
                    }

                    if (purchase_invoice.contact.id_cost_center == 0 && purchase_invoice.purchase_invoice_detail.FirstOrDefault() != null)
                    {
                        purchase_invoice.contact.id_cost_center = purchase_invoice.purchase_invoice_detail.FirstOrDefault().id_cost_center;
                    }

                    if (purchase_invoice.State == EntityState.Added)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        Entry(purchase_invoice).State = EntityState.Added;
                    }
                    else if (purchase_invoice.State == EntityState.Modified)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        Entry(purchase_invoice).State = EntityState.Modified;
                    }
                    else if (purchase_invoice.State == EntityState.Deleted)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        base.purchase_invoice.Remove(purchase_invoice);
                    }
                    NumberOfRecords += 1;
                }
                else if (purchase_invoice.State > 0)
                {
                    if (purchase_invoice.State != EntityState.Unchanged)
                    {
                        Entry(purchase_invoice).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve()
        {
            foreach (purchase_invoice invoice in base.purchase_invoice.Local.Where(x => x.IsSelected == true))
            {
                if (invoice.Error == null)
                {
                    if (invoice.id_purchase_invoice == 0)
                    {
                        SaveChanges();
                    }

                    invoice.app_condition = app_condition.Find(invoice.id_condition);
                    invoice.app_contract = app_contract.Find(invoice.id_contract);
                    invoice.app_currencyfx = app_currencyfx.Find(invoice.id_currencyfx);

                    if (invoice.status == Status.Documents_General.Pending)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();

                        ///Insert Payment Schedual Logic
                        payment_schedualList = _Payment.insert_Schedual(invoice);

                        //Insert into Stock.
                        Insert_Items_2_Movement(invoice);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }

                        invoice.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }
                }
                else if (invoice.Error != null)
                {
                    invoice.HasErrors = true;
                }

                invoice.IsSelected = false;
            }
        }

        /// <summary>
        /// Executes code that will insert Invoiced Items into Movement.
        /// </summary>
        /// <param name="Invoice"></param>
        public void Insert_Items_2_Movement(purchase_invoice invoice)
        {
            if (invoice.purchase_invoice_detail.Where(x => x.item != null).Any())
            {
                if (invoice.status == Status.Documents_General.Annulled)
                {
                    //Logica
                    ReApprove(invoice);
                }
                else // Pending
                {
                    List<item_movement> item_movementList = new List<item_movement>();

                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    item_movementList = _Stock.PurchaseInvoice_Approve(this, invoice);

                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        item_movement.AddRange(item_movementList);
                    }
                }
            }
        }

        private void ReApprove(purchase_invoice invoice)
        {
            foreach (purchase_invoice_detail purchase_invoice_detail in invoice.purchase_invoice_detail.Where(x => x.item.item_product.Count() > 0))
            {
                if (purchase_invoice_detail.item_movement.Count > 0)
                {
                    item_movement item_movement = purchase_invoice_detail.item_movement.FirstOrDefault();
                    if (item_movement != null)
                    {
                        item_movement.trans_date = invoice.trans_date;
                        item_movement.timestamp = DateTime.Now;

                        if (item_movement.credit != purchase_invoice_detail.quantity)
                        {
                            item_movement.credit = purchase_invoice_detail.quantity;
                        }

                        item_movement_value item_movement_value = item_movement.item_movement_value.FirstOrDefault();
                        decimal UnitValue = Brillo.Currency.convert_Values(purchase_invoice_detail.unit_cost, invoice.id_currencyfx, CurrentSession.Get_Currency_Default_Rate().id_currencyfx, App.Modules.Purchase);
                        if (item_movement_value != null)
                        {
                            if (item_movement_value.unit_value != UnitValue)
                            {
                                item_movement_value.unit_value = UnitValue;
                            }
                        }
                    }
                }
                else
                {
                    //New
                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    base.item_movement.Add(_Stock.CreditOnly_Movement(Status.Stock.InStock, App.Names.PurchaseInvoice, invoice.id_purchase_invoice, purchase_invoice_detail.id_purchase_invoice_detail,
                        invoice.id_currencyfx, purchase_invoice_detail.item.item_product.FirstOrDefault().id_item_product,
                        (int)purchase_invoice_detail.id_location, purchase_invoice_detail.quantity,
                        invoice.trans_date, purchase_invoice_detail.unit_cost, "Purchase Invoice Fix", null, purchase_invoice_detail.expire_date, purchase_invoice_detail.batch_code));
                }
            }
            SaveChanges();
        }

        public void Anull()
        {
            foreach (purchase_invoice purchase_invoice in base.purchase_invoice.Local)
            {
                if (purchase_invoice.IsSelected && purchase_invoice.status == Status.Documents_General.Approved && purchase_invoice.Error == null)
                {
                    int count = purchase_invoice.purchase_invoice_detail.Where(x => x.purchase_return_detail == null).Count();
                    if (count > 0)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.revert_Schedual(purchase_invoice);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.revert_Stock(this, App.Names.PurchaseInvoice, purchase_invoice);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            base.payment_schedual.RemoveRange(payment_schedualList);
                        }

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            base.item_movement.RemoveRange(item_movementList);
                        }

                        purchase_invoice.status = Status.Documents_General.Annulled;
                        SaveChanges();
                    }
                }
            }
        }
    }
}