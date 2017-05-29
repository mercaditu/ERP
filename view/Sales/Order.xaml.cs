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
    public partial class Order : Page
    {
        private CollectionViewSource sales_orderViewSource;

        //private db db = new db();
        private entity.Controller.Sales.OrderController SalesDB;

        private cntrl.Panels.pnl_ItemMovementExpiry pnl_ItemMovementExpiry;

        public Order()
        {
            InitializeComponent();

           SalesDB = FindResource("SalesOrder") as entity.Controller.Sales.OrderController;
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                //Load Controller.
                SalesDB.Initialize();
            }
        }

        #region DataLoad

        private async void Load_PrimaryDataThread()
        {
            //Load Base Class
            SalesDB.Load(new Settings().FilterByBranch);

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                sales_orderViewSource = FindResource("sales_orderViewSource") as CollectionViewSource;
                sales_orderViewSource.Source = SalesDB.db.sales_order.Local;

                CollectionViewSource sales_ordersales_order_detailViewSource = FindResource("sales_ordersales_order_detailViewSource") as CollectionViewSource;
                if (SalesDB.db.sales_invoice.Local.Count() > 0)
                {
                    if (sales_ordersales_order_detailViewSource.View != null)
                    {
                        sales_ordersales_order_detailViewSource.View.Refresh();
                    }
                }
            }));
        }

        private async void Load_SecondaryDataThread()
        {
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesDB.db, entity.App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
            }));
        }

        #endregion DataLoad

        private void Page_Loaded(object sender, EventArgs e)
        {
            Load_PrimaryDataThread();
            Load_SecondaryDataThread();
        }

        #region toolbar Events

        private void New_Click(object sender)
        {
            Settings SalesSettings = new Settings();

            sales_order sales_order = SalesDB.Create(new Settings().TransDate_Offset, false);
            sales_orderViewSource.View.MoveCurrentTo(sales_order);

            cbxCurrency.get_DefaultCurrencyActiveRate();
        }

        private void Edit_Click(object sender)
        {
            if (sales_orderDataGrid.SelectedItem != null)
            {
                if (sales_orderDataGrid.SelectedItem is sales_order Order)
                {
                    SalesDB.Edit(Order);
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Delete_Click(object sender)
        {
            if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SalesDB.Archive();
            }
        }

        private void Save_Click(object sender)
        {
            if (SalesDB.SaveChanges_WithValidation())
            {
                toolBar.msgSaved(SalesDB.NumberOfRecords);
                sales_orderViewSource.View.Refresh();
                sbxContact.Text = "";
            }
        }

        private void Cancel_Click(object sender)
        {
            sales_orderViewSource.View.MoveCurrentToFirst();
            SalesDB.CancelAllChanges();

            if (sales_orderViewSource.View != null)
            {
                sales_orderViewSource.View.Refresh();
            }
        }

        private void Approve_Click(object sender)
        {
            if (SalesDB.Approve())
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesDB.db, entity.App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cbxDocument.SelectedIndex = 0;
                toolBar.msgApproved(SalesDB.NumberOfRecords);
            }
        }

        private void Anull_Click(object sender)
        {
            if (SalesDB.Annull())
            {
                toolBar.msgAnnulled(SalesDB.NumberOfRecords);
            }
        }

        #endregion toolbar Events

        #region Filter Data

        private void Set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = SalesDB.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
                sales_order.id_contact = contact.id_contact;
                sales_order.contact = contact;

                //Check Credit Rating.
                CheckCredit(null, null);
                Task thread_SecondaryData = Task.Factory.StartNew(() => Set_ContactPref_Thread(contact));
            }
        }

        private async void Set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxContactRelation.ItemsSource = SalesDB.db.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;

                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    
                    //Currency Selection
                    cbxCurrency.get_ActiveRateXContact(ref objContact);

                    //SalesMan
                    if (objContact.sales_rep != null)
                        cbxSalesRep.SelectedValue = objContact.sales_rep.id_sales_rep;
                }));
            }
        }

        private void Condition_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        #endregion Filter Data

        private void Calculate_vat(object sender, EventArgs e)
        {
            sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
            sales_order.RaisePropertyChanged("GrandTotal");
            if (sales_order != null)
            {
                List<sales_order_detail> sales_order_detail = sales_order.sales_order_detail.ToList();
                if (sales_order_detail.Count > 0)
                {
                    dgvvat.ItemsSource = sales_order_detail
                        .Join(SalesDB.db.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
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
            Calculate_vat(sender, e);
        }

        private void sales_orderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                sales_order sales_order = (sales_order)sales_orderDataGrid.SelectedItem;
                if (sales_order != null)
                {
                    Calculate_vat(sender, e);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Customize_MouseUp(object sender, MouseButtonEventArgs e)
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

        private async void Item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                item item = await SalesDB.db.items.FindAsync(sbxItem.ItemID);

                if (item != null && item.id_item > 0 && sales_order != null)
                {
                    item_product item_product = item.item_product.FirstOrDefault();

                    if (item_product != null && item_product.can_expire)
                    {
                        crud_modalExpire.Visibility = Visibility.Visible;
                        pnl_ItemMovementExpiry = new cntrl.Panels.pnl_ItemMovementExpiry(sales_order.id_branch, null, item_product.id_item_product);
                        crud_modalExpire.Children.Add(pnl_ItemMovementExpiry);
                    }
                    else
                    {
                        Settings SalesSettings = new Settings();
                        Task Thread = Task.Factory.StartNew(() => Select_Item(sales_order, item, sbxItem.QuantityInStock, SalesSettings.AllowDuplicateItem, null, sbxItem.Quantity));
                    }
                }
                sales_order.RaisePropertyChanged("GrandTotal");
            }
        }

        private void Select_Item(sales_order sales_order, item item, decimal QuantityInStock, bool AllowDuplicateItem, item_movement item_movement, decimal quantity)
        {
            long id_movement = item_movement != null ? item_movement.id_movement : 0;

            if (sales_order.sales_order_detail.Where(a => a.id_item == item.id_item && a.movement_id == id_movement).FirstOrDefault() == null || AllowDuplicateItem)
            {
                sales_order_detail _sales_order_detail = new sales_order_detail()
                {
                    State = EntityState.Added,
                    sales_order = sales_order,
                    quantity = quantity,
                    Quantity_InStock = QuantityInStock,
                    Contact = sales_order.contact,
                    item_description = item.description,
                    item = item,
                    id_item = item.id_item
                };

                if (item_movement != null)
                {
                    _sales_order_detail.batch_code = item_movement.code;
                    _sales_order_detail.expire_date = item_movement.expire_date;
                    _sales_order_detail.movement_id = (int)item_movement.id_movement;
					_sales_order_detail.Quantity_InStockLot = item_movement.avlquantity;
				}

                sales_order.sales_order_detail.Add(_sales_order_detail);
            }
            else
            {
                sales_order_detail sales_order_detail = sales_order.sales_order_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_order_detail.quantity += quantity;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                CollectionViewSource sales_ordersales_order_detailViewSource = FindResource("sales_ordersales_order_detailViewSource") as CollectionViewSource;
                sales_ordersales_order_detailViewSource.View.Refresh();
                Calculate_vat(null, null);
            }));
        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && sales_orderViewSource != null)
            {
                sales_orderViewSource.View.Filter = i =>
                {
                    sales_order Order = i as sales_order;

                    string number = Order.number ?? "";
                    string customer = Order.contact != null ? Order.contact.name : "";

                    if (customer.ToLower().Contains(query.ToLower())
                        || number.ToLower().Contains(query.ToLower()))
                    {
                        return true;
                    }

                    return false;
                };
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
            MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                //DeleteDetailGridRow
                dgvSalesDetail.CancelEdit();
                SalesDB.db.sales_order_detail.Remove(e.Parameter as sales_order_detail);
                CollectionViewSource sales_ordersales_order_detailViewSource = FindResource("sales_ordersales_order_detailViewSource") as CollectionViewSource;
                sales_ordersales_order_detailViewSource.View.Refresh();
            }
        }

        private void Currency_LostFocus(object sender, RoutedEventArgs e)
        {
            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
            if (sales_order != null)
            {
                if (sales_order.id_currencyfx > 0)
                {
                    if (SalesDB.db.app_currencyfx.Where(x => x.id_currencyfx == sales_order.id_currencyfx).FirstOrDefault() != null)
                    {
                        sales_order.app_currencyfx = SalesDB.db.app_currencyfx.Where(x => x.id_currencyfx == sales_order.id_currencyfx).FirstOrDefault();
                    }
                }
            }
            Calculate_vat(sender, e);
        }

        private void sales_order_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Calculate_vat(sender, e);
        }

        private void Print_Click(object sender, MouseButtonEventArgs e)
        {
            sales_order sales_order = sales_orderDataGrid.SelectedItem as sales_order;
            if (sales_order != null)
            {
                entity.Brillo.Document.Start.Manual(sales_order, sales_order.app_document_range);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void SalesBudget_Click(object sender, RoutedEventArgs e)
        {
            cntrl.PanelAdv.pnlSalesBudget pnlSalesBudget = new cntrl.PanelAdv.pnlSalesBudget();

            crud_modal.Visibility = Visibility.Visible;

            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
            if (sales_order != null)
            {
                contact contact = SalesDB.db.contacts.Where(x => x.id_contact == sales_order.id_contact).FirstOrDefault();
                pnlSalesBudget._contact = contact;
            }
            pnlSalesBudget.SalesBudget_Click += Budget_Click;

            sales_order _sales_order = (sales_order)sales_orderViewSource.View.CurrentItem;
            pnlSalesBudget.sales_order = _sales_order;
            pnlSalesBudget.db = SalesDB.db;
            crud_modal.Children.Add(pnlSalesBudget);
        }

        public async void Budget_Click(object sender)
        {
            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;

            if (sales_order != null)
            {
                sales_order.contact = await SalesDB.db.contacts.Where(x => x.id_contact == sales_order.id_contact).FirstOrDefaultAsync();
                sales_order.app_contract = await SalesDB.db.app_contract.Where(x => x.id_contract == sales_order.id_contract).FirstOrDefaultAsync();

                foreach (sales_order_detail detail in sales_order.sales_order_detail)
                {
                    detail.CurrencyFX_ID = sales_order.id_currencyfx;
                    detail.item = await SalesDB.db.items.Where(x => x.id_item == detail.id_item).FirstOrDefaultAsync();
                    detail.app_vat_group = await SalesDB.db.app_vat_group.Where(x => x.id_vat_group == detail.id_vat_group).FirstOrDefaultAsync();
                }

                cbxContactRelation.ItemsSource = SalesDB.db.contacts.Where(x => x.parent.id_contact == sales_order.id_contact).ToList();

                CollectionViewSource sales_ordersales_order_detailViewSource = ((CollectionViewSource)(FindResource("sales_ordersales_order_detailViewSource")));
                sales_orderViewSource.View.Refresh();
                sales_ordersales_order_detailViewSource.View.Refresh();
                crud_modal.Children.Clear();
                crud_modal.Visibility = Visibility.Collapsed;
            }
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

        private void CheckCredit(object sender, RoutedEventArgs e)
        {
            if (sales_orderViewSource != null)
            {
                if (sales_orderViewSource.View.CurrentItem is sales_order o)
                {
                    o.app_currencyfx = SalesDB.db.app_currencyfx.Find(o.id_currencyfx);
                    o.contact = SalesDB.db.contacts.Find(o.id_contact);
                    o.app_contract = SalesDB.db.app_contract.Find(o.id_contract);

                    if (o.app_currencyfx != null && o.contact != null && o.app_contract != null)
                    {
                        entity.Controller.Finance.Credit Credit = new entity.Controller.Finance.Credit();
                        Credit.CheckLimit_InSales(0, o.app_currencyfx, o.contact, o.app_contract);
                    }
                }
            }
        }

        private void Invoice_Click(object sender, MouseButtonEventArgs e)
        {
            sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;

            if (sales_order != null && sales_order.status == Status.Documents_General.Approved && sales_order.sales_invoice.Count() == 0)
            {
                sales_invoice sales_invoice = new sales_invoice()
                {
                    barcode = sales_order.barcode,
                    code = sales_order.code,
                    trans_date = DateTime.Now,
                    comment = sales_order.comment,
                    id_condition = sales_order.id_condition,
                    id_contact = sales_order.id_contact,
                    contact = sales_order.contact,
                    id_contract = sales_order.id_contract,
                    id_currencyfx = sales_order.id_currencyfx,
                    id_project = sales_order.id_project,
                    id_sales_rep = sales_order.id_sales_rep,
                    id_weather = sales_order.id_weather,
                    is_impex = sales_order.is_impex,
                    sales_order = sales_order
                };

                foreach (sales_order_detail sales_order_detail in sales_order.sales_order_detail)
                {
                    sales_invoice_detail sales_invoice_detail = new sales_invoice_detail()
                    {
                        comment = sales_order_detail.comment,
                        discount = sales_order_detail.discount,
                        id_item = sales_order_detail.id_item,
                        item_description = sales_order_detail.item_description,
                        id_location = sales_order_detail.id_location,
                        id_project_task = sales_order_detail.id_project_task,
                        id_sales_order_detail = sales_order_detail.id_sales_order_detail,
                        id_vat_group = sales_order_detail.id_vat_group,
                        quantity = sales_order_detail.quantity - (sales_order_detail.sales_invoice_detail != null ? sales_order_detail.sales_invoice_detail.Sum(x => x.quantity) : 0),
                        unit_cost = sales_order_detail.unit_cost,
                        unit_price = sales_order_detail.unit_price,
                        movement_id = sales_order_detail.movement_id
                    };

                    //If both are null, then we can just ignore the whole code.
                    if (sales_order_detail.expire_date != null || !string.IsNullOrEmpty(sales_order_detail.batch_code))
                    {
                        sales_invoice_detail.expire_date = sales_order_detail.expire_date;
                        sales_invoice_detail.batch_code = sales_order_detail.batch_code;
                    }

                    sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                }

                SalesDB.db.sales_invoice.Add(sales_invoice);
                crm_opportunity crm_opportunity = sales_order.crm_opportunity;
                crm_opportunity.sales_invoice.Add(sales_invoice);
                SalesDB.db.SaveChanges();

                MessageBox.Show("Invoice Created Successfully..");
            }
            else
            {
                MessageBox.Show("Invoice already created Or status is not Approved..");
            }
        }

        private async void Crud_modalExpire_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modalExpire.Visibility == Visibility.Collapsed || crud_modalExpire.Visibility == Visibility.Hidden)
            {
                sales_order sales_order = sales_orderViewSource.View.CurrentItem as sales_order;
                item item = await SalesDB.db.items.FindAsync(sbxItem.ItemID);

                if (item != null && item.id_item > 0 && sales_order != null)
                {
                    item_movement item_movement = SalesDB.db.item_movement.Find(pnl_ItemMovementExpiry.MovementID);
                    if (item_movement != null)
                    {
                        Settings SalesSettings = new Settings();
                        Task Thread = Task.Factory.StartNew(() => Select_Item(sales_order, item, sbxItem.QuantityInStock, SalesSettings.AllowDuplicateItem, item_movement, sbxItem.Quantity));
                    }
                }
            }
        }
    }
}