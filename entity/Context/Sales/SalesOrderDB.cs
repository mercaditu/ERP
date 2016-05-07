using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial  class SalesOrderDB : BaseDB
    {
        public sales_order New()
        {
            sales_order sales_order = new sales_order();
            sales_order.State = EntityState.Added;
            sales_order.id_range = Brillo.GetDefault.Return_RangeID(App.Names.SalesOrder);
            sales_order.status = Status.Documents_General.Pending;
            
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
            List<sales_order> sales_orderLIST = base.sales_order.Local.ToList();
            foreach (sales_order sales_order in sales_orderLIST)
            {
                if(sales_order.IsSelected && sales_order.Error == null)
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
                }
                else if (sales_order.State > 0)
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

        public void Approve()
        {

            foreach (sales_order sales_order in base.sales_order.Local.Where(x => x.status != Status.Documents_General.Approved))
            {
                if (sales_order.status != Status.Documents_General.Approved &&
                    sales_order.IsSelected &&
                    sales_order.Error == null)
                {
                    if (sales_order.id_sales_order == 0)
                    {
                        SaveChanges();
                    }

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
                            Brillo.Logic.Range.branch_Code = base.app_branch.Where(x => x.id_branch == sales_order.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = base.app_terminal.Where(x => x.id_terminal == sales_order.id_terminal).FirstOrDefault().code;
                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == sales_order.id_range).FirstOrDefault();
                            sales_order.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            sales_order.RaisePropertyChanged("number");
                            sales_order.is_issued = true;

                            Brillo.Document.Start.Automatic(sales_order, app_document_range);
                        }
                        else
                        {
                            sales_order.is_issued = false;
                        }

                        sales_order.status = Status.Documents_General.Approved;
                        SaveChanges();
                    }
                }

                if (sales_order.Error != null)
                {
                    sales_order.HasErrors = true;
                }
            }
        }

        public void Annull()
        {
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
                sales_order.IsSelected = false;
            }
        }

    }
}
