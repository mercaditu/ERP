using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity 
{
    public partial class SalesReturnDB:BaseDB
    {
        public sales_return New()
        {
            sales_return sales_return = new sales_return();
            sales_return.State = EntityState.Added;
            sales_return.id_range = Brillo.GetDefault.Range(App.Names.SalesInvoice);
            sales_return.status = Status.Documents_General.Pending;
            sales_return.trans_date = DateTime.Now;

            sales_return.State = EntityState.Added;
            sales_return.IsSelected = true;

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
                else if (sales_return.State > 0)
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
            if (sales_return.id_sales_invoice == 0 || sales_return.id_sales_invoice == null)
            {
                crm_opportunity crm_opportunity = new crm_opportunity();
                crm_opportunity.id_contact = sales_return.id_contact;
                crm_opportunity.id_currency = sales_return.id_currencyfx;
                crm_opportunity.value = sales_return.sales_return_detail.Sum(x => x.SubTotal_Vat);

                crm_opportunity.sales_return.Add(sales_return);
                base.crm_opportunity.Add(crm_opportunity);
            }
            else
            {
                crm_opportunity crm_opportunity = sales_invoice.Where(x => x.id_sales_invoice == sales_return.id_sales_invoice).FirstOrDefault().crm_opportunity;
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

                    if (sales_return.status != Status.Documents_General.Approved)
                    {
                        if (sales_return.number == null && sales_return.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = base.app_branch.Where(x => x.id_branch == sales_return.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = base.app_terminal.Where(x => x.id_terminal == sales_return.id_terminal).FirstOrDefault().code;
                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == sales_return.id_range).FirstOrDefault();
                            sales_return.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            sales_return.RaisePropertyChanged("number");
                            sales_return.is_issued = true;

                            Brillo.Document.Start.Automatic(sales_order, app_document_range);
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
                        item_movementList = _Stock.insert_Stock(this,sales_return);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            payment_schedual.AddRange(payment_schedualList);
                        }
                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            item_movement.AddRange(item_movementList);
                        }

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
                    item_movementList = _Stock.revert_Stock(this, App.Names.SalesReturn, sales_return.id_sales_return);

                    if (payment_schedualList != null && payment_schedualList.Count > 0)
                    {
                        base.payment_schedual.AddRange(payment_schedualList);
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
