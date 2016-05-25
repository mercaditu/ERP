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

namespace Cognitivo.Sales
{
    public partial class Budget : Page
    {
        SalesBudgetDB SalesBudgetDB = new SalesBudgetDB();

        CollectionViewSource sales_budgetViewSource,
            sales_budgetsales_budget_detailViewSource,
            projectViewSource;

        public Budget()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings SalesSettings = new Settings();

                if (SalesSettings.FilterByBranch)
                {
                    SalesBudgetDB.sales_budget.Where(a => a.id_company == entity.CurrentSession.Id_Company && a.id_branch == entity.CurrentSession.Id_Branch).OrderByDescending(x => x.trans_date).Load();
                }
                else
                {
                    SalesBudgetDB.sales_budget.Where(a => a.id_company == entity.CurrentSession.Id_Company).OrderByDescending(x => x.trans_date).Load();
                }

                sales_budgetViewSource = ((CollectionViewSource)(FindResource("sales_budgetViewSource")));
                sales_budgetViewSource.Source = SalesBudgetDB.sales_budget.Local;
                sales_budgetsales_budget_detailViewSource = FindResource("sales_budgetsales_budget_detailViewSource") as CollectionViewSource;

                CollectionViewSource branchViewSource = ((CollectionViewSource)(FindResource("branchViewSource")));
                branchViewSource.Source = SalesBudgetDB.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == entity.CurrentSession.Id_Company).OrderBy(b => b.name).ToList();

                CollectionViewSource contractViewSource = ((CollectionViewSource)(FindResource("contractViewSource")));
                // Load data by setting the CollectionViewSource.Source property:
                SalesBudgetDB.app_contract.Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                contractViewSource.Source = SalesBudgetDB.app_contract.Local;

                CollectionViewSource conditionViewSource = ((CollectionViewSource)(FindResource("conditionViewSource")));
                // Load data by setting the CollectionViewSource.Source property:
                SalesBudgetDB.app_condition.Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                conditionViewSource.Source = SalesBudgetDB.app_condition.Local;

                projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
                // Load data by setting the CollectionViewSource.Source property:
                SalesBudgetDB.projects.Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                projectViewSource.Source = SalesBudgetDB.projects.Local;

