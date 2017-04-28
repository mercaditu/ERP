using entity.Brillo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace entity.Controller.Sales
{
    public class OrderController
    {
        public int NumberOfRecords;

        /// <summary>
        /// Database Context. Already Initialized.
        /// </summary>
        public db db { get; set; }
        public Brillo.Promotion.Start Promotions { get; set; }

        #region Properties

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

        #endregion

        public OrderController()
        {

        }

        public void Initialize()
        {
            db = new db();
            Promotions = new Brillo.Promotion.Start(true);
        }

        #region Load

        public async void Load(bool FilterByTerminal)
        {
            var predicate = PredicateBuilder.True<sales_order>();
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

            await db.sales_order.Where(predicate)
                    .OrderByDescending(x => x.trans_date)
                    .ThenBy(x => x.number)
                    .LoadAsync();
        }

        #endregion

        #region Create

        public sales_order Create(int TransDate_OffSet, bool IsMigration)
        {
            sales_order Order = new sales_order()
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

            security_user security_user = db.security_user.Find(Order.id_user);
            if (security_user != null)
            {
                Order.security_user = security_user;
            }

            //This is to skip query code in case of Migration. Helps speed up migrations.
            if (IsMigration == false)
            {
                Order.app_document_range = Brillo.Logic.Range.List_Range(db, App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
                Order.id_condition = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_condition).FirstOrDefault();
                Order.id_contract = CurrentSession.Contracts.Where(x => x.is_default).Select(x => x.id_contract).FirstOrDefault();
            }

            db.sales_order.Add(Order);

            return Order;
        }

        public sales_order_detail Create_Detail(
            ref sales_order Order,
            item Item,
            item_movement ItemMovement,
            bool AllowDouble,
            decimal QuantityInStock,
            decimal Quantity)
        {
            int? i = null;

            sales_order_detail Detail = new sales_order_detail()
            {
                State = EntityState.Added,
                CurrencyFX_ID = Order.id_currencyfx,
                Contact = Order.contact,
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

            if (Order.app_contract == null && Order.id_contract > 0)
            {
                Order.app_contract = db.app_contract.Find(Order.id_contract);
            }

            if (Order.app_contract != null)
            {
                if (Order.app_contract.surcharge != null)
                {
                    decimal surcharge = (decimal)Order.app_contract.surcharge;
                    Detail.unit_price = Detail.unit_price * (1 + surcharge);
                }
            }

            Order.sales_order_detail.Add(Detail);

            //Check for Promotions after each insert.
            //Check_Promotions(Order);

            return Detail;
        }

        public void Edit(sales_order Order)
        {
            if (Order != null)
            {
                Order.IsSelected = true;
                Order.State = EntityState.Modified;
                db.Entry(Order).State = EntityState.Modified;
            }
        }

        public bool Archive()
        {
            foreach (sales_order order in db.sales_order.Local.Where(x => x.IsSelected))
            {
                order.is_archived = true;
            }

            db.SaveChanges();
            return true;
        }
        
        #endregion

        #region Save

        public int SaveChanges_and_Validate()
        {
            NumberOfRecords = 0;
            foreach (sales_order order in db.sales_order.Local.Where(x => x.IsSelected && x.id_contact > 0))
            {
                if (order.Error == null)
                {
                    if (order.State == EntityState.Added)
                    {
                        order.timestamp = DateTime.Now;
                        order.State = EntityState.Unchanged;
                        db.Entry(order).State = EntityState.Added;
                        Add_CRM(order);

                        //Check Promotions before Saving.
                        Check_Promotions(order);
                    }
                    else if (order.State == EntityState.Modified)
                    {
                        order.timestamp = DateTime.Now;
                        order.State = EntityState.Unchanged;
                        db.Entry(order).State = EntityState.Modified;

                        //Check Promotions before Saving.
                        Check_Promotions(order);
                    }
                    else if (order.State == EntityState.Deleted)
                    {
                        order.timestamp = DateTime.Now;
                        order.is_head = false;
                        order.State = EntityState.Deleted;
                        db.Entry(order).State = EntityState.Modified;
                    }
                    NumberOfRecords += 1;
                }
                if (order.State > 0)
                {
                    if (order.State != EntityState.Unchanged)
                    {
                        db.Entry(order).State = EntityState.Unchanged;
                    }
                }
            }

            return db.SaveChanges();
        }

        private void Add_CRM(sales_order order)
        {
            if (order.id_sales_budget == 0 || order.id_sales_budget == null)
            {
                crm_opportunity crm_opportunity = new crm_opportunity()
                {
                    id_contact = order.id_contact,
                    id_currency = order.id_currencyfx,
                    value = order.sales_order_detail.Sum(x => x.SubTotal_Vat)
                };

                crm_opportunity.sales_order.Add(order);
                db.crm_opportunity.Add(crm_opportunity);
            }
            else
            {
                crm_opportunity crm_opportunity = db.sales_budget.Where(x => x.id_sales_budget == order.id_sales_budget).FirstOrDefault().crm_opportunity;
                crm_opportunity.sales_order.Add(order);
                db.crm_opportunity.Attach(crm_opportunity);
            }
        }

        public bool CancelAllChanges()
        {
            if (MessageBox.Show(Localize.Question_Cancel, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var entry in db.ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            {
                                entry.CurrentValues.SetValues(entry.OriginalValues);
                                entry.State = EntityState.Unchanged;
                                break;
                            }
                        case EntityState.Deleted:
                            {
                                entry.State = EntityState.Unchanged;
                                break;
                            }
                        case EntityState.Added:
                            {
                                entry.State = EntityState.Detached;
                                break;
                            }
                    }
                }
            }

            return true;
        }

        #endregion

        #region Approve

        /// <summary>
        /// Approves the Sales Order.
        /// </summary>
        /// <returns></returns>
        public bool Approve()
        {
            foreach (sales_order sales_order in db.sales_order.Local.Where(x => x.status != Status.Documents_General.Approved && x.id_contact > 0))
            {
                NumberOfRecords = 0;

                if (sales_order.status != Status.Documents_General.Approved &&
                    sales_order.IsSelected &&
                    sales_order.Error == null)
                {
                    if (sales_order.id_sales_order == 0 && sales_order.id_contact > 0)
                    {
                        SaveChanges_and_Validate();
                    }

                    sales_order.app_condition = db.app_condition.Find(sales_order.id_condition);
                    sales_order.app_contract = db.app_contract.Find(sales_order.id_contract);
                    sales_order.app_currencyfx = db.app_currencyfx.Find(sales_order.id_currencyfx);

                    if (sales_order.status != Status.Documents_General.Approved)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.insert_Schedual(sales_order);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(db, sales_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            db.payment_schedual.AddRange(payment_schedualList);
                        }

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            db.item_movement.AddRange(item_movementList);
                        }

                        if (sales_order.number == null && sales_order.id_range != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == sales_order.id_branch).FirstOrDefault().code;
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == sales_order.id_terminal).FirstOrDefault().code;

                            app_document_range app_document_range = db.app_document_range.Where(x => x.id_range == sales_order.id_range).FirstOrDefault();
                            sales_order.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            sales_order.RaisePropertyChanged("number");
                            sales_order.is_issued = true;

                            //Save Changes before Printing, so that all fields show up.
                            sales_order.status = Status.Documents_General.Approved;
                            sales_order.timestamp = DateTime.Now;
                            db.SaveChanges();

                            Brillo.Document.Start.Automatic(sales_order, app_document_range);
                        }
                        else
                        {
                            sales_order.is_issued = false;
                            sales_order.status = Status.Documents_General.Approved;
                            sales_order.timestamp = DateTime.Now;
                            db.SaveChanges();
                        }

                        //This ensures that only checked items go into requests at the time of approval.
                        if (sales_order.sales_order_detail.Where(x => x.IsSelected).Count() > 0)
                        {
                            item_request item_request = new item_request()
                            {
                                name = sales_order.contact.name,
                                comment = sales_order.comment,
                                id_sales_order = sales_order.id_sales_order,
                                id_branch = sales_order.id_branch,
                                request_date = (DateTime)sales_order.delivery_date
                            };

                            foreach (sales_order_detail data in sales_order.sales_order_detail.Where(x => x.IsSelected))
                            {
                                item_request_detail item_request_detail = new item_request_detail();
                                item_request_detail.date_needed_by = (DateTime)sales_order.delivery_date;
                                item_request_detail.id_sales_order_detail = data.id_sales_order_detail;
                                item_request_detail.urgency = entity.item_request_detail.Urgencies.Medium;
                                int ItemID = data.item.id_item;
                                item_request_detail.id_item = ItemID;
                                item item = db.items.Find(ItemID);

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
                            db.item_request.Add(item_request);
                        }

                        db.SaveChanges();
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

        #endregion

        #region Annul

        public bool Annull()
        {
            NumberOfRecords = 0;
            foreach (sales_order sales_order in db.sales_order.Local.Where(x => x.IsSelected && x.Error == null))
            {
                db.SaveChanges();

                if (sales_order.status == Status.Documents_General.Approved)
                {
                    if (sales_order.sales_invoice == null || sales_order.sales_invoice.Count == 0)
                    {
                        List<payment_schedual> payment_schedualList = new List<payment_schedual>();
                        Brillo.Logic.Payment _Payment = new Brillo.Logic.Payment();
                        payment_schedualList = _Payment.revert_Schedual(sales_order);

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.revert_Stock(db, App.Names.SalesOrder, sales_order.id_sales_order);

                        if (payment_schedualList != null && payment_schedualList.Count > 0)
                        {
                            db.payment_schedual.RemoveRange(payment_schedualList);
                        }

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            db.item_movement.RemoveRange(item_movementList);
                        }

                        sales_order.status = Status.Documents_General.Annulled;
                        db.SaveChanges();
                    }
                }

                NumberOfRecords += 1;
                sales_order.IsSelected = false;
            }

            return true;
        }

        #endregion

        #region Integrations



        #endregion

        #region Promotions

        public async void Check_Promotions(sales_order Invoice)
        {
            if (Invoice != null)
            {
                //Cleanup Code
                if (Invoice.sales_order_detail.Where(x => x.id_sales_promotion != null).ToList().Count() > 0)
                {
                    foreach (sales_order_detail sales_order_detail in Invoice.sales_order_detail.Where(x => x.id_sales_promotion != null).ToList())
                    {
                        if (sales_order_detail.id_sales_order_detail != sales_order_detail.id_sales_promotion)
                        {
                            db.sales_order_detail.Remove(sales_order_detail);
                        }
                    }
                }

                ///Promotions Code
                //Promotions.Calculate_SalesInvoice(ref Invoice);
                Invoice.RaisePropertyChanged("GrandTotal");

                //Fixup Code.
                foreach (sales_order_detail sales_order_detail in Invoice.sales_order_detail)
                {
                    //Gets the Item into Context.
                    if (sales_order_detail.item == null)
                    {
                        sales_order_detail.item = await db.items.FindAsync(sales_order_detail.id_item);
                    }

                    //Gets the Promotion into Context.
                    if (sales_order_detail.id_sales_promotion > 0 && sales_order_detail.sales_promotion == null)
                    {
                        sales_order_detail.sales_promotion = await db.sales_promotion.FindAsync(sales_order_detail.id_sales_promotion);
                    }
                }
            }
        }

        #endregion
    }
}
