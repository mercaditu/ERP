using entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Purchase
{
    public partial class PackingList : Page
    {
        private PurchasePackingListDB PurchasePackingListDB = new entity.PurchasePackingListDB();
        private CollectionViewSource purchase_packingViewSource, purchase_packingpurchase_packinglist_detailViewSource, purchase_packingpurchase_packing_detailApprovedViewSource, purchase_orderViewSource;
        private cntrl.PanelAdv.pnlPurchaseOrder pnlPurchaseOrder;

        public PackingList()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //entity.Properties.Settings _setting = new entity.Properties.Settings();

            purchase_packingViewSource = FindResource("purchase_packingViewSource") as CollectionViewSource;
            await PurchasePackingListDB.purchase_packing.Where(a => a.id_company == CurrentSession.Id_Company).Include(x => x.contact).LoadAsync(); //.Include("purchase_packing_detail").LoadAsync();
            purchase_packingViewSource.Source = PurchasePackingListDB.purchase_packing.Local;
            purchase_packingpurchase_packinglist_detailViewSource = FindResource("purchase_packingpurchase_packing_detailViewSource") as CollectionViewSource;
            purchase_packingpurchase_packing_detailApprovedViewSource = FindResource("purchase_packingpurchase_packing_detailApprovedViewSource") as CollectionViewSource;

            CollectionViewSource app_branchViewSource = FindResource("app_branchViewSource") as CollectionViewSource;
            app_branchViewSource.Source = await PurchasePackingListDB.app_branch.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();

            CollectionViewSource app_terminalViewSource = FindResource("app_terminalViewSource") as CollectionViewSource;
            app_terminalViewSource.Source = await PurchasePackingListDB.app_terminal.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();

            await PurchasePackingListDB.app_measurement.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).LoadAsync();
            CollectionViewSource app_measurevolume = FindResource("app_measurevolume") as CollectionViewSource;
            CollectionViewSource app_measureweight = FindResource("app_measureweight") as CollectionViewSource;
            app_measurevolume.Source = PurchasePackingListDB.app_measurement.Local;
            app_measureweight.Source = PurchasePackingListDB.app_measurement.Local;

            //purchase_orderViewSource = FindResource("purchase_orderViewSource") as CollectionViewSource;
            //purchase_orderViewSource.Source = dbContext.purchase_order.Where(a => a.id_company == CurrentSession.Id_Company && a.status == Status.Documents_General.Approved).Include(a => a.purchase_order_detail).ToList();

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PurchasePackingListDB, entity.App.Names.PurchasePacking, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cbxPackingType.ItemsSource = Enum.GetValues(typeof(Status.PackingTypes));
                filterDetail();
                filterVerifiedDetail(0);
            }));
            cbxBranch.SelectedIndex = 0;
        }

        private void filterVerifiedDetail(int id_item)
        {
            if (purchase_packingpurchase_packing_detailApprovedViewSource != null)
            {
                if (purchase_packingpurchase_packing_detailApprovedViewSource.View != null)
                {

                    purchase_packingpurchase_packing_detailApprovedViewSource.View.Filter = i =>
                    {
                        purchase_packing_detail purchase_packing_detail = (purchase_packing_detail)i;
                        if (id_item > 0)
                        {
                            if (purchase_packing_detail.user_verified == true && purchase_packing_detail.id_item==id_item) 
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            if (purchase_packing_detail.user_verified == true)
                                return true;
                            else
                                return false;
                        }
                    };

                }
            }
        }
        private void filterDetail()
        {
            if (purchase_packingpurchase_packinglist_detailViewSource != null)
            {
                if (purchase_packingpurchase_packinglist_detailViewSource.View != null)
                {

                    purchase_packingpurchase_packinglist_detailViewSource.View.Filter = i =>
                    {
                        purchase_packing_detail purchase_packing_detail = (purchase_packing_detail)i;
                        if (purchase_packing_detail.user_verified == false)
                            return true;
                        else
                            return false;
                    };

                }
            }
        }

        #region Toolbar Events

        private void toolBar_btnNew_Click(object sender)
        {
            InvoiceSetting PurchaseSettings = new InvoiceSetting();
            purchase_packing purchase_packing = PurchasePackingListDB.New();
            purchase_packing.trans_date = DateTime.Now.AddDays(PurchaseSettings.TransDate_OffSet);
            purchase_packing.State = EntityState.Added;
            PurchasePackingListDB.purchase_packing.Add(purchase_packing);
            purchase_packingViewSource.View.Refresh();
            //purchase_packingViewSource.View.MoveCurrentToLast();

            //Refresh_GroupByGrid();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_packingDataGrid.SelectedItem != null)
            {
                purchase_packing purchase_packing = (purchase_packing)purchase_packingDataGrid.SelectedItem;
                purchase_packing.IsSelected = true;
                purchase_packing.State = EntityState.Modified;
                PurchasePackingListDB.Entry(purchase_packing).State = EntityState.Modified;
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
                PurchasePackingListDB.purchase_packing.Remove((purchase_packing)purchase_packingDataGrid.SelectedItem);
                purchase_packingViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (PurchasePackingListDB.SaveChanges() > 0)
            {
                purchase_packingViewSource.View.Refresh();
                toolBar.msgSaved(PurchasePackingListDB.NumberOfRecords);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            purchase_packinglist_detailDataGrid.CancelEdit();
            purchase_packingViewSource.View.MoveCurrentToFirst();
            PurchasePackingListDB.CancelAllChanges();
            if (purchase_packingpurchase_packinglist_detailViewSource.View != null)
                purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();
        }

        #endregion Toolbar Events

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Purchase.InvoiceSetting.Default.Save();
            popupCustomize.IsOpen = false;
        }

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

        private void btnApprove_Click(object sender)
        {
            PurchasePackingListDB.Approve();
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as purchase_packing_detail != null)
            {
                e.CanExecute = true;
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
                    PurchasePackingListDB.purchase_packing_detail.Remove(e.Parameter as purchase_packing_detail);
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
            PurchasePackingListDB.Annull();
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
            pnlPurchaseOrder._entity = PurchasePackingListDB;
            if (sbxContact.ContactID > 0)
            {
                contact contact = PurchasePackingListDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlPurchaseOrder._contact = contact;
            }
            pnlPurchaseOrder.mode = cntrl.PanelAdv.pnlPurchaseOrder.module.packing_list;
            pnlPurchaseOrder.PurchaseOrder_Click += PurchaseOrder_Click;
            crud_modal.Children.Add(pnlPurchaseOrder);
        }

        public void PurchaseOrder_Click(object sender)
        {
            purchase_packing purchase_packing = (purchase_packing)purchase_packingViewSource.View.CurrentItem;
            if (purchase_packing != null)
            {
                foreach (purchase_order item in pnlPurchaseOrder.selected_purchase_order)
                {
                    foreach (purchase_order_detail _purchase_order_detail in item.purchase_order_detail)
                    {
                        purchase_packing_detail purchase_packing_detail = new purchase_packing_detail();
                        purchase_packing_detail.id_purchase_order_detail = _purchase_order_detail.id_purchase_order_detail;
                        purchase_packing_detail.id_item = (int)_purchase_order_detail.id_item;
                        purchase_packing_detail.item = _purchase_order_detail.item;
                        purchase_packing_detail.batch_code = _purchase_order_detail.batch_code;
                        purchase_packing_detail.expire_date = _purchase_order_detail.expire_date;
                        purchase_packing_detail.quantity = _purchase_order_detail.quantity;
                        purchase_packing_detail.user_verified = false;
                        purchase_packing.purchase_packing_detail.Add(purchase_packing_detail);

                        purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();

                        PurchasePackingListDB.Entry(purchase_packing).Entity.State = EntityState.Added;

                        crud_modal.Children.Clear();
                        crud_modal.Visibility = Visibility.Collapsed;
                        filterDetail();
                       // filterVerifiedDetail();
                    }
                    Refresh_GroupByGrid();
                    GridVerifiedList.SelectedIndex = 0;
                }
            }
        }

        private void SmartBox_Item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                purchase_packing purchase_packing = purchase_packingViewSource.View.CurrentItem as purchase_packing;
                if (purchase_packing != null)
                {
                    purchase_packing_detail _purchase_packing_detail = purchase_packingpurchase_packinglist_detailViewSource.View.OfType<purchase_packing_detail>().Where(x => x.id_item == sbxItem.ItemID ).FirstOrDefault();
                    if (_purchase_packing_detail != null)
                    {
                      
                            purchase_packing_detail purchase_packing_detail = new purchase_packing_detail();
                            purchase_packing_detail.id_purchase_order_detail = _purchase_packing_detail.id_purchase_order_detail;
                            purchase_packing_detail.id_item = (int)_purchase_packing_detail.id_item;
                            purchase_packing_detail.item = _purchase_packing_detail.item;
                            purchase_packing_detail.verified_quantity = sbxItem.Quantity;
                            purchase_packing_detail.user_verified = true;
                            purchase_packing.purchase_packing_detail.Add(purchase_packing_detail);

                            purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();
                            purchase_packingpurchase_packing_detailApprovedViewSource.View.Refresh();
                            filterDetail();
                            Refresh_GroupByGrid();
                            //filterVerifiedDetail();
                     

                        //Filter UserVerified True.
                    }
                   
                }
            }
        }

        private void Refresh_GroupByGrid()
        {
            purchase_packing purchase_packing = purchase_packingViewSource.View.CurrentItem as purchase_packing;
            if (purchase_packing != null)
            {
                if (purchase_packing.purchase_packing_detail.Count() > 0)
                {
                    //This code should be in Selection Changed of DataGrid and after inserting new items.
                    var VerifiedItemList = purchase_packing.purchase_packing_detail
                        .Where(x => x.user_verified == false)
                        .GroupBy(x => x.id_item)
                        .Select(x => new
                        {
                            ItemName = x.Max(y => y.item.name),
                            ItemCode = x.Max(y => y.item.code),
                            VerifiedQuantity = x.Where(y => y.user_verified).Sum(y => y.verified_quantity), //Only sum Verified Quantity if IsVerifiyed is True.
                            Quantity = x.Max(y => y.quantity),
                            id_item= x.Max(y => y.item.id_item)
                        })
                        .ToList();

                    GridVerifiedList.ItemsSource = VerifiedItemList;
                   
                }
            }
        }

        private void purchase_packingDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh_GroupByGrid();
        }

        private void GridVerifiedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             dynamic obj = GridVerifiedList.SelectedItem;
            if (obj!=null)
            {
               filterVerifiedDetail(obj.id_item);
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
                    contact contact = PurchasePackingListDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
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

        #endregion Filter Data

        private void cbxBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxBranch.SelectedItem != null)
            {
                app_branch app_branch = cbxBranch.SelectedItem as app_branch;
                if (app_branch != null)
                {
                    cbxLocation.ItemsSource = app_branch.app_location.ToList();
                }
            }
        }
    }
}