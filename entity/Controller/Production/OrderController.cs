using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace entity.Controller.Production
{
    public class OrderController : Base
    {

        public int Count { get; set; }

        public int PageSize { get { return _PageSize; } set { _PageSize = value; } }
        public int _PageSize = 100;


        public int PageCount
        {
            get
            {
                return (Count / PageSize) < 1 ? 1 : (Count / PageSize);
            }
        }


        public async void Load(production_order.ProductionOrderTypes ProductionOrderTypes, int PageIndex)
        {
            var predicate = PredicateBuilder.True<production_order>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.production_line.app_location.id_branch == CurrentSession.Id_Branch);


            if (ProductionOrderTypes==production_order.ProductionOrderTypes.Fraction)
            {
                predicate = predicate.And(x => x.type == ProductionOrderTypes);
                if (Count == 0)
                {
                    Count = db.production_order.Where(predicate).Count();
                }
                await db.production_order.Where(predicate).Include(x=>x.production_order_detail).OrderByDescending(x => x.trans_date)
                    .Skip(PageIndex * PageSize).Take(PageSize).LoadAsync();
            }
            else
            {
                predicate = predicate.And(x => (x.type == ProductionOrderTypes || x.type == production_order.ProductionOrderTypes.Internal));
                if (Count == 0)
                {
                    Count = db.production_order.Where(predicate).Count();
                }
                await db.production_order.Where(predicate).OrderByDescending(x => x.trans_date)
                    .Skip(PageIndex * PageSize).Take(PageSize).LoadAsync();
            }

         
            await db.production_line.Where(x => x.id_company == CurrentSession.Id_Company && x.app_location.id_branch == CurrentSession.Id_Branch).LoadAsync();
            await db.hr_time_coefficient.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.items.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
        }

        public production_order Create_Fraction(int Line)
        {
            production_order Order = new production_order()
            {
                State = EntityState.Added,
                status = Status.Production.Pending,
                type = production_order.ProductionOrderTypes.Fraction,
                IsSelected = true
            };

            db.production_order.Add(Order);

            return Order;
        }

        public production_order Create_Normal(int Line)
        {
            production_order Order = new production_order()
            {
                State = EntityState.Added,
                status = Status.Production.Pending,
                type = production_order.ProductionOrderTypes.Production,
                IsSelected = true
            };

            db.production_order.Add(Order);

            return Order;
        }

        public void CreateDetail()
        {

        }

        public bool Edit(production_order Order)
        {
            Order.IsSelected = true;
            Order.State = EntityState.Modified;
            db.Entry(Order).State = EntityState.Modified;

            return true;
        }

        public void EditDetail(production_order_detail Detail)
        {

        }

        public bool Archive()
        {
            MessageBoxResult res = MessageBox.Show(Brillo.Localize.Question_Archive, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                foreach (production_order Order in db.production_order.Local.Where(x => x.IsSelected))
                {
                    Order.is_archived = true;
                }
            }

            db.SaveChanges();
            return true;
        }

        public bool Approve(production_order.ProductionOrderTypes Type)
        {
            foreach (production_order_detail production_order_detail in db.production_order_detail.Local.Where(x => x.IsSelected && x.status == Status.Production.Approved).OrderByDescending(x => x.is_input))
            {
                if (production_order_detail.production_order != null)
                {
                    foreach (production_execution_detail production_execution_detail in production_order_detail.production_execution_detail.Where(x => x.status == null || x.status < Status.Production.Approved))
                    {
                        ///Assign this so that inside Stock Brillo we can run special logic required for Production or Fraction.
                        ///Production: Sums all input Childs to the Cost.
                        ///Fraction: Takes a Fraction of the parent.
                        ///TODO: Fraction only takes cost of parent. We need to include other things as well.

                        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                        List<item_movement> item_movementList = new List<item_movement>();
                        item_movementList = _Stock.insert_Stock(db, production_execution_detail);

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            db.item_movement.AddRange(item_movementList);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                                throw ex;
                            }
                            
                        }

                        production_order_detail.status = Status.Production.Executed;
                        production_order_detail.RaisePropertyChanged("status");
                        production_order_detail.State = EntityState.Modified;

                        production_execution_detail.State = EntityState.Modified;
                        production_execution_detail.status = Status.Production.Executed;

                        if (production_execution_detail.project_task != null)
                        {
                            production_execution_detail.project_task.status = Status.Project.Executed;
                        }
                        else
                        {
                            if (production_execution_detail.id_project_task != null && production_execution_detail.id_project_task > 0)
                            {
                                project_task project_task = db.project_task.Where(x => x.id_project_task == production_execution_detail.id_project_task).FirstOrDefault();
                                if (project_task != null)
                                {
                                    project_task.status = Status.Project.Executed;
                                }
                            }
                        }

                        if (Type == production_order.ProductionOrderTypes.Fraction)
                        {
                            if (production_execution_detail != null && production_execution_detail.production_order_detail != null && production_execution_detail.production_order_detail.production_order != null && production_execution_detail.production_order_detail.production_order.app_document_range != null)
                            {
                                Brillo.Document.Start.Automatic(production_execution_detail, production_execution_detail.production_order_detail.production_order.app_document_range);
                            }

                        }
                        NumberOfRecords += 1;
                    }
                }
            }

            db.SaveChanges();
            return true;
        }

        public void ApproveDetail()
        {

        }

        public void AnnullDetail()
        {

        }

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (production_order production_order in db.production_order.Local.Where(x => x.id_production_line > 0))
            {
                if (production_order.IsSelected && production_order.Error == null)
                {
                    if (production_order.State == EntityState.Added)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        db.Entry(production_order).State = EntityState.Added;
                    }
                    else if (production_order.State == EntityState.Modified)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        db.Entry(production_order).State = EntityState.Modified;
                    }
                    else if (production_order.State == EntityState.Deleted)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        db.production_order.Remove(production_order);
                    }
                    NumberOfRecords += 1;
                }
                else if (production_order.State > 0)
                {
                    if (production_order.State != EntityState.Unchanged)
                    {
                        db.Entry(production_order).State = EntityState.Unchanged;
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
    }
}
