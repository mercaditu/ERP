using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace entity.Controller.Purchase
{
    public class OrderController : Base
    {
        public async void Load(bool filterbyBranch)
        {
            var predicate = PredicateBuilder.True<purchase_order>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.is_archived == false);
                    
            if (filterbyBranch)
            {
                predicate = predicate.And(x => x.id_branch == CurrentSession.Id_Branch);
            }

          
            await db.purchase_order.Where(predicate)
                    .OrderByDescending(x => x.trans_date)
                    .ThenBy(x => x.number)
                    .LoadAsync();

           

            await db.app_department.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).ToListAsync();
            await db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await db.app_cost_center.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active).OrderBy(a => a.name).ToListAsync();

        }

        #region CRUD

        public purchase_order Create(int TransDate_OffSet)
        {
            purchase_order purchase_order = new purchase_order()
            {
                State = EntityState.Added,
            app_document_range = Brillo.Logic.Range.List_Range(db, App.Names.PurchaseOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault(),
            id_condition = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_condition,
            id_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_contract,
            status = Status.Documents_General.Pending,
            trans_date = DateTime.Now.AddDays(TransDate_OffSet),
            app_branch = db.app_branch.Find(CurrentSession.Id_Branch),
            app_terminal = db.app_terminal.Find(CurrentSession.Id_Terminal),
            IsSelected = true,

            //Navigation Properties
            app_currencyfx = db.app_currencyfx.Find(CurrentSession.Get_Currency_Default_Rate().id_currencyfx)
              
            };
            db.purchase_order.Add(purchase_order);
            return purchase_order;
        }

        public purchase_order Edit(purchase_order Order)
        {
            Order.IsSelected = true;
            Order.State = EntityState.Modified;
            db.Entry(Order).State = EntityState.Modified;

            return Order;
        }

        public void Archived()
        {
            foreach (purchase_order Order in db.purchase_order.Local.Where(x => x.IsSelected))
            {
                Order.is_archived = Order.is_archived ? false : true;
                Order.IsSelected = false;
            }

            db.SaveChanges();
        }

        #endregion

        #region Save

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (purchase_order purchase_order in db.purchase_order.Local)
            {
                if (purchase_order.IsSelected)
                {
                    if (purchase_order.State == EntityState.Added)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        db.Entry(purchase_order).State = EntityState.Added;
                        purchase_order.IsSelected = false;
                    }
                    else if (purchase_order.State == EntityState.Modified)
                    {
                        purchase_order.timestamp = DateTime.Now;
                        purchase_order.State = EntityState.Unchanged;
                        db.Entry(purchase_order).State = EntityState.Modified;
                        purchase_order.IsSelected = false;
                    }
                    NumberOfRecords += 1;
                }

                if (purchase_order.State > 0)
                {
                    if (purchase_order.State != EntityState.Unchanged)
                    {
                        if (purchase_order.purchase_order_detail.Count() > 0)
                        {
                            db.purchase_order_detail.RemoveRange(purchase_order.purchase_order_detail);
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
                db.SaveChanges();
                return true;
            }

        }

        #endregion

        #region Approve
        public void Approve()
        {
            foreach (purchase_order purchase_order in db.purchase_order.Local.Where(x => x.IsSelected == true))
            {
                if (purchase_order.Error == null)
                {
                    if (purchase_order.id_purchase_order == 0)
                    {
                        SaveChanges_WithValidation();
                    }
                    purchase_order.app_condition = db.app_condition.Find(purchase_order.id_condition);
                    purchase_order.app_contract = db.app_contract.Find(purchase_order.id_contract);
                    purchase_order.app_currencyfx = db.app_currencyfx.Find(purchase_order.id_currencyfx);

                    if (purchase_order.status != Status.Documents_General.Approved)
                    {
                        if (purchase_order.number == null && purchase_order.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == purchase_order.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == purchase_order.id_terminal).FirstOrDefault().code;

                            app_document_range app_document_range = db.app_document_range.Find(purchase_order.id_range);
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
                            db.payment_schedual.AddRange(payment_schedualList);
                        }

                        purchase_order.status = Status.Documents_General.Approved;

                        foreach (purchase_order_detail purchase_order_detail in purchase_order.purchase_order_detail.Where(x => x.item != null))
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
                                db.production_service_account.Add(production_service_account);
                            }
                        }

                        SaveChanges_WithValidation();
                    }

                    if (purchase_order.is_issued)
                    {
                        app_document_range app_document_range = db.app_document_range.Find(purchase_order.id_range);
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
        }
       
    
        #endregion

        #region Annul

        public bool Annull()
        {
            NumberOfRecords = 0;
            foreach (purchase_order purchase_order in db.purchase_order.Local)
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
                        item_movementList = _Stock.revert_Stock(db, App.Names.PurchaseOrder, purchase_order.id_purchase_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            db.payment_schedual.RemoveRange(payment_schedualList);
                        }

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            db.item_movement.RemoveRange(item_movementList);
                        }

                        purchase_order.status = Status.Documents_General.Annulled;
                        SaveChanges_WithValidation();

                        //Clean Up
                        purchase_order.IsSelected = false;
                        NumberOfRecords += 1;
                    }
                }
            }
            return true;
        }

        #endregion

   
    }
}
