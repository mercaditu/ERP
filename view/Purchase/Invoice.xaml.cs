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
using System.Windows.Documents;
using cntrl.Class;

namespace Cognitivo.Purchase
{
    public partial class Invoice : Page, IDisposable
    {
        CollectionViewSource purchase_invoiceViewSource;
        CollectionViewSource purchase_invoicepurchase_invoice_detailViewSource;

        PurchaseInvoiceDB PurchaseInvoiceDB = new PurchaseInvoiceDB();

        cntrl.PanelAdv.pnlPurchaseOrder pnlPurchaseOrder = new cntrl.PanelAdv.pnlPurchaseOrder();

        public Invoice()
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
            InvoiceSetting InvoiceSetting = new InvoiceSetting();
            if (InvoiceSetting.filterbyBranch)
            {
                await PurchaseInvoiceDB.purchase_invoice.Where(a => a.id_company == CurrentSession.Id_Company && a.id_branch == CurrentSession.Id_Company).Include(x => x.contact).OrderByDescending(x => x.trans_date).ToListAsync();
            }
            else
            {
                await PurchaseInvoiceDB.purchase_invoice.Where(a => a.id_company == CurrentSession.Id_Company).Include(x => x.contact).OrderByDescending(x => x.trans_date).ToListAsync();
            }

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                purchase_invoiceViewSource = ((CollectionViewSource)(FindResource("purchase_invoiceViewSource")));
                purchase_invoiceViewSource.Source = PurchaseInvoiceDB.purchase_invoice.Local;
            }));
        }

        private async void load_SecondaryDataThread()
        {
            await PurchaseInvoiceDB.app_department.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDepartment.ItemsSource = PurchaseInvoiceDB.app_department.Local;
            }));

            await PurchaseInvoiceDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
                app_dimensionViewSource.Source = PurchaseInvoiceDB.app_dimension.Local;
            }));

            await PurchaseInvoiceDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
                app_measurementViewSource.Source = PurchaseInvoiceDB.app_measurement.Local;
            }));

            await PurchaseInvoiceDB.app_cost_center.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active).OrderBy(a => a.name).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_cost_centerViewSource = FindResource("app_cost_centerViewSource") as CollectionViewSource;
                app_cost_centerViewSource.Source = PurchaseInvoiceDB.app_cost_center.Local;
            }));
        }

        private void pageInvoice_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                purchase_invoicepurchase_invoice_detailViewSource = ((CollectionViewSource)(FindResource("purchase_invoicepurchase_invoice_detailViewSource")));
                load_PrimaryData();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #region Toolbar events
        private void toolBar_btnNew_Click(object sender)
        {
            InvoiceSetting _pref_PurchaseInvoice = new InvoiceSetting();
            purchase_invoice purchase_invoice = PurchaseInvoiceDB.New(_pref_PurchaseInvoice.TransDate_OffSet);

            sbxContact.Text = "";
            sbxItem.Text = "";

            purchase_invoiceViewSource.View.MoveCurrentTo(purchase_invoice);
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_invoiceDataGrid.SelectedItem != null)
            {
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                purchase_invoice.IsSelected = true;
                purchase_invoice.State = EntityState.Modified;
                PurchaseInvoiceDB.Entry(purchase_invoice).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }
        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                    purchase_invoice.is_head = false;
                    purchase_invoice.State = EntityState.Deleted;
                    purchase_invoice.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {


            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;

            if (purchase_invoice.status == Status.Documents_General.Approved)
            {

                if (purchase_invoice != null)
                {
                    UpdateMovementReApprove UpdateMovementReApprove = new UpdateMovementReApprove();
                    CheckMovementReApprove CheckMovementReApprove = new CheckMovementReApprove();
                    UpdatePaymentReApprove UpdatePaymentReApprove = new UpdatePaymentReApprove();
                    CheckPaymentReApprove CheckPaymentReApprove = new CheckPaymentReApprove();
                    //  MovementReApprove.Start(SalesInvoiceDB,sales_invoice.id_sales_invoice,entity.App.Names.SalesInvoice);
                    string Message = CheckPaymentReApprove.Check_ContractChanges(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);

                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdatePaymentReApprove.Update_ContractChanges(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                        }
                    }
                    Message = CheckPaymentReApprove.Check_ValueUP(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdatePaymentReApprove.Update_ValueUP(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);


                        }
                    }
                    Message = CheckPaymentReApprove.Check_ValueDown(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdatePaymentReApprove.Update_ValueDown(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);


                        }
                    }
                    Message += CheckPaymentReApprove.Check_CurrencyChange(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdatePaymentReApprove.Update_CurrencyChange(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);


                        }
                    }
                    Message = CheckPaymentReApprove.Check_DateChange(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdatePaymentReApprove.Update_DateChange(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);


                        }
                    }



                    Message = "";
                    Message = CheckMovementReApprove.CheckQuantityUP(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);

                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdateMovementReApprove.QuantityUP(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                        }
                    }
                    Message = CheckMovementReApprove.CheckQuantityDown(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);

                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdateMovementReApprove.QuantityDown(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                        }
                    }
                    Message = CheckMovementReApprove.CheckDateChange(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);

                    if (Message != "")
                    {
                        Message += "\n" + "Are You Sure Want To Change The Data..";
                        if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UpdateMovementReApprove.DateChange(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                        }

                        Message = CheckMovementReApprove.CheckNewMovement(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);

                        if (Message != "")
                        {
                            Message += "\n" + "Are You Sure Want To Change The Data..";
                            if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                UpdateMovementReApprove.NewMovement(PurchaseInvoiceDB, purchase_invoice.id_purchase_invoice, entity.App.Names.PurchaseInvoice);
                            }
                        }




                    }
                    //SalesInvoiceDB.ReApprove(sales_invoice);
                    purchase_invoiceViewSource.View.Refresh();
                    PurchaseInvoiceDB.SaveChanges();
                }
                else
                {

                    if (PurchaseInvoiceDB.SaveChanges() > 0)
                    {
                        purchase_invoiceViewSource.View.Refresh();
                        toolBar.msgSaved(PurchaseInvoiceDB.NumberOfRecords);
                    }
                }

            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            PurchaseInvoiceDB.CancelAllChanges();
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
            purchase_invoice.State = EntityState.Unchanged;
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            PurchaseInvoiceDB.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            purchase_invoice purchase_invoice = purchase_invoiceDataGrid.SelectedItem as purchase_invoice;
            if (purchase_invoice != null)
            {
                purchase_invoice.status = Status.Documents_General.Annulled;
                crud_modalAnull.Visibility = Visibility.Visible;
                cntrl.PanelAdv.ActionPanelAnull ActionPanelAnull = new cntrl.PanelAdv.ActionPanelAnull();
                ActionPanelAnull.ID = purchase_invoice.id_purchase_invoice;
                ActionPanelAnull.Application = entity.App.Names.PurchaseInvoice;
                ActionPanelAnull.db = PurchaseInvoiceDB;
                crud_modalAnull.Children.Add(ActionPanelAnull);

            }
        }

        #endregion

        #region Filter Data


        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = PurchaseInvoiceDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                purchase_invoice.id_contact = contact.id_contact;
                purchase_invoice.contact = contact;
                purchase_invoice.code = contact.trans_code;
                purchase_invoice.RaisePropertyChanged("code");

                ///Start Thread to get Data.
                Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
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
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
            //Contract
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = PurchaseInvoiceDB.app_contract.Where(a => a.is_active == true
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


        #endregion

        #region Datagrid Events
        private void calculate_vat(object sender, EventArgs e)
        {
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
            purchase_invoice.RaisePropertyChanged("GrandTotal");
            if (purchase_invoice != null)
            {
                List<purchase_invoice_detail> purchase_invoice_detail = purchase_invoice.purchase_invoice_detail.ToList();
                dgvvat.ItemsSource = purchase_invoice_detail
                        .Join(PurchaseInvoiceDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                            , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_cost * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
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
        //    purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
        //    if (purchase_invoice != null)
        //    {
        //        purchase_invoice.get_Puchase_Total();
        //    }
        //}

        private void purchase_invoice_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            purchase_invoice_detail purchase_invoice_detail = (purchase_invoice_detail)dgvPurchaseDetail.SelectedItem;
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        //private void purchase_invoiceDataGrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //calculate_total(sender, e);
        //}

        private void purchase_invoiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                if (purchase_invoice != null)
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
            purchase_invoice_detail purchase_invoice_detail = (purchase_invoice_detail)e.NewItem;
            purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
            //p   purchase_invoice_detail.id_branch = purchase_invoice.id_branch;
        }
        #endregion

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
        #endregion

        private void item_Select(object sender, EventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;
            if (purchase_invoice != null)
            {
                item item = null;
                contact contact = null;

                if (sbxItem.ItemID > 0)
                {
                    item = PurchaseInvoiceDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                }

                if (purchase_invoice.id_contact > 0 || sbxContact.ContactID > 0)
                {
                    contact = PurchaseInvoiceDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                }

                InvoiceSetting InvoiceSetting = new InvoiceSetting();
                Task Thread = Task.Factory.StartNew(() => SelectProduct_Thread(sender, e, purchase_invoice, item, contact, InvoiceSetting.AllowDuplicateItems));
            }
        }

        private void SelectProduct_Thread(object sender, EventArgs e, purchase_invoice purchase_invoice, item item, contact contact, bool AllowDuplicate)
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
                    _purchase_invoice_detail.quantity += 1;
                    //Return because Item exists, and will +1 in Quantity
                    return;
                }
                else
                {
                    //Item DOES NOT Exist in Context
                    purchase_invoice_detail.item = item;
                    purchase_invoice_detail.id_item = item.id_item;
                    purchase_invoice_detail.item_description = item.name;
                    purchase_invoice_detail.quantity = 1;

                    //If Item Exists in previous purchase... then get Last Cost. Problem, will get in stored value, in future we will need to add logic to convert into current currency.
                    purchase_invoice_detail old_PurchaseInvoice = PurchaseInvoiceDB.purchase_invoice_detail
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
                    purchase_invoice_dimension purchase_invoice_dimension = new purchase_invoice_dimension();
                    purchase_invoice_dimension.id_dimension = item_dimension.id_app_dimension;
                    purchase_invoice_dimension.app_dimension = item_dimension.app_dimension;
                    purchase_invoice_dimension.id_measurement = item_dimension.id_measurement;
                    purchase_invoice_dimension.app_measurement = item_dimension.app_measurement;
                    purchase_invoice_dimension.purchase_invoice_detail = purchase_invoice_detail;
                    purchase_invoice_dimension.value = item_dimension.value;
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
            if (contact != null)
            {
                if (contact.app_cost_center != null)
                {
                    app_cost_center app_cost_center = contact.app_cost_center;
                    if (app_cost_center.id_cost_center > 0)
                        purchase_invoice_detail.id_cost_center = app_cost_center.id_cost_center;
                }
            }
            else
            {
                //If Contact does not exist, and If product exist, then take defualt Product Cost Center. Else, bring Administrative
                if (item != null)
                {
                    int id_cost_center = 0;

                    if (item.item_product != null)
                    {
                        app_cost_center app_cost_center = PurchaseInvoiceDB.app_cost_center.Where(a => a.is_product && a.is_active && a.id_company == CurrentSession.Id_Company).FirstOrDefault();
                        if (app_cost_center != null)
                            id_cost_center = Convert.ToInt32(app_cost_center.id_cost_center);
                        if (id_cost_center > 0)
                            purchase_invoice_detail.id_cost_center = id_cost_center;
                    }
                    else if (item.item_asset != null)
                    {
                        app_cost_center app_cost_center = PurchaseInvoiceDB.app_cost_center.Where(a => a.is_fixedasset == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault();
                        if (app_cost_center != null)
                            id_cost_center = Convert.ToInt32(app_cost_center.id_cost_center);
                        if (id_cost_center > 0)
                            purchase_invoice_detail.id_cost_center = id_cost_center;
                    }
                }
                else
                {
                    int id_cost_center = 0;
                    app_cost_center app_cost_center = PurchaseInvoiceDB.app_cost_center.Where(a => a.is_administrative == true && a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault();
                    if (app_cost_center != null)
                        id_cost_center = Convert.ToInt32(app_cost_center.id_cost_center);
                    if (id_cost_center > 0)
                        purchase_invoice_detail.id_cost_center = id_cost_center;
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
            else if (PurchaseInvoiceDB.app_vat_group.Where(x => x.is_active && x.is_default && x.id_company == CurrentSession.Id_Company).Any())
            {
                purchase_invoice_detail.id_vat_group = PurchaseInvoiceDB.app_vat_group.Where(x => x.is_active && x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query) && purchase_invoiceViewSource != null)
                {
                    purchase_invoiceViewSource.View.Filter = i =>
                    {
                        purchase_invoice purchase_invoice = i as purchase_invoice;
                        string number = purchase_invoice.number != null ? purchase_invoice.number : "";
                        if (purchase_invoice.contact.name.ToLower().Contains(query.ToLower()) || number.ToLower().Contains(query.ToLower()))
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
            catch (Exception)
            { }
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
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;

                    //DeleteDetailGridRow
                    dgvPurchaseDetail.CancelEdit();

                    PurchaseInvoiceDB.purchase_invoice_detail.Remove(e.Parameter as purchase_invoice_detail);
                    purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
                }
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
                    if (PurchaseInvoiceDB.app_currencyfx.Where(x => x.id_currencyfx == purchase_invoice.id_currencyfx).FirstOrDefault() != null)
                    {


                        purchase_invoice.app_currencyfx = PurchaseInvoiceDB.app_currencyfx.Where(x => x.id_currencyfx == purchase_invoice.id_currencyfx).FirstOrDefault();
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
            pnlPurchaseOrder._entity = PurchaseInvoiceDB;
            if (sbxContact.ContactID > 0)
            {
                contact contact = PurchaseInvoiceDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlPurchaseOrder._contact = contact;
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

                purchase_invoice.app_department = purchase_order.app_department;
                purchase_invoice.id_department = purchase_order.id_department;

                purchase_invoice.app_condition = purchase_order.app_condition;
                purchase_invoice.id_condition = purchase_order.id_condition;

                purchase_invoice.app_contract = purchase_order.app_contract;
                purchase_invoice.id_contract = purchase_order.id_contract;

                if (purchase_order.project != null)
                {
                    purchase_invoice.project = purchase_order.project;
                    purchase_invoice.id_project = purchase_order.id_project;
                }

                foreach (purchase_order_detail _purchase_order_detail in purchase_order.purchase_order_detail)
                {
                    if (purchase_invoice.purchase_invoice_detail.Where(x => x.id_item == _purchase_order_detail.id_item).Count() == 0)
                    {
                        purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();
                        purchase_invoice.State = EntityState.Modified;
                        purchase_invoice_detail.purchase_invoice = purchase_invoice;
                        purchase_invoice_detail.id_purchase_order_detail = _purchase_order_detail.id_purchase_order_detail;
                        purchase_invoice_detail.id_vat_group = _purchase_order_detail.id_vat_group;
                        purchase_invoice_detail.app_cost_center = _purchase_order_detail.app_cost_center;
                        purchase_invoice_detail.id_cost_center = _purchase_order_detail.id_cost_center;
                        purchase_invoice_detail.item = _purchase_order_detail.item;
                        purchase_invoice_detail.id_item = _purchase_order_detail.id_item;
                        purchase_invoice_detail.item_description = _purchase_order_detail.item_description;
                        purchase_invoice_detail.quantity = _purchase_order_detail.quantity - PurchaseInvoiceDB.purchase_invoice_detail
                                                                    .Where(x => x.id_purchase_order_detail == _purchase_order_detail.id_purchase_order_detail)
                                                                    .GroupBy(x => x.id_purchase_order_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault();
                        purchase_invoice_detail.unit_cost = _purchase_order_detail.unit_cost;

                        foreach (purchase_order_dimension purchase_order_dimension in _purchase_order_detail.purchase_order_dimension)
                        {
                            purchase_invoice_dimension purchase_invoice_dimension = new purchase_invoice_dimension();
                            purchase_invoice_dimension.id_dimension = purchase_order_dimension.id_dimension;
                            purchase_invoice_dimension.value = purchase_order_dimension.value;
                            purchase_invoice_dimension.id_measurement = purchase_order_dimension.id_measurement;

                            //Add Dimension to Detail
                            purchase_invoice_detail.purchase_invoice_dimension.Add(purchase_invoice_dimension);
                        }
                        //Add Detail to Header
                        purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                    }
                }
            }

            PurchaseInvoiceDB.Entry(purchase_invoice).Entity.State = EntityState.Added;
            purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
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

        private void Totals_btnClean_Click(object sender)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;

            if (purchase_invoice != null)
            {
                decimal TrailingDecimals = purchase_invoice.GrandTotal - Math.Floor(purchase_invoice.GrandTotal);
                purchase_invoice.DiscountWithoutPercentage += TrailingDecimals;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (PurchaseInvoiceDB != null)
            {
                if (disposing)
                {
                    PurchaseInvoiceDB.Dispose();
                    // Dispose other managed resources.
                }
                //release unmanaged resources.
            }
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
            if (purchase_invoice != null)
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
                if (PurchaseInvoiceDB.purchase_invoice.Where(x => x.number == purchase_invoice.number && x.id_purchase_invoice > 0 && x.id_contact == purchase_invoice.id_contact).Any())
                {
                    toolBar.msgWarning("Duplicate Invoice");
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
                    purchase_invoice.contact.trans_code = purchase_invoice.code;
                }
            }
        }
    }
}
