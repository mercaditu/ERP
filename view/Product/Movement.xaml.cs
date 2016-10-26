using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Data;
using entity;

namespace Cognitivo.Product
{
    public partial class Movement : Page
    {
        ProductTransferDB ProductTransferDB = new ProductTransferDB();
        ProductMovementDB ProductMovementDB = new ProductMovementDB();
        CollectionViewSource item_transferViewSource;
        Configs.itemMovement itemMovement = new Configs.itemMovement();

        public Movement()
        {
            InitializeComponent();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            try
            {
                item_transfer item_transfer = new item_transfer();
                item_transfer.State = EntityState.Added;
                item_transfer.transfer_type = entity.item_transfer.Transfer_type.movemnent;
                item_transfer.IsSelected = true;
                item_transfer.status = Status.Transfer.Pending;
                ProductTransferDB.item_transfer.Add(item_transfer); 

                item_transferViewSource.View.MoveCurrentToLast();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                toolBar.msgWarning("Please Select a record");
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
                toolBar.msgWarning("Please Select a record");
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_transferViewSource = ((CollectionViewSource)(FindResource("item_transferViewSource")));
            ProductTransferDB.item_transfer.Where(a => a.id_company == CurrentSession.Id_Company && a.transfer_type == item_transfer.Transfer_type.movemnent).OrderByDescending(x => x.trans_date).Load();
            item_transferViewSource.Source = ProductTransferDB.item_transfer.Local;

            CollectionViewSource itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
            ProductTransferDB.items.Where(a => a.id_company == CurrentSession.Id_Company && a.item_product.Count() > 0).Load();
            itemViewSource.Source = ProductTransferDB.items.Local;

            await ProductTransferDB.app_document_range.Where(d => d.is_active == true
                                        && d.app_document.id_application == entity.App.Names.Movement
                                        && d.id_company == CurrentSession.Id_Company).Include(i => i.app_document).ToListAsync();

            cbxDocument.ItemsSource = ProductTransferDB.app_document_range.Local;

            await ProductTransferDB.app_department.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
            cbxDepartment.ItemsSource = ProductTransferDB.app_department.Local;

            await ProductTransferDB.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
            cbxBranch.ItemsSource = ProductTransferDB.app_branch.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            ProductTransferDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_dimensionViewSource.Source = ProductTransferDB.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            ProductTransferDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_measurementViewSource.Source = ProductTransferDB.app_measurement.Local;

            cbxBranch_SelectionChanged(sender, null);


        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = ProductTransferDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
                item_transfer.employee = contact;
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
                    if (ProductTransferDB.app_branch.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault() != null)
                    {
                        entity.Brillo.Logic.Range.branch_Code = ProductTransferDB.app_branch.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault().code;
                    }
                }
                if (item_transfer.id_terminal > 0)
                {
                    if (ProductTransferDB.app_terminal.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault() != null)
                    {
                        entity.Brillo.Logic.Range.terminal_Code = ProductTransferDB.app_terminal.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault().code;
                    }
                }
                if (item_transfer.id_user > 0)
                {
                    if (ProductTransferDB.security_user.Where(x => x.id_user == item_transfer.id_user).FirstOrDefault() != null)
                    {
                        entity.Brillo.Logic.Range.user_Code = ProductTransferDB.security_user.Where(x => x.id_user == item_transfer.id_user).FirstOrDefault().code;
                    }
                }
                if (item_transfer.id_project > 0)
                {
                    if (ProductTransferDB.projects.Where(x => x.id_project == item_transfer.id_project).FirstOrDefault() != null)
                    {
                        entity.Brillo.Logic.Range.project_Code = ProductTransferDB.projects.Where(x => x.id_project == item_transfer.id_project).FirstOrDefault().code;
                    }
                }

                app_document_range app_document_range = item_transfer.app_document_range;
                item_transfer.number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
            }

