using entity;
using System;
using System.Collections.Generic;
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
		public int PageIndex = 0;
		private PurchasePackingListDB PurchasePackingListDB = new PurchasePackingListDB();
        private CollectionViewSource purchase_packingViewSource, purchase_packingpurchase_packinglist_detailViewSource, purchase_packingpurchase_packing_detailApprovedViewSource;
        private cntrl.PanelAdv.pnlPurchaseOrder pnlPurchaseOrder;

        public PackingList()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //entity.Properties.Settings _setting = new entity.Properties.Settings();

            purchase_packingViewSource = FindResource("purchase_packingViewSource") as CollectionViewSource;
            await PurchasePackingListDB.purchase_packing.Where(a => a.id_company == CurrentSession.Id_Company).Include(x => x.contact).OrderByDescending(x => x.trans_date).Take(100).Skip(PageIndex).LoadAsync(); //.Include("purchase_packing_detail").LoadAsync();
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

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PurchasePackingListDB, entity.App.Names.PurchasePacking, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cbxPackingType.ItemsSource = Enum.GetValues(typeof(Status.PackingTypes));
                filterDetail();
                filterVerifiedDetail(0);
                if (purchase_packingpurchase_packinglist_detailViewSource.View != null)
                {
                    purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();
                }
                if (purchase_packingpurchase_packing_detailApprovedViewSource.View != null)
                {
                    purchase_packingpurchase_packing_detailApprovedViewSource.View.Refresh();
                }
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
                            if (purchase_packing_detail.verified_by != null && purchase_packing_detail.id_item == id_item)
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            if (purchase_packing_detail.verified_by != null)
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
                        if (purchase_packing_detail.verified_by == null)
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
            PurchasePackingListDB.purchase_packing.Add(purchase_packing);
            purchase_packingViewSource.View.Refresh();
            purchase_packingViewSource.View.MoveCurrentToLast();

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
                        purchase_packing_detail.app_location = PurchasePackingListDB.app_location.Where(x => x.id_branch == purchase_packing.id_branch && x.is_active && x.is_default).FirstOrDefault();
                        purchase_packing_detail.item = _purchase_order_detail.item;
                        purchase_packing_detail.batch_code = _purchase_order_detail.batch_code;
                        purchase_packing_detail.expire_date = _purchase_order_detail.expire_date;
                        purchase_packing_detail.quantity = _purchase_order_detail.quantity;
                      //  purchase_packing_detail.verified_by = CurrentSession.Id_User;
                        purchase_packing.purchase_packing_detail.Add(purchase_packing_detail);

                        purchase_packingpurchase_packinglist_detailViewSource.View.Refresh();

                        PurchasePackingListDB.Entry(purchase_packing).Entity.State = EntityState.Added;

                        crud_modal.Children.Clear();
                        crud_modal.Visibility = Visibility.Collapsed;
                        filterDetail();
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
                    purchase_packing_detail _purchase_packing_detail = purchase_packingpurchase_packinglist_detailViewSource.View.OfType<purchase_packing_detail>().Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                    if (_purchase_packing_detail != null)
                    {
                        purchase_packing_detail purchase_packing_detail = new purchase_packing_detail();
                        purchase_packing_detail.id_purchase_order_detail = _purchase_packing_detail.id_purchase_order_detail;
                        purchase_packing_detail.id_item = _purchase_packing_detail.id_item;
                        purchase_packing_detail.item = _purchase_packing_detail.item;
                        purchase_packing_detail.verified_quantity = sbxItem.Quantity;
                        purchase_packing_detail.quantity = sbxItem.Quantity;
                        purchase_packing_detail.security_user = PurchasePackingListDB.security_user.Find(CurrentSession.Id_User);
                        purchase_packing_detail.app_location = PurchasePackingListDB.app_location.Where(x => x.id_branch == purchase_packing.id_branch && x.is_active && x.is_default).FirstOrDefault();
                        purchase_packing_detail.verified_by = CurrentSession.Id_User;
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
                        .Where(x => x.verified_by == null)
                        .GroupBy(x => x.id_item)
                        .Select(x => new
                        {
                            ItemName = x.Max(y => y.item.name),
                            ItemCode = x.Max(y => y.item.code),
                            VerifiedQuantity = purchase_packing.purchase_packing_detail.Where(y => y.verified_by != null && y.id_item == x.Max(z => z.id_item)).Sum(y => y.verified_quantity), //Only sum Verified Quantity if IsVerifiyed is True.
                            Quantity = x.Max(y => y.quantity),
                            id_item = x.Max(y => y.id_item)
                        })
                        .ToList();

                    GridVerifiedList.ItemsSource = VerifiedItemList;
                }
            }
        }

        private void purchase_packingDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh_GroupByGrid();
            GridVerifiedList.SelectedIndex = 0;
        }

        private void GridVerifiedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic obj = GridVerifiedList.SelectedItem;
            if (obj != null)
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

        private void btnPurchaseInvoice_Click(object sender, MouseButtonEventArgs e)
        {
            purchase_packing packing = purchase_packingViewSource.View.CurrentItem as purchase_packing;
            if (packing != null)
            {
                if (packing.status == Status.Documents_General.Approved)
                {
                    List<purchase_invoice_detail> DetailList = new List<purchase_invoice_detail>();
                    //For now I only want to bring items not verified. Mainly because I want to prevent duplciating items in Purchase Invoice.
                    //I would like to some how check for inconsistancies or let user check for them before approving.
                    foreach (purchase_packing_detail PackingDetail in packing.purchase_packing_detail.Where(x => x.verified_by == null))
                    {
                        purchase_invoice_detail detail = new purchase_invoice_detail()
                        {
                            item = PackingDetail.item,
                            item_description = PackingDetail.purchase_order_detail.item_description,
                            quantity = PackingDetail.quantity,
                            unit_cost = PackingDetail.purchase_order_detail.unit_cost,
                            discount = PackingDetail.purchase_order_detail.discount,
                            id_vat_group = PackingDetail.purchase_order_detail.id_vat_group,
                            purchase_order_detail = PackingDetail.purchase_order_detail,
                            id_cost_center = PackingDetail.purchase_order_detail.id_cost_center
                        };
                        purchase_packing_detail_relation purchase_packing_detail_relation = new entity.purchase_packing_detail_relation();
                        purchase_packing_detail_relation.id_purchase_invoice_detail = detail.id_purchase_invoice_detail;
                        purchase_packing_detail_relation.purchase_invoice_detail = detail;
                        purchase_packing_detail_relation.id_purchase_packing_detail = PackingDetail.id_purchase_packing_detail;
                        purchase_packing_detail_relation.purchase_packing_detail = PackingDetail;
                        PurchasePackingListDB.purchase_packing_detail_relation.Add(purchase_packing_detail_relation);
                        DetailList.Add(detail);
                    }

                    //Only if Details have been added, should we include the Purchase Invoice into Context.
                    if (DetailList.Count() > 0)
                    {
                        purchase_order Order = PurchasePackingListDB.purchase_order.Find(DetailList.FirstOrDefault().purchase_order_detail.id_purchase_order);
                        if (Order != null)
                        {
                            purchase_invoice _purchase_invoice = new purchase_invoice()
                            {
                                contact = packing.contact,
                                app_branch = packing.app_branch,
                                id_contract = Order.id_contract,
                                id_condition = Order.id_condition,
                                id_currencyfx = Order.id_currencyfx
                            };

                            foreach (var item in DetailList)
                            {
                             
                                _purchase_invoice.purchase_invoice_detail.Add(item);
                            }

                            PurchasePackingListDB.purchase_invoice.Add(_purchase_invoice);
                           
                            try
                            {
                                PurchasePackingListDB.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                }
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
		private void navPagination_btnNextPage_Click(object sender)
		{
			PageIndex = PageIndex + 100;
			Page_Loaded(null, null);
		}

		private void navPagination_btnPreviousPage_Click(object sender)
		{
			PageIndex = PageIndex - 100;
			Page_Loaded(null, null);
		}

		private void navPagination_btnFirstPage_Click(object sender)
		{
			PageIndex = 0;
			Page_Loaded(null, null);
		}
	}
}