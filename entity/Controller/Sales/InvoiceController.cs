using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MoreLinq;
using entity.BrilloQuery;
using System.Data;

namespace entity.Controller.Sales
{
    public class InvoiceController : Base, IDisposable
    {
        public void Dispose()
        {
            // Dispose(true);
            GC.SuppressFinalize(this);
        }
        public InvoiceController()
        {
            LoadPromotion();
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

        public Brillo.Promotion.Start Promotions { get; set; }

        #region Properties

        public enum Messages
        {
            None,
            CreditLimit_Exceeded,
            DocumentRange_Finished,
            StockExceed
        }

        public List<Messages> Msg { get; set; }

        public int Count { get; set; }

        public int PageSize { get { return _PageSize; } set { _PageSize = value; } }
        public int _PageSize = 100;


        public int PageCount
        {
            get
            {
                return (Count / PageSize) < 1 ? 1 : (Count / PageSize);
            }
        }

        #endregion

        #region Load

        public void LoadPromotion()
        {
            ///Initialize Promotions List. Inside is a Boolean value to Load or not. 
            ///This will help when trying to load Controller remotely without UI
            Promotions = new Brillo.Promotion.Start(true);
        }

        public async void Load(bool FilterByTerminal, int PageIndex)
        {
            var predicate = PredicateBuilder.True<sales_invoice>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.is_head == true);
            predicate = predicate.And(x => x.is_archived == false);
            predicate = predicate.And(x => x.id_branch == CurrentSession.Id_Branch);

            //If FilterByTerminal is true, then will add aditional Where into query.
            if (FilterByTerminal)
            {
                predicate = predicate.And(x => x.id_terminal == CurrentSession.Id_Terminal);
            }

            if (Count == 0)
            {
                Count = db.sales_invoice.Where(predicate).Count();
            }

            await db.sales_invoice.Where(predicate)
                    .OrderByDescending(x => x.trans_date)
                    .ThenBy(x => x.number)
                    .Skip(PageIndex * PageSize).Take(PageSize)
                    .LoadAsync();

        }

        #endregion

        #region Create

