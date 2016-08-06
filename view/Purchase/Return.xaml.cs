using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Threading.Tasks;

namespace Cognitivo.Purchase
{
    public partial class Return : Page
    {
        PurchaseReturnDB dbContext = new PurchaseReturnDB();

        CollectionViewSource
            purchaseReturnViewSource,
            purchaseInvoiceViewSource,
            purchase_returnpurchase_return_detailViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        cntrl.PanelAdv.pnlPurchaseInvoice pnlPurchaseInvoice;
        public Return()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //PurchaseReturn
            purchaseReturnViewSource = (CollectionViewSource)FindResource("purchase_returnViewSource");
            dbContext.purchase_return.Where(a => a.id_company == _entity.company_ID).OrderByDescending(x => x.trans_date).Load();
            purchaseReturnViewSource.Source = dbContext.purchase_return.Local;
            purchase_returnpurchase_return_detailViewSource = FindResource("purchase_returnpurchase_return_detailViewSource") as CollectionViewSource;

            //PurchaseInvoice
            purchaseInvoiceViewSource = (CollectionViewSource)FindResource("purchase_invoiceViewSource");
            purchaseInvoiceViewSource.Source = dbContext.purchase_invoice.Where(a => a.id_company == _entity.company_ID).ToList();

            CollectionViewSource currencyfxViewSource = (CollectionViewSource)FindResource("currencyfxViewSource");
            dbContext.app_currencyfx.Include("app_currency").Where(x => x.app_currency.id_company == _entity.company_ID).Load();
            currencyfxViewSource.Source = dbContext.app_currencyfx.Local;

            CollectionViewSource app_cost_centerViewSource = (CollectionViewSource)FindResource("app_cost_centerViewSource");
            app_cost_centerViewSource.Source = dbContext.app_cost_center.Where(a => a.is_active == true && a.id_company == _entity.company_ID).ToList();

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.Get_VAT_Group(); //dbContext.app_vat_group.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).ToList();

            cbxReturnType.ItemsSource = Enum.GetValues(typeof(Status.ReturnTypes));
        }

