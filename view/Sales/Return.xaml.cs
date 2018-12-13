using entity;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace Cognitivo.Sales
{
    public partial class Return : Page
    {
        private entity.Controller.Sales.ReturnController SalesReturnDB;
	
		private CollectionViewSource salesReturnViewSource, sales_returnsales_return_detailViewSource;
        private cntrl.PanelAdv.pnlSalesInvoice pnlSalesInvoice;

        public Return()
        {
            InitializeComponent();
            //Load Controller.
            SalesReturnDB = FindResource("SalesReturnDB") as entity.Controller.Sales.ReturnController;

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                //Load Controller.
                SalesReturnDB.Initialize();
            }
        }

        private void load_PrimaryDataThread()
        {
            SalesReturnDB.Load(false, dataPager.PagedSource.PageIndex);

            salesReturnViewSource = (CollectionViewSource)FindResource("sales_returnViewSource");
            salesReturnViewSource.Source = SalesReturnDB.db.sales_return.Local;

            if (dataPager.PageCount == 0)
            {
                dataPager.PageCount = SalesReturnDB.PageCount;
            }
        }

        private void load_SecondaryDataThread()
        {
            sales_returnsales_return_detailViewSource = FindResource("sales_returnsales_return_detailViewSource") as CollectionViewSource;

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_vat_groupViewSource.Source = CurrentSession.VAT_Groups;

            cbxReturnType.ItemsSource = Enum.GetValues(typeof(Status.ReturnTypes));
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesReturnDB.db, entity.App.Names.SalesReturn, CurrentSession.Id_Branch, CurrentSession.Id_Company);

            CollectionViewSource app_branchViewSource = ((CollectionViewSource)(FindResource("app_branchViewSource")));
            app_branchViewSource.Source = CurrentSession.Branches;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                load_PrimaryDataThread();
                load_SecondaryDataThread();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #region Toolbar

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_return_detailDataGrid.CancelEdit();
            salesReturnViewSource.View.MoveCurrentToFirst();
            SalesReturnDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    sales_return s_return = (sales_return)sales_returnDataGrid.SelectedItem;
                    s_return.is_archived = true;
                    SalesReturnDB.db.SaveChanges();
                    salesReturnViewSource.View.MoveCurrentToFirst();
                    toolBar_btnSave_Click(sender);
                    sbxContact.Text = "";

                    load_PrimaryDataThread();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_returnDataGrid.SelectedItem != null)
            {
                sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
                if (sales_return.status == Status.Documents_General.Pending)
                {
                    sales_return.IsSelected = true;
                    sales_return.State = EntityState.Modified;
                    SalesReturnDB.db.Entry(sales_return).State = EntityState.Modified;
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            ReturnSetting _pref_SalesReturn = new ReturnSetting();
            sales_return objSalRtn = SalesReturnDB.Create(_pref_SalesReturn.TransDate_OffSet,false);
            objSalRtn.trans_date = DateTime.Now.AddDays(_pref_SalesReturn.TransDate_OffSet);
            SalesReturnDB.db.sales_return.Add(objSalRtn);
            salesReturnViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            SalesReturnDB.SaveChanges_WithValidation();
        }

        #endregion Toolbar

        private void calculate_vat(object sender, EventArgs e)
        {
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
            sales_return.RaisePropertyChanged("GrandTotal");
        }

        private void sales_return_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void sales_returnDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
            if (sales_return != null)
            {
                calculate_vat(sender, e);
            }
        }
        private async void SearchInSource_Click(object sender, KeyEventArgs e, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                Page_Loaded(null, null);
                //Brings data into view.
                toolBar_btnSearch_Click(sender, query);
            }
            else
            {
                salesReturnViewSource = FindResource("sales_returnViewSource") as CollectionViewSource;
                salesReturnViewSource.Source = await SalesReturnDB.db.sales_return
                    .Where
                    (
                    x =>
                    x.contact.name.Contains(query) ||
                    x.contact.gov_code.Contains(query) ||
                    x.number.Contains(query)
                    )
                .OrderByDescending(x => x.trans_date)
                .ThenBy(x => x.number)
                .ToListAsync();
            }
        }
        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                   
                 
                    salesReturnViewSource.View.Filter = i =>
                    {
                        sales_return sales_return = i as sales_return;
                        string number = sales_return.number ?? "";
                        string customer = "";
                        if (sales_return.contact!=null)
                        {
                            customer = sales_return.contact.name;
                        }
                        if (customer.ToLower().Contains(query.ToLower()) || number.ToLower().Contains(query.ToLower()))
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
                    salesReturnViewSource.View.Filter = null;
                }
            }
            catch (Exception)
            { }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_return_detail != null)
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
                    sales_return sales_return = salesReturnViewSource.View.CurrentItem as sales_return;
                    //DeleteDetailGridRow
                    sales_return_detailDataGrid.CancelEdit();
                    SalesReturnDB.db.sales_return_detail.Remove(e.Parameter as sales_return_detail);
                    sales_returnsales_return_detailViewSource.View.Refresh();
                    calculate_vat(null, null);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            try
            {
                if (sbxContact.ContactID > 0)
                {
                    contact contact = SalesReturnDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
                    sales_return.id_contact = contact.id_contact;
                    sales_return.contact = contact;

                    ///Start Thread to get Data.
                    Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxCurrency.get_ActiveRateXContact(ref objContact);
                }));
            }
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                Settings SalesSettings = new Settings();
                sales_return sales_return = salesReturnViewSource.View.CurrentItem as sales_return;
                item item = SalesReturnDB.db.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_return != null)
                {
                    int LineLimit = 0;

                    if (sales_return.id_range > 0)
                    {
                        app_document_range app_document_range = SalesReturnDB.db.app_document_range.Find(sales_return.id_range);
                        if (app_document_range.app_document.line_limit != null)
                        {
                            LineLimit = (int)app_document_range.app_document.line_limit;
                        }
                    }
                    if (SalesSettings.BlockExcessItem == true && LineLimit > 0 && sales_return.sales_return_detail.Count + 1 > LineLimit)
                    {
                        toolBar.msgWarning("Your Item Limit is Exceed");
                    }
                    else
                    {
                        Task Thread = Task.Factory.StartNew(() => select_Item(sales_return, SalesSettings.BlockExcessItem ,item, sbxItem.Quantity));
                    }
                }
                sales_return.RaisePropertyChanged("GrandTotal");
            }
        }

        private void select_Item(sales_return sales_return,bool AllowDuplicateItem, item item, decimal quantity)
        {
            if (sales_return.sales_return_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || AllowDuplicateItem)
            {
                sales_return_detail _sales_return_detail = new sales_return_detail();
                _sales_return_detail.State = EntityState.Added;
                _sales_return_detail.sales_return = sales_return;
                _sales_return_detail.Contact = sales_return.contact;
                _sales_return_detail.item_description = item.description;
                _sales_return_detail.item = item;
                _sales_return_detail.id_item = item.id_item;
                _sales_return_detail.quantity = quantity;
                sales_return.sales_return_detail.Add(_sales_return_detail);
            }
            else
            {
                sales_return_detail sales_return_detail = sales_return.sales_return_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_return_detail.quantity += quantity;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                sales_returnsales_return_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }

        private void salesinvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            sales_invoice sales_invoice = (sales_invoice)Hyperlink.Tag;
            if (sales_invoice != null)
            {
                entity.Brillo.Document.Start.Automatic(sales_invoice, sales_invoice.app_document_range);
            }
        }

        private void sales_return_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_return_detail sales_return_detail = (sales_return_detail)e.NewItem;
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;

            sales_return_detail.sales_return = sales_return;
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            SalesReturnDB.Approve();
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesReturnDB.db, entity.App.Names.SalesReturn, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
            cbxDocument.SelectedIndex = 0;

            load_PrimaryDataThread();
            salesReturnViewSource.View.Refresh();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            SalesReturnDB.Annull();
            foreach (sales_return sales_return in salesReturnViewSource.View.Cast<sales_return>().ToList())
            {
                sales_return.IsSelected = false;
            }
        }

        private void sales_return_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void btnSalesInvoice_Click(object sender, RoutedEventArgs e)
        {
            sales_return _sales_return = (sales_return)salesReturnViewSource.View.CurrentItem;

            if (_sales_return != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                pnlSalesInvoice = new cntrl.PanelAdv.pnlSalesInvoice();
                pnlSalesInvoice.db = new ImpexDB();

                if (sbxContact.ContactID > 0)
                {
                    contact contact = SalesReturnDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    pnlSalesInvoice._contact = contact;
                }

                pnlSalesInvoice.SalesInvoice_Click += SalesInvoice_Click;
                crud_modal.Children.Add(pnlSalesInvoice);
            }
        }

        public void SalesInvoice_Click(object sender)
        {
            sales_return _sales_return = (sales_return)salesReturnViewSource.View.CurrentItem;
            if (_sales_return != null)
            {
                if (pnlSalesInvoice.selected_sales_invoice.FirstOrDefault() != null)
                {


                    sbxContact.Text = pnlSalesInvoice.selected_sales_invoice.FirstOrDefault().contact.name;

                    foreach (sales_invoice sales_invoice in pnlSalesInvoice.selected_sales_invoice)
                    {
                        _sales_return.State = EntityState.Modified;
                        _sales_return.id_condition = sales_invoice.id_condition;
                        _sales_return.id_contract = sales_invoice.id_contract;
                        _sales_return.id_currencyfx = sales_invoice.id_currencyfx;
                        _sales_return.id_sales_invoice = sales_invoice.id_sales_invoice;

                        contact contact = SalesReturnDB.db.contacts.Where(x => x.id_contact == sales_invoice.id_contact).FirstOrDefault();
                        _sales_return.id_contact = contact.id_contact;
                        _sales_return.contact = contact;
                        sbxContact.Text = contact.name;

                        foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice.sales_invoice_detail.Where(x => x.Balance > 0))
                        {
                            sales_return_detail sales_return_detail = new sales_return_detail();
                            if (SalesReturnDB.db.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == _sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault() != null)
                            {
                                sales_return_detail.sales_invoice_detail = SalesReturnDB.db.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == _sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                            }

                            sales_return_detail.sales_return = _sales_return;

                            if (SalesReturnDB.db.items.Where(x => x.id_item == _sales_invoice_detail.id_item).FirstOrDefault() != null)
                            {
                                sales_return_detail.item = SalesReturnDB.db.items.Where(x => x.id_item == _sales_invoice_detail.id_item).FirstOrDefault();
                                sales_return_detail.item_description = _sales_invoice_detail.item_description;
                                sales_return_detail.movement_id = _sales_invoice_detail.movement_id;
                                sales_return_detail.batch_code = _sales_invoice_detail.batch_code;
                                sales_return_detail.expire_date = _sales_invoice_detail.expire_date;
                                //Automatically Checks Products to go back in Stock. This is most likely scenario.
                                if (sales_return_detail.item.item_product.Count() > 0)
                                {
                                    sales_return_detail.IsSelected = true;
                                }
                            }

                            sales_return_detail.id_item = _sales_invoice_detail.id_item;
                            sales_return_detail.quantity = _sales_invoice_detail.Balance;

                            sales_return_detail.id_vat_group = _sales_invoice_detail.id_vat_group;
                            sales_return_detail.unit_price = _sales_invoice_detail.unit_price;
                            sales_return_detail.CurrencyFX_ID = _sales_return.id_currencyfx;
                            sales_return_detail.unit_cost = _sales_invoice_detail.unit_cost;
                            _sales_return.sales_return_detail.Add(sales_return_detail);

                            SalesReturnDB.db.Entry(_sales_return).Entity.State = EntityState.Added;
                            crud_modal.Children.Clear();
                            crud_modal.Visibility = Visibility.Collapsed;
                            salesReturnViewSource.View.Refresh();

                            sales_returnsales_return_detailViewSource.View.Refresh();
                        }
                    }
                }
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            sales_return sales_return = sales_returnDataGrid.SelectedItem as sales_return;
            if (sales_return != null)
            {
                entity.Brillo.Document.Start.Manual(sales_return, sales_return.app_document_range);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
           
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Settings.Default.Save();
            Cognitivo.Properties.Settings.Default.Save();
            Settings _pref_SalesReturn = new Settings();
            _pref_SalesReturn = Settings.Default;
            popupCustomize.IsOpen = false;
        }

		private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void toolBar_btnFocus_Click(object sender)
        {
            salesReturnViewSource = FindResource("salesReturnViewSource") as CollectionViewSource;
            salesReturnViewSource.Source = SalesReturnDB.db.sales_return.Where(x => x.id_sales_return == toolBar.ref_id).ToList();
        }
        private void toolBar_btnClear_Click(object sender)
        {
            SalesReturnDB.Initialize();
            salesReturnViewSource = FindResource("salesReturnViewSource") as CollectionViewSource;
            load_PrimaryDataThread();
        }

        

        private void dataPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            load_PrimaryDataThread();
        }
	}
}