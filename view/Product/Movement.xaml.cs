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
        CollectionViewSource item_transferViewSource;

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
                cbxBranch_SelectionChanged(sender, null);
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
            await ProductTransferDB.item_transfer.Where(a => a.id_company == CurrentSession.Id_Company && a.transfer_type == item_transfer.Transfer_type.movemnent).OrderByDescending(x => x.trans_date).LoadAsync();
            item_transferViewSource.Source = ProductTransferDB.item_transfer.Local;
            
            await ProductTransferDB.app_document_range.Where(d => d.is_active == true
                                        && d.app_document.id_application == entity.App.Names.Movement
                                        && d.id_company == CurrentSession.Id_Company).Include(i => i.app_document).ToListAsync();

            cbxDocument.ItemsSource = ProductTransferDB.app_document_range.Local;

            await ProductTransferDB.app_department.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
            cbxDepartment.ItemsSource = ProductTransferDB.app_department.Local;

            //;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            await ProductTransferDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).ToListAsync();
            app_dimensionViewSource.Source = ProductTransferDB.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            await ProductTransferDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToListAsync();
            app_measurementViewSource.Source = ProductTransferDB.app_measurement.Local;

            CollectionViewSource branchViewSource = ((CollectionViewSource)(FindResource("branchViewSource")));
            await ProductTransferDB.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
            branchViewSource.Source = ProductTransferDB.app_branch.Local; //ProductTransferDB.app_branch.Local;

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
                    CollectionViewSource location_originViewSource = ((CollectionViewSource)(FindResource("location_originViewSource")));
                    location_originViewSource.View.MoveCurrentTo(app_location);
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
                        item_movement item_movement = ProductTransferDB.item_movement.Where(x => x.id_movement == item_transfer_detail.movement_id).FirstOrDefault();
                        Items_InStockLIST = stockBrillo.ScalarMovement(item_movement);
                    }
                }
                else
                {
                    entity.Brillo.Stock stockBrillo = new entity.Brillo.Stock();
                    Items_InStockLIST = stockBrillo.List(app_location.id_branch, app_location.id_location, item_transfer_detail.id_item_product);
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
                                    item_movement.item_movement_value.Sum(x=>x.unit_value),
                                    stock.comment_Generator(entity.App.Names.Movement, item_transfer_detail.item_transfer.number != null ? item_transfer_detail.item_transfer.number.ToString() : "", ""),
                                    DimensionList, null, null
                                    );
                    item_movement_dest.parent = item_movement.parent;
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

        private async void cbxItem_KeyDown(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
                item item = await ProductTransferDB.items.FindAsync(sbxItem.ItemID);
                item_product item_product = item.item_product.FirstOrDefault();
                if (item != null)
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
                    else
                    {
                        if (item_transfer != null)
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
