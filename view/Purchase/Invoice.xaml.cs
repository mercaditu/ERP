using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace Cognitivo.Purchase
{
    public partial class Invoice : Page, IDisposable
    {
        private CollectionViewSource purchase_invoiceViewSource;
        private CollectionViewSource purchase_invoicepurchase_invoice_detailViewSource;

        private entity.Controller.Purchase.InvoiceController PurchaseDB;
        private cntrl.PanelAdv.pnlPurchaseOrder pnlPurchaseOrder;

        public Invoice()
        {
            InitializeComponent();
            PurchaseDB = FindResource("PurchaseInvoice") as entity.Controller.Purchase.InvoiceController;

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                PurchaseDB.Initialize();
            }
        }

        private void load_PrimaryData()
        {
            Load_PrimaryDataThread();
            Load_SecondaryDataThread();
        }

        private void Load_PrimaryDataThread()
        {
            PurchaseDB.Load();

            purchase_invoicepurchase_invoice_detailViewSource = FindResource("purchase_invoicepurchase_invoice_detailViewSource") as CollectionViewSource;
            purchase_invoiceViewSource = FindResource("purchase_invoiceViewSource") as CollectionViewSource;

            purchase_invoiceViewSource.Source = PurchaseDB.db.purchase_invoice.Local;
        }

        private void Load_SecondaryDataThread()
        {
            cbxDepartment.ItemsSource = PurchaseDB.db.app_department.Local;
            CollectionViewSource app_dimensionViewSource = FindResource("app_dimensionViewSource") as CollectionViewSource;
            app_dimensionViewSource.Source = PurchaseDB.db.app_dimension.Local;
            CollectionViewSource app_measurementViewSource = FindResource("app_measurementViewSource") as CollectionViewSource;
            app_measurementViewSource.Source = PurchaseDB.db.app_measurement.Local;
            CollectionViewSource app_cost_centerViewSource = FindResource("app_cost_centerViewSource") as CollectionViewSource;
            app_cost_centerViewSource.Source = PurchaseDB.db.app_cost_center.Local;
        }

        private void pageInvoice_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                load_PrimaryData();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #region Toolbar events

        private void New_Click(object sender)
        {
            sbxContact.Text = "";
            sbxItem.Text = "";

            purchase_invoice purchase_invoice = PurchaseDB.Create(new InvoiceSetting().TransDate_OffSet);
            purchase_invoiceViewSource.View.MoveCurrentTo(purchase_invoice);
        }

        private void Edit_Click(object sender)
        {
            if (purchase_invoiceDataGrid.SelectedItem != null)
            {
                PurchaseDB.Edit(purchase_invoiceDataGrid.SelectedItem as purchase_invoice);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Delete_Click(object sender)
        {
            if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                PurchaseDB.Archived();
            }
        }

        private void Save_Click(object sender)
        {
            if (PurchaseDB.SaveChanges_WithValidation())
            {
                purchase_invoiceViewSource.View.Refresh();
                toolBar.msgSaved(PurchaseDB.NumberOfRecords);
            }
        }

        private void Cancel_Click(object sender)
        {
            PurchaseDB.CancelAllChanges();
        }

        private void Approve_Click(object sender)
        {
            PurchaseDB.Approve();
        }

        private void Anull_Click(object sender)
        {
            if (MessageBox.Show("Anull?", "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                PurchaseDB.Annull();
            }
        }

        #endregion Toolbar events

        #region Filter Data

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = PurchaseDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;

                if (contact != null && purchase_invoice != null)
                {
                    purchase_invoice.id_contact = contact.id_contact;
                    purchase_invoice.contact = contact;

                    if (contact.trans_code_exp > DateTime.Today)
                    {
                        purchase_invoice.code = contact.trans_code;
                        purchase_invoice.RaisePropertyChanged("code");
                    }

                    ///Start Thread to get Data.
                    Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
                }
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    //Currency
                    cbxCurrency.get_ActiveRateXContact(ref objContact);

                    if (cbxCondition.SelectedItem != null && cbxContract.SelectedItem != null && cbxCurrency.SelectedValue > 0)
                    {
                        sbxItem.SmartBoxItem_Focus();
                    }
                }));
            }
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
            //Contract
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;

                if (app_condition != null)
                {
                    cbxContract.ItemsSource = PurchaseDB.db.app_contract.Where(a => a.is_active == true
                                                        && a.id_company == CurrentSession.Id_Company
                                                        && a.id_condition == app_condition.id_condition).ToList();
                    //Selects first Item
                    if (purchase_invoice != null)
                    {
                        if (purchase_invoice.id_contract == 0)
                        {
                            cbxContract.SelectedIndex = 0;
                        }
                        else
                        {
                            cbxContract.SelectedValue = purchase_invoice.id_contract;
                        }
                    }
                }
            }
        }

        #endregion Filter Data

        #region Datagrid Events

        private void calculate_vat(object sender, EventArgs e)
        {
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
            if (purchase_invoice != null)
            {
                purchase_invoice.RaisePropertyChanged("GrandTotal");

                List<purchase_invoice_detail> purchase_invoice_detail = purchase_invoice.purchase_invoice_detail.ToList();
                dgvvat.ItemsSource = purchase_invoice_detail
                        .Join(PurchaseDB.db.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                            , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_cost * (cfx.app_vat.coefficient * cfx.percentage), id_vat = cfx.app_vat.id_vat, ad })
                            .GroupBy(a => new { a.name, a.id_vat, a.ad })
                    .Select(g => new
                    {
                        id_vat = g.Key.id_vat,
                        name = g.Key.name,
                        value = g.Sum(a => a.value * a.ad.quantity)
                    }).ToList();
            }
        }

        private void purchase_invoice_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void purchase_invoiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                if (purchase_invoice != null)
                {
                    calculate_vat(sender, e);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void purchase_invoice_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            purchase_invoice_detail purchase_invoice_detail = (purchase_invoice_detail)e.NewItem;
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
        }

        #endregion Datagrid Events

        #region Popup

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            InvoiceSetting _pref_PurchaseInvoice = new InvoiceSetting();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            InvoiceSetting.Default.Save();
            _pref_PurchaseInvoice = InvoiceSetting.Default;
            popupCustomize.IsOpen = false;
        }

        #endregion Popup

        private void item_Select(object sender, EventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;
            if (purchase_invoice != null)
            {
                item item = null;
                contact contact = null;

                if (sbxItem.ItemID > 0)
                {
                    item = PurchaseDB.db.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                }

                if (purchase_invoice.id_contact > 0 || sbxContact.ContactID > 0)
                {
                    contact = PurchaseDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                }

                InvoiceSetting InvoiceSetting = new InvoiceSetting();
                Task Thread = Task.Factory.StartNew(() => SelectProduct_Thread(sender, e, purchase_invoice, item, contact, InvoiceSetting.AllowDuplicateItems, sbxItem.Quantity));
            }
        }

        private void SelectProduct_Thread(object sender, EventArgs e, purchase_invoice purchase_invoice, item item, contact contact, bool AllowDuplicate, decimal quantity)
        {
            purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();
            purchase_invoice_detail.purchase_invoice = purchase_invoice;

            //ItemLink
            if (item != null)
            {
                purchase_invoice_detail detail_withitem = purchase_invoice.purchase_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                if (detail_withitem != null && AllowDuplicate)
                {
                    //Item Exists in Context, so add to sum.
                    purchase_invoice_detail _purchase_invoice_detail = detail_withitem;
                    _purchase_invoice_detail.quantity += quantity;
                    //Return because Item exists, and will +1 in Quantity
                    return;
                }
                else
                {
                    //Item DOES NOT Exist in Context
                    purchase_invoice_detail.item = item;
                    purchase_invoice_detail.id_item = item.id_item;
                    purchase_invoice_detail.item_description = item.supplier_name;
                    purchase_invoice_detail.quantity = quantity;

                    //If Item Exists in previous purchase... then get Last Cost. Problem, will get in stored value, in future we will need to add logic to convert into current currency.
                    purchase_invoice_detail old_PurchaseInvoice = PurchaseDB.db.purchase_invoice_detail
                        .Where(x => x.id_item == item.id_item && x.purchase_invoice.id_contact == purchase_invoice.id_contact)
                        .OrderByDescending(y => y.purchase_invoice.trans_date)
                        .FirstOrDefault();

                    if (old_PurchaseInvoice != null)
                    {
                        purchase_invoice_detail.id_vat_group = old_PurchaseInvoice.id_vat_group;
                        purchase_invoice_detail.unit_cost = old_PurchaseInvoice.unit_cost;
                    }
                }

                foreach (item_dimension item_dimension in item.item_dimension)
                {
                    purchase_invoice_dimension purchase_invoice_dimension = new purchase_invoice_dimension()
                    {
                        id_dimension = item_dimension.id_app_dimension,
                        app_dimension = item_dimension.app_dimension,
                        id_measurement = item_dimension.id_measurement,
                        app_measurement = item_dimension.app_measurement,
                        purchase_invoice_detail = purchase_invoice_detail,
                        value = item_dimension.value
                    };
                    purchase_invoice_detail.purchase_invoice_dimension.Add(purchase_invoice_dimension);
                }
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    purchase_invoice_detail.item_description = sbxItem.Text;
                }));
            }

            //Cost Center
            if (item != null)
            {
                //If Contact does not exist, and If product exist, then take defualt Product Cost Center. Else, bring Administrative
                if (item.item_product.Count() > 0)
                {
                    int CostCenter = PurchaseDB.db.app_cost_center.Where(a => a.is_product && a.is_active && a.id_company == CurrentSession.Id_Company).Select(y => y.id_cost_center).FirstOrDefault();

                    if (CostCenter > 0)
                    {
                        purchase_invoice_detail.id_cost_center = CostCenter;
                    }
                    else
                    {
                        app_cost_center cost_center = new app_cost_center();
                        cost_center.name = entity.Brillo.Localize.StringText("Product");
                        cost_center.is_product = true;
                        PurchaseDB.db.app_cost_center.Add(cost_center);

                        purchase_invoice_detail.app_cost_center = cost_center;
                    }
                }
                else if (item.item_asset.Count() > 0)
                {
                    int CostCenter = PurchaseDB.db.app_cost_center.Where(a => a.is_fixedasset && a.is_active && a.id_company == CurrentSession.Id_Company).Select(y => y.id_cost_center).FirstOrDefault();

                    if (CostCenter > 0)
                    {
                        purchase_invoice_detail.id_cost_center = CostCenter;
                    }
                    else
                    {
                        app_cost_center cost_center = new app_cost_center();
                        cost_center.name = entity.Brillo.Localize.StringText("FixedAsset");
                        cost_center.is_fixedasset = true;
                        PurchaseDB.db.app_cost_center.Add(cost_center);

                        purchase_invoice_detail.app_cost_center = cost_center;
                    }
                }
            }
            else
            {
                if (contact != null && contact.id_cost_center != null)
                {
                    purchase_invoice_detail.id_cost_center = (int)contact.id_cost_center;
                }
                else
                {
                    int id_cost_center = PurchaseDB.db.app_cost_center.Where(a => a.is_administrative == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).Select(y => y.id_cost_center).FirstOrDefault();
                    if (id_cost_center > 0)
                    {
                        purchase_invoice_detail.id_cost_center = id_cost_center;
                    }
                    else
                    {
                        app_cost_center cost_center = new app_cost_center();
                        cost_center.name = entity.Brillo.Localize.StringText("Expense");
                        cost_center.is_administrative = true;
                        PurchaseDB.db.app_cost_center.Add(cost_center);

                        purchase_invoice_detail.app_cost_center = cost_center;
                    }
                }
            }

            //VAT
            if (item != null)
            {
                if (item.id_vat_group > 0)
                {
                    purchase_invoice_detail.id_vat_group = item.id_vat_group;
                }
            }
            else if (PurchaseDB.db.app_vat_group.Where(x => x.is_active && x.is_default && x.id_company == CurrentSession.Id_Company).Any())
            {
                purchase_invoice_detail.id_vat_group = PurchaseDB.db.app_vat_group.Where(x => x.is_active && x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && purchase_invoiceViewSource != null)
            {
                purchase_invoiceViewSource.View.Filter = i =>
                {
                    purchase_invoice purchase_invoice = i as purchase_invoice;
                    string number = purchase_invoice.number != null ? purchase_invoice.number : "";
                    string contact = purchase_invoice.contact != null ? purchase_invoice.contact.name : "";

                    if (contact.ToLower().Contains(query.ToLower()) || number.ToLower().Contains(query.ToLower()))
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
                purchase_invoiceViewSource.View.Filter = null;
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as purchase_invoice_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;
                if (purchase_invoice != null)
                {
                    if (PurchaseDB.db.purchase_packing_relation.Where(x => x.id_purchase_invoice == purchase_invoice.id_purchase_invoice).Count() == 0)
                    {
                        MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            //DeleteDetailGridRow
                            dgvPurchaseDetail.CancelEdit();

                            PurchaseDB.db.purchase_invoice_detail.Remove(e.Parameter as purchase_invoice_detail);
                            purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
                        }
                    }
                }
                purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;
            if (purchase_invoice != null)
            {
                if (purchase_invoice.id_currencyfx > 0)
                {
                    app_currencyfx app_currencyfx = PurchaseDB.db.app_currencyfx.Where(x => x.id_currencyfx == purchase_invoice.id_currencyfx).FirstOrDefault();
                    if (app_currencyfx != null)
                    {
                        purchase_invoice.app_currencyfx = app_currencyfx;
                    }
                }
            }
            calculate_vat(sender, e);
        }

        private void btnDuplicateInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;
            if (purchase_invoice != null)
            {
                if (purchase_invoice.id_purchase_invoice != 0)
                {
                    using (db db = new db())
                    {
                        var originalEntity = db.purchase_invoice.AsNoTracking()
                                        .FirstOrDefault(x => x.id_purchase_invoice == purchase_invoice.id_purchase_invoice);
                        db.purchase_invoice.Add(originalEntity);
                        purchase_invoiceViewSource.View.Refresh();
                        purchase_invoiceViewSource.View.MoveCurrentToLast();
                    }
                }
                else
                {
                    toolBar.msgWarning("Please save before duplicating");
                }
            }
        }

        private void btnRecivePayment_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceDataGrid.SelectedItem as purchase_invoice;

            if (purchase_invoice != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.Curd.receive_payment recive_payment = new cntrl.Curd.receive_payment();
                recive_payment.purchase_invoice = purchase_invoice;
                crud_modal.Children.Add(recive_payment);
            }
        }

        private void purchase_invoice_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void btnBurnInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Open Purchase Order
        /// </summary>
        private void btnPurchaseOreder_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlPurchaseOrder = new cntrl.PanelAdv.pnlPurchaseOrder();
            pnlPurchaseOrder._entity = PurchaseDB.db;

            if (sbxContact.ContactID > 0)
            {
                contact contact = PurchaseDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                if (contact != null)
                {
                    pnlPurchaseOrder._contact = contact;
                }
            }
            pnlPurchaseOrder.PurchaseOrder_Click += PurchaseOrder_Click;
            crud_modal.Children.Add(pnlPurchaseOrder);
        }

        /// <summary>
        /// Save Purchase Order
        /// </summary>
        public void PurchaseOrder_Click(object sender)
        {
            CollectionViewSource purchase_invoicepurchase_invoice_detailViewSource = FindResource("purchase_invoicepurchase_invoice_detailViewSource") as CollectionViewSource;
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceViewSource.View.CurrentItem;

            foreach (purchase_order purchase_order in pnlPurchaseOrder.selected_purchase_order)
            {
                purchase_invoice.contact = purchase_order.contact;
                purchase_invoice.id_contact = purchase_order.id_contact;
                purchase_invoice.RaisePropertyChanged("contact");

                purchase_invoice.app_department = purchase_order.app_department;
                purchase_invoice.id_department = purchase_order.id_department;

                purchase_invoice.app_condition = purchase_order.app_condition;
                purchase_invoice.id_condition = purchase_order.id_condition;

                purchase_invoice.app_contract = purchase_order.app_contract;
                purchase_invoice.id_contract = purchase_order.id_contract;
                purchase_invoice.is_impex = purchase_order.is_impex;

                if (purchase_order.id_project != null)
                {
                    purchase_invoice.project = purchase_order.project;
                    purchase_invoice.id_project = purchase_order.id_project;
                    purchase_invoice.RaisePropertyChanged("project");
                }

                foreach (purchase_order_detail _purchase_order_detail in purchase_order.purchase_order_detail)
                {
                    if (_purchase_order_detail.item != null && _purchase_order_detail.item.id_item_type == item.item_type.ServiceContract)
                    {
                        production_service_account production_service_account = PurchaseDB.db.production_service_account.
                                                                                 Where(x => x.id_purchase_order_detail == _purchase_order_detail.id_purchase_order_detail).FirstOrDefault();
                        if (production_service_account != null)
                        {
                            List< production_service_account> production_service_accountList = production_service_account.child.
                                                                 Where(x => x.id_purchase_invoice_detail == null).ToList() ;
                            foreach (production_service_account item in production_service_accountList)
                            {
                                purchase_invoice_detail purchase_invoice_detailProdcution = new purchase_invoice_detail();
                                purchase_invoice_detailProdcution.id_cost_center = _purchase_order_detail.id_cost_center;
                                purchase_invoice_detailProdcution.comment = _purchase_order_detail.comment;
                                purchase_invoice_detailProdcution.discount = _purchase_order_detail.discount;
                                purchase_invoice_detailProdcution.id_item = _purchase_order_detail.id_item;
                                purchase_invoice_detailProdcution.item_description = _purchase_order_detail.item_description;
                                purchase_invoice_detailProdcution.id_location = _purchase_order_detail.id_location;
                                purchase_invoice_detailProdcution.purchase_order_detail = _purchase_order_detail;
                                purchase_invoice_detailProdcution.id_vat_group = _purchase_order_detail.id_vat_group;
                                purchase_invoice_detailProdcution.quantity = item.debit;
                                purchase_invoice_detailProdcution.unit_cost = item.unit_cost;
                                purchase_invoice_detailProdcution.batch_code = _purchase_order_detail.batch_code;
                                purchase_invoice_detailProdcution.expire_date = _purchase_order_detail.expire_date;
                                purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detailProdcution);
                            }
                        }
                        else
                        {
                            toolBar.msgWarning("Item Not Used");
                            return;
                        }

                    }
                    else
                    {
                        if (purchase_invoice.purchase_invoice_detail.Where(x => x.id_item == _purchase_order_detail.id_item).Count() == 0)
                        {
                            purchase_invoice.State = EntityState.Modified;

                            purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail()
                            {
                                purchase_invoice = purchase_invoice,
                                id_purchase_order_detail = _purchase_order_detail.id_purchase_order_detail,
                                id_vat_group = _purchase_order_detail.id_vat_group,
                                app_cost_center = _purchase_order_detail.app_cost_center,
                                id_cost_center = _purchase_order_detail.id_cost_center,
                                item = _purchase_order_detail.item,
                                id_item = _purchase_order_detail.id_item,
                                item_description = _purchase_order_detail.item_description,
                                quantity = _purchase_order_detail.quantity - PurchaseDB.db.purchase_invoice_detail
                                                                        .Where(x => x.id_purchase_order_detail == _purchase_order_detail.id_purchase_order_detail)
                                                                        .GroupBy(x => x.id_purchase_order_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault(),
                                unit_cost = _purchase_order_detail.unit_cost,
                                batch_code = _purchase_order_detail.batch_code,
                                expire_date = _purchase_order_detail.expire_date
                            };
                            foreach (purchase_order_dimension purchase_order_dimension in _purchase_order_detail.purchase_order_dimension)
                            {
                                purchase_invoice_dimension purchase_invoice_dimension = new purchase_invoice_dimension()
                                {
                                    id_dimension = purchase_order_dimension.id_dimension,
                                    value = purchase_order_dimension.value,
                                    id_measurement = purchase_order_dimension.id_measurement
                                };

                                //Add Dimension to Detail
                                purchase_invoice_detail.purchase_invoice_dimension.Add(purchase_invoice_dimension);
                            }
                            //Add Detail to Header
                            purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                        }
                    }
                }
            }

            PurchaseDB.db.Entry(purchase_invoice).Entity.State = EntityState.Added;
            purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
            calculate_vat(sender, null);
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
        }

        private void purchaseorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            purchase_order purchase_order = (purchase_order)Hyperlink.Tag;
            if (purchase_order != null)
            {
                entity.Brillo.Document.Start.Manual(purchase_order, purchase_order.app_document_range);
            }
        }

        private void dgvRow_ShowRowDetail(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            DataGridRow Row = e.OriginalSource as DataGridRow;

            if (Row != null)
            {
                if (Row.DetailsVisibility == System.Windows.Visibility.Collapsed)
                {
                    Row.DetailsVisibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Row.DetailsVisibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void chbxRowDetail_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;
            if ((bool)chbx.IsChecked)
            {
                dgvPurchaseDetail.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                dgvPurchaseDetail.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (PurchaseDB.db != null)
            {
                if (disposing)
                {
                    PurchaseDB.db.Dispose();
                    // Dispose other managed resources.
                }
                //release unmanaged resources.
            }
        }

        private void Fraction_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;

            foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice.purchase_invoice_detail)
            {
                purchase_invoice_detail.Quantity_Factored = entity.Brillo.ConversionFactor.Factor_Quantity(purchase_invoice_detail.item, purchase_invoice_detail.quantity, purchase_invoice_detail.GetDimensionValue());
                purchase_invoice_detail.RaisePropertyChanged("Quantity_Factored");
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceDataGrid.SelectedItem as purchase_invoice;

            if (purchase_invoice != null && purchase_invoice.status != Status.Documents_General.Pending)
            {
                app_document_range app_document_range;

                if (purchase_invoice.app_document_range != null)
                {
                    app_document_range = purchase_invoice.app_document_range;
                }
                else
                {
                    app_document app_document = new entity.app_document();
                    app_document.id_application = entity.App.Names.PurchaseInvoice;
                    app_document.name = "PurchaseInvoice";

                    app_document_range = new app_document_range();
                    app_document_range.use_default_printer = false;
                    app_document_range.app_document = app_document;
                }

                entity.Brillo.Document.Start.Manual(purchase_invoice, app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        private void btnInvoiceNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceDataGrid.SelectedItem as purchase_invoice;

            if (purchase_invoice != null && !string.IsNullOrEmpty(purchase_invoice.number) && purchase_invoice.id_contact > 0)
            {
                using (db db = new db())
                {
                    if (db.purchase_invoice.Where(x => x.number == purchase_invoice.number && x.id_contact == purchase_invoice.id_contact).Count() > 0)
                    {
                        toolBar.msgWarning("Duplicate Invoice");
                    }
                }
            }
        }

        private void lblTransCode_LostFocus(object sender, RoutedEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceDataGrid.SelectedItem as purchase_invoice;

            if (purchase_invoice != null)
            {
                if (purchase_invoice.contact != null)
                {
                    if (purchase_invoice.contact.trans_code != purchase_invoice.code)
                    {
                        purchase_invoice.contact.trans_code = purchase_invoice.code;
                    }
                }
            }
        }

        private void toolBar_btnReturn_Click(object sender, MouseButtonEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;

            if (purchase_invoice != null && purchase_invoice.status == Status.Documents_General.Approved && purchase_invoice.purchase_return.Count() == 0)
            {
                purchase_return purchase_return = new purchase_return();
                purchase_return.barcode = purchase_invoice.barcode;
                purchase_return.code = purchase_invoice.code;
                purchase_return.trans_date = DateTime.Now;
                purchase_return.comment = purchase_invoice.comment;
                purchase_return.id_condition = purchase_invoice.id_condition;
                purchase_return.id_contact = purchase_invoice.id_contact;
                purchase_return.contact = purchase_return.contact;
                purchase_return.id_contract = purchase_invoice.id_contract;
                purchase_return.id_currencyfx = purchase_invoice.id_currencyfx;
                purchase_return.id_project = purchase_invoice.id_project;
                purchase_return.id_sales_rep = purchase_invoice.id_sales_rep;
                purchase_return.id_weather = purchase_invoice.id_weather;
                purchase_return.is_impex = purchase_invoice.is_impex;
                purchase_return.purchase_invoice = purchase_invoice;
                foreach (purchase_invoice_detail detail in purchase_invoice.purchase_invoice_detail)
                {
                    purchase_return_detail purchase_return_detail = new purchase_return_detail();
                    purchase_return_detail.id_cost_center = detail.id_cost_center;
                    purchase_return_detail.comment = detail.comment;
                    purchase_return_detail.discount = detail.discount;
                    purchase_return_detail.id_item = detail.id_item;
                    purchase_return_detail.item_description = detail.item_description;
                    purchase_return_detail.id_location = detail.id_location;
                    purchase_return_detail.purchase_invoice_detail = detail;
                    purchase_return_detail.id_vat_group = detail.id_vat_group;
                    purchase_return_detail.quantity = detail.quantity - (detail.purchase_return_detail != null ? detail.purchase_return_detail.Sum(x => x.quantity) : 0);
                    purchase_return_detail.unit_cost = detail.unit_cost;
                    purchase_return_detail.batch_code = detail.batch_code;
                    purchase_return_detail.expire_date = detail.expire_date;
                    purchase_return.purchase_return_detail.Add(purchase_return_detail);
                }

                PurchaseDB.db.purchase_return.Add(purchase_return);
                PurchaseDB.db.SaveChanges();
                MessageBox.Show("Return Created Successfully..");
            }
            else
            {
                MessageBox.Show("Return Already Created Or Status is Not Approved ..");
            }
        }

        private void Refinance_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceDataGrid.SelectedItem as purchase_invoice;
            if (purchase_invoice != null)
            {
                entity.Brillo.Security Security = new entity.Brillo.Security(entity.App.Names.AccountsReceivable);
                if (Security.create)
                {
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.Curd.RefinancePurchase RefinancePurchase = new cntrl.Curd.RefinancePurchase();
                    RefinancePurchase.purchase_invoice = purchase_invoice;
                    crud_modal.Children.Add(RefinancePurchase);
                }
                else
                {
                    toolBar.msgWarning("Access Denied. Please contact your Administrator.");
                }
            }
        }


    }
}