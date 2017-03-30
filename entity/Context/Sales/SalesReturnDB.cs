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
            else
            {
                //Fix if no Default is Available.
                app_contract app_contract = new app_contract();
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
            foreach (sales_return sales_return in base.sales_return.Local.Where(x => x.IsSelected))
            {
                if (sales_return.Error == null)
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
            sales_invoice_detail sales_invoice_detail = sales_return.sales_return_detail.FirstOrDefault() != null ? sales_return.sales_return_detail.FirstOrDefault().sales_invoice_detail : null;
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

        public void SpiltInvoice(sales_return sales_return)
        {
            if ((sales_return.number == null || sales_return.number == string.Empty) && sales_return.app_document_range != null)
            {
                int document_line_limit = 0;

                if (sales_return.app_document_range.app_document.line_limit != null)
                {
                    document_line_limit = (int)sales_return.app_document_range.app_document.line_limit;
                }

                if (document_line_limit > 0 && sales_return.sales_return_detail.Count > document_line_limit)
                {
                    int NoOfInvoice = (int)Math.Ceiling(sales_return.sales_return_detail.Count / (decimal)document_line_limit);

                    //Counter Variable for not loosing place in Detail
                    int position = 0;

                    for (int i = 1; i <= NoOfInvoice; i++)
                    {
                        sales_return _return = new sales_return();
                        _return.code = sales_return.code;
                        _return.comment = sales_return.comment;
                        _return.return_type = sales_return.return_type;
                        // _invoice.CreditLimit = invoice.CreditLimit;
                        _return.app_branch = sales_return.app_branch;
                        _return.id_branch = sales_return.id_branch;
                        _return.app_company = sales_return.app_company;
                        _return.id_company = sales_return.id_company;
                        _return.app_condition = sales_return.app_condition;
                        _return.id_condition = sales_return.id_condition;
                        _return.contact = sales_return.contact;
                        _return.id_contact = sales_return.id_contact;
                        _return.app_contract = sales_return.app_contract;
                        _return.id_contract = sales_return.id_contract;
                        _return.app_currencyfx = sales_return.app_currencyfx;
                        _return.id_currencyfx = sales_return.id_currencyfx;
                        _return.id_opportunity = sales_return.id_opportunity;
                        _return.project = sales_return.project;
                        _return.id_project = sales_return.id_project;
                        _return.app_document_range = sales_return.app_document_range;
                        _return.id_range = sales_return.id_range;
                        _return.sales_invoice = sales_return.sales_invoice;
                        _return.id_sales_invoice = sales_return.id_sales_invoice;
                        _return.sales_rep = sales_return.sales_rep;
                        _return.id_sales_rep = sales_return.id_sales_rep;
                        _return.app_terminal = sales_return.app_terminal;
                        _return.id_terminal = sales_return.id_terminal;
                        _return.security_user = sales_return.security_user;
                        _return.id_user = sales_return.id_user;
                        _return.id_weather = sales_return.id_weather;
                        _return.number = sales_return.number;
                        _return.GrandTotal = sales_return.GrandTotal;
                        //  _invoice.accounting_journal = invoice.accounting_journal;
                        _return.is_head = sales_return.is_head;
                        _return.is_issued = sales_return.is_issued;
                        _return.IsSelected = sales_return.IsSelected;
                        _return.State = EntityState.Added;
                        _return.status = Status.Documents_General.Pending;
                        _return.trans_date = sales_return.trans_date;

                        foreach (sales_return_detail detail in sales_return.sales_return_detail.Skip(position).Take(document_line_limit))
                        {
                            sales_return_detail sales_return_detail = new sales_return_detail();
                            sales_return_detail.item_description = detail.item_description;
                            sales_return_detail.discount = detail.discount;
                            sales_return_detail.id_company = detail.id_company;
                            sales_return_detail.item = detail.item;
                            sales_return_detail.id_item = detail.id_item;
                            sales_return_detail.id_location = detail.id_location;
                            sales_return_detail.id_project_task = detail.id_project_task;
                            sales_return_detail.sales_invoice_detail = detail.sales_invoice_detail;
                            sales_return_detail.id_vat_group = detail.id_vat_group;
                            sales_return_detail.is_head = detail.is_head;
                            sales_return_detail.IsSelected = detail.IsSelected;
                            sales_return_detail.quantity = detail.quantity;
                            sales_return_detail.State = EntityState.Added;
                            sales_return_detail.SubTotal = detail.SubTotal;
                            sales_return_detail.SubTotal_Vat = detail.SubTotal_Vat;
                            sales_return_detail.unit_cost = detail.unit_cost;
                            sales_return_detail.unit_price = detail.unit_price;
                            sales_return_detail.UnitPrice_Vat = detail.UnitPrice_Vat;
                            //sales_return_detail.sales_return = _return;
                            _return.sales_return_detail.Add(sales_return_detail);
                            position += 1;
                        }
                        base.sales_return.Add(_return);
                    }

                    sales_return.is_head = false;
                    sales_return.status = Status.Documents_General.Approved;

                    SaveChanges();
                }
            }
        }

        public void Approve()
        {
            List<sales_return> SalesReturnList = base.sales_return.Local.Where(x =>
                                                x.status != Status.Documents_General.Approved
                                                        && x.IsSelected && x.Error == null).ToList();
            foreach (sales_return sales_return in SalesReturnList)
            {
                SpiltInvoice(sales_return);
            }

            foreach (sales_return sales_return in base.sales_return.Local.Where(x => x.status != Status.Documents_General.Approved))
            {
                if (sales_return.status != Status.Documents_General.Approved &&
                    sales_return.IsSelected && sales_return.is_head &&
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

                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        payment_schedualList = _Payment.insert_Schedual(sales_return);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.SalesReturn_Approve(this, sales_return);

                        if (item_movementList.Count() > 0)
                        {
                            item_movement.AddRange(item_movementList);

                            foreach (sales_return_detail sales_return_detail in sales_return.sales_return_detail.Where(x => x.item.item_product != null && x.IsSelected))
                            {
                                if (sales_return_detail.item_movement.FirstOrDefault() != null)
                                {
                                    if (sales_return_detail.item_movement.FirstOrDefault().item_movement_value != null)
                                    {
                                        sales_return_detail.unit_cost = Brillo.Currency.convert_Values(sales_return_detail.item_movement.FirstOrDefault().item_movement_value.Sum(x => x.unit_value),
                                        sales_return_detail.item_movement.FirstOrDefault().item_movement_value.FirstOrDefault().id_currencyfx,
                                        sales_return_detail.sales_return.id_currencyfx, App.Modules.Sales);
                                    }
                                }
                            }
                        }
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

                        SaveChanges();

                        //Automatically Link Return & Sales
                        Linked2Sales(sales_return);

                        sales_return.IsSelected = false;
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
            payment_type payment_type = base.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.CreditNote).FirstOrDefault();

            payment payment = new payment();
            payment.id_contact = sales_return.id_contact;
            payment.status = Status.Documents_General.Approved;

            BrilloQuery.Sales Sales = new BrilloQuery.Sales();
            List<BrilloQuery.ReturnInvoice_Integration> ReturnList = Sales.Get_ReturnInvoice_Integration(sales_return.id_sales_return);

            foreach (BrilloQuery.ReturnInvoice_Integration Return in ReturnList)
            {
                if (Return.InvoiceID > 0)
                {
                    //Sales Invoice Integrated.
                    sales_invoice sales_invoice = base.sales_invoice.Find(Return.InvoiceID);
                    decimal Return_GrandTotal_ByInvoice = ReturnList.Where(x => x.InvoiceID == Return.InvoiceID).Sum(x => x.SubTotalVAT);

                    foreach (payment_schedual payment_schedual in sales_invoice.payment_schedual.Where(x => x.AccountReceivableBalance > 0))
                    {
                        if (payment_schedual.AccountReceivableBalance > 0 && Return_GrandTotal_ByInvoice > 0)
                        {

                            decimal PaymentValue = 
                                payment_schedual.AccountReceivableBalance < Return_GrandTotal_ByInvoice 
                                ? 
                                payment_schedual.AccountReceivableBalance 
                                : 
                                Return_GrandTotal_ByInvoice;

                            Return_GrandTotal_ByInvoice -= PaymentValue;

                            payment_schedual Schedual = new payment_schedual();
                            Schedual.credit = PaymentValue;
                            Schedual.debit = 0;
                            Schedual.id_currencyfx = sales_return.id_currencyfx;
                            Schedual.sales_return = sales_return;
                            Schedual.trans_date = sales_return.trans_date;
                            Schedual.expire_date = sales_return.trans_date;
                            Schedual.status = Status.Documents_General.Approved;
                            Schedual.id_contact = sales_return.id_contact;
                            Schedual.can_calculate = true;
                            Schedual.parent = payment_schedual;

                            payment_detail payment_detail = new payment_detail();
                            payment_detail.id_currencyfx = sales_return.id_currencyfx;
                            payment_detail.id_sales_return = sales_return.id_sales_return;
                            payment_detail.payment_type = payment_type != null ? payment_type : Fix_PaymentType();

                            payment_detail.value = PaymentValue;
                            payment_detail.payment_schedual.Add(Schedual);

                            payment.payment_detail.Add(payment_detail);
                        }
                    }
                }
            }

            base.payments.Add(payment);
        }

        private payment_type Fix_PaymentType()
        {
            //In case Payment type doesn not exist, this will create it and try to fix the error.
            payment_type payment_type = new payment_type();
            payment_type.payment_behavior = entity.payment_type.payment_behaviours.CreditNote;
            payment_type.name = LocExtension.GetLocalizedValue<string>("Cognitivo:local:SalesReturn");
            base.payment_type.Add(payment_type);

            return payment_type;
        }

        public void Anull()
        {
            foreach (sales_return sales_return in base.sales_return.Local.Where(x => x.IsSelected && x.Error == null && x.status == Status.Documents_General.Approved))
            {
                //Clean the Payment Schedual. If Return has benn used, this will clean it from existance.
                if (sales_return.payment_schedual != null && sales_return.payment_schedual.Count > 0)
                {
                    foreach (payment_schedual payment_schedual in sales_return.payment_schedual)
                    {
                        if (payment_schedual.payment_detail != null)
                        {
                            //Remove Payment Detail from history.
                            base.payment_detail.Remove(payment_schedual.payment_detail);
                        }
                    }
                    //Remove Schedual from history.
                    base.payment_schedual.RemoveRange(sales_return.payment_schedual);
                }

                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                List<item_movement> item_movementList = new List<item_movement>();
                item_movementList = _Stock.revert_Stock(this, App.Names.SalesReturn, sales_return);

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