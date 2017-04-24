using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Controller.Sales
{
    public class SalesInvoice
    {
        public int NumberOfRecords;
        public db db { get; set; }

        public DateTime Start_Range
        {
            get { return _start_Range; }
            set
            {
                if (_start_Range != value)
                {
                    _start_Range = value;
                }
            }
        }
        private DateTime _start_Range = DateTime.Now.AddDays(-7);

        public DateTime End_Range
        {
            get { return _end_Range; }
            set
            {
                if (_end_Range != value)
                {
                    _end_Range = value;
                }
            }
        }
        private DateTime _end_Range = DateTime.Now.AddDays(+1);

        public enum Messages
        {
            None,
            CreditLimit_Exceeded,
            DocumentRange_Finished
        }

        public List<Messages> Msg { get; set; }

        public SalesInvoice()
        {

        }

        public async void Load(bool FilterByTerminal)
        {
            var predicate = PredicateBuilder.True<sales_invoice>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.is_head == true);
            predicate = predicate.And(x => x.is_archived == false);
            predicate = predicate.And(x => x.id_branch == CurrentSession.Id_Branch);

            //If FilterByTerminal is true, then will add aditional Where into query.
            if (FilterByTerminal)
            {
                predicate = predicate.And(x => x.id_branch == CurrentSession.Id_Terminal);
            }

            if (Start_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date >= Start_Range.Date);
            }

            if (End_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date <= End_Range.Date);
            }

            await db.sales_invoice.Where(predicate)
                    .OrderByDescending(x => x.trans_date)
                    .ThenBy(x => x.number)
                    .LoadAsync();
        }

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
                app_branch = db.app_branch.Find(CurrentSession.Id_Branch)
            };

            //This is to skip query code in case of Migration. Helps speed up migrations.
            if (IsMigration == false)
            {
                sales_invoice.app_document_range = Brillo.Logic.Range.List_Range(db, App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
                sales_invoice.id_condition = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_condition).FirstOrDefault();
                sales_invoice.id_contract = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_contract).FirstOrDefault();
            }

            return sales_invoice;
        }

        public sales_invoice_detail Create_Detail(
            ref sales_invoice Invoice, 
            item Item,
            item_movement ItemMovement,
            bool AllowDouble,
            decimal QuantityInStock, 
            decimal Quantity)
        {
            int? i = null;

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
                movement_id = ItemMovement != null ? (int)ItemMovement.id_movement : i
            };

            int VatGroupID = (int)sales_invoice_detail.id_vat_group;
            sales_invoice_detail.app_vat_group = db.app_vat_group.Find(VatGroupID);

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

            return sales_invoice_detail;
        }


        #endregion

        #region Save

        public int SaveChanges_and_Validate()
        {
            NumberOfRecords = 0;
            foreach (sales_invoice invoice in db.sales_invoice.Local.Where(x => x.IsSelected && x.id_contact > 0))
            {
                if (invoice.Error == null)
                {
                    if (invoice.State == EntityState.Added)
                    {
                        invoice.timestamp = DateTime.Now;
                        invoice.State = EntityState.Unchanged;
                        db.Entry(invoice).State = EntityState.Added;
                        Add_CRM(invoice);
                    }
                    else if (invoice.State == EntityState.Modified)
                    {
                        invoice.timestamp = DateTime.Now;
                        invoice.State = EntityState.Unchanged;
                        db.Entry(invoice).State = EntityState.Modified;
                    }
                    else if (invoice.State == EntityState.Deleted)
                    {
                        invoice.timestamp = DateTime.Now;
                        invoice.is_head = false;
                        invoice.State = EntityState.Deleted;
                        db.Entry(invoice).State = EntityState.Modified;
                    }
                    NumberOfRecords += 1;
                }
                if (invoice.State > 0)
                {
                    if (invoice.State != EntityState.Unchanged)
                    {
                        db.Entry(invoice).State = EntityState.Unchanged;
                    }
                }
            }

            return db.SaveChanges();
        }

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

        public bool CancelAllChanges()
        {
            return false;
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
                                                x.status != Status.Documents_General.Approved
                                                        && x.IsSelected && x.Error == null).ToList();
            foreach (sales_invoice invoice in SalesInvoiceList)
            {
                SpiltInvoice(invoice);
            }

            foreach (sales_invoice invoice in db.sales_invoice.Local.Where(x =>
                                                x.status != Status.Documents_General.Approved && x.is_head
                                                        && x.IsSelected && x.Error == null && x.id_contact > 0))
            {
                if (invoice.id_sales_invoice == 0 && invoice.id_contact > 0)
                {
                    db.SaveChanges();
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
                    }
                    else
                    {
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
                    //Credit Not Approved Message.
                    Msg.Add(Messages.CreditLimit_Exceeded);
                }

                ApprovalStatus = true;
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
            item_movementList = _Stock.SalesInvoice_Approve(db, invoice);

            if (item_movementList.Count() > 0)
            {
                db.item_movement.AddRange(item_movementList);

                foreach (sales_invoice_detail sales_detail in invoice.sales_invoice_detail.Where(x => x.item.item_product.Count() > 0))
                {
                    if (sales_detail.item_movement.FirstOrDefault() != null)
                    {
                        if (sales_detail.item_movement.FirstOrDefault().item_movement_value != null)
                        {
                            sales_detail.unit_cost = Brillo.Currency.convert_Values(sales_detail.item_movement.FirstOrDefault().item_movement_value.Sum(x => x.unit_value),
                            sales_detail.item_movement.FirstOrDefault().item_movement_value.FirstOrDefault().id_currencyfx,
                            sales_detail.sales_invoice.id_currencyfx, App.Modules.Sales);
                        }
                    }
                }
            }
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
                    //Loop through the Payment Schedual. And remove payments made.
                    foreach (payment_schedual payment_schedual in Invoice.payment_schedual)
                    {
                        if (payment_schedual.Action == payment_schedual.Actions.Delete)
                        {
                            Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                            _Payment.DeletePaymentSchedual(db, payment_schedual.id_payment_schedual);
                        }
                    }

                    ///Since the above Foreach will run through a mix of payment scheduals, we have no way of knowing if we will have
                    ///payment headers. So we run this code to clean.
                    List<payment> EmptyPayments = db.payments.Where(x => x.payment_detail.Count() == 0).ToList();
                    if (EmptyPayments.Count() > 0)
                    {
                        db.payments.RemoveRange(EmptyPayments);
                    }

                    foreach (sales_invoice_detail detail in Invoice.sales_invoice_detail)
                    {
                        foreach (item_movement item_movement in detail.item_movement)
                        {
                            if (item_movement.Action == item_movement.Actions.Delete)
                            {
                                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                                db.item_movement.Remove(item_movement);
                            }
                            else if (item_movement.Action == item_movement.Actions.ReApprove)
                            {
                                foreach (var item in item_movement.child)
                                {
                                    List<item_movement> item_movementList = db.item_movement.Where(x =>
                                    x.id_item_product == item_movement.id_item_product &&
                                    x.id_movement != item_movement.id_movement &&
                                    x.credit > 0).ToList();
                                    foreach (item_movement _item_movement in item_movementList)
                                    {
                                        if (_item_movement.avlquantity > item.credit)
                                        {
                                            item.parent = _item_movement;
                                        }
                                        else
                                        {
                                            item.parent = null;
                                        }
                                    }
                                }
                                db.item_movement.Remove(item_movement);
                            }
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
    }
}
