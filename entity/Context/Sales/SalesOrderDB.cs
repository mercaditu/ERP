using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class SalesOrderDB : BaseDB
    {
        public sales_order New()
        {
            sales_order sales_order = new sales_order();
            sales_order.State = EntityState.Added;
            sales_order.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault(); //Brillo.GetDefault.Return_RangeID(App.Names.SalesBudget);
            sales_order.status = Status.Documents_General.Pending;

            app_contract _app_contract = CurrentSession.Contracts.Where(x => x.is_default).FirstOrDefault(); // app_contract.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_default && x.app_contract_detail.Sum(y => y.coefficient) > 0).FirstOrDefault();
            if (_app_contract != null)
            {
                sales_order.id_condition = _app_contract.id_condition;
                sales_order.id_contract = _app_contract.id_contract;
            }

            security_user security_user = base.security_user.Find(sales_order.id_user);
            if (security_user != null)
            {
                sales_order.security_user = security_user;
            }

            sales_order.State = EntityState.Added;
            sales_order.IsSelected = true;

            return sales_order;
        }

        public override int SaveChanges()
        {
            validate_Order();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Order();
            return base.SaveChangesAsync();
        }

        private void validate_Order()
        {
            NumberOfRecords = 0;

            List<sales_order> sales_orderLIST = base.sales_order.Local.Where(x => x.IsSelected && x.id_contact > 0).ToList();
            foreach (sales_order sales_order in sales_orderLIST)
            {
                if (sales_order.IsSelected && sales_order.Error == null)
                {
                    if (sales_order.State == EntityState.Added)
                    {
                        sales_order.timestamp = DateTime.Now;
                        sales_order.State = EntityState.Unchanged;
                        Entry(sales_order).State = EntityState.Added;
                        add_CRM(sales_order);
                    }
                    else if (sales_order.State == EntityState.Modified)
                    {
                        sales_order.timestamp = DateTime.Now;
                        sales_order.State = EntityState.Unchanged;
                        Entry(sales_order).State = EntityState.Modified;
                    }
                    else if (sales_order.State == EntityState.Deleted)
                    {
                        sales_order.timestamp = DateTime.Now;
                        sales_order.is_head = false;
                        sales_order.State = EntityState.Deleted;
                        Entry(sales_order).State = EntityState.Modified;
                    }

                    NumberOfRecords += 1;
                }
                if (sales_order.State > 0)
                {
                    if (sales_order.State != EntityState.Unchanged)
                    {
                        Entry(sales_order).State = EntityState.Unchanged;
                    }
                }
            }
        }

        private void add_CRM(sales_order order)
        {
            if (order.id_sales_budget == 0 || order.id_sales_budget == null)
            {
                crm_opportunity crm_opportunity = new crm_opportunity();
                crm_opportunity.id_contact = order.id_contact;
                crm_opportunity.id_currency = order.id_currencyfx;

                crm_opportunity.value = order.sales_order_detail.Sum(x => x.SubTotal_Vat);

                crm_opportunity.sales_order.Add(order);
                base.crm_opportunity.Add(crm_opportunity);
            }
            else
            {
                crm_opportunity crm_opportunity = sales_budget.Where(x => x.id_sales_budget == order.id_sales_budget).FirstOrDefault().crm_opportunity;
                crm_opportunity.sales_order.Add(order);
                base.crm_opportunity.Attach(crm_opportunity);
            }
        }

        public bool Approve()
        {
            foreach (sales_order sales_order in base.sales_order.Local.Where(x => x.status != Status.Documents_General.Approved && x.id_contact > 0))
            {
                NumberOfRecords = 0;

                if (sales_order.status != Status.Documents_General.Approved &&
                    sales_order.IsSelected &&
                    sales_order.Error == null)
                {
                    if (sales_order.id_sales_order == 0 && sales_order.id_contact > 0)
                    {
                        SaveChanges();
                    }
                    sales_order.app_condition = app_condition.Find(sales_order.id_condition);
                    sales_order.app_contract = app_contract.Find(sales_order.id_contract);
                    sales_order.app_currencyfx = app_currencyfx.Find(sales_order.id_currencyfx);

                    if (sales_order.status != Status.Documents_General.Approved)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.insert_Schedual(sales_order);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(this, sales_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

                        if (sales_order.number == null && sales_order.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == sales_order.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == sales_order.id_terminal).FirstOrDefault().code;

                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == sales_order.id_range).FirstOrDefault();
                            sales_order.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            sales_order.RaisePropertyChanged("number");
                            sales_order.is_issued = true;

                            //Save Changes before Printing, so that all fields show up.
                            sales_order.status = Status.Documents_General.Approved;
                            sales_order.timestamp = DateTime.Now;
                            SaveChanges();

                            Brillo.Document.Start.Automatic(sales_order, app_document_range);
                        }
                        else
                        {
                            sales_order.is_issued = false;
                            sales_order.status = Status.Documents_General.Approved;
                            sales_order.timestamp = DateTime.Now;
                            SaveChanges();
                        }

                        //This ensures that only checked items go into requests at the time of approval.
                        if (sales_order.sales_order_detail.Where(x => x.IsSelected).Count() > 0)
                        {
                            item_request item_request = new item_request();
                            item_request.name = sales_order.contact.name;
                            item_request.comment = sales_order.comment;

                            item_request.id_sales_order = sales_order.id_sales_order;
                            item_request.id_branch = sales_order.id_branch;

                            item_request.request_date = (DateTime)sales_order.delivery_date;

                            foreach (sales_order_detail data in sales_order.sales_order_detail.Where(x => x.IsSelected))
                            {
                                item_request_detail item_request_detail = new item_request_detail();
                                item_request_detail.date_needed_by = (DateTime)sales_order.delivery_date;
                                item_request_detail.id_sales_order_detail = data.id_sales_order_detail;
                                item_request_detail.urgency = entity.item_request_detail.Urgencies.Medium;
                                int idItem = data.item.id_item;
                                item_request_detail.id_item = idItem;
                                item item = base.items.Where(x => x.id_item == idItem).FirstOrDefault();
                                if (item != null)
                                {
                                    item_request_detail.item = item;
                                    item_request_detail.comment = item_request_detail.item.name;
                                }

                                item_request_detail.quantity = data.quantity;

                                foreach (item_dimension item_dimension in item.item_dimension)
                                {
                                    item_request_dimension item_request_dimension = new item_request_dimension();
                                    item_request_dimension.id_dimension = item_dimension.id_app_dimension;
                                    item_request_dimension.app_dimension = item_dimension.app_dimension;
                                    item_request_dimension.id_measurement = item_dimension.id_measurement;
                                    item_request_dimension.app_measurement = item_dimension.app_measurement;
                                    item_request_dimension.value = item_dimension.value;
                                    item_request_detail.item_request_dimension.Add(item_request_dimension);
                                }

                                item_request.item_request_detail.Add(item_request_detail);
                            }
                            base.item_request.Add(item_request);
                        }

                        SaveChanges();
                    }

                    NumberOfRecords += 1;
                    sales_order.IsSelected = false;
                }

                if (sales_order.Error != null)
                {
                    sales_order.HasErrors = true;
                }
            }

            return true;
        }

        public bool Annull()
        {
            NumberOfRecords = 0;
            foreach (sales_order sales_order in base.sales_order.Local)
            {
                if (sales_order.IsSelected && sales_order.Error == null)
                {
                    SaveChanges();

                    if (sales_order.status == Status.Documents_General.Approved)
                    {
                        if (sales_order.sales_invoice == null || sales_order.sales_invoice.Count == 0)
                        {
                            List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                            Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                            payment_schedualList = _Payment.revert_Schedual(sales_order);

                            Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                            List<item_movement> item_movementList = new List<item_movement>();
                            item_movementList = _Stock.revert_Stock(this, App.Names.SalesOrder, sales_order.id_sales_order);

                            if (payment_schedualList != null && payment_schedualList.Count > 0)
                            {
                                base.payment_schedual.RemoveRange(payment_schedualList);
                            }

                            if (item_movementList != null && item_movementList.Count > 0)
                            {
                                base.item_movement.RemoveRange(item_movementList);
                            }

                            sales_order.status = Status.Documents_General.Annulled;
                            SaveChanges();
                        }
                    }
                }

                NumberOfRecords += 1;
                sales_order.IsSelected = false;
            }
            return true;
        }
    }
}