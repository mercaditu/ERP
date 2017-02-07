using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Windows.Documents;
using System.Threading.Tasks;

namespace Cognitivo.Purchase
{
    public partial class PackingList : Page
    {
        entity.PurchasePackingListDB dbContext = new entity.PurchasePackingListDB();
        Purchase.InvoiceSetting PurchaseSettings = new InvoiceSetting();
        CollectionViewSource purchase_packingViewSource, purchase_packingpurchase_packinglist_detailViewSource, purchase_orderViewSource;
        cntrl.PanelAdv.pnlPurchaseOrder pnlPurchaseOrder;

        public PackingList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //entity.Properties.Settings _setting = new entity.Properties.Settings();

            purchase_packingViewSource = FindResource("purchase_packingViewSource") as CollectionViewSource;
            dbContext.purchase_packing.Where(a => a.id_company == CurrentSession.Id_Company).Include("purchase_packing_detail").Load();
            purchase_packingViewSource.Source = dbContext.purchase_packing.Local;
            purchase_packingpurchase_packinglist_detailViewSource = FindResource("purchase_packingpurchase_packing_detailViewSource") as CollectionViewSource;

            CollectionViewSource app_branchViewSource = FindResource("app_branchViewSource") as CollectionViewSource;
            app_branchViewSource.Source = dbContext.app_branch.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

            CollectionViewSource app_terminalViewSource = FindResource("app_terminalViewSource") as CollectionViewSource;
            app_terminalViewSource.Source = dbContext.app_terminal.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

            dbContext.app_measurement.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            CollectionViewSource app_measurevolume = FindResource("app_measurevolume") as CollectionViewSource;
            CollectionViewSource app_measureweight = FindResource("app_measureweight") as CollectionViewSource;
            app_measurevolume.Source = dbContext.app_measurement.Local;
            app_measureweight.Source = dbContext.app_measurement.Local;

