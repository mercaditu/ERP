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

namespace Cognitivo.Product
{
    public partial class Transfer : Page
    {
        ProductTransferDB ProductTransferDB = new ProductTransferDB();

        CollectionViewSource item_transferViewSource, transfercostViewSource, item_transferitem_transfer_detailViewSource;
        List<Class.transfercost> clsTotalGrid = null;

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

            ProductTransferDB.Entry(item_transfer).State = EntityState.Added;
       
            item_transferViewSource.View.MoveCurrentToLast();
         }

        private void toolBar_btnSave_Click(object sender)
        {
            try
            {
                item_transfer item_transfer = (item_transfer)item_transferViewSource.View.CurrentItem;

                app_document_range app_document_range = ProductTransferDB.app_document_range.Where(x => x.id_range == item_transfer.id_range).FirstOrDefault();
                item_transfer.number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
                item_transfer.RaisePropertyChanged("number");

                item_transfer.app_branch_origin = (app_branch)id_branch_originComboBox.SelectedItem;
                item_transfer.app_branch_destination = (app_branch)id_branch_destinComboBox.SelectedItem;
                item_transfer.app_location_origin = ((app_branch)id_branch_originComboBox.SelectedItem).app_location.Where(x => x.is_default == true).FirstOrDefault();
                item_transfer.app_location_destination = ((app_branch)id_branch_destinComboBox.SelectedItem).app_location.Where(x => x.is_default == true).FirstOrDefault();

                IEnumerable<DbEntityValidationResult> validationresult = ProductTransferDB.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    for (int i = 0; i < item_transfer_detailDataGrid.Items.Count - 1; i++)
                    {

                        item_transfer_detail item = (item_transfer_detail)item_transfer_detailDataGrid.Items[i];

                        item_movement item_movement_origin = new item_movement();
                        item_movement_origin.debit = 0;
                        item_movement_origin.credit = item.quantity_origin;
                        item_movement_origin.id_application = global::entity.App.Names.SalesInvoice;
                        item_movement_origin.id_location = item.item_transfer.app_location_origin.id_location;
                        item_movement_origin.transaction_id = 0;
                        item_movement_origin.status = Status.Stock.InStock;
                        item_movement_origin.trans_date = item.item_transfer.trans_date;
                        if (item.item_product.id_item_product != 0)
                        {
                            if (ProductTransferDB.item_product.Where(x => x.id_item == item.item_product.id_item_product).FirstOrDefault() != null)
                            {
                                item_movement_origin.id_item_product = ProductTransferDB.item_product.Where(x => x.id_item == item.item_product.id_item_product).FirstOrDefault().id_item_product;
                            }
                        }

                        ProductTransferDB.item_movement.Add(item_movement_origin);
                        item_movement item_movement_dest = new item_movement();
                        item_movement_dest.debit = item.quantity_destination;
                        item_movement_dest.credit = 0;
                        item_movement_dest.id_application = global::entity.App.Names.PurchaseInvoice;
                        item_movement_dest.id_location = item.item_transfer.app_location_destination.id_location;
                        item_movement_dest.transaction_id = 0;
                        item_movement_dest.status = Status.Stock.InStock;
                        item_movement_dest.trans_date = item.item_transfer.trans_date;
                        if (item.item_product.id_item_product != 0)
                        {
                            if (ProductTransferDB.item_product.Where(x => x.id_item == item.item_product.id_item_product).FirstOrDefault() != null)
                            {
                                item_movement_dest.id_item_product = ProductTransferDB.item_product.Where(x => x.id_item == item.item_product.id_item_product).FirstOrDefault().id_item_product;
                            }

                        }
                        clsTotalGrid = (List<Class.transfercost>)transfercostViewSource.Source;
                        foreach (Class.transfercost _clsTotalGrid in clsTotalGrid)
                        {
                            if (item.quantity_origin == 0)
                            {
                                item_movement_value item_movement_detail = new item_movement_value();
                                item_movement_detail.unit_value = _clsTotalGrid.cost / item.quantity_destination;
                                item_movement_detail.id_currencyfx = 0;
                                item_movement_detail.comment = String.Format("Transaction from transfer");
                                item_movement_origin.item_movement_value.Add(item_movement_detail);
                            }

                        }
                        ProductTransferDB.item_movement.Add(item_movement_dest);
                    }

                    ProductTransferDB.SaveChangesAsync();

                   entity.Brillo.Document.Start.Automatic(item_transfer, app_document_range);
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
                a.id_branch == CurrentSession.Id_Branch &&
                a.transfer_type == item_transfer.Transfer_type.transfer)
                .Include(i => i.item_transfer_detail)
                .Load();
            item_transferViewSource.Source = ProductTransferDB.item_transfer.Local;

            item_transferitem_transfer_detailViewSource = ((CollectionViewSource)(this.FindResource("item_transferitem_transfer_detailViewSource")));

            CollectionViewSource branch_originViewSource = ((CollectionViewSource)(this.FindResource("branch_originViewSource")));
            ProductTransferDB.app_branch.Where(a => a.is_active == true).OrderBy(a => a.name).Load();
            branch_originViewSource.Source = ProductTransferDB.app_branch.Local;

            CollectionViewSource branch_destViewSource = ((CollectionViewSource)(this.FindResource("branch_destViewSource")));
            branch_destViewSource.Source = ProductTransferDB.app_branch.Local;

            CollectionViewSource security_userViewSource = ((CollectionViewSource)(this.FindResource("security_userViewSource")));
            ProductTransferDB.security_user.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            security_userViewSource.Source = ProductTransferDB.security_user.Local;

            clsTotalGrid = new List<Class.transfercost>();
            transfercostViewSource = this.FindResource("transfercostViewSource") as CollectionViewSource;
            transfercostViewSource.Source = clsTotalGrid;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.ItemTransfer, CurrentSession.Id_Branch, CurrentSession.Id_terminal);
        }

        private async void SmartBox_Item_Select(object sender, RoutedEventArgs e)
        {
            item_transfer item_transfer = (item_transfer)item_transferViewSource.View.CurrentItem;
            item item = await ProductTransferDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefaultAsync();

            if (item != null && item.item_product != null && item_transfer != null)
            {
                item_transfer_detail item_transfer_detail = new item_transfer_detail();
                item_transfer_detail.quantity_origin = 1;
                item_transfer_detail.item_product = item.item_product.FirstOrDefault();
                item_transfer_detail.RaisePropertyChanged("item_product");
                item_transfer.item_transfer_detail.Add(item_transfer_detail);
            }
            item_transferViewSource.View.Refresh();
            item_transferitem_transfer_detailViewSource.View.Refresh();
        }
    }
}
