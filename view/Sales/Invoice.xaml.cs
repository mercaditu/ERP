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
using cntrl.Controls;
using System.Linq.Expressions;

namespace Cognitivo.Sales
{
    public partial class Invoice : INotifyPropertyChanged
    {
        //Global Variables
        CollectionViewSource sales_invoiceViewSource;
        CollectionViewSource sales_invoicesales_invoice_detailViewSource;
        CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource;

        SalesInvoiceDB SalesInvoiceDB = new SalesInvoiceDB();

        cntrl.PanelAdv.pnlPacking pnlPacking;
        cntrl.PanelAdv.pnlSalesOrder pnlSalesOrder;

        public entity.App.Names AppName { get; set; }
        public app_geography Geography { get; set; }
        public contact Contact { get; set; }
        public item Item { get; set; }

        public DateTime start_Range
        {
            get { return _start_Range; }
            set
            {
                if (_start_Range != value)
                {
                    _start_Range = value;
                }
            }
        }
        private DateTime _start_Range = DateTime.Now.AddMonths(-1);


        public DateTime end_Range
        {
            get { return _end_Range; }
            set
            {
                if (_end_Range != value)
                {
                    _end_Range = value;
                }
            }
        }
        private DateTime _end_Range = DateTime.Now.AddDays(+1);

        /// <summary>
        /// Condition KeyWord Array.
        /// </summary>
        public string[] ConditionArray { get; set; }
        public string tbxCondition
        {
            get
            {
                return _tbxCondition;
            }
            set
            {
                if (_tbxCondition != value)
                {
                    _tbxCondition = value;
                    ConditionArray = _tbxCondition.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        private string _tbxCondition;

        /// <summary>
        /// Contract KeyWord Array.
        /// </summary>
        public string[] ContractArray { get; set; }
        public string tbxContract
        {
            get
            {
                return _tbxContract;
            }
            set
            {
                if (_tbxContract != value)
                {
                    _tbxContract = value;
                    ContractArray = _tbxContract.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        private string _tbxContract;

        /// <summary>
        /// Tag KeyWord Array.
        /// </summary>
        public string[] TagArray { get; set; }
        public string tbxTag
        {
            get
            {
                return _tbxTag;
            }
            set
            {
                if (_tbxTag != value)
                {
                    _tbxTag = value;
                    TagArray = _tbxTag.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        private string _tbxTag;

        /// <summary>
        /// Brand KeyWord Array.
        /// </summary>
        public string[] BrandArray { get; set; }
        public string tbxBrand
        {
            get
            {
                return _tbxBrand;
            }
            set
            {
                if (_tbxBrand != value)
                {
                    _tbxBrand = value;
                    TagArray = _tbxBrand.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        private string _tbxBrand;

        public Invoice()
        {
            InitializeComponent();
            AppName = entity.App.Names.SalesInvoice;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Expression<Func<entity.sales_invoice, bool>> QueryBuilder()
        {
            var predicate = PredicateBuilder.True<entity.sales_invoice>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.is_head == true);

            if (ConditionArray != null)
            {
                if (ConditionArray.Count() > 0)
                {
                    predicate = predicate.And(x => ConditionArray.Contains(x.app_condition.name));
                }
            }

            if (ContractArray != null)
            {
                if (ContractArray.Count() > 0)
                {
                    predicate = predicate.And(x => ContractArray.Contains(x.app_contract.name));
                }
            }
            if (start_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date >= start_Range.Date);

            }
            if (end_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date <= end_Range.Date);

            }
            if (Contact != null)
            {
                predicate = predicate.And(x => x.contact == Contact);
            }
            return predicate;
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
            load_PrimaryDataThread();
            load_SecondaryDataThread();
        }

        private async void load_PrimaryDataThread()
        {
            SalesInvoiceDB = new entity.SalesInvoiceDB();
            Settings SalesSettings = new Settings();
            var predicate = QueryBuilder();

            if (SalesSettings.FilterByBranch)
            {
                await SalesInvoiceDB.sales_invoice.Where(predicate).OrderByDescending(x => x.trans_date).LoadAsync();

            }
            else
            {
                await SalesInvoiceDB.sales_invoice.Where(predicate).OrderByDescending(x => x.trans_date).LoadAsync();
            }


            await Dispatcher.InvokeAsync(new Action(() =>
            {
                sales_invoiceViewSource = ((CollectionViewSource)(FindResource("sales_invoiceViewSource")));
                sales_invoiceViewSource.Source = SalesInvoiceDB.sales_invoice.Local;
            }));
        }

        private async void load_SecondaryDataThread()
        {
            SalesInvoiceDB.app_contract.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).ToList();

            cbxContract.ItemsSource = SalesInvoiceDB.app_contract.Local;


            SalesInvoiceDB.app_condition.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

            cbxCondition.ItemsSource = SalesInvoiceDB.app_condition.Local;


            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            SalesInvoiceDB.sales_rep.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxSalesRep.ItemsSource = SalesInvoiceDB.sales_rep.Local;
            }));

            SalesInvoiceDB.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxBranch.ItemsSource = SalesInvoiceDB.app_branch.Local;
            }));

            SalesInvoiceDB.app_terminal.Where(b => b.is_active == true && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxTerminal.ItemsSource = SalesInvoiceDB.app_terminal.Local;
            }));

            SalesInvoiceDB.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = SalesInvoiceDB.app_vat_group.Local;
            }));

