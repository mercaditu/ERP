using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Controller.Production
{
    public class ExecutionController : Base
    {

        public async void Load(production_order.ProductionOrderTypes ProductionOrderTypes)
        {
            if (ProductionOrderTypes == production_order.ProductionOrderTypes.Fraction)
            {
                await db.production_order.Where(a => a.id_company == CurrentSession.Id_Company && a.type == ProductionOrderTypes && a.production_line.app_location.id_branch == CurrentSession.Id_Branch).OrderByDescending(x => x.trans_date).LoadAsync();
            }
            else
            {
                await db.production_order.Where(a => a.id_company == CurrentSession.Id_Company && (a.type == ProductionOrderTypes || a.type==production_order.ProductionOrderTypes.Internal) && a.production_line.app_location.id_branch == CurrentSession.Id_Branch).OrderByDescending(x => x.trans_date).LoadAsync();
            }

            await db.production_line.Where(x => x.id_company == CurrentSession.Id_Company && x.app_location.id_branch == CurrentSession.Id_Branch).LoadAsync();
            await db.hr_time_coefficient.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
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
                        production_order_detail.State = System.Data.Entity.EntityState.Modified;

                        production_execution_detail.State = System.Data.Entity.EntityState.Modified;
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

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;


            foreach (var error in db.GetValidationErrors())
            {
                db.Entry(error.Entry.Entity).State = EntityState.Detached;
            }

            db.SaveChanges();
            foreach (production_execution_detail production_execution_detail in db.production_execution_detail.Local)
            {
                production_execution_detail.State = EntityState.Unchanged;
            }
            return true;
        }
    }
}
