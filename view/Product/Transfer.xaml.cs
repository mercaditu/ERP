using System;
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
using System.Windows.Controls.Primitives;

namespace Cognitivo.Product
{
    public partial class Transfer : Page
    {
        ProductTransferDB ProductTransferDB = new ProductTransferDB();
        Class.StockCalculations StockCalculations = new Class.StockCalculations();
        CollectionViewSource item_transferViewSource, transfercostViewSource, item_transferitem_transfer_detailViewSource;
        List<Class.transfercost> clsTotalGrid = null;
        Configs.itemMovement itemMovement = new Configs.itemMovement();
      //  item_movement Selecteditem_movement;
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
            item_transfer.status = Status.Transfer.Pending;
            item_transfer.app_branch_origin = ProductTransferDB.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault();
            ProductTransferDB.Entry(item_transfer).State = EntityState.Added;

            item_transferViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            item_transfer item_transfer = (item_transfer)item_transferViewSource.View.CurrentItem;
            item_transfer.app_branch_destination = (id_branch_destinComboBox.SelectedItem as app_branch);
            item_transfer.app_branch_origin = (id_branch_originComboBox.SelectedItem as app_branch);
            item_transfer.app_location_destination = (id_branch_destinComboBox.SelectedItem as app_branch).app_location.Where(x => x.is_default).FirstOrDefault();
            item_transfer.app_location_origin = (id_branch_originComboBox.SelectedItem as app_branch).app_location.Where(x => x.is_default).FirstOrDefault();
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
                a.transfer_type == item_transfer.Transfer_type.transfer)
                //.Include(i => i.item_transfer_detail)
                .Load();
            item_transferViewSource.Source = ProductTransferDB.item_transfer.Local;

