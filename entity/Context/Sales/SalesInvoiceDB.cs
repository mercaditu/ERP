using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class SalesInvoiceDB : BaseDB
    {
        public sales_invoice New(int TransDate_OffSet, bool IsMigration)
        {
            sales_invoice sales_invoice = new sales_invoice();
            sales_invoice.State = EntityState.Added;
            sales_invoice.status = Status.Documents_General.Pending;
            sales_invoice.IsSelected = true;
            sales_invoice.trans_type = Status.TransactionTypes.Normal;
            sales_invoice.trans_date = DateTime.Now.AddDays(TransDate_OffSet);
            sales_invoice.timestamp = DateTime.Now;

            //Navigation Properties
            sales_invoice.app_currencyfx = app_currencyfx.Find(CurrentSession.Get_Currency_Default_Rate().id_currencyfx);
            sales_invoice.app_branch = app_branch.Find(CurrentSession.Id_Branch);


            //This is to skip query code in case of Migration. Helps speed up migrations.
            if (IsMigration == false)
            {
                sales_invoice.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
                sales_invoice.id_condition = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_condition;
                sales_invoice.id_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault().id_contract;
            }

            return sales_invoice;
        }

        public override int SaveChanges()
        {
            validate_Invoice();
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Invoice();
            return SaveChangesAsync();
        }

        private void validate_Invoice()
        {
            NumberOfRecords = 0;

            foreach (sales_invoice invoice in sales_invoice.Local.Where(x => x.IsSelected))
            {
                if (invoice.Error == null)
                {
                    if (invoice.State == EntityState.Added)
                    {
                        invoice.timestamp = DateTime.Now;
                        invoice.State = EntityState.Unchanged;
                        Entry(invoice).State = EntityState.Added;
                        add_CRM(invoice);
                    }
                    else if (invoice.State == EntityState.Modified)
                    {
                        invoice.timestamp = DateTime.Now;
                        invoice.State = EntityState.Unchanged;
                        Entry(invoice).State = EntityState.Modified;
                    }
                    else if (invoice.State == EntityState.Deleted)
                    {
                        invoice.timestamp = DateTime.Now;
                        invoice.is_head = false;
                        invoice.State = EntityState.Deleted;
                        Entry(invoice).State = EntityState.Modified;
                    }
                    NumberOfRecords += 1;
                }
                if (invoice.State > 0)
                {
                    if (invoice.State != EntityState.Unchanged)
                    {
                        Entry(invoice).State = EntityState.Unchanged;
                    }
                }
            }
        }

        private sales_invoice new_Version(sales_invoice old_invoice)
        {
            sales_invoice sales_invoice = new sales_invoice();
            return sales_invoice;
        }
        public void ReApprove(sales_invoice invoice)
        {
            foreach (sales_invoice_detail detail in invoice.sales_invoice_detail.Where(x => x.item.item_product.Count() > 0))
            {
                //detail is added
                if (Entry(detail).State == EntityState.Added)
                {
                    //add stock
                    MovementReApprove(invoice, detail);
                    //add payment
                    PaymentReapprove(invoice, detail, EntityState.Added);

                }
                if (detail.State == EntityState.Deleted)
                {

                    //delete stock
                    List<item_movement> ItemMovementList = detail.item_movement.Where(x => x.parent == null).ToList();
                    if (detail.item_movement.Count == ItemMovementList.Count)
                    {
                        base.item_movement.RemoveRange(detail.item_movement);

                    }
                    //delete payment is pending b'coz confused
                    PaymentReapprove(invoice, detail, EntityState.Added);
                   

                }
                if (Entry(detail).State == EntityState.Modified)
                {//stock modified as per requirement 
                    decimal quntity = detail.quantity;
                    // qunaity is more
                    if (detail.item_movement.Count > 0)
                    {

                        //set quantity without parent
                        if (detail.quantity > detail.item_movement.Sum(x => x.debit))
                        {
                            decimal QunatityDiffmov = detail.quantity - detail.item_movement.Sum(x => x.debit);
                            foreach (item_movement item_movement in detail.item_movement)
                            {
                                if (QunatityDiffmov > 0)
                                {
                                    if (item_movement.parent == null)
                                    {
                                        item_movement.debit = item_movement.debit + QunatityDiffmov;
                                        QunatityDiffmov = 0;
                                    }
                                }

                            }
                            //if qunatity not set then add new record
                            if (QunatityDiffmov > 0)
                            {
                                MovementReApprove(invoice, detail);

                            }
                        }
                        //less
                        else
                        {
                            decimal QunatityDiff = detail.item_movement.Sum(x => x.debit) - detail.quantity;
                            decimal QuantityUsed = detail.item_movement.Where(x => x.parent != null).Sum(x => x.debit);
                            if (QuantityUsed < QunatityDiff)
                            {//see with movemement without parent
                                foreach (item_movement item_movement in detail.item_movement.Where(x => x.parent == null))
                                {
                                    if (QunatityDiff > 0)
                                    {
                                        if (item_movement.debit != (QunatityDiff - QuantityUsed))
                                        {
                                            item_movement.debit = (QunatityDiff - QuantityUsed);
                                            QunatityDiff = 0;
                                        }
                                    }

                                    else
                                    {
                                        //remove extra without parent movement
                                        base.item_movement.Remove(item_movement);
                                    }

                                }
                            }

                        }





                    }
                    //paymnets
                    PaymentReapprove(invoice, detail, EntityState.Added);


                }


            }




            SaveChanges();
        }
        public void PaymentReapprove(sales_invoice invoice, sales_invoice_detail detail, EntityState status)
        {
            if (status == EntityState.Deleted)
            {
                List<payment_schedual> payment_schedualList = invoice.payment_schedual.Where(x => x.id_payment_detail == null || x.id_payment_detail == 0).ToList();
                if (invoice.payment_schedual.Count == payment_schedualList.Count)
                {
                    payment_schedual payment_schedual = payment_schedualList.FirstOrDefault();
                    if (payment_schedual != null)
                    {
                        payment_schedual.debit = payment_schedual.debit - detail.SubTotal_Vat;
                    }
                }
            }
            else
            {
                List<payment_schedual> payment_schedualList = base.payment_schedual.Where(x => x.id_sales_invoice == invoice.id_sales_invoice).ToList();
                List<payment_schedual> payment_schedualListNotUsed;

                if (payment_schedualList.Count() > 0)
                {
                    payment_schedualListNotUsed = payment_schedualList.Where(x => x.id_payment_detail == null || x.id_payment_detail == 0).ToList();
                    if (payment_schedualList.FirstOrDefault() != null)
                    {
                        //when currency is not changed
                        if (payment_schedualList.FirstOrDefault().id_currencyfx == invoice.id_currencyfx)
                        {

                            //more
                            if (invoice.GrandTotal > payment_schedualList.Sum(x => x.debit))
                            {
                                payment_schedual payment_schedual = payment_schedualListNotUsed.LastOrDefault();
                                if (payment_schedual != null)
                                {
                                    payment_schedual.debit = payment_schedual.debit + (invoice.GrandTotal - payment_schedualList.Sum(x => x.debit));
                                }

                            }
                            //less
                            else
                            {
                                payment_schedual payment_schedual = payment_schedualListNotUsed.LastOrDefault();
                                if (payment_schedual != null)
                                {
                                    payment_schedual.debit = payment_schedual.debit - (payment_schedualList.Sum(x => x.debit) - invoice.GrandTotal);
                                }
                            }
                        }
                        //  when currency is  changed
                        else
                        {
                            if (payment_schedualListNotUsed.Count() == payment_schedualList.Count())
                            {
                                payment_schedual.RemoveRange(payment_schedualList);
                                List<payment_schedual> payment_schedualListNew = new List<payment_schedual>();
                                Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                                payment_schedualListNew = _Payment.insert_Schedual(invoice);

                                //Save Promisory Note first, because it is referenced in Payment Schedual
                                if (_Payment.payment_promissory_noteLIST != null && _Payment.payment_promissory_noteLIST.Count > 0)
                                {
                                    payment_promissory_note.AddRange(_Payment.payment_promissory_noteLIST);
                                }

                                //Payment Schedual
                                if (payment_schedualListNew != null && payment_schedualListNew.Count > 0)
                                {
                                    payment_schedual.AddRange(payment_schedualListNew);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void MovementReApprove(sales_invoice invoice,sales_invoice_detail detail)
        {
            if (detail.id_location == null)
            {
                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                detail.id_location = _Stock.FindNFix_Location(detail.item.item_product.FirstOrDefault(), detail.app_location, invoice.app_branch);
                detail.app_location = base.app_location.Find(detail.id_location);
            }
            else
            {
                detail.app_location = base.app_location.Find(detail.id_location);
            }

            Brillo.Logic.Stock _stock = new Brillo.Logic.Stock();
            Brillo.Stock stock = new Brillo.Stock();
            List<StockList> Items_InStockLIST = stock.List(detail.app_location.id_branch, (int)detail.id_location, detail.item.item_product.FirstOrDefault().id_item_product);

            base.item_movement.AddRange(_stock.DebitOnly_MovementLIST(this, Items_InStockLIST, Status.Stock.InStock,
                                        App.Names.SalesInvoice,
                                        detail.id_sales_invoice,
                                        detail.id_sales_invoice_detail,
                                        invoice.id_currencyfx,
                                        detail.item.item_product.FirstOrDefault(),
                                        (int)detail.id_location,
                                        detail.quantity,
                                        invoice.trans_date,
                                        "Sales Invoice Fix"
                                        ));
        }

        private void add_CRM(sales_invoice invoice)
        {
            if (invoice.id_sales_order == 0 || invoice.id_sales_order == null)
            {
                crm_opportunity crm_opportunity = new crm_opportunity();
                crm_opportunity.id_contact = invoice.id_contact;
                crm_opportunity.id_currency = invoice.id_currencyfx;
                crm_opportunity.value = invoice.GrandTotal;

                crm_opportunity.sales_invoice.Add(invoice);
                base.crm_opportunity.Add(crm_opportunity);
            }
            else
            {
                crm_opportunity crm_opportunity = sales_order.Where(x => x.id_sales_order == invoice.id_sales_order).FirstOrDefault().crm_opportunity;
                crm_opportunity.sales_invoice.Add(invoice);
                base.crm_opportunity.Attach(crm_opportunity);
            }
        }

        public bool Approve(bool IsDiscountStock)
        {
            bool ApprovalStatus = false;
            NumberOfRecords = 0;
            List<sales_invoice> SalesInvoiceList = sales_invoice.Local.Where(x =>
                                                x.status != Status.Documents_General.Approved
                                                        && x.IsSelected && x.Error == null).ToList();
            foreach (sales_invoice invoice in SalesInvoiceList)
            {
                SpiltInvoice(invoice);
            }

            foreach (sales_invoice invoice in sales_invoice.Local.Where(x =>
                                                x.status != Status.Documents_General.Approved && x.is_head
                                                        && x.IsSelected && x.Error == null))
            {
                if (invoice.id_sales_invoice == 0)
                {
                    SaveChanges();
                }

                invoice.app_condition = app_condition.Find(invoice.id_condition);
                invoice.app_contract = app_contract.Find(invoice.id_contract);
                invoice.app_currencyfx = app_currencyfx.Find(invoice.id_currencyfx);

                if (Check_CreditLimit(invoice))
                {

                    //Logic
                    List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                    Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                    payment_schedualList = _Payment.insert_Schedual(invoice);

                    //Save Promisory Note first, because it is referenced in Payment Schedual
                    if (_Payment.payment_promissory_noteLIST != null && _Payment.payment_promissory_noteLIST.Count > 0)
                    {
                        payment_promissory_note.AddRange(_Payment.payment_promissory_noteLIST);
                    }

                    //Payment Schedual
                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        payment_schedual.AddRange(payment_schedualList);
                    }
                    //Item Movement
                    if (IsDiscountStock)
                    {
                        Insert_Items_2_Movement(invoice);
                    }

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

                        app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == invoice.id_range).FirstOrDefault();

                        invoice.is_issued = true;
                        invoice.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                        invoice.RaisePropertyChanged("number");
                        invoice.status = Status.Documents_General.Approved;
                        invoice.timestamp = DateTime.Now;

                        //Save Changes before Printing, so that all fields show up.
                        SaveChanges();

                        Brillo.Document.Start.Automatic(invoice, app_document_range);
                    }
                    else
                    {
                        invoice.is_issued = false;

                        invoice.status = Status.Documents_General.Approved;
                        invoice.timestamp = DateTime.Now;

                        SaveChanges();
                    }

                    NumberOfRecords += 1;
                    invoice.IsSelected = false;
                }
                ApprovalStatus = true;
            }

            return ApprovalStatus; ;
        }


        /// <summary>
        /// Executes code that will insert Invoiced Items into Movement.
        /// </summary>
        /// <param name="invoice"></param>
        public void Insert_Items_2_Movement(sales_invoice invoice)
        {
            List<item_movement> item_movementList = new List<item_movement>();

            Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
            item_movementList = _Stock.SalesInvoice_Approve(this, invoice);

            if (item_movementList.Count() > 0)
            {
                item_movement.AddRange(item_movementList);

                foreach (sales_invoice_detail sales_detail in invoice.sales_invoice_detail.Where(x => x.item.item_product != null))
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

        public sales_invoice_detail Select_Item(ref sales_invoice sales_invoice, item item, decimal QuantityInStock, bool AllowDuplicateItem)
        {
            if (item != null && item.id_item > 0 && sales_invoice != null)
            {
                if (sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item && a.IsPromo == false).FirstOrDefault() == null || AllowDuplicateItem)
                {
                    return AddDetail(ref sales_invoice, item, QuantityInStock);
                }
                else
                {
                    sales_invoice_detail sales_invoice_detail = sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                    sales_invoice_detail.quantity += 1;
                    return sales_invoice_detail;
                }
            }
            return null;
        }

        public sales_invoice_detail AddDetail(ref sales_invoice sales_invoice, item item, decimal QuantityInStock)
        {
            sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();

            sales_invoice_detail.State = EntityState.Added;

            sales_invoice_detail.CurrencyFX_ID = sales_invoice.id_currencyfx;
            sales_invoice_detail.Contact = sales_invoice.contact;

            sales_invoice_detail.item_description = item.name;
            sales_invoice_detail.item = item;
            sales_invoice_detail.id_item = item.id_item;
            sales_invoice_detail.Quantity_InStock = QuantityInStock;

            int VatGroupID = (int)sales_invoice_detail.id_vat_group;
            sales_invoice_detail.app_vat_group = app_vat_group.Find(VatGroupID);

            sales_invoice_detail.quantity += 1;

            sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
            return sales_invoice_detail;
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
                        sales_invoice _invoice = new sales_invoice();
                        _invoice.code = invoice.code;
                        _invoice.comment = invoice.comment;
                        // _invoice.CreditLimit = invoice.CreditLimit;
                        _invoice.app_branch = invoice.app_branch;
                        _invoice.id_branch = invoice.id_branch;
                        _invoice.app_company = invoice.app_company;
                        _invoice.id_company = invoice.id_company;
                        _invoice.app_condition = invoice.app_condition;
                        _invoice.id_condition = invoice.id_condition;
                        _invoice.contact = invoice.contact;
                        _invoice.id_contact = invoice.id_contact;
                        _invoice.app_contract = invoice.app_contract;
                        _invoice.id_contract = invoice.id_contract;
                        _invoice.app_currencyfx = invoice.app_currencyfx;
                        _invoice.id_currencyfx = invoice.id_currencyfx;
                        _invoice.id_opportunity = invoice.id_opportunity;
                        _invoice.project = invoice.project;
                        _invoice.id_project = invoice.id_project;
                        _invoice.app_document_range = invoice.app_document_range;
                        _invoice.id_range = invoice.id_range;
                        _invoice.sales_order = invoice.sales_order;
                        _invoice.id_sales_order = invoice.id_sales_order;
                        _invoice.sales_rep = invoice.sales_rep;
                        _invoice.id_sales_rep = invoice.id_sales_rep;
                        _invoice.app_terminal = invoice.app_terminal;
                        _invoice.id_terminal = invoice.id_terminal;
                        _invoice.security_user = invoice.security_user;
                        _invoice.id_user = invoice.id_user;
                        _invoice.id_weather = invoice.id_weather;
                        _invoice.number = invoice.number;
                        _invoice.GrandTotal = invoice.GrandTotal;
                        //  _invoice.accounting_journal = invoice.accounting_journal;
                        _invoice.is_head = invoice.is_head;
                        _invoice.is_issued = invoice.is_issued;
                        _invoice.IsSelected = invoice.IsSelected;
                        _invoice.State = EntityState.Added;
                        _invoice.status = Status.Documents_General.Pending;
                        _invoice.trans_date = invoice.trans_date;

                        foreach (sales_invoice_detail detail in invoice.sales_invoice_detail.Skip(position).Take(document_line_limit))
                        {
                            sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();
                            sales_invoice_detail.item_description = detail.item_description;
                            sales_invoice_detail.discount = detail.discount;
                            sales_invoice_detail.id_company = detail.id_company;
                            sales_invoice_detail.item = detail.item;
                            sales_invoice_detail.id_item = detail.id_item;
                            sales_invoice_detail.id_location = detail.id_location;
                            sales_invoice_detail.id_project_task = detail.id_project_task;
                            sales_invoice_detail.id_sales_order_detail = detail.id_sales_order_detail;
                            sales_invoice_detail.id_vat_group = detail.id_vat_group;
                            sales_invoice_detail.is_head = detail.is_head;
                            sales_invoice_detail.IsSelected = detail.IsSelected;
                            sales_invoice_detail.quantity = detail.quantity;
                            sales_invoice_detail.State = EntityState.Added;
                            sales_invoice_detail.SubTotal = detail.SubTotal;
                            sales_invoice_detail.SubTotal_Vat = detail.SubTotal_Vat;
                            sales_invoice_detail.unit_cost = detail.unit_cost;
                            sales_invoice_detail.unit_price = detail.unit_price;
                            sales_invoice_detail.UnitPrice_Vat = detail.UnitPrice_Vat;
                            _invoice.sales_invoice_detail.Add(sales_invoice_detail);
                            position += 1;
                        }
                        sales_invoice.Add(_invoice);
                    }

                    invoice.is_head = false;
                    invoice.status = Status.Documents_General.Approved;

                    SaveChanges();
                }
            }
        }

        public void Anull()
        {
            SaveChanges();

            foreach (sales_invoice sales_invoice in base.sales_invoice.Local.Where(x => x.IsSelected && x.Error == null))
            {
                if (sales_invoice.sales_invoice_detail.Where(x => x.sales_return_detail == null).Count() > 0
                    &&
                    sales_invoice.is_accounted == false)
                {
                    List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                    Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                    payment_schedualList = _Payment.revert_Schedual(sales_invoice);

                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    List<item_movement> item_movementList = new List<item_movement>();
                    item_movementList = _Stock.revert_Stock(this, App.Names.SalesInvoice, sales_invoice);

                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        payment_schedual.RemoveRange(payment_schedualList);
                    }

                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        item_movement.RemoveRange(item_movementList);
                    }

                    sales_invoice.status = Status.Documents_General.Annulled;
                    SaveChanges();
                }
            }
        }

        /// <summary>
        /// Checks for Credit Limit. 
        /// </summary>
        /// <param name="sales_invoice"></param>
        /// <returns></returns>
        public bool Check_CreditLimit(sales_invoice sales_invoice)
        {
            //If Contact Credit Limit is none, we will assume that Credit Limit is not enforced.
            if (sales_invoice.contact.credit_limit != null)
            {
                //If Sales Contract is Cash. Credit Limit is not enforced.
                if (sales_invoice.app_contract.app_contract_detail.Sum(x => x.interval) > 0)
                {
                    //Script that checks Contacts Credit Availability.
                    sales_invoice.contact.Check_CreditAvailability();

                    //Check if Availability is greater than 0.
                    if (sales_invoice.contact.credit_availability > 0)
                    {
                        decimal TotalSales = sales_invoice.GrandTotal;
                        decimal CreditAvailability = (decimal)sales_invoice.contact.credit_availability;

                        if (CreditAvailability < TotalSales)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}