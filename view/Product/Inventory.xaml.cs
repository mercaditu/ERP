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
        CollectionViewSource item_inventoryViewSource, item_inventoryitem_inventory_detailViewSource, app_branchapp_locationViewSource, app_branchViewSource;
        List<item_inventory_detail> item_inventory_detailList;
        cntrl.Panels.pnl_ItemMovement objpnl_ItemMovement;
        int company_ID = entity.Properties.Settings.Default.company_ID;

        public Inventory()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            app_branchapp_locationViewSource = (CollectionViewSource)(FindResource("app_branchapp_locationViewSource"));
            item_inventoryViewSource = ((CollectionViewSource)(FindResource("item_inventoryViewSource")));
            InventoryDB.item_inventory.Where(a => a.id_company == company_ID).Load();
            item_inventoryViewSource.Source = InventoryDB.item_inventory.Local;

            app_branchViewSource = (CollectionViewSource)(FindResource("app_branchViewSource"));
            InventoryDB.app_branch.Include(b => b.app_location)
                .Where(a => a.is_active == true
                    && a.can_stock == true
                    && a.id_company == company_ID)
                .OrderBy(a => a.name).Load();
            app_branchViewSource.Source = InventoryDB.app_branch.Local;

            app_branchViewSource.View.MoveCurrentToFirst();
          

            app_branchapp_locationViewSource.View.MoveCurrentToFirst();
          

        }

        private async void BindItemMovement()
        {
            

               
                item_inventory item_inventory = (item_inventory)item_inventoryViewSource.View.CurrentItem;

                if (item_inventory.item_inventory_detail.Count == 0)
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


                        if (app_branchapp_locationViewSource!=null)
                        {
                            app_location app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;
                            item_inventory_detail.app_location = app_location;
                            item_inventory_detail.id_location = app_location.id_location;
                            if (InventoryDB.item_movement.Where(x => x.id_item_product == i.id_item_product
                                                             && x.id_location == app_location.id_location
                                                             && x.status == Status.Stock.InStock).ToList().Count > 0)
                            {
                                if (cbxBranch.SelectedValue != null)
                                {
                                    int id_branch = (int)cbxBranch.SelectedValue;

                                    item_inventory_detail.value_system = InventoryDB.item_movement
                                                                           .Where(x => x.id_item_product == i.id_item_product && x.app_location.id_branch == id_branch && x.status == Status.Stock.InStock)
                                                                           .Sum(y => y.credit - y.debit);
                                }
                                else
                                {
                                    item_inventory_detail.value_system = 0;
                                }
                            }
                            else
                            {
                                item_inventory_detail.value_system = 0;
                            }
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
                        if (_item_inventory_detail.id_inventory_detail == 0)
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


        }

        private void item_inventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (item_inventoryViewSource != null)
            {
                if (item_inventoryViewSource.View != null)
                {
                    item_inventory item_inventory = (item_inventory)item_inventoryViewSource.View.CurrentItem;
                    item_inventory_detailList = item_inventory.item_inventory_detail.ToList();
                    if (item_inventory_detailList.Count() > 0)
                    {
                        dgvdetail.ItemsSource = item_inventory_detailList;
                    }
                   

                }

            }

        }

        private void location_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (item_inventoryDataGrid.SelectedItem != null)
            {
                if (app_branchapp_locationViewSource != null)
                {
                    if (app_branchapp_locationViewSource.View != null)
                    {
                        BindItemMovement();
                    }
                }

            }
        }



        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            item_inventory_detail item_inventory_detail = dgvdetail.SelectedItem as item_inventory_detail;
            if (item_inventory_detail != null)
            {
                if (objpnl_ItemMovement != null)
                {
                    item_inventory_detail.value_counted = objpnl_ItemMovement.quantity;
                    item_inventory_detail.RaisePropertyChanged("value_counted");
                }
            }

            //item_inventoryViewSource = ((CollectionViewSource)(FindResource("item_inventoryViewSource")));
            //InventoryDB.item_inventory.Where(a => a.id_company == company_ID).Load();
            //item_inventoryViewSource.Source = InventoryDB.item_inventory.Local;
        }

        private void EditCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_inventory_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void EditCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Hidden;
            item_inventory_detail item_inventory_detail = e.Parameter as item_inventory_detail;
            if (item_inventory_detail != null)
            {
                crud_modal.Visibility = System.Windows.Visibility.Visible;
                objpnl_ItemMovement = new cntrl.Panels.pnl_ItemMovement();
                objpnl_ItemMovement.id_item_product = item_inventory_detail.item_product.id_item_product;
                objpnl_ItemMovement.id_location = item_inventory_detail.id_location;
                crud_modal.Children.Add(objpnl_ItemMovement);
            }

        }




    }
}
