using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Data;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Cognitivo.Purchase
{
    public partial class Order : Page, INotifyPropertyChanged
    {
        CollectionViewSource purchase_orderViewSource;
        CollectionViewSource purchase_orderpurchase_order_detailViewSource;
        PurchaseOrderDB PurchaseOrderDB = new PurchaseOrderDB();

        public Order()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (PurchaseOrderDB != null)
        //    {
        //        if (disposing)
        //        {
        //            PurchaseOrderDB.Dispose();
        //            // Dispose other managed resources.
        //        }
        //        //release unmanaged resources.
        //    }
        //}
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
                purchase_orderViewSource = ((CollectionViewSource)(FindResource("purchase_orderViewSource")));
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
           

            sbxContact.Text = "";
            sbxItem.Text = "";
            purchase_orderViewSource.View.MoveCurrentTo(purchase_order);
      
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_orderDataGrid.SelectedItem != null)
            {
                purchase_order purchase_order_old = (purchase_order)purchase_orderDataGrid.SelectedItem;
                purchase_order_old.IsSelected = true;
                purchase_order_old.State = EntityState.Modified;
                PurchaseOrderDB.Entry(purchase_order_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a record");
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
            if (PurchaseOrderDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(PurchaseOrderDB.NumberOfRecords);

                purchase_orderViewSource.View.Refresh();
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
                    purchase_order.id_contact = contact.id_contact;
                    purchase_order.contact = contact;

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
        #endregion

        #region Datagrid Events
        private void calculate_vat(object sender, EventArgs e)
        {
            purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
            purchase_order.RaisePropertyChanged("GrandTotal");
            if (purchase_order != null)
            {
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

        //private void calculate_total(object sender, EventArgs e)
        //{
        //    purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
        //    if (purchase_order != null)
        //    {
        //        purchase_order.get_Puchase_Total();
        //    }
        //}

        private void purchase_invoice_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            purchase_order_detail purchase_order_detail = (purchase_order_detail)purchase_order_detailDataGrid.SelectedItem;
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        //private void purchase_invoiceDataGrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    calculate_total(sender, e);
        //}

        private void purchase_orderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
                if (purchase_order != null)
                {
                    calculate_vat(sender, e);

                    
                }
                //calculate_total(sender, e);
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
        #endregion

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
                        if (purchase_order.contact.name.ToLower().Contains(query.ToLower()))
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
                    //DeleteDetailGridRow
                    purchase_order_detailDataGrid.CancelEdit();
                    purchase_order.purchase_order_detail.Remove(e.Parameter as purchase_order_detail);
                    purchase_orderpurchase_order_detailViewSource.View.Refresh();
                    // calculate_total(sender, e);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }



        private void item_Select(object sender, EventArgs e)
        {
            purchase_order purchase_order = purchase_orderDataGrid.SelectedItem as purchase_order;
            item item = null;
            contact contact = null;

            if (sbxItem.ItemID > 0)
            {

                item = PurchaseOrderDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                if (sbxContact.ContactID > 0)
                {
                    contact = PurchaseOrderDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                }
            }
            OrderSetting OrderSetting = new OrderSetting();
            Task Thread = Task.Factory.StartNew(() => SelectProduct_Thread(sender, e, purchase_order, item, contact, OrderSetting.AllowDuplicateItems));
        }

        private void SelectProduct_Thread(object sender, EventArgs e, purchase_order purchase_order, item item, contact contact, bool AllowDuplicate)
        {
            purchase_order_detail purchase_order_detail = new purchase_order_detail();
            purchase_order_detail.purchase_order = purchase_order;
            //ItemLink 
            if (item != null)
            {
                if (purchase_order.purchase_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() != null  && AllowDuplicate == false)
                {
                    //Item Exists in Context, so add to sum.
                    purchase_order_detail _purchase_order_detail = purchase_order.purchase_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                    _purchase_order_detail.quantity += 1;

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
                    purchase_order_detail.item_description = item.name;
                    purchase_order_detail.quantity = 1;
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
            if (contact != null && contact.app_cost_center != null)
            {
                app_cost_center app_cost_center = contact.app_cost_center;
                if (app_cost_center.id_cost_center > 0)
                    purchase_order_detail.id_cost_center = app_cost_center.id_cost_center;
            }
            else
            {
                if (item != null)
                {
                    int id_cost_center = 0;

                    if (item.item_product != null)
                    {
                        app_cost_center app_cost_center = PurchaseOrderDB.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault();
                        if (app_cost_center != null)
                            id_cost_center = Convert.ToInt32(app_cost_center.id_cost_center);
                        if (id_cost_center > 0)
                            purchase_order_detail.id_cost_center = id_cost_center;   
                    }
                    else if (item.item_asset != null)
                    {
                        app_cost_center app_cost_center = PurchaseOrderDB.app_cost_center.Where(a => a.is_fixedasset == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault();
                        if (app_cost_center != null)
                            id_cost_center = Convert.ToInt32(app_cost_center.id_cost_center);
                        if (id_cost_center > 0)
                            purchase_order_detail.id_cost_center = id_cost_center;
                    }
                }
                else
                {
                    int id_cost_center = 0;
                    app_cost_center app_cost_center = PurchaseOrderDB.app_cost_center.Where(a => a.is_administrative == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault();
                    if (app_cost_center  != null)
                        id_cost_center = Convert.ToInt32(app_cost_center.id_cost_center);
                    if (id_cost_center > 0)
                        purchase_order_detail.id_cost_center = id_cost_center;
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
                entity.Brillo.Logic.Document _Document = new entity.Brillo.Logic.Document();
                app_document_range app_document_range = (app_document_range)cmbdocument.SelectedItem;
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
                entity.Brillo.Document.Start.Manual(purchase_order, purchase_order.app_document_range);
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

        
    }
}
