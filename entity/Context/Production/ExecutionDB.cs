using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class ExecutionDB : BaseDB
    {
        public production_order New(string name, production_order.ProductionOrderTypes Type, int Line)
        {
            production_order production_order = new production_order();
            production_order.id_production_line = Line;
            production_order.type = Type;
            production_order.trans_date = DateTime.Now;
            production_order.status = Status.Production.Pending;
            production_order.name = name;
            return production_order;
        }

        public override int SaveChanges()
        {
            validate_Execution();
            return base.SaveChanges();
        }

        public int SaveChangesWithoutValidation()
        {
            // validate_Execution();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Execution();
            return base.SaveChangesAsync();
        }

        private void validate_Execution()
        {
            foreach (production_order production_order in base.production_order.Local)
            {
                if (production_order.IsSelected && production_order.Error == null)
                {
                    if (production_order.State == EntityState.Added)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        Entry(production_order).State = EntityState.Added;
                    }
                    else if (production_order.State == EntityState.Modified)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        Entry(production_order).State = EntityState.Modified;
                    }
                    else if (production_order.State == EntityState.Deleted)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        base.production_order.Remove(production_order);
                    }
                }
                else if (production_order.State > 0)
                {
                    if (production_order.State != EntityState.Unchanged)
                    {
                        Entry(production_order).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public int Approve(production_order.ProductionOrderTypes Type)
        {
            foreach (production_order_detail production_order_detail in base.production_order_detail.Local.Where(x => x.IsSelected && x.status == Status.Production.Approved).OrderByDescending(x => x.is_input))
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
                        item_movementList = _Stock.insert_Stock(this, production_execution_detail);

                        if (item_movementList != null && item_movementList.Count > 0)
                        {
                            base.item_movement.AddRange(item_movementList);
                            base.SaveChanges();
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
                                project_task project_task = base.project_task.Where(x => x.id_project_task == production_execution_detail.id_project_task).FirstOrDefault();
                                if (project_task != null)
                                {
                                    project_task.status = Status.Project.Executed;
                                }
                            }
                        }

                        if (Type==entity.production_order.ProductionOrderTypes.Fraction)
                        {
                            if (production_execution_detail!=null && production_execution_detail.production_order_detail!=null && production_execution_detail.production_order_detail.production_order!=null && production_execution_detail.production_order_detail.production_order.app_document_range!=null)
                            {
                                Brillo.Document.Start.Automatic(production_execution_detail, production_execution_detail.production_order_detail.production_order.app_document_range);
                            }
                          
                        }
                        NumberOfRecords += 1;
                    }
                }
            }

            base.SaveChanges();
            return NumberOfRecords;
        }

        //public void Anull()
        //{
        //    foreach (production_execution production_execution in base.production_execution.Local.Where(x =>
        //                                                          x.IsSelected && x.Error == null))
        //    {
        //        Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
        //        List<item_movement> item_movementList = new List<item_movement>();
        //        item_movementList = _Stock.revert_Stock(this, App.Names.ProductionExecution, production_execution);

        //        if (item_movementList != null && item_movementList.Count > 0)
        //        {
        //            item_movement.RemoveRange(item_movementList);
        //        }

        //        production_execution.status = Status.Documents_General.Annulled;
        //    }
        //}
    }
}