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

        public void ApproveOrigin(int origin)
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
                                item_transfer_detail.status = Status.Documents_General.Approved;
                            }



                        }



                    }

                }

            }
            base.SaveChanges();
        }
        public void ApproveDestination(decimal cost, int dest)
        {
            entity.Brillo.Logic.Stock stock = new Brillo.Logic.Stock();

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
                                item_movement item_movement_Dest;
                                app_currencyfx app_currencyfx = base.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                                app_location app_location = base.app_location.Where(x => x.id_branch == dest).FirstOrDefault();
                                List<item_movement> Items_InStockLIST = base.item_movement.Where(x => x.id_location == app_location.id_location
                                                                      && x.id_item_product == item_transfer_detail.id_item_product
                                                                      && x.status == entity.Status.Stock.InStock
                                                                      && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();
                                item_movement_Dest = stock.Credit_Movement(Status.Stock.InStock, App.Names.Transfer, 0, app_currencyfx.id_currencyfx, item_transfer_detail.item_product, app_location.id_location,
                                      item_transfer_detail.quantity_destination, item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(App.Names.Transfer, "", ""));

                                if (item_transfer_detail.quantity_origin == 0)
                                {
                                    item_movement_value item_movement_value = new item_movement_value();
                                    item_movement_value.unit_value = cost / item_transfer_detail.quantity_destination;
                                    item_movement_value.id_currencyfx = 0;
                                    item_movement_value.comment = String.Format("Transaction from transfer");
                                    item_movement_Dest.item_movement_value.Add(item_movement_value);
                                }
                                base.item_movement.Add(item_movement_Dest);
                                item_transfer.status = Status.Documents_General.Approved;
                            }




                        }

                    }
                }



            }
            base.SaveChanges();
        }
    }
}
