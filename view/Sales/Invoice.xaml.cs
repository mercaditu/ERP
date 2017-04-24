using cntrl.Class;
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
using System.Windows.Documents;
using System.Windows.Input;

namespace Cognitivo.Sales
{
    public partial class Invoice
    {
        //Global Variables
        private CollectionViewSource sales_invoiceViewSource;

        private CollectionViewSource sales_invoicesales_invoice_detailViewSource;
        private CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource;

        private db db = new db();
        private entity.Controller.Sales.SalesInvoice SalesDB;

        private cntrl.PanelAdv.pnlPacking pnlPacking;
        private cntrl.PanelAdv.pnlSalesOrder pnlSalesOrder;

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (db != null)
            {
                if (disposing)
                {
                    db.Dispose();
                    // Dispose other managed resources.
                }
                //release unmanaged resources.
            }
        }
        #endregion

        public Invoice()
        {
            InitializeComponent();

            //Load DB into Controller.
            SalesDB = FindResource("sales_invoicesales_invoice_detailViewSource") as entity.Controller.Sales.SalesInvoice;
            SalesDB.db = db;
        }

        #region DataLoad

        private void SalesInvoice_Loaded(object sender, EventArgs e)
        {
            sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = (CollectionViewSource)FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource");
            sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;

            Load_PrimaryDataThread(null, null);
            Load_SecondaryDataThread();
        }

        private void Load_PrimaryDataThread(object sender, EventArgs e)
        {
            Settings Settings = new Settings();
            SalesDB.Load(Settings.FilterByBranch);

            sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
            sales_invoiceViewSource.Source = db.sales_invoice.Local;

            if (db.sales_invoice.Local.Count() > 0)
            {
                if (sales_invoicesales_invoice_detailViewSource.View != null)
                {
                    sales_invoicesales_invoice_detailViewSource.View.Refresh();
                }
            }
        }

