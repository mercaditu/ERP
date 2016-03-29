using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
namespace entity
{
    public partial class ProductTransferDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_ProductTransfer();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_ProductTransfer();
            return base.SaveChangesAsync();
        }

        private void validate_ProductTransfer()
        {
            foreach (item_transfer item_transfer in base.item_transfer.Local)
            {
                if (item_transfer.IsSelected)
                // && item_transfer.Error == null)
                {
                    if (item_transfer.State == EntityState.Added)
                    {
                        item_transfer.timestamp = DateTime.Now;
                        item_transfer.State = EntityState.Unchanged;
                        Entry(item_transfer).State = EntityState.Added;
                    }
                    else if (item_transfer.State == EntityState.Modified)
                    {
                        item_transfer.timestamp = DateTime.Now;
                        item_transfer.State = EntityState.Unchanged;
                        Entry(item_transfer).State = EntityState.Modified;
                    }
                    else if (item_transfer.State == EntityState.Deleted)
                    {
                        item_transfer.timestamp = DateTime.Now;
                        item_transfer.State = EntityState.Unchanged;
                        base.item_transfer.Remove(item_transfer);
                    }
                }
                else if (item_transfer.State > 0)
                {
                    if (item_transfer.State != EntityState.Unchanged)
                    {
                        Entry(item_transfer).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void ApproveOrigin(int origin,int dest,bool movebytruck)
        {
            entity.Brillo.Logic.Stock stock = new Brillo.Logic.Stock();

            foreach (item_transfer item_transfer in base.item_transfer.Local)
            {
                if (item_transfer.IsSelected)
                {

                    foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail)
                    {


                        if (item_transfer_detail.status != Status.Documents_General.Approved)
                        {

                            if (item_transfer_detail.item_product != null)
                            {
                                List<item_movement> item_movement_originList;
                                app_currencyfx app_currencyfx = base.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                                app_location app_location = base.app_location.Where(x => x.id_branch == origin).FirstOrDefault();
                                List<item_movement> Items_InStockLIST = base.item_movement.Where(x => x.id_location == app_location.id_location
                                                                      && x.id_item_product == item_transfer_detail.id_item_product
                                                                      && x.status == entity.Status.Stock.InStock
                                                                      && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();
                                item_movement_originList = stock.Debit_MovementLIST(Items_InStockLIST, Status.Stock.InStock, App.Names.Transfer, 0, app_currencyfx, item_transfer_detail.item_product, app_location,
                                      item_transfer_detail.quantity_origin, item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(App.Names.Transfer, "", ""));
                                base.item_movement.AddRange(item_movement_originList);
                                if (movebytruck)
                                {
                                    item_movement item_movement_Dest;
                                    app_currencyfx app_currencyfxdest = base.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                                    app_location app_locationdest = base.app_location.Where(x => x.id_branch == dest).FirstOrDefault();


                                    item_movement_Dest = stock.Credit_Movement(Status.Stock.InStock, App.Names.Transfer, 0, app_currencyfxdest.id_currencyfx,
                                        item_transfer_detail.item_product, app_locationdest.id_location,
                                          item_transfer_detail.quantity_origin, item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(App.Names.Transfer, "", ""));
                                    base.item_movement.Add(item_movement_Dest);
                                }
                                item_transfer_detail.status = Status.Documents_General.Approved;
                            }
                        }
                    }

                }
            }

            try 
            { 
                base.SaveChanges(); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
        public void ApproveDestination(decimal cost,int origin,int dest,bool MoveByTruck)
        {
            foreach (item_transfer item_transfer in base.item_transfer.Local)
            {
                if (item_transfer.IsSelected)
                {
                    foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail)
                    {
                        if (item_transfer.status != Status.Documents_General.Approved)
                        {
                            if (item_transfer_detail.item_product != null)
                            {
                                entity.Brillo.Logic.Stock stock = new Brillo.Logic.Stock();

                                if (MoveByTruck)
                                {
                                    item_movement item_movement_origin;
                                    item_movement item_movement_destination;
                                    app_currencyfx app_currencyfx = base.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                                    app_location app_location = base.app_location.Where(x => x.id_branch == origin).FirstOrDefault();

                                    ///Discount From Destination. Because merchendice is returned to Origin, so it must be discounted from Destintation.
                                    item_movement_destination = stock.Debit_Movement(
                                          Status.Stock.InStock,
                                          App.Names.Transfer,
                                          0,
                                          item_transfer_detail.item_product,
                                          app_location,
                                          item_transfer_detail.quantity_destination,
                                          item_transfer_detail.item_transfer.trans_date,
                                          stock.comment_Generator(App.Names.Transfer, item_transfer.number, item_transfer.app_branch_origin.name + " <= " + item_transfer.app_branch_destination.name)
                                    );

                                    ///Add it to Origin.
                                    item_movement_origin = stock.Credit_Movement(
                                          Status.Stock.InStock, 
                                          App.Names.Transfer, 
                                          0, 
                                          app_currencyfx.id_currencyfx, 
                                          item_transfer_detail.item_product, 
                                          app_location.id_location,
                                          item_transfer_detail.quantity_destination, 
                                          item_transfer_detail.item_transfer.trans_date, 
                                          stock.comment_Generator(App.Names.Transfer, item_transfer.number, item_transfer.app_branch_origin.name + " => " + item_transfer.app_branch_destination.name)
                                    );

                                    base.item_movement.Add(item_movement_destination);
                                    base.item_movement.Add(item_movement_origin);
                                }
                                else
                                {
                                    item_movement item_movement_Dest;
                                    app_currencyfx app_currencyfx = base.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                                    app_location app_location = base.app_location.Where(x => x.id_branch == dest).FirstOrDefault();

                                    item_movement_Dest = 
                                        stock.Credit_Movement(
                                            Status.Stock.InStock, 
                                            App.Names.Transfer, 
                                            0, 
                                            app_currencyfx.id_currencyfx, 
                                            item_transfer_detail.item_product, 
                                            app_location.id_location,
                                            item_transfer_detail.quantity_destination, 
                                            item_transfer_detail.item_transfer.trans_date, 
                                            stock.comment_Generator(App.Names.Transfer, "", "")
                                            );

                                    if (item_transfer_detail.quantity_destination == 0)
                                    {
                                        item_movement_value item_movement_value = new item_movement_value();
                                        item_movement_value.unit_value = cost / item_transfer_detail.quantity_destination;
                                        item_movement_value.id_currencyfx = 0;
                                        item_movement_value.comment = String.Format("Transaction from transfer");
                                        item_movement_Dest.item_movement_value.Add(item_movement_value);
                                    }
                                    base.item_movement.Add(item_movement_Dest);
                                }
                               
                                item_transfer.status = Status.Documents_General.Approved;
                            }

                            if ((item_transfer.number == null || item_transfer.number == string.Empty) && item_transfer.id_range > 0)
                            {

                                if (base.app_branch.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault() != null)
                                {
                                    Brillo.Logic.Range.branch_Code = base.app_branch.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault().code;
                                }
                                if (base.app_terminal.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault() != null)
                                {
                                    Brillo.Logic.Range.terminal_Code = base.app_terminal.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault().code;
                                }
                                if (base.security_user.Where(x => x.id_user == item_transfer.id_user).FirstOrDefault() != null)
                                {
                                    Brillo.Logic.Range.user_Code = base.security_user.Where(x => x.id_user == item_transfer.id_user).FirstOrDefault().code;
                                }
                                if (base.projects.Where(x => x.id_project == item_transfer.id_project).FirstOrDefault() != null)
                                {
                                    Brillo.Logic.Range.project_Code = base.projects.Where(x => x.id_project == item_transfer.id_project).FirstOrDefault().code;
                                }

                                app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == item_transfer.id_range).FirstOrDefault();
                                item_transfer.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                                item_transfer.RaisePropertyChanged("number");
                            }
                        }

                    }
                }
            }

            base.SaveChanges();
        }
    }
}