        public sales_invoice Create(int TransDate_OffSet, bool IsMigration)
        {
            sales_invoice sales_invoice = new sales_invoice()
            {
                State = EntityState.Added,
                status = Status.Documents_General.Pending,
                IsSelected = true,
                trans_type = Status.TransactionTypes.Normal,
                trans_date = DateTime.Now.AddDays(TransDate_OffSet),
                timestamp = DateTime.Now,

                //Navigation Properties
                app_currencyfx = db.app_currencyfx.Find(CurrentSession.Get_Currency_Default_Rate().id_currencyfx),
                app_branch = db.app_branch.Find(CurrentSession.Id_Branch),
                app_terminal = db.app_terminal.Find(CurrentSession.Id_Terminal)
            };

            //This is to skip query code in case of Migration. Helps speed up migrations.
            if (IsMigration == false)
            {
                sales_invoice.app_document_range = Brillo.Logic.Range.List_Range(db, App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
                if (CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault() != null)
                {
                    sales_invoice.id_condition = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_condition;
                }
                else
                {
                    sales_invoice.id_condition = CurrentSession.Contracts.FirstOrDefault().id_condition;
                }
                if (CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault() != null)
                {
                    sales_invoice.id_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_contract;
                }
                else
                {
                    sales_invoice.id_contract = CurrentSession.Contracts.FirstOrDefault().id_contract;
                }

            }

            return sales_invoice;
        }

        public sales_invoice_detail Create_Detail(
            ref sales_invoice Invoice, item Item, item_movement ItemMovement,
            bool AllowDouble,
            decimal? QuantityInStock,
            decimal Quantity)
        {
            int? i = null;
            int? location = null;

            sales_invoice_detail sales_invoice_detail = new sales_invoice_detail()
            {
                State = EntityState.Added,
                CurrencyFX_ID = Invoice.id_currencyfx,
                Contact = Invoice.contact,
                item_description = Item.name,
                item = Item,
                id_item = Item.id_item,
                Quantity_InStock = QuantityInStock,
                quantity = Quantity,
                batch_code = ItemMovement != null ? ItemMovement.code : "",
                expire_date = ItemMovement != null ? ItemMovement.expire_date : null,
                movement_id = ItemMovement != null ? (int)ItemMovement.id_movement : i,
                id_location = ItemMovement != null ? ItemMovement.id_location : location,

            };


            int VatGroupID = (int)sales_invoice_detail.id_vat_group;
            //  sales_invoice_detail.app_vat_group = db.app_vat_group.Find(VatGroupID);

            if (Invoice.app_contract == null && Invoice.id_contract > 0)
            {
                Invoice.app_contract = db.app_contract.Find(Invoice.id_contract);
            }

            if (Invoice.app_contract != null)
            {
                if (Invoice.app_contract.surcharge != null)
                {
                    decimal surcharge = (decimal)Invoice.app_contract.surcharge;
                    sales_invoice_detail.unit_price = sales_invoice_detail.unit_price * (1 + surcharge);
                }
            }

            Invoice.sales_invoice_detail.Add(sales_invoice_detail);

            //Check for Promotions after each insert.
            Check_Promotions(Invoice);

            return sales_invoice_detail;
        }

        public void Edit(sales_invoice Invoice)
        {
            if (Invoice != null)
            {
                Invoice.IsSelected = true;
                Invoice.State = EntityState.Modified;
                db.Entry(Invoice).State = EntityState.Modified;
            }
        }

        public void Archive()
        {
            foreach (sales_invoice Invoice in db.sales_invoice.Local.Where(x => x.IsSelected))
            {
                Invoice.is_archived = true;
            }

            db.SaveChanges();
        }

        #endregion

        #region Save

        private void Add_CRM(sales_invoice invoice)
        {
            if (invoice.id_sales_order == 0 || invoice.id_sales_order == null)
            {
                crm_opportunity crm_opportunity = new crm_opportunity()
                {
                    id_contact = invoice.id_contact,
                    id_currency = invoice.id_currencyfx,
                    value = invoice.GrandTotal
                };

                crm_opportunity.sales_invoice.Add(invoice);
                db.crm_opportunity.Add(crm_opportunity);
            }
            else
            {
                crm_opportunity crm_opportunity = db.sales_order.Find(invoice.id_sales_order).crm_opportunity;
                crm_opportunity.sales_invoice.Add(invoice);
                db.crm_opportunity.Attach(crm_opportunity);
            }
        }

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (sales_invoice sales_invoice in db.sales_invoice.Local.Where(x => x.IsSelected && x.id_contact > 0))
            {
                int count = sales_invoice.sales_invoice_detail.Where(x => x.Error != null).Count();

                if (sales_invoice.IsSelected && sales_invoice.Error == null && count == 0)
                {
                    if (sales_invoice.State == EntityState.Added)
                    {
                        sales_invoice.timestamp = DateTime.Now;
                        sales_invoice.State = EntityState.Unchanged;
                        db.Entry(sales_invoice).State = EntityState.Added;
                        sales_invoice.IsSelected = false;
                        Add_CRM(sales_invoice);
                    }
                    else if (sales_invoice.State == EntityState.Modified)
                    {
                        sales_invoice.timestamp = DateTime.Now;
                        sales_invoice.State = EntityState.Unchanged;
                        db.Entry(sales_invoice).State = EntityState.Modified;
                        sales_invoice.IsSelected = false;
                    }
                    NumberOfRecords += 1;
                }
                else
                {
                    return false;
                }

                if (sales_invoice.State != EntityState.Unchanged && sales_invoice.Error != null)
                {
                    if (sales_invoice.sales_invoice_detail.Count() > 0)
                    {
                        db.sales_invoice_detail.RemoveRange(sales_invoice.sales_invoice_detail);
                    }

                    if (sales_invoice.crm_opportunity != null)
                    {
                        db.crm_opportunity.Remove(sales_invoice.crm_opportunity);
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
                catch (Exception EX)
                {
                    throw EX;
                }

                return true;
            }

        }

        #endregion

        #region Approve

        /// <summary>
        /// Approves the Sales Invoice, discounting from Stock and creating a Payment Schedual.
        /// </summary>
        /// <returns></returns>
        public bool Approve()
        {
            bool ApprovalStatus = false;
            Msg = new List<Messages>();

            NumberOfRecords = 0;

            List<sales_invoice> SalesInvoiceList = db.sales_invoice.Local.Where(x =>
                                                x.status == Status.Documents_General.Pending &&
                                                        x.IsSelected &&
                                                        x.Error == null &&
                                                        x.id_contact > 0)
                                                        .ToList();
            foreach (sales_invoice invoice in SalesInvoiceList)
            {
                SpiltInvoice(invoice);
            }

            foreach (sales_invoice invoice in SalesInvoiceList)
            {
                if (invoice.id_sales_invoice == 0 && invoice.id_contact > 0)
                {
                    if (SaveChanges_WithValidation() == false)
                    {
                        return false;
                    }
                }

                Check_Promotions(invoice);

                List<StockList> ListofStock = new List<StockList>();

                //Get List and Refresh Data in Session for future use.
                ListofStock = CurrentItems.getProducts_InStock(invoice.id_branch, DateTime.Now, true).Where(x => x.BranchID == invoice.id_branch).ToList();

                foreach (sales_invoice_detail sales_invoice_detail in invoice.sales_invoice_detail)
                {
                    if (sales_invoice_detail.sales_packing_relation.Count() == 0)
                    {
                        item.item_type CurrentItemType = sales_invoice_detail.item.id_item_type;
                        if (CurrentItemType == item.item_type.Product || CurrentItemType == item.item_type.RawMaterial || CurrentItemType == item.item_type.Supplies)
                        {
                            decimal Quantity_InStock = 0;

                            if (sales_invoice_detail.id_location != null)
                            {
                                Quantity_InStock = (decimal)ListofStock
                                    .Where(x => x.LocationID == sales_invoice_detail.id_location && x.ItemID == sales_invoice_detail.id_item)
                                    .Sum(x => x.Quantity);
                            }
                            else
                            {
                                Quantity_InStock = (decimal)ListofStock
                                    .Where(x => x.ItemID == sales_invoice_detail.id_item)
                                    .Sum(x => x.Quantity);
                            }

                            if (Quantity_InStock < sales_invoice_detail.quantity)
                            {
                                Msg.Add(Messages.StockExceed);
                                return false;
                            }
                        }
                    }
                }

                invoice.app_condition = db.app_condition.Find(invoice.id_condition);
                invoice.app_contract = db.app_contract.Find(invoice.id_contract);
                invoice.app_currencyfx = db.app_currencyfx.Find(invoice.id_currencyfx);

                Finance.Credit Credit = new Finance.Credit();

                if (Credit.CheckLimit_InSales(
                    invoice.GrandTotal,
                    invoice.app_currencyfx,
                    invoice.contact,
                    invoice.app_contract))
                {
                    //Logic
                    List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                    Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                    payment_schedualList = _Payment.insert_Schedual(invoice);

                    //Save Promisory Note first, because it is referenced in Payment Schedual
                    if (_Payment.payment_promissory_noteLIST != null && _Payment.payment_promissory_noteLIST.Count > 0)
                    {
                        db.payment_promissory_note.AddRange(_Payment.payment_promissory_noteLIST);
                    }

                    //Payment Schedual
                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        db.payment_schedual.AddRange(payment_schedualList);
                    }

                    //Item Movement
                    Insert_Items_2_Movement(invoice);

                    if ((invoice.number == null || invoice.number == string.Empty) && invoice.id_range > 0)
                    {
                        if (invoice.id_branch > 0)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == invoice.id_branch).FirstOrDefault().code;
                        }
                        if (invoice.id_terminal > 0)
                        {
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == invoice.id_terminal).FirstOrDefault().code;
                        }

                        app_document_range app_document_range = db.app_document_range.Where(x => x.id_range == invoice.id_range).FirstOrDefault();

                        if (app_document_range != null)
                        {
                            invoice.code = app_document_range.code;
                        }

                        invoice.is_issued = true;
                        invoice.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                        invoice.RaisePropertyChanged("number");
                        invoice.status = Status.Documents_General.Approved;
                        invoice.timestamp = DateTime.Now;

                        //Save Changes before Printing, so that all fields show up.
                        db.SaveChanges();

                        Brillo.Document.Start.Automatic(invoice, app_document_range);
                        ApprovalStatus = true;
                    }
                    else
                    {
                        ApprovalStatus = true;
                        invoice.is_issued = false;

                        invoice.status = Status.Documents_General.Approved;
                        invoice.timestamp = DateTime.Now;

                        db.SaveChanges();
                    }

                    NumberOfRecords += 1;
                    invoice.IsSelected = false;
                }
                else
                {
                    ApprovalStatus = false;
                    //Credit Not Approved Message.
                    Msg.Add(Messages.CreditLimit_Exceeded);
                }


            }

