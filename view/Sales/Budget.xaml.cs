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
using System.Windows.Input;

namespace Cognitivo.Sales
{
    public partial class Budget : Page
    {
        //private SalesBudgetDB SalesBudgetDB = new SalesBudgetDB();

        private entity.Controller.Sales.BudgetController SalesBudgetDB;
        private CollectionViewSource sales_budgetViewSource, sales_budgetsales_budget_detailViewSource;
	
		public Budget()
        {
            InitializeComponent();
			
			SalesBudgetDB = FindResource("SalesBudget") as entity.Controller.Sales.BudgetController;
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                //Load Controller.
                SalesBudgetDB.Initialize();
            }
        }

        private void Page_Loaded(object sender, EventArgs e)
        {
		
			Settings SalesSettings = new Settings();
            SalesBudgetDB.Load(false, SalesSettings.FilterByBranch, dataPager.PagedSource.PageIndex);

            if (dataPager.PageCount == 0)
            {
                dataPager.PageCount = SalesBudgetDB.PageCount;
            }

            sales_budgetViewSource = FindResource("sales_budgetViewSource") as CollectionViewSource;
            sales_budgetViewSource.Source = SalesBudgetDB.db.sales_budget.Local;
			sales_budgetsales_budget_detailViewSource = FindResource("sales_budgetsales_budget_detailViewSource") as CollectionViewSource;

            CollectionViewSource app_document_rangeViewSource = FindResource("app_document_rangeViewSource") as CollectionViewSource;
            app_document_rangeViewSource.Source = entity.Brillo.Logic.Range.List_Range(SalesBudgetDB.db, entity.App.Names.SalesBudget, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
        }

        private void Print_Click(object sender, MouseButtonEventArgs e)
        {
            if (sales_budgetDataGrid.SelectedItem is sales_budget sales_budget)
            {
                entity.Brillo.Document.Start.Manual(sales_budget, sales_budget.app_document_range);
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        #region toolbar Events

        private void New_Click(object sender)
        {
            sales_budget sales_budget = SalesBudgetDB.Create(new Settings().TransDate_Offset, false);
            cbxCurrency.get_DefaultCurrencyActiveRate();

            SalesBudgetDB.db.sales_budget.Add(sales_budget);
            sales_budgetViewSource.View.MoveCurrentTo(sales_budget);
        }

        private void Edit_Click(object sender)
        {
            if (sales_budgetDataGrid.SelectedItem != null)
            {
                sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;

                if (sales_budget.status == Status.Documents_General.Pending)
                {
                    sales_budget.IsSelected = true;
                    sales_budget.State = EntityState.Modified;
                    SalesBudgetDB.db.Entry(sales_budget).State = EntityState.Modified;
                }
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void Save_Click(object sender)
        {
            if (SalesBudgetDB.SaveChanges_WithValidation())
            {
                sales_budgetViewSource.View.Refresh();
                toolBar.msgSaved(SalesBudgetDB.NumberOfRecords);
                sbxContact.Text = "";
            }
        }

        private void Delete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
                if (sales_budget!=null)
                {
                    SalesBudgetDB.db.sales_budget.Remove((sales_budget)sales_budgetDataGrid.SelectedItem);
                    sales_budgetViewSource.View.MoveCurrentToFirst();
                    Save_Click(sender);
                }
              
            }
        }

        private void Cancel_Click(object sender)
        {
            sales_budget_detailDataGrid.CancelEdit();
            sales_budgetViewSource.View.MoveCurrentToFirst();
            SalesBudgetDB.CancelAllChanges();

            if (sales_budgetsales_budget_detailViewSource.View != null)
                sales_budgetsales_budget_detailViewSource.View.Refresh();
        }

        #endregion toolbar Events

        #region Filter Data

        private async void SearchInSource_Click(object sender, KeyEventArgs e, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                Page_Loaded(null, null);
                //Brings data into view.
                Search_Click(sender, query);
            }
            else
            {
                sales_budgetViewSource = FindResource("sales_budgetViewSource") as CollectionViewSource;
                sales_budgetViewSource.Source = await SalesBudgetDB.db.sales_budget
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

        private void id_conditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = CurrentSession.Contracts.Where(x => x.id_condition == app_condition.id_condition).ToList();

                sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;

                if (sales_budget != null)
                {
                    if (sales_budget.id_contract == 0)
                    {
                        cbxContract.SelectedIndex = 0;
                    }
                }
            }
        }

        #endregion Filter Data

        private void Customize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void Customize_Closed(object sender, EventArgs e)
        {
            Settings SalesSettings = new Settings();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Settings.Default.Save();
            SalesSettings = Settings.Default;
            popupCustomize.IsOpen = false;
        }

        private void Calculate_Vat(object sender, EventArgs e)
        {
            sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
            if (sales_budget != null)
            {
                sales_budget.RaisePropertyChanged("GrandTotal");
                List<sales_budget_detail> sales_budget_detail = sales_budget.sales_budget_detail.ToList();
                dgvvat.ItemsSource = sales_budget_detail
                     .Join(SalesBudgetDB.db.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
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

        private void Sales_BudgetDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
            if (sales_budget != null)
            {
                Calculate_Vat(sender, e);
            }
        }

        private void Sales_Budget_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Calculate_Vat(sender, e);
        }

        private void Approve_Click(object sender)
        {
            if (SalesBudgetDB.Approve())
            {
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesBudgetDB.db, entity.App.Names.SalesBudget, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
                cbxDocument.SelectedIndex = 0;
                toolBar.msgApproved(SalesBudgetDB.NumberOfRecords);
            }
        }

        private void Anull_Click(object sender)
        {
            if (SalesBudgetDB.Annull())
            {
                toolBar.msgAnnulled(SalesBudgetDB.NumberOfRecords);
            }
        }

        private async void Item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
                item item = await SalesBudgetDB.db.items.FindAsync(sbxItem.ItemID);

                if (sales_budget != null && item != null)
                {
                    if (item != null && item.id_item > 0 && sales_budget != null)
                    {

                        int LineLimit = 0;
                        if (sales_budget.id_range > 0)
                        {
                            app_document_range app_document_range = SalesBudgetDB.db.app_document_range.Find(sales_budget.id_range);
                            if (app_document_range.app_document.line_limit != null)
                            {
                                LineLimit = (int)app_document_range.app_document.line_limit;
                            }
                        }

                        Settings SalesSettings = new Settings();
                        if (SalesSettings.BlockExcessItem == true && LineLimit > 0 && sales_budget.sales_budget_detail.Count + 1 > LineLimit)
                        {
                            toolBar.msgWarning("Your Item Limit is Exceed");
                        }
                        else
                        {
                            item_product item_product = item.item_product.FirstOrDefault();

                            Task Thread = Task.Factory.StartNew(() => Select_Item(sales_budget, item, sbxItem.QuantityInStock, SalesSettings.AllowDuplicateItem, null, sbxItem.Quantity));
                        }
                    }

                    sales_budget.RaisePropertyChanged("GrandTotal");
                }
            }
        }

        private void Select_Item(sales_budget sales_budget, item item, decimal QuantityInStock, bool AllowDuplicateItem, item_movement item_movement, decimal quantity)
        {
            long id_movement = item_movement != null ? item_movement.id_movement : 0;

            if (sales_budget.sales_budget_detail.Where(a => a.id_item == item.id_item && a.movement_id == id_movement).FirstOrDefault() == null || AllowDuplicateItem)
            {
                sales_budget_detail _sales_budget_detail = new sales_budget_detail()
                {
                    State = EntityState.Added,
                    sales_budget = sales_budget,
                    Quantity_InStock = QuantityInStock,
                    Contact = sales_budget.contact,
                    quantity = quantity,
                    item_description = item.description,
                    item = item,
                    id_item = item.id_item
                };

                if (item_movement != null)
                {
                    _sales_budget_detail.batch_code = item_movement.code;
                    _sales_budget_detail.expire_date = item_movement.expire_date;
                    _sales_budget_detail.movement_id = (int)item_movement.id_movement;
                }

                sales_budget.sales_budget_detail.Add(_sales_budget_detail);
            }
            else
            {
                sales_budget_detail sales_budget_detail = sales_budget.sales_budget_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_budget_detail.quantity += quantity;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                sales_budgetsales_budget_detailViewSource.View.Refresh();
                Calculate_Vat(null, null);
            }));
        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                sales_budgetViewSource.View.Filter = i =>
                {
                    sales_budget Budget = i as sales_budget;
                    string Name = Budget.contact != null ? Budget.contact.name : "";
                    string Number = Budget.number != null ? Budget.number : "";
					string trans_date = Budget.trans_date.Date.ToString("dd/MM/yyyy") ?? "";

					if (Name.ToLower().Contains(query.ToLower()) || Number.ToLower().Contains(query.ToLower()) || trans_date==query)
                    {
                        return true;
                    }

                    return false;
                };
            }
            else
            {
                sales_budgetViewSource.View.Filter = null;
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_budget_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    sales_budget_detailDataGrid.CancelEdit();
                    SalesBudgetDB.db.sales_budget_detail.Remove(e.Parameter as sales_budget_detail);
                    sales_budgetsales_budget_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private async void Set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = await SalesBudgetDB.db.contacts.FindAsync(sbxContact.ContactID);
                sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;

                if (contact != null && sales_budget != null)
                {
                    sales_budget.id_contact = contact.id_contact;
                    sales_budget.contact = contact;

                    //Checks Credit upcon Selection of Contact..
                    CheckCredit(null, null);

                    Task thread_SecondaryData = Task.Factory.StartNew(() => Set_ContactPref_Thread(contact));
                }
            }
        }

        private async void Set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxContactRelation.ItemsSource = SalesBudgetDB.db.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

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

                    //SalesMan
                    if (objContact.sales_rep != null)
                    {
                        cbxSalesRep.SelectedValue = objContact.id_sales_rep;
                    }
                }));
            }
        }

        private async void Currency_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sales_budgetViewSource.View.CurrentItem is sales_budget sales_budget)
            {
                if (sales_budget.id_currencyfx > 0)
                {
                    app_currencyfx app_currencyfx = await SalesBudgetDB.db.app_currencyfx.FindAsync(sales_budget.id_currencyfx);

                    if (app_currencyfx != null)
                    {
                        sales_budget.app_currencyfx = app_currencyfx;
                    }
                }
            }
            Calculate_Vat(sender, e);
        }

        private void Sales_Budget_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Calculate_Vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Calculate_Vat(sender, e);
        }

        private void CheckCredit(object sender, RoutedEventArgs e)
        {
            if (sales_budgetViewSource != null)
            {
                if (sales_budgetViewSource.View.CurrentItem is sales_budget o)
                {
                    o.app_currencyfx = SalesBudgetDB.db.app_currencyfx.Find(o.id_currencyfx);
                    o.contact = SalesBudgetDB.db.contacts.Find(o.id_contact);
                    o.app_contract = SalesBudgetDB.db.app_contract.Find(o.id_contract);

                    if (o.app_currencyfx != null && o.contact != null && o.app_contract != null)
                    {
                        entity.Controller.Finance.Credit Credit = new entity.Controller.Finance.Credit();
                        Credit.CheckLimit_InSales(0, o.app_currencyfx, o.contact, o.app_contract);
                    }
                }
            }
        }

        private void Order_Click(object sender, MouseButtonEventArgs e)
        {
            sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;

            if (sales_budget != null && sales_budget.status == Status.Documents_General.Approved )
            {
                sales_order sales_order = new sales_order()
                {
                    barcode = sales_budget.barcode,
                    code = sales_budget.code,
                    trans_date = DateTime.Now,
                    comment = sales_budget.comment,
                    delivery_date = sales_budget.delivery_date,
                    id_condition = sales_budget.id_condition,
                    id_contact = sales_budget.id_contact,
                    contact = sales_budget.contact,
                    id_contract = sales_budget.id_contract,
                    id_currencyfx = sales_budget.id_currencyfx,
                    id_project = sales_budget.id_project,
                    id_sales_rep = sales_budget.id_sales_rep,
                    id_weather = sales_budget.id_weather,
                    is_impex = sales_budget.is_impex,
                    sales_budget = sales_budget
                };

                foreach (sales_budget_detail sales_budget_detail in sales_budget.sales_budget_detail)
                {
                    decimal _qty = sales_budget_detail.quantity - sales_budget_detail.sales_order_detail
                                                                  .Where(x => x.sales_order.status != Status.Documents_General.Annulled)
                                                                  .Sum(x => x.quantity);
                    sales_order_detail sales_order_detail = new sales_order_detail()
                    {
                        comment = sales_budget_detail.comment,
                        discount = sales_budget_detail.discount,
                        id_item = sales_budget_detail.id_item,
                        item_description = sales_budget_detail.item_description,
                        id_location = sales_budget_detail.id_location,
                        id_project_task = sales_budget_detail.id_project_task,
                        id_sales_budget_detail = sales_budget_detail.id_sales_budget_detail,
                        id_vat_group = sales_budget_detail.id_vat_group,
                        quantity = sales_budget_detail.id_project_task!=null ?sales_budget_detail.quantity :_qty,
                        unit_cost = sales_budget_detail.unit_cost,
                        unit_price = sales_budget_detail.unit_price,
                        movement_id = sales_budget_detail.movement_id,
                        expire_date = sales_budget_detail.expire_date,
                        batch_code = sales_budget_detail.batch_code
                    };

                    ///Prevent adding 0 Quantity items. In the code above, 
                    ///We check Budget.Quantity - Order.Quantity where Order is not annulled.
                    ///If there are pending or approved Orders, those quantities will be included inthe minus, and if those
                    ///values cause the Current Sales Order Detail to have 0 or negative quantities, then we cannot push it
                    ///into a new sales order.

                    if (sales_order_detail.quantity > 0)
                    {
                        sales_order.sales_order_detail.Add(sales_order_detail);
                    }
                }

                SalesBudgetDB.db.sales_order.Add(sales_order);
                crm_opportunity crm_opportunity = sales_budget.crm_opportunity;
                crm_opportunity.sales_order.Add(sales_order);
                SalesBudgetDB.db.SaveChanges();

                //toolBar.
                MessageBox.Show(entity.Brillo.Localize.StringText("Done"));
            }
            else
            {
                MessageBox.Show("Order already created or status is not Approved..");
            }
        }

        private void toolBar_btnFocus_Click(object sender)
        {
            if (toolBar.ref_id > 0)
            {
                sales_budgetViewSource = FindResource("sales_budgetViewSource") as CollectionViewSource;
                sales_budgetViewSource.Source = SalesBudgetDB.db.sales_budget.Where(x => x.id_sales_budget == toolBar.ref_id).ToList();
            }
        }
        private void toolBar_btnClear_Click(object sender)
        {
            SalesBudgetDB.Initialize();
            sales_budgetViewSource = FindResource("sales_budgetViewSource") as CollectionViewSource;
            Page_Loaded(null, null);
        }

      

        private void dataPager_OnDemandLoading(object sender, Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs e)
        {
            Page_Loaded(null, null);
        }

      
	}
}