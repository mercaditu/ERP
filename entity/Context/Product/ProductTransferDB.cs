﻿using System;
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

        /// <summary>
        /// Should only
        /// </summary>
        public int ApproveOrigin(int ID_BranchOrigin, int ID_BranchDestination, bool movebytruck)
        {
            NumberOfRecords = 0;
            foreach (item_transfer item_transfer in base.item_transfer.Local.Where(x => x.IsSelected))
            {
                if (item_transfer.id_transfer == 0)
                {
                    SaveChanges();
                }
            }
            foreach (item_transfer item_transfer in base.item_transfer.Local.Where(x => x.IsSelected))
            {
                foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail.Where(x => x.status == Status.Documents_General.Pending))
                {
                    Discount_Items_Origin(item_transfer_detail, ID_BranchOrigin, ID_BranchDestination, movebytruck);

                    //transit
                    NumberOfRecords += 1;
                    item_transfer.status = Status.Transfer.Transit;

                    ///The Raise property code was cuasing the toolbar to be unresponsive to status changes, 
                    ///causeing the user to close and start again. and so i commented it.
                    //item_transfer.RaisePropertyChanged("status");
                }

                if (item_transfer != null)
                {
                    ///entity.Brillo.Document.Start.Manual(item_transfer, item_transfer.app_document_range);
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

            return NumberOfRecords;
        }

        public int ApproveDestination(int ID_BranchOrigin, int ID_BranchDestination, bool MoveByTruck)
        {
            NumberOfRecords = 0;

            foreach (item_transfer item_transfer in base.item_transfer.Local.Where(x => x.IsSelected))
            {
                foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail.Where(x => x.IsSelected && x.status == Status.Documents_General.Pending))
                {
                    if (item_transfer_detail.item_product != null)
                    {
                        Credit_Items_Destination(item_transfer_detail, ID_BranchOrigin, ID_BranchDestination, MoveByTruck);

                        NumberOfRecords += 1;

                        item_transfer_detail.status = Status.Documents_General.Approved;
                        item_transfer_detail.RaisePropertyChanged("status");
                        item_transfer.status = Status.Transfer.Approved;
                        item_transfer.RaisePropertyChanged("status");
                    }
                }

                ///Print Document only if 
                if ((item_transfer.number == null || item_transfer.number == string.Empty) && item_transfer.id_range > 0)
                {
                    if (item_transfer.id_branch > 0)
                    {
                        if (CurrentSession.Branches.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault() != null)
                        {
                            Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault().code;
                        }
                    }

                    if (item_transfer.id_terminal > 0)
                    {
                        if (CurrentSession.Terminals.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault() != null)
                        {
                            Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault().code;
                        }
                    }

                    if (item_transfer.id_user > 0)
                    {
                        security_user security_user = base.security_user.Find(item_transfer.id_user);
                        if (security_user != null)
                        {
                            Brillo.Logic.Range.user_Code = security_user.code;
                        }
                    }

                    if (item_transfer.id_project > 0)
                    {
                        project projects = base.projects.Find(item_transfer.id_project);
                        if (projects != null)
                        {
                            Brillo.Logic.Range.project_Code = projects.code;
                        }
                    }
                    
                    app_document_range app_document_range = base.app_document_range.Find(item_transfer.id_range);
                    item_transfer.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                    item_transfer.RaisePropertyChanged("number");
                }

            }

            base.SaveChanges();

            return NumberOfRecords;
        }

        /// <summary>
        /// Executes code that will insert Invoiced Items into Movement.
        /// </summary>
        /// <param name="invoice"></param>
        public void Credit_Items_Destination(item_transfer_detail item_transfer_detail, int ID_BranchOrigin, int ID_BranchDestination, bool MoveByTruck)
        {
            entity.Brillo.Logic.Stock stock = new Brillo.Logic.Stock();
            app_currencyfx app_currencyfx = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == CurrentSession.Currency_Default.id_currency).FirstOrDefault();
            app_location app_location_dest = CurrentSession.Locations.Where(x => x.id_branch == ID_BranchDestination && x.is_default).FirstOrDefault();

            if (MoveByTruck)
            {
                List<entity.Brillo.StockList> Items_InStockLIST;
                entity.Brillo.Stock stockBrillo = new Brillo.Stock();
                Items_InStockLIST = stockBrillo.MovementForTransfer(item_transfer_detail.id_transfer_detail, item_transfer_detail.id_item_product);

                List<item_movement> item_movement_LIST = new List<entity.item_movement>();
                ///Discount From Destination. Because merchendice is returned to Origin, so it must be discounted from Destintation.
                item_movement_LIST =
                    stock.DebitOnly_MovementLIST(this, Items_InStockLIST, Status.Stock.InStock, App.Names.Transfer, item_transfer_detail.id_transfer, item_transfer_detail.id_transfer_detail,
                    app_currencyfx.id_currencyfx, item_transfer_detail.item_product, app_location_dest.id_location, item_transfer_detail.quantity_destination,
                    item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(App.Names.Transfer, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""));

                base.item_movement.AddRange(item_movement_LIST);

                app_location app_location_origin = CurrentSession.Locations.Where(x => x.id_branch == ID_BranchOrigin && x.is_default).FirstOrDefault();

                //Credit in Origin only if it is MoveByTruck.
                item_movement item_movement_origin;
                item_movement_origin =
                    stock.CreditOnly_Movement(
                        Status.Stock.InStock,
                        App.Names.Transfer,
                        item_transfer_detail.id_transfer,
                        item_transfer_detail.id_transfer_detail,
                        app_currencyfx.id_currencyfx,
                        item_transfer_detail.id_item_product,
                        app_location_origin.id_location,
                        item_transfer_detail.quantity_destination,
                        item_transfer_detail.item_transfer.trans_date,
                        0,
                        stock.comment_Generator(App.Names.Transfer, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""),
                        null
                        );

                base.item_movement.Add(item_movement_origin);
            }
            else
            {
                //Credit Destination.
                item_movement item_movement_dest;
                List<item_movement> Items_InStockLIST = base.item_movement.Where(x => x.id_transfer_detail == item_transfer_detail.id_transfer_detail &&
                    x.id_item_product == item_transfer_detail.id_item_product && x.debit > 0).ToList();
                int count = 1;
                if (Items_InStockLIST.Count()>0)
                {
                    count = Items_InStockLIST.Count();
                }

                item_movement parent_item_movement = Items_InStockLIST.FirstOrDefault();

                item_movement_dest =
                            stock.CreditOnly_Movement(
                                Status.Stock.InStock,
                                App.Names.Transfer,
                                item_transfer_detail.id_transfer,
                                item_transfer_detail.id_transfer_detail,
                                app_currencyfx.id_currencyfx,
                                item_transfer_detail.id_item_product,
                                app_location_dest.id_location,
                                item_transfer_detail.quantity_destination,
                                item_transfer_detail.item_transfer.trans_date,
                                Items_InStockLIST.Sum(x => (x.item_movement_value.Sum(y => y.unit_value) / (x.item_movement_value.Count() != 0 ? x.item_movement_value.Count() : 1)))/count,
                                stock.comment_Generator(App.Names.Transfer, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""),
                                null
                                );
                item_movement_dest.parent = Items_InStockLIST.FirstOrDefault();
                base.item_movement.Add(item_movement_dest);
            }
        }

        /// <summary>
        /// Executes code that will insert Invoiced Items into Movement.
        /// </summary>
        /// <param name="invoice"></param>
        public void Discount_Items_Origin(item_transfer_detail item_transfer_detail, int ID_BranchOrigin, int ID_BranchDestination, bool movebytruck)
        {
            Brillo.Logic.Stock stock = new Brillo.Logic.Stock();

            if (item_transfer_detail.item_product != null)
            {
                app_currencyfx app_currencyfx = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == CurrentSession.Currency_Default.id_currency).FirstOrDefault();
                app_location app_location = CurrentSession.Locations.Where(x => x.id_branch == ID_BranchOrigin && x.is_default).FirstOrDefault();

                if (movebytruck)
                {
                    List<Brillo.StockList> Items_InStockLIST;
                    if (item_transfer_detail.movement_id != null)
                    {
                        Brillo.Stock stockBrillo = new Brillo.Stock();
                        Items_InStockLIST = stockBrillo.ScalarMovement(base.item_movement.Where(x => x.id_movement == item_transfer_detail.movement_id).FirstOrDefault());
                    }
                    else
                    {
                        Brillo.Stock stockBrillo = new Brillo.Stock();
                        Items_InStockLIST = stockBrillo.List(app_location.id_branch, app_location.id_location, item_transfer_detail.id_item_product);
                    }

                    List<item_movement> item_movement_originList;
                    item_movement_originList = stock.DebitOnly_MovementLIST(this, Items_InStockLIST, Status.Stock.InStock, App.Names.Transfer, item_transfer_detail.id_transfer, item_transfer_detail.id_transfer_detail, app_currencyfx.id_currencyfx, item_transfer_detail.item_product, app_location.id_location,
                            item_transfer_detail.quantity_origin, item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(App.Names.Transfer, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""));

                    base.item_movement.AddRange(item_movement_originList);

                    item_movement item_movement_Dest;
                    app_location app_locationdest = CurrentSession.Locations.Where(x => x.id_branch == ID_BranchDestination && x.is_default).FirstOrDefault();

                    item_movement_Dest = stock.CreditOnly_Movement(
                        Status.Stock.InStock,
                        App.Names.Transfer,
                        item_transfer_detail.id_transfer,
                        item_transfer_detail.id_transfer_detail,
                        app_currencyfx.id_currencyfx,
                        item_transfer_detail.id_item_product,
                        app_locationdest.id_location,
                            item_transfer_detail.quantity_origin,
                            item_transfer_detail.item_transfer.trans_date,
                            0,
                            stock.comment_Generator(App.Names.Transfer, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""),
                            null);

                    base.item_movement.Add(item_movement_Dest);
                }
                else
                {
                    List<Brillo.StockList> Items_InStockLIST;
                    if (item_transfer_detail.movement_id != null)
                    {
                        Brillo.Stock stockBrillo = new Brillo.Stock();
                        Items_InStockLIST = stockBrillo.ScalarMovement(base.item_movement.Where(x => x.id_movement == item_transfer_detail.movement_id).FirstOrDefault());

                    }
                    else
                    {
                        Brillo.Stock stockBrillo = new Brillo.Stock();
                        Items_InStockLIST = stockBrillo.List(app_location.id_branch, app_location.id_location, item_transfer_detail.id_item_product);
                    }



                    ///Debit Movement from Origin.
                    List<item_movement> item_movement_originList;
                    item_movement_originList = stock.DebitOnly_MovementLIST(this, Items_InStockLIST, Status.Stock.InStock, App.Names.Transfer, item_transfer_detail.id_transfer, item_transfer_detail.id_transfer_detail, app_currencyfx.id_currencyfx, item_transfer_detail.item_product, app_location.id_location,
                            item_transfer_detail.quantity_origin, item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(App.Names.Transfer, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""));

                    base.item_movement.AddRange(item_movement_originList);
                }
            }
        }
    }
}
