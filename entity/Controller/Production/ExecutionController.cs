using System;
using System.Collections.Generic;
using System.Linq;

namespace entity.Controller.Production
{
    public class ExecutionController
    {
        db db { get; set; }

        int NumberOfRecords = 0;

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            db = new db();
        }

        public bool Create()
        {

            return true;
        }

        public bool Delete()
        {

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
                            db.SaveChanges();
                        }

                        production_order_detail.status = Status.Production.Executed;
                        production_order_detail.RaisePropertyChanged("status");
                        production_order_detail.State = db.EntityState.Modified;

                        production_execution_detail.State = db.EntityState.Modified;
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

                        if (Type == entity.production_order.ProductionOrderTypes.Fraction)
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
    }
}
