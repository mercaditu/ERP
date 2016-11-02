using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WPFLocalizeExtension.Extensions;

namespace entity
{
    public partial class SalesReturnDB : BaseDB
    {
        public sales_return New()
        {
            sales_return sales_return = new sales_return();
            sales_return.State = EntityState.Added;
            sales_return.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.SalesReturn, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault(); //Brillo.GetDefault.Return_RangeID(App.Names.SalesReturn);
            sales_return.status = Status.Documents_General.Pending;
            sales_return.trans_date = DateTime.Now;

            sales_return.id_branch = CurrentSession.Id_Branch;
            sales_return.app_branch = base.app_branch.Find(sales_return.id_branch);
            sales_return.State = EntityState.Added;
            sales_return.IsSelected = true;

            //Get any value, so that it doesn't cause NotNull Exception. This data is not important.
            app_contract _app_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault(); // app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default && x.app_contract_detail.Sum(y => y.coefficient) > 0).FirstOrDefault();
            if (_app_contract != null)
            {
                sales_return.id_condition = _app_contract.id_condition;
                sales_return.id_contract = _app_contract.id_contract;
            }

            return sales_return;
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
            foreach (sales_return sales_return in base.sales_return.Local)
            {

                if (sales_return.IsSelected && sales_return.Error == null)
                {
                    if (sales_return.State == EntityState.Added)
                    {
                        sales_return.timestamp = DateTime.Now;
                        sales_return.State = EntityState.Unchanged;
                        Entry(sales_return).State = EntityState.Added;
                        add_CRM(sales_return);
                    }
                    else if (sales_return.State == EntityState.Modified)
                    {
                        sales_return.timestamp = DateTime.Now;
                        sales_return.State = EntityState.Unchanged;
                        Entry(sales_return).State = EntityState.Modified;
                    }
                    else if (sales_return.State == EntityState.Deleted)
                    {
                        sales_return.timestamp = DateTime.Now;
                        sales_return.is_head = false;
                        sales_return.State = EntityState.Deleted;
                        Entry(sales_return).State = EntityState.Modified;
                    }
                }
                if (sales_return.State > 0)
                {
                    if (sales_return.State != EntityState.Unchanged)
                    {
                        Entry(sales_return).State = EntityState.Unchanged;
                    }
                }
            }
        }

        private void add_CRM(sales_return sales_return)
        {
            if (true)
            {

            }
            sales_invoice_detail sales_invoice_detail = sales_return.sales_return_detail.FirstOrDefault() != null? sales_return.sales_return_detail.FirstOrDefault().sales_invoice_detail:null;
            if (sales_invoice_detail == null)
            {
                crm_opportunity crm_opportunity = new crm_opportunity();
                crm_opportunity.id_contact = sales_return.id_contact;
                crm_opportunity.id_currency = sales_return.id_currencyfx;
                crm_opportunity.value = sales_return.GrandTotal;

                crm_opportunity.sales_return.Add(sales_return);
                base.crm_opportunity.Add(crm_opportunity);
            }
            else
            {
                crm_opportunity crm_opportunity = sales_invoice.Where(x => x.id_sales_invoice == sales_invoice_detail.id_sales_invoice).FirstOrDefault().crm_opportunity;
                crm_opportunity.sales_return.Add(sales_return);
                base.crm_opportunity.Attach(crm_opportunity);
            }
        }

        public void Approve()
        {
            foreach (sales_return sales_return in base.sales_return.Local.Where(x => x.status != Status.Documents_General.Approved))
            {
                if (sales_return.status != Status.Documents_General.Approved &&
                    sales_return.IsSelected &&
                    sales_return.Error == null)
                {

                    if (sales_return.id_sales_return == 0)
                    {
                        SaveChanges();
                    }
                    sales_return.app_condition = app_condition.Find(sales_return.id_condition);
                    sales_return.app_contract = app_contract.Find(sales_return.id_contract);
                    sales_return.app_currencyfx = app_currencyfx.Find(sales_return.id_currencyfx);

                    if (sales_return.status != Status.Documents_General.Approved)
                    {
                        if (sales_return.number == null && sales_return.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == sales_return.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == sales_return.id_terminal).FirstOrDefault().code;

                            app_document_range app_document_range = base.app_document_range.Find(sales_return.id_range);

                            sales_return.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            sales_return.RaisePropertyChanged("number");
                            sales_return.is_issued = true;

                            //Save values before printing.
                            SaveChanges();

                            Brillo.Document.Start.Automatic(sales_return, app_document_range);
                        }
                        else
                        {
                            sales_return.is_issued = false;
                        }

                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.insert_Schedual(sales_return);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(this, sales_return);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

                        //Automatically Link Return & Sales
                        Linked2Sales(sales_return);

                        sales_return.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }
                    else if (sales_return.Error != null)
                    {
                        sales_return.HasErrors = true;
                    }
                }
            }
        }

        private void Linked2Sales(sales_return sales_return)
        {
            payment payment = new payment();
            payment.id_contact = sales_return.id_contact;
            payment.status = Status.Documents_General.Approved;

            var Sum_SalesReturnDetail =
                sales_return.sales_return_detail
                .Where(x => x.sales_invoice_detail != null)
                .GroupBy(x => x.sales_invoice_detail.id_sales_invoice)
                .Select(group => new
                {
                    SalesInvoice = group.Select(y => y.sales_invoice_detail.sales_invoice),
                    Sum_SRD = group.Sum(y => y.SubTotal_Vat)
                }).ToList();

            foreach (var Row in Sum_SalesReturnDetail)
            {

                foreach (payment_schedual payment_schedual in Row.SalesInvoice.FirstOrDefault().payment_schedual)
                {

                }
            }


            //Adds Sales Invoice into List for later comparison. This list will help know how much to apply into each sales payment schedual.
            foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail)
            {
                if (sales_return_detail.sales_invoice_detail != null)
                { 
                    //Globalize Total Values
                    List<sales_invoice> Affected_SalesInvoice = new List<sales_invoice>();
                    sales_invoice sales_invoice = sales_return_detail.sales_invoice_detail.sales_invoice;
                    if (sales_invoice != null)
                    {
                        if (Affected_SalesInvoice.Contains(sales_invoice) == false)
                        {
                            Affected_SalesInvoice.Add(sales_invoice);
                        }
                    }


                }
                else
                {
                    //Create Open Schedual
                }
            }

            foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail.OrderBy(x => x.sales_invoice_detail.id_sales_invoice))
            {
                //For each Sales Return.
                decimal Return_SubTotal = sales_return.sales_return_detail.Sum(x => x.SubTotal_Vat);

                if (sales_return_detail.sales_invoice_detail != null)
                {
                    sales_invoice_detail sales_invoice_detail = sales_return_detail.sales_invoice_detail;
                    sales_invoice sales_invoice = sales_invoice_detail.sales_invoice;

                    foreach (payment_schedual payment_schedual in sales_invoice.payment_schedual)
                    {
                        //SalesBalance += payment_schedual.AccountReceivableBalance;

                        //if (sales_return_detail.SubTotal_Vat > SalesBalance && SalesBalance > 0)
                        //{

                        //}
                        //else
                        //{

                        //}
                    }
                }
            }


            payment_detail payment_detail = new payment_detail();
            payment_detail.id_currencyfx = sales_return.id_currencyfx;
            payment_detail.id_sales_return = sales_return.id_sales_return;

            //Creates Payment Type if does not exist.
            if (base.payment_type.Where(x => x.payment_behavior == entity.payment_type.payment_behaviours.CreditNote).FirstOrDefault() != null)
            {
                payment_detail.id_payment_type = base.payment_type.Where(x => x.payment_behavior == entity.payment_type.payment_behaviours.CreditNote).FirstOrDefault().id_payment_type;
            }
            else
            {
                //In case Payment type doesn not exist, this will create it and try to fix the error.
                payment_type payment_type = new payment_type();
                payment_type.payment_behavior = entity.payment_type.payment_behaviours.CreditNote;
                payment_type.name = LocExtension.GetLocalizedValue<string>("Cognitivo:local:SalesReturn");
                base.payment_type.Add(payment_type);

                payment_detail.payment_type = payment_type;
            }

            //1) Get Balance of Invoice.
            //2) Clean balance per balance.

            foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail)
            {
                if (sales_return_detail.sales_invoice_detail != null)
                {
                    sales_invoice_detail sales_invoice_detail = sales_return_detail.sales_invoice_detail;
                    sales_invoice sales_invoice = sales_invoice_detail.sales_invoice;

                    foreach (payment_schedual payment_schedual in sales_invoice.payment_schedual)
                    {
                        //SalesBalance += payment_schedual.AccountReceivableBalance;
                    }
                }
            }

            ///If Return > Schedual Balance
            //if (sales_return.GrandTotal > SalesBalance && SalesBalance > 0)
            //{
            //    /// 
            //    /// Consume Balance + Create new Schedual with remainder
            //    ///
            //    payment_detail.value = SalesBalance;

            //    payment_schedual Schedual = new payment_schedual();
            //    Schedual.credit = 0;
            //    //Remainder balance
            //    Schedual.debit = sales_return.GrandTotal - SalesBalance;
            //    Schedual.id_currencyfx = sales_return.id_currencyfx;
            //    Schedual.sales_return = sales_return;
            //    Schedual.trans_date = sales_return.trans_date;
            //    Schedual.expire_date = sales_return.trans_date;
            //    Schedual.status = Status.Documents_General.Approved;
            //    Schedual.id_contact = sales_return.id_contact;
            //    Schedual.can_calculate = false;

            //    base.payment_schedual.Add(Schedual);
            //}
            //else ///If Return < Schedual Balance
            //{
            //    payment_schedual Schedual = new payment_schedual();
            //    Schedual.debit = 0;
            //    Schedual.credit = sales_return.GrandTotal;
            //    Schedual.id_currencyfx = sales_return.id_currencyfx;
            //    Schedual.sales_return = sales_return;
            //    Schedual.trans_date = sales_return.trans_date;
            //    Schedual.expire_date = sales_return.trans_date;
            //    Schedual.status = Status.Documents_General.Approved;
            //    Schedual.id_contact = sales_return.id_contact;
            //    Schedual.can_calculate = true;
            //    Schedual.parent = sales_return.sales_invoice.payment_schedual.FirstOrDefault();

            //    payment_detail.value = sales_return.GrandTotal;
            //    payment_detail.payment_schedual.Add(Schedual);
            //}


            payment.payment_detail.Add(payment_detail);
            base.payments.Add(payment);
        }

        public void Anull()
        {
            foreach (sales_return sales_return in base.sales_return.Local)
            {
                if (sales_return.IsSelected && sales_return.Error == null)
                {
                    List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                    Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                    payment_schedualList = _Payment.revert_Schedual(sales_return);

                    Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                    List<item_movement> item_movementList = new List<item_movement>();
                    item_movementList = _Stock.revert_Stock(this, App.Names.SalesReturn, sales_return);

                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        base.payment_schedual.RemoveRange(payment_schedualList);
                    }
                    if (item_movementList != null && item_movementList.Count > 0)
                    {
                        base.item_movement.RemoveRange(item_movementList);
                    }

                    sales_return.status = Status.Documents_General.Annulled;
                    SaveChanges();
                }
            }
        }
    }
}