            item_transfer.user_requested = ProductTransferDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
            item_transfer.user_given = ProductTransferDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
            item_transfer.status = Status.Transfer.Approved;

            ProductTransferDB.SaveChanges();

            for (int i = 0; i < item_transfer_detailDataGrid.Items.Count; i++)
            {
                entity.Brillo.Logic.Stock stock = new entity.Brillo.Logic.Stock();

                item_transfer_detail item_transfer_detail = (item_transfer_detail)item_transfer_detailDataGrid.Items[i];

                List<entity.Brillo.StockList> Items_InStockLIST;
                app_currencyfx app_currencyfx = ProductMovementDB.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                app_location app_location = item_transfer_detail.item_transfer.app_location_origin;

                if (item_transfer_detail.movement_id!=null)
                {

                    if (itemMovement.item_movement != null)
                    {
                        entity.Brillo.Stock stockBrillo = new entity.Brillo.Stock();
                        Items_InStockLIST = stockBrillo.ScalarMovement(itemMovement.item_movement);
                    }
                    else
                    {
                        entity.Brillo.Stock stockBrillo = new entity.Brillo.Stock();
                        item_movement item_movement = ProductTransferDB.item_movement.Where(x => x.id_movement == item_transfer_detail.movement_id).FirstOrDefault();
                        Items_InStockLIST = stockBrillo.ScalarMovement(item_movement);
                    }
                }
               
              
                else
                {
                    entity.Brillo.Stock stockBrillo = new entity.Brillo.Stock();
                    Items_InStockLIST = stockBrillo.List(app_location.app_branch, app_location, item_transfer_detail.item_product);
                }

                ///Debit Movement from Origin.
                List<item_movement> item_movement_originList;
                item_movement_originList = stock.DebitOnly_MovementLIST(ProductMovementDB, Items_InStockLIST, Status.Stock.InStock, entity.App.Names.Transfer, item_transfer_detail.id_transfer, item_transfer_detail.id_transfer_detail, app_currencyfx, item_transfer_detail.item_product, app_location,
                        item_transfer_detail.quantity_origin, item_transfer_detail.item_transfer.trans_date, stock.comment_Generator(entity.App.Names.Movement, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""));

                ProductMovementDB.item_movement.AddRange(item_movement_originList);

                //Credit Movement to Destination
                item_movement item_movement_dest;
                item_movement parent_item_movement = item_movement_originList.FirstOrDefault();

                List<item_movement_dimension> DimensionList = null;

                if (item_movement_originList.FirstOrDefault().item_movement_dimension.Count() > 0)
                {
                    DimensionList = new List<item_movement_dimension>();
                    foreach (item_movement_dimension item_movement_dimension in item_movement_originList.FirstOrDefault().item_movement_dimension)
                    {
                        item_movement_dimension _item_movement_dimension = new item_movement_dimension();
                        _item_movement_dimension.id_dimension = item_movement_dimension.id_dimension;
                        _item_movement_dimension.value = item_movement_dimension.value;
                        DimensionList.Add(_item_movement_dimension);
                    }
                }

                item_movement_dest =
                            stock.CreditOnly_Movement(
                                Status.Stock.InStock,
                                entity.App.Names.Transfer,
                                item_transfer_detail.id_transfer,
                                item_transfer_detail.id_transfer_detail,
                                app_currencyfx,
                                item_transfer_detail.item_product,
                                item_transfer_detail.item_transfer.app_location_destination,
                                item_transfer_detail.quantity_destination,
                                item_transfer_detail.item_transfer.trans_date,
                                item_movement_originList.Sum(x => (x.item_movement_value.Sum(y => y.unit_value) / (x.item_movement_value.Count() != 0 ? x.item_movement_value.Count() : 1))),
                                stock.comment_Generator(entity.App.Names.Movement, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""),
                                DimensionList
                                );
                ProductMovementDB.item_movement.Add(item_movement_dest);
                item_transfer.status = Status.Transfer.Approved;
            }

