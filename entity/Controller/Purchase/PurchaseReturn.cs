using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WPFLocalizeExtension.Extensions;

namespace entity.Controller.Purchase
{
   public class ReturnController:Base
    {
        public int Count { get; set; }

        public int PageSize { get { return _PageSize; } set { _PageSize = value; } }
        public int _PageSize = 100;


        public int PageCount
        {
            get
            {
                return (Count % PageSize) == 0 ? (Count % PageSize) : (Count / PageSize) + 1;
            }
        }
        public async void Load(int PageIndex)
        {
            var predicate = PredicateBuilder.True<purchase_return>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.is_archived == false);

            if (Count == 0)
            {
                Count = db.purchase_return.Where(predicate).Count();
            }

            await db.purchase_return.Where(predicate)
                    .OrderByDescending(x => x.trans_date)
                    .ThenBy(x => x.number)
                        .Skip(PageIndex * PageSize).Take(PageSize)
                    .LoadAsync();

            await db.app_cost_center.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active).OrderBy(a => a.name).ToListAsync();

        }

        #region CRUD

        public purchase_return Create()
        {
            purchase_return purchase_return = new purchase_return()
            {
                State = EntityState.Added,
                app_document_range = Brillo.Logic.Range.List_Range(db, App.Names.PurchaseReturn, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault(),
               // id_condition = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_condition,
              //  id_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_contract,
                status = Status.Documents_General.Pending,
                trans_date = DateTime.Now,
                app_branch = db.app_branch.Find(CurrentSession.Id_Branch),
                app_terminal = db.app_terminal.Find(CurrentSession.Id_Terminal),
                IsSelected = true,

                //Navigation Properties
                app_currencyfx = db.app_currencyfx.Find(CurrentSession.Get_Currency_Default_Rate().id_currencyfx)

            };
            if (CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault() != null)
            {
                purchase_return.id_condition = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_condition;
            }
            else
            {
                purchase_return.id_condition = CurrentSession.Contracts.FirstOrDefault().id_condition;
            }
            if (CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault() != null)
            {
                purchase_return.id_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_contract;
            }
            else
            {
                purchase_return.id_contract = CurrentSession.Contracts.FirstOrDefault().id_contract;
            }

            db.purchase_return.Add(purchase_return);
            return purchase_return;
        }

        public purchase_return Edit(purchase_return Return)
        {
            Return.IsSelected = true;
            Return.State = EntityState.Modified;
            db.Entry(Return).State = EntityState.Modified;

            return Return;
        }

        public void Archived()
        {
            foreach (purchase_return Return in db.purchase_return.Local.Where(x => x.IsSelected))
            {
                Return.is_archived = Return.is_archived ? false : true;
                Return.IsSelected = false;
            }

            db.SaveChanges();
        }

        #endregion

        #region Save

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (purchase_return purchase_return in db.purchase_return.Local)
            {
                if (purchase_return.IsSelected && purchase_return.Error == null)
                {
                    if (purchase_return.State == EntityState.Added)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        db.Entry(purchase_return).State = EntityState.Added;
                        purchase_return.IsSelected = false;
                    }
                    else if (purchase_return.State == EntityState.Modified)
                    {
                        purchase_return.timestamp = DateTime.Now;
                        purchase_return.State = EntityState.Unchanged;
                        db.Entry(purchase_return).State = EntityState.Modified;
                        purchase_return.IsSelected = false;
                    }
                    NumberOfRecords += 1;
                }

                if (purchase_return.State > 0)
                {
                    if (purchase_return.State != EntityState.Unchanged && purchase_return.Error != null)
                    {
                        if (purchase_return.purchase_return_detail.Count() > 0)
                        {
                            db.purchase_return_detail.RemoveRange(purchase_return.purchase_return_detail);
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
        public bool Approve()
        {
            NumberOfRecords = 0;

            foreach (purchase_return purchase_return in db.purchase_return.Local.Where(x => x.status != Status.Documents_General.Approved))
            {
                if (purchase_return.status != Status.Documents_General.Approved &&
                    purchase_return.IsSelected &&
                    purchase_return.Error == null)
                {
                    if (purchase_return.id_purchase_return == 0)
                    {
                        SaveChanges_WithValidation();
                    }

                    purchase_return.app_condition = db.app_condition.Find(purchase_return.id_condition);
                    purchase_return.app_contract = db.app_contract.Find(purchase_return.id_contract);
                    purchase_return.app_currencyfx = db.app_currencyfx.Find(purchase_return.id_currencyfx);

                    if (purchase_return.status != Status.Documents_General.Approved)
                    {
                        if (purchase_return.number == null && purchase_return.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == purchase_return.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == purchase_return.id_terminal).FirstOrDefault().code;

                            app_document_range app_document_range = db.app_document_range.Find(purchase_return.id_range);

                            purchase_return.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            purchase_return.RaisePropertyChanged("number");
                            purchase_return.is_issued = true;

                            //Save values before printing.
                            SaveChanges_WithValidation();

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
                            db.payment_schedual.AddRange(payment_schedualList);
                        }

                        CurrentItems.getProducts_InStock(purchase_return.id_branch, DateTime.Now, true);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.PurchaseReturn_Approve(db, purchase_return);
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            db.item_movement.AddRange(item_movementList);
                        }

                        SaveChanges_WithValidation();

                        //Automatically Link Return & Purchase
                        Linked2Sales(purchase_return);

                        purchase_return.status = Status.Documents_General.Approved;
                        SaveChanges_WithValidation();

                        NumberOfRecords += 1;
                        purchase_return.IsSelected = false;
                    }
                    else if (purchase_return.Error != null)
                    {
                        purchase_return.HasErrors = true;
                    }
                }
            }

            return true;
        }

        private void Linked2Sales(purchase_return purchase_return)
        {
            payment_type payment_type = db.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.CreditNote).FirstOrDefault();

            payment payment = new payment();
            payment.id_contact = purchase_return.id_contact;
            payment.status = Status.Documents_General.Approved;

            BrilloQuery.Purchase Purchase = new BrilloQuery.Purchase();
            List<BrilloQuery.ReturnInvoice_Integration> ReturnList = Purchase.Get_ReturnInvoice_Integration(purchase_return.id_purchase_return);

            foreach (BrilloQuery.ReturnInvoice_Integration item in ReturnList)
            {
                if (item.InvoiceID > 0)
                {
                   //Purchase Invoice Integrated.
                    purchase_invoice purchase_invoice = db.purchase_invoice.Find(item.InvoiceID);
                    decimal Return_GrandTotal_ByInvoice = ReturnList.Where(x => x.InvoiceID == item.InvoiceID).Sum(x => x.SubTotalVAT);

                    foreach (payment_schedual payment_schedual in purchase_invoice.payment_schedual.Where(x => x.AccountPayableBalance > 0))
                    {
                        if (payment_schedual.AccountPayableBalance > 0 && Return_GrandTotal_ByInvoice > 0)
                        {
                            decimal PaymentValue = payment_schedual.AccountPayableBalance < Return_GrandTotal_ByInvoice ? payment_schedual.AccountPayableBalance : Return_GrandTotal_ByInvoice;
                            Return_GrandTotal_ByInvoice -= PaymentValue;

                            payment_schedual Schedual = new payment_schedual()
                            {
                                debit = PaymentValue,
                                credit = 0,
                                id_currencyfx = purchase_return.id_currencyfx,
                                purchase_return = purchase_return,
                                trans_date = purchase_return.trans_date,
                                expire_date = purchase_return.trans_date,
                                status = Status.Documents_General.Approved,
                                id_contact = purchase_return.id_contact,
                                can_calculate = true,
                                parent = payment_schedual //base.payment_schedual.Where(x => x.id_purchase_return == purchase_return.id_purchase_return).FirstOrDefault();
                            };

                            payment_detail payment_detail = new payment_detail()
                            {
                                id_currencyfx = purchase_return.id_currencyfx,
                                id_purchase_return = purchase_return.id_purchase_return,
                                payment_type = payment_type != null ? payment_type : Fix_PaymentType(),
                                value = PaymentValue
                            };

                            payment_detail.payment_schedual.Add(Schedual);

                            payment.payment_detail.Add(payment_detail);
                        }
                    }
                }
            }

            if (payment.payment_detail.Count() > 0)
            {
                db.payments.Add(payment);
            }
        }

        private payment_type Fix_PaymentType()
        {
            //In case Payment type doesn not exist, this will create it and try to fix the error.
            payment_type payment_type = new payment_type()
            {
                payment_behavior = entity.payment_type.payment_behaviours.CreditNote,
                name = LocExtension.GetLocalizedValue<string>("Cognitivo:local:PurchaseReturn")
            };

            db.payment_type.Add(payment_type);

            return payment_type;
        }
        #endregion

        #region Annul

        public bool Annull()
        {
            NumberOfRecords = 0;
            foreach (purchase_return purchase_return in db.purchase_return.Local.Where(x => x.IsSelected && x.Error == null && x.status == Status.Documents_General.Approved))
            {
                List<payment_schedual> payment_schedualList = new List<payment_schedual>();

                //Clean the Payment Schedual. If Return has benn used, this will clean it from existance.
                if (purchase_return.payment_schedual != null && purchase_return.payment_schedual.Count > 0)
                {
                    foreach (payment_schedual payment_schedual in purchase_return.payment_schedual)
                    {
                        if (payment_schedual.payment_detail != null)
                        {
                            //Remove Payment Detail from history.
                            db.payment_detail.Remove(payment_schedual.payment_detail);
                        }
                    }
                    //Remove Schedual from history.
                    db.payment_schedual.RemoveRange(purchase_return.payment_schedual);
                }

                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                List<item_movement> item_movementList = new List<item_movement>();
                item_movementList = _Stock.revert_Stock(db, App.Names.PurchaseReturn, purchase_return);

                if (item_movementList != null && item_movementList.Count > 0)
                {
                    db.item_movement.RemoveRange(item_movementList);
                }

                //Finalize
                purchase_return.status = Status.Documents_General.Annulled;
                db.SaveChanges();
            }
            return true;
        }

        #endregion
    }
}
