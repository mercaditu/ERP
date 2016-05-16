using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace entity
{
    public partial class SalesInvoiceDB : BaseDB
    {
        public sales_invoice New(int TransDate_OffSet)
        {
            sales_invoice sales_invoice = new sales_invoice();
            sales_invoice.State = EntityState.Added;
            sales_invoice.status = Status.Documents_General.Pending;
            sales_invoice.IsSelected = true;
            sales_invoice.id_range = Brillo.GetDefault.Return_RangeID(App.Names.SalesInvoice);
            sales_invoice.trans_type = Status.TransactionTypes.Normal;
            sales_invoice.trans_date = DateTime.Now.AddDays(TransDate_OffSet);
            sales_invoice.State = EntityState.Added;

            sales_invoice.IsSelected = true;
            sales_invoice.app_branch = app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault();
            sales_invoice.id_range = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault().id_range;

            if (app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault() != null)
            {
                if (app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault().app_condition != null)
                {
                    sales_invoice.app_condition = app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault().app_condition;
                    sales_invoice.app_contract = app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default).FirstOrDefault();
                }
            }

            sales_invoice.app_currencyfx = Brillo.Currency.get_DefaultFX(this);
                //} app_currency.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_priority).FirstOrDefault().app_currencyfx.Where(y => y.is_active).FirstOrDefault();
                //} 

            return sales_invoice;
        }

        public override int SaveChanges()
        {
            validate_Invoice();
            try
            {
                return base.SaveChanges();
            }
           catch(Exception ex)
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
            IList<sales_invoice> sales_invoiceLIST = sales_invoice.Local.ToList();

            foreach (sales_invoice invoice in sales_invoiceLIST)
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
                        //invoice.is_head = false;
                        //sales_invoice.Local.Add(new_Version(invoice));
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
                }
                else if (invoice.State > 0)
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

        public void Approve(bool IsDiscountStock)
        {
            foreach (sales_invoice invoice in sales_invoice.Local.Where(x =>
                                                x.status != Status.Documents_General.Approved
                                                        && x.IsSelected && x.Error == null))
            {
                SpiltInvoice(invoice);
            }

            foreach (sales_invoice invoice in sales_invoice.Local.Where(x =>
                                                x.status != Status.Documents_General.Approved
                                                        && x.IsSelected && x.Error == null))
            {
                if (invoice.CreditLimit >= 0)
                {
                    if (invoice.id_sales_invoice == 0)
                    {
                        SaveChanges();
                    }

                    //Logic
                    List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                    Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                    payment_schedualList = _Payment.insert_Schedual(invoice);

                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        payment_schedual.AddRange(payment_schedualList);
                    }
                    if (IsDiscountStock)
                    {
                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(this, invoice);
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }
                    }

                    if ((invoice.number == null || invoice.number == string.Empty) && invoice.id_range > 0)
                    {
                        invoice.is_issued = true;
                        if (invoice.id_branch > 0)
                        {
                            Brillo.Logic.Range.branch_Code = app_branch.Where(x => x.id_branch == invoice.id_branch).FirstOrDefault().code;
                        }
                        if (invoice.id_terminal > 0)
                        {
                            Brillo.Logic.Range.terminal_Code = app_terminal.Where(x => x.id_terminal == invoice.id_terminal).FirstOrDefault().code;
                        }

                        app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == invoice.id_range).FirstOrDefault();
                        invoice.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                        invoice.RaisePropertyChanged("number");

                        //Save Changes before Printing, so that all fields show up.
                        invoice.status = Status.Documents_General.Approved;
                        SaveChanges();

                        Brillo.Document.Start.Automatic(invoice, app_document_range);
                    }
                    else
                    {
                        invoice.is_issued = false;

                        invoice.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }

                    invoice.IsSelected = false;

                    if (invoice.Error != null)
                    {
                        invoice.HasErrors = true;
                    }
                }

            }
        }

        public sales_invoice_detail Select_Item(ref sales_invoice sales_invoice, item item, bool AllowDuplicateItem)
        {
            if (item != null && item.id_item > 0 && sales_invoice != null)
            {
               // Task Thread = Task.Factory.StartNew(() => select_Item(sales_invoice, item, AllowDuplicateItem));
               return  select_Item(ref sales_invoice, item, AllowDuplicateItem);
            }
            return null;
        }

        private sales_invoice_detail select_Item(ref sales_invoice sales_invoice, item item, bool AllowDuplicateItem)
        {
            if (sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || AllowDuplicateItem)
            {
                sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail();
                _sales_invoice_detail.State = EntityState.Added;
                _sales_invoice_detail.sales_invoice = sales_invoice;
                
                _sales_invoice_detail.Contact = sales_invoice.contact;
                _sales_invoice_detail.item_description = item.description;
                _sales_invoice_detail.item = item;
                _sales_invoice_detail.id_item = item.id_item;

                _sales_invoice_detail.quantity += 1;
                _sales_invoice_detail.app_vat_group = base.app_vat_group.Where(x => x.id_vat_group == _sales_invoice_detail.id_vat_group).FirstOrDefault();
                sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);
                return _sales_invoice_detail;
            }
            else
            {
                sales_invoice_detail sales_invoice_detail = sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_invoice_detail.quantity += 1;
                return sales_invoice_detail;
            }

       
            //Dispatcher.BeginInvoke((Action)(() =>
            //{

            //    CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
            //    sales_invoicesales_invoice_detailViewSource.View.Refresh();
            //    calculate_vat(null, null);
            //}));
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
                        _invoice.CreditLimit = invoice.CreditLimit;
                        _invoice.id_branch = invoice.id_branch;
                        _invoice.id_company = invoice.id_company;
                        _invoice.id_condition = invoice.id_condition;
                        _invoice.id_contact = invoice.id_contact;
                        _invoice.id_contract = invoice.id_contract;
                        _invoice.id_currencyfx = invoice.id_currencyfx;
                        _invoice.id_opportunity = invoice.id_opportunity;
                        _invoice.id_project = invoice.id_project;
                        _invoice.id_range = invoice.id_range;
                        _invoice.id_sales_order = invoice.id_sales_order;
                        _invoice.id_sales_rep = invoice.id_sales_rep;
                        _invoice.id_terminal = invoice.id_terminal;
                        _invoice.id_user = invoice.id_user;
                        _invoice.id_weather = invoice.id_weather;
                        _invoice.number = invoice.number;
                        _invoice.GrandTotal = invoice.GrandTotal;
                        _invoice.accounting_journal = invoice.accounting_journal;
                        _invoice.is_head = invoice.is_head;
                        _invoice.is_issued = invoice.is_issued;
                        _invoice.IsSelected = invoice.IsSelected;
                        _invoice.State = EntityState.Added;
                        _invoice.status = Status.Documents_General.Pending;


                        foreach (sales_invoice_detail detail in invoice.sales_invoice_detail.Skip(position).Take(document_line_limit))
                        {
                            sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();
                            sales_invoice_detail.item_description = detail.item_description;
                            sales_invoice_detail.discount = detail.discount;
                            sales_invoice_detail.id_company = detail.id_company;
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
                }

                SaveChanges();

            }
        }

        public void Anull()
        {
            SaveChanges();

            foreach (sales_invoice sales_invoice in base.sales_invoice.Local)
            {
                if (sales_invoice.IsSelected && sales_invoice.Error == null)
                {
                    if (sales_invoice.sales_return == null || 
                        sales_invoice.sales_return.Count == 0 ||
                        sales_invoice.accounting_journal == null)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.revert_Schedual(sales_invoice);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.revert_Stock(this, App.Names.SalesInvoice, sales_invoice.id_sales_invoice);

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
        }
    }
}