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

namespace Cognitivo.Product
{
    public partial class Movement : Page
    {
        ProductTransferDB dbContext = new ProductTransferDB();
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
                item_transfer.status = Status.Documents_General.Pending;
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
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = dbContext.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    dbContext.SaveChanges();

                    MainGrid.IsEnabled = false;
                    toolBar.msgSaved();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
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
            entity.Properties.Settings _entity = new entity.Properties.Settings();

            item_transferViewSource = ((CollectionViewSource)(FindResource("item_transferViewSource")));
            dbContext.item_transfer.Where(a => a.id_company == _entity.company_ID && a.transfer_type == item_transfer.Transfer_type.movemnent).Include("item_transfer_detail").Load();
            item_transferViewSource.Source = dbContext.item_transfer.Local;



            CollectionViewSource itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
            dbContext.items.Where(a => a.id_company == _entity.company_ID && a.item_product.Count() > 0
           ).Load();
            itemViewSource.Source = dbContext.items.Local;

            dbContext.app_document_range.Where(d => d.is_active == true
                                        && d.app_document.id_application == entity.App.Names.Movement
                                        && d.id_company == _entity.company_ID).Include(i => i.app_document).ToList();

            cbxDocument.ItemsSource = dbContext.app_document_range.Local;



            dbContext.app_department.Where(b => b.is_active == true && b.id_company == _entity.company_ID).OrderBy(b => b.name).ToList();

            cbxDepartment.ItemsSource = dbContext.app_department.Local;


            dbContext.projects.Where(b => b.is_active == true && b.id_company == _entity.company_ID).OrderBy(b => b.name).ToList();

            cbxProject.ItemsSource = dbContext.projects.Local;


            dbContext.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == _entity.company_ID).OrderBy(b => b.name).ToList();

            cbxBranch.ItemsSource = dbContext.app_branch.Local;
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
            entity.Brillo.Logic.Stock stock = new entity.Brillo.Logic.Stock();
            item_transfer item_transfer = item_transferViewSource.View.CurrentItem as item_transfer;
            if ((item_transfer.number == null || item_transfer.number == string.Empty) && item_transfer.app_document_range != null)
            {
                entity.Brillo.Logic.Document _Document = new entity.Brillo.Logic.Document();
                entity.Brillo.Logic.Range.branch_Code = String.Empty;
                entity.Brillo.Logic.Range.terminal_Code = String.Empty;
                app_document_range app_document_range = item_transfer.app_document_range;
                item_transfer.number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);

            }
            entity.Properties.Settings setting = new entity.Properties.Settings();
            item_transfer.user_given = dbContext.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
            item_transfer.status = Status.Documents_General.Approved;
            dbContext.SaveChanges();
            ProductMovementDB ProductMovementDB = new ProductMovementDB();
            for (int i = 0; i < item_transfer_detailDataGrid.Items.Count; i++)
            {
                item_transfer_detail item = (item_transfer_detail)item_transfer_detailDataGrid.Items[i];

                item_movement item_movement_origin = new item_movement();
                item_movement_origin.debit =  item.quantity_origin;
                item_movement_origin.credit = 0;
                item_movement_origin.id_application = global::entity.App.Names.Movement;
                item_movement_origin.id_location = item.item_transfer.app_location_origin.id_location;
               // item_movement_origin.transaction_id = 0;
                item_movement_origin.status = Status.Stock.InStock;
                item_movement_origin.trans_date = item.item_transfer.trans_date;
              
                item_movement_origin.comment = stock.comment_Generator(entity.App.Names.Transfer, item.item_transfer.number.ToString(), "");
                if (item.item_product.id_item_product != 0)
                {
                    item_movement_origin.id_item_product = item.item_product.id_item_product;
                }

                ProductMovementDB.item_movement.Add(item_movement_origin);
                item_movement item_movement_dest = new item_movement();
                item_movement_dest.debit = 0;
                item_movement_dest.credit = item.quantity_destination;
                item_movement_dest.id_application = global::entity.App.Names.Movement;
                item_movement_dest.id_location = item.item_transfer.app_location_destination.id_location;
               // item_movement_dest.transaction_id = 0;
                item_movement_dest.status = Status.Stock.InStock;
                item_movement_dest.trans_date = item.item_transfer.trans_date;
               
                item_movement_dest.comment = stock.comment_Generator(entity.App.Names.Transfer, item.item_transfer.number.ToString(), "");
                if (item.item_product.id_item_product != 0)
                {
                    item_movement_dest.id_item_product = item.item_product.id_item_product;
                }

                ProductMovementDB.item_movement.Add(item_movement_dest);
                entity.Brillo.Logic.Document Document = new entity.Brillo.Logic.Document();
                Document.Document_PrintItemRequest(item_transfer.app_document_range.id_document, item_transfer);

                item_transfer.status = Status.Documents_General.Approved;
                ProductMovementDB.SaveChanges();
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
            if (item_transfer != null)
            {
                item_transfer_detail item_transfer_detail = new item_transfer_detail();
                item_transfer_detail.id_item_product = ((item)cbxItem.Data).item_product.FirstOrDefault().id_item_product;
                item_transfer_detail.item_product = ((item)cbxItem.Data).item_product.FirstOrDefault();
                item_transfer_detail.quantity_destination = 1;
                item_transfer_detail.quantity_origin = 1;
                item_transfer.item_transfer_detail.Add(item_transfer_detail);
            }
            CollectionViewSource item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(FindResource("item_transferitem_transfer_detailViewSource")));
            item_transferitem_transfer_detailViewSource.View.Refresh();
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
                location_destViewSource.Source =  dbContext.app_location.Where(a => a.is_active == true && a.id_branch == item_transfer.id_branch ).OrderBy(a => a.name).ToList();
                CollectionViewSource location_originViewSource = ((CollectionViewSource)(FindResource("location_originViewSource")));
                location_originViewSource.Source = dbContext.app_location.Where(a => a.is_active == true && a.id_branch == item_transfer.id_branch).OrderBy(a => a.name).ToList();
            }
        }
    }
}
