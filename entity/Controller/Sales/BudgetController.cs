using entity.Brillo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace entity.Controller.Sales
{
    public class BudgetController : Base
    { 
        public Brillo.Promotion.Start Promotions { get; set; }
        
        public BudgetController()
        {

        }

        #region Load

        public async void Load(bool FilterByTerminal)
        {
            var predicate = PredicateBuilder.True<sales_budget>();
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

            await db.sales_budget.Where(predicate)
                    .OrderByDescending(x => x.trans_date)
                    .ThenBy(x => x.number)
                    .LoadAsync();
        }

        #endregion

        #region Create

        public sales_budget Create(int TransDate_OffSet, bool IsMigration)
        {
            sales_budget Budget = new sales_budget()
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

            security_user security_user = db.security_user.Find(Budget.id_user);
            if (security_user != null)
            {
                Budget.security_user = security_user;
            }

            //This is to skip query code in case of Migration. Helps speed up migrations.
            if (IsMigration == false)
            {
                Budget.app_document_range = Brillo.Logic.Range.List_Range(db, App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
                Budget.id_condition = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_condition).FirstOrDefault();
                Budget.id_contract = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_contract).FirstOrDefault();
            }

            db.sales_budget.Add(Budget);

            return Budget;
        }

        public sales_budget_detail Create_Detail(
            ref sales_budget Budget,
            item Item,
            item_movement ItemMovement,
            bool AllowDouble,
            decimal QuantityInStock,
            decimal Quantity)
        {
            int? i = null;

            sales_budget_detail Detail = new sales_budget_detail()
            {
                State = EntityState.Added,
                CurrencyFX_ID = Budget.id_currencyfx,
                Contact = Budget.contact,
                item_description = Item.name,
                item = Item,
                id_item = Item.id_item,
                Quantity_InStock = QuantityInStock,
                quantity = Quantity,
                batch_code = ItemMovement != null ? ItemMovement.code : "",
                expire_date = ItemMovement != null ? ItemMovement.expire_date : null,
                movement_id = ItemMovement != null ? (int)ItemMovement.id_movement : i
            };

            int VatGroupID = (int)Detail.id_vat_group;
            Detail.app_vat_group = db.app_vat_group.Find(VatGroupID);

            if (Budget.app_contract == null && Budget.id_contract > 0)
            {
                Budget.app_contract = db.app_contract.Find(Budget.id_contract);
            }

            if (Budget.app_contract != null)
            {
                if (Budget.app_contract.surcharge != null)
                {
                    decimal surcharge = (decimal)Budget.app_contract.surcharge;
                    Detail.unit_price = Detail.unit_price * (1 + surcharge);
                }
            }

            Budget.sales_budget_detail.Add(Detail);

            //Check for Promotions after each insert.
            //Check_Promotions(Order);

            return Detail;
        }

        public void Edit(sales_budget Budget)
        {
            if (Budget != null)
            {
                Budget.IsSelected = true;
                Budget.State = EntityState.Modified;
                db.Entry(Budget).State = EntityState.Modified;
            }
        }

        public bool Archive()
        {
            foreach (sales_budget order in db.sales_budget.Local.Where(x => x.IsSelected))
            {
                order.is_archived = true;
            }

            db.SaveChanges();
            return true;
        }

        #endregion

        #region Save

        //public int SaveChanges_and_Validate()
        //{
        //    NumberOfRecords = 0;
        //    foreach (sales_budget budget in db.sales_budget.Local.Where(x => x.IsSelected && x.id_contact > 0))
        //    {
        //        if (budget.Error == null)
        //        {
        //            if (budget.State == EntityState.Added)
        //            {
        //                budget.timestamp = DateTime.Now;
        //                budget.State = EntityState.Unchanged;
        //                db.Entry(budget).State = EntityState.Added;
        //                Add_CRM(budget);

        //                //Check Promotions before Saving.
        //                Check_Promotions(budget);
        //            }
        //            else if (budget.State == EntityState.Modified)
        //            {
        //                budget.timestamp = DateTime.Now;
        //                budget.State = EntityState.Unchanged;
        //                db.Entry(budget).State = EntityState.Modified;

        //                //Check Promotions before Saving.
        //                Check_Promotions(budget);
        //            }
        //            else if (budget.State == EntityState.Deleted)
        //            {
        //                budget.timestamp = DateTime.Now;
        //                budget.is_head = false;
        //                budget.State = EntityState.Deleted;
        //                db.Entry(budget).State = EntityState.Modified;
        //            }
        //            NumberOfRecords += 1;
        //        }
        //        if (budget.State > 0)
        //        {
        //            if (budget.State != EntityState.Unchanged)
        //            {
        //                db.Entry(budget).State = EntityState.Unchanged;
        //            }
        //        }
        //    }

        //    return db.SaveChanges();
        //}

        private void Add_CRM(sales_budget budget)
        {

            crm_opportunity crm_opportunity = new crm_opportunity()
            {
                id_contact = budget.id_contact,
                id_currency = budget.id_currencyfx,
                value = budget.sales_budget_detail.Sum(x => x.SubTotal_Vat)
            };

            crm_opportunity.sales_budget.Add(budget);
            db.crm_opportunity.Add(crm_opportunity);

        }
        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (sales_budget sales_budget in db.sales_budget.Local.Where(x => x.IsSelected && x.id_contact > 0))
            {
                if (sales_budget.IsSelected && sales_budget.Error == null)
                {
                    if (sales_budget.State == EntityState.Added)
                    {
                        sales_budget.timestamp = DateTime.Now;
                        sales_budget.State = EntityState.Unchanged;
                        db.Entry(sales_budget).State = EntityState.Added;
                        sales_budget.IsSelected = false;
                        Add_CRM(sales_budget);
                    }
                    else if (sales_budget.State == EntityState.Modified)
                    {
                        sales_budget.timestamp = DateTime.Now;
                        sales_budget.State = EntityState.Unchanged;
                        db.Entry(sales_budget).State = EntityState.Modified;
                        sales_budget.IsSelected = false;
                    }
                    NumberOfRecords += 1;
                }

                if (sales_budget.State > 0)
                {
                    if (sales_budget.State != EntityState.Unchanged && sales_budget.Error != null)
                    {
                        if (sales_budget.sales_budget_detail.Count() > 0)
                        {
                            db.sales_budget_detail.RemoveRange(sales_budget.sales_budget_detail);
                        }

                     
                       

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
                db.SaveChanges();
                return true;
            }

        }

    

        #endregion

        #region Approve

        /// <summary>
        /// Approves the Sales Order.
        /// </summary>
        /// <returns></returns>
        public bool Approve()
        {
            NumberOfRecords = 0;
            foreach (sales_budget sales_budget in db.sales_budget.Local.Where(x => x.status != Status.Documents_General.Approved && x.id_contact > 0))
            {
                if (sales_budget.status != Status.Documents_General.Approved &&
                    sales_budget.IsSelected &&
                    sales_budget.Error == null)
                {
                    if (sales_budget.id_sales_budget == 0)
                    {
                        SaveChanges_WithValidation();
                    }

                    sales_budget.app_condition = db.app_condition.Find(sales_budget.id_condition);
                    sales_budget.app_contract = db.app_contract.Find(sales_budget.id_contract);
                    sales_budget.app_currencyfx = db.app_currencyfx.Find(sales_budget.id_currencyfx);
                    if (sales_budget.status != Status.Documents_General.Approved)
                    {
                        if (sales_budget.number == null && sales_budget.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == sales_budget.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == sales_budget.id_terminal).FirstOrDefault().code;
                            app_document_range app_document_range = db.app_document_range.Where(x => x.id_range == sales_budget.id_range).FirstOrDefault();
                            sales_budget.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            sales_budget.RaisePropertyChanged("number");
                            sales_budget.is_issued = true;

                            Brillo.Document.Start.Automatic(sales_budget, app_document_range);

                            //Save Changes before Printing, so that all fields show up.
                            sales_budget.status = Status.Documents_General.Approved;
                            sales_budget.timestamp = DateTime.Now;
                            SaveChanges_WithValidation();
                        }
                        else
                        {
                            sales_budget.is_issued = false;
                            sales_budget.status = Status.Documents_General.Approved;
                            sales_budget.timestamp = DateTime.Now;
                            SaveChanges_WithValidation();
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

        #endregion

        #region Annul

        public bool Annull()
        {
            NumberOfRecords = 0;

            foreach (sales_budget budget in db.sales_budget.Local)
            {
                if (budget.IsSelected && budget.Error == null)
                {
                    if (budget.sales_order == null || budget.sales_order.Count() == 0)
                    {
                        budget.status = Status.Documents_General.Annulled;
                        SaveChanges_WithValidation();
                    }

                    NumberOfRecords += 1;
                    budget.IsSelected = false;
                }
            }

            return true;
        }

        #endregion

        #region Integrations



        #endregion

        #region Promotions

        public async void Check_Promotions(sales_budget Budget)
        {
            if (Budget != null)
            {
                //Cleanup Code
                if (Budget.sales_budget_detail.Where(x => x.id_sales_promotion != null).ToList().Count() > 0)
                {
                    foreach (sales_budget_detail sales_budget_detail in Budget.sales_budget_detail.Where(x => x.id_sales_promotion != null).ToList())
                    {
                        if (sales_budget_detail.id_sales_budget_detail != sales_budget_detail.id_sales_promotion)
                        {
                            db.sales_budget_detail.Remove(sales_budget_detail);
                        }
                    }
                }

                ///Promotions Code
                //Promotions.Calculate_SalesInvoice(ref Invoice);
                Budget.RaisePropertyChanged("GrandTotal");

                //Fixup Code.
                foreach (sales_budget_detail sales_budget_detail in Budget.sales_budget_detail)
                {
                    //Gets the Item into Context.
                    if (sales_budget_detail.item == null)
                    {
                        sales_budget_detail.item = await db.items.FindAsync(sales_budget_detail.id_item);
                    }

                    //Gets the Promotion into Context.
                    if (sales_budget_detail.id_sales_promotion > 0 && sales_budget_detail.sales_promotion == null)
                    {
                        sales_budget_detail.sales_promotion = await db.sales_promotion.FindAsync(sales_budget_detail.id_sales_promotion);
                    }
                }
            }
        }

        #endregion

    }
}