            purchase_orderViewSource = FindResource("purchase_orderViewSource") as CollectionViewSource;
            purchase_orderViewSource.Source = dbContext.purchase_order.Where(a => a.id_company == CurrentSession.Id_Company && a.status == Status.Documents_General.Approved).Include(a => a.purchase_order_detail).ToList();

            Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(dbContext, entity.App.Names.PurchasePacking, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cbxPackingType.ItemsSource = Enum.GetValues(typeof(Status.PackingTypes));
            }));
            cbxBranch.SelectedIndex = 0;
        }

        #region Toolbar Events
        private void toolBar_btnNew_Click(object sender)
        {


            purchase_packing purchase_packing = dbContext.New();
            purchase_packing.trans_date = DateTime.Now.AddDays(PurchaseSettings.TransDate_OffSet);
            purchase_packing.State = System.Data.Entity.EntityState.Added;
            dbContext.purchase_packing.Add(purchase_packing);
            purchase_packingViewSource.View.Refresh();
            purchase_packingViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_packingDataGrid.SelectedItem != null)
            {
                purchase_packing purchase_packing = (purchase_packing)purchase_packingDataGrid.SelectedItem;
                purchase_packing.IsSelected = true;
                purchase_packing.State = System.Data.Entity.EntityState.Modified;
                dbContext.Entry(purchase_packing).State = EntityState.Modified;
                purchase_packingViewSource.View.Refresh();
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }

        }
        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                dbContext.purchase_packing.Remove((purchase_packing)purchase_packingDataGrid.SelectedItem);
                purchase_packingViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }
        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                purchase_packingViewSource.View.Refresh();
                toolBar.msgSaved(dbContext.NumberOfRecords);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            purchase_packinglist_detailDataGrid.CancelEdit();
            purchase_packingViewSource.View.MoveCurrentToFirst();
            dbContext.CancelAllChanges();
            if (purchase_packingpurchase_packinglist_detailViewSource.View != null)
                purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();
        }
        #endregion

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Purchase.InvoiceSetting.Default.Save();
            popupCustomize.IsOpen = false;
        }

        //private void item_Select(object sender, EventArgs e)
        //{
        //    app_branch app_branch=null;
        //    if (sbxItem.ItemID > 0)
        //    {
        //        purchase_packing purchase_packing = purchase_packingViewSource.View.CurrentItem as purchase_packing;
        //        item item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
              
               
        //        if (item != null && item.id_item > 0 && purchase_packing != null)
        //        {
                 
        //            if (cbxBranch.SelectedItem != null)
        //            { app_branch = cbxBranch.SelectedItem as app_branch; }
        //            Task Thread = Task.Factory.StartNew(() => select_Item(purchase_packing, item, app_branch));
        //        }
        //    }
        //}

        //private void select_Item(purchase_packing purchase_packing, item item, app_branch app_branch)
        //{
        //    if (purchase_packing.purchase_packing_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null)
        //    {
        //        purchase_packing_detail _purchase_packing_detail = new purchase_packing_detail();
        //        _purchase_packing_detail.purchase_packing = purchase_packing;
        //        _purchase_packing_detail.item = item;
        //        _purchase_packing_detail.quantity = 1;
        //        _purchase_packing_detail.id_item = item.id_item;
        //        if (app_branch != null)
        //        {
                   
        //            _purchase_packing_detail.id_location = app_branch.app_location.Where(x => x.is_default).FirstOrDefault().id_location;
        //            _purchase_packing_detail.app_location = app_branch.app_location.Where(x => x.is_default).FirstOrDefault();

        //        }


        //        purchase_packing.purchase_packing_detail.Add(_purchase_packing_detail);
        //    }
        //    else
        //    {
        //        purchase_packing_detail purchase_packing_detail = purchase_packing.purchase_packing_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
        //        purchase_packing_detail.quantity += 1;
        //    }

        //    Dispatcher.BeginInvoke((Action)(() =>
        //    {

        //        purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();

        //    }));
        //}

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.contact contact = new cntrl.Curd.contact();
            crud_modal.Children.Add(contact);
        }

        private void purchase_packingDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnApprove_Click(object sender)
        {
            Cognitivo.Purchase.InvoiceSetting PurchaseSettings = new Cognitivo.Purchase.InvoiceSetting();
            dbContext.Approve(PurchaseSettings.DiscountStock_Packing);
        }

        private void cbxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as purchase_packing_detail != null)
            {
                //purchase_packing_detail purchase_packing_detail = e.Parameter as purchase_packing_detail;
                //if (string.IsNullOrEmpty(purchase_packing_detail.Error))
                //{
                e.CanExecute = true;
                //}
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //DeleteDetailGridRow
                    purchase_packinglist_detailDataGrid.CancelEdit();
                    dbContext.purchase_packing_detail.Remove(e.Parameter as purchase_packing_detail);
                    purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
                //throw;
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            dbContext.Annull();
        }



        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id_contact = sbxContact.ContactID;
            if (purchase_orderViewSource != null)
            {
                if (purchase_orderViewSource.View != null)
                {
                    if (purchase_orderViewSource.View.Cast<purchase_order>().Count() > 0)
                    {
                        purchase_orderViewSource.View.Filter = i =>
                        {
                            purchase_order purchase_order = (purchase_order)i;
                            if (purchase_order.id_contact == id_contact)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }

        }


        private void btnpurchaseOrder_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlPurchaseOrder = new cntrl.PanelAdv.pnlPurchaseOrder();
            pnlPurchaseOrder._entity = dbContext;
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlPurchaseOrder._contact = contact;
            }
            pnlPurchaseOrder.mode = cntrl.PanelAdv.pnlPurchaseOrder.module.packing_list;
            pnlPurchaseOrder.PurchaseOrder_Click += PurchaseOrder_Click;
            crud_modal.Children.Add(pnlPurchaseOrder);
        }

        public void PurchaseOrder_Click(object sender)
        {
            purchase_packing purchase_packing = (purchase_packing)purchase_packingViewSource.View.CurrentItem;


            foreach (purchase_order item in pnlPurchaseOrder.selected_purchase_order)
            {

                foreach (purchase_order_detail _purchase_order_detail in item.purchase_order_detail)
                {
                    if (purchase_packing.purchase_packing_detail.Where(x => x.id_item == _purchase_order_detail.id_item).Count() == 0)
                    {
                        purchase_packing_detail purchase_packing_detail = new purchase_packing_detail();
                        purchase_packing_detail.id_purchase_order_detail = _purchase_order_detail.id_purchase_order_detail;
                        purchase_packing_detail.id_item =(int)_purchase_order_detail.id_item;
                        purchase_packing_detail.item = _purchase_order_detail.item;
                        purchase_packing_detail.batch_code = _purchase_order_detail.batch_code;
                        purchase_packing_detail.expire_date = _purchase_order_detail.expire_date;
                        purchase_packing_detail.quantity = _purchase_order_detail.quantity;
                        purchase_packing.purchase_packing_detail.Add(purchase_packing_detail);
                    }

                    purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();
                    dbContext.Entry(purchase_packing).Entity.State = EntityState.Added;
                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                }
            }
        }
        #region Filter Data
        private void set_ContactPrefKeyStroke(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                set_ContactPref(sender, e);
            }
        }



        private void set_ContactPref(object sender, EventArgs e)
        {
            try
            {
                if (sbxContact.ContactID > 0)
                {
                    contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    purchase_packing purchase_packing = (purchase_packing)purchase_packingDataGrid.SelectedItem;
                    purchase_packing.id_contact = contact.id_contact;
                    purchase_packing.contact = contact;



                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }





        #endregion

      

        private void cbxBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxBranch.SelectedItem != null)
            {
                app_branch app_branch = cbxBranch.SelectedItem as app_branch;
                cbxLocation.ItemsSource = app_branch.app_location.ToList();


            }
        }

       


    }
}