        private async void Load_SecondaryDataThread()
        {
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(db, entity.App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
            }));

            cbxTransType.ItemsSource = Enum.GetValues(typeof(Status.TransactionTypes));
        }

        #endregion DataLoad

        #region "Action Events"

        private void New_Click(object sender)
        {
            Settings SalesSettings = new Settings();
            sales_invoice sales_invoice = SalesDB.Create(SalesSettings.TransDate_Offset, false);
            cbxCurrency.get_DefaultCurrencyActiveRate();
            db.sales_invoice.Add(sales_invoice);

            sales_invoiceViewSource.View.MoveCurrentToLast();
            sbxContact.Text = "";
            sbxItem.Text = "";
        }

        private void Edit_Click(object sender)
        {
            if (sales_invoiceDataGrid.SelectedItem != null)
            {
                sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
                if (sales_invoice != null)
                {
                    sales_invoice.IsSelected = true;
                    sales_invoice.State = EntityState.Modified;
                    db.Entry(sales_invoice).State = EntityState.Modified;
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Delete_Click(object sender)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
            if (sales_invoice != null && sales_invoice.State != EntityState.Added)
            {
                sales_invoice.is_archived = true;
                sales_invoice.State = EntityState.Modified;
                db.SaveChanges();

                Load_PrimaryDataThread(null, null);
            }
            else if (sales_invoice != null && sales_invoice.State == EntityState.Added)
            {
                Cancel_Click(sender);
            }
        }

        private void Save_Click(object sender)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;

            if (cbxDocument.SelectedItem is app_document_range app_document_range)
            {
                if (app_document_range.range_current > app_document_range.range_end)
                {
                    toolBar.msgWarning("Document Range is finished");
                    return;
                }
            }

            if (SalesDB.SaveChanges_and_Validate() > 0)
            {
                sales_invoiceViewSource.View.Refresh();
                toolBar.msgSaved(SalesDB.NumberOfRecords);
            }
        }

        private void Cancel_Click(object sender)
        {
            if (SalesDB.CancelAllChanges())
            {
                if (sales_invoiceViewSource.View != null)
                {
                    sales_invoiceViewSource.View.MoveCurrentToFirst();
                    sales_invoiceViewSource.View.Refresh();
                }
            }
        }

        private void Approve_Click(object sender)
        {
            if (SalesDB.Approve())
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(db, entity.App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cbxDocument.SelectedIndex = 0;
            }
            else
            {
                foreach (var Msg in SalesDB.Msg)
                {
                    toolBar.msgWarning(Msg.ToString());
                }
            }

            Load_PrimaryDataThread(null, null);
        }

        private void Anull_Click(object sender)
        {
            MessageBoxResult result = MessageBox.Show("Anull?", "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SalesDB.Annull();
            }
        }

        #endregion "Action Events"

        #region Filter Data

        private void Set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;

                if (db.contacts.Find(sbxContact.ContactID) != null && sales_invoice != null)
                {
                    //This code blocks incase a Sales Invoice already has an Associated Sales Order
                    if (sales_invoice.sales_order == null)
                    {
                        //Empty so that memory does not bring incorrect currency calculation
                        sales_invoice.contact = db.contacts.Find(sbxContact.ContactID);
                        sales_invoice.id_contact = db.contacts.Find(sbxContact.ContactID).id_contact;

                        ///Start Thread to get Data.
                        Task thread_SecondaryData = Task.Factory.StartNew(() => ContactPref_Thread(db.contacts.Find(sbxContact.ContactID)));
                    }
                }
            }
        }

        private async void ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxContactRelation.ItemsSource = db.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

                    if (objContact.id_sales_rep != null)
                    {
                        cbxSalesRep.SelectedValue = Convert.ToInt32(objContact.id_sales_rep);
                    }
                    //Condition
                    if (objContact.app_contract != null)
                    {
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    }
                    //Contract
                    if (objContact.id_contract != null)
                    {
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    }
                    //Currency
                    cbxCurrency.get_ActiveRateXContact(ref objContact);

                    //entity.Controller.Finance.Credit Credit = new entity.Controller.Finance.Credit();
                    //Credit.CheckLimit_InSales(sales_invoice.GrandTotal, sales_invoice.app_currencyfx, sales_invoice.contact, sales_invoice.app_contract);

                    if (cbxCondition.SelectedItem != null && cbxContract.SelectedItem != null && cbxCurrency.SelectedValue > 0)
                    {
                        sbxItem.SmartBoxItem_Focus();
                    }
                }));
            }
        }

        private void Condition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxCondition.SelectedItem != null)
            {
                cbxContract.ItemsSource = CurrentSession.Contracts.Where(x => x.id_condition == (cbxCondition.SelectedItem as app_condition).id_condition).ToList();

                if ((sales_invoice)sales_invoiceDataGrid.SelectedItem != null)
                {
                    if (((sales_invoice)sales_invoiceDataGrid.SelectedItem).id_contract == 0)
                    {
                        cbxContract.SelectedIndex = 0;
                    }
                }
            }
        }

        #endregion Filter Data

        private void calculate_vat(object sender, EventArgs e)
        {
            if ((sales_invoice)sales_invoiceDataGrid.SelectedItem != null)
            {
                ((sales_invoice)sales_invoiceDataGrid.SelectedItem).RaisePropertyChanged("GrandTotal");
                List<sales_invoice_detail> sales_invoice_detail = ((sales_invoice)sales_invoiceDataGrid.SelectedItem).sales_invoice_detail.ToList();
                if (sales_invoice_detail.Count > 0)
                {
                    var listvat = sales_invoice_detail
                           .Join(db.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                               , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * (cfx.app_vat.coefficient * cfx.percentage), id_vat = cfx.app_vat.id_vat, ad })
                               .GroupBy(a => new { a.name, a.id_vat, a.ad })
                       .Select(g => new
                       {
                           id_vat = g.Key.id_vat,
                           name = g.Key.name,
                           value = g.Sum(a => a.value * a.ad.quantity)
                       }).ToList();

                    dgvVAT.ItemsSource = listvat.GroupBy(x => x.id_vat).Select(g => new
                    {
                        id_vat = g.Max(y => y.id_vat),
                        name = g.Max(y => y.name),
                        value = g.Sum(a => a.value)
                    }).ToList();
                }
            }
        }

        private void Detail_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
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
            Settings SalesSettings = new Settings();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;

            Settings.Default.Save();
            SalesSettings = Settings.Default;
            popupCustomize.IsOpen = false;
        }

        #endregion PrefSettings

        private void Detail_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            ((sales_invoice_detail)e.NewItem).sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
        }

        private void Item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                if (sales_invoiceViewSource.View.CurrentItem is sales_invoice sales_invoice)
                {
                    int LineLimit = 0;
                    if (sales_invoice.id_range > 0)
                    {
                        app_document_range app_document_range = db.app_document_range.Find(sales_invoice.id_range);
                        if (app_document_range.app_document.line_limit != null)
                        {
                            LineLimit = (int)app_document_range.app_document.line_limit;
                        }
                    }

                    Settings SalesSettings = new Settings();
                    if (SalesSettings.BlockExcessItem == true && LineLimit > 0 && sales_invoice.sales_invoice_detail.Count + 1 > LineLimit)
                    {
                        toolBar.msgWarning("Your Item Limit is Exceed");
                    }
                    else
                    {
                        //int BranchID = (int)cbxBranch.SelectedValue;

                        item item = db.items.Find(sbxItem.ItemID);
                        item_product item_product = item.item_product.FirstOrDefault();

                        if (item_product != null && item_product.can_expire)
                        {
                            crud_modalExpire.Visibility = Visibility.Visible;
                            cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(sales_invoice.id_branch, null, item.item_product.FirstOrDefault().id_item_product);
                            crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                        }
                        else
                        {
                            sales_invoice_detail _sales_invoice_detail = 
                                SalesDB.Create_Detail(ref sales_invoice, item, null,
                                SalesSettings.AllowDuplicateItem,
                                sbxItem.QuantityInStock,
                                sbxItem.Quantity);

                            sales_invoicesales_invoice_detailViewSource.View.Refresh();
                            sales_invoice.RaisePropertyChanged("GrandTotal");
                        }
                    }
                }
            }
        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && sales_invoiceViewSource != null)
            {
                sales_invoiceViewSource.View.Filter = i =>
                {
                    sales_invoice Invoice = i as sales_invoice;
                    contact contact = Invoice.contact ?? null;

                    if (Invoice != null)
                    {
                        //Protect the code against null values.
                        string number = Invoice.number ?? "";
                        string customer = "";
                        string cust_code = "";
                        string cust_gov_code = "";

                        if (contact != null)
                        {
                            if (contact.name != null)
                            {
                                customer = contact.name.ToLower();
                            }
                            if (contact.code != null)
                            {
                                cust_code = contact.code.ToLower();
                            }
                            if (contact.gov_code != null)
                            {
                                cust_gov_code = contact.gov_code.ToLower();
                            }

                            if (customer.Contains(query.ToLower())
                                ||
                                cust_code.Contains(query.ToLower())
                                ||
                                cust_gov_code.Contains(query.ToLower())
                                ||
                                number.Contains(query))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                };
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
                MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                    dgvSalesDetail.CancelEdit();
                    db.sales_invoice_detail.Remove(e.Parameter as sales_invoice_detail);
                    sales_invoicesales_invoice_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Currency_LostFocus(object sender, RoutedEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            if (sales_invoice != null)
            {
                if (sales_invoice.id_currencyfx > 0)
                {
                    app_currencyfx app_currencyfx = db.app_currencyfx.Find(sales_invoice.id_currencyfx);
                    if (app_currencyfx != null)
                    {
                        sales_invoice.app_currencyfx = app_currencyfx;
                    }
                }
            }
            calculate_vat(sender, e);
        }

        private void Detail_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void PackingList_Click(object sender, RoutedEventArgs e)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;

            if (sales_invoice != null)
            {
                crud_modal.Visibility = Visibility.Visible;

                pnlPacking = new cntrl.PanelAdv.pnlPacking()
                {
                    _entity = db,
                    _contact = db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault() //sbxContact.Contact as contact;
                };

                pnlPacking.Link_Click += Link_Click;
                crud_modal.Children.Add(pnlPacking);
                (FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource).View.Refresh();
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        public void Link_Click(object sender)
        {
            sales_invoice _sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem;

            foreach (sales_packing item in pnlPacking.selected_sales_packing)
            {
                sales_packing sales_packing = db.sales_packing.Where(x => x.id_sales_packing == item.id_sales_packing).FirstOrDefault();

                foreach (sales_packing_detail _sales_packing_detail in sales_packing.sales_packing_detail.Where(x => x.user_verified))
                {
                    sales_order_detail sales_order_detail = _sales_packing_detail.sales_order_detail;

                    sales_invoice_detail sales_invoice_detail = new sales_invoice_detail()
                    {
                        id_location = _sales_packing_detail.id_location
                    };

                    sales_packing_relation sales_packing_relation = new sales_packing_relation()
                    {
                        id_sales_packing_detail = _sales_packing_detail.id_sales_packing_detail,
                        sales_packing_detail = _sales_packing_detail,
                        id_sales_invoice_detail = sales_invoice_detail.id_sales_invoice_detail,
                        sales_invoice_detail = sales_invoice_detail
                    };

                    sales_invoice_detail.sales_packing_relation.Add(sales_packing_relation);

                    if (_sales_packing_detail.expire_date != null || !string.IsNullOrEmpty(_sales_packing_detail.batch_code))
                    {
                        sales_invoice_detail.expire_date = _sales_packing_detail.expire_date;
                        sales_invoice_detail.batch_code = _sales_packing_detail.batch_code;
                    }

                    //if SalesOrder Exists, use it for Price and VAT.
                    if (sales_order_detail != null)
                    {
                        sales_invoice_detail.sales_invoice = _sales_invoice;
                        sales_invoice_detail.Contact = db.contacts.Find(sbxContact.ContactID);// sbxContact.Contact;
                        sales_invoice_detail.item = _sales_packing_detail.item;
                        sales_invoice_detail.id_item = _sales_packing_detail.id_item;
                        sales_invoice_detail.quantity = Convert.ToDecimal(_sales_packing_detail.verified_quantity);
                        sales_invoice_detail.id_vat_group = sales_order_detail.id_vat_group;
                        sales_invoice_detail.State = EntityState.Added;
                        sales_invoice_detail.unit_price = sales_order_detail.unit_price + sales_order_detail.discount;
                        sales_invoice_detail.discount = sales_order_detail.discount;

                        _sales_invoice.id_sales_rep = sales_order_detail.sales_order.id_sales_rep;
                        _sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                    }
                    else
                    {
                        //If Sales Order does not exist, use Price and VAT From standard of the company.
                        SalesDB.Create_Detail(ref _sales_invoice, 
                            _sales_packing_detail.item, 
                            null, 
                            false, 
                            0, 
                            (decimal)_sales_packing_detail.verified_quantity);
                    }
                }

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                sales_invoicesales_invoice_detailViewSource.View.MoveCurrentToFirst();
            }

            CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource") as CollectionViewSource;

            if (sales_invoicesales_invoice_detailsales_packinglist_relationViewSource != null)
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = db.sales_packing_relation.Local.Where(x => x.sales_invoice_detail.id_sales_invoice == _sales_invoice.id_sales_invoice).ToList();
            }
            else
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = null;
            }

            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
            _sales_invoice.RaisePropertyChanged("GrandTotal");
        }

        private void SalesOrder_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlSalesOrder = new cntrl.PanelAdv.pnlSalesOrder()
            {
                db = db,
                _contact = db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault(), // sbxContact.Contact;
                mode = cntrl.PanelAdv.pnlSalesOrder.module.sales_invoice
            };
            pnlSalesOrder.SalesOrder_Click += SalesOrder_Save;
            crud_modal.Children.Add(pnlSalesOrder);
        }

        public void SalesOrder_Save(object sender)
        {
            sales_invoice Invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem;
            if (Invoice != null)
            {
                sbxContact.Text = pnlSalesOrder.selected_sales_order.FirstOrDefault().contact.name;
                foreach (sales_order sales_order in pnlSalesOrder.selected_sales_order)
                {
                    Invoice.State = EntityState.Modified;
                    Invoice.contact = sales_order.contact;

                    cbxContactRelation.ItemsSource = db.contacts.Where(x => x.parent.id_contact == sales_order.contact.id_contact).ToList();

                    Invoice.id_contact = sales_order.contact.id_contact;
                    Invoice.id_condition = sales_order.id_condition;
                    Invoice.id_contract = sales_order.id_contract;
                    Invoice.id_currencyfx = sales_order.id_currencyfx;
                    Invoice.app_currencyfx = sales_order.app_currencyfx;
                    Invoice.id_sales_order = sales_order.id_sales_order;
                    Invoice.id_project = sales_order.id_project;
                    Invoice.id_sales_rep = sales_order.id_sales_rep;

                    foreach (sales_order_detail _sales_order_detail in sales_order.sales_order_detail)
                    {
                        sales_invoice_detail sales_invoice_detail = new sales_invoice_detail()
                        {

                            //There is an issue that the detail does not know of the currency previously selected. Maybe this can help.
                            CurrencyFX_ID = sales_order.app_currencyfx.id_currencyfx,

                            id_sales_order_detail = _sales_order_detail.id_sales_order_detail,
                            sales_order_detail = _sales_order_detail,
                            id_project_task = _sales_order_detail.id_project_task,
                            Contact = Invoice.contact,
                            sales_invoice = Invoice,
                            item = _sales_order_detail.item,
                            id_item = _sales_order_detail.id_item,
                            quantity = _sales_order_detail.quantity - db.sales_invoice_detail
                                                                                     .Where(x => x.id_sales_order_detail == _sales_order_detail.id_sales_order_detail)
                                                                                     .GroupBy(x => x.id_sales_order_detail)
                                                                                     .Select(x => x.Sum(y => y.quantity))
                                                                                     .FirstOrDefault(),
                            id_vat_group = _sales_order_detail.id_vat_group,
                            unit_price = _sales_order_detail.unit_price,
                            movement_id = _sales_order_detail.movement_id
                        };

                        if (_sales_order_detail.expire_date != null || !string.IsNullOrEmpty(_sales_order_detail.batch_code))
                        {
                            sales_invoice_detail.expire_date = _sales_order_detail.expire_date;
                            sales_invoice_detail.batch_code = _sales_order_detail.batch_code;
                        }
                        if (sales_invoice_detail.quantity > 0)
                        {
                            Invoice.sales_invoice_detail.Add(sales_invoice_detail);
                        }
                    }

                    db.Entry(Invoice).Entity.State = EntityState.Added;
                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                    sales_invoiceViewSource.View.Refresh();
                    sales_invoicesales_invoice_detailViewSource.View.Refresh();
                }
            }
            Invoice.RaisePropertyChanged("GrandTotal");
        }

        private void salesorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            if ((sales_order)Hyperlink.Tag != null)
            {
                entity.Brillo.Document.Start.Automatic((sales_order)Hyperlink.Tag, ((sales_order)Hyperlink.Tag).app_document_range);
            }
        }

        private void salespackinglist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((sales_packing)((Hyperlink)sender).Tag != null)
            {
                entity.Brillo.Document.Start.Automatic((sales_packing)((Hyperlink)sender).Tag, ((sales_packing)((Hyperlink)sender).Tag).app_document_range);
            }
        }

        private void Print_Click(object sender, MouseButtonEventArgs e)
        {
            if (sales_invoiceDataGrid.SelectedItem is sales_invoice sales_invoice && sales_invoice.status == Status.Documents_General.Approved)
            {
                entity.Brillo.Document.Start.Manual(sales_invoice, sales_invoice.app_document_range);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Invoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource") as CollectionViewSource;

            if (sales_invoicesales_invoice_detailsales_packinglist_relationViewSource != null)
            {
                if (sales_invoiceDataGrid.SelectedItem as sales_invoice != null)
                {
                    (sales_invoiceDataGrid.SelectedItem as sales_invoice).RaisePropertyChanged("GrandTotal");
                    int id_sales_invoice = (sales_invoiceDataGrid.SelectedItem as sales_invoice).id_sales_invoice;
                    sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = db.sales_packing_relation.Where(x => x.sales_invoice_detail.id_sales_invoice == id_sales_invoice).ToList();
                }
            }
            else
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = null;
            }
            
        }

        private void RecivePayment_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sales_invoiceDataGrid.SelectedItem is sales_invoice sales_invoice)
            {
                if (new entity.Brillo.Security(entity.App.Names.AccountsReceivable).create)
                {
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.Curd.receive_payment recive_payment = new cntrl.Curd.receive_payment();
                    recive_payment.sales_invoice = sales_invoice;
                    crud_modal.Children.Add(recive_payment);
                }
                else
                {
                    toolBar.msgWarning("Access Denied. Please contact your Administrator.");
                }
            }
        }

        private async void AccountsRecievable_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            if (sales_invoice != null)
            {
                List<payment_schedual> payment_schedualList = await db.payment_schedual
                                     .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company && x.id_contact == sales_invoice.id_contact
                                         && (x.id_sales_invoice > 0 || x.id_sales_order > 0) && x.id_note == null
                                         && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                                         .OrderBy(x => x.expire_date).ToListAsync();

                payment_schedualDataGrid.ItemsSource = payment_schedualList.GroupBy(x => x.id_currencyfx).Select(x => new { CustName = x.Max(s => s.contact.name), AccountReceivableBalance = x.Sum(y => y.AccountReceivableBalance), Currency = x.Max(z => z.app_currencyfx.app_currency.name) });
                crud_modalDuePaymnet.Visibility = Visibility.Visible;
            }
        }

        private void GridSearch(object sender, RoutedEventArgs e)
        {
            Load_PrimaryDataThread(null, null);
        }

        private void Branch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxBranch.SelectedItem != null)
            {
                app_branch app_branch = cbxBranch.SelectedItem as app_branch;
                cbxLocation.ItemsSource = CurrentSession.Locations.Where(x => x.id_branch == app_branch.id_branch).ToList();
            }
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            crud_modalDuePaymnet.Visibility = Visibility.Collapsed;
        }

        private void CheckCredit(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource != null)
            {
                if (sales_invoiceViewSource.View.CurrentItem is sales_invoice sales_invoice)
                {
                    sales_invoice.app_currencyfx = db.app_currencyfx.Find(sales_invoice.id_currencyfx);
                    Class.CreditLimit Limit = new Class.CreditLimit();
                    Limit.Check_CreditAvailability(sales_invoice);
                }
            }
        }

        private void Return_Click(object sender, MouseButtonEventArgs e)
        {
            if (sales_invoiceViewSource.View.CurrentItem is sales_invoice sales_invoice && 
                sales_invoice.status == Status.Documents_General.Approved && 
                sales_invoice.sales_return.Count() == 0)
            {
                sales_return sales_return = new sales_return()
                {
                    barcode = sales_invoice.barcode,
                    code = sales_invoice.code,
                    trans_date = DateTime.Now,
                    comment = sales_invoice.comment,
                    id_condition = sales_invoice.id_condition,
                    id_contact = sales_invoice.id_contact,
                    contact = sales_invoice.contact,
                    id_contract = sales_invoice.id_contract,
                    id_currencyfx = sales_invoice.id_currencyfx,
                    id_project = sales_invoice.id_project,
                    id_sales_rep = sales_invoice.id_sales_rep,
                    id_weather = sales_invoice.id_weather,
                    is_impex = sales_invoice.is_impex,
                    sales_invoice = sales_invoice
                };

                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
                {
                    sales_return_detail sales_return_detail = new sales_return_detail()
                    {
                        comment = sales_invoice_detail.comment,
                        discount = sales_invoice_detail.discount,
                        id_item = sales_invoice_detail.id_item,
                        item_description = sales_invoice_detail.item_description,
                        id_location = sales_invoice_detail.id_location,
                        id_project_task = sales_invoice_detail.id_project_task,
                        sales_invoice_detail = sales_invoice_detail,
                        id_vat_group = sales_invoice_detail.id_vat_group,
                        quantity = sales_invoice_detail.quantity - sales_invoice_detail.sales_return_detail.Sum(x => x.quantity),
                        unit_cost = sales_invoice_detail.unit_cost,
                        unit_price = sales_invoice_detail.unit_price,
                        movement_id = sales_invoice_detail.movement_id
                    };

                    if (sales_invoice_detail.expire_date != null || !string.IsNullOrEmpty(sales_invoice_detail.batch_code))
                    {
                        sales_return_detail.expire_date = sales_invoice_detail.expire_date;
                        sales_return_detail.batch_code = sales_invoice_detail.batch_code;
                    }

                    sales_return.sales_return_detail.Add(sales_return_detail);
                }

                db.sales_return.Add(sales_return);
                crm_opportunity crm_opportunity = sales_invoice.crm_opportunity;
                crm_opportunity.sales_return.Add(sales_return);
                db.SaveChanges();
                MessageBox.Show("Return Created Successfully ..");
            }
            else
            {
                MessageBox.Show("Return Already Created Or Status is Not Approved..");
            }
        }

        private void Refinance_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceDataGrid.SelectedItem as sales_invoice;
            if (sales_invoice != null)
            {
                if (new entity.Brillo.Security(entity.App.Names.AccountsReceivable).create)
                {
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.Curd.RefinanceSales RefinanceSales = new cntrl.Curd.RefinanceSales();
                    RefinanceSales.sales_invoice = sales_invoice;
                    crud_modal.Children.Add(RefinanceSales);
                }
                else
                {
                    toolBar.msgWarning("Access Denied. Please contact your Administrator.");
                }
            }
        }

        private void crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                sales_invoice sales_invoice = sales_invoiceDataGrid.SelectedItem as sales_invoice;
                item item = db.items.Find(sbxItem.ItemID);

                cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry = crud_modalExpire.Children.OfType<cntrl.Panels.pnl_ItemMovementExpiry>().FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_invoice != null)
                {
                    if (pnl_ItemMovementExpiry.MovementID > 0)
                    {
                        item_movement item_movement = db.item_movement.Find(pnl_ItemMovementExpiry.MovementID);

                        Settings SalesSettings = new Settings();
                        SalesDB.Create_Detail(ref sales_invoice, item, item_movement, SalesSettings.AllowDuplicateItem, sbxItem.QuantityInStock, sbxItem.Quantity);
                        sales_invoicesales_invoice_detailViewSource.View.Refresh();
                        sales_invoice.RaisePropertyChanged("GrandTotal");
                    }
                    else
                    {
                        toolBar.msgWarning("Batch not selected correctly.");
                    }
                }

                //Cleans for reuse.
                crud_modalExpire.Children.Clear();
            }
        }

        private void chbxRowDetail_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;
            if ((bool)chbx.IsChecked)
            {
                dgvSalesDetail.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                dgvSalesDetail.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }

        private void navList_CheckedChanged(object sender, RoutedEventArgs e)
        {
            int TotalPending = 0;
            int TotalApproved = 0;

            foreach (sales_invoice item in db.sales_invoice.Local.Where(x => x.IsSelected))
            {
                if (item.status == Status.Documents_General.Pending)
                {
                    TotalPending += 1;
                    //RaisePropertyChanged("TotalPending");
                }
                else if (item.status == Status.Documents_General.Approved)
                {
                    TotalApproved += 1;
                    //RaisePropertyChanged("TotalApproved");
                }
            }

            toolBar.TotalApproved = TotalApproved;
            toolBar.TotalPending = TotalPending;
        }

        private void ApproveEdit_Click(object sender)
        {

        }

        private void ReApprove_Click(object sender)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
            if (sales_invoice != null)
            {
                UpdateMovementReApprove UpdateMovementReApprove = new UpdateMovementReApprove();
                CheckMovementReApprove CheckMovementReApprove = new CheckMovementReApprove();
                UpdatePaymentReApprove UpdatePaymentReApprove = new UpdatePaymentReApprove();
                CheckPaymentReApprove CheckPaymentReApprove = new CheckPaymentReApprove();

                string Message = CheckPaymentReApprove.Check_ContractChanges(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdatePaymentReApprove.Update_ContractChanges(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }

                Message = CheckPaymentReApprove.Check_ValueUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdatePaymentReApprove.Update_ValueUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }

                Message = CheckPaymentReApprove.Check_ValueDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdatePaymentReApprove.Update_ValueDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }

                Message += CheckPaymentReApprove.Check_CurrencyChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdatePaymentReApprove.Update_CurrencyChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }

                Message = CheckPaymentReApprove.Check_DateChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdatePaymentReApprove.Update_DateChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }

                Message = "";
                Message = CheckMovementReApprove.CheckQuantityUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);

                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdateMovementReApprove.QuantityUP(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }
                Message = CheckMovementReApprove.CheckQuantityDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);

                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdateMovementReApprove.QuantityDown(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }
                Message = CheckMovementReApprove.CheckDateChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);

                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdateMovementReApprove.DateChange(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }

                Message = CheckMovementReApprove.CheckNewMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);

                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdateMovementReApprove.NewMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }
                Message = CheckMovementReApprove.CheckDeleteMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);

                if (Message != "")
                {
                    Message += "\n" + "Are You Sure Want To Change The Data..";
                    if (MessageBox.Show(Message, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UpdateMovementReApprove.DeleteMovement(db, sales_invoice.id_sales_invoice, entity.App.Names.SalesInvoice);
                    }
                }
            }
            //SalesInvoiceDB.ReApprove(sales_invoice);
            sales_invoiceViewSource.View.Refresh();
            db.SaveChanges();
        }
    }
}