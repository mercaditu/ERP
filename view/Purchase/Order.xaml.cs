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
using System.Windows.Input;

namespace Cognitivo.Purchase
{
    public partial class Order : Page, INotifyPropertyChanged
    {
        private CollectionViewSource purchase_orderViewSource;
        private CollectionViewSource purchase_orderpurchase_order_detailViewSource;
        private PurchaseOrderDB PurchaseOrderDB = new PurchaseOrderDB();

        public Order()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void load_PrimaryData()
        {
            load_PrimaryDataThread();
            load_SecondaryDataThread();
        }

        private async void load_PrimaryDataThread()
        {
            OrderSetting OrderSetting = new OrderSetting();
            if (OrderSetting.filterbyBranch)
            {
                await PurchaseOrderDB.purchase_order.Where(a => a.id_company == CurrentSession.Id_Company && a.id_branch == CurrentSession.Id_Branch
                                      ).Include(x => x.contact).OrderByDescending(x => x.trans_date).ToListAsync();
            }
            else
            {
                await PurchaseOrderDB.purchase_order.Where(a => a.id_company == CurrentSession.Id_Company
                                      ).Include(x => x.contact).OrderByDescending(x => x.trans_date).ToListAsync();
            }

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                purchase_orderViewSource = (FindResource("purchase_orderViewSource") as CollectionViewSource);
                purchase_orderViewSource.Source = PurchaseOrderDB.purchase_order.Local;
            }));
        }

        private async void load_SecondaryDataThread()
        {
            await PurchaseOrderDB.app_department.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).ToListAsync();
            cbxDepartment.ItemsSource = PurchaseOrderDB.app_department.Local;

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cmbdocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PurchaseOrderDB, entity.App.Names.PurchaseOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
            }));

            await PurchaseOrderDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
                app_dimensionViewSource.Source = PurchaseOrderDB.app_dimension.Local;
            }));

            await PurchaseOrderDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
                app_measurementViewSource.Source = PurchaseOrderDB.app_measurement.Local;
            }));

            await PurchaseOrderDB.app_cost_center.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active).OrderBy(a => a.name).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_cost_centerViewSource = FindResource("app_cost_centerViewSource") as CollectionViewSource;
                app_cost_centerViewSource.Source = PurchaseOrderDB.app_cost_center.Local;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            purchase_orderpurchase_order_detailViewSource = FindResource("purchase_orderpurchase_order_detailViewSource") as CollectionViewSource;
            load_PrimaryData();
        }

        private void New_Click(object sender)
        {
            OrderSetting _pref_PurchaseOrder = new OrderSetting();
            purchase_order purchase_order = PurchaseOrderDB.New(_pref_PurchaseOrder.TransDate_OffSet);
            if (purchase_order != null)
            {
                sbxContact.Text = "";
                sbxItem.Text = "";
                purchase_orderViewSource.View.MoveCurrentTo(purchase_order);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_orderDataGrid != null)
            {
                if (purchase_orderDataGrid.SelectedItem != null)
                {
                    purchase_order purchase_order_old = purchase_orderDataGrid.SelectedItem as purchase_order;

                    if (purchase_order_old != null)
                    {
                        purchase_order_old.IsSelected = true;
                        purchase_order_old.State = EntityState.Modified;
                        PurchaseOrderDB.Entry(purchase_order_old).State = EntityState.Modified;
                    }
                }
                else
                {
                    toolBar.msgWarning("Please select a record");
                }
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
                    purchase_order.is_head = false;
                    purchase_order.State = EntityState.Deleted;
                    purchase_order.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Save_Click(object sender)
        {
            try
            {
                if (PurchaseOrderDB.SaveChanges() > 0)
                {
                    toolBar.msgSaved(PurchaseOrderDB.NumberOfRecords);
                    purchase_orderViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            PurchaseOrderDB.CancelAllChanges();
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            if (PurchaseOrderDB.Approve())
            {
                cmbdocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PurchaseOrderDB, entity.App.Names.PurchaseOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cmbdocument.SelectedIndex = 0;
                toolBar.msgApproved(PurchaseOrderDB.NumberOfRecords);
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            if (PurchaseOrderDB.Anull())
            {
                toolBar.msgAnnulled(PurchaseOrderDB.NumberOfRecords);
            }
        }

        #region Filter Data

        private async void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = await PurchaseOrderDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefaultAsync();

                if (contact != null)
                {
                    purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;

                    if (purchase_order != null)
                    {
                        purchase_order.id_contact = contact.id_contact;
                        purchase_order.contact = contact;

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
                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    //Currency
                    cbxCurrency.get_ActiveRateXContact(ref objContact);
                }));
            }
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
            //Contract
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                if (app_condition != null)
                {
                    cbxContract.ItemsSource = CurrentSession.Contracts.Where(x => x.id_condition == app_condition.id_condition).ToList();
                    //Selects first Item
                    if (purchase_order != null)
                    {
                        if (purchase_order.id_contract == 0)
                        {
                            cbxContract.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        #endregion Filter Data

        #region Datagrid Events

        private void calculate_vat(object sender, EventArgs e)
        {
            purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;

            if (purchase_order != null)
            {
                purchase_order.RaisePropertyChanged("GrandTotal");

                List<purchase_order_detail> purchase_order_detail = purchase_order.purchase_order_detail.ToList();
                dgvVAT.ItemsSource = purchase_order_detail
                        .Join(PurchaseOrderDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
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

        private void purchase_orderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
                if (purchase_order != null)
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
            purchase_order_detail purchase_order_detail = (purchase_order_detail)e.NewItem;
            purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
        }

        #endregion Datagrid Events

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;

            OrderSetting _pref_PurchaseOrder = new OrderSetting();
            OrderSetting.Default.Save();
            _pref_PurchaseOrder = OrderSetting.Default;

            InvoiceSetting _pref_PurchaseInvoice = new InvoiceSetting();
            InvoiceSetting.Default.Save();
            _pref_PurchaseInvoice = InvoiceSetting.Default;

            popupCustomize.IsOpen = false;
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    purchase_orderViewSource.View.Filter = i =>
                    {
                        purchase_order purchase_order = i as purchase_order;
                        string contact = purchase_order.contact != null ? purchase_order.contact.name : "";
                        string number = string.IsNullOrEmpty(purchase_order.number) ? "" : purchase_order.number;

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
                    purchase_orderViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as purchase_order_detail != null)
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
                    purchase_order purchase_order = purchase_orderViewSource.View.CurrentItem as purchase_order;
                    if (purchase_order != null)
                    {
                        //DeleteDetailGridRow
                        purchase_order_detailDataGrid.CancelEdit();
                        purchase_order.purchase_order_detail.Remove(e.Parameter as purchase_order_detail);
                        purchase_orderpurchase_order_detailViewSource.View.Refresh();
                        // calculate_total(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_Select(object sender, EventArgs e)
        {
            purchase_order purchase_order = purchase_orderViewSource.View.CurrentItem as purchase_order;
            if (purchase_order != null)
            {
                item item = null;
                contact contact = null;

                if (sbxItem.ItemID > 0)
                {
                    item = PurchaseOrderDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                }

                if (purchase_order.id_contact > 0 || sbxContact.ContactID > 0)
                {
                    contact = PurchaseOrderDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                }

                InvoiceSetting InvoiceSetting = new InvoiceSetting();
                Task Thread = Task.Factory.StartNew(() => SelectProduct_Thread(sender, e, purchase_order, item, contact, InvoiceSetting.AllowDuplicateItems, sbxItem.Quantity));
            }
        }

        private void SelectProduct_Thread(object sender, EventArgs e, purchase_order purchase_order, item item, contact contact, bool AllowDuplicate, decimal quantity)
        {
            purchase_order_detail purchase_order_detail = new purchase_order_detail();
            purchase_order_detail.purchase_order = purchase_order;

            //ItemLink
            if (item != null)
            {
                if (purchase_order.purchase_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() != null && AllowDuplicate == false)
                {
                    //Item Exists in Context, so add to sum.
                    purchase_order_detail _purchase_order_detail = purchase_order.purchase_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                    _purchase_order_detail.quantity += quantity;

                    //Return because Item exists, and will +1 in Quantity
                    return;
                }
                else
                {
                    //If Item Exists in previous purchase... then get Last Cost. Problem, will get in stored value, in future we will need to add logic to convert into current currency.
                    purchase_invoice_detail purchase_invoice_detail = PurchaseOrderDB.purchase_invoice_detail
                        .Where(x => x.id_item == item.id_item && x.purchase_invoice.id_contact == purchase_order.id_contact && x.purchase_invoice.status == Status.Documents_General.Approved)
                        .OrderByDescending(y => y.purchase_invoice.trans_date)
                        .FirstOrDefault();
                    if (purchase_invoice_detail != null)
                    {
                        purchase_order_detail.unit_cost = purchase_invoice_detail.unit_cost;
                    }

                    //Item DOES NOT Exist in Context
                    purchase_order_detail.item = item;
                    purchase_order_detail.id_item = item.id_item;
                    purchase_order_detail.item_description = item.supplier_name;
                    purchase_order_detail.quantity = quantity;
                }
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    purchase_order_detail.item_description = sbxItem.Text;
                }));
            }

            //Cost Center
            if (item != null)
            {
                //If Contact does not exist, and If product exist, then take defualt Product Cost Center. Else, bring Administrative
                if (item.item_product.Count() > 0)
                {
                    int CostCenter = PurchaseOrderDB.app_cost_center.Where(a => a.is_product && a.is_active && a.id_company == CurrentSession.Id_Company).Select(y => y.id_cost_center).FirstOrDefault();

                    if (CostCenter > 0)
                    {
                        purchase_order_detail.id_cost_center = CostCenter;
                    }
                    else
                    {
                        app_cost_center cost_center = new app_cost_center();
                        cost_center.name = entity.Brillo.Localize.StringText("Product");
                        cost_center.is_product = true;
                        PurchaseOrderDB.app_cost_center.Add(cost_center);

                        purchase_order_detail.app_cost_center = cost_center;
                    }
                }
                else if (item.item_asset.Count() > 0)
                {
                    int CostCenter = PurchaseOrderDB.app_cost_center.Where(a => a.is_fixedasset && a.is_active && a.id_company == CurrentSession.Id_Company).Select(y => y.id_cost_center).FirstOrDefault();

                    if (CostCenter > 0)
                    {
                        purchase_order_detail.id_cost_center = CostCenter;
                    }
                    else
                    {
                        app_cost_center cost_center = new app_cost_center();
                        cost_center.name = entity.Brillo.Localize.StringText("FixedAsset");
                        cost_center.is_fixedasset = true;
                        PurchaseOrderDB.app_cost_center.Add(cost_center);

                        purchase_order_detail.app_cost_center = cost_center;
                    }
                }
            }
            else
            {
                if (contact != null && contact.id_cost_center != null)
                {
                    purchase_order_detail.id_cost_center = (int)contact.id_cost_center;
                }
                else
                {
                    int id_cost_center = PurchaseOrderDB.app_cost_center.Where(a => a.is_administrative == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).Select(y => y.id_cost_center).FirstOrDefault();
                    if (id_cost_center > 0)
                    {
                        purchase_order_detail.id_cost_center = id_cost_center;
                    }
                    else
                    {
                        app_cost_center cost_center = new app_cost_center();
                        cost_center.name = entity.Brillo.Localize.StringText("Expense");
                        cost_center.is_administrative = true;
                        PurchaseOrderDB.app_cost_center.Add(cost_center);

                        purchase_order_detail.app_cost_center = cost_center;
                    }
                }
            }

            //VAT
            if (item != null)
            {
                if (item.id_vat_group > 0)
                {
                    purchase_order_detail.id_vat_group = item.id_vat_group;
                }
            }
            else if (PurchaseOrderDB.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault() != null)
            {
                purchase_order_detail.id_vat_group = PurchaseOrderDB.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                purchase_order.purchase_order_detail.Add(purchase_order_detail);
                purchase_orderpurchase_order_detailViewSource.View.Refresh();
                calculate_vat(sender, e);
            }));
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            purchase_order purchase_order = purchase_orderDataGrid.SelectedItem as purchase_order;
            if (purchase_order != null)
            {
                if (purchase_order.id_currencyfx > 0)
                {
                    if (PurchaseOrderDB.app_currencyfx.Where(x => x.id_currencyfx == purchase_order.id_currencyfx).FirstOrDefault() != null)
                    {
                        purchase_order.app_currencyfx = PurchaseOrderDB.app_currencyfx.Where(x => x.id_currencyfx == purchase_order.id_currencyfx).FirstOrDefault();
                    }
                }
            }
            calculate_vat(sender, e);
        }

        public string _number { get; set; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbdocument.SelectedValue != null)
            {
                //entity.Brillo.Logic.Document _Document = new entity.Brillo.Logic.Document();
                app_document_range app_document_range = cmbdocument.SelectedItem as app_document_range;

                _number = entity.Brillo.Logic.Range.calc_Range(app_document_range, false);
            }
        }

        private void chbxRowDetail_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;
            if ((bool)chbx.IsChecked)
            {
                purchase_order_detailDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                purchase_order_detailDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }

        private void purchase_order_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            purchase_order purchase_order = purchase_orderDataGrid.SelectedItem as purchase_order;
            if (purchase_order != null)
            {
                purchase_order.app_document_range = purchase_order.app_document_range != null ? PurchaseOrderDB.app_document_range.Find(purchase_order.id_range) : purchase_order.app_document_range;

                if (purchase_order.app_document_range != null)
                {
                    entity.Brillo.Document.Start.Manual(purchase_order, purchase_order.app_document_range);
                }
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            purchase_order purchase_order = purchase_orderDataGrid.SelectedItem as purchase_order;
            foreach (purchase_order_detail purchase_order_detail in purchase_order.purchase_order_detail)
            {
                purchase_order_detail.Quantity_Factored = entity.Brillo.ConversionFactor.Factor_Quantity(purchase_order_detail.item, purchase_order_detail.quantity, purchase_order_detail.GetDimensionValue());
                purchase_order_detail.RaisePropertyChanged("Quantity_Factored");
            }
        }

        private void toolBar_btnInvoice_Click(object sender, MouseButtonEventArgs e)
        {
            purchase_order purchase_order = purchase_orderViewSource.View.CurrentItem as purchase_order;
            if (purchase_order != null && purchase_order.status == Status.Documents_General.Approved && purchase_order.purchase_invoice.Count() == 0)
            {
                purchase_invoice purchase_invoice = new purchase_invoice();
                purchase_invoice.barcode = purchase_order.barcode;
                purchase_invoice.code = purchase_order.code;
                purchase_invoice.trans_date = DateTime.Now;
                purchase_invoice.comment = purchase_order.comment;
                purchase_invoice.id_condition = purchase_order.id_condition;
                purchase_invoice.id_contact = purchase_order.id_contact;
                purchase_invoice.contact = purchase_order.contact;
                purchase_invoice.id_contract = purchase_order.id_contract;
                purchase_invoice.id_currencyfx = purchase_order.id_currencyfx;
                purchase_invoice.id_project = purchase_order.id_project;
                purchase_invoice.id_sales_rep = purchase_order.id_sales_rep;
                purchase_invoice.id_weather = purchase_order.id_weather;
                purchase_invoice.is_impex = purchase_order.is_impex;
                purchase_invoice.purchase_order = purchase_order;
                purchase_invoice.id_department = purchase_order.id_department;

                foreach (purchase_order_detail detail in purchase_order.purchase_order_detail)
                {
                    purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();
                    purchase_invoice_detail.id_cost_center = detail.id_cost_center;
                    purchase_invoice_detail.comment = detail.comment;
                    purchase_invoice_detail.discount = detail.discount;
                    purchase_invoice_detail.id_item = detail.id_item;
                    purchase_invoice_detail.item_description = detail.item_description;
                    purchase_invoice_detail.id_location = detail.id_location;
                    purchase_invoice_detail.purchase_order_detail = detail;
                    purchase_invoice_detail.id_vat_group = detail.id_vat_group;
                    purchase_invoice_detail.quantity = detail.quantity - detail.purchase_invoice_detail.Sum(x => x.quantity);
                    purchase_invoice_detail.unit_cost = detail.unit_cost;
                    purchase_invoice_detail.batch_code = detail.batch_code;
                    purchase_invoice_detail.expire_date = detail.expire_date;

                    foreach (purchase_order_dimension dim in detail.purchase_order_dimension)
                    {
                        purchase_invoice_dimension dimension = new purchase_invoice_dimension();
                        dimension.id_dimension = dim.id_dimension;
                        dimension.id_measurement = dim.id_measurement;
                        dimension.value = dim.value;
                        
                        purchase_invoice_detail.purchase_invoice_dimension.Add(dimension);
                    }

                    purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                }

                PurchaseOrderDB.purchase_invoice.Add(purchase_invoice);
                toolBar.msgApproved(PurchaseOrderDB.SaveChanges());
            }
            else
            {
                toolBar.msgWarning("Please check that Order is Approved.");
            }
        }
    }
}