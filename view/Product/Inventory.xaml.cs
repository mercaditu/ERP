using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Product
{
    public partial class Inventory : Page
    {
        private InventoryDB InventoryDB = new InventoryDB();

        private CollectionViewSource item_inventoryViewSource,
            item_inventoryitem_inventory_detailViewSource,
            app_branchapp_locationViewSource,
            app_branchViewSource;

        private cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry;
        private cntrl.Panels.pnl_ItemMovement objpnl_ItemMovement;
        private int CurrencyID = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;

        public Inventory()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_inventoryitem_inventory_detailViewSource = (CollectionViewSource)(FindResource("item_inventoryitem_inventory_detailViewSource"));
            app_branchapp_locationViewSource = (CollectionViewSource)(FindResource("app_branchapp_locationViewSource"));
            item_inventoryViewSource = ((CollectionViewSource)(FindResource("item_inventoryViewSource")));

            await InventoryDB.item_inventory.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(x => x.trans_date).LoadAsync();
            item_inventoryViewSource.Source = InventoryDB.item_inventory.Local;

            app_branchViewSource = (CollectionViewSource)(FindResource("app_branchViewSource"));
            await InventoryDB.app_branch
             .Where(a => a.is_active == true
                 && a.can_stock == true
                 && a.id_company == CurrentSession.Id_Company).Include(x => x.app_location)
             .OrderBy(a => a.name).LoadAsync();
            app_branchViewSource.Source = InventoryDB.app_branch.Local;

            filetr_detail();
        }

        private void filetr_detail()
        {
            app_location app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;
            item_inventory item_inventory = item_inventoryViewSource.View.CurrentItem as item_inventory;

            if (app_location != null)
            {
                if (item_inventoryitem_inventory_detailViewSource != null)
                {
                    if (item_inventoryitem_inventory_detailViewSource.View != null)
                    {
                        item_inventoryitem_inventory_detailViewSource.View.Filter = i =>
                        {
                            item_inventory_detail item_inventory_detail = (item_inventory_detail)i;
                            if (item_inventory_detail.id_location == app_location.id_location && item_inventory_detail.id_inventory == item_inventory.id_inventory)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }

        private void BindItemMovement()
        {
            item_inventory item_inventory = null;
            app_location app_location = null;

            item_inventory = (item_inventory)item_inventoryViewSource.View.CurrentItem;
            app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;

            if (app_location != null && item_inventory != null)
            {
                if (app_location != null)
                {
                    List<item_product> item_productLIST = InventoryDB.item_product.Where(x => x.id_company == CurrentSession.Id_Company && x.item.is_active).Include(y => y.item).ToList(); //.Select(x=>x.id_item_product).ToList();
                    Class.StockCalculations Stock = new Class.StockCalculations();
                    List<Class.StockList> StockList = Stock.ByBranchLocation(app_location.id_location, item_inventory.trans_date);

                    foreach (item_product item_product in item_productLIST.OrderBy(x => x.item.name))
                    {
                        int i = item_product.id_item_product;

                        if (item_inventory.item_inventory_detail.Where(x => x.id_item_product == i && x.id_location == app_location.id_location).Any())
                        {
                            item_inventory_detail item_inventory_detail = item_inventory.item_inventory_detail.Where(x => x.id_item_product == i).FirstOrDefault();
                            if (StockList.Where(x => x.ProductID == i).FirstOrDefault() != null)
                            {
                                item_inventory_detail.value_system = StockList.Where(x => x.ProductID == i).FirstOrDefault().Quantity;
                                item_inventory_detail.unit_value = StockList.Where(x => x.ProductID == i).FirstOrDefault().Cost;
                            }
                            else
                            {
                                item_inventory_detail.value_system = 0;
                                item_inventory_detail.unit_value = 0;
                            }
                        }
                        else
                        {
                            item_inventory_detail item_inventory_detail = new item_inventory_detail();
                            item_inventory_detail.State = EntityState.Added;
                            item_inventory_detail.item_product = item_product;
                            item_inventory_detail.id_item_product = i;

                            item_inventory_detail.app_location = app_location;
                            item_inventory_detail.id_location = app_location.id_location;
                            item_inventory_detail.timestamp = DateTime.Now;

                            if (StockList.Where(x => x.ProductID == i).FirstOrDefault() != null)
                            {
                                item_inventory_detail.value_system = StockList.Where(x => x.ProductID == i).FirstOrDefault().Quantity;
                                item_inventory_detail.unit_value = StockList.Where(x => x.ProductID == i).FirstOrDefault().Cost;
                            }
                            else
                            {
                                item_inventory_detail.value_system = 0;
                            }

                            if (CurrencyID > 0)
                            {
                                item_inventory_detail.id_currencyfx = CurrencyID;
                            }
                            item_inventory_detail.item_inventory = item_inventory;
                            item_inventory.item_inventory_detail.Add(item_inventory_detail);
                        }
                    }
                }

                item_inventoryitem_inventory_detailViewSource.View.Refresh();
                filetr_detail();
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            try
            {
                app_branch app_branch = app_branchViewSource.View.CurrentItem as app_branch;
                item_inventory item_inventory = new item_inventory();
                item_inventory.IsSelected = true;
                item_inventory.id_branch = app_branch.id_branch;
                item_inventory.trans_date = DateTime.Now;
                InventoryDB.Entry(item_inventory).State = EntityState.Added;
                item_inventory.State = EntityState.Added;
                app_branchViewSource.View.MoveCurrentToFirst();
                app_branchapp_locationViewSource.View.MoveCurrentToFirst();
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
            if (InventoryDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(InventoryDB.NumberOfRecords);
                item_inventoryViewSource.View.Refresh();
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

            if (InventoryDB.Approve())
            {
                toolBar.msgApproved(InventoryDB.NumberOfRecords);
            }
        }

        private void location_ListBox_SelectionChanged(object sender, RoutedEventArgs e)
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
            if (crud_modal.IsVisible == false)
            {
                item_inventory item_inventory = (item_inventory)item_inventoryViewSource.View.CurrentItem;

                item_inventoryViewSource.View.Refresh();
                item_inventoryitem_inventory_detailViewSource.View.Refresh();
                item_inventoryitem_inventory_detailViewSource.View.MoveCurrentToFirst();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            app_location app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;

            if (app_location != null)
            {
                if (item_inventoryitem_inventory_detailViewSource != null)
                {
                    if (item_inventoryitem_inventory_detailViewSource.View != null)
                    {
                        item_inventoryitem_inventory_detailViewSource.View.Filter = i =>
                        {
                            item_inventory_detail item_inventory_detail = (item_inventory_detail)i;
                            if ((item_inventory_detail.item_product.item.name.ToUpper().Contains(txtsearch.Text.ToUpper()) ||
                                 item_inventory_detail.item_product.item.code.ToUpper().Contains(txtsearch.Text.ToUpper())) &&
                                 app_location.id_location == item_inventory_detail.id_location)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }

        private void EditCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_inventory_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void Excel_Drop(object sender, DragEventArgs e)
        {
            item_inventory item_inventory = item_inventoryViewSource.View.CurrentItem as item_inventory;
            entity.Brillo.Inventory2Excel Inv2Excel = new entity.Brillo.Inventory2Excel();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (Inv2Excel.Read(file.FirstOrDefault(), item_inventory))
                {
                    toolBar.msgSaved(1);

                    item_inventoryViewSource.View.Refresh();
                    item_inventoryitem_inventory_detailViewSource.View.Refresh();
                    item_inventoryitem_inventory_detailViewSource.View.MoveCurrentToFirst();
                }
            }
        }

        private void Excel_Create(object sender, RoutedEventArgs e)
        {
            item_inventory item_inventory = item_inventoryViewSource.View.CurrentItem as item_inventory;
            entity.Brillo.Inventory2Excel Inv2Excel = new entity.Brillo.Inventory2Excel();

            if (Inv2Excel.Create(item_inventory))
            {
                toolBar.msgSaved(1);
            }
        }

        private void EditCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Hidden;
            item_inventory_detail item_inventory_detail = e.Parameter as item_inventory_detail;
            item_inventory item_inventory = (item_inventory)item_inventoryDataGrid.SelectedItem;

            if (item_inventory_detail != null)
            {
                if (item_inventory_detail.item_inventory_dimension.Count() == 0)
                {
                    if (InventoryDB.item_dimension.Where(x => x.id_item == item_inventory_detail.item_product.id_item).ToList() != null)
                    {
                        List<item_dimension> item_dimensionList = InventoryDB.item_dimension.Where(x => x.id_item == item_inventory_detail.item_product.id_item).ToList();
                        if (item_dimensionList.Count() > 0)
                        {
                            crud_modal.Visibility = Visibility.Visible;
                            objpnl_ItemMovement = new cntrl.Panels.pnl_ItemMovement();
                            foreach (item_dimension item_dimension in item_dimensionList)
                            {
                                item_inventory_dimension item_inventory_dimension = new item_inventory_dimension();
                                item_inventory_dimension.id_dimension = item_dimension.id_app_dimension;
                                item_inventory_dimension.value = item_dimension.value;
                                //  item_inventory_dimension.id_measurement = item_dimension.id_measurement;
                                item_inventory_detail.item_inventory_dimension.Add(item_inventory_dimension);
                            }
                            item_inventory_detail.IsSelected = true;

                            objpnl_ItemMovement.item_inventoryList = item_inventoryitem_inventory_detailViewSource.View.OfType<item_inventory_detail>().Where(x => x.id_item_product == item_inventory_detail.id_item_product).ToList();
                            objpnl_ItemMovement.InventoryDB = InventoryDB;
                            crud_modal.Children.Add(objpnl_ItemMovement);
                        }
                    }
                }
                if (item_inventory_detail.item_product.can_expire)
                {
                    crud_modalExpire.Visibility = Visibility.Visible;
                    pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(item_inventory_detail.item_inventory.id_branch, null, item_inventory_detail.id_item_product);
                    crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                }
            }
        }

        private void location_ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (item_inventoryDataGrid.SelectedItem != null)
            {
                if (app_branchapp_locationViewSource != null)
                {
                    if (app_branchapp_locationViewSource.View != null)
                    {
                        filetr_detail();
                    }
                }
            }
        }

        private void crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                item_inventory_detail item_inventory_detail = (item_inventory_detail)item_inventoryitem_inventory_detailViewSource.View.CurrentItem;

                if (item_inventory_detail != null)
                {
                    item_movement item_movement = InventoryDB.item_movement.Find(pnl_ItemMovementExpiry.MovementID);
                    if (item_movement != null)
                    {
                        item_inventory_detail.batch_code = item_movement.code;
                        item_inventory_detail.expire_date = item_movement.expire_date;
                        item_inventory_detail.movement_id = (int)item_movement.id_movement;
                    }
                }
            }
        }
    }
}