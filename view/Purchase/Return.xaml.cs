using entity;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Purchase
{
    public partial class Return : Page
    {
		public int PageIndex = 0;
		private entity.Controller.Purchase.ReturnController PurchaseReturnDB;
        private CollectionViewSource
            purchaseReturnViewSource,
            purchase_returnpurchase_return_detailViewSource;

        private cntrl.PanelAdv.pnlPurchaseInvoice pnlPurchaseInvoice;
        private cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry;

        public Return()
        {
            InitializeComponent();
            PurchaseReturnDB = FindResource("PurchaseReturn") as entity.Controller.Purchase.ReturnController;

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                PurchaseReturnDB.Initialize();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PurchaseReturnDB.Load(PageIndex);
            
            //PurchaseReturn
            purchaseReturnViewSource = FindResource("purchase_returnViewSource") as CollectionViewSource;
           
            purchaseReturnViewSource.Source = PurchaseReturnDB.db.purchase_return.Local;

			purchase_returnpurchase_return_detailViewSource = FindResource("purchase_returnpurchase_return_detailViewSource") as CollectionViewSource;

            CollectionViewSource app_cost_centerViewSource = FindResource("app_cost_centerViewSource") as CollectionViewSource;
            app_cost_centerViewSource.Source = PurchaseReturnDB.db.app_cost_center.Local;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.VAT_Groups;

            cbxReturnType.ItemsSource = Enum.GetValues(typeof(Status.ReturnTypes));
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                PurchaseReturnDB.db.purchase_return.Remove((purchase_return)purchase_returnDataGrid.SelectedItem);
                purchaseReturnViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            purchase_return purchase_return = PurchaseReturnDB.Create();
           
            purchaseReturnViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (PurchaseReturnDB.SaveChanges_WithValidation())
            {
                toolBar.msgSaved(PurchaseReturnDB.NumberOfRecords);
                sbxContact.Text = "";
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_returnDataGrid.SelectedItem != null)
            {
                purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;

                PurchaseReturnDB.Edit(purchase_return);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            PurchaseReturnDB.CancelAllChanges();
        }

        #region Datagrid Events

        private void calculate_vat(object sender, EventArgs e)
        {
            purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
            purchase_return.RaisePropertyChanged("GrandTotal");
        }

        private void purchase_return_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void purchase_return_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            purchase_return_detail purchase_return_detail = (purchase_return_detail)e.NewItem;
            purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
        }

        #endregion Datagrid Events

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                purchaseReturnViewSource.View.Filter = i =>
                {
                    purchase_return purchase_return = i as purchase_return;
                    string name = purchase_return.contact != null ? purchase_return.contact.name : "";
                    string number = string.IsNullOrEmpty(purchase_return.number) ? purchase_return.number : "";
                    string total = purchase_return.GrandTotal.ToString();

                    if (name.ToLower().Contains(query.ToLower()) || number.ToLower().Contains(query.ToLower()) || total.ToLower().Contains(query.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
            else
            {
                purchaseReturnViewSource.View.Filter = null;
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as purchase_return_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    purchase_return purchase_return = purchaseReturnViewSource.View.CurrentItem as purchase_return;
                    //DeleteDetailGridRow
                    purchase_return_detailDataGrid.CancelEdit();
                    PurchaseReturnDB.db.purchase_return_detail.Remove(e.Parameter as purchase_return_detail);
                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = PurchaseReturnDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                if (contact != null)
                {
                    purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
                    purchase_return.id_contact = contact.id_contact;
                    purchase_return.contact = contact;

                    if (purchase_return != null)
                    {
                        ///Start Thread to get Data.
                        Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
                    }
                }
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    //cbxContactRelation.ItemsSource = SalesInvoiceDB.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

                    //if (objContact.id_sales_rep != null)
                    //    cbxs.SelectedValue = Convert.ToInt32(objContact.id_sales_rep);
                    ////Condition
                    //if (objContact.app_contract != null)
                    //    cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    ////Contract
                    //if (objContact.id_contract != null)
                    //    cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    ////Currency

                    cbxCurrency.get_ActiveRateXContact(ref objContact);
                }));
            }
        }

        private async void select_Item(purchase_return purchase_return, item item, int id_contact, item_movement item_movement)
        {
            long id_movement = item_movement != null ? item_movement.id_movement : 0;
            purchase_return_detail purchase_return_detail = purchase_return.purchase_return_detail
                .Where(a => a.id_item == sbxItem.ItemID && a.movement_id == id_movement).FirstOrDefault();

            if (purchase_return_detail == null)
            {
                int id_cost_center = 0;

                purchase_return_detail _purchase_return_detail = new purchase_return_detail();
                //Check for contact
                if (id_contact > 0)
                {
                    contact contact = await PurchaseReturnDB.db.contacts.FindAsync(id_contact);
                    if (contact.app_cost_center != null)
                    {
                        app_cost_center app_cost_center = contact.app_cost_center as app_cost_center;
                        if (app_cost_center.is_product == true)
                        {
                            id_cost_center = app_cost_center.id_cost_center;
                            if (id_cost_center > 0)
                                _purchase_return_detail.id_cost_center = id_cost_center;
                        }
                        else
                        {
                            app_cost_center _app_cost_center = PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault();
                            if (_app_cost_center != null)
                                id_cost_center = Convert.ToInt32(_app_cost_center.id_cost_center);
                            if (id_cost_center > 0)
                                _purchase_return_detail.id_cost_center = id_cost_center;
                        }
                    }
                    else
                    {
                        if (PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                            id_cost_center = Convert.ToInt32(PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault().id_cost_center);
                        if (id_cost_center > 0)
                            _purchase_return_detail.id_cost_center = id_cost_center;
                    }
                }
                else
                {
                    if (PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                        id_cost_center = Convert.ToInt32(PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault().id_cost_center);
                    if (id_cost_center > 0)
                        _purchase_return_detail.id_cost_center = id_cost_center;
                }
                if (PurchaseReturnDB.db.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                {
                    _purchase_return_detail.id_vat_group = PurchaseReturnDB.db.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
                }

                if (item_movement != null)
                {
                    _purchase_return_detail.batch_code = item_movement.code;
                    _purchase_return_detail.expire_date = item_movement.expire_date;
                    _purchase_return_detail.movement_id = (int)item_movement.id_movement;
					
                }

                _purchase_return_detail.purchase_return = purchase_return;
                _purchase_return_detail.item = item;
                _purchase_return_detail.id_item = sbxItem.ItemID;
                _purchase_return_detail.item_description = item.supplier_name;
                _purchase_return_detail.quantity = sbxItem.Quantity;
                purchase_return.purchase_return_detail.Add(_purchase_return_detail);
            }
            else
            {
                purchase_return_detail.quantity += sbxItem.Quantity;
            }

            await Dispatcher.BeginInvoke((Action)(() =>
         {
             purchase_returnpurchase_return_detailViewSource.View.Refresh();
             calculate_vat(null, null);
         }));
        }

        private async void item_Select(object sender, EventArgs e)
        {
            try
            {
                if (sbxItem.ItemID > 0)
                {
                    item item = await PurchaseReturnDB.db.items.FindAsync(sbxItem.ItemID);
                    item_product item_product = item.item_product.FirstOrDefault();
                    purchase_return purchase_return = purchase_returnDataGrid.SelectedItem as purchase_return;
                    if (item_product != null && item_product.can_expire)
                    {
                        crud_modalExpire.Visibility = Visibility.Visible;
                        pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(purchase_return.id_branch, null, item.item_product.FirstOrDefault().id_item_product); ;
                        crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                    }
                    else
                    {
                        Task Thread = Task.Factory.StartNew(() => select_Item(purchase_return, item, sbxContact.ContactID, null));
                    }
                }
                else
                {
                    //Other
                    purchase_return purchase_return = purchase_returnDataGrid.SelectedItem as purchase_return;
                    purchase_return_detail purchase_return_detail = purchase_return.purchase_return_detail.Where(a => a.item_description == sbxItem.Text).FirstOrDefault();
                    if (purchase_return_detail == null)
                    {
                        purchase_return_detail _purchase_return_detail = new entity.purchase_return_detail();
                        int id_cost_center = 0;
                        //Check for contact
                        if (sbxContact.ContactID > 0)
                        {
                            contact contact = PurchaseReturnDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                            if (contact.app_cost_center != null)
                            {
                                app_cost_center app_cost_center = contact.app_cost_center as app_cost_center;
                                if (app_cost_center.is_product == false)
                                {
                                    id_cost_center = app_cost_center.id_cost_center;
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                                else
                                {
                                    if (PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                                        id_cost_center = Convert.ToInt32(PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault().id_cost_center);
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                            }
                            else
                            {
                                if (PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                                    id_cost_center = Convert.ToInt32(PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault().id_cost_center);
                                if (id_cost_center > 0)
                                    _purchase_return_detail.id_cost_center = id_cost_center;
                            }
                        }
                        else
                        {
                            if (PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                                id_cost_center = Convert.ToInt32(PurchaseReturnDB.db.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault().id_cost_center);
                            if (id_cost_center > 0)
                                _purchase_return_detail.id_cost_center = id_cost_center;
                        }
                        if (PurchaseReturnDB.db.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
                        {
                            _purchase_return_detail.id_vat_group = PurchaseReturnDB.db.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
                        }
                        _purchase_return_detail.purchase_return = purchase_return;
                        _purchase_return_detail.item_description = sbxItem.Text;
                        _purchase_return_detail.id_item = 0;
                        purchase_return.purchase_return_detail.Add(_purchase_return_detail);
                    }
                    else
                    {
                        purchase_return_detail.quantity += 1;
                    }
                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                    calculate_vat(sender, e);
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            if (PurchaseReturnDB.Approve())
            {
                toolBar.msgApproved(PurchaseReturnDB.NumberOfRecords);
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            PurchaseReturnDB.Annull();
            foreach (purchase_return purchase_return in purchaseReturnViewSource.View.Cast<purchase_return>().ToList())
            {
                purchase_return.IsSelected = false;
            }
        }

        private void purchase_return_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void btnPurchaseInvoice_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlPurchaseInvoice = new cntrl.PanelAdv.pnlPurchaseInvoice();
            pnlPurchaseInvoice._entity = new ImpexDB();

            if (sbxContact.ContactID > 0)
            {
                contact contact = PurchaseReturnDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlPurchaseInvoice._contact = contact;
            }

            pnlPurchaseInvoice.PurchaseInvoice_Click += PurchaseInvoice_Click;
            crud_modal.Children.Add(pnlPurchaseInvoice);
        }

        public void PurchaseInvoice_Click(object sender)
        {
            purchase_return _purchase_return = (purchase_return)purchaseReturnViewSource.View.CurrentItem;

            sbxContact.Text = pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault().contact.name;
            foreach (purchase_invoice item in pnlPurchaseInvoice.selected_purchase_invoice)
            {
                _purchase_return.State = EntityState.Modified;
                _purchase_return.id_condition = item.id_condition;
                _purchase_return.id_contract = item.id_contract;
                _purchase_return.id_currencyfx = item.id_currencyfx;
                _purchase_return.id_purchase_invoice = item.id_purchase_invoice;

                foreach (purchase_invoice_detail _purchase_invoice_detail in item.purchase_invoice_detail)
                {
                    purchase_return_detail purchase_return_detail = new purchase_return_detail();
                    purchase_return_detail.id_purchase_invoice_detail = _purchase_invoice_detail.id_purchase_invoice_detail;
                    purchase_return_detail.purchase_invoice_detail = PurchaseReturnDB.db.purchase_invoice_detail.Where(x => x.id_purchase_invoice_detail == _purchase_invoice_detail.id_purchase_invoice_detail).FirstOrDefault();
                    purchase_return_detail.id_cost_center = _purchase_invoice_detail.id_cost_center;
                    purchase_return_detail.id_location = _purchase_invoice_detail.id_location;

                    app_location app_location = PurchaseReturnDB.db.app_location.Where(x => x.id_location == _purchase_invoice_detail.id_location).FirstOrDefault();
                    if (app_location != null)
                    {
                        purchase_return_detail.app_location = app_location;
                    }

                    purchase_return_detail.purchase_return = _purchase_return;
                    if (PurchaseReturnDB.db.items.Where(x => x.id_item == _purchase_invoice_detail.id_item).FirstOrDefault() != null)
                    {
                        purchase_return_detail.id_item = _purchase_invoice_detail.id_item;
                        purchase_return_detail.item = PurchaseReturnDB.db.items.Where(x => x.id_item == _purchase_invoice_detail.id_item).FirstOrDefault();
                    }

                    purchase_return_detail.item_description = _purchase_invoice_detail.item_description;

                    purchase_return_detail.quantity = _purchase_invoice_detail.quantity - PurchaseReturnDB.db.purchase_return_detail
                                                                                 .Where(x => x.id_purchase_invoice_detail == _purchase_invoice_detail.id_purchase_invoice_detail)
                                                                                 .GroupBy(x => x.id_purchase_invoice_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault();

                    purchase_return_detail.id_vat_group = _purchase_invoice_detail.id_vat_group;
                    purchase_return_detail.unit_cost = _purchase_invoice_detail.unit_cost;
                    purchase_return_detail.CurrencyFX_ID = _purchase_return.id_currencyfx;
                    purchase_return_detail.batch_code = _purchase_invoice_detail.batch_code;
                    purchase_return_detail.expire_date = _purchase_invoice_detail.expire_date;
                    _purchase_return.purchase_return_detail.Add(purchase_return_detail);

                    PurchaseReturnDB.db.Entry(_purchase_return).Entity.State = EntityState.Added;
                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                    purchaseReturnViewSource.View.Refresh();

                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                }
            }
        }

        private async void crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                purchase_return _purchase_return = (purchase_return)purchaseReturnViewSource.View.CurrentItem;
                item item = await PurchaseReturnDB.db.items.FindAsync(sbxItem.ItemID);

                if (item != null && item.id_item > 0 && _purchase_return != null)
                {
                    item_movement item_movement = PurchaseReturnDB.db.item_movement.Find(pnl_ItemMovementExpiry.MovementID);

                    Task Thread = Task.Factory.StartNew(() => select_Item(_purchase_return, item, sbxContact.ContactID, item_movement));
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