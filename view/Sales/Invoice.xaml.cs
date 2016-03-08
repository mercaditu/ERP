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

namespace Cognitivo.Sales
{
    public partial class Invoice : INotifyPropertyChanged
    {
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        int company_ID;
        int branch_ID;

        //Global Variables
        CollectionViewSource sales_invoiceViewSource;
        CollectionViewSource sales_invoicesales_invoice_detailViewSource, 
            sales_invoicesales_invoice_detailsales_packinglist_relationViewSource;
        SalesInvoiceDB SalesInvoiceDB = new SalesInvoiceDB();
        
        cntrl.PanelAdv.pnlPacking pnlPacking;
        cntrl.PanelAdv.pnlSalesOrder pnlSalesOrder;

        public Invoice()
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

        void filter_sales()
        {
            if (sales_invoiceViewSource != null)
            {
                if (sales_invoiceViewSource.View != null)
                {
                    if (sales_invoiceViewSource.View.Cast<sales_invoice>().Count() > 0)
                    {
                        sales_invoiceViewSource.View.Filter = i =>
                        {
                            sales_invoice sales_invoice = (sales_invoice)i;
                            if (sales_invoice.is_head == true)
                                return true;
                            else
                                return false;
                        };
                    }
                }
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
            InvoiceSetting InvoiceSetting = new InvoiceSetting();
            if (InvoiceSetting.filter_Branch)
            {
                await SalesInvoiceDB.sales_invoice.Where(a => a.id_company == company_ID && a.id_branch == branch_ID
                                               && (a.is_head == true)).ToListAsync();
                
            }
            else
            {
                await SalesInvoiceDB.sales_invoice.Where(a => a.id_company == company_ID 
                                              && (a.is_head == true)).ToListAsync();
            }

            
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
                sales_invoiceViewSource.Source = SalesInvoiceDB.sales_invoice.Local;
            }));
        }

        private async void load_SecondaryDataThread()
        {
            SalesInvoiceDB.app_contract.Where(a => a.is_active == true && a.id_company == company_ID ).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxContract.ItemsSource = SalesInvoiceDB.app_contract.Local;
            }));

            SalesInvoiceDB.app_condition.Where(a => a.is_active == true && a.id_company == company_ID ).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxCondition.ItemsSource = SalesInvoiceDB.app_condition.Local;
            }));

          
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesInvoice,  CurrentSession.Id_Branch, _setting.terminal_ID);
          

            SalesInvoiceDB.sales_rep.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxSalesRep.ItemsSource = SalesInvoiceDB.sales_rep.Local;
            }));

            SalesInvoiceDB.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == company_ID).OrderBy(b => b.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxBranch.ItemsSource = SalesInvoiceDB.app_branch.Local;
            }));

            SalesInvoiceDB.app_terminal.Where(b => b.is_active == true && b.id_company == company_ID).OrderBy(b => b.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxTerminal.ItemsSource = SalesInvoiceDB.app_terminal.Local;
            }));

            SalesInvoiceDB.app_vat_group.Where(a => a.is_active == true && a.id_company == company_ID).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = SalesInvoiceDB.app_vat_group.Local;
            }));
        }
        #endregion

        private void SalesInvoice_Loaded(object sender, RoutedEventArgs e)
        {
            toolBar.appName = entity.App.Names.SalesInvoice;
            sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = (CollectionViewSource)FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource");
            sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
          
            load_PrimaryData();

            filter_sales();
        }

        #region "Action Events"

        private void btnNew_Click(object sender)
        {
            InvoiceSetting _pref_SalesInvoice = new InvoiceSetting();

            sales_invoice sales_invoice = SalesInvoiceDB.New();
            sales_invoice.trans_date = DateTime.Now.AddDays(_pref_SalesInvoice.TransDate_OffSet);
            sales_invoice.State = EntityState.Added;
            cbxCurrency.get_DefaultCurrencyActiveRate();

            SalesInvoiceDB.Entry(sales_invoice).State = EntityState.Added;
            sales_invoiceViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_invoiceDataGrid.SelectedItem != null)
            {
                sales_invoice sales_invoice_old = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
                sales_invoice_old.IsSelected = true;
                sales_invoice_old.State = EntityState.Modified;
                SalesInvoiceDB.Entry(sales_invoice_old).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
                    sales_invoice.is_head = false;
                    sales_invoice.State = EntityState.Deleted;
                    sales_invoice.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void btnSave_Click(object sender)
        {
            SalesInvoiceDB.SaveChanges();
            sales_invoiceViewSource.View.Refresh();
            toolBar.msgSaved();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            SalesInvoiceDB.CancelAllChanges();
        }

        private void btnApprove_Click(object sender)
        {
            SalesInvoiceDB.Approve();
            filter_sales();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            SalesInvoiceDB.Anull();
        }
        #endregion

        #region Filter Data

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = SalesInvoiceDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
                sales_invoice.id_contact = contact.id_contact;
                sales_invoice.contact = contact;

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
                    cbxContactRelation.ItemsSource = SalesInvoiceDB.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

                    if (objContact.id_sales_rep != null)
                        cbxSalesRep.SelectedValue = Convert.ToInt32(objContact.id_sales_rep);
                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    //Currency
                    cbxCurrency.get_ActiveRateXContact(ref objContact);
                }));

                await SalesInvoiceDB.projects.Where(a => a.is_active == true 
                                                 && a.id_company == company_ID 
                                                 && a.id_contact == objContact.id_contact)
                                            .OrderBy(a => a.name)
                                            .ToListAsync();
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxProject.ItemsSource = SalesInvoiceDB.projects.Local;
                }));
            }
        }

        private async void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Contract
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = await SalesInvoiceDB.app_contract.Where(a => a.is_active == true
                                                                        && a.id_company == company_ID
                                                                        && a.id_condition == app_condition.id_condition).ToListAsync();
                cbxContract.SelectedIndex = 0;
            }
        }

        #endregion

        private void calculate_vat(object sender, EventArgs e)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
            sales_invoice.RaisePropertyChanged("GrandTotal");
            if (sales_invoice != null)
            {
                List<sales_invoice_detail> sales_invoice_detail = sales_invoice.sales_invoice_detail.ToList();
                if (sales_invoice_detail.Count > 0)
                {
                    dgvVAT.ItemsSource = sales_invoice_detail
                        .Join(SalesInvoiceDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
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

        private void sales_invoice_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        #region PrefSettings
        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            InvoiceSetting _pref_SalesInvoice = new InvoiceSetting();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            InvoiceSetting.Default.Save();
            _pref_SalesInvoice = InvoiceSetting.Default;
            popupCustomize.IsOpen = false;
        }
        #endregion

        private void sales_invoice_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_invoice_detail sales_invoice_detail = (sales_invoice_detail)e.NewItem;
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;

            sales_invoice_detail.sales_invoice = sales_invoice;
        }

        #region QuickLinks
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
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.condition condition = new cntrl.condition();
                    crud_modal.Children.Add(condition);
                }
            }
        }

        private void EditProduct(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                item item = SalesInvoiceDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_invoice != null)
                {
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_invoice, item));
                }
            }
        }

        private void select_Item(sales_invoice sales_invoice, item item)
        {
            InvoiceSetting InvoiceSetting = new InvoiceSetting();
            if (sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || InvoiceSetting.duplicate_Items)
            {
                sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail();
                _sales_invoice_detail.sales_invoice = sales_invoice;
                _sales_invoice_detail.item_description = item.description;
                _sales_invoice_detail.item = item;
                _sales_invoice_detail.id_item = item.id_item;

                sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);
            }
            else
            {
                sales_invoice_detail sales_invoice_detail = sales_invoice.sales_invoice_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_invoice_detail.quantity += 1;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
               
                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && sales_invoiceViewSource != null)
            {
                try
                {
                    sales_invoiceViewSource.View.Filter = i =>
                    {
                        sales_invoice sales_invoice = i as sales_invoice;
                        if (sales_invoice.contact.name.ToLower().Contains(query.ToLower())
                            || sales_invoice.number.ToLower().Contains(query.ToLower())
                            || sales_invoice.trans_date.ToString() == query)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    toolBar.msgError(ex);
                }
            }
            else
            {
                sales_invoiceViewSource.View.Filter = null;
            }
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_invoice_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {

                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                    //DeleteDetailGridRow
                    dgvSalesDetail.CancelEdit();
                    SalesInvoiceDB.sales_invoice_detail.Remove(e.Parameter as sales_invoice_detail);
                    sales_invoicesales_invoice_detailViewSource.View.Refresh();
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

        private void btnDuplicateInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (db db = new db())
            {
                sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                if (sales_invoice != null)
                {
                    if (sales_invoice.id_sales_invoice != 0)
                    {
                        var originalEntity = db.sales_invoice.AsNoTracking()
                                     .FirstOrDefault(x => x.id_sales_invoice == sales_invoice.id_sales_invoice);
                        db.sales_invoice.Add(originalEntity);
                        sales_invoiceViewSource.View.Refresh();
                        sales_invoiceViewSource.View.MoveCurrentToLast();
                    }
                    else
                    {
                        toolBar.msgWarning("Please save before duplicating");
                    }
                }
            }
        }

        private void sales_invoice_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
          
           
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void btnPackingList_Click(object sender, RoutedEventArgs e)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;

            if (sales_invoice != null)
            {
                crud_modal.Visibility = Visibility.Visible;

                pnlPacking = new cntrl.PanelAdv.pnlPacking();
                pnlPacking._entity = SalesInvoiceDB;
                pnlPacking._contact = sbxContact.Contact as contact;
                pnlPacking.Link_Click += Link_Click;
                crud_modal.Children.Add(pnlPacking);
                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
            }
            else
            {
                toolBar.msgWarning("Check Sales Invoice");
            }
        }

        public void Link_Click(object sender)
        {
            sales_invoice _sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem;
            foreach (sales_packing item in pnlPacking.selected_sales_packing)
            {
                sales_packing sales_packing = SalesInvoiceDB.sales_packing.Where(x => x.id_sales_packing == item.id_sales_packing).FirstOrDefault(); 
              //  _sales_invoice.State = EntityState.Modified;

                foreach (sales_packing_detail _sales_packing_detail in sales_packing.sales_packing_detail)
                {
                   
                    if (_sales_invoice.sales_invoice_detail.Where(x => x.id_item == _sales_packing_detail.id_item).Count() == 0)
                    {
                        sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();
                        
                        sales_invoice_detail.sales_invoice = _sales_invoice;

                        sales_invoice_detail.item = _sales_packing_detail.item;
                        sales_invoice_detail.id_item = _sales_packing_detail.id_item;
                        sales_invoice_detail.quantity = _sales_packing_detail.quantity;
                        sales_invoice_detail.unit_price = 0;

                        sales_packing_relation sales_packing_relation = new sales_packing_relation();
                        sales_packing_relation.id_sales_packing_detail = _sales_packing_detail.id_sales_packing_detail;
                        sales_packing_relation.sales_packing_detail = _sales_packing_detail;
                        //sales_packing_relation.id_sales_packing_detail = _sales_packing_detail.id_sales_packing_detail;

                        sales_invoice_detail.sales_packing_relation.Add(sales_packing_relation);
                        _sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                    }
                }

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                sales_invoicesales_invoice_detailViewSource.View.MoveCurrentToFirst();
                CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource") as CollectionViewSource;
                if (sales_invoicesales_invoice_detailViewSource.View.CurrentItem!=null)
                {
                    sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = (sales_invoicesales_invoice_detailViewSource.View.CurrentItem as sales_invoice_detail).sales_packing_relation;      
                }
                else
                {
                    sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = null;

                }
               // SalesInvoiceDB.Entry(_sales_invoice).Entity.State = EntityState.Added;
                //sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.View.Refresh();
                crud_modal.Children.Clear();
                crud_modal.Visibility = Visibility.Collapsed;
            }
        }


        private void btnSalesOrder_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlSalesOrder = new cntrl.PanelAdv.pnlSalesOrder();
            pnlSalesOrder._entity = SalesInvoiceDB;
            pnlSalesOrder.mode = cntrl.PanelAdv.pnlSalesOrder.module.sales_invoice;
            pnlSalesOrder.SalesOrder_Click += SalesOrder_Click;
            crud_modal.Children.Add(pnlSalesOrder);
        }

        public void SalesOrder_Click(object sender)
        {
            sales_invoice _sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem;

            sbxContact.Text = pnlSalesOrder.selected_sales_order.FirstOrDefault().contact.name;
            foreach (sales_order item in pnlSalesOrder.selected_sales_order)
            {
                _sales_invoice.State = EntityState.Modified;
                _sales_invoice.id_condition = item.id_condition;
                _sales_invoice.id_contract = item.id_contract;
                _sales_invoice.id_currencyfx = item.id_currencyfx;
                _sales_invoice.id_sales_order = item.id_sales_order;
                foreach (sales_order_detail _sales_order_detail in item.sales_order_detail)
                {
                    if (_sales_invoice.sales_invoice_detail.Where(x => x.id_item == _sales_order_detail.id_item).Count() == 0)
                    {
                        sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();
                        sales_invoice_detail.id_sales_order_detail = _sales_order_detail.id_sales_order_detail;
                        sales_invoice_detail.sales_order_detail = _sales_order_detail;
                        sales_invoice_detail.sales_invoice = _sales_invoice;
                        sales_invoice_detail.item = _sales_order_detail.item;
                        sales_invoice_detail.id_item = _sales_order_detail.id_item;
                        sales_invoice_detail.quantity = _sales_order_detail.quantity - SalesInvoiceDB.sales_invoice_detail
                                                                                     .Where(x => x.id_sales_order_detail == _sales_order_detail.id_sales_order_detail)
                                                                                     .GroupBy(x => x.id_sales_order_detail)
                                                                                     .Select(x => x.Sum(y => y.quantity))
                                                                                     .FirstOrDefault();
                        sales_invoice_detail.unit_price = _sales_order_detail.unit_price;
                        _sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                    }
                }
                SalesInvoiceDB.Entry(_sales_invoice).Entity.State = EntityState.Added;
                crud_modal.Children.Clear();
                crud_modal.Visibility = Visibility.Collapsed;
                sales_invoiceViewSource.View.Refresh();
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
            }
        }

        private void navPagination_btnSearch_Click(object sender)
        {
            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Day)
            {
                List<sales_invoice> sales_invoiceList = SalesInvoiceDB.sales_invoice.Local.Where(x => x.id_company == company_ID && (x.trans_date >= navPagination.start_Date)).ToList();
                sales_invoiceViewSource.Source = sales_invoiceList;
            }
            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Month)
            {
                List<sales_invoice> sales_invoice = SalesInvoiceDB.sales_invoice.Local.Where(x => x.id_company == company_ID && (x.trans_date >= navPagination.start_Date)).ToList();
                sales_invoiceViewSource.Source = sales_invoice;
            }
            if (navPagination.DisplayMode == cntrl.navPagination.DisplayModes.Year)
            {
                List<sales_invoice> sales_invoice = SalesInvoiceDB.sales_invoice.Local.Where(x => x.id_company == company_ID && (x.trans_date >= navPagination.start_Date)).ToList();
                sales_invoiceViewSource.Source = sales_invoice;
            }
        }



        private void salesorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            sales_order sales_order = (sales_order)Hyperlink.Tag;
            if (sales_order != null)
            {
                entity.Brillo.Document.Start.Automatic(sales_order, sales_order.app_document_range);
            }
        }

        private void salespackinglist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            sales_packing sales_packing = (sales_packing)Hyperlink.Tag;
            if (sales_packing != null)
            {
                entity.Brillo.Document.Start.Automatic(sales_packing, sales_packing.app_document_range);
            }
           
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceDataGrid.SelectedItem as sales_invoice;
            if (sales_invoice != null)
            {
                //  sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                entity.Brillo.Document.Start.Manual(sales_invoice, sales_invoice.app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        private void sales_invoiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
            CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource") as CollectionViewSource;
            if (sales_invoicesales_invoice_detailViewSource.View != null)
            {
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                sales_invoicesales_invoice_detailViewSource.View.MoveCurrentToFirst();

                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = (sales_invoicesales_invoice_detailViewSource.View.CurrentItem as sales_invoice_detail).sales_packing_relation;

            }
            else
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = null;
            }
          
        }

         private void btnRecivePayment_PreviewMouseUp(object sender, MouseButtonEventArgs e)
         {
             sales_invoice sales_invoice = sales_invoiceDataGrid.SelectedItem as sales_invoice;
             if (sales_invoice != null)
             {
                 crud_modal.Visibility = System.Windows.Visibility.Visible;
                 cntrl.Curd.receive_payment recive_payment = new cntrl.Curd.receive_payment();
                 recive_payment.sales_invoice = sales_invoice;
                 crud_modal.Children.Add(recive_payment);
             }
         }

        
       

      
    }
}
