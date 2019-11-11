using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Controller.Purchase
{
    public class InvoiceController : Base, IDisposable
    {
        public decimal Count { get; set; }

        public int PageSize { get { return _PageSize; } set { _PageSize = value; } }
        public int _PageSize = 100;


        public int PageCount
        {
            get
            {
                return Math.Ceiling((Count / (decimal)PageSize)) < 1 ? 1 : Convert.ToInt32(Math.Ceiling((Count / (decimal)PageSize)));
            }
        }

        public void Dispose()
        {
            // Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this != null)
            {
                if (disposing)
                {
                    this.Dispose();
                    // Dispose other managed resources.
                }
                //release unmanaged resources.
            }
        }
        public async void Load(int PageIndex)
        {

            var predicate = PredicateBuilder.True<purchase_invoice>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.is_archived == false);
            predicate = predicate.And(x => x.id_branch == CurrentSession.Id_Branch);

            if (Count == 0)
            {
                Count = db.purchase_invoice.Where(predicate).Count();
            }

            await db.purchase_invoice.Where(predicate)
                .Include(x => x.contact)
                .OrderByDescending(x => x.trans_date)
                  .Skip(PageIndex * PageSize).Take(PageSize)
                .LoadAsync();

            await db.app_department.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).ToListAsync();
            await db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await db.app_cost_center.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active).OrderBy(a => a.name).ToListAsync();

        }

        #region CRUD

        public purchase_invoice Create(int TransDate_OffSet)
        {
            purchase_invoice purchase_invoice = new purchase_invoice()
            {
                State = EntityState.Added,
                status = Status.Documents_General.Pending,
                IsSelected = true,
                trans_type = Status.TransactionTypes.Normal,
                trans_date = DateTime.Now.AddDays(TransDate_OffSet),
                timestamp = DateTime.Now,

                //Navigation Properties
                app_currencyfx = db.app_currencyfx.Find(CurrentSession.Get_Currency_Default_Rate().id_currencyfx),
                app_branch = db.app_branch.Find(CurrentSession.Id_Branch)
            };
            db.purchase_invoice.Add(purchase_invoice);
            return purchase_invoice;
        }
  
        public purchase_invoice_detail Create_Detail(
           ref purchase_invoice Invoice, item Item, item_movement ItemMovement,
          decimal Quantity)
        {
            purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail()
            {
                State = EntityState.Added,
                CurrencyFX_ID = Invoice.id_currencyfx,
                item_description = Item.name,
                item = Item,
                id_item = Item.id_item,
                quantity = Quantity,
                batch_code = ItemMovement != null ? ItemMovement.code : "",
                expire_date = ItemMovement != null ? ItemMovement.expire_date : null,
            };

            int VatGroupID = (int)purchase_invoice_detail.id_vat_group;
            purchase_invoice_detail.app_vat_group = db.app_vat_group.Find(VatGroupID);

            if (Invoice.app_contract == null && Invoice.id_contract > 0)
            {
                Invoice.app_contract = db.app_contract.Find(Invoice.id_contract);
            }
            
            Invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
            return purchase_invoice_detail;
        }

        public purchase_invoice Edit(purchase_invoice Invoice)
        {
            Invoice.IsSelected = true;
            Invoice.State = EntityState.Modified;
            db.Entry(Invoice).State = EntityState.Modified;

            return Invoice;
        }

        public void Archived()
        {
            foreach (purchase_invoice Invoice in db.purchase_invoice.Local.Where(x => x.IsSelected))
            {
                Invoice.is_archived = Invoice.is_archived ? false : true;
                Invoice.IsSelected = false;
            }
           
            db.SaveChanges();
       

        }

        #endregion

        #region Approve
        public void Approve()
        {
            foreach (purchase_invoice invoice in db.purchase_invoice.Local.Where(x => x.IsSelected == true))
            {
                if (invoice.Error == null)
                {
                    if (invoice.id_purchase_invoice == 0)
                    {
                        SaveChanges_WithValidation();
                    }

                    invoice.app_condition = db.app_condition.Find(invoice.id_condition);
                    invoice.app_contract = db.app_contract.Find(invoice.id_contract);
                    invoice.app_currencyfx = db.app_currencyfx.Find(invoice.id_currencyfx);

                    if (invoice.status == Status.Documents_General.Pending)
                    {
                        if (invoice.code != null)
                        {
                            invoice.contact.code = invoice.code;
                            invoice.contact.trans_code_exp = invoice.contact.trans_code_exp ?? DateTime.Now.AddMonths(1);
                        }

                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();

                        ///Insert Payment Schedual Logic
                        payment_schedualList = _Payment.insert_Schedual(invoice);

                        //Insert into Stock.
                        Insert_Items_2_Movement(invoice);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            db.payment_schedual.AddRange(payment_schedualList);
                        }

                        invoice.status = Status.Documents_General.Approved;
                        SaveChanges_WithValidation();
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
                    item_movementList = _Stock.PurchaseInvoice_Approve(db, invoice);

                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        db.item_movement.AddRange(item_movementList);
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

                        //item_movement_value item_movement_value = item_movement.item_movement_value.FirstOrDefault();
                        //decimal UnitValue = Brillo.Currency.convert_Values(purchase_invoice_detail.unit_cost, invoice.id_currencyfx, CurrentSession.Get_Currency_Default_Rate().id_currencyfx, App.Modules.Purchase);
                        //if (item_movement_value != null)
                        //{
                        //    if (item_movement_value.unit_value != UnitValue)
                        //    {
                        //        item_movement_value.unit_value = UnitValue;
                        //    }
                        //}
                    }
                }
                else
                {
                    //New
                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    db.item_movement.Add(_Stock.CreditOnly_Movement(Status.Stock.InStock, App.Names.PurchaseInvoice, invoice.id_purchase_invoice, purchase_invoice_detail.id_purchase_invoice_detail,
                        invoice.id_currencyfx, purchase_invoice_detail.item.item_product.FirstOrDefault().id_item_product,
                        (int)purchase_invoice_detail.id_location, purchase_invoice_detail.quantity,
                        invoice.trans_date, purchase_invoice_detail.unit_cost, "Purchase Invoice Fix", null, purchase_invoice_detail.expire_date, purchase_invoice_detail.batch_code,null));
                }
            }
            SaveChanges_WithValidation();
        }
        #endregion

        #region Annul

        public void Annull()
        {
            ///Only run through Invoices that have been approved.
            foreach (purchase_invoice Invoice in db.purchase_invoice.Local
                .Where(x => x.IsSelected && x.status == Status.Documents_General.Approved))
            {
                //Block any annull if user is not Master.
                if (Invoice.is_accounted == false || CurrentSession.UserRole.is_master)
                {
                    List<payment_schedual> SchedualList = Invoice.payment_schedual.ToList();
                    //Loop through the Payment Schedual. And remove payments made.
                    foreach (payment_schedual payment_schedual in SchedualList)
                    {
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        _Payment.DeletePaymentSchedual(db, payment_schedual.id_payment_schedual);
                    }

                    ///Since the above Foreach will run through a mix of payment scheduals, we have no way of knowing if we will have
                    ///payment headers. So we run this code to clean.
                    ///
                    List<payment> PaymentList = db.payments.Include(x => x.payment_detail).ToList();
                    List<payment> EmptyPayments = PaymentList.Where(x => x.payment_detail.Count() == 0).ToList();
                    if (EmptyPayments.Count() > 0)
                    {
                        db.payments.RemoveRange(EmptyPayments);
                    }

                    foreach (purchase_invoice_detail detail in Invoice.purchase_invoice_detail)
                    {
                        detail.id_purchase_order_detail = null;
                        List<item_movement> ItemMovementList = detail.item_movement.ToList();
                        foreach (item_movement item_movement in ItemMovementList)
                        {
                            if (item_movement.id_purchase_packing_detail != null)
                            {
                                //Remove ref of sales. keep packing reference. this will allow you to reuse packing ref.
                                item_movement.id_purchase_invoice_detail = null;
                                //delete relationship between detail and packing.
                                db.purchase_packing_detail_relation.RemoveRange(detail.purchase_packing_detail_relation);
                            }
                            else
                            {
                                //TODO Remove parent references of all child movements to prevent error or undue loss of data on next step.
                                foreach (var child in item_movement.child)
                                {
                                    child.parent = null;
                                }

                                //Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                                db.item_movement.Remove(item_movement);
                            }

                            //if (item_movement.Action == item_movement.Actions.Delete)
                            //{
                            //    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                            //    db.item_movement.Remove(item_movement);
                            //}
                            //else if (item_movement.Action == item_movement.Actions.ReApprove)
                            //{
                            //    foreach (var item in item_movement.child)
                            //    {
                            //        List<item_movement> item_movementList = db.item_movement.Where(x =>
                            //        x.id_item_product == item_movement.id_item_product &&
                            //        x.id_movement != item_movement.id_movement &&
                            //        x.debit > 0).ToList();
                            //        foreach (item_movement _item_movement in item_movementList)
                            //        {
                            //            if (_item_movement.avlquantity > item.credit)
                            //            {
                            //                item.parent = _item_movement;
                            //            }
                            //            else
                            //            {
                            //                item.parent = null;
                            //            }
                            //        }
                            //    }
                            //    db.item_movement.Remove(item_movement);
                            //}
                        }
                    }

                    //Change Status to Annulled.
                    Invoice.status = Status.Documents_General.Annulled;
                }

                Invoice.IsSelected = false;
                Invoice.RaisePropertyChanged("status");
            }

            db.SaveChanges();
        }

        #endregion

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (purchase_invoice purchase_invoice in db.purchase_invoice.Local)
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

                    if (purchase_invoice.contact.trans_code != purchase_invoice.code)
                    {
                        purchase_invoice.contact.trans_code = purchase_invoice.code;
                    }

                    if (purchase_invoice.State == EntityState.Added)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        db.Entry(purchase_invoice).State = EntityState.Added;
                    }
                    else if (purchase_invoice.State == EntityState.Modified)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        db.Entry(purchase_invoice).State = EntityState.Modified;
                    }
                    else if (purchase_invoice.State == EntityState.Deleted)
                    {
                        purchase_invoice.timestamp = DateTime.Now;
                        purchase_invoice.State = EntityState.Unchanged;
                        db.purchase_invoice.Remove(purchase_invoice);
                    }

                    purchase_invoice.IsSelected = false;
                    NumberOfRecords += 1;
                }
                else if (purchase_invoice.State > 0)
                {
                    if (purchase_invoice.State != EntityState.Unchanged)
                    {
                        db.Entry(purchase_invoice).State = EntityState.Unchanged;

                        if (purchase_invoice.purchase_invoice_detail.Count() > 0)
                        {
                            db.purchase_invoice_detail.RemoveRange(purchase_invoice.purchase_invoice_detail);
                        }
                    }
                }
            }

            foreach (var error in db.GetValidationErrors())
            {
                db.Entry(error.Entry.Entity).State = EntityState.Detached;
            }

            if (db.GetValidationErrors().Count() > 0)
            {
                return false;
            }
            else
            {
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
              
                return true;
            }
        }


        /// <summary>
        /// Links a Sales Packing and brings items into Sales Invoice.
        /// </summary>
        /// <param name="Invoice">Sales Invoice</param>
        /// <param name="PackingID">Sales Packing ID</param>
        /// <returns>True if Correct. False if Error or Unfinsihed</returns>
        /// 
        public bool Link_PackingList(purchase_invoice Invoice, int PackingID)
        {
            //Bring into Context.
            purchase_packing Packing = db.purchase_packing.Find(PackingID);

            foreach (purchase_packing_detail _purchase_packing_detail in Packing.purchase_packing_detail.Where(x => x.verified_by!=null))
            {
                purchase_order_detail purchase_order_detail = _purchase_packing_detail.purchase_order_detail;

                purchase_invoice_detail Detail = new purchase_invoice_detail()
                {
                    id_location = _purchase_packing_detail.id_location
                };

              purchase_packing_detail_relation purchase_packing_relation = new purchase_packing_detail_relation()
                {
                    id_purchase_packing_detail = _purchase_packing_detail.id_purchase_packing_detail,
                  purchase_packing_detail = _purchase_packing_detail,
                  //id_sales_invoice_detail = Detail.id_sales_invoice_detail,
                  purchase_invoice_detail = Detail
                };

                Detail.purchase_packing_detail_relation.Add(purchase_packing_relation);

                if (_purchase_packing_detail.expire_date != null || !string.IsNullOrEmpty(_purchase_packing_detail.batch_code))
                {
                    Detail.expire_date = _purchase_packing_detail.expire_date;
                    Detail.batch_code = _purchase_packing_detail.batch_code;
                }

                //if SalesOrder Exists, use it for Price and VAT.
                if (purchase_order_detail != null)
                {
                    Detail.purchase_invoice = Invoice;
                    Detail.id_cost_center = purchase_order_detail.id_cost_center;
                    Detail.item = _purchase_packing_detail.item;
                    Detail.id_item = _purchase_packing_detail.id_item;
                    Detail.quantity = Convert.ToDecimal(_purchase_packing_detail.verified_quantity);
                    Detail.id_vat_group = purchase_order_detail.id_vat_group;
                    Detail.State = EntityState.Added;
                    Detail.unit_cost = purchase_order_detail.unit_cost + purchase_order_detail.discount;
                    Detail.discount = purchase_order_detail.discount;
             
                    Invoice.purchase_invoice_detail.Add(Detail);
                }
                else
                {
                    //If Sales Order does not exist, use Price and VAT From standard of the company.
                    Create_Detail(ref Invoice,
                        _purchase_packing_detail.item,
                        null,
                        (decimal)_purchase_packing_detail.verified_quantity);
                }
            }

            return true;
        }
    }
}
