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
using System.Data.Entity.Validation;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Documents;


namespace Cognitivo.Sales
{
    public partial class Order : Page, INotifyPropertyChanged
    {
        //public string orderNumber { get; set; }

        CollectionViewSource sales_orderViewSource;
        SalesOrderDB dbContext = new SalesOrderDB();
        contact _contact = new contact();
        ContactDB ContactdbContext = new ContactDB();
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        int company_ID;
        int branch_ID;
        //cntrl.PanelAdv.pnlSalesBudget pnlSalesBudget = new cntrl.PanelAdv.pnlSalesBudget();

        public Order()
        {
            InitializeComponent();
            company_ID = _setting.company_ID;
            branch_ID = _setting.branch_ID;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region DataLoad
        private void load_PrimaryData()
        {
            //Task task_PrimaryData = Task.Factory.StartNew(() => load_PrimaryDataThread());
            //Task thread_SecondaryData = task_PrimaryData.ContinueWith(antTask => load_SecondaryDataThread());

            load_PrimaryDataThread();
            load_SecondaryDataThread();
        }

        private async void load_PrimaryDataThread()
        {
            OrderSetting OrderSetting = new OrderSetting();
            if (OrderSetting.filterbyBranch)
            {
                await dbContext.sales_order.Where(a => a.id_company == company_ID && a.id_branch == branch_ID
                                            && (
                    //a.trans_date >= navPagination.start_Date
                    // && a.trans_date <= navPagination.end_Date 
                    // && 
                                             a.is_head == true)).Include("sales_order_detail").ToListAsync();
            }
            else
            {
                await dbContext.sales_order.Where(a => a.id_company == company_ID 
                                            && (
                    //a.trans_date >= navPagination.start_Date
                    // && a.trans_date <= navPagination.end_Date 
                    // && 
                                             a.is_head == true)).Include("sales_order_detail").ToListAsync();
            }
            
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                sales_orderViewSource = ((CollectionViewSource)(FindResource("sales_orderViewSource")));
                sales_orderViewSource.Source = dbContext.sales_order.Local;
            }));
        }

        private async void load_SecondaryDataThread()
        {
            //dbContext.sales_budget.Where(x => x.status == Status.Documents_General.Approved && x.id_company == company_ID).ToList();
            //await Dispatcher.InvokeAsync(new Action(() =>
            //{
            //    cbxContract.ItemsSource = dbContext.sales_budget.Local;
            //}));

            dbContext.projects.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxProject.ItemsSource = dbContext.projects.Local;
            }));

            dbContext.app_contract.Where(a => a.is_active == true && a.id_company == company_ID).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxContract.ItemsSource = dbContext.app_contract.Local;
            }));

            dbContext.app_condition.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxCondition.ItemsSource = dbContext.app_condition.Local;
            }));


            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesOrder, _setting.branch_ID, _setting.terminal_ID);
            }));

            dbContext.app_branch.Where(b => b.is_active == true && b.id_company == company_ID).OrderBy(b => b.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxBranch.ItemsSource = dbContext.app_branch.Local;
            }));

            dbContext.app_vat_group.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = dbContext.app_vat_group.Local;
            }));




        }

        #endregion
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                load_PrimaryData();
            }
            catch (Exception ex) { toolBar.msgError(ex); }
        }

        #region toolbar Events
        private void New_Click(object sender)
        {
            OrderSetting _pref_SalesOrder = new OrderSetting();

            sales_order sales_order = dbContext.New();
            sales_order.trans_date = DateTime.Now.AddDays(_pref_SalesOrder.TransDate_OffSet);

            cbxCurrency.get_DefaultCurrencyActiveRate();

            dbContext.sales_order.Add(sales_order);

            sales_orderViewSource.View.MoveCurrentTo(sales_order);
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_orderDataGrid.SelectedItem != null)
            {
                sales_order sales_order_old = (sales_order)sales_orderDataGrid.SelectedItem;
                sales_order_old.IsSelected = true;
                sales_order_old.State = EntityState.Modified;
                dbContext.Entry(sales_order_old).State = EntityState.Modified;
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
                    sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
                    sales_order.is_head = false;
                    sales_order.State = EntityState.Deleted;
                    sales_order.IsSelected = true;
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
                dbContext.SaveChanges();
                sales_orderViewSource.View.Refresh();
                toolBar.msgSaved();
            }
            catch (DbEntityValidationException ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void btnApprove_Click(object sender)
        {
            dbContext.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            dbContext.Annull();
        }
        #endregion

        #region Filter Data

     

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
                sales_order.id_contact = contact.id_contact;
                sales_order.contact = contact;
                Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxContactRelation.ItemsSource = dbContext.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    //Currency
                    app_currencyfx app_currencyfx = null;
                    if (objContact.app_currency != null && objContact.app_currency.app_currencyfx.Any(a => a.is_active) && objContact.app_currency.app_currencyfx.Count > 0)
                        app_currencyfx = objContact.app_currency.app_currencyfx.Where(a => a.is_active == true).First();
                    if (app_currencyfx != null)
                        cbxCurrency.SelectedValue = Convert.ToInt32(app_currencyfx.id_currencyfx);
                }));

                await dbContext.projects.Where(a => a.is_active == true && a.id_company == company_ID && a.id_contact == objContact.id_contact).OrderBy(a => a.name).ToListAsync();
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
                                                                        && a.id_company == company_ID
                                                                        && a.id_condition == app_condition.id_condition).ToListAsync();
                cbxContract.SelectedIndex = 0;
            }
        }

        #endregion

        //private void calculate_total(object sender, EventArgs e)
        //{
        //    sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
        //    if (sales_order != null)
        //    {
        //        sales_order.get_Sales_order_Total();
        //    }
        //}

        private void calculate_vat(object sender, EventArgs e)
        {
            sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
            sales_order.RaisePropertyChanged("GrandTotal");
            if (sales_order != null)
            {
                List<sales_order_detail> sales_order_detail = sales_order.sales_order_detail.ToList();
                if (sales_order_detail.Count > 0)
                {
                    dgvvat.ItemsSource = sales_order_detail
                        .Join(dbContext.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                            , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
                            .GroupBy(a => new { a.name, a.id_vat, a.ad })
                    .Select(g => new
                    {
                        id_vat = g.Key.id_vat,
                        name = g.Key.name,
                        value = g.Sum(a => a.value * a.ad.quantity)
                    }).ToList();
                }
            }
        }

        private void sales_order_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        //private void sales_orderDataGrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    calculate_total(sender, e);
        //}

        private void sales_orderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
                if (sales_order != null)
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

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            OrderSetting _pref_SalesOrder = new OrderSetting();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            OrderSetting.Default.Save();
            _pref_SalesOrder = OrderSetting.Default;
            popupCustomize.IsOpen = false;
        }

        private void sales_order_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_order_detail sales_order_detail = (sales_order_detail)e.NewItem;
            sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;

            sales_order_detail.sales_order = sales_order;
        }

        #region ExpressAdd/Edit
        private void hrefAddCust_PreviewMouseUp(object sender, RoutedEventArgs e)
        {
            sbxContact.Contact.State = EntityState.Added;
            sbxContact.Contact.is_customer = true;
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.contact contact = new cntrl.Curd.contact();
           // contact.btnSave_Click += ContactSave_Click;
            contact.contactobject = sbxContact.Contact;
            crud_modal.Children.Add(contact);

        }

        private void hrefEditCust_PreviewMouseUp(object sender, RoutedEventArgs e)
        {
            ////dbContext entity = new dbContext();
            //contact selectedcontact = (contact)contactComboBox.Data;
            //_contact = ContactdbContext.contacts.Where(x => x.id_contact == selectedcontact.id_contact).FirstOrDefault();
            //_contact.State = EntityState.Modified;
            //ContactdbContext.contacts.Add(_contact);
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
        public void ContactSave_Click(object sender)
        {
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;

        }
        private void hrefAddCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource conditionViewSource = (CollectionViewSource)FindResource("conditionViewSource");
                db.app_condition.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).Load();
                conditionViewSource.Source = db.app_condition.Local;
                //dbContext entity = new dbContext();
                crud_modal.Visibility = Visibility.Visible;
                cntrl.condition condition = new cntrl.condition();
                crud_modal.Children.Add(condition);
            }
        }

        private void hrefEditCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource conditionViewSource = (CollectionViewSource)FindResource("conditionViewSource");
                db.app_condition.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).Load();
                conditionViewSource.Source = db.app_condition.Local;
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                if (app_condition != null)
                {
                    //dbContext entity = new dbContext();
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.condition condition = new cntrl.condition();
                    crud_modal.Children.Add(condition);
                }
            }
        }
        private void hrefAddContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource contractViewSource = (CollectionViewSource)FindResource("contractViewSource");
                db.app_contract.Where(a => a.is_active == true && a.id_company == company_ID).Load();
                contractViewSource.Source = db.app_contract.Local;

                dbContext entity = new dbContext();
                crud_modal.Visibility = Visibility.Visible;
                cntrl.contract contract = new cntrl.contract();
                contract.app_contractViewSource = contractViewSource;
                contract.MainViewSource = sales_orderViewSource;
                contract.curObject = sales_orderViewSource.View.CurrentItem;
                contract.entity = entity;
                contract.operationMode = cntrl.Class.clsCommon.Mode.Add;
                contract.isExternalCall = true;
                crud_modal.Children.Add(contract);
            }
        }
        private void hrefEditContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                CollectionViewSource contractViewSource = (CollectionViewSource)FindResource("contractViewSource");
                db.app_contract.Where(a => a.is_active == true && a.id_company == company_ID).Load();
                contractViewSource.Source = db.app_contract.Local;

                app_contract app_contract = cbxContract.SelectedItem as app_contract;
                if (app_contract != null)
                {
                    dbContext entity = new dbContext();
                    crud_modal.Visibility = System.Windows.Visibility.Visible;
                    cntrl.contract contract = new cntrl.contract();
                    contract.app_contractViewSource = contractViewSource;
                    contract.MainViewSource = sales_orderViewSource;
                    contract.curObject = sales_orderViewSource.View.CurrentItem;
                    contract.entity = entity;
                    contract.app_contractobject = app_contract;
                    contract.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                    contract.isExternalCall = true;
                    crud_modal.Children.Add(contract);
                }
            }
        }


        #endregion

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                item item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_order != null)
                {
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_order, item));
                }
            }
        }

        private void select_Item(sales_order sales_order, item item)
        {
            OrderSetting OrderSetting = new OrderSetting();
            if (sales_order.sales_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || OrderSetting.AllowDuplicateItems)
            {
                sales_order_detail _sales_order_detail = new sales_order_detail();
                _sales_order_detail.sales_order = sales_order;
                _sales_order_detail.Contact = sales_order.contact;
                _sales_order_detail.item_description = item.description;
                _sales_order_detail.item = item;
                _sales_order_detail.id_item = item.id_item;
            
                sales_order.sales_order_detail.Add(_sales_order_detail);
            }
            else
            {
                sales_order_detail sales_order_detail = sales_order.sales_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_order_detail.quantity += 1;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                CollectionViewSource sales_ordersales_order_detailViewSource = FindResource("sales_ordersales_order_detailViewSource") as CollectionViewSource;
                sales_ordersales_order_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && sales_orderViewSource != null)
            {
                try
                {
                    sales_orderViewSource.View.Filter = i =>
                    {
                        sales_order sales_order = i as sales_order;
                        if (sales_order.contact.name.ToLower().Contains(query.ToLower())
                            || sales_order.number.ToLower().Contains(query.ToLower())
                            || sales_order.trans_date == Convert.ToDateTime(query) || sales_order.sales_budget.number.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch { }
            }
            else
            {
                sales_orderViewSource.View.Filter = null;
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_order_detail != null)
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
                    sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                    //DeleteDetailGridRow
                    dgvSalesDetail.CancelEdit();
                    sales_order_detail sales_order_detail = e.Parameter as sales_order_detail;
                    dbContext.sales_order_detail.Remove(sales_order_detail);
                    CollectionViewSource sales_ordersales_order_detailViewSource = FindResource("sales_ordersales_order_detailViewSource") as CollectionViewSource;
                    sales_ordersales_order_detailViewSource.View.Refresh();
                    //calculate_total(sender, e);

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



        private void sales_order_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        //public string number { get; set; }

        //private void cbxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cbxDocument.SelectedValue != null)
        //    {
        //        entity.Brillo.Logic.Document _Document = new entity.Brillo.Logic.Document();
        //        app_document_range app_document_range = (app_document_range)cbxDocument.SelectedItem;
        //        sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;

        //        entity.Brillo.Range.branch_Code = dbContext.app_branch.Where(x => x.id_branch == sales_order.id_branch).FirstOrDefault().code;
        //        entity.Brillo.Range.terminal_Code = dbContext.app_terminal.Where(x => x.id_terminal == sales_order.id_terminal).FirstOrDefault().code;
        //        orderNumber = entity.Brillo.Range.calc_Range(app_document_range, false);
        //        RaisePropertyChanged("orderNumber");
        //    }
        //}

      

        private void btnDuplicateInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                if (sales_order != null)
                {
                    if (sales_order.id_sales_order != 0)
                    {
                        var originalEntity = db.sales_invoice.AsNoTracking()
                                     .FirstOrDefault(x => x.id_sales_invoice == sales_order.id_sales_order);
                        db.sales_invoice.Add(originalEntity);
                        sales_orderViewSource.View.Refresh();
                        sales_orderViewSource.View.MoveCurrentToLast();
                    }
                    else
                    {
                        toolBar.msgWarning("Please save before duplicating");
                    }
                }
            }
        }

        private void navPagination_btnSearch_Click(object sender)
        {
            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Day)
            {
                List<sales_invoice> sales_invoiceList = dbContext.sales_invoice.Local.Where(x => x.id_company == company_ID && (x.trans_date >= navPagination.start_Date)).ToList();
                sales_orderViewSource.Source = sales_invoiceList;
            }
            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Month)
            {
                List<sales_invoice> sales_invoice = dbContext.sales_invoice.Local.Where(x => x.id_company == company_ID && (x.trans_date >= navPagination.start_Date)).ToList();
                sales_orderViewSource.Source = sales_invoice;
            }
            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Year)
            {
                List<sales_invoice> sales_invoice = dbContext.sales_invoice.Local.Where(x => x.id_company == company_ID && (x.trans_date >= navPagination.start_Date)).ToList();
                sales_orderViewSource.Source = sales_invoice;
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            sales_order sales_order = sales_orderDataGrid.SelectedItem as sales_order;
            if (sales_order != null)
            {
                entity.Brillo.Document.Start.Manual(sales_order, sales_order.app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        cntrl.PanelAdv.pnlSalesBudget pnlSalesBudget = new cntrl.PanelAdv.pnlSalesBudget();

        private void btnSalesBudget_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlSalesBudget._contact = contact;
            }
            pnlSalesBudget.SalesBudget_Click += SalesBudget_Click;

            sales_order _sales_order = (sales_order)sales_orderViewSource.View.CurrentItem;
            pnlSalesBudget.sales_order = _sales_order;

            crud_modal.Children.Add(pnlSalesBudget);
        }

        public async void SalesBudget_Click(object sender)
        {
            sales_order sales_order = (sales_order)sales_orderViewSource.View.CurrentItem;
            foreach (sales_order_detail detail in sales_order.sales_order_detail)
            {
                detail.item = await dbContext.items.Where(x => x.id_item == detail.id_item).FirstOrDefaultAsync();
            }

            CollectionViewSource sales_ordersales_order_detailViewSource = ((CollectionViewSource)(FindResource("sales_ordersales_order_detailViewSource")));
            sales_orderViewSource.View.Refresh();
            sales_ordersales_order_detailViewSource.View.Refresh();
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
        }

        private void SalesBudget_DocViewer_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            sales_budget sales_budget = (sales_budget)Hyperlink.Tag;
            if (sales_budget != null)
            {
                entity.Brillo.Document.Start.Automatic(sales_budget, sales_budget.app_document_range);
            }
        }
    }
}
