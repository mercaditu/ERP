using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class ExecutionDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Execution();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Execution();
            return base.SaveChangesAsync();
        }

        private void validate_Execution()
        {
            foreach (production_execution production_execution in base.production_execution.Local)
            {
                if (production_execution.IsSelected && production_execution.Error == null)
                {
                    if (production_execution.State == EntityState.Added)
                    {
                        production_execution.timestamp = DateTime.Now;
                        production_execution.State = EntityState.Unchanged;
                        Entry(production_execution).State = EntityState.Added;
                    }
                    else if (production_execution.State == EntityState.Modified)
                    {
                        production_execution.timestamp = DateTime.Now;
                        production_execution.State = EntityState.Unchanged;
                        Entry(production_execution).State = EntityState.Modified;
                    }
                    else if (production_execution.State == EntityState.Deleted)
                    {
                        production_execution.timestamp = DateTime.Now;
                        production_execution.State = EntityState.Unchanged;
                        base.production_execution.Remove(production_execution);
                    }
                }
                else if (production_execution.State > 0)
                {
                    if (production_execution.State != EntityState.Unchanged)
                    {
                        Entry(production_execution).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve()
        {
            foreach (production_execution production_execution in base.production_execution.Local.Where(x =>
                                                                  x.IsSelected && x.Error == null))
            {
                if (production_execution.id_production_execution == 0)
                {
                    SaveChanges();
                }
                foreach (production_execution_detail production_execution_detail in production_execution.production_execution_detail)
                {
                    if (production_execution_detail.production_order_detail != null)
                    {

                        if (production_execution_detail.production_order_detail.status == Status.Production.Approved)
                        {
                            if (production_execution_detail.production_order_detail.production_order != null)
                            {
                                //Logic
                                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                                List<item_movement> item_movementList = new List<item_movement>();
                                item_movementList = _Stock.insert_Stock(this, production_execution_detail);

                                if (item_movementList != null && item_movementList.Count > 0)
                                {
                                    item_movement.AddRange(item_movementList);
                                }
                                production_execution_detail.production_order_detail.status = Status.Production.Executed;
                                production_execution_detail.production_order_detail.RaisePropertyChanged("status");
                               production_execution_detail.production_order_detail.State = EntityState.Modified;

                                production_execution_detail.State = EntityState.Modified;
                                production_execution_detail.status = Status.Production.Executed;

                                if (production_execution_detail.project_task != null)
                                {
                                    production_execution_detail.project_task.status = Status.Project.Executed;
                                }
                            }
                        }
                    }
                   



                    production_execution_detail.State = EntityState.Unchanged;

                }
            }

            SaveChanges();
        }

        public void Anull()
        {
            foreach (production_execution production_execution in base.production_execution.Local.Where(x =>
                                                                  x.IsSelected && x.Error == null))
            {
                Brillo.Logic.Stock _Stock = new Brillo.Logic.Stock();
                List<item_movement> item_movementList = new List<item_movement>();
                item_movementList = _Stock.revert_Stock(this, App.Names.ProductionExecution, production_execution);

                if (item_movementList != null && item_movementList.Count > 0)
                {
                    item_movement.RemoveRange(item_movementList);
                }
                production_execution.status = Status.Documents_General.Annulled;
            }
        }

    }
}
