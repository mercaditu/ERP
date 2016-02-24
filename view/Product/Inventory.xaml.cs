using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Data;
using entity;
using System.Collections.Generic;

namespace Cognitivo.Product
{
    public partial class Inventory : Page
    {
        InventoryDB InventoryDB = new InventoryDB();
        CollectionViewSource item_inventoryViewSource, item_inventoryitem_inventory_detailViewSource;

        int company_ID = entity.Properties.Settings.Default.company_ID;

        public Inventory()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_inventoryViewSource = ((CollectionViewSource)(FindResource("item_inventoryViewSource")));
            InventoryDB.item_inventory.Where(a => a.id_company == company_ID).Load();
            item_inventoryViewSource.Source = InventoryDB.item_inventory.Local;

            CollectionViewSource app_branchViewSource = (CollectionViewSource)(FindResource("app_branchViewSource"));
            app_branchViewSource.Source = await InventoryDB.app_branch.Include(b => b.app_location)
                .Where(a => a.is_active == true
                    && a.can_stock == true
                    && a.id_company == company_ID)
                .OrderBy(a => a.name).ToListAsync();
        }

        private void BindItemMovement()
        {
            int id_branch = (int)cbxBranch.SelectedValue;
            item_inventory item_inventory = (item_inventory)item_inventoryViewSource.View.CurrentItem;

            //List<item_product> item_productLIST = InventoryDB.item_product.Where(x => x.id_company == company_ID && x.item.is_active).ToList();

            //foreach (item_product i in item_productLIST)
            //{
            //    item_inventory_detail item_inventory_detail = new item_inventory_detail();
            //    item_inventory_detail.item_product = i;
            //    item_inventory_detail.value_counted = 0;

            //    using (db db = new db())
            //    {
            //        item_inventory_detail.app_location = db.app_location.Where(x => x.id_branch == id_branch && x.is_default).FirstOrDefault();

            //        if (db.item_movement.Where(x => x.id_item_product == i.id_item_product
            //                                     && x.app_location.id_branch == id_branch
            //                                     && x.status == Status.Stock.InStock).ToList().Count > 0)
            //        {
            //            item_inventory_detail.value_system = db.item_movement
            //                                                   .Where(x => x.id_item_product == i.id_item_product && x.app_location.id_branch == id_branch && x.status == Status.Stock.InStock)
            //                                                   .Sum(y => y.credit - y.debit);
            //        }
            //        else
            //        {
            //            item_inventory_detail.value_system = 0;
            //        }
            //    }

            //    item_inventory.item_inventory_detail.Add(item_inventory_detail);
            //}
            var movement =
                (from item_movement in  InventoryDB.item_movement join item_product in  InventoryDB.item_product on item_movement.id_item_product equals item_product.id_item_product
                        into its
                 from p in its.DefaultIfEmpty()
                
                 group p by new {p}
                     into last
                     select new
                     {
                         code = last.Key.p.item.code,
                         item_product = last.Key.p,
                         name = last.Key.p.item.name,
                        // BranchName = last.Key.item_movement.app_location.app_branch.name,
                        // id_location = last.Key.item_movement.app_location.id_location,
                         itemid = last.Key.p.item.id_item,
                         //quntitiy = last.Sum(x => last.Key.item_movement.credit) - last.Sum(x => last.Key.item_movement.debit),
                         id_item_product = last.Key.p.id_item_product,
                         measurement = last.Key.p.item.id_measurement
                     }).ToList();

            //var movement =
            //    (from items in InventoryDB.items
            //        join item_product in InventoryDB.item_product on items.id_item equals item_product.id_item
            //            into its
            //     from p in its.DefaultIfEmpty()
            //        join item_movement in InventoryDB.item_movement on p.id_item_product equals item_movement.id_item_product
            //        into IMS
            //     from a in IMS
            //        join AM in InventoryDB.app_branch on a.app_location.id_branch equals AM.id_branch
            //        where a.app_location.id_branch == id_branch
            //        group a by new { a.item_product }
            //            into last
            //            select new
            //            {
            //                code = last.Key.item_product.item.code,
            //                item_product = last.Key.item_product,
            //                name = last.Key.item_product.item.name,
            //                BranchName = last.OrderBy(m => m.app_location.id_branch),
            //                id_location = last.Max(x=>x.id_location),
            //                itemid = last.Key.item_product.item.id_item,
            //                quntitiy = last.Sum(x => x.credit) - last.Sum(x => x.debit),
            //                id_item_product = last.Key.item_product.id_item_product,
            //                measurement = last.Key.item_product.item.id_measurement
            //            }).ToList();

            foreach (var item in movement)
            {
                item_inventory_detail item_inventory_detail = new item_inventory_detail();
                //item_inventory_detail.app_location = InventoryDB.app_location.Where(x => x.id_location == item.id_location).FirstOrDefault();
                item_inventory_detail.item_product = InventoryDB.item_product.Where(x => x.id_item_product == item.id_item_product).FirstOrDefault();
                item_inventory_detail.id_item_product = item.id_item_product;
                //item_inventory_detail.id_location = item.id_location;
                item_inventory_detail.id_item_product = item.id_item_product;
               // item_inventory_detail.value_system = item.quntitiy;
                item_inventory.item_inventory_detail.Add(item_inventory_detail);
            }

            item_inventoryitem_inventory_detailViewSource = ((CollectionViewSource)(FindResource("item_inventoryitem_inventory_detailViewSource")));
            item_inventoryitem_inventory_detailViewSource.View.Refresh();
            item_inventoryitem_inventory_detailViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            try
            {
                item_inventory item_inventory = new item_inventory();
                item_inventory.IsSelected = true;
                item_inventory.trans_date = DateTime.Now;
                InventoryDB.Entry(item_inventory).State = EntityState.Added;
                item_inventory.State = EntityState.Added;
                item_inventoryViewSource.View.Refresh();
                item_inventoryViewSource.View.MoveCurrentToLast();

                if (item_inventoryDataGrid.SelectedItem != null)
                {
                    BindItemMovement();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (item_inventoryDataGrid.SelectedItem != null)
            {
                item_inventory item_inventory_old = (item_inventory)item_inventoryDataGrid.SelectedItem;
                item_inventory_old.IsSelected = true;
                item_inventory_old.State = EntityState.Modified;
                InventoryDB.Entry(item_inventory_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            try
            {
                InventoryDB.SaveChanges();
                item_inventoryViewSource.View.Refresh();
                toolBar.msgSaved();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            InventoryDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBox.Show("Function Not Available");
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            item_inventory item_inventory = (item_inventory)item_inventoryDataGrid.SelectedItem;
            item_inventory.id_branch = (int)cbxBranch.SelectedValue;
            InventoryDB.Approve();
        }

        private void CbxBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (item_inventoryDataGrid.SelectedItem != null)
            {
                BindItemMovement();
            }

        }

    

       
    }
}
