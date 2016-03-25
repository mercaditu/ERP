﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.Entity;
using entity;
using System.Data;
using System.Data.Entity.Validation;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Cognitivo.Product
{
    public partial class Transfer : Page
    {
        ProductTransferDB ProductTransferDB = new ProductTransferDB();

        CollectionViewSource item_transferViewSource, transfercostViewSource, item_transferitem_transfer_detailViewSource;
        List<Class.transfercost> clsTotalGrid = null;

        public Transfer()
        {
            InitializeComponent();
        }


        private void toolBar_btnNew_Click(object sender)
        {
            item_transfer item_transfer = new item_transfer();
            item_transfer.State = System.Data.Entity.EntityState.Added;
            item_transfer.transfer_type = entity.item_transfer.Transfer_type.transfer;
            item_transfer.IsSelected = true;

            ProductTransferDB.Entry(item_transfer).State = EntityState.Added;

            item_transferViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            ProductTransferDB.SaveChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {

        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ProductTransferDB.CancelAllChanges();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (itemDataGrid.SelectedItem != null)
            {
                item_transfer item_transfer = (item_transfer)itemDataGrid.SelectedItem;
                item_transfer.IsSelected = true;
                item_transfer.State = System.Data.Entity.EntityState.Modified;
                ProductTransferDB.Entry(item_transfer).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Filter by branch.
            item_transferViewSource = ((CollectionViewSource)(this.FindResource("item_transferViewSource")));
            ProductTransferDB.item_transfer.Where(a =>
                a.id_company == CurrentSession.Id_Company &&
                a.id_branch == CurrentSession.Id_Branch &&
                a.transfer_type == item_transfer.Transfer_type.transfer)
                .Include(i => i.item_transfer_detail)
                .Load();
            item_transferViewSource.Source = ProductTransferDB.item_transfer.Local;

            item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(this.FindResource("item_transferitem_transfer_detailViewSource")));

            CollectionViewSource branch_originViewSource = ((CollectionViewSource)(this.FindResource("branch_originViewSource")));
            ProductTransferDB.app_branch.Where(a => a.is_active == true).OrderBy(a => a.name).Load();
            branch_originViewSource.Source = ProductTransferDB.app_branch.Local;

            CollectionViewSource branch_destViewSource = ((CollectionViewSource)(this.FindResource("branch_destViewSource")));
            branch_destViewSource.Source = ProductTransferDB.app_branch.Local;

            CollectionViewSource security_userViewSource = ((CollectionViewSource)(this.FindResource("security_userViewSource")));
            ProductTransferDB.security_user.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            security_userViewSource.Source = ProductTransferDB.security_user.Local;

            clsTotalGrid = new List<Class.transfercost>();
            transfercostViewSource = this.FindResource("transfercostViewSource") as CollectionViewSource;
            transfercostViewSource.Source = clsTotalGrid;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.Transfer, CurrentSession.Id_Branch, CurrentSession.Id_terminal);
        }

        private void SmartBox_Item_Select(object sender, RoutedEventArgs e)
        {
            item_transfer item_transfer = (item_transfer)item_transferViewSource.View.CurrentItem;
            item item = ProductTransferDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

            if (item != null && item.item_product != null && item_transfer != null)
            {
                item_transfer_detail item_transfer_detail = new item_transfer_detail();
                item_transfer_detail.quantity_origin = 1;
                item_transfer_detail.item_product = item.item_product.FirstOrDefault();
                item_transfer_detail.RaisePropertyChanged("item_product");
                item_transfer.item_transfer_detail.Add(item_transfer_detail);
            }
            item_transferViewSource.View.Refresh();
            item_transferitem_transfer_detailViewSource.View.Refresh();
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            try
            {


                clsTotalGrid = (List<Class.transfercost>)transfercostViewSource.Source;
                ProductTransferDB.Approve(clsTotalGrid.Sum(x => x.cost));



                toolBar.msgSaved();

            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {

        }
    }
}
