using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
  public partial class SalesBudgetDB : BaseDB
    {
        public sales_budget New()
        {
            sales_budget sales_budget = new sales_budget();
            sales_budget.status = Status.Documents_General.Pending;
            sales_budget.State = EntityState.Added;
            sales_budget.app_document_range = Brillo.Logic.Range.List_Range(this, App.Names.SalesBudget, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault(); //Brillo.GetDefault.Return_RangeID(App.Names.SalesBudget);
            sales_budget.trans_date = DateTime.Now;

            sales_budget.IsSelected = true;

            return sales_budget;
        }

        public override int SaveChanges()
        {
            validate_Budget();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Budget();
            return base.SaveChangesAsync();
        }

        private void validate_Budget()
        {
            NumberOfRecords = 0;

            foreach (sales_budget sales_budget in base.sales_budget.Local)
            {
                if (sales_budget.IsSelected && sales_budget.Error == null)
                {
                    if (sales_budget.State == EntityState.Added)
                    {
                        sales_budget.timestamp = DateTime.Now;
                        sales_budget.State = EntityState.Unchanged;
                        Entry(sales_budget).State = EntityState.Added;
                        add_CRM(sales_budget);
                    }
                    else if (sales_budget.State == EntityState.Modified)
                    {
                        sales_budget.timestamp = DateTime.Now;
                        sales_budget.State = EntityState.Unchanged;
                        Entry(sales_budget).State = EntityState.Modified;
                    }
                    else if (sales_budget.State == EntityState.Deleted)
                    {
                        sales_budget.timestamp = DateTime.Now;
                        sales_budget.is_head = false;
                        sales_budget.State = EntityState.Deleted;
                        Entry(sales_budget).State = EntityState.Modified;
                    }

                    NumberOfRecords += 1;
                }
                if (sales_budget.State > 0)
                {
                    if (sales_budget.State != EntityState.Unchanged)
                    {
                        Entry(sales_budget).State = EntityState.Unchanged;
                    }
                }
            }
        }

        private void add_CRM(sales_budget invoice)
        {
            crm_opportunity crm_opportunity = new crm_opportunity();
            crm_opportunity.id_contact = invoice.id_contact;
            crm_opportunity.id_currency = invoice.id_currencyfx;
            crm_opportunity.value = invoice.sales_budget_detail.Sum(x => x.SubTotal_Vat); 

            crm_opportunity.sales_budget.Add(invoice);
            base.crm_opportunity.Add(crm_opportunity);
        }

        public bool Approve()
        {
            NumberOfRecords = 0;
            foreach (sales_budget sales_budget in base.sales_budget.Local.Where(x => x.status != Status.Documents_General.Approved))
            {
                if (sales_budget.status != Status.Documents_General.Approved &&
                    sales_budget.IsSelected &&
                    sales_budget.Error == null)
                {
                    if (sales_budget.id_sales_budget == 0)
                    {
                        SaveChanges();
                    }

                    if (sales_budget.status != Status.Documents_General.Approved)
                    {
                        if (sales_budget.number == null && sales_budget.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = base.app_branch.Where(x => x.id_branch == sales_budget.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = base.app_terminal.Where(x => x.id_terminal == sales_budget.id_terminal).FirstOrDefault().code;
                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == sales_budget.id_range).FirstOrDefault();
                            sales_budget.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            sales_budget.RaisePropertyChanged("number");
                            sales_budget.is_issued = true;

                            Brillo.Document.Start.Automatic(sales_budget, app_document_range);

                            //Save Changes before Printing, so that all fields show up.
                            sales_budget.status = Status.Documents_General.Approved;
                            sales_budget.timestamp = DateTime.Now;
                            SaveChanges();
                        }
                        else
                        {
                            sales_budget.is_issued = false;
                            sales_budget.status = Status.Documents_General.Approved;
                            sales_budget.timestamp = DateTime.Now;
                            SaveChanges();
                        }
                    }

                    NumberOfRecords += 1;
                    sales_budget.IsSelected = false;
                }

                if (sales_budget.Error != null)
                {
                    sales_budget.HasErrors = true;
                }
            }

            return true;
        }

        public bool Anull()
        {
            NumberOfRecords = 0;

            foreach (sales_budget budget in base.sales_budget.Local)
            {
                if (budget.IsSelected && budget.Error == null)
                {
                    if(budget.sales_order.Count()==0)
                    {
                        budget.status = Status.Documents_General.Annulled;
                        SaveChanges();
                    }

                    NumberOfRecords += 1;
                    budget.IsSelected = false;
                }
            }

            return true;
        }
    }
}