        private void toolBar_btnDelete_Click(object sender)
        {

            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                dbContext.purchase_return.Remove((purchase_return)purchase_returnDataGrid.SelectedItem);
                purchaseReturnViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            purchase_return purchase_return = dbContext.New();
            dbContext.purchase_return.Add(purchase_return);
            purchaseReturnViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                toolBar.msgSaved(dbContext.NumberOfRecords);
                sbxContact.Text = "";   
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_returnDataGrid.SelectedItem != null)
            {
                purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
                purchase_return.IsSelected = true;
                purchase_return.State = EntityState.Modified;
                dbContext.Entry(purchase_return).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        #region Datagrid Events
        private void calculate_vat(object sender, EventArgs e)
        {
            //purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
            //purchase_return.RaisePropertyChanged("GrandTotal");
            //List<purchase_return_detail> purchase_return_detail = purchase_return.purchase_return_detail.ToList();
            //dgvvat.ItemsSource = purchase_return_detail
            //     .Join(dbContext.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
            //          , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_cost * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
            //          .GroupBy(a => new { a.name, a.id_vat, a.ad })
            //   .Select(g => new
            //   {
            //       id_vat = g.Key.id_vat,
            //       name = g.Key.name,
            //       value = g.Sum(a => a.value * a.ad.quantity)
            //   }).ToList();
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

        #endregion

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    purchaseReturnViewSource.View.Filter = i =>
                    {
                        purchase_return purchase_return = i as purchase_return;
                        if (purchase_return.contact.name.ToLower().Contains(query.ToLower()))
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
            catch (Exception)
            { }
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
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    purchase_return purchase_return = purchaseReturnViewSource.View.CurrentItem as purchase_return;
                    //DeleteDetailGridRow
                    purchase_return_detailDataGrid.CancelEdit();
                    dbContext.purchase_return_detail.Remove(e.Parameter as purchase_return_detail);
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
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
                purchase_return.id_contact = contact.id_contact;
                purchase_return.contact = contact;

                if (purchase_return == null)
                {
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

        private void item_Select(object sender, EventArgs e)
        {
            try
            {
                if (sbxItem.ItemID > 0)
                {
                    item item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                    purchase_return purchase_return = purchase_returnDataGrid.SelectedItem as purchase_return;
                    purchase_return_detail purchase_return_detail = purchase_return.purchase_return_detail.Where(a => a.id_item == sbxItem.ItemID).FirstOrDefault();
                    int id_cost_center = 0;
                    if (purchase_return_detail == null)
                    {
                        purchase_return_detail _purchase_return_detail = new entity.purchase_return_detail();
                        //Check for contact
                        if (sbxContact.ContactID > 0)
                        {
                            contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
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
                                    if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                        id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                            }
                            else
                            {
                                if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                    id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                if (id_cost_center > 0)
                                    _purchase_return_detail.id_cost_center = id_cost_center;
                            }
                        }
                        else
                        {
                            if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                            if (id_cost_center > 0)
                                _purchase_return_detail.id_cost_center = id_cost_center;
                        }
                        if (dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault() != null)
                        {
                            _purchase_return_detail.id_vat_group = dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault().id_vat_group;
                        }
                        _purchase_return_detail.purchase_return = purchase_return;
                        _purchase_return_detail.item = item;
                        _purchase_return_detail.id_item = sbxItem.ItemID;
                        purchase_return.purchase_return_detail.Add(_purchase_return_detail);
                    }
                    else
                    {
                        purchase_return_detail.quantity += 1;
                    }
                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                    //calculate_total(sender, e);
                    calculate_vat(sender, e);

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
                            contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
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
                                    if (dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                        id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                    if (id_cost_center > 0)
                                        _purchase_return_detail.id_cost_center = id_cost_center;
                                }
                            }
                            else
                            {
                                if (dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                    id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                                if (id_cost_center > 0)
                                    _purchase_return_detail.id_cost_center = id_cost_center;
                            }
                        }
                        else
                        {
                            if (dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault() != null)
                                id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == false && a.is_active == true && a.id_company == _entity.company_ID).FirstOrDefault().id_cost_center);
                            if (id_cost_center > 0)
                                _purchase_return_detail.id_cost_center = id_cost_center;
                        }
                        if (dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault() != null)
                        {
                            _purchase_return_detail.id_vat_group = dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _entity.company_ID).FirstOrDefault().id_vat_group;
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
            dbContext.Approve();
            foreach (purchase_return purchase_return in purchaseReturnViewSource.View.Cast<purchase_return>().ToList())
            {
                purchase_return.IsSelected = false;
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            dbContext.Anull();
            foreach (purchase_return purchase_return in purchaseReturnViewSource.View.Cast<purchase_return>().ToList())
            {
                purchase_return.IsSelected = false;
            }
        }

        private void cbxCurrency_MouseEnter(object sender, MouseEventArgs e)
        {
            //purchase_return purchase_return = (purchase_return)purchase_returnDataGrid.SelectedItem;
            //if (purchase_return != null)
            //    purchase_return.is_edit = true;
        }

        private void purchase_return_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void btnPurchaseInvoice_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlPurchaseInvoice = new cntrl.PanelAdv.pnlPurchaseInvoice();
            pnlPurchaseInvoice._entity = new ImpexDB();
            //    pnlSalesInvoice.contactViewSource = contactViewSource;
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlPurchaseInvoice._contact = contact;
            }

            pnlPurchaseInvoice.PurchaseInvoice_Click += PurchaseInvoice_Click;
            crud_modal.Children.Add(pnlPurchaseInvoice);
        }
        public async void PurchaseInvoice_Click(object sender)
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
                    if (_purchase_return.purchase_return_detail.Where(x => x.id_item == _purchase_invoice_detail.id_item).Count() == 0)
                    {
                        purchase_return_detail purchase_return_detail = new purchase_return_detail();
                        purchase_return_detail.id_purchase_invoice_detail = _purchase_invoice_detail.id_purchase_invoice_detail;
                        purchase_return_detail.id_cost_center = _purchase_invoice_detail.id_cost_center;
                        purchase_return_detail.id_location = _purchase_invoice_detail.id_location;

                        if (dbContext.app_location.Where(x => x.id_location == _purchase_invoice_detail.id_location).FirstOrDefault() != null)
                        {
                            purchase_return_detail.app_location = dbContext.app_location.Where(x => x.id_location == _purchase_invoice_detail.id_location).FirstOrDefault();
                        }

                        purchase_return_detail.purchase_return = _purchase_return;

                        if (dbContext.items.Where(x => x.id_item == _purchase_invoice_detail.id_item).FirstOrDefault() != null)
                        {
                            purchase_return_detail.id_item = _purchase_invoice_detail.id_item;
                            purchase_return_detail.item = dbContext.items.Where(x => x.id_item == _purchase_invoice_detail.id_item).FirstOrDefault();
                        }
                 
                        purchase_return_detail.item_description = _purchase_invoice_detail.item_description;

                        purchase_return_detail.quantity = _purchase_invoice_detail.quantity - dbContext.purchase_return_detail
                                                                                     .Where(x => x.id_purchase_invoice_detail == _purchase_invoice_detail.id_purchase_invoice_detail)
                                                                                     .GroupBy(x => x.id_purchase_invoice_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault();
                        
                        purchase_return_detail.id_vat_group = _purchase_invoice_detail.id_vat_group;
                        purchase_return_detail.unit_cost = _purchase_invoice_detail.unit_cost;
                        purchase_return_detail.CurrencyFX_ID = _purchase_return.id_currencyfx;
                        _purchase_return.purchase_return_detail.Add(purchase_return_detail);
                    }
                    dbContext.Entry(_purchase_return).Entity.State = EntityState.Added;
                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                    purchaseReturnViewSource.View.Refresh();

                    purchase_returnpurchase_return_detailViewSource.View.Refresh();
                }
            }
        }
    }
}