            cbxTransType.ItemsSource = Enum.GetValues(typeof(Status.TransactionTypes));

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
            Settings SalesSettings = new Settings();
            sales_invoice sales_invoice = SalesInvoiceDB.New(SalesSettings.TransDate_Offset);
            cbxCurrency.get_DefaultCurrencyActiveRate();

            SalesInvoiceDB.sales_invoice.Add(sales_invoice);

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
            if (SalesInvoiceDB.SaveChanges() > 0)
            {
                sales_invoiceViewSource.View.Refresh();
                toolBar.msgSaved(SalesInvoiceDB.NumberOfRecords);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            SalesInvoiceDB.CancelAllChanges();
        }

        private void btnApprove_Click(object sender)
        {
            Settings SalesSettings = new Settings();
            if (SalesInvoiceDB.Approve(SalesSettings.DiscountStock))
            {
                filter_sales();
            }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            SalesInvoiceDB.Anull();
            foreach (sales_invoice sales_invoice in sales_invoiceViewSource.View.Cast<sales_invoice>().ToList())
            {
                sales_invoice.IsSelected = false;
            }
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

                if (sales_invoice.sales_order == null)
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
                                                    && a.id_company == CurrentSession.Id_Company
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
                                                                        && a.id_company == CurrentSession.Id_Company
                                                                        && a.id_condition == app_condition.id_condition).ToListAsync();
                cbxContract.SelectedIndex = 0;
            }
        }

        #endregion

        private void calculate_vat(object sender, EventArgs e)
        {
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;
           
            if (sales_invoice != null)
            {
                sales_invoice.RaisePropertyChanged("GrandTotal");
                List<sales_invoice_detail> sales_invoice_detail = sales_invoice.sales_invoice_detail.ToList();
                if (sales_invoice_detail.Count > 0)
                {
                    var listvat = sales_invoice_detail
                           .Join(SalesInvoiceDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                               , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
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
            Settings SalesSettings = new Settings();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;

            Settings.Default.Save();
            SalesSettings = Settings.Default;
            popupCustomize.IsOpen = false;
        }
        #endregion

        private void sales_invoice_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_invoice_detail sales_invoice_detail = (sales_invoice_detail)e.NewItem;
            sales_invoice sales_invoice = (sales_invoice)sales_invoiceDataGrid.SelectedItem;

            sales_invoice_detail.sales_invoice = sales_invoice;
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                Settings SalesSettings = new Settings();
                sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                item item = SalesInvoiceDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                SalesInvoiceDB.Select_Item(ref sales_invoice, item, SalesSettings.AllowDuplicateItem);


                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                sales_invoice.RaisePropertyChanged("GrandTotal");
            }
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

                        if (sales_invoice != null)
                        {
                            //Protect the code against null values.
                            string number = sales_invoice.number != null ? sales_invoice.number : "";
                            string customer = sales_invoice.contact != null ? sales_invoice.contact.name : "";

                            if ((customer.ToLower().Contains(query.ToLower()))
                                || number.Contains(query))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    throw ex;
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
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
            if (sales_invoice!=null)
            {
                if (sales_invoice.id_currencyfx>0)
                {
                    if (SalesInvoiceDB.app_currencyfx.Where(x => x.id_currencyfx == sales_invoice.id_currencyfx).FirstOrDefault() != null)
                    {


                        sales_invoice.app_currencyfx = SalesInvoiceDB.app_currencyfx.Where(x => x.id_currencyfx == sales_invoice.id_currencyfx).FirstOrDefault();
                    }
                }
            }
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

                foreach (sales_packing_detail _sales_packing_detail in sales_packing.sales_packing_detail)
                {

                    if (_sales_invoice.sales_invoice_detail.Where(x => x.id_item == _sales_packing_detail.id_item).Count() == 0)
                    {
                        sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();
                        sales_invoice_detail.sales_invoice = _sales_invoice;
                        sales_invoice_detail.Contact = sbxContact.Contact;
                        sales_invoice_detail.item = _sales_packing_detail.item;
                        sales_invoice_detail.id_item = _sales_packing_detail.id_item;
                        sales_invoice_detail.quantity = _sales_packing_detail.quantity;
                        // sales_invoice_detail.unit_price = 0;

                        sales_packing_relation sales_packing_relation = new sales_packing_relation();
                        sales_packing_relation.id_sales_packing_detail = _sales_packing_detail.id_sales_packing_detail;
                        sales_packing_relation.sales_packing_detail = _sales_packing_detail;

                        sales_invoice_detail.sales_packing_relation.Add(sales_packing_relation);
                        _sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                    }
                }

                CollectionViewSource sales_invoicesales_invoice_detailViewSource = FindResource("sales_invoicesales_invoice_detailViewSource") as CollectionViewSource;
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
                sales_invoicesales_invoice_detailViewSource.View.MoveCurrentToFirst();
               
            }
            CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource") as CollectionViewSource;
            if (sales_invoicesales_invoice_detailsales_packinglist_relationViewSource != null)
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = SalesInvoiceDB.sales_packing_relation.Local.Where(x => x.sales_invoice_detail.id_sales_invoice == _sales_invoice.id_sales_invoice).ToList();
            }
            else
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = null;

            }
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
            _sales_invoice.RaisePropertyChanged("GrandTotal");

        }


        private void btnSalesOrder_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlSalesOrder = new cntrl.PanelAdv.pnlSalesOrder();
            pnlSalesOrder._entity = SalesInvoiceDB;
            pnlSalesOrder._contact = sbxContact.Contact;
            pnlSalesOrder.mode = cntrl.PanelAdv.pnlSalesOrder.module.sales_invoice;
            pnlSalesOrder.SalesOrder_Click += SalesOrder_Click;
            crud_modal.Children.Add(pnlSalesOrder);
        }

        public void SalesOrder_Click(object sender)
        {
            sales_invoice _sales_invoice = (sales_invoice)sales_invoiceViewSource.View.CurrentItem;

            sbxContact.Text = pnlSalesOrder.selected_sales_order.FirstOrDefault().contact.name;
            foreach (sales_order sales_order in pnlSalesOrder.selected_sales_order)
            {
                _sales_invoice.State = EntityState.Modified;
                _sales_invoice.contact = sales_order.contact;

                cbxContactRelation.ItemsSource = SalesInvoiceDB.contacts.Where(x => x.parent.id_contact == sales_order.contact.id_contact).ToList();

                _sales_invoice.id_contact = sales_order.contact.id_contact;
                _sales_invoice.id_condition = sales_order.id_condition;
                _sales_invoice.id_contract = sales_order.id_contract;
                _sales_invoice.id_currencyfx = sales_order.id_currencyfx;
                _sales_invoice.app_currencyfx = sales_order.app_currencyfx;
                _sales_invoice.id_sales_order = sales_order.id_sales_order;

                foreach (sales_order_detail _sales_order_detail in sales_order.sales_order_detail)
                {
                    sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();

                    //There is an issue that the detail does not know of the currency previously selected. Maybe this can help.
                    sales_invoice_detail.CurrencyFX_ID = sales_order.app_currencyfx.id_currencyfx;

                    sales_invoice_detail.id_sales_order_detail = _sales_order_detail.id_sales_order_detail;
                    sales_invoice_detail.sales_order_detail = _sales_order_detail;
                    sales_invoice_detail.Contact = _sales_invoice.contact;
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

                SalesInvoiceDB.Entry(_sales_invoice).Entity.State = EntityState.Added;
                crud_modal.Children.Clear();
                crud_modal.Visibility = Visibility.Collapsed;
                sales_invoiceViewSource.View.Refresh();
                sales_invoicesales_invoice_detailViewSource.View.Refresh();
            }
            _sales_invoice.RaisePropertyChanged("GrandTotal");
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
                entity.Brillo.Document.Start.Manual(sales_invoice, sales_invoice.app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        private void sales_invoiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_invoice sales_invoice = sales_invoiceDataGrid.SelectedItem as sales_invoice;
            CollectionViewSource sales_invoicesales_invoice_detailsales_packinglist_relationViewSource = FindResource("sales_invoicesales_invoice_detailsales_packinglist_relationViewSource") as CollectionViewSource;
            if (sales_invoicesales_invoice_detailsales_packinglist_relationViewSource != null)
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = SalesInvoiceDB.sales_packing_relation.Where(x => x.sales_invoice_detail.id_sales_invoice == sales_invoice.id_sales_invoice).ToList();
            }
            else
            {
                sales_invoicesales_invoice_detailsales_packinglist_relationViewSource.Source = null;

            }
            calculate_vat(sender,e);

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

        private void btnGridSearch(object sender, RoutedEventArgs e)
        {
            load_PrimaryDataThread();
        }

        private void btnTotalClean_Click(object sender)
        {
            sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;

            if (sales_invoice != null)
            {
                decimal TrailingDecimals = sales_invoice.GrandTotal - Math.Floor(sales_invoice.GrandTotal);
                sales_invoice.DiscountWithoutPercentage += TrailingDecimals;
            }
        }

        private void cbxBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxBranch.SelectedItem!=null)
            {
                app_branch app_branch = cbxBranch.SelectedItem as app_branch;
                cbxLocation.ItemsSource = app_branch.app_location.ToList();
                
            }
        }

      

      

        
    }
}
