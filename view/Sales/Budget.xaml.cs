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

        public string budget_number { get; set; }

        contact _contact;

        ContactDB ContactdbContext = new ContactDB();
        SalesBudgetDB dbContext = new SalesBudgetDB();

        CollectionViewSource sales_budgetViewSource, 
            sales_budgetsales_budget_detailViewSource, 
            projectViewSource, 
            contractViewSource, 
             
            contactViewSource, 
            conditionViewSource;

        BudgetSetting _pref_SalesBudget = new BudgetSetting();

        public Budget()
        {
            InitializeComponent();
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //public void RaisePropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.Properties.Settings _settings = new entity.Properties.Settings();

                sales_budgetViewSource = ((CollectionViewSource)(FindResource("sales_budgetViewSource")));
                if (_pref_SalesBudget.filterbyBranch)
                {
                    dbContext.sales_budget.Where(a => a.id_company == _settings.company_ID && a.id_branch == _settings.branch_ID).Include(x => x.sales_budget_detail).Load();
                }
                else
                {
                    dbContext.sales_budget.Where(a => a.id_company == _settings.company_ID ).Include(x => x.sales_budget_detail).Load();
                }
                sales_budgetViewSource.Source = dbContext.sales_budget.Local;
                sales_budgetsales_budget_detailViewSource = FindResource("sales_budgetsales_budget_detailViewSource") as CollectionViewSource;

                CollectionViewSource branchViewSource = ((CollectionViewSource)(FindResource("branchViewSource")));
                branchViewSource.Source = dbContext.app_branch.Where(b => b.can_invoice == true && b.is_active == true && b.id_company == _settings.company_ID).OrderBy(b => b.name).ToList();

                contractViewSource = ((CollectionViewSource)(FindResource("contractViewSource")));
                // Load data by setting the CollectionViewSource.Source property:
                dbContext.app_contract.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).Load();
                contractViewSource.Source = dbContext.app_contract.Local;

                conditionViewSource = ((CollectionViewSource)(FindResource("conditionViewSource")));
                // Load data by setting the CollectionViewSource.Source property:
                dbContext.app_condition.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).Load();
                conditionViewSource.Source = dbContext.app_condition.Local;

                projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
                // Load data by setting the CollectionViewSource.Source property:
                dbContext.projects.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).Load();
                projectViewSource.Source = dbContext.projects.Local;

                CollectionViewSource app_document_rangeViewSource = FindResource("app_document_rangeViewSource") as CollectionViewSource;
                app_document_rangeViewSource.Source = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesBudget, _settings.branch_ID, _settings.terminal_ID);
         

                //CollectionViewSource app_document_rangeViewSource = FindResource("app_document_rangeViewSource") as CollectionViewSource;
                //app_document_rangeViewSource.Source = await dbContext.app_document_range.Where(d => d.is_active == true && d.app_document.id_application ==entity.App.Names.SalesBudget && d.id_company == _settings.company_ID).ToListAsync();

                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = await dbContext.app_vat_group
                    .Where(a => a.is_active == true && a.id_company == _settings.company_ID)
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
            sales_budget sales_budget = dbContext.New();
            cbxCurrency.get_DefaultCurrencyActiveRate();

            dbContext.sales_budget.Add(sales_budget);
            sales_budgetViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_budgetDataGrid.SelectedItem != null)
            {
                sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
                sales_budget.IsSelected = true;
                sales_budget.State = EntityState.Modified;
                dbContext.Entry(sales_budget).State = EntityState.Modified;
            }

            else
            {
                toolBar.msgWarning("Please Select A Record");
            }
        }

        private void Save_Click(object sender)
        {
                dbContext.SaveChanges();
                sales_budgetViewSource.View.Refresh();
                toolBar.msgSaved();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                dbContext.sales_budget.Remove((sales_budget)sales_budgetDataGrid.SelectedItem);
                sales_budgetViewSource.View.MoveCurrentToFirst();
                Save_Click(sender);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_budget_detailDataGrid.CancelEdit();
            sales_budgetViewSource.View.MoveCurrentToFirst();
            dbContext.CancelAllChanges();
            
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
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            BudgetSetting.Default.Save();
            _pref_SalesBudget = BudgetSetting.Default;
            popupCustomize.IsOpen = false;
        }

        private void calculate_vat(object sender, EventArgs e)
        {
            sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
            sales_budget.RaisePropertyChanged("GrandTotal");
            List<sales_budget_detail> sales_budget_detail = sales_budget.sales_budget_detail.ToList();
            dgvvat.ItemsSource = sales_budget_detail
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

        //private void calculate_total(object sender, EventArgs e)
        //{
        //    sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
        //    if (sales_budget != null)
        //    {
        //        sales_budget.get_Sales_Budget_Total();
        //    }
        //}

        private void sales_budgetDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
            if (sales_budget != null)
            {
                calculate_vat(sender, e);
                
               
            }
            //calculate_total(sender, e);
        }

        //private void sales_budgetDataGrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    calculate_total(sender, e);
        //}

        private void sales_budget_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        #region ExpressAdd/Edit
        private void AddNewContact_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            sbxContact.Contact.State = EntityState.Added;
            sbxContact.Contact.is_customer = true;
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.contact contact = new cntrl.Curd.contact();
           // contact.btnSave_Click += ContactSave_Click;
            contact.contactobject = sbxContact.Contact;
            crud_modal.Children.Add(contact);
        }

       
        public void ContactSave_Click(object sender)
        {
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
            
        }
        private void AddNewCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.condition condition = new cntrl.condition();
            crud_modal.Children.Add(condition);
        }

        private void EditCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            app_condition app_condition = cbxCondition.SelectedItem as app_condition;
            if (app_condition != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.condition condition = new cntrl.condition();
                crud_modal.Children.Add(condition);
            }
        }

        private void AddNewContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.contract contract = new cntrl.contract();
            contract.app_contractViewSource = contractViewSource;
            contract.MainViewSource = sales_budgetViewSource;
            contract.curObject = sales_budgetViewSource.View.CurrentItem;
            contract.operationMode = cntrl.Class.clsCommon.Mode.Add;
            contract.isExternalCall = true;
            crud_modal.Children.Add(contract);
        }

        private void EditContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            app_contract app_contract = cbxContract.SelectedItem as app_contract;
            if (app_contract != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.contract contract = new cntrl.contract();
                contract.app_contractViewSource = contractViewSource;
                contract.MainViewSource = sales_budgetViewSource;
                contract.curObject = sales_budgetViewSource.View.CurrentItem;
                //contract.entity = _entity;
                contract.app_contractobject = app_contract;
                contract.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                contract.isExternalCall = true;
                crud_modal.Children.Add(contract);
            }
        }
        #endregion

        private void toolBar_btnApprove_Click(object sender)
        {
            dbContext.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            dbContext.Anull();
        }

        private void item_Select(object sender, EventArgs e)
        {
           
            if (sbxItem.ItemID > 0)
            {
                sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
                item item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_budget!= null)
                {
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_budget, item));
                }
            }
        }

        private void select_Item(sales_budget sales_budget, item item)
        {

            if (sales_budget.sales_budget_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || _pref_SalesBudget.AllowDuplicateItems)
            {
                sales_budget_detail _sales_budget_detail = new sales_budget_detail();
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
                    //DeleteDetailGridRow
                    sales_budget_detailDataGrid.CancelEdit();
                    dbContext.sales_budget_detail.Remove(e.Parameter as sales_budget_detail);
                    sales_budgetsales_budget_detailViewSource.View.Refresh();
                    //calculate_total(sender, e);

                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        //private void contactComboBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (contactComboBox.Data != null)
        //    {
        //        contact contact = (contact)contactComboBox.Data;
        //        GetContactDetail_PreviewMouseUp(null, null);
        //    }
        //}

        //private void contactComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (contactComboBox.Data != null)
        //    {
        //        contact contact = (contact)contactComboBox.Data;
        //        GetContactDetail_PreviewMouseUp(null, null);
        //        contactComboBox.focusGrid = false;
        //        contactComboBox.Text = contact.name;
        //    }
        //}

       

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
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
                    cbxContactRelation.ItemsSource = dbContext.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    //Currency
                    cbxCurrency.get_ActiveRateXContact(ref objContact);

                    projectViewSource.Source = dbContext.projects.Where(a => a.is_active == true
                                 && a.id_company == entity.Properties.Settings.Default.company_ID
                                 && a.id_contact == objContact.id_contact).OrderBy(a => a.name).ToList();
                }));
            }
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void btnDuplicateInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
            if (sales_budget != null)
            {
                if (sales_budget.id_sales_budget != 0)
                {
                    var originalEntity = dbContext.sales_budget.AsNoTracking().FirstOrDefault(x => x.id_sales_budget == sales_budget.id_sales_budget);
                    dbContext.sales_budget.Add(originalEntity);
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

        //private void cbxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cbxDocument.SelectedValue != null)
        //    {
        //        entity.Brillo.Logic.Document _Document = new entity.Brillo.Logic.Document();
              
        //        app_document_range app_document_range = (app_document_range)cbxDocument.SelectedItem;
        //        sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;

        //        entity.Brillo.Range.branch_Code = dbContext.app_branch.Where(x => x.id_branch == sales_budget.id_branch).FirstOrDefault().code;
        //        entity.Brillo.Range.terminal_Code = dbContext.app_terminal.Where(x => x.id_terminal == sales_budget.id_terminal).FirstOrDefault().code;
        //        budget_number = entity.Brillo.Range.calc_Range(app_document_range, false);
        //        RaisePropertyChanged("budget_number");
        //    }
        //}

        private void lblEditProduct_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //item _item = (item)itemComboBox.Data;
            //if (_item != null)
            //{
                
            //    crud_modal.Visibility = Visibility.Visible;
            //    cntrl.Curd.item item = new cntrl.Curd.item();
            //    item.itemViewSource = itemViewSource;
            //    item.MainViewSource = sales_budgetViewSource;
            //    item.curObject = sales_budgetViewSource.View.CurrentItem;
            //    //item._entity = _entity;
            //    item.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            //    item.STbox = itemComboBox;
            //    item.itemobject = _item;
            //    crud_modal.Children.Add(item);
            //}
            //else
            //{
            //    toolBar.msgWarning("No Item Selected");
            //}
        }
    }
}