                SalesBudgetDB.sales_rep.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxSalesRep.ItemsSource = SalesBudgetDB.sales_rep.Local;
                }));

                CollectionViewSource app_document_rangeViewSource = FindResource("app_document_rangeViewSource") as CollectionViewSource;
                app_document_rangeViewSource.Source = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesBudget, entity.CurrentSession.Id_Branch, entity.CurrentSession.Id_Terminal);

                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = await SalesBudgetDB.app_vat_group
                    .Where(a => a.is_active == true && a.id_company == entity.CurrentSession.Id_Company)
                    .OrderBy(a => a.name).ToListAsync();
                
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            sales_budget sales_budget = sales_budgetDataGrid.SelectedItem as sales_budget;
            if (sales_budget != null)
            {
              //  sales_invoice sales_invoice = sales_invoiceViewSource.View.CurrentItem as sales_invoice;
                entity.Brillo.Document.Start.Manual(sales_budget, sales_budget.app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

        #region toolbar Events
        private void New_Click(object sender)
        {
            sales_budget sales_budget = SalesBudgetDB.New();
            cbxCurrency.get_DefaultCurrencyActiveRate();

            SalesBudgetDB.sales_budget.Add(sales_budget);
            sales_budgetViewSource.View.MoveCurrentTo(sales_budget);
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_budgetDataGrid.SelectedItem != null)
            {
                sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
                sales_budget.IsSelected = true;
                sales_budget.State = EntityState.Modified;
                SalesBudgetDB.Entry(sales_budget).State = EntityState.Modified;
            }

            else
            {
                toolBar.msgWarning("Please Select A Record");
            }
        }

        private void Save_Click(object sender)
        {
            if (SalesBudgetDB.SaveChanges() == 1)
            {
                sales_budgetViewSource.View.Refresh();
                toolBar.msgSaved(SalesBudgetDB.NumberOfRecords);
                sbxContact.Text = "";   
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                SalesBudgetDB.sales_budget.Remove((sales_budget)sales_budgetDataGrid.SelectedItem);
                sales_budgetViewSource.View.MoveCurrentToFirst();
                Save_Click(sender);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_budget_detailDataGrid.CancelEdit();
            sales_budgetViewSource.View.MoveCurrentToFirst();
            SalesBudgetDB.CancelAllChanges();
            
            if (sales_budgetsales_budget_detailViewSource.View != null)
                sales_budgetsales_budget_detailViewSource.View.Refresh();
        }

     
        #endregion

        #region Filter Data
    
        private void id_conditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;

                CollectionViewSource contractViewSource = ((CollectionViewSource)(FindResource("contractViewSource")));
                contractViewSource.View.Filter = i =>
                {
                    app_contract objContract = (app_contract)i;
                    if (objContract.id_condition == app_condition.id_condition)
                    { return true; }
                    else
                    { return false; }
                };
                cbxContract.SelectedIndex = 0;
            }

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
            Settings SalesSettings = new Settings();

            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Settings.Default.Save();
            SalesSettings = Settings.Default;
            popupCustomize.IsOpen = false;
        }

        private void calculate_vat(object sender, EventArgs e)
        {
            sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
            sales_budget.RaisePropertyChanged("GrandTotal");
            List<sales_budget_detail> sales_budget_detail = sales_budget.sales_budget_detail.ToList();
            dgvvat.ItemsSource = sales_budget_detail
                 .Join(SalesBudgetDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
                      , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
                      .GroupBy(a => new { a.name, a.id_vat, a.ad })
               .Select(g => new
               {
                   id_vat = g.Key.id_vat,
                   name = g.Key.name,
                   value = g.Sum(a => a.value * a.ad.quantity)
               }).ToList();
        }

        private void sales_budgetDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;

            if (sales_budget != null)
            {
                calculate_vat(sender, e);
            }
        }

        private void sales_budget_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            if (SalesBudgetDB.Approve())
	        {
                toolBar.msgApproved(SalesBudgetDB.NumberOfRecords);
	        }
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            if (SalesBudgetDB.Anull())
            {
                toolBar.msgAnnulled(SalesBudgetDB.NumberOfRecords);
            }
        }

        private void item_Select(object sender, EventArgs e)
        {
           
            if (sbxItem.ItemID > 0)
            {
                sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
                item item = SalesBudgetDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_budget!= null)
                {
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_budget, item));
                }
                sales_budget.RaisePropertyChanged("GrandTotal");
            }
        }

        private void select_Item(sales_budget sales_budget, item item)
        {
            Settings SalesSettings = new Settings();

            if (sales_budget.sales_budget_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || SalesSettings.AllowDuplicateItem)
            {
                sales_budget_detail _sales_budget_detail = new sales_budget_detail();
                _sales_budget_detail.State = EntityState.Added;
                _sales_budget_detail.sales_budget = sales_budget;
                _sales_budget_detail.Contact = sales_budget.contact;
                _sales_budget_detail.item_description = item.description;
                _sales_budget_detail.item = item;
            
                _sales_budget_detail.id_item = item.id_item;
           
                sales_budget.sales_budget_detail.Add(_sales_budget_detail);
            }
            else
            {
                sales_budget_detail sales_budget_detail = sales_budget.sales_budget_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_budget_detail.quantity += 1;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
              
                sales_budgetsales_budget_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    sales_budgetViewSource.View.Filter = i =>
                    {
                        sales_budget sales_budget = i as sales_budget;
                        if (sales_budget.contact.name.ToLower().Contains(query.ToLower()))
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
                    sales_budgetViewSource.View.Filter = null;
                }
            }
            catch (Exception)
            { }
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

                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
                    sales_budget_detailDataGrid.CancelEdit();
                    SalesBudgetDB.sales_budget_detail.Remove(e.Parameter as sales_budget_detail);
                    sales_budgetsales_budget_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = SalesBudgetDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
                sales_budget.id_contact = contact.id_contact;
                sales_budget.contact = contact;
                Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    cbxContactRelation.ItemsSource = SalesBudgetDB.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;

                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);

                    //Currency
                    cbxCurrency.get_ActiveRateXContact(ref objContact);

                    //SalesMan
                    if (objContact.sales_rep != null)
                        cbxCondition.SelectedValue = objContact.sales_rep.id_sales_rep;

                    projectViewSource.Source = SalesBudgetDB.projects.Where(a => a.is_active == true
                                 && a.id_company == entity.Properties.Settings.Default.company_ID
                                 && a.id_contact == objContact.id_contact).OrderBy(a => a.name).ToList();
                }));
            }
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            calculate_vat(sender, e);
        }

        private void btnDuplicateInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
            if (sales_budget != null)
            {
                if (sales_budget.id_sales_budget != 0)
                {
                    var originalEntity = SalesBudgetDB.sales_budget.AsNoTracking().FirstOrDefault(x => x.id_sales_budget == sales_budget.id_sales_budget);
                    SalesBudgetDB.sales_budget.Add(originalEntity);
                    sales_budgetViewSource.View.Refresh();
                    sales_budgetViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    toolBar.msgWarning("Please save before duplicating");
                }
            }
        }

        private void sales_budget_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void lblEditProduct_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
