﻿using entity;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace cntrl.Controls
{
    public partial class InventoryFlowDataGrid : UserControl
    {
        public long? ParentID { get; set; }
        public int ProductID { get; set; }
        public long? MovementID { get; set; }

        public ItemMovement ItemMovement { get; set; }
        
        private CollectionViewSource item_movementViewSource;
        public App.Names ApplicationName { get; set; }

        public InventoryFlowDataGrid()
        {
            InitializeComponent();
            ApplicationName = App.Names.Movement;
        }

        public event SelectionChangedEventHandler SelectionChanged;

        //public event SelectionChangedEventHandler Selection_WithOut_ChildChanged;
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemMovement = new ItemMovement();

            ItemMovement = item_movementViewSource.View.CurrentItem as ItemMovement;
            if (ItemMovement != null)
            {
                MovementID = ItemMovement.MovementID;
                SelectionChanged(sender, e);
            }
        }

        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            using (db db = new db())
            {
                var MovementList = (from item in db.item_movement
                                    join loc in db.app_location on item.id_location equals loc.id_location
                                    join b in db.app_branch on loc.id_branch equals b.id_branch
                                    join ip in db.item_product on item.id_item_product equals ip.id_item_product
                                    join i in db.items on ip.id_item equals i.id_item
                                    where
                                    item.parent.id_movement == ParentID
                                    &&
                                    item.id_item_product == ProductID
                                    select new ItemMovement
                                    {
                                        MovementID = item.id_movement,
                                        Branch = b.name,
                                        Location = loc.name,
                                        ProductCode = i.code,
                                        ProductName = i.name,
                                        Date = item.trans_date,
                                        ExpiryDate = item.expire_date,
                                        BatchCode = item.code,
                                        Quantity = item.credit - item.debit,
                                        Cost = item.item_movement_value_rel.total_value,
                                        Comment = item.comment,
                                        DisplayImage = item.credit > 0 ? true : false,
                                        IsTransfer = item.id_transfer_detail != null ? true : false
                                    })
                                    .OrderByDescending(x => x.Date)
                                    .ToList();

                item_movementViewSource = FindResource("item_movementViewSource") as CollectionViewSource;
                item_movementViewSource.Source = MovementList;
                item_movementViewSource.View.Refresh();


                Filter_NegativeTransactions();
            }
           
          




          
          
        }

        private void Filter_NegativeTransactions()
        {
            if (item_movementViewSource != null)
            {
                item_movementViewSource.View.Filter = i =>
                {
                    ItemMovement ItemMovement = i as ItemMovement;

                    //If IsTransfer then hide negative quantities.
                    if (ItemMovement.IsTransfer)
                    {
                        if (ItemMovement.Quantity < 0)
                        {
                            return false;
                        }
                    }

                    return true;
                };
            }
            else
            {
                item_movementViewSource.View.Filter = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemMovement ItemMovement = item_movementViewSource.View.CurrentItem as ItemMovement;
            if (ItemMovement != null)
            {
                MovementID = ItemMovement.MovementID;
                using (db db = new db())
                {
                    entity.Brillo.Document.Start.Automatic(db.item_movement.Find(MovementID), "MovementLabel");
                }
            }
        }

        private void dgItemMovement_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (CurrentSession.UserRole.is_master)
            {
                if (item_movementViewSource != null)
                {
                    if (item_movementViewSource.View != null)
                    {
                        ItemMovement ItemMovement = item_movementViewSource.View.CurrentItem as ItemMovement;
                        if (ItemMovement != null)
                        {
                            decimal Cost = ItemMovement.Cost;
                            long MovementID = ItemMovement.MovementID;

                            using (db db = new db())
                            {
                                item_movement mov = db.item_movement.Find(MovementID);
                                if (mov != null)
                                {
                                    db.item_movement_value_detail.RemoveRange(mov.item_movement_value_rel.item_movement_value_detail);
                                    mov.Update_ChildVales(Cost, false,DateTime.Now);
                                    db.SaveChangesAsync();
                                    MessageBox.Show(entity.Brillo.Localize.StringText("Done"), "Cognitivo ERP", MessageBoxButton.OK);
                                    LoadData();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Sorry, only Administrators are allowed to update costs.", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void UpdateBatch_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.UserRole.is_master)
            {
                if (item_movementViewSource != null)
                {
                    if (item_movementViewSource.View != null)
                    {
                        ItemMovement ItemMovement = item_movementViewSource.View.CurrentItem as ItemMovement;
                        if (ItemMovement != null)
                        {
                            string Code = ItemMovement.BatchCode;
                            DateTime? ExpireDate = ItemMovement.ExpiryDate??null;
                            long MovementID = ItemMovement.MovementID;

                            using (db db = new db())
                            {
                                item_movement mov = db.item_movement.Find(MovementID);
                                if (mov != null)
                                {
                                    mov.code = Code;
                                    mov.expire_date = ExpireDate ?? null;
                                    mov.Update_ChildBatch(Code,ExpireDate);
                                    db.SaveChangesAsync();
                                    MessageBox.Show(entity.Brillo.Localize.StringText("Done"), "Cognitivo ERP", MessageBoxButton.OK);
                                    LoadData();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Sorry, only Administrators are allowed to update costs.", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }

    public class ItemMovement
    {
        public byte[] file;

        public long MovementID { get; set; }
        public DateTime Date { get; set; }
        public string Branch { get; set; }
        public string Location { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string BatchCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }

        public string Comment { get; set; }
        public bool DisplayImage { get; set; }
        public bool IsTransfer { get; set; }
    }
}