            item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(this.FindResource("item_transferitem_transfer_detailViewSource")));

            ProductTransferDB.app_branch.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company && a.app_location.Count > 0).OrderBy(a => a.name).Load();
            CollectionViewSource branch_originViewSource = ((CollectionViewSource)(this.FindResource("branch_originViewSource")));
            branch_originViewSource.Source = ProductTransferDB.app_branch.Local;
            CollectionViewSource branch_destViewSource = ((CollectionViewSource)(this.FindResource("branch_destViewSource")));
            branch_destViewSource.Source = ProductTransferDB.app_branch.Local;

            CollectionViewSource security_userViewSource = ((CollectionViewSource)(this.FindResource("security_userViewSource")));
            ProductTransferDB.security_user.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            security_userViewSource.Source = ProductTransferDB.security_user.Local;

            clsTotalGrid = new List<Class.transfercost>();
            transfercostViewSource = this.FindResource("transfercostViewSource") as CollectionViewSource;
            transfercostViewSource.Source = clsTotalGrid;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.Transfer, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
        }

        private void SmartBox_Item_Select(object sender, RoutedEventArgs e)
        {
            
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            item item = ProductTransferDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

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
                        item.id_item_type != entity.item.item_type.Task &&
                        item.id_item_type != entity.item.item_type.Service &&
                        item.id_item_type != entity.item.item_type.ServiceContract)
                    {
                        item_transfer_detail item_transfer_detail = new item_transfer_detail();

                        item_transfer_detail.status = Status.Documents_General.Pending;
                        item_transfer_detail.quantity_origin = 1;

                        int BranchID = (int)id_branch_originComboBox.SelectedValue;

                        item_transfer_detail.Quantity_InStock = StockCalculations.StockCount_ByBranch(BranchID, item.id_item, DateTime.Now);
                        item_transfer_detail.timestamp = DateTime.Now;
                        item_transfer_detail.item_product = item.item_product.FirstOrDefault();
                        item_transfer_detail.id_item_product = item_transfer_detail.item_product.id_item_product;
                        item_transfer_detail.RaisePropertyChanged("item_product");

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

           
        }

        private void toolBar_btnApproveOrigin_Click(object sender)
        {
            try
            {
                item_transfer item_transfer = (item_transfer)item_transferViewSource.View.CurrentItem;
                item_transfer.app_branch_destination = (id_branch_destinComboBox.SelectedItem as app_branch);
                item_transfer.app_branch_origin = (id_branch_originComboBox.SelectedItem as app_branch);
                item_transfer.app_location_destination = (id_branch_destinComboBox.SelectedItem as app_branch).app_location.Where(x => x.is_default).FirstOrDefault();
                item_transfer.app_location_origin = (id_branch_originComboBox.SelectedItem as app_branch).app_location.Where(x => x.is_default).FirstOrDefault();

                TransferSetting TransferSetting = new Product.TransferSetting();

                int NumberofRecords = ProductTransferDB.ApproveOrigin((int)id_branch_originComboBox.SelectedValue, (int)id_branch_destinComboBox.SelectedValue, TransferSetting.movebytruck);
                toolBar.msgSaved(NumberofRecords);
                item_transferViewSource.View.Refresh();
                 
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnApproveDestination_Click(object sender)
        {
            TransferSetting TransferSetting = new Product.TransferSetting();

            item_transfer item_transfer = (item_transfer)itemDataGrid.SelectedItem;
            item_transfer.IsSelected = true;

            //clsTotalGrid = (List<Class.transfercost>)transfercostViewSource.Source;
            int NumberOfRecords = ProductTransferDB.ApproveDestination((int)id_branch_originComboBox.SelectedValue, (int)id_branch_destinComboBox.SelectedValue, TransferSetting.movebytruck);
            if (NumberOfRecords > 0)
            {
                itemMovement = new Configs.itemMovement();
                toolBar.msgSaved(NumberOfRecords);

               
            }
        }

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            TransferSetting TransferSetting = new TransferSetting();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            TransferSetting.Default.Save();
            TransferSetting = TransferSetting.Default;
            popupCustomize.IsOpen = false;
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            if (item_transfer != null)
            {
                if (item_transfer.app_document_range!=null)
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

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
         
            item item = ProductTransferDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            //Selecteditem_movement = itemMovement.item_movement;
            if (crud_modal.Visibility == Visibility.Hidden)
            {

                if (item != null &&
                          item.item_product != null &&
                          item_transfer != null &&
                          item.id_item_type != entity.item.item_type.Task &&
                          item.id_item_type != entity.item.item_type.Service &&
                          item.id_item_type != entity.item.item_type.ServiceContract)
                {
                    item_transfer_detail item_transfer_detail = new item_transfer_detail();

                    item_transfer_detail.status = Status.Documents_General.Pending;
                    item_transfer_detail.quantity_origin = 1;
                  
                    item_transfer_detail.timestamp = DateTime.Now;
                    item_transfer_detail.movement_id = (int)itemMovement.item_movement.id_movement;
                    item_transfer_detail.item_product = item.item_product.FirstOrDefault();
                    item_transfer_detail.id_item_product = item_transfer_detail.item_product.id_item_product;
                    item_transfer_detail.Quantity_InStock = StockCalculations.StockCount_ByBranch((int)id_branch_originComboBox.SelectedValue, item_transfer_detail.item_product.id_item, DateTime.Now);
                    item_transfer_detail.RaisePropertyChanged("item_product");
                    foreach (item_movement_dimension item_movement_dimension in itemMovement.item_movement.item_movement_dimension)
                    {
                        item_transfer_dimension item_transfer_dimension = new item_transfer_dimension();
                        item_transfer_dimension.id_transfer_detail = item_transfer_detail.id_transfer_detail;
                        item_transfer_dimension.id_dimension = item_movement_dimension.id_dimension;
                        if (ProductTransferDB.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault() != null)
                        {
                            item_transfer_dimension.app_dimension = ProductTransferDB.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault();

                        }
                        item_transfer_dimension.value = item_movement_dimension.value;
                        item_transfer_detail.item_transfer_dimension.Add(item_transfer_dimension);
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
    }
}
