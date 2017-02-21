using entity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Product
{
    public partial class Transfer : Page
    {
        private ProductTransferDB ProductTransferDB = new ProductTransferDB();
        private Class.StockCalculations StockCalculations = new Class.StockCalculations();
        private CollectionViewSource item_transferViewSource, item_transferitem_transfer_detailViewSource;
        private Configs.itemMovement itemMovement = new Configs.itemMovement();
        private cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry;

        public Transfer()
        {
            InitializeComponent();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            item_transfer item_transfer = new item_transfer();
            item_transfer.State = EntityState.Added;
            item_transfer.transfer_type = entity.item_transfer.Transfer_type.transfer;
            item_transfer.IsSelected = true;
            item_transfer.status = Status.Transfer.Pending;
            item_transfer.app_branch_origin = ProductTransferDB.app_branch.Find(CurrentSession.Id_Branch);
            ProductTransferDB.Entry(item_transfer).State = EntityState.Added;

            item_transferViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            item_transfer item_transfer = (item_transfer)item_transferViewSource.View.CurrentItem;
            if (item_transfer != null)
            {
                item_transfer.app_branch_destination = (id_branch_destinComboBox.SelectedItem as app_branch);
                item_transfer.app_branch_origin = (id_branch_originComboBox.SelectedItem as app_branch);
                item_transfer.app_location_destination = (id_branch_destinComboBox.SelectedItem as app_branch).app_location.Where(x => x.is_default).FirstOrDefault();
                item_transfer.app_location_origin = (id_branch_originComboBox.SelectedItem as app_branch).app_location.Where(x => x.is_default).FirstOrDefault();
            }

            ProductTransferDB.SaveChanges();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ProductTransferDB.CancelAllChanges();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            item_transfer item_transfer = itemDataGrid.SelectedItem as item_transfer;

            if (item_transfer != null)
            {
                item_transfer.IsSelected = true;
                item_transfer.State = EntityState.Modified;
                ProductTransferDB.Entry(item_transfer).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Filter by branch.
            item_transferViewSource = ((CollectionViewSource)(this.FindResource("item_transferViewSource")));
            await ProductTransferDB.item_transfer.Where(a =>
                    a.id_company == CurrentSession.Id_Company &&
                    a.transfer_type == item_transfer.Transfer_type.transfer).Include(x => x.app_branch_destination).Include(y => y.app_branch_origin).OrderByDescending(x => x.trans_date)
                    .LoadAsync();
            item_transferViewSource.Source = ProductTransferDB.item_transfer.Local;

            item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(this.FindResource("item_transferitem_transfer_detailViewSource")));

            await ProductTransferDB.app_branch.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company && a.app_location.Count > 0).OrderBy(a => a.name).LoadAsync();

            CollectionViewSource branch_originViewSource = ((CollectionViewSource)(this.FindResource("branch_originViewSource")));
            branch_originViewSource.Source = ProductTransferDB.app_branch.Local;

            CollectionViewSource branch_destViewSource = ((CollectionViewSource)(this.FindResource("branch_destViewSource")));
            branch_destViewSource.Source = ProductTransferDB.app_branch.Local;

            CollectionViewSource security_userViewSource = ((CollectionViewSource)(this.FindResource("security_userViewSource")));
            await ProductTransferDB.security_user.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).LoadAsync();
            security_userViewSource.Source = ProductTransferDB.security_user.Local;

            ProductTransferDB.app_measurement.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            CollectionViewSource app_measurevolume = FindResource("app_measurevolume") as CollectionViewSource;
            CollectionViewSource app_measureweight = FindResource("app_measureweight") as CollectionViewSource;
            app_measurevolume.Source = ProductTransferDB.app_measurement.Local;
            app_measureweight.Source = ProductTransferDB.app_measurement.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(ProductTransferDB, entity.App.Names.Transfer, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
        }

        private void SmartBox_Item_Select(object sender, RoutedEventArgs e)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            item item = ProductTransferDB.items.Find(sbxItem.ItemID);

            if (item != null)
            {
                if (item.item_dimension.Count() > 0)
                {
                    crud_modal.Children.Clear();
                    itemMovement.id_item = item.id_item;
                    itemMovement.id_location = item_transfer.app_branch_origin.app_location.FirstOrDefault().id_location;
                    itemMovement.db = ProductTransferDB;

                    crud_modal.Visibility = Visibility.Visible;
                    crud_modal.Children.Add(itemMovement);
                }
                else
                {
                    if (item != null &&
                        item.item_product != null &&
                        item_transfer != null &&
                        item.id_item_type != item.item_type.Task &&
                        item.id_item_type != item.item_type.Service &&
                        item.id_item_type != item.item_type.ServiceContract)
                    {
                        if (item_transfer.item_transfer_detail.Where(a => a.id_item_product == item.item_product.FirstOrDefault().id_item_product).FirstOrDefault() == null)
                        {
                            if (item.item_product.FirstOrDefault() != null && item.item_product.FirstOrDefault().can_expire)
                            {
                                crud_modalExpire.Visibility = Visibility.Visible;
                                pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(item_transfer.app_branch_origin.id_branch, null, item.item_product.FirstOrDefault().id_item_product);
                                crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                            }
                            else
                            {
                                item_transfer_detail item_transfer_detail = new item_transfer_detail();

                                item_transfer_detail.status = Status.Documents_General.Pending;
                                item_transfer_detail.quantity_origin = 1;

                                int BranchID = (int)id_branch_originComboBox.SelectedValue;
                                decimal? stock = sbxItem.QuantityInStock; //StockCalculations.Count_ByBranch(BranchID, item.id_item, DateTime.Now);
                                item_transfer_detail.Quantity_InStock = Convert.ToDecimal(stock != null ? stock : 0);

                                item_transfer_detail.timestamp = DateTime.Now;
                                item_transfer_detail.item_product = item.item_product.FirstOrDefault();
                                item_transfer_detail.id_item_product = item_transfer_detail.item_product.id_item_product;
                                item_transfer_detail.RaisePropertyChanged("item_product");
                                item_transfer.item_transfer_detail.Add(item_transfer_detail);
                            }
                        }
                        else
                        {
                            item_transfer_detail item_transfer_detail = item_transfer.item_transfer_detail.Where(a => a.id_item_product == item.item_product.FirstOrDefault().id_item_product).FirstOrDefault();
                            item_transfer_detail.quantity_origin += 1;
                        }
                    }
                    else
                    {
                        toolBar.msgWarning("Item Error");
                    }

                    item_transferViewSource.View.Refresh();
                    item_transferitem_transfer_detailViewSource.View.Refresh();
                }
            }
        }

        private void toolBar_btnApproveOrigin_Click(object sender)
        {
            item_transfer item_transfer = (item_transfer)itemDataGrid.SelectedItem;

            if (item_transfer != null)
            {
                item_transfer.IsSelected = true;
                ProductTransferDB.ApproveOrigin(item_transfer, false);
            }
        }

        private void toolBar_btnApproveDestination_Click(object sender)
        {
            item_transfer item_transfer = (item_transfer)itemDataGrid.SelectedItem;

            if (item_transfer != null)
            {
                item_transfer.IsSelected = true;
                ProductTransferDB.ApproveOrigin(item_transfer, false);
                ProductTransferDB.ApproveDestination(item_transfer, false);
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            if (item_transfer != null)
            {
                if (item_transfer.app_document_range != null)
                {
                    entity.Brillo.Document.Start.Manual(item_transfer, item_transfer.app_document_range);
                }
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_transfer_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
                    //DeleteDetailGridRow
                    item_transfer_detailDataGrid.CancelEdit();
                    ProductTransferDB.item_transfer_detail.Remove(e.Parameter as item_transfer_detail);
                    item_transferitem_transfer_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void btnCheckDestination_Click(object sender, RoutedEventArgs e)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            ToggleButton button = sender as ToggleButton;

            if (item_transfer != null && button != null)
            {
                foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail)
                {
                    if ((bool)button.IsChecked)
                    {
                        item_transfer_detail.IsSelected = true;
                    }
                    else
                    {
                        item_transfer_detail.IsSelected = false;
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtsearch.Text != string.Empty)
            {
                if (item_transferitem_transfer_detailViewSource != null)
                {
                    if (item_transferitem_transfer_detailViewSource.View != null)
                    {
                        item_transferitem_transfer_detailViewSource.View.Filter = i =>
                        {
                            item_transfer_detail item_transfer_detail = (item_transfer_detail)i;
                            item item = item_transfer_detail.item_product != null ? item_transfer_detail.item_product.item != null ? item_transfer_detail.item_product.item : null : null;
                            if (item != null)
                            {
                                if (item.name.ToUpper().Contains(txtsearch.Text.ToUpper()) || item.code.ToUpper().Contains(txtsearch.Text.ToUpper()))
                                    return true;
                                else
                                    return false;
                            }

                            return false;
                        };
                    }
                }
            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            item item = ProductTransferDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;

            if (crud_modal.Visibility == Visibility.Hidden)
            {
                if (item != null &&
                          item.item_product != null &&
                          item_transfer != null &&
                          item.id_item_type != item.item_type.Task &&
                          item.id_item_type != item.item_type.Service &&
                          item.id_item_type != item.item_type.ServiceContract)
                {
                    item_transfer_detail item_transfer_detail = new item_transfer_detail();

                    item_transfer_detail.status = Status.Documents_General.Pending;
                    item_transfer_detail.quantity_origin = 1;

                    item_transfer_detail.timestamp = DateTime.Now;
                  
                  
                    item_transfer_detail.item_product = item.item_product.FirstOrDefault();
                    item_transfer_detail.id_item_product = item_transfer_detail.item_product.id_item_product;
                    item_transfer_detail.Quantity_InStock = sbxItem.QuantityInStock; //(decimal)StockCalculations.Count_ByBranch((int)id_branch_originComboBox.SelectedValue, item_transfer_detail.item_product.id_item, DateTime.Now);
                    item_transfer_detail.RaisePropertyChanged("item_product");

                    if (itemMovement.item_movement != null)
                    {
                        item_transfer_detail.movement_id = (int)itemMovement.item_movement.id_movement;
                        foreach (item_movement_dimension item_movement_dimension in itemMovement.item_movement.item_movement_dimension)
                        {
                            item_transfer_dimension item_transfer_dimension = new item_transfer_dimension();
                            item_transfer_dimension.item_transfer_detail = item_transfer_detail;
                            item_transfer_dimension.id_dimension = item_movement_dimension.id_dimension;
                            if (ProductTransferDB.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault() != null)
                            {
                                item_transfer_dimension.app_dimension = ProductTransferDB.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault();
                            }

                            item_transfer_dimension.value = item_movement_dimension.value;
                            item_transfer_detail.item_transfer_dimension.Add(item_transfer_dimension);
                        }
                    }
                    
                    item_transfer.item_transfer_detail.Add(item_transfer_detail);
                }
                else
                {
                    toolBar.msgWarning("Item Error");
                }

                item_transferViewSource.View.Refresh();
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
                        string number = item_transfer.number;
                        string origin = item_transfer.app_branch_origin != null ? item_transfer.app_branch_origin.name : "";

                        if (number != null)
                        {
                            if (number.ToLower().Contains(query.ToLower()) || origin.ToLower().Contains(query.ToLower()))
                            {
                                return true;
                            }
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

        private void sbxItemAsset_Select(object sender, RoutedEventArgs e)
        {
            //if (sbxItemAsset.ItemID>0)
            //{
            //    item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            //    if (item_transfer!=null)
            //    {
            //        item_transfer.id_item_asset = sbxItemAsset.ItemID;
            //    }
            //}
        }

        private async void crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
                item item = await ProductTransferDB.items.FindAsync(sbxItem.ItemID);

                if (item != null && item.id_item > 0 && item_transfer != null)
                {
                    item_transfer_detail item_transfer_detail = new item_transfer_detail();
                    item_movement item_movement = ProductTransferDB.item_movement.Find(pnl_ItemMovementExpiry.MovementID);

                    if (item_transfer_detail != null)
                    {
                        item_transfer_detail.movement_id = (int)item_movement.id_movement;
                        item_transfer_detail.batch_code = item_movement.code;
                        item_transfer_detail.expire_date = item_movement.expire_date;
                    }

                    item_transfer_detail.status = Status.Documents_General.Pending;
                    item_transfer_detail.quantity_origin = 1;

                    int BranchID = (int)id_branch_originComboBox.SelectedValue;
                    decimal? stock = sbxItem.QuantityInStock; //StockCalculations.Count_ByBranch(BranchID, item.id_item, DateTime.Now);
                    item_transfer_detail.Quantity_InStock = Convert.ToDecimal(stock != null ? stock : 0);

                    item_transfer_detail.timestamp = DateTime.Now;
                    item_transfer_detail.item_product = item.item_product.FirstOrDefault();
                    item_transfer_detail.id_item_product = item_transfer_detail.item_product.id_item_product;
                    item_transfer_detail.RaisePropertyChanged("item_product");
                    item_transfer.item_transfer_detail.Add(item_transfer_detail);
                }
            }
        }
    }
}