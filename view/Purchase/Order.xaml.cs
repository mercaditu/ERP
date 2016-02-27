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

namespace Cognitivo.Purchase
{
    public partial class Order : Page, INotifyPropertyChanged
    {
        CollectionViewSource purchase_orderViewSource;
        CollectionViewSource purchase_orderpurchase_order_detailViewSource;
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        PurchaseOrderDB dbContext = new PurchaseOrderDB();
        contact _contact = new contact();
        ContactDB ContactDB = new ContactDB();

        public Order()
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
            await dbContext.purchase_order.Where(a => a.id_company == _setting.company_ID
                                           ).Include(x => x.purchase_order_detail).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                purchase_orderViewSource = ((CollectionViewSource)(FindResource("purchase_orderViewSource")));
                purchase_orderViewSource.Source = dbContext.purchase_order.Local;
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

            dbContext.projects.Where(a => a.is_active == true && a.id_company == _setting.company_ID).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxProject.ItemsSource = dbContext.projects.Local;
            }));

           
            //dbContext.app_document_range.Where(d => d.is_active == true
            //                               && d.app_document.id_application == entity.App.Names.PurchaseOrder && d.id_company == _setting.company_ID).Include(i => i.app_document).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cmbdocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.PurchaseOrder,_setting.branch_ID,_setting.terminal_ID);
            }));

            //cmbdocument
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
            dbContext.app_document_range.Where(d => d.is_active == true
                                           && d.app_document.id_application == entity.App.Names.PurchaseOrder
                                           && d.id_company == _setting.company_ID).Include("app_document").ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cmbdocument.ItemsSource = dbContext.app_document_range.Local;
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

            purchase_order purchase_order = dbContext.New();
            purchase_order.trans_date = DateTime.Now.AddDays(_pref_PurchaseOrder.TransDate_OffSet);
    
            dbContext.Entry(purchase_order).State = EntityState.Added;
            purchase_orderViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (purchase_orderDataGrid.SelectedItem != null)
            {
                purchase_order purchase_order_old = (purchase_order)purchase_orderDataGrid.SelectedItem;
                purchase_order_old.IsSelected = true;
                purchase_order_old.State = EntityState.Modified;
                dbContext.Entry(purchase_order_old).State = EntityState.Modified;
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
            dbContext.SaveChangesAsync();
            purchase_orderViewSource.View.Refresh();
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

        #region Filter Data


        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
                purchase_order.id_contact = contact.id_contact;
                purchase_order.contact = contact;

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
            purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
            purchase_order.RaisePropertyChanged("GrandTotal");
            if (purchase_order != null)
            {
                List<purchase_order_detail> purchase_order_detail = purchase_order.purchase_order_detail.ToList();
                dgvVAT.ItemsSource = purchase_order_detail
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
            OrderSetting _pref_PurchaseOrder = new OrderSetting();
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            OrderSetting.Default.Save();
            _pref_PurchaseOrder = OrderSetting.Default;
            popupCustomize.IsOpen = false;
        }

        #region Add/Edit Config
        private void hrefAddCust_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            sbxContact.Contact.State = EntityState.Added;
            sbxContact.Contact.is_customer = true;
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.contact contact = new cntrl.Curd.contact();
            contact.btnSave_Click += ContactSave_Click;
            contact.contactobject = sbxContact.Contact;
            crud_modal.Children.Add(contact);

        }

        public void ContactSave_Click(object sender)
        {
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
            load_PrimaryData();
        }

        private void EditContact_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //dbContext entity = new dbContext();
            //contact selectedcontact = (contact)contactComboBox.Data;
            //_contact = ContactDB.contacts.Where(x => x.id_contact == selectedcontact.id_contact).FirstOrDefault();
            //_contact.State = EntityState.Modified;
            //ContactDB.contacts.Add(_contact);
            //if (_contact != null)
            //{
            //    contactComboBox.Text = "";
            //    contactComboBox.Data = null;
            //    crud_modal.Visibility = Visibility.Visible;
            //    contactComboBox.IsDisplayed = false;
            //    cntrl.Curd.contact contact = new cntrl.Curd.contact();
            //    contact.contactobject = _contact;
            //    contact.btnSave_Click += ContactSave_Click;
            //    crud_modal.Children.Add(contact);
            //}
            //else
            //{
            //    MessageBox.Show("Please select contact first.", "Cognitivo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            //}
        }
 


        private void CreateNewCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
                //condition.MainViewSource = purchase_orderViewSource;
                //condition.curObject = purchase_orderViewSource.View.CurrentItem;
                //condition.entity = entity;
                //condition.operationMode = cntrl.Class.clsCommon.Mode.Add;
                //condition.isExternalCall = true;
                crud_modal.Children.Add(condition);
            }
        }

        private void EditCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
                    crud_modal.Children.Add(condition);
                }
            }
        }

        private void CreateNewContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
                contract.MainViewSource = purchase_orderViewSource;
                contract.curObject = purchase_orderViewSource.View.CurrentItem;
                contract.entity = entity;
                contract.operationMode = cntrl.Class.clsCommon.Mode.Add;
                contract.isExternalCall = true;
                crud_modal.Children.Add(contract);
            }
        }

        private void EditContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
                    contract.MainViewSource = purchase_orderViewSource;
                    contract.curObject = purchase_orderViewSource.View.CurrentItem;
                    contract.entity = entity;
                    contract.app_contractobject = app_contract;
                    contract.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                    contract.isExternalCall = true;
                    crud_modal.Children.Add(contract);
                }
            }
        }
        #endregion

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

                item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                if (sbxContact.ContactID > 0)
                {
                    contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                }
            }

            Task Thread = Task.Factory.StartNew(() => SelectProduct_Thread(sender, e, purchase_order, item, contact));
        }

        private void SelectProduct_Thread(object sender, EventArgs e, purchase_order purchase_order, item item, contact contact)
        {
            purchase_order_detail purchase_order_detail = new purchase_order_detail();
            purchase_order_detail.purchase_order = purchase_order;

            //ItemLink 
            if (item != null)
            {
                if (purchase_order.purchase_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() != null)
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
                    if (dbContext.purchase_invoice_detail
                        .Where(x => x.id_item == item.id_item && x.purchase_invoice.id_contact == purchase_order.id_contact)
                        .OrderByDescending(y => y.purchase_invoice.trans_date)
                        .FirstOrDefault() != null)
                    {
                        purchase_order_detail.unit_cost = dbContext.purchase_invoice_detail
                        .Where(x => x.id_item == item.id_item && x.purchase_invoice.id_contact == purchase_order.id_contact)
                        .OrderByDescending(y => y.purchase_invoice.trans_date)
                        .FirstOrDefault().unit_cost;
                    }

                    //Item DOES NOT Exist in Context
                    purchase_order_detail.item = item;
                    purchase_order_detail.id_item = item.id_item;
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
                //If Contact does not exist, and If product exist, then take defualt Product Cost Center. Else, keep blank.
                if (item != null)
                {
                    int id_cost_center = 0;
                    if (dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _setting.company_ID).FirstOrDefault() != null)
                        id_cost_center = Convert.ToInt32(dbContext.app_cost_center.Where(a => a.is_product == true && a.is_active == true && a.id_company == _setting.company_ID).FirstOrDefault().id_cost_center);
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
            else if (dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _setting.company_ID).FirstOrDefault() != null)
            {
                purchase_order_detail.id_vat_group = dbContext.app_vat_group.Where(x => x.is_active == true && x.is_default == true && x.id_company == _setting.company_ID).FirstOrDefault().id_vat_group;
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
            //calculate_total(sender, e);
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

        private void txtTotal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                purchase_order purchase_order = (purchase_order)purchase_orderDataGrid.SelectedItem;
                BindingExpression binding = txtTotal.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
                calculate_vat(sender, e);
            }
        }

        private void purchase_order_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void lblEditProduct_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
                 
            //        crud_modal.Visibility = Visibility.Visible;
                  
            //        cntrl.Curd.item item = new cntrl.Curd.item();
            //        item.itemViewSource = itemViewSource;
            //        item.MainViewSource = purchase_orderViewSource;
            //        item.curObject = purchase_orderViewSource.View.CurrentItem;
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

        private void cbxCurrency_MouseEnter(object sender, MouseEventArgs e)
        {

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
    }
}