            if (item_transfer.status == Status.Transfer.Approved && item_transfer.app_document_range != null)
            {
                entity.Brillo.Document.Start.Automatic(item_transfer, item_transfer.app_document_range);
            }

            if (ProductMovementDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ProductMovementDB.NumberOfRecords);
                itemMovement = new Configs.itemMovement();

            }
        }

        private void cbxItem_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                insertDetail();
            }
        }

        private void cbxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            insertDetail();
        }

        void insertDetail()
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            item item = ((item)cbxItem.Data);

            if (item != null)
            {
                if (item.item_dimension.Count() > 0)
                {
                    crud_modal.Children.Clear();
                    itemMovement.id_item = (int)((item)cbxItem.Data).id_item;
                    itemMovement.id_location = item_transfer.app_location_origin.id_location;
                    itemMovement.db = ProductMovementDB;

                    crud_modal.Visibility = Visibility.Visible;
                    crud_modal.Children.Add(itemMovement);
                }
                else
                {
                    if (item_transfer != null)
                    {
                        item_transfer_detail item_transfer_detail = new item_transfer_detail();
                        item_transfer_detail.id_item_product = ((item)cbxItem.Data).item_product.FirstOrDefault().id_item_product;
                        item_transfer_detail.item_product = ((item)cbxItem.Data).item_product.FirstOrDefault();
                        item_transfer_detail.quantity_destination = 1;
                        item_transfer_detail.quantity_origin = 1;

                        if (itemMovement.item_movement != null)
                        {
                            if (itemMovement.item_movement.item_movement_dimension != null)
                            {
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
                            }
                        }

                        item_transfer.item_transfer_detail.Add(item_transfer_detail);
                    }

                    CollectionViewSource item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(FindResource("item_transferitem_transfer_detailViewSource")));
                    item_transferitem_transfer_detailViewSource.View.Refresh();
                }
            }



        }

        #region Filter Data
        //private void set_ContactPrefKeyStroke(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        set_ContactPref(sender, e);
        //    }
        //}

        //private void set_ContactPref(object sender, EventArgs e)
        //{
        //    if (contactComboBox.Data != null)
        //    {
        //        int id = ((contact)contactComboBox.Data).id_contact;
        //        contact contact = dbContext.contacts.Where(x => x.id_contact == id).FirstOrDefault();
        //        contactComboBox.focusGrid = false;
        //        contactComboBox.Text = contact.name;

        //    }
        //}




        #endregion

        private void cbxBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;

            if (item_transfer != null)
            {
                CollectionViewSource location_destViewSource = ((CollectionViewSource)(FindResource("location_destViewSource")));
                location_destViewSource.Source = ProductTransferDB.app_location.Where(a => a.is_active == true && a.id_branch == item_transfer.id_branch).OrderBy(a => a.name).ToList();
               
                CollectionViewSource location_originViewSource = ((CollectionViewSource)(FindResource("location_originViewSource")));
                location_originViewSource.Source = ProductTransferDB.app_location.Where(a => a.is_active == true && a.id_branch == item_transfer.id_branch).OrderBy(a => a.name).ToList();
            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Selecteditem_movement = itemMovement.item_movement;
            if (crud_modal.Visibility == Visibility.Hidden)
            {
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
                if (item_transfer != null)
                {
                    item_transfer_detail item_transfer_detail = new item_transfer_detail();
                    item_transfer_detail.id_item_product = ((item)cbxItem.Data).item_product.FirstOrDefault().id_item_product;
                    item_transfer_detail.item_product = ((item)cbxItem.Data).item_product.FirstOrDefault();
                    if (itemMovement.item_movement!=null)
                    {
                        item_transfer_detail.movement_id = (int?)itemMovement.item_movement.id_movement;
                    }
                
                    item_transfer_detail.quantity_destination = 1;
                    item_transfer_detail.quantity_origin = 1;
                    item item = ((item)cbxItem.Data);

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
    }
}
