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
    public partial class Order : Page
    {
        CollectionViewSource sales_orderViewSource;
        SalesOrderDB SalesOrderDB = new SalesOrderDB();
        cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry;
        public Order()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region DataLoad
        private void load_PrimaryData()
        {
            load_PrimaryDataThread();
            load_SecondaryDataThread();
        }

        private async void load_PrimaryDataThread()
        {
            Settings SalesSettings = new Settings();
            if (SalesSettings.FilterByBranch)
            {
                await SalesOrderDB.sales_order.Where(a => a.id_company == CurrentSession.Id_Company && a.id_branch == CurrentSession.Id_Branch
                                            && (
                                             //a.trans_date >= navPagination.start_Date
                                             // && a.trans_date <= navPagination.end_Date 
                                             // && 
                                             a.is_head == true)).Include(x => x.contact).OrderByDescending(x => x.trans_date).ToListAsync();
            }
            else
            {
                await SalesOrderDB.sales_order.Where(a => a.id_company == CurrentSession.Id_Company
                                            && (
                                             //a.trans_date >= navPagination.start_Date
                                             // && a.trans_date <= navPagination.end_Date 
                                             // && 
                                             a.is_head == true)).Include(x => x.contact).OrderByDescending(x => x.trans_date).ToListAsync();
            }

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                sales_orderViewSource = ((CollectionViewSource)(FindResource("sales_orderViewSource")));
                sales_orderViewSource.Source = SalesOrderDB.sales_order.Local;
            }));

        }

        private async void load_SecondaryDataThread()
        {
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesOrderDB, entity.App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
            }));
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load_PrimaryData();
        }

        #region toolbar Events

        private void New_Click(object sender)
        {
            Settings SalesSettings = new Settings();

            sales_order sales_order = SalesOrderDB.New();
            sales_order.trans_date = DateTime.Now.AddDays(SalesSettings.TransDate_Offset);

            cbxCurrency.get_DefaultCurrencyActiveRate();
            SalesOrderDB.sales_order.Add(sales_order);
            sales_orderViewSource.View.MoveCurrentTo(sales_order);
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_orderDataGrid.SelectedItem != null)
            {
                sales_order sales_order_old = (sales_order)sales_orderDataGrid.SelectedItem;
                sales_order_old.IsSelected = true;
                sales_order_old.State = EntityState.Modified;
                SalesOrderDB.Entry(sales_order_old).State = EntityState.Modified;
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

                if (SalesOrderDB.SaveChanges() > 0)
                {
                    toolBar.msgSaved(SalesOrderDB.NumberOfRecords);
                    sales_orderViewSource.View.Refresh();
                    sbxContact.Text = "";
                }
            }
            catch (DbEntityValidationException ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_orderViewSource.View.MoveCurrentToFirst();
            SalesOrderDB.CancelAllChanges();

            if (sales_orderViewSource.View != null)
                sales_orderViewSource.View.Refresh();
        }

        private void btnApprove_Click(object sender)
        {
            if (SalesOrderDB.Approve())
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesOrderDB, entity.App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cbxDocument.SelectedIndex = 0;
                toolBar.msgApproved(SalesOrderDB.NumberOfRecords);
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            if (SalesOrderDB.Annull())
            {
                toolBar.msgAnnulled(SalesOrderDB.NumberOfRecords);
            }
        }
        #endregion

        #region Filter Data

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = SalesOrderDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
                sales_order.id_contact = contact.id_contact;
                sales_order.contact = contact;
                contact.Check_CreditAvailability();
                Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxContactRelation.ItemsSource = SalesOrderDB.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

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

                    //SalesMan
                    if (objContact.sales_rep != null)
                        cbxSalesRep.SelectedValue = objContact.sales_rep.id_sales_rep;
                }));
            }
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
            //Contract
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                if (app_condition != null)
                {
                    cbxContract.ItemsSource = CurrentSession.Contracts.Where(x => x.id_condition == app_condition.id_condition).ToList();
                }

                if (sales_order != null)
                {
                    if (sales_order.id_contract == 0)
                    {
                        cbxContract.SelectedIndex = 0;
                    }
                }
            }
        }

        #endregion

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
                        .Join(SalesOrderDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                            , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * (cfx.app_vat.coefficient * cfx.percentage), id_vat = cfx.app_vat.id_vat, ad })
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

        private void sales_orderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
                if (sales_order != null)
                {
                    calculate_vat(sender, e);
                }
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
            Settings SalesSettings = new Settings();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Settings.Default.Save();
            SalesSettings = Settings.Default;
            popupCustomize.IsOpen = false;
        }

        private void sales_order_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_order_detail sales_order_detail = (sales_order_detail)e.NewItem;
            sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;

            sales_order_detail.sales_order = sales_order;
        }

        private async void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                item item = await SalesOrderDB.items.FindAsync(sbxItem.ItemID);

                if (item != null && item.id_item > 0 && sales_order != null)
                {
                    item_product item_product = item.item_product.FirstOrDefault();

                    if (item_product != null && item_product.can_expire)
                    {
                        crud_modalExpire.Visibility = Visibility.Visible;
                        pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry();
                        pnl_ItemMovementExpiry.id_item_product = item.item_product.FirstOrDefault().id_item_product;
                        crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                    }
                    else
                    {
                        Settings SalesSettings = new Settings();
                        Task Thread = Task.Factory.StartNew(() => select_Item(sales_order, item, sbxItem.QuantityInStock, SalesSettings.AllowDuplicateItem, null));
                    }
                 
                }
                sales_order.RaisePropertyChanged("GrandTotal");
            }
        }

        private void select_Item(sales_order sales_order, item item, decimal QuantityInStock, bool AllowDuplicateItem,int? movement_id)
        {
            if (sales_order.sales_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || AllowDuplicateItem)
            {
                sales_order_detail _sales_order_detail = new sales_order_detail();
                _sales_order_detail.State = EntityState.Added;
                _sales_order_detail.sales_order = sales_order;
                _sales_order_detail.Quantity_InStock = QuantityInStock;
                _sales_order_detail.Contact = sales_order.contact;
                _sales_order_detail.item_description = item.description;
                _sales_order_detail.item = item;
                _sales_order_detail.id_item = item.id_item;
                _sales_order_detail.movement_id = movement_id;
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

                        string number = sales_order.number != null ? sales_order.number : "";
                        string customer = sales_order.contact != null ? sales_order.contact.name : "";

                        if (customer.ToLower().Contains(query.ToLower())
                            || number.ToLower().Contains(query.ToLower()))
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
                    SalesOrderDB.sales_order_detail.Remove(sales_order_detail);
                    CollectionViewSource sales_ordersales_order_detailViewSource = FindResource("sales_ordersales_order_detailViewSource") as CollectionViewSource;
                    sales_ordersales_order_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
            if (sales_order != null)
            {
                if (sales_order.id_currencyfx > 0)
                {
                    if (SalesOrderDB.app_currencyfx.Where(x => x.id_currencyfx == sales_order.id_currencyfx).FirstOrDefault() != null)
                    {


                        sales_order.app_currencyfx = SalesOrderDB.app_currencyfx.Where(x => x.id_currencyfx == sales_order.id_currencyfx).FirstOrDefault();
                    }
                }
            }
            calculate_vat(sender, e);
        }



        private void sales_order_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

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

        private void btnSalesBudget_Click(object sender, RoutedEventArgs e)
        {
            cntrl.PanelAdv.pnlSalesBudget pnlSalesBudget = new cntrl.PanelAdv.pnlSalesBudget();

            crud_modal.Visibility = Visibility.Visible;
            if (sbxContact.ContactID > 0)
            {
                contact contact = SalesOrderDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlSalesBudget._contact = contact;
            }
            pnlSalesBudget.SalesBudget_Click += SalesBudget_Click;

            sales_order _sales_order = (sales_order)sales_orderViewSource.View.CurrentItem;
            pnlSalesBudget.sales_order = _sales_order;
            pnlSalesBudget.db = SalesOrderDB;
            crud_modal.Children.Add(pnlSalesBudget);
        }

        public async void SalesBudget_Click(object sender)
        {
            sales_order sales_order = (sales_order)sales_orderViewSource.View.CurrentItem;
            sales_order.contact = await SalesOrderDB.contacts.Where(x => x.id_contact == sales_order.id_contact).FirstOrDefaultAsync();
            sales_order.app_contract = await SalesOrderDB.app_contract.Where(x => x.id_contract == sales_order.id_contract).FirstOrDefaultAsync();

            foreach (sales_order_detail detail in sales_order.sales_order_detail)
            {
                detail.CurrencyFX_ID = sales_order.id_currencyfx;
                detail.item = await SalesOrderDB.items.Where(x => x.id_item == detail.id_item).FirstOrDefaultAsync();
                detail.app_vat_group = await SalesOrderDB.app_vat_group.Where(x => x.id_vat_group == detail.id_vat_group).FirstOrDefaultAsync();
            }

            cbxContactRelation.ItemsSource = SalesOrderDB.contacts.Where(x => x.parent.id_contact == sales_order.id_contact).ToList();

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

        private void Totals_btnClean_Click(object sender)
        {
            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;

            if (sales_order != null)
            {
                decimal TrailingDecimals = sales_order.GrandTotal - Math.Floor(sales_order.GrandTotal);
                sales_order.DiscountWithoutPercentage += TrailingDecimals;
            }
        }

        private void lblCheckCredit(object sender, RoutedEventArgs e)
        {

            if (sales_orderViewSource != null)
            {
                sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                sales_order.app_currencyfx = SalesOrderDB.app_currencyfx.Find(sales_order.id_currencyfx);
                Class.CreditLimit Limit = new Class.CreditLimit();
                Limit.Check_CreditAvailability(sales_order);
            }

        }

        private void toolBar_btnInvoice_Click(object sender, MouseButtonEventArgs e)
        {
            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
            if (sales_order != null && sales_order.status == Status.Documents_General.Approved && sales_order.sales_invoice.Count()==0)
            {
                sales_invoice sales_invoice = new sales_invoice();
                sales_invoice.barcode = sales_order.barcode;
                sales_invoice.code = sales_order.code;
                sales_invoice.trans_date = DateTime.Now;
                sales_invoice.comment = sales_order.comment;
                sales_invoice.id_condition = sales_order.id_condition;
                sales_invoice.id_contact = sales_order.id_contact;
                sales_invoice.contact = sales_order.contact;
                sales_invoice.id_contract = sales_order.id_contract;
                sales_invoice.id_currencyfx = sales_order.id_currencyfx;
                sales_invoice.id_project = sales_order.id_project;
                sales_invoice.id_sales_rep = sales_order.id_sales_rep;
                sales_invoice.id_weather = sales_order.id_weather;
                sales_invoice.is_impex = sales_order.is_impex;
                sales_invoice.sales_order = sales_order;
                foreach (sales_order_detail sales_order_detail in sales_order.sales_order_detail)
                {
                    sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();
                    sales_invoice_detail.comment = sales_order_detail.comment;
                    sales_invoice_detail.discount = sales_order_detail.discount;
                    sales_invoice_detail.id_item = sales_order_detail.id_item;
                    sales_invoice_detail.item_description = sales_order_detail.item_description;
                    sales_invoice_detail.id_location = sales_order_detail.id_location;
                    sales_invoice_detail.id_project_task = sales_order_detail.id_project_task;
                    sales_invoice_detail.id_sales_order_detail = sales_order_detail.id_sales_order_detail;
                    sales_invoice_detail.id_vat_group = sales_order_detail.id_vat_group;
                    sales_invoice_detail.quantity = sales_order_detail.quantity - sales_order_detail.sales_invoice_detail.Sum(x => x.quantity);
                    sales_invoice_detail.unit_cost = sales_order_detail.unit_cost;
                    sales_invoice_detail.unit_price = sales_order_detail.unit_price;
                    sales_invoice_detail.movement_id = sales_order_detail.movement_id;
                    sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);

                }

                SalesOrderDB.sales_invoice.Add(sales_invoice);
                crm_opportunity crm_opportunity = sales_order.crm_opportunity;
                crm_opportunity.sales_invoice.Add(sales_invoice);
                SalesOrderDB.SaveChanges();
                MessageBox.Show("Invoice Created Successfully..");
            }
            else
            {
                MessageBox.Show("Invoice Already Created Or Status is Not Approved..");
            }
        }

        private  async void crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
            item item = await SalesOrderDB.items.FindAsync(sbxItem.ItemID);

            if (item != null && item.id_item > 0 && sales_order != null)
            {
                Settings SalesSettings = new Settings();
                if (pnl_ItemMovementExpiry.item_movement!=null)
                {
                  
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_order, item, sbxItem.QuantityInStock, SalesSettings.AllowDuplicateItem, (int)pnl_ItemMovementExpiry.item_movement.id_movement));
                }
                else
                {
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_order, item, sbxItem.QuantityInStock, SalesSettings.AllowDuplicateItem, null));
                }

            }
        }

       
    }
}
