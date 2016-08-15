using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Data;
using entity;
using System.Data.Entity.Validation;
using System.Windows.Input;
using entity.Brillo;

namespace Cognitivo.Product
{
    public partial class Movement : Page
    {
        ProductTransferDB dbContext = new ProductTransferDB();
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
                dbContext.Entry(item_transfer).State = EntityState.Added;

                item_transferViewSource.View.MoveCurrentToLast();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                toolBar.msgSaved(dbContext.NumberOfRecords);
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                dbContext.item_transfer.Remove((item_transfer)item_transferDataGrid.SelectedItem);
                item_transferViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_transferViewSource = ((CollectionViewSource)(FindResource("item_transferViewSource")));
            dbContext.item_transfer.Where(a => a.id_company == CurrentSession.Id_Company && a.transfer_type == item_transfer.Transfer_type.movemnent).Include("item_transfer_detail").Load();
            item_transferViewSource.Source = dbContext.item_transfer.Local;

            CollectionViewSource itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
            dbContext.items.Where(a => a.id_company == CurrentSession.Id_Company && a.item_product.Count() > 0).Load();
            itemViewSource.Source = dbContext.items.Local;

            dbContext.app_document_range.Where(d => d.is_active == true
                                        && d.app_document.id_application == entity.App.Names.Movement
                                        && d.id_company == CurrentSession.Id_Company).Include(i => i.app_document).ToList();

            cbxDocument.ItemsSource = dbContext.app_document_range.Local;

            dbContext.app_department.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            cbxDepartment.ItemsSource = dbContext.app_department.Local;

            //dbContext.projects.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            //cbxProject.ItemsSource = dbContext.projects.Local;

            dbContext.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            cbxBranch.ItemsSource = dbContext.app_branch.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            dbContext.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_dimensionViewSource.Source = dbContext.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            dbContext.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_measurementViewSource.Source = dbContext.app_measurement.Local;
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
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
                if (dbContext.app_branch.Where(x => x.id_branch == item_transfer.id_branch).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.branch_Code = dbContext.app_branch.Where(x => x.id_branch ==item_transfer. id_branch).FirstOrDefault().code;
                }
                if (dbContext.app_terminal.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.terminal_Code = dbContext.app_terminal.Where(x => x.id_terminal == item_transfer.id_terminal).FirstOrDefault().code;
                }
                if (dbContext.security_user.Where(x => x.id_user == item_transfer.id_user).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.user_Code = dbContext.security_user.Where(x => x.id_user == item_transfer.id_user).FirstOrDefault().code;
                }
                if (dbContext.projects.Where(x => x.id_project == item_transfer.id_project).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.project_Code = dbContext.projects.Where(x => x.id_project == item_transfer.id_project).FirstOrDefault().code;
                }
          
                app_document_range app_document_range = item_transfer.app_document_range;
                item_transfer.number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
            }

            item_transfer.user_requested = dbContext.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
            item_transfer.user_given = dbContext.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
            item_transfer.status = Status.Transfer.Approved;

            dbContext.SaveChanges();

            for (int i = 0; i < item_transfer_detailDataGrid.Items.Count; i++)
            {
                entity.Brillo.Logic.Stock stock = new entity.Brillo.Logic.Stock();

                item_transfer_detail item_transfer_detail = (item_transfer_detail)item_transfer_detailDataGrid.Items[i];

                List<item_movement> Items_InStockLIST;
                app_currencyfx app_currencyfx = ProductMovementDB.app_currencyfx.Where(x => x.app_currency.is_active).FirstOrDefault();
                app_location app_location = item_transfer_detail.item_transfer.app_location_origin;

                if (itemMovement.item_movement != null)
                {
                    Items_InStockLIST = new List<item_movement>();
                    Items_InStockLIST.Add(itemMovement.item_movement);
                }
                else
                {
                    Items_InStockLIST = ProductMovementDB.item_movement.Where(x => x.id_location == app_location.id_location
                                                        && x.id_item_product == item_transfer_detail.id_item_product
                                                        && x.status == entity.Status.Stock.InStock
                                                        && (x.credit - (x._child.Count() > 0 ? x._child.Sum(y => y.debit) : 0)) > 0).ToList();
                }



                ///Debit Movement from Origin.
                List<item_movement> item_movement_originList;
                item_movement_originList = stock.DebitOnly_MovementLIST(ProductMovementDB, Items_InStockLIST, Status.Stock.InStock, entity.App.Names.Movement, item_transfer_detail.id_transfer, item_transfer_detail.id_transfer_detail, app_currencyfx, item_transfer_detail.item_product, app_location,
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
                                entity.App.Names.Movement,
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
                //item_movement_dest._parent = Items_InStockLIST.FirstOrDefault();
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
                itemMovement   = new Configs.itemMovement();
                
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
                                    
                                    if (dbContext.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault() != null)
                                    {
                                        item_transfer_dimension.app_dimension = dbContext.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault();
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
                location_destViewSource.Source = dbContext.app_location.Where(a => a.is_active == true && a.id_branch == item_transfer.id_branch).OrderBy(a => a.name).ToList();
                CollectionViewSource location_originViewSource = ((CollectionViewSource)(FindResource("location_originViewSource")));
                location_originViewSource.Source = dbContext.app_location.Where(a => a.is_active == true && a.id_branch == item_transfer.id_branch).OrderBy(a => a.name).ToList();
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
                    item_transfer_detail.movement_id = (int)itemMovement.item_movement.id_movement;
                    item_transfer_detail.quantity_destination = 1;
                    item_transfer_detail.quantity_origin = 1;
                    item item = ((item)cbxItem.Data);
                    foreach (item_movement_dimension item_movement_dimension in itemMovement.item_movement.item_movement_dimension)
                    {
                        item_transfer_dimension item_transfer_dimension = new item_transfer_dimension();
                        item_transfer_dimension.id_transfer_detail = item_transfer_detail.id_transfer_detail;
                        item_transfer_dimension.id_dimension = item_movement_dimension.id_dimension;
                        if (dbContext.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault() != null)
                        {
                            item_transfer_dimension.app_dimension = dbContext.app_dimension.Where(x => x.id_dimension == item_movement_dimension.id_dimension).FirstOrDefault();

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
    }
}
