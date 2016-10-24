using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public partial class item_requestDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Item_Request();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Item_Request();
            return base.SaveChangesAsync();
        }

        private void validate_Item_Request()
        {
            foreach (item_request item_request in base.item_request.Local)
            {
                if (item_request.IsSelected)
                {
                    if (item_request.State == EntityState.Added)
                    {
                        item_request.timestamp = DateTime.Now;
                        item_request.State = EntityState.Unchanged;
                        Entry(item_request).State = EntityState.Added;
                    }
                    else if (item_request.State == EntityState.Modified)
                    {
                        item_request.timestamp = DateTime.Now;
                        item_request.State = EntityState.Unchanged;
                        Entry(item_request).State = EntityState.Modified;
                    }
                    else if (item_request.State == EntityState.Deleted)
                    {
                        item_request.timestamp = DateTime.Now;
                        item_request.State = EntityState.Unchanged;
                        base.item_request.Remove(item_request);
                    }
                }
                else if (item_request.State > 0)
                {
                    if (item_request.State != EntityState.Unchanged)
                    {
                        Entry(item_request).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve()
        {
            OrderDB orderdb = new OrderDB();
            foreach (item_request item_request in base.item_request.Local.Where(x =>
                                                x.status != Status.Documents_General.Approved
                                                        && x.IsSelected))
            {
                item_transfer item_transfer = new item_transfer();
                item_transfer.status = Status.Transfer.Pending;
               // entity.Properties.Settings setting = new Properties.Settings();
                item_transfer.user_requested = base.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
                item_transfer.id_item_request = item_request.id_item_request;
                if (base.app_department.FirstOrDefault() != null)
                {
                    item_transfer.id_department = base.app_department.FirstOrDefault().id_department;
                }

                if (base.app_document_range.Where(x => x.app_document.id_application == App.Names.Movement).FirstOrDefault() != null)
                {
                    item_transfer.id_range = base.app_document_range.Where(x => x.app_document.id_application == App.Names.Movement).FirstOrDefault().id_range;
                }

                item_transfer item_transfertrans = new item_transfer();
                item_transfertrans.status = Status.Transfer.Pending;

                item_transfertrans.user_requested = base.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
                item_transfertrans.id_item_request = item_request.id_item_request;
                if (base.app_department.FirstOrDefault() != null)
                {
                    item_transfertrans.id_department = base.app_department.FirstOrDefault().id_department;
                }

                if (base.app_document_range.Where(x => x.app_document.id_application == App.Names.Movement).FirstOrDefault() != null)
                {
                    item_transfertrans.id_range = base.app_document_range.Where(x => x.app_document.id_application == App.Names.Movement).FirstOrDefault().id_range;
                }

                purchase_tender purchase_tender = new purchase_tender();
                purchase_tender.status = Status.Documents_General.Pending;
                purchase_tender.id_department = item_request.id_department;

                purchase_tender.name = item_request.name;
                purchase_tender.code = 000;
                purchase_tender.trans_date = item_request.request_date;
                purchase_tender.comment = item_request.comment;

                int Line = 0;
                if (production_line.FirstOrDefault() != null)
                {
                    Line = production_line.FirstOrDefault().id_production_line;
                }

                production_order production_order = orderdb.New(item_request.name, production_order.ProductionOrderTypes.Fraction, Line);

                production_order.id_project = item_request.id_project;

                // production_execution production_execution = new production_execution();


                foreach (item_request_detail item_request_detail in item_request.item_request_detail)
                {
                    foreach (item_request_decision item in item_request_detail.item_request_decision)
                    {
                        if ((item_request.number == null || item_request.number == string.Empty) && item_request.id_range > 0)
                        {
                            if (item_request.id_branch > 0)
                            {
                                Brillo.Logic.Range.branch_Code = app_branch.Where(x => x.id_branch == item_request.id_branch).FirstOrDefault().code;
                            }


                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == item_request.id_range).FirstOrDefault();


                            item_request.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            item_request.RaisePropertyChanged("number");
                           
                        }

                        //if (item.IsSelected == true)
                        //{
                        if (item.decision == entity.item_request_decision.Decisions.Movement)
                        {
                            //  ProductTransferDB ProductTransferDB = new entity.ProductTransferDB();
                            //  item_transfer _item_transfer = new entity.item_transfer();
                            item_transfer.status = Status.Transfer.Pending;
                            item_transfer.IsSelected = true;

                            item_transfer.State = EntityState.Added;
                            item_transfer.user_requested = base.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
                            item_transfer.id_item_request = item_request.id_item_request;
                            if (base.app_department.FirstOrDefault() != null)
                            {
                                item_transfer.id_department = base.app_department.FirstOrDefault().id_department;
                            }
                            if (base.app_document_range.Where(x => x.app_document.id_application == App.Names.Movement).FirstOrDefault() != null)
                            {
                                item_transfer.id_range = base.app_document_range.Where(x => x.app_document.id_application == App.Names.Movement).FirstOrDefault().id_range;
                            }

                            int id_location = (int)item.id_location;

                            app_location app_location = base.app_location.Where(x => x.id_location == id_location && x.is_default).FirstOrDefault();
                            item_transfer.app_location_origin = app_location;
                            item_transfer.app_branch_origin = app_location.app_branch;

                            item_transfer.comment = "Transfer item Request from " + item.decision.ToString();


                            //Create Transfer Detail in DB.
                            item_transfer_detail item_transfer_detail = new item_transfer_detail();
                            item_transfer_detail.id_item_product = item_request_detail.item.item_product.FirstOrDefault().id_item_product;
                            item_transfer_detail.item_product = base.item_product.Where(x => x.id_item_product == item_transfer_detail.id_item_product).FirstOrDefault();
                            item_transfer_detail.quantity_origin = item.quantity;
                            item_transfer_detail.quantity_destination = item.quantity;
                            item_transfer_detail.movement_id = item.movement_id;

                            //PROJECT
                            if (item_request_detail.id_project_task != null)
                            {
                                //Transfer related to Project because there is a Project.
                                item_transfer_detail.id_project_task = item_request_detail.id_project_task;

                                int id_branch = (int)base.projects.Where(x => x.id_project == item_request_detail.project_task.id_project).FirstOrDefault().id_branch;
                                item_transfer.app_location_destination = base.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault().app_location.Where(x => x.is_default).FirstOrDefault();
                                item_transfer.app_branch_destination = base.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault();
                                item_transfer.id_project = item_request_detail.project_task.id_project;
                            }

                            //SALES ORDER
                            if (item_request_detail.id_sales_order_detail != null)
                            {
                                item_transfer.app_location_destination = base.app_branch.Where(x => x.id_branch == item_request.sales_order.app_branch.id_branch).FirstOrDefault().app_location.Where(x => x.is_default).FirstOrDefault();
                                item_transfer.app_branch_destination = base.app_branch.Where(x => x.id_branch == item_request.sales_order.app_branch.id_branch).FirstOrDefault();
                            }

                            //PRODUCTION ORDER
                            if (item_request_detail.id_order_detail != null)
                            {
                                //Get Production Line
                                int id_production_line = base.production_order_detail.Where(x => x.id_order_detail == item_request_detail.id_order_detail).FirstOrDefault().production_order.id_production_line;
                                //Get Location based on Line
                                app_location app_location_Destination = base.production_line.Where(x => x.id_production_line == id_production_line).FirstOrDefault().app_location;
                                item_transfer.app_location_destination = app_location_Destination;
                                //Get Branch based on Location
                                item_transfer.app_branch_destination = base.app_branch.Where(x => x.id_branch == app_location_Destination.id_branch).FirstOrDefault();
                            }

                            foreach (item_request_dimension item_request_dimension in item_request_detail.item_request_dimension)
                            {
                                item_transfer_dimension item_transfer_dimension = new item_transfer_dimension();
                                item_transfer_dimension.id_dimension = item_request_dimension.id_dimension;
                                item_transfer_dimension.value = item_request_dimension.value;
                                item_transfer_detail.item_transfer_dimension.Add(item_transfer_dimension);
                            }

                            item_transfer.item_transfer_detail.Add(item_transfer_detail);
                            item_transfer.transfer_type = item_transfer.Transfer_type.movemnent;

                            base.item_transfer.Add(item_transfer);
                        }

                        else if (item.decision == entity.item_request_decision.Decisions.Transfer)
                        {
                            int id_location = (int)item.id_location;
                            item_transfertrans.app_location_origin = base.app_location.Where(x => x.id_location == id_location).FirstOrDefault();
                            item_transfertrans.app_branch_origin = base.app_location.Where(x => x.id_location == id_location).FirstOrDefault().app_branch;
                            item_transfertrans.comment = "Transfer item Request from " + item.decision.ToString();
                            if (item_request_detail.id_project_task != null)
                            {
                                int id_branch = (int)base.projects.Where(x => x.id_project == item_request_detail.project_task.id_project).FirstOrDefault().id_branch;
                                item_transfertrans.app_location_destination = base.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault().app_location.Where(x => x.is_default).FirstOrDefault();
                                item_transfertrans.app_branch_destination = base.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault();
                                item_transfertrans.id_project = item_request_detail.project_task.id_project;
                            }
                            if (item_request_detail.id_sales_order_detail != null)
                            {

                                item_transfertrans.app_location_destination = item_request.sales_order.app_branch.app_location.Where(x => x.is_default).FirstOrDefault();
                                item_transfertrans.app_branch_destination = item_request.sales_order.app_branch;
                            }
                            if (item_request_detail.id_order_detail != null)
                            { 
                                //Get Production Line
                                int id_production_line = base.production_order_detail.Where(x => x.id_order_detail == item_request_detail.id_order_detail).FirstOrDefault().production_order.id_production_line;
                                //Get Location based on Line
                                app_location app_location_Destination = base.production_line.Where(x => x.id_production_line == id_production_line).FirstOrDefault().app_location;
                                item_transfertrans.app_location_destination = app_location_Destination;
                                //Get Branch based on Location
                                item_transfertrans.app_branch_destination = base.app_branch.Where(x => x.id_branch == app_location_Destination.id_branch).FirstOrDefault();
                            }

                            item_transfer_detail item_transfer_detail = new item_transfer_detail();
                            item_transfer_detail.id_item_product = item_request_detail.item.item_product.FirstOrDefault().id_item_product;
                            if (item_request_detail.project_task != null)
                            {
                                item_transfer_detail.id_project_task = item_request_detail.project_task.id_project_task;
                            }


                            foreach (item_request_dimension item_request_dimension in item_request_detail.item_request_dimension)
                            {
                                item_transfer_dimension item_transfer_dimension = new item_transfer_dimension();
                                item_transfer_dimension.id_dimension = item_request_dimension.id_dimension;
                                item_transfer_dimension.value = item_request_dimension.value;
                                item_transfer_detail.item_transfer_dimension.Add(item_transfer_dimension);
                            }
                            item_transfer_detail.quantity_origin = item.quantity;
                            item_transfer_detail.quantity_destination = item.quantity;
                            item_transfer_detail.movement_id = item.movement_id;
                            item_transfertrans.item_transfer_detail.Add(item_transfer_detail);

                            item_transfertrans.transfer_type = entity.item_transfer.Transfer_type.transfer;

                        }
                        else if (item.decision == entity.item_request_decision.Decisions.Production)
                        {
                            production_order_detail production_order_detail = new production_order_detail();
                            production_order_detail.name = item_request_detail.item.name;
                            production_order_detail.quantity = item.quantity;
                            production_order_detail.status = Status.Production.Pending;
                            production_order_detail.is_input = false;
                            production_order_detail.id_item = item_request_detail.item.id_item;

                            if (item.item_request_detail.id_project_task != null)
                            {
                                production_order_detail.id_project_task = item.item_request_detail.id_project_task;

                            }

                            foreach (item_request_dimension item_request_dimension in item_request_detail.item_request_dimension)
                            {
                                production_order_dimension production_order_dimension = new production_order_dimension();
                                production_order_dimension.id_dimension = item_request_dimension.id_dimension;
                                production_order_dimension.id_measurement = item_request_dimension.id_measurement;
                                production_order_dimension.value = item_request_dimension.value;
                                production_order_detail.production_order_dimension.Add(production_order_dimension);
                            }

                            production_order.production_order_detail.Add(production_order_detail);

                            //production_execution.production_order = production_order;
                            //production_execution.id_production_line = production_order.id_production_line;
                            //production_execution.trans_date = DateTime.Now;
                        }
                        else if (item.decision==entity.item_request_decision.Decisions.Internal)
                        {
                            List<entity.Brillo.StockList> Items_InStockLIST;
                            entity.Brillo.Stock stockBrillo = new entity.Brillo.Stock();
                            item_product item_product = item.item_request_detail.item.item_product.FirstOrDefault();
                            app_location app_location = item_request.app_branch.app_location.Where(x => x.is_default).FirstOrDefault();
                            if (item_product != null && app_location != null)
                            {
                                Items_InStockLIST = stockBrillo.List(item_request.app_branch, item_request.app_branch.app_location.Where(x=>x.is_default).FirstOrDefault(), item.item_request_detail.item.item_product.FirstOrDefault());
                                entity.Brillo.Logic.Stock stock = new entity.Brillo.Logic.Stock();
                                List<item_movement> item_movement_originList;
                                item_movement_originList = stock.DebitOnly_MovementLIST(this, Items_InStockLIST, Status.Stock.InStock, entity.App.Names.Movement, item_request.id_item_request, item.item_request_detail.id_item_request_detail,
                                CurrentSession.Get_Currency_Default_Rate(),
                                    item_product, app_location,
                                        item.quantity, item_request.timestamp, stock.comment_Generator(entity.App.Names.RequestResource, item_request.number != null ? item_request.number.ToString() : "", ""));

                                base.item_movement.AddRange(item_movement_originList);
                            }
                          
                        }
                        else
                        {

                            if (item_request_detail.id_project_task != null)
                            {
                                if (base.projects.Where(x => x.id_project == item_request_detail.project_task.id_project).FirstOrDefault().id_branch != null)
                                {
                                    int id_branch = (int)base.projects.Where(x => x.id_project == item_request_detail.project_task.id_project).FirstOrDefault().id_branch;
                                    purchase_tender.app_branch = base.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault();
                                }
                                else
                                {
                                    purchase_tender.app_branch = base.app_branch.Where(x => x.can_invoice == true && x.can_stock == true).FirstOrDefault();
                                }

                            }
                            if (item_request_detail.id_sales_order_detail != null)
                            {

                                purchase_tender.app_branch = item_request.sales_order.app_branch;
                            }

                            if (item_request_detail.id_order_detail != null)
                            {
                                if (base.production_order_detail.Where(x => x.id_order_detail == item_request_detail.id_order_detail).FirstOrDefault() != null)
                                {
                                    if (base.production_order_detail.Where(x => x.id_order_detail == item_request_detail.id_order_detail).FirstOrDefault().project_task != null)
                                    {
                                        int id_project = base.production_order_detail.Where(x => x.id_order_detail == item_request_detail.id_order_detail).FirstOrDefault().project_task.id_project;
                                        if (base.projects.Where(x => x.id_project == id_project).FirstOrDefault() != null)
                                        {
                                            int id_branch = (int)base.projects.Where(x => x.id_project == id_project).FirstOrDefault().id_branch;

                                            if (base.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault() != null)
                                            {
                                                purchase_tender.id_project = item_request_detail.project_task.id_project;
                                                purchase_tender.app_branch = base.app_branch.Where(x => x.id_branch == id_branch).FirstOrDefault();
                                            }

                                        }


                                    }
                                }
                            }
                            else
                            {
                                purchase_tender.app_branch = base.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault();
                            }


                            purchase_tender_item purchase_tender_item = new purchase_tender_item();
                            purchase_tender_item.id_item = item_request_detail.id_item;
                            purchase_tender_item.item_description = item_request_detail.comment;
                            purchase_tender_item.quantity = item.quantity;


                            foreach (item_request_dimension item_request_dimension in item_request_detail.item_request_dimension)
                            {
                                purchase_tender_dimension purchase_tender_dimension = new purchase_tender_dimension();
                                purchase_tender_dimension.id_dimension = item_request_dimension.id_dimension;
                                purchase_tender_dimension.id_measurement = item_request_dimension.id_measurement;
                                purchase_tender_dimension.value = item_request_dimension.value;
                                purchase_tender_item.purchase_tender_dimension.Add(purchase_tender_dimension);
                            }

                            purchase_tender.purchase_tender_item_detail.Add(purchase_tender_item);




                        }
                        item_request.status = Status.Documents_General.Approved;

                        if ((item_request.number == null || item_request.number == string.Empty) && item_request.id_range > 0)
                        {
                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == item_request.id_range).FirstOrDefault();
                            Brillo.Document.Start.Automatic(item_request, app_document_range);
                        }

                    }
                }
                if (purchase_tender.purchase_tender_item_detail.Count() > 0)
                {
                    base.purchase_tender.Add(purchase_tender);
                }
                if (item_transfer.item_transfer_detail.Count() > 0)
                {
                    base.item_transfer.Add(item_transfer);
                }
                if (item_transfertrans.item_transfer_detail.Count() > 0)
                {
                    base.item_transfer.Add(item_transfertrans);
                }
                if (production_order.production_order_detail.Count() > 0)
                {
                    orderdb.production_order.Add(production_order);
                    //orderdb.production_execution.Add(production_execution);
                }
            }
            orderdb.SaveChanges();
           
            SaveChanges();
        }
    }
}
