using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Product
{
    public partial class Movement : Page
    {
        private ProductTransferDB ProductTransferDB = new ProductTransferDB();
        private CollectionViewSource item_transferViewSource;
        private cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry;

        public Movement()
        {
            InitializeComponent();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            item_transfer item_transfer = new item_transfer();
            item_transfer.State = EntityState.Added;
            item_transfer.trans_date = DateTime.Now;
            item_transfer.transfer_type = entity.item_transfer.Transfer_Types.Movement;
            item_transfer.IsSelected = true;
            item_transfer.status = Status.Transfer.Pending;
            ProductTransferDB.item_transfer.Add(item_transfer);

            item_transferViewSource.View.MoveCurrentToLast();
            cbxBranch_SelectionChanged(sender, null);
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_transfer_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                //DeleteDetailGridRow
                if (e.Parameter as item_transfer_detail != null)
                {
                    //ontact_field_valueDataGrid.CancelEdit();
                    ProductTransferDB.item_transfer_detail.Remove(e.Parameter as item_transfer_detail);
                    CollectionViewSource item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(FindResource("item_transferitem_transfer_detailViewSource")));
                    item_transferitem_transfer_detailViewSource.View.Refresh();
                }

            }
        }


        private void toolBar_btnSave_Click(object sender)
        {
            if (ProductTransferDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ProductTransferDB.NumberOfRecords);
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                ProductTransferDB.item_transfer.Remove((item_transfer)item_transferDataGrid.SelectedItem);
                item_transferViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ProductTransferDB.CancelAllChanges();
            if (item_transferDataGrid.SelectedItem != null)
            {
                item_transfer item_transfer = (item_transfer)item_transferDataGrid.SelectedItem;
                item_transfer.IsSelected = true;
                item_transfer.State = EntityState.Unchanged;
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (item_transferDataGrid.SelectedItem != null)
            {
                item_transfer item_transfer = (item_transfer)item_transferDataGrid.SelectedItem;
                item_transfer.IsSelected = true;
                item_transfer.State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_transferViewSource = FindResource("item_transferViewSource") as CollectionViewSource;
            await ProductTransferDB.item_transfer
                .Where(a =>
                    a.id_company == CurrentSession.Id_Company &&
                    a.id_branch == CurrentSession.Id_Branch &&
                    a.transfer_type == item_transfer.Transfer_Types.Movement
                )
                .OrderByDescending(x => x.trans_date)
                .LoadAsync();
            item_transferViewSource.Source = ProductTransferDB.item_transfer.Local;

            await ProductTransferDB.app_document_range.Where(d => d.is_active == true
                                        && d.app_document.id_application == entity.App.Names.Movement
                                        && d.id_company == CurrentSession.Id_Company).Include(i => i.app_document).ToListAsync();

            cbxDocument.ItemsSource = ProductTransferDB.app_document_range.Local;

            await ProductTransferDB.app_department.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
            cbxDepartment.ItemsSource = ProductTransferDB.app_department.Local;

            CollectionViewSource app_dimensionViewSource = FindResource("app_dimensionViewSource") as CollectionViewSource;
            await ProductTransferDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).ToListAsync();
            app_dimensionViewSource.Source = ProductTransferDB.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = FindResource("app_measurementViewSource") as CollectionViewSource;
            await ProductTransferDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToListAsync();
            app_measurementViewSource.Source = ProductTransferDB.app_measurement.Local;

            CollectionViewSource branchViewSource = FindResource("branchViewSource") as CollectionViewSource;
            await ProductTransferDB.app_branch
                .Where(b => b.can_stock == true && b.is_active == true && b.id_company == CurrentSession.Id_Company && b.id_branch == CurrentSession.Id_Branch)
                .Include(x => x.app_location)
                .OrderBy(b => b.name).ToListAsync();
            branchViewSource.Source = ProductTransferDB.app_branch.Local; //ProductTransferDB.app_branch.Local;

            if (ProductTransferDB.app_branch.Local.Count() == 0)
            {
                toolBar.msgWarning("Your current branch (" + CurrentSession.Branches.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault().name + "), is not authorized. ");
            }

            CollectionViewSource location_originViewSource = FindResource("location_originViewSource") as CollectionViewSource;
            CollectionViewSource location_destViewSource = FindResource("location_destViewSource") as CollectionViewSource;
            location_originViewSource.Source = ProductTransferDB.app_location.Local;
            location_destViewSource.Source = ProductTransferDB.app_location.Local;

            cbxBranch_SelectionChanged(sender, null);
        }

        private void cbxBranch_SelectionChanged(object sender, EventArgs e)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            if (item_transfer != null)
            {
                if (item_transfer.State == EntityState.Added || item_transfer.State == EntityState.Modified)
                {
                    app_location app_location = ProductTransferDB.app_location.Where(x => x.is_default && x.id_branch == item_transfer.id_branch).FirstOrDefault();
                    CollectionViewSource location_originViewSource = FindResource("location_originViewSource") as CollectionViewSource;
                    if (location_originViewSource != null)
                    {
                        if (location_originViewSource.View != null)
                        {
                            location_originViewSource.View.MoveCurrentTo(app_location);
                        }
                    }
                }
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = ProductTransferDB.contacts.Find(sbxContact.ContactID);
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;

                if (contact != null && item_transfer != null)
                {
                    item_transfer.employee = contact;
                }
            }
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;

            if ((item_transfer.number == null || item_transfer.number == string.Empty) && item_transfer.app_document_range != null)
            {
                entity.Brillo.Logic.Document _Document = new entity.Brillo.Logic.Document();
                if (item_transfer.id_branch > 0)
                {
                    app_branch app_branch = CurrentSession.Branches.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault();
                    if (app_branch != null)
                    {
                        entity.Brillo.Logic.Range.branch_Code = app_branch.code;
                    }
                }
                if (item_transfer.id_terminal > 0)
                {
                    app_terminal app_terminal = CurrentSession.Terminals.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault();
                    if (app_terminal != null)
                    {
                        entity.Brillo.Logic.Range.terminal_Code = app_terminal.code;
                    }
                }
                if (item_transfer.id_user > 0)
                {
                    security_user security_user = ProductTransferDB.security_user.Find(item_transfer.id_user);
                    if (security_user != null)
                    {
                        entity.Brillo.Logic.Range.user_Code = security_user.code;
                    }
                }
                if (item_transfer.id_project > 0)
                {
                    project projects = ProductTransferDB.projects.Find(item_transfer.id_project);
                    if (projects != null)
                    {
                        entity.Brillo.Logic.Range.project_Code = projects.code;
                    }
                }

                app_document_range app_document_range = item_transfer.app_document_range;
                item_transfer.number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
            }

            //item_transfer.user_requested = ProductTransferDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
            //item_transfer.user_given = ProductTransferDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
            item_transfer.status = Status.Transfer.Approved;

            ProductTransferDB.SaveChanges();

           
            CurrentItems.getProducts_InStock(item_transfer.id_branch, DateTime.Now, true);

            for (int i = 0; i < item_transfer_detailDataGrid.Items.Count; i++)
            {
                entity.Brillo.Logic.Stock stock = new entity.Brillo.Logic.Stock();
                item_transfer_detail item_transfer_detail = (item_transfer_detail)item_transfer_detailDataGrid.Items[i];

                List<entity.Brillo.StockList> Items_InStockLIST = new List<entity.Brillo.StockList>();
                app_currencyfx app_currencyfx = ProductTransferDB.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                app_location app_location = item_transfer_detail.item_transfer.app_location_origin;

                if (item_transfer_detail.movement_id != null)
                {
                    if (item_transfer_detail.movement_id > 0)
                    {
                        entity.Brillo.Stock stockBrillo = new entity.Brillo.Stock();
                        Items_InStockLIST = stockBrillo.ScalarMovement((long)item_transfer_detail.movement_id);
                    }
                }
                else
                {

                    Items_InStockLIST = CurrentItems.getProducts_InStock(app_location.id_branch, DateTime.Now, false).Where(x => x.LocationID == app_location.id_location && x.ProductID == item_transfer_detail.id_item_product).ToList();

                }

                ///Debit Movement from Origin.
                List<item_movement> item_movement_originList;
                item_movement_originList = stock.DebitOnly_MovementLIST(ProductTransferDB, Items_InStockLIST, Status.Stock.InStock, entity.App.Names.Transfer, item_transfer_detail.id_transfer, item_transfer_detail.id_transfer_detail, app_currencyfx.id_currencyfx, item_transfer_detail.item_product, app_location.id_location,
                        item_transfer_detail.quantity_origin, item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(entity.App.Names.Movement, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""));

                ProductTransferDB.item_movement.AddRange(item_movement_originList);

                //Credit Movement to Destination
                foreach (item_movement item_movement in item_movement_originList)
                {
                    item_movement item_movement_dest;

                    List<item_movement_dimension> DimensionList = null;

                    if (item_movement.item_movement_dimension.Count() > 0)
                    {
                        DimensionList = new List<item_movement_dimension>();
                        foreach (item_movement_dimension item_movement_dimension in item_movement.item_movement_dimension)
                        {
                            item_movement_dimension _item_movement_dimension = new item_movement_dimension();
                            _item_movement_dimension.id_dimension = item_movement_dimension.id_dimension;
                            _item_movement_dimension.value = item_movement_dimension.value;
                            DimensionList.Add(_item_movement_dimension);
                        }
                    }
                    decimal Unit_cost = 0;
                    if (item_movement.id_movement_value_rel > 0)
                    {
                        item_movement_value_rel item_movement_value_rel = ProductTransferDB.item_movement_value_rel.Where(x => x.id_movement_value_rel == item_movement.id_movement_value_rel).FirstOrDefault();
                        if (item_movement_value_rel != null)
                        {
                            Unit_cost = item_movement_value_rel.total_value;
                        }

                    }

                    item_movement_dest =
                                stock.CreditOnly_Movement(
                                    Status.Stock.InStock,
                                    entity.App.Names.Transfer,
                                    item_transfer_detail.id_transfer,
                                    item_transfer_detail.id_transfer_detail,
                                    app_currencyfx.id_currencyfx,
                                    item_transfer_detail.id_item_product,
                                    item_transfer_detail.item_transfer.app_location_destination.id_location,
                                    item_movement.debit,
                                    item_transfer_detail.item_transfer.trans_date,
                                    Unit_cost,
                                    stock.comment_Generator(entity.App.Names.Movement, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""),
                                    DimensionList, item_transfer_detail.expire_date, item_transfer_detail.batch_code, item_movement.parent
                                    );

                    //   item_movement_dest.parent = item_movement.parent;
                    item_movement.barcode = item_movement.parent != null ? item_movement.parent.barcode : entity.Brillo.Barcode.RandomGenerator();

                    ProductTransferDB.item_movement.Add(item_movement_dest);

                    item_transfer.status = Status.Transfer.Approved;
                }
            }

            if (item_transfer.status == Status.Transfer.Approved && item_transfer.app_document_range != null)
            {
                entity.Brillo.Document.Start.Automatic(item_transfer, item_transfer.app_document_range);
            }

            if (ProductTransferDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ProductTransferDB.NumberOfRecords);
            }
        }

        private void cbxItem_KeyDown(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
                item item = ProductTransferDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                item_product item_product = item.item_product.FirstOrDefault();

                if (item != null && item_transfer != null)
                {
                    if (item.item_dimension.Count() > 0)
                    {
                        crud_modal.Children.Clear();
                        Configs.itemMovement itemMovement = new Configs.itemMovement();

                        itemMovement.id_item = item.id_item;
                        itemMovement.id_location = item_transfer.app_location_origin.id_location;
                        itemMovement.db = ProductTransferDB;
                        itemMovement.Save += ItemMovement_Save;
                        crud_modal.Visibility = Visibility.Visible;
                        crud_modal.Children.Add(itemMovement);
                    }
                    else if (item_product != null && item_product.can_expire)
                    {
                        crud_modalExpire.Visibility = Visibility.Visible;
                        pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(null, item_transfer.app_location_origin.id_location, item_product.id_item_product);
                        crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                    }
                    else
                    {
                        if (item_product != null)
                        {
                            item_transfer_detail item_transfer_detail = new item_transfer_detail();
                            item_transfer_detail.id_item_product = item_product.id_item_product;
                            item_transfer_detail.item_product = item_product;
                            item_transfer_detail.quantity_destination = 1;
                            item_transfer_detail.quantity_origin = 1;

                            item_transfer.item_transfer_detail.Add(item_transfer_detail);
                        }

                        CollectionViewSource item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(FindResource("item_transferitem_transfer_detailViewSource")));
                        item_transferitem_transfer_detailViewSource.View.Refresh();
                    }
                }
            }
        }

        private void ItemMovement_Save(object sender, RoutedEventArgs e)
        {
            Configs.itemMovement itemMovement = sender as Configs.itemMovement;
            if (itemMovement != null)
            {
                item item = ProductTransferDB.items.Find(sbxItem.ItemID);
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;

                if (item_transfer != null)
                {
                    item_transfer_detail item_transfer_detail = new item_transfer_detail();
                    item_transfer_detail.id_item_product = item.item_product.FirstOrDefault().id_item_product;
                    item_transfer_detail.item_product = item.item_product.FirstOrDefault();

                    if (itemMovement.item_movement != null)
                    {
                        item_transfer_detail.movement_id = (int?)itemMovement.item_movement.id_movement;
                    }

                    item_transfer_detail.quantity_destination = 1;
                    item_transfer_detail.quantity_origin = 1;

                    if (itemMovement.item_movement != null)
                    {
                        foreach (item_movement_dimension item_movement_dimension in itemMovement.item_movement.item_movement_dimension)
                        {
                            item_transfer_dimension item_transfer_dimension = new item_transfer_dimension();
                            item_transfer_dimension.item_transfer_detail = item_transfer_detail;
                            item_transfer_dimension.id_dimension = item_movement_dimension.id_dimension;

                            app_dimension app_dimension = ProductTransferDB.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault();
                            if (app_dimension != null)
                            {
                                item_transfer_dimension.app_dimension = app_dimension;
                            }

                            item_transfer_dimension.value = item_movement_dimension.value;
                            item_transfer_detail.item_transfer_dimension.Add(item_transfer_dimension);
                        }
                    }

                    item_transfer.item_transfer_detail.Add(item_transfer_detail);
                }

                CollectionViewSource item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(FindResource("item_transferitem_transfer_detailViewSource")));
                item_transferitem_transfer_detailViewSource.View.Refresh();
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                item_transferViewSource.View.Filter = i =>
                {
                    item_transfer item_transfer = i as item_transfer;
                    if (item_transfer != null)
                    {
                        string number = item_transfer.number != null ? item_transfer.number : "";
                        string origin = item_transfer.app_location_origin.name != null ? item_transfer.app_location_origin.name : "";
                        //string
                        if (number.ToLower().Contains(query.ToLower()) || origin.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                    }
                    return false;
                };
            }
            else
            {
                item_transferViewSource.View.Filter = null;
            }
        }

        private void toolBar_btnPrint_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            if (item_transfer.status == Status.Transfer.Approved && item_transfer.app_document_range != null)
            {
                entity.Brillo.Document.Start.Automatic(item_transfer, item_transfer.app_document_range);
            }
        }

        private void crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
                item item = ProductTransferDB.items.Find(sbxItem.ItemID);
                item_product item_product = item.item_product.FirstOrDefault();

                if (item != null && item.id_item > 0 && item_transfer != null)
                {
                    item_transfer_detail item_transfer_detail = new item_transfer_detail();
                    item_transfer_detail.id_item_product = item_product.id_item_product;
                    item_transfer_detail.item_product = item_product;
                    item_transfer_detail.quantity_destination = 1;
                    item_transfer_detail.quantity_origin = 1;

                    item_movement item_movement = ProductTransferDB.item_movement.Find(pnl_ItemMovementExpiry.MovementID);

                    if (item_movement != null)
                    {
                        item_transfer_detail.batch_code = item_movement.code;
                        item_transfer_detail.expire_date = item_movement.expire_date;

                        item_transfer_detail.movement_id = (int)item_movement.id_movement;
                        item_transfer_detail.Quantity_InStockLot = item_movement.avlquantity;
                    }

                    item_transfer.item_transfer_detail.Add(item_transfer_detail);

                    CollectionViewSource item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(FindResource("item_transferitem_transfer_detailViewSource")));
                    item_transferitem_transfer_detailViewSource.View.Refresh();
                }


            }
        }

        private void chbxRowDetail_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;
            if ((bool)chbx.IsChecked)
            {
                item_transfer_detailDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                item_transfer_detailDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }
    }
}