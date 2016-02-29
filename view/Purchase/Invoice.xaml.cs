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
using System.Data.Entity.Validation;
using System.Windows.Media;
using System.Windows.Documents;

namespace Cognitivo.Purchase
{
    public partial class Invoice : Page
    {
        CollectionViewSource purchase_invoiceViewSource;
        entity.Properties.Settings _setting = new entity.Properties.Settings();

        PurchaseInvoiceDB dbContext = new PurchaseInvoiceDB();
        contact _contact = new contact();
        ContactDB ContactdbContext = new ContactDB();
     
        CollectionViewSource purchase_invoicepurchase_invoice_detailViewSource;
        cntrl.PanelAdv.pnlPurchaseOrder pnlPurchaseOrder = new cntrl.PanelAdv.pnlPurchaseOrder();

        public Invoice()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void load_PrimaryData()
        {
            load_PrimaryDataThread();
            load_SecondaryDataThread();
        }

        private async void load_PrimaryDataThread()
        {
            Cognitivo.Purchase.InvoiceSetting InvoiceSetting = new Cognitivo.Purchase.InvoiceSetting();
            if (InvoiceSetting.filterbyBranch)
            {
                await dbContext.purchase_invoice.Where(a => a.id_company == _setting.company_ID && a.id_branch==_setting.branch_ID).ToListAsync();   
            }
            else
            {
                await dbContext.purchase_invoice.Where(a => a.id_company == _setting.company_ID).ToListAsync();
            }

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                purchase_invoiceViewSource = ((CollectionViewSource)(FindResource("purchase_invoiceViewSource")));
                purchase_invoiceViewSource.Source = dbContext.purchase_invoice.Local;
            }));
        }

        private async void load_SecondaryDataThread()
        {
        
            dbContext.app_contract.Where(a => a.is_active == true && a.id_company == _setting.company_ID).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxContract.ItemsSource = dbContext.app_contract.Local;
            }));


            dbContext.app_department.Where(a => a.is_active == true && a.id_company == _setting.company_ID).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDepartment.ItemsSource = dbContext.app_department.Local;
            }));


            dbContext.app_condition.Where(a => a.is_active == true && a.id_company == _setting.company_ID).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxCondition.ItemsSource = dbContext.app_condition.Local;
            }));

            dbContext.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == _setting.company_ID).OrderBy(b => b.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxBranch.ItemsSource = dbContext.app_branch.Local;
            }));

            dbContext.app_vat_group.Where(a => a.is_active == true && a.id_company == _setting.company_ID).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = dbContext.app_vat_group.Local;
            }));

          

           

            dbContext.app_cost_center.Where(a => a.id_company == _setting.company_ID && a.is_active == true).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_cost_centerViewSource = FindResource("app_cost_centerViewSource") as CollectionViewSource;
                app_cost_centerViewSource.Source = dbContext.app_cost_center.Local;
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

            purchase_invoice purchase_invoice = dbContext.New();
            purchase_invoice.trans_date = DateTime.Now.AddDays(_pref_PurchaseInvoice.TransDate_OffSet);
            dbContext.Entry(purchase_invoice).State = EntityState.Added;
            
            purchase_invoiceViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_invoiceDataGrid.SelectedItem != null)
            {
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                purchase_invoice.IsSelected = true;
                purchase_invoice.State = EntityState.Modified;
                dbContext.Entry(purchase_invoice).State = EntityState.Modified;
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
            dbContext.SaveChanges();
            purchase_invoiceViewSource.View.Refresh();
            toolBar.msgSaved();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            dbContext.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            dbContext.Anull();
        }

        #endregion

        #region Filter Data
     

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                purchase_invoice.id_contact = contact.id_contact;
                purchase_invoice.contact = contact;

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

             
                await dbContext.projects.Where(a => a.is_active == true && a.id_company == _setting.company_ID && a.id_contact == objContact.id_contact).OrderBy(a => a.name).ToListAsync();
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxProject.ItemsSource = dbContext.projects.Local;
                }));
            }
        }

        private async void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Contract
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = await dbContext.app_contract.Where(a => a.is_active == true
                                                                        && a.id_company == _setting.company_ID
                                                                        && a.id_condition == app_condition.id_condition).ToListAsync();
                //Selects first Item
                cbxContract.SelectedIndex = 0;
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
                        .Join(dbContext.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
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

        #region curd extra
        private void hrefAddCust_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            sbxContact.Contact.State = EntityState.Added;
            sbxContact.Contact.is_customer = true;
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.contact contact = new cntrl.Curd.contact();
            contact.btnSave_Click += Save_Click;
            contact.contactobject = sbxContact.Contact;
            crud_modal.Children.Add(contact);

        }

        public void Save_Click(object sender)
        {
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
            load_PrimaryData();
        }

        private void btnEditContact(object sender, MouseButtonEventArgs e)
        {
            //dbContext entity = new dbContext();
            //if (sbxContact.ContactID > 0)
            //{
            //    contact contact = ContactdbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

            //}
           
            //_contact.State = EntityState.Modified;
            //ContactdbContext.contacts.Add(_contact);

            //if (_contact != null)
            //{
          
            //    crud_modal.Visibility = Visibility.Visible;
              
            //    cntrl.Curd.contact contact = new cntrl.Curd.contact();
            //    contact.contactobject = _contact;
            //    contact.btnSave_Click += Save_Click;
            //    crud_modal.Children.Add(contact);
            //}
            //else
            //{
            //    MessageBox.Show("Please select contact first.", "Cognitivo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            //}
        }
        //public void Save_Click(object sender)
        //{
        //    if (_contact.State == EntityState.Added)
        //    {
        //        ContactdbContext.contacts.Add(_contact);
        //    }
        //    _contact.IsSelected = true;

        //    IEnumerable<DbEntityValidationResult> validationresult = ContactdbContext.GetValidationErrors();
        //    if (validationresult.Count() == 0)
        //    {
        //        try
        //        {
        //            ContactdbContext.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            toolBar.msgError(ex);
        //        }

        //        crud_modal.Children.Clear();
        //        crud_modal.Visibility = Visibility.Collapsed;
        //        contactComboBox.Text = _contact.name;
        //        load_PrimaryData();
        //    }
        //    else
        //    {
        //        MessageBox.Show("error");
        //    }

        //}




        private void btnNewCondition(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource conditionViewSource = (CollectionViewSource)FindResource("conditionViewSource");
                db.app_condition.Where(a => a.is_active == true && a.id_company == _setting.company_ID).OrderBy(a => a.name).Load();
                conditionViewSource.Source = db.app_condition.Local;
                dbContext entity = new dbContext();
                crud_modal.Visibility = Visibility.Visible;
                cntrl.condition condition = new cntrl.condition();
                //condition.conditionViewSource = conditionViewSource;
                //condition.MainViewSource = purchase_invoiceViewSource;
                //condition.curObject = purchase_invoiceViewSource.View.CurrentItem;
                //condition.entity = entity;
                //condition.operationMode = cntrl.Class.clsCommon.Mode.Add;
                //condition.isExternalCall = true;
                crud_modal.Children.Add(condition);
            }
        }

        private void btnEditCondition(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource conditionViewSource = (CollectionViewSource)FindResource("conditionViewSource");
                db.app_condition.Where(a => a.is_active == true && a.id_company == _setting.company_ID).OrderBy(a => a.name).Load();
                conditionViewSource.Source = db.app_condition.Local;
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                if (app_condition != null)
                {
                    dbContext entity = new dbContext();
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.condition condition = new cntrl.condition();
                    //condition.conditionViewSource = conditionViewSource;
                    //condition.MainViewSource = purchase_invoiceViewSource;
                    //condition.curObject = purchase_invoiceViewSource.View.CurrentItem;
                    //condition.entity = entity;
                    //condition.app_conditionobject = app_condition;
                    //condition.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                    //condition.isExternalCall = true;
                    crud_modal.Children.Add(condition);
                }
            }
        }

        private void btnNewContract(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource contractViewSource = (CollectionViewSource)FindResource("contractViewSource");
                db.app_contract.Where(a => a.is_active == true && a.id_company == _setting.company_ID).Load();
                contractViewSource.Source = db.app_contract.Local;

                dbContext entity = new dbContext();
                crud_modal.Visibility = Visibility.Visible;
                cntrl.contract contract = new cntrl.contract();
                contract.app_contractViewSource = contractViewSource;
                contract.MainViewSource = purchase_invoiceViewSource;
                contract.curObject = purchase_invoiceViewSource.View.CurrentItem;
                contract.entity = entity;
                contract.operationMode = cntrl.Class.clsCommon.Mode.Add;
                contract.isExternalCall = true;
                crud_modal.Children.Add(contract);
            }
        }

        private void btnEditContract(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource contractViewSource = (CollectionViewSource)FindResource("contractViewSource");
                db.app_contract.Where(a => a.is_active == true && a.id_company == _setting.company_ID).Load();
                contractViewSource.Source = db.app_contract.Local;

                app_contract app_contract = cbxContract.SelectedItem as app_contract;
                if (app_contract != null)
                {
                    dbContext entity = new dbContext();
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.contract contract = new cntrl.contract();
                    contract.app_contractViewSource = contractViewSource;
                    contract.MainViewSource = purchase_invoiceViewSource;
                    contract.curObject = purchase_invoiceViewSource.View.CurrentItem;
                    contract.entity = entity;
                    contract.app_contractobject = app_contract;
                    contract.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                    contract.isExternalCall = true;
                    crud_modal.Children.Add(contract);
                }
            }
        }
        private void lblEditProduct_PreviewMouseUp(object sender, RoutedEventArgs e)
        {
            //using (db db = new db())
            //{
            //    CollectionViewSource itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
            //    itemViewSource.Source = db.items.Local.Where(x => x.is_active == true);
            //    //No Aditional Filter for Detail

            //    item _item;
            //    _item = (item)itemComboBox.Data;
            //    if (_item != null)
            //    {
            //        dbContext entity = new dbContext();
            //        itemComboBox.Text = "";
            //        itemComboBox.Data = null;
            //        crud_modal.Visibility = Visibility.Visible;
            //        itemComboBox.IsDisplayed = false;
            //        cntrl.Curd.item item = new cntrl.Curd.item();
            //        item.itemViewSource = itemViewSource;
            //        item.MainViewSource = purchase_invoiceViewSource;
            //        item.curObject = purchase_invoiceViewSource.View.CurrentItem;
            //        item._entity = entity;
            //        item.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            //        item.STbox = itemComboBox;
            //        item.itemobject = _item;
            //        crud_modal.Children.Add(item);
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please select item first.", "Cognitivo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            //    }
            //}
        }
        #endregion

        private void item_Select(object sender, EventArgs e)
        {
            purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;
            item item = null;
            contact contact = null;

            if (sbxItem.ItemID > 0)
            {
                item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && purchase_invoice != null)
                {
                    if (sbxContact.ContactID>0)
                    {
                        contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();                        
                    }
                    
                }
            }

            Task Thread = Task.Factory.StartNew(() => SelectProduct_Thread(sender, e, purchase_invoice, item, contact));
        }

      

       

        private void SelectProduct_Thread(object sender, EventArgs e,purchase_invoice purchase_invoice, item item, contact contact)
        {
            purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();
            purchase_invoice_detail.purchase_invoice = purchase_invoice;
            Cognitivo.Purchase.InvoiceSetting InvoiceSetting = new Cognitivo.Purchase.InvoiceSetting();
            //ItemLink 
            if (item != null)
            {
                if (purchase_invoice.purchase_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() != null || !InvoiceSetting.AllowDuplicateItems)
                {
                    //Item Exists in Context, so add to sum.
                    purchase_invoice_detail _purchase_invoice_detail = purchase_invoice.purchase_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                    _purchase_invoice_detail.quantity += 1;

                    //Return because Item exists, and will +1 in Quantity
                    return;
                }
                else
                {
                    //If Item Exists in previous purchase... then get Last Cost. Problem, will get in stored value, in future we will need to add logic to convert into current currency.
                    if (dbContext.purchase_invoice_detail
                        .Where(x => x.id_item == item.id_item && x.purchase_invoice.id_contact == purchase_invoice.id_contact)
                        .OrderByDescending(y => y.purchase_invoice.trans_date)
                        .FirstOrDefault() != null)
                    {
                        purchase_invoice_detail.unit_cost = dbContext.purchase_invoice_detail
                        .Where(x => x.id_item == item.id_item && x.purchase_invoice.id_contact == purchase_invoice.id_contact)
                        .OrderByDescending(y => y.purchase_invoice.trans_date)
                        .FirstOrDefault().unit_cost;
                    }

                    //Item DOES NOT Exist in Context
                    purchase_invoice_detail.item = item;
                    purchase_invoice_detail.id_item = item.id_item;
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
            if (contact != null && contact.app_cost_center != null)
            {
                app_cost_center app_cost_center = contact.app_cost_center;
                if (app_cost_center.id_cost_center > 0)
                    purchase_invoice_detail.id_cost_center = app_cost_center.id_cost_center;
            }
            else
            {
                //If Contact does not exist, and If product exist, then take defualt Product Cost Center. Else, keep blank.
                if (item != null)
                {
                    int id_cost_center = 0;
                    if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _setting.company_ID).FirstOrDefault() != null)
                        id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _setting.company_ID).FirstOrDefault().id_cost_center);
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
            else if (dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _setting.company_ID).FirstOrDefault() != null)
            {
                purchase_invoice_detail.id_vat_group = dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _setting.company_ID).FirstOrDefault().id_vat_group;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
                //calculate_total(sender, e);
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
                        if (purchase_invoice.contact.name.ToLower().Contains(query.ToLower()))
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
                    //if (e.Parameter.GetType().BaseType == typeof(purchase_invoice_detail) || e.Parameter.GetType()== typeof(purchase_invoice_detail))
                    //{
                        purchase_invoice purchase_invoice = purchase_invoiceViewSource.View.CurrentItem as purchase_invoice;

                        //DeleteDetailGridRow
                        dgvPurchaseDetail.CancelEdit();

                        dbContext.purchase_invoice_detail.Remove(e.Parameter as purchase_invoice_detail);
                        purchase_invoicepurchase_invoice_detailViewSource.View.Refresh();
                        //calculate_total(sender, e);
                  //  }
              
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            //calculate_total(sender, e);
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
                //cntrl.Curd.recive_payment recive_payment = new cntrl.Curd.recive_payment();
                //recive_payment.purchase_invoice = purchase_invoice;
                //crud_modal.Children.Add(recive_payment);
            }
        }

        private void txtTotal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtTotal.Background = Brushes.White;
                purchase_invoice purchase_invoice = (purchase_invoice)purchase_invoiceDataGrid.SelectedItem;
                BindingExpression binding = txtTotal.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
            else
            {
                if(txtTotal.Background != Brushes.Beige)
                {
                    txtTotal.Background = Brushes.Beige;
                }
            }
        }

        private void purchase_invoice_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void btnBurnInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void navPagination_btnSearch_Click(object sender)
        {
            dbContext entity = new dbContext();

            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Day)
            {
                List<purchase_invoice> purchase_invoice = entity.db.purchase_invoice.Where(x => x.trans_date >= navPagination.start_Date).ToList();
                purchase_invoiceViewSource.Source = purchase_invoice;
            }

            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Month)
            {
                List<purchase_invoice> purchase_invoice = entity.db.purchase_invoice.Where(x => x.trans_date >= navPagination.start_Date).ToList();
                //List<purchase_invoice> purchase_invoice = entity.db.purchase_invoice.Where(x => x.trans_date.Month >= navPagination.start_Date.Month && x.trans_date.Month <= navPagination.end_Date.Month).ToList();
                purchase_invoiceViewSource.Source = purchase_invoice;
            }

            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Year)
            {
                List<purchase_invoice> purchase_invoice = entity.db.purchase_invoice.Where(x => x.trans_date >= navPagination.start_Date).ToList();
                //List<purchase_invoice> purchase_invoice = entity.db.purchase_invoice.Where(x => x.trans_date.Year >= navPagination.start_Date.Year && x.trans_date.Year <= navPagination.end_Date.Year).ToList();
                purchase_invoiceViewSource.Source = purchase_invoice;
            }
        }
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Open Purchase Order
        /// </summary>
        private void btnPurchaseOreder_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlPurchaseOrder._entity = dbContext;
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlPurchaseOrder._contact = contact;
            }
           // pnlPurchaseOrder.contactViewSource = contactViewSource;
            pnlPurchaseOrder.PurchaseOrder_Click += PurchaseOrder_Click;
            crud_modal.Children.Add(pnlPurchaseOrder);
        }

        /// <summary>
        /// Save Purchase Order
        /// </summary>
        public void PurchaseOrder_Click(object sender)
        {
            CollectionViewSource purchase_invoicepurchase_invoice_detailViewSource = FindResource("purchase_invoicepurchase_invoice_detailViewSource") as CollectionViewSource;
            purchase_invoice _purchase_invoice = (purchase_invoice)purchase_invoiceViewSource.View.CurrentItem;

            foreach (purchase_order purchase_order in pnlPurchaseOrder.selected_purchase_order)
            {
                _purchase_invoice.contact = purchase_order.contact;
                _purchase_invoice.id_contact = purchase_order.id_contact;

                _purchase_invoice.app_department = purchase_order.app_department;
                _purchase_invoice.id_department = purchase_order.id_department;

                _purchase_invoice.app_condition = purchase_order.app_condition;
                _purchase_invoice.id_condition = purchase_order.id_condition;

                _purchase_invoice.app_contract = purchase_order.app_contract;
                _purchase_invoice.id_contract = purchase_order.id_contract;

                foreach (purchase_order_detail _purchase_order_detail in purchase_order.purchase_order_detail)
                {
                    if (_purchase_invoice.purchase_invoice_detail.Where(x => x.id_item == _purchase_order_detail.id_item).Count() == 0)
                    {
                        purchase_invoice_detail purchase_invoice_detail = new purchase_invoice_detail();
                        _purchase_invoice.State = EntityState.Modified;
                        purchase_invoice_detail.purchase_invoice = _purchase_invoice;
                        purchase_invoice_detail.app_cost_center = _purchase_order_detail.app_cost_center;
                        purchase_invoice_detail.id_cost_center = _purchase_order_detail.id_cost_center;
                        purchase_invoice_detail.item = _purchase_order_detail.item;
                        purchase_invoice_detail.id_item = _purchase_order_detail.id_item;
                        purchase_invoice_detail.quantity = _purchase_order_detail.quantity - dbContext.purchase_invoice_detail
                                                                                     .Where(x => x.id_purchase_order_detail == _purchase_order_detail.id_purchase_order_detail)
                                                                                     .GroupBy(x => x.id_purchase_order_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault(); ;
                        purchase_invoice_detail.unit_cost = _purchase_order_detail.unit_cost;
                        _purchase_invoice.purchase_invoice_detail.Add(purchase_invoice_detail);
                    }

                }
            }

            dbContext.Entry(_purchase_invoice).Entity.State = EntityState.Added;
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
                entity.Brillo.Logic.Document Document = new entity.Brillo.Logic.Document();
                Document.Document_PrintPurchaseOrder(0, purchase_order);
            }
        }
    }
}