            return ApprovalStatus;
        }

        /// <summary>
        /// Split Invoice from the particular invoice
        /// </summary>
        /// <param name="invoice">object for the sales invoice</param>
        public void SpiltInvoice(sales_invoice invoice)
        {
            if ((invoice.number == null || invoice.number == string.Empty) && invoice.app_document_range != null)
            {
                int document_line_limit = 0;

                if (invoice.app_document_range.app_document.line_limit != null)
                {
                    document_line_limit = (int)invoice.app_document_range.app_document.line_limit;
                }

                if (document_line_limit > 0 && invoice.sales_invoice_detail.Count > document_line_limit)
                {
                    int NoOfInvoice = (int)Math.Ceiling(invoice.sales_invoice_detail.Count / (decimal)document_line_limit);

                    //Counter Variable for not loosing place in Detail
                    int position = 0;

                    for (int i = 1; i <= NoOfInvoice; i++)
                    {
                        sales_invoice _invoice = new sales_invoice()
                        {
                            code = invoice.code,
                            comment = invoice.comment,
                            app_branch = invoice.app_branch,
                            id_branch = invoice.id_branch,
                            app_company = invoice.app_company,
                            id_company = invoice.id_company,
                            app_condition = invoice.app_condition,
                            id_condition = invoice.id_condition,
                            contact = invoice.contact,
                            id_contact = invoice.id_contact,
                            app_contract = invoice.app_contract,
                            id_contract = invoice.id_contract,
                            app_currencyfx = invoice.app_currencyfx,
                            id_currencyfx = invoice.id_currencyfx,
                            id_opportunity = invoice.id_opportunity,
                            project = invoice.project,
                            id_project = invoice.id_project,
                            app_document_range = invoice.app_document_range,
                            id_range = invoice.id_range,
                            sales_order = invoice.sales_order,
                            id_sales_order = invoice.id_sales_order,
                            sales_rep = invoice.sales_rep,
                            id_sales_rep = invoice.id_sales_rep,
                            app_terminal = invoice.app_terminal,
                            id_terminal = invoice.id_terminal,
                            security_user = invoice.security_user,
                            id_user = invoice.id_user,
                            id_weather = invoice.id_weather,
                            number = invoice.number,
                            GrandTotal = invoice.GrandTotal,
                            is_head = invoice.is_head,
                            is_issued = invoice.is_issued,
                            IsSelected = invoice.IsSelected,
                            is_impex = invoice.is_impex,
                            State = EntityState.Added,
                            status = Status.Documents_General.Pending,
                            trans_date = invoice.trans_date
                        };

                        //Loop through the details. Skipping to page the sales invoices into smaller invoices.
                        foreach (sales_invoice_detail detail in invoice.sales_invoice_detail.Skip(position).Take(document_line_limit))
                        {
                            sales_invoice_detail sales_invoice_detail = new sales_invoice_detail()
                            {
                                item_description = detail.item_description,
                                discount = detail.discount,
                                id_company = detail.id_company,
                                item = detail.item,
                                id_item = detail.id_item,
                                id_location = detail.id_location,
                                id_project_task = detail.id_project_task,
                                id_sales_order_detail = detail.id_sales_order_detail,
                                id_vat_group = detail.id_vat_group,
                                is_head = detail.is_head,
                                IsSelected = detail.IsSelected,
                                quantity = detail.quantity,
                                State = EntityState.Added,
                                SubTotal = detail.SubTotal,
                                SubTotal_Vat = detail.SubTotal_Vat,
                                unit_cost = detail.unit_cost,
                                unit_price = detail.unit_price,
                                UnitPrice_Vat = detail.UnitPrice_Vat
                            };

                            _invoice.sales_invoice_detail.Add(sales_invoice_detail);
                            position += 1;
                        }
                        db.sales_invoice.Add(_invoice);
                    }

                    invoice.is_head = false;
                    invoice.status = Status.Documents_General.Approved;

                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Executes code that will insert Invoiced Items into Movement.
        /// </summary>
        /// <param name="invoice"></param>
        public void Insert_Items_2_Movement(sales_invoice invoice)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
            item_movementList = _Stock.SalesInvoice_Approve(db, ref invoice);

            if (item_movementList.Count() > 0)
            {
                db.item_movement.AddRange(item_movementList);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Loop through each Item Movement and assign cost to detail for reporting purposes.
            foreach (sales_invoice_detail sales_detail in invoice.sales_invoice_detail)
            {
                string Cost = @"
                                set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                select sum(im.debit * c.unit_value / sid.quantity) as Cost from item_movement_value_detail as c
                                join item_movement_value_rel as rel on c.id_movement_value_rel = rel.id_movement_value_rel
                                join item_movement as im on rel.id_movement_value_rel = im.id_movement_value_rel
                                join sales_invoice_detail as sid on im.id_sales_invoice_detail = sid.id_sales_invoice_detail
                                where sid.id_sales_invoice_detail = " + sales_detail.id_sales_invoice_detail;

                DataTable dt = QueryExecutor.DT(Cost);
                if (dt.Rows.Count > 0)
                {
                    if (sales_detail.unit_cost == 0)
                    {
                        sales_detail.unit_cost = Brillo.Currency.convert_Values(Convert.ToDecimal(dt.Rows[0]["Cost"] is DBNull ? 0 : dt.Rows[0]["Cost"]),
                                 CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                              sales_detail.sales_invoice.id_currencyfx,
                              entity.App.Modules.Sales
                              );
                    }

                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void ReApprove_Click(sales_invoice invoice)
        {
            //if (invoice != null)
            //{
            //    //Finance
            //    CheckPaymentReApprove CheckPaymentReApprove = new CheckPaymentReApprove();

            //    string Message = CheckPaymentReApprove.Check_ContractChanges(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    if (Message != "")
            //    {
            //        Message += "\n" + "Are You Sure Want To Change The Data..";
            //        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //        {
            //            new UpdatePaymentReApprove().Update_ContractChanges(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //        }
            //    }

            //    Message = CheckPaymentReApprove.Check_ValueUP(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    if (Message != "")
            //    {
            //        Message += "\n" + "Are You Sure Want To Change The Data..";
            //        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //        {
            //            new UpdatePaymentReApprove().Update_ValueUP(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //        }
            //    }

            //    Message = CheckPaymentReApprove.Check_ValueDown(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    if (Message != "")
            //    {
            //        Message += "\n" + "Are You Sure Want To Change The Data..";
            //        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //        {
            //            new UpdatePaymentReApprove().Update_ValueDown(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //        }
            //    }

            //    Message += CheckPaymentReApprove.Check_CurrencyChange(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    if (Message != "")
            //    {
            //        Message += "\n" + "Are You Sure Want To Change The Data..";
            //        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //        {
            //            new UpdatePaymentReApprove().Update_CurrencyChange(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //        }
            //    }

            //    Message = CheckPaymentReApprove.Check_DateChange(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    if (Message != "")
            //    {
            //        Message += "\n" + "Are You Sure Want To Change The Data..";
            //        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //        {
            //            new UpdatePaymentReApprove().Update_DateChange(db, invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //        }
            //    }

            //    //Movement
            //    Approve_Check CheckMovementReApprove = new Approve_Check();

            //    //Check for Quantity Up
            //    if (CheckMovementReApprove.QuantityUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //    {
            //        new UpdateMovementReApprove().QuantityUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    }

            //    //Check for Quantity Down
            //    if (CheckMovementReApprove.QuantityDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //    {
            //        new UpdateMovementReApprove().QuantityDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    }

            //    //Checks for Date Changes
            //    if (CheckMovementReApprove.DateChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //    {
            //        new UpdateMovementReApprove().DateChange(SalesDB.db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    }

            //    //Checks for New Detail Insertions
            //    if (CheckMovementReApprove.CreateDetail(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //    {
            //        new UpdateMovementReApprove().NewMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    }

            //    //Check if Item has been Removed
            //    if (CheckMovementReApprove.RemovedDetail(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice))
            //    {
            //        new UpdateMovementReApprove().DeleteMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
            //    }
            //}
            ////SalesInvoiceDB.ReApprove(sales_invoice);
            //sales_invoiceViewSource.View.Refresh();
            //SalesDB.db.SaveChanges();
        }

        #endregion

        #region Annul

        public void Annull()
        {
            ///Only run through Invoices that have been approved.
            foreach (sales_invoice Invoice in db.sales_invoice.Local
                .Where(x => x.IsSelected && x.status == Status.Documents_General.Approved))
            {
                //Block any annull if user is not Master.
                if (Invoice.is_accounted == false || CurrentSession.UserRole.is_master)
                {
                    List<payment_schedual> payment_schedualList = Invoice.payment_schedual.ToList();
                    //Loop through the Payment Schedual. And remove payments made.
                    foreach (payment_schedual payment_schedual in payment_schedualList)
                    {
                        //if (payment_schedual.Action == payment_schedual.Actions.Delete)
                        //{
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        _Payment.DeletePaymentSchedual(db, payment_schedual.id_payment_schedual);
                        //}
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

                    foreach (sales_invoice_detail detail in Invoice.sales_invoice_detail)
                    {
                        List<item_movement> ItemMovementList = detail.item_movement.ToList();
                        foreach (item_movement item_movement in ItemMovementList)
                        {
                            if (item_movement.id_sales_packing_detail != null)
                            {
                                //Remove ref of sales. keep packing reference. this will allow you to reuse packing ref.
                                item_movement.id_sales_invoice_detail = null;
                                //delete relationship between detail and packing.
                                db.sales_packing_relation.RemoveRange(detail.sales_packing_relation);
                            }
                            else
                            {
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
                            //        x.credit > 0).ToList();
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

        #region Integrations

        /// <summary>
        /// Links a Sales Packing and brings items into Sales Invoice.
        /// </summary>
        /// <param name="Invoice">Sales Invoice</param>
        /// <param name="PackingID">Sales Packing ID</param>
        /// <returns>True if Correct. False if Error or Unfinsihed</returns>
        public bool Link_PackingList(sales_invoice Invoice, int PackingID)
        {
            //Bring into Context.
            sales_packing Packing = db.sales_packing.Find(PackingID);
          
            //Group Packing Detail so as to reduce the amount of lines in 
            //Inoivces if the Product, Batch Code, Expiration Date are the same.
            List<sales_packing_detail> GroupDetail = Packing.sales_packing_detail
                .AsEnumerable()
                .Where(x => x.user_verified == true)
                .GroupBy(g => new { g.id_item, g.batch_code, g.expire_date, g.id_sales_order_detail })
                .SelectMany(a => a)
                .ToList();

            foreach (sales_packing_detail _sales_packing_detail in GroupDetail)
            {
                sales_order_detail sales_order_detail = _sales_packing_detail.sales_order_detail;

                sales_invoice_detail Detail = new sales_invoice_detail()
                {
                    id_location = _sales_packing_detail.id_location
                };

                sales_packing_relation sales_packing_relation = new sales_packing_relation()
                {
                    id_sales_packing_detail = _sales_packing_detail.id_sales_packing_detail,
                    sales_packing_detail = _sales_packing_detail,
                    sales_invoice_detail = Detail
                };

                Detail.sales_packing_relation.Add(sales_packing_relation);

                if (_sales_packing_detail.expire_date != null || !string.IsNullOrEmpty(_sales_packing_detail.batch_code))
                {
                    Detail.expire_date = _sales_packing_detail.expire_date;
                    Detail.batch_code = _sales_packing_detail.batch_code;
                }

                //if SalesOrder Exists, use it for Price and VAT.
                if (sales_order_detail != null)
                {
                    Detail.sales_invoice = Invoice;
                    Detail.Contact = db.contacts.Find(Invoice.id_contact);// sbxContact.Contact;
                    Detail.item = _sales_packing_detail.item;
                    Detail.id_item = _sales_packing_detail.id_item;
                    Detail.quantity = Convert.ToDecimal(_sales_packing_detail.verified_quantity);
                    Detail.id_vat_group = sales_order_detail.id_vat_group;
                    Detail.State = EntityState.Added;
                    Detail.unit_price = sales_order_detail.unit_price + sales_order_detail.discount;
                    Detail.discount = sales_order_detail.discount;
                    Invoice.id_sales_rep = sales_order_detail.sales_order.id_sales_rep;
                    Invoice.app_contract = sales_order_detail.sales_order.app_contract;
                    Invoice.RaisePropertyChanged("app_contract");
                    Invoice.RaisePropertyChanged("id_contract");
                    Invoice.id_condition = sales_order_detail.sales_order.id_condition;
                    Invoice.sales_invoice_detail.Add(Detail);
                }
                else
                {
                    //If Sales Order does not exist, use Price and VAT From standard of the company.
                    Create_Detail(ref Invoice,
                        _sales_packing_detail.item,
                        null,
                        false,
                        0,
                        (decimal)_sales_packing_detail.verified_quantity);
                }
            }

            return true;
        }

        public bool Link_Order(sales_invoice Invoice, int OrderID)
        {

            return true;
        }
        #endregion

        #region Promotions

        public async void Check_Promotions(sales_invoice Invoice)
        {
            if (Invoice != null)
            {
                //Cleanup Code
                if (Invoice.sales_invoice_detail.Where(x => x.IsPromo).ToList().Count() > 0)
                {
                    foreach (sales_invoice_detail sales_invoice_detail in Invoice.sales_invoice_detail.Where(x => x.IsPromo).ToList())
                    {
                        if (sales_invoice_detail.id_sales_invoice_detail != sales_invoice_detail.PromoID)
                        {
                            db.sales_invoice_detail.Remove(sales_invoice_detail);
                        }
                    }
                }

                ///Promotions Code
                Promotions.Calculate_SalesInvoice(ref Invoice);
                Invoice.RaisePropertyChanged("GrandTotal");

                //Fixup Code.
                foreach (sales_invoice_detail sales_invoice_detail in Invoice.sales_invoice_detail)
                {
                    //Gets the Item into Context.
                    if (sales_invoice_detail.item == null)
                    {
                        sales_invoice_detail.item = await db.items.FindAsync(sales_invoice_detail.id_item);
                    }

                    //Gets the Promotion into Context.
                    if (sales_invoice_detail.id_sales_promotion > 0 && sales_invoice_detail.sales_promotion == null)
                    {
                        sales_invoice_detail.sales_promotion = await db.sales_promotion.FindAsync(sales_invoice_detail.id_sales_promotion);
                    }
                }
            }
        }

        #endregion
    }
}
