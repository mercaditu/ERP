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
        List<item_inventory_detail> item_inventory_detailList;
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

        private async void BindItemMovement()
        {
            int id_branch = (int)cbxBranch.SelectedValue;
            item_inventory item_inventory = (item_inventory)item_inventoryViewSource.View.CurrentItem;

            if (item_inventory.item_inventory_detail.Count==0)
            {
                item_inventory_detailList = new List<entity.item_inventory_detail>();
                List<item_product> item_productLIST = await InventoryDB.item_product.Where(x => x.id_company == company_ID && x.item.is_active).ToListAsync();

                foreach (item_product i in item_productLIST)
                {
                    item_inventory_detail item_inventory_detail = new item_inventory_detail();
                    item_inventory_detail.item_product = i;
                    item_inventory_detail.value_counted = 0;

                    //using (InventoryDB db = new InventoryDB())
                    //{
                    item_inventory_detail.app_location = InventoryDB.app_location.Where(x => x.id_branch == id_branch && x.is_default).FirstOrDefault();
                    item_inventory_detail.id_location = InventoryDB.app_location.Where(x => x.id_branch == id_branch && x.is_default).FirstOrDefault().id_location;
                    if (InventoryDB.item_movement.Where(x => x.id_item_product == i.id_item_product
                                                     && x.app_location.id_branch == id_branch
                                                     && x.status == Status.Stock.InStock).ToList().Count > 0)
                    {
                        item_inventory_detail.value_system = InventoryDB.item_movement
                                                               .Where(x => x.id_item_product == i.id_item_product && x.app_location.id_branch == id_branch && x.status == Status.Stock.InStock)
                                                               .Sum(y => y.credit - y.debit);
                    }
                    else
                    {
                        item_inventory_detail.value_system = 0;
                    }
                    item_inventory_detail.item_inventory = item_inventory;
                    //  }

                    item_inventory_detailList.Add(item_inventory_detail);
                }
                
            }
            else
            {
                item_inventory_detailList = item_inventory.item_inventory_detail.ToList();
            }
           
            
            
            dgvdetail.ItemsSource = item_inventory_detailList;
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
              
                foreach (item_inventory_detail _item_inventory_detail in item_inventory_detailList)
                {
                    if (_item_inventory_detail.value_counted != 0)
                    {
                        if (_item_inventory_detail.id_inventory_detail==0)
                        {
                            InventoryDB.item_inventory_detail.Add(_item_inventory_detail);
                        }
                       
                    }
                }
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

        private void item_inventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (item_inventoryViewSource != null)
            {
                if (item_inventoryViewSource.View!=null)
                {
                    item_inventory item_inventory = (item_inventory)item_inventoryViewSource.View.CurrentItem;
                    item_inventory_detailList = item_inventory.item_inventory_detail.ToList();
                    dgvdetail.ItemsSource = item_inventory_detailList;
                   
                    
                }
              
            }
           
        }

    

       
    }
}
