using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private CollectionViewSource item_inventoryViewSource,
            item_inventoryitem_inventory_detailViewSource,
            app_branchapp_locationViewSource,
            app_branchViewSource;

        private cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry;
        private cntrl.Panels.pnl_ItemMovement objpnl_ItemMovement;
        public entity.Controller.Product.InventoryController InventoryController;

        public Inventory()
        {
            InitializeComponent();
            InventoryController = FindResource("InventoryController") as entity.Controller.Product.InventoryController;

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                //Load Controller.
                InventoryController.Initialize();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_inventoryitem_inventory_detailViewSource = FindResource("item_inventoryitem_inventory_detailViewSource") as CollectionViewSource;
            app_branchapp_locationViewSource = FindResource("app_branchapp_locationViewSource") as CollectionViewSource;
            item_inventoryViewSource = FindResource("item_inventoryViewSource") as CollectionViewSource;
            InventoryController.Load();

            item_inventoryViewSource.Source = InventoryController.db.item_inventory.Local;
            app_branchViewSource = FindResource("app_branchViewSource") as CollectionViewSource;
            app_branchViewSource.Source = InventoryController.db.app_branch.Local;
            FilterDetail();
        }

        private void FilterDetail()
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
            item_inventory item_inventory = item_inventoryViewSource.View.CurrentItem as item_inventory;
            app_location app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;

            if (app_location != null && item_inventory != null)
            {
                if (app_location != null)
                {
                    List<item_product> item_productLIST = InventoryController.db.item_product.Where(x => x.id_company == CurrentSession.Id_Company && x.item.is_active).Include(y => y.item).ToList(); //.Select(x=>x.id_item_product).ToList();
                    Class.StockCalculations Stock = new Class.StockCalculations();

                    List<Class.StockList> StockList = Stock.ByBranchLocation(app_location.id_location, item_inventory.trans_date);
                    List<Class.StockList> BatchList = Stock.ByLocation_BatchCode(app_location.id_location, item_inventory.trans_date).Where(x => x.Quantity > 0).ToList();

                    ///List through the entire product list.
                    foreach (item_product item_product in item_productLIST.OrderBy(x => x.item.name))
                    {
                        int i = item_product.id_item_product;
                        
                        //If Product Can Expire property is set to true, then we should loop through each Batch Code with a positive Balance.
                        if (item_product.can_expire && BatchList.Where(x => x.ProductID == i).Count() > 0)
                        {
                            foreach (var Batch in BatchList.Where(x => x.ProductID == i).OrderBy(x => x.ExpiryDate))
                            {
                                if (item_inventory.item_inventory_detail.Where(x => x.movement_id == Batch.MovementID).Any())
                                {
                                    item_inventory_detail item_inventory_detail = item_inventory.item_inventory_detail.Where(x => x.movement_id == Batch.MovementID).FirstOrDefault();

                                    if (item_inventory_detail != null)
                                    {
                                        ///Since this item already exists in Inventory, we should update the values. The following code
                                        ///will check for difference between Original and Updated Values and also update the Counted Value for that same difference.
                                        decimal Quantity_Original = item_inventory_detail.value_system;
                                        decimal Quantity_Updated = BatchList.Where(x => x.MovementID == Batch.MovementID).FirstOrDefault() != null ? BatchList.Where(x => x.MovementID == Batch.MovementID).FirstOrDefault().Quantity : 0;
                                        decimal Quantity_Difference = Quantity_Updated - Quantity_Original;

                                        item_inventory_detail.value_system = Quantity_Updated;
                                        item_inventory_detail.value_counted += Quantity_Difference;

                                        //Get the newest Cost.
                                        if (BatchList.Where(x => x.MovementID == Batch.MovementID).FirstOrDefault() != null)
                                        {
                                            item_inventory_detail.unit_value = BatchList.Where(x => x.MovementID == Batch.MovementID).FirstOrDefault().Cost;
                                        }
                                        else
                                        {
                                            //If Product does not exist in BatchList, see if user has already defined cost. else add 0.
                                            if (item_inventory_detail.unit_value > 0)
                                            {
                                                // If UnitValue > 0, means user must have manually inserted value. Leave it as is.
                                            }
                                            else
                                            {
                                                item_inventory_detail.unit_value = 0;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    item_inventory.item_inventory_detail.Add(new item_inventory_detail()
                                    {
                                        value_system = Batch.Quantity,
                                        unit_value = Batch.Cost,
                                        batch_code = Batch.BatchCode,
                                        expire_date = Batch.ExpiryDate,
                                        movement_id = Batch.MovementID,
                                        State = EntityState.Added,
                                        item_product = item_product,
                                        id_item_product = i,
                                        app_location = app_location,
                                        id_location = app_location.id_location,
                                        timestamp = DateTime.Now,
                                    });
                                }
                            }
                        }
                        else
                        ///Normal Use. Not for Batch Code.
                        {
                            item_inventory_detail item_inventory_detail = new item_inventory_detail();

                            if (item_inventory.item_inventory_detail.Where(x => x.id_item_product == i && x.id_location == app_location.id_location).Any())
                            {
                                ///Since this item already exists in Inventory, we should update the values. The following code
                                ///will check for difference between Original and Updated Values and also update the Counted Value for that same difference.
                                item_inventory_detail = item_inventory.item_inventory_detail.Where(x => x.id_item_product == i && x.id_location == app_location.id_location).FirstOrDefault();
                                
                                decimal Quantity_Original = item_inventory_detail.value_system;
                                decimal Quantity_Updated = StockList.Where(x => x.ProductID == i).FirstOrDefault() != null ? StockList.Where(x => x.ProductID == i).FirstOrDefault().Quantity : 0;
                                decimal Quantity_Difference = Quantity_Updated - Quantity_Original;

                                item_inventory_detail.value_system = Quantity_Updated;
                                item_inventory_detail.value_counted += Quantity_Difference;
                                item_inventory_detail.unit_value = StockList.Where(x => x.ProductID == i).FirstOrDefault() != null ? StockList.Where(x => x.ProductID == i).FirstOrDefault().Cost : 0;
                            }
                            else
                            {
                                item_inventory_detail.State = EntityState.Added;
                                item_inventory_detail.item_product = item_product;
                                item_inventory_detail.id_item_product = i;
                                item_inventory_detail.app_location = app_location;
                                item_inventory_detail.id_location = app_location.id_location;
                                item_inventory_detail.timestamp = DateTime.Now;
                                item_inventory_detail.value_system = StockList.Where(x => x.ProductID == i).FirstOrDefault() != null ? StockList.Where(x => x.ProductID == i).FirstOrDefault().Quantity : 0;
                                item_inventory_detail.unit_value = StockList.Where(x => x.ProductID == i).FirstOrDefault() != null ? StockList.Where(x => x.ProductID == i).FirstOrDefault().Cost : 0;

                                if (CurrentSession.Get_Currency_Default_Rate() != null)
                                { item_inventory_detail.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx; }

                                item_inventory_detail.item_inventory = item_inventory;
                                item_inventory_detail.id_inventory = item_inventory.id_inventory;
                                item_inventory.item_inventory_detail.Add(item_inventory_detail);
                            }
                        }
                    }
                }

                item_inventoryViewSource.View.Refresh();
                item_inventoryViewSource.View.MoveCurrentTo(item_inventory);
                app_branchapp_locationViewSource.View.MoveCurrentTo(app_location);
                item_inventoryitem_inventory_detailViewSource.View.Refresh();
                FilterDetail();
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            app_branch app_branch = app_branchViewSource.View.CurrentItem as app_branch;
            if (app_branch != null)
            {
                item_inventory item_inventory = new item_inventory()
                {
                    IsSelected = true,
                    id_branch = app_branch.id_branch,
                    trans_date = DateTime.Now,
                    State = EntityState.Added
                };

                InventoryController.db.Entry(item_inventory).State = EntityState.Added;
                app_branchViewSource.View.MoveCurrentToFirst();
                app_branchapp_locationViewSource.View.MoveCurrentToFirst();
                item_inventoryViewSource.View.Refresh();
                item_inventoryViewSource.View.MoveCurrentTo(item_inventory);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (InventoryController.db.item_inventory.Local.Where(x => x.IsSelected).Count() > 0)
            {
                foreach (item_inventory existing_inv in InventoryController.db.item_inventory.Local.Where(x => x.IsSelected))
                {
                    existing_inv.IsSelected = true;
                    existing_inv.State = EntityState.Modified;
                    InventoryController.db.Entry(existing_inv).State = EntityState.Modified;
                }
            }
            else if (item_inventoryViewSource.View.CurrentItem != null)
            {
                if (item_inventoryViewSource.View.CurrentItem is item_inventory item_inventory)
                {
                    item_inventory selected_inv = (item_inventory)item_inventoryDataGrid.SelectedItem;
                    selected_inv.IsSelected = true;
                    selected_inv.State = EntityState.Modified;
                    InventoryController.db.Entry(selected_inv).State = EntityState.Modified;
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (InventoryController.SaveChanges_WithValidation())
            {
                toolBar.msgSaved(1);
                item_inventoryViewSource.View.Refresh();
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            InventoryController.CancelAllChanges();
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            if (InventoryController.Approve())
            {
                toolBar.msgApproved(1);
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
            if (app_branchapp_locationViewSource.View.CurrentItem is app_location app_location)
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

            item_inventory.IsSelected = true;
            InventoryController.SaveChanges_WithValidation();

            app_location app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;
            if (app_location!=null)
            {
                if (Inv2Excel.Create(item_inventory, app_location.id_location))
                {
                    toolBar.msgSaved(1);
                }
            }
          
            toolBar_btnEdit_Click(null);
        }

        private void item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                add_item(sbxItem.ItemID);
            }
        }

        public void add_item(int id_item)
        {
            item_inventory item_inventory = item_inventoryViewSource.View.CurrentItem as item_inventory;
            if (item_inventory != null)
            {
                item_product item_product = InventoryController.db.item_product.Where(x => x.id_item == id_item).FirstOrDefault();
                if (item_product != null)
                {
                    app_location app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;
                    item_inventory_detail _item_inventory_detail = item_inventoryitem_inventory_detailViewSource.View.OfType<item_inventory_detail>().Where(x => x.id_item_product == item_product.id_item_product).FirstOrDefault();

                    item_inventory_detail item_inventory_detail = new item_inventory_detail()
                    {
                        id_inventory = item_inventory.id_inventory,
                        id_item_product = item_product.id_item_product,
                        item_product = item_product,
                        id_location = app_location.id_location,

                        State = EntityState.Added,
                        timestamp = item_inventory.trans_date
                    };

                    if (_item_inventory_detail == null)
                    {
                        Class.StockCalculations Stock = new Class.StockCalculations();
                        item_inventory_detail.value_system = 0;
                        item_inventory_detail.unit_value = 0; 
                    }
                    else
                    {
                        item_inventory_detail.value_system = 0;
                    }

                    if (CurrentSession.Get_Currency_Default_Rate() != null)
                    {
                        item_inventory_detail.id_currencyfx = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                    }

                    item_inventory.item_inventory_detail.Add(item_inventory_detail);
                    item_inventoryitem_inventory_detailViewSource.View.Refresh();
                }
            }
        }

        private void cbxExpiryCode_Unchecked(object sender, RoutedEventArgs e)
        {
            app_location app_location = app_branchapp_locationViewSource.View.CurrentItem as app_location;

            if (app_location != null)
            {
                if (item_inventoryitem_inventory_detailViewSource != null)
                {
                    if (item_inventoryitem_inventory_detailViewSource.View != null)
                    {
                        foreach (item_inventory_detail detail in item_inventoryitem_inventory_detailViewSource.View)
                        {
                            if (detail.id_location == app_location.id_location)
                            {
                                detail.IsSelected = false;
                                detail.RaisePropertyChanged("IsSelected");
                            }
                        }
                    }
                }
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
                    if (InventoryController.db.item_dimension.Where(x => x.id_item == item_inventory_detail.item_product.id_item).ToList() != null)
                    {
                        List<item_dimension> item_dimensionList = InventoryController.db.item_dimension.Where(x => x.id_item == item_inventory_detail.item_product.id_item).ToList();
                        if (item_dimensionList.Count() > 0)
                        {
                            crud_modal.Visibility = Visibility.Visible;
                            objpnl_ItemMovement = new cntrl.Panels.pnl_ItemMovement();

                            foreach (item_dimension item_dimension in item_dimensionList)
                            {
                                item_inventory_dimension item_inventory_dimension = new item_inventory_dimension()
                                {
                                    id_dimension = item_dimension.id_app_dimension,
                                    value = item_dimension.value
                                };

                                item_inventory_detail.item_inventory_dimension.Add(item_inventory_dimension);
                            }

                            item_inventory_detail.IsSelected = true;

                            objpnl_ItemMovement.item_inventoryList = item_inventoryitem_inventory_detailViewSource.View.OfType<item_inventory_detail>().Where(x => x.id_item_product == item_inventory_detail.id_item_product).ToList();
                            objpnl_ItemMovement.InventoryDB = InventoryController.db;
                            crud_modal.Children.Add(objpnl_ItemMovement);
                        }
                    }
                }
                else
                {
                    objpnl_ItemMovement = new cntrl.Panels.pnl_ItemMovement()
                    {
                        item_inventoryList = item_inventoryitem_inventory_detailViewSource.View.OfType<item_inventory_detail>().Where(x => x.id_item_product == item_inventory_detail.id_item_product).ToList(),
                        InventoryDB = InventoryController.db
                    };

                    crud_modal.Visibility = Visibility.Visible;
                    crud_modal.Children.Add(objpnl_ItemMovement);
                }
                if (item_inventory_detail.item_product.can_expire)
                {
                    crud_modalExpire.Visibility = Visibility.Visible;
                    pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(item_inventory_detail.item_inventory.id_branch, null, item_inventory_detail.id_item_product);
                    crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.UserRole.is_master)
            {
                if (item_inventoryitem_inventory_detailViewSource.View.CurrentItem is item_inventory_detail item_inventory_detail)
                {
                    decimal cost = item_inventory_detail.unit_value;
                    item_movement item_movement = InventoryController.db.item_movement.Where(x => x.id_inventory_detail == item_inventory_detail.id_inventory_detail).FirstOrDefault();
                    if (item_movement != null)
                    {
                        item_movement.Update_ChildVales(Convert.ToDecimal(cost), true);
                    }
                }
            }
            InventoryController.db.SaveChanges();
        }

        private void location_ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (item_inventoryDataGrid.SelectedItem != null)
            {
                if (app_branchapp_locationViewSource != null)
                {
                    if (app_branchapp_locationViewSource.View != null)
                    {
                        FilterDetail();
                    }
                }
            }
        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                item_inventoryViewSource.View.Filter = i =>
                {
                    item_inventory Inventory = i as item_inventory;
                    string comment = Inventory.comment != null ? Inventory.comment: "";
                 

                    if (comment.ToLower().Contains(query.ToLower()))
                    {
                        return true;
                    }

                    return false;
                };
            }
            else
            {
                item_inventoryViewSource.View.Filter = null;
            }
        }

        private void crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                item_inventory_detail item_inventory_detail = (item_inventory_detail)item_inventoryitem_inventory_detailViewSource.View.CurrentItem;

                if (item_inventory_detail != null)
                {
                    item_movement item_movement = InventoryController.db.item_movement.Find(pnl_ItemMovementExpiry.MovementID);
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