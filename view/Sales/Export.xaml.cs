using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using entity.Brillo;

namespace Cognitivo.Sales
{
    /// <summary>
    /// Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : Page
    {
        ImpexDB ImpexDB = new ImpexDB();
        CollectionViewSource impexViewSource, impeximpex_expenseViewSource, sales_invoiceViewSource = null;
        cntrl.PanelAdv.pnlSalesInvoice pnlSalesInvoice = new cntrl.PanelAdv.pnlSalesInvoice();

        List<entity.Class.Impex_ItemDetail> Impex_CostDetailLIST = new List<entity.Class.Impex_ItemDetail>();
        List<entity.Class.Impex_Products> Impex_ProductsLIST = new List<entity.Class.Impex_Products>();

        int company_ID = CurrentSession.Id_Company;

        public Export()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //sales_invoiceViewSource
                sales_invoiceViewSource = this.FindResource("sales_invoiceViewSource") as CollectionViewSource;


                impexViewSource = this.FindResource("impexViewSource") as CollectionViewSource;
                ImpexDB.impex
                    .Include(x => x.impex_export)
                    .Include(x => x.impex_expense)
                    .Where(x => x.impex_type == impex._impex_type.Export && x.is_active == true && x.id_company == company_ID)
                    .Load();
                impexViewSource.Source = ImpexDB.impex.Local;
                impeximpex_expenseViewSource = this.FindResource("impeximpex_expenseViewSource") as CollectionViewSource;

                //incotermViewSource
                CollectionViewSource incotermViewSource = this.FindResource("incotermViewSource") as CollectionViewSource;
                incotermViewSource.Source = ImpexDB.impex_incoterm.OrderBy(a => a.name).ToList();
                //statusViewSource
                //CollectionViewSource statusViewSource = this.FindResource("statusViewSource") as CollectionViewSource;
                //statusViewSource.Source = _entity.db.app_status.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();
                //itemsViewSource
                CollectionViewSource itemsViewSource = this.FindResource("itemsViewSource") as CollectionViewSource;
                itemsViewSource.Source = ImpexDB.items.Where(a => a.is_active == true && a.id_company == company_ID).ToList();
                //impex_typeComboBox.ItemsSource = Enum.GetValues(typeof(entity.impex._impex_type));
                //CurrencyFx
                CollectionViewSource currencyfxViewSource = this.FindResource("currencyfxViewSource") as CollectionViewSource;
                currencyfxViewSource.Source = ImpexDB.app_currencyfx.Include("app_currency").Where(a => a.is_active == true).ToList();
                //incotermconditionViewSource
                CollectionViewSource incotermconditionViewSource = this.FindResource("incotermconditionViewSource") as CollectionViewSource;

                incotermconditionViewSource.Source = ImpexDB.impex_incoterm_condition.OrderBy(a => a.name).ToList();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #region Toolbar events
        private void toolBar_btnNew_Click(object sender)
        {
            sales_invoiceViewSource.Source = ImpexDB.sales_invoice.Where(a => a.id_company == company_ID && a.id_contact == 0 && a.is_issued == true).OrderByDescending(a => a.trans_date).ToList();
            impex impex = new impex();
            impex.impex_type = entity.impex._impex_type.Export;
            impex.eta = DateTime.Now;
            impex.etd = DateTime.Now;
            impex.is_active = true;
            //id_sal_invoiceComboBox.SelectedIndex = 0;
            impex.State = EntityState.Added;
            impex.status = Status.Documents_General.Pending;
            impex.IsSelected = true;
            ImpexDB.impex.Add(impex);
            impexViewSource.View.MoveCurrentToLast();
            Impex_CostDetailLIST.Clear();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = (impex)impexDataGrid.SelectedItem;
                impex.IsSelected = true;
                impex.State = EntityState.Modified;
                ImpexDB.Entry(impex).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                ImpexDB.impex.Remove(impex);
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            impex impex = impexDataGrid.SelectedItem as impex;

            List<impex_expense> impexexpenselist = impex.impex_expense.Where(x => x.value <= 0).ToList();
            foreach (impex_expense impex_expense in impexexpenselist)
            {
                impex.impex_expense.Remove(impex_expense);
            }

            if (ImpexDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ImpexDB.NumberOfRecords);
            }
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            try
            {
                impex impex = impexDataGrid.SelectedItem as impex;

                if (impex.status != Status.Documents_General.Approved)
                {
                    List<impex_expense> impex_expenses = impex.impex_expense.ToList();
                    List<entity.Class.Impex_ItemDetail> ImpexImportDetails = (List<entity.Class.Impex_ItemDetail>)impex_ExportDataGrid.ItemsSource;
                    if (ImpexImportDetails.Count > 0)
                    {
                        //To make sure we have a Purchase Total
                        decimal SalesTotal = ImpexImportDetails.Sum(i => i.sub_total);
                        if (SalesTotal != 0)
                        {
                            foreach (entity.Class.Impex_ItemDetail detail in ImpexImportDetails)
                            {
                                //Get total value of a Product Row
                                decimal itemTotal = detail.quantity * detail.unit_cost;

                                sales_invoice sales_invoice = ImpexDB.sales_invoice.Where(x => x.id_sales_invoice == detail.id_invoice).FirstOrDefault();
                                item_movement item_movement = ImpexDB.item_movement.Where(x => x.id_sales_invoice_detail == detail.id_invoice_detail).FirstOrDefault();

                                foreach (impex_expense _impex_expense in impex_expenses)
                                {
                                    decimal condition_value = (decimal)_impex_expense.value;
                                    if (condition_value != 0 && itemTotal != 0)
                                    {
                                        //Coeficient is used to get prorated cost of one item
                                        item_movement_value item_movement_detail = new item_movement_value();

                                        decimal Cost = Math.Round((decimal)_impex_expense.value / ImpexImportDetails.Sum(x => x.quantity), 2);

                                        //decimal Cost = Impex_CostDetail.unit_cost * coeficient;

                                        //Improve this in future. For now take from Purchase
                                        using (db db = new db())
                                        {
                                            int ID_CurrencyFX_Default = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
                                            decimal DefaultCurrency_Cost = Currency.convert_Values(Cost, sales_invoice.id_currencyfx, ID_CurrencyFX_Default, null);

                                            item_movement_detail.unit_value = DefaultCurrency_Cost;
                                            item_movement_detail.id_currencyfx = ID_CurrencyFX_Default;
                                        }

                                        item_movement_detail.comment = _impex_expense.impex_incoterm_condition.name;
                                        item_movement.item_movement_value.Add(item_movement_detail);
                                    }
                                }
                            }
                            impex.status = Status.Documents_General.Approved;
                            ImpexDB.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            impeximpex_expenseDataGrid.CancelEdit();
            impexViewSource.View.MoveCurrentToFirst();
            ImpexDB.CancelAllChanges();
        }
        #endregion

        private void btnImportInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                foreach (sales_invoice sales_invoice in pnlSalesInvoice.selected_sales_invoice)
                {
                    if (sales_invoice != null)
                    {
                        getProratedCostCounted(sales_invoice, true);
                    }
                }
                productDataGrid.ItemsSource = null;
                impex_ExportDataGrid.ItemsSource = null;
                impex_ExportDataGrid.ItemsSource = Impex_CostDetailLIST;
                productDataGrid.ItemsSource = Impex_ProductsLIST;
            }

        }

        private void impexDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Impex_CostDetailLIST.Clear();
            Impex_ProductsLIST.Clear();
            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                foreach (impex_export impex_export in impex.impex_export)
                {
                    getProratedCostCounted(impex_export.sales_invoice, false);
                }

            }
        }
        private void getProratedCostCounted(sales_invoice sales_invoice, bool isNew)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            if (isNew == true)
            {
                //impex export invoice.
                if (impex.impex_export.Count > 0)
                {
                    //Update
                    impex_export impex_export = impex.impex_export.First();
                    impex_export.id_sales_invoice = sales_invoice.id_sales_invoice;
                    impex_export.sales_invoice = sales_invoice;
                }
                else
                {
                    //Insert
                    impex_export impex_export = new impex_export();
                    impex_export.id_sales_invoice = sales_invoice.id_sales_invoice;
                    impex_export.sales_invoice = sales_invoice;
                    impex.impex_export.Add(impex_export);
                }
            }
            else
            {
                if (sales_invoice == null)
                {
                    //Impex datagrid selection change.
                    impex_export impex_export = impex.impex_export.First();
                    sales_invoice = impex_export.sales_invoice;
                }

            }

            //Get expences
            List<impex_expense> impex_expense = impex.impex_expense.ToList();
            decimal totalExpence = 0;
            foreach (var item in impex_expense)
            {
                totalExpence +=(decimal) item.value;
            }

            //Insert sales Invoice Detail
            if (sales_invoice != null)
            {
                List<sales_invoice_detail> sales_invoice_detail = sales_invoice.sales_invoice_detail.ToList();

                decimal TotalInvoiceAmount = 0;
                foreach (var item in sales_invoice_detail)
                {
                    TotalInvoiceAmount += (item.quantity * item.UnitPrice_Vat);
                }

                if (Impex_ProductsLIST.Where(x => x.id_item == 0).Count() == 0)
                {
                    entity.Class.Impex_Products ImpexImportProductDetails = new entity.Class.Impex_Products();
                    ImpexImportProductDetails.id_item = 0;
                    ImpexImportProductDetails.item = "ALL";
                    Impex_ProductsLIST.Add(ImpexImportProductDetails);
                }
                foreach (sales_invoice_detail _sales_invoice_detail in sales_invoice_detail.Where(x => x.item != null && x.item.item_product != null))
                {
                    int id_item = (int)_sales_invoice_detail.id_item;
                    if (Impex_ProductsLIST.Where(x => x.id_item == id_item).Count() == 0)
                    {
                        entity.Class.Impex_Products ImpexImportProductDetails = new entity.Class.Impex_Products();
                        ImpexImportProductDetails.id_item = (int)_sales_invoice_detail.id_item;
                        ImpexImportProductDetails.item = ImpexDB.items.Where(a => a.id_item == _sales_invoice_detail.id_item).FirstOrDefault().name;
                        Impex_ProductsLIST.Add(ImpexImportProductDetails);
                    }

                    entity.Class.Impex_ItemDetail ImpexImportDetails = new entity.Class.Impex_ItemDetail();
                    ImpexImportDetails.number = _sales_invoice_detail.sales_invoice.number;
                    ImpexImportDetails.id_item = (int)_sales_invoice_detail.id_item;
                    ImpexImportDetails.item = ImpexDB.items.Where(a => a.id_item == _sales_invoice_detail.id_item).FirstOrDefault().name;
                    ImpexImportDetails.quantity = _sales_invoice_detail.quantity;
                    ImpexImportDetails.unit_cost = _sales_invoice_detail.UnitPrice_Vat;
                    ImpexImportDetails.id_invoice = _sales_invoice_detail.id_sales_invoice;
                    ImpexImportDetails.id_invoice_detail = (int)_sales_invoice_detail.id_sales_invoice_detail;
                    if (totalExpence > 0)
                    {
                        //  ImpexImportDetails.prorated_cost = Math.Round(item.unit_cost + (ImpexImportDetails.unit_cost / TotalInvoiceAmount) * totalExpence, 2);
                        ImpexImportDetails.prorated_cost = Math.Round(_sales_invoice_detail.UnitPrice_Vat + (totalExpence / ImpexImportDetails.quantity), 2);
                    }

                    decimal SubTotal = (_sales_invoice_detail.quantity * ImpexImportDetails.prorated_cost);
                    ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
                    Impex_CostDetailLIST.Add(ImpexImportDetails);
                }

            }

        }

      

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as impex_expense != null)
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
                    //DeleteDetailGridRow
                    impeximpex_expenseDataGrid.CancelEdit();
                    ImpexDB.impex_expense.Remove(e.Parameter as impex_expense);
                    impeximpex_expenseViewSource.View.Refresh();
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            try
            {

                if (sbxContact.ContactID > 0)
                {
                    contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                    impex impex = (impex)impexViewSource.View.CurrentItem;
                    impex.contact = contact;

                    if (contact != null)
                    {
                        sales_invoiceViewSource.Source = ImpexDB.sales_invoice.Where(a => a.id_company == company_ID && a.id_contact == contact.id_contact && a.status == Status.Documents_General.Approved && a.is_impex).OrderByDescending(a => a.trans_date).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            List<entity.Class.Impex_ItemDetail> Impex_CostDetail = null;

            foreach (entity.Class.Impex_Products Impex_Products in Impex_ProductsLIST)
            {
                decimal totalExpense = 0;
                decimal totalQuantity = 0;

                if (Impex_Products.id_item == 0)
                {
                    Impex_CostDetail = impex_ExportDataGrid.ItemsSource.OfType<entity.Class.Impex_ItemDetail>().ToList();
                    totalExpense = (decimal)impex.impex_expense.Where(x => x.id_item == 0).Sum(x => x.value);
                    totalQuantity = Impex_CostDetail.Sum(x => x.quantity);
                }
                else
                {
                    Impex_CostDetail = impex_ExportDataGrid.ItemsSource.OfType<entity.Class.Impex_ItemDetail>().ToList().Where(x => x.id_item == Impex_Products.id_item || x.id_item == 0).ToList();
                    totalExpense = (decimal)impex.impex_expense.Where(x => x.id_item == Impex_Products.id_item).Sum(x => x.value);
                    totalQuantity = Impex_CostDetail.Sum(x => x.quantity);
                }

                foreach (entity.Class.Impex_ItemDetail _ImpexImportDetails in Impex_CostDetail)
                {
                    if (totalExpense > 0)
                    {
                        _ImpexImportDetails.prorated_cost = Math.Round(_ImpexImportDetails.unit_cost + (totalExpense / totalQuantity), 2);

                        decimal SubTotal = (_ImpexImportDetails.quantity * _ImpexImportDetails.prorated_cost);
                        _ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
                    }


                }

            }
        }
        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlSalesInvoice = new cntrl.PanelAdv.pnlSalesInvoice();
            pnlSalesInvoice.ImpexDB = ImpexDB;
            impex impex = (impex)impexViewSource.View.CurrentItem;
            //    pnlSalesInvoice.contactViewSource = contactViewSource;
            if (sbxContact.ContactID > 0 || impex.id_contact > 0)
            {
                int id_contact = 0;
                if (sbxContact.ContactID > 0)
                {
                    id_contact = sbxContact.ContactID;
                }
                else
                {
                    id_contact = impex.id_contact;
                }
                contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlSalesInvoice._contact = contact;
            }

            pnlSalesInvoice.SalesInvoice_Click += SalesInvoice_Click;
            crud_modal.Children.Add(pnlSalesInvoice);

        }
        public void SalesInvoice_Click(object sender)
        {
            if (pnlSalesInvoice.selected_sales_invoice.FirstOrDefault() != null)
            {
                if (pnlSalesInvoice.selected_sales_invoice.FirstOrDefault().contact != null)
                {
                    contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                    impex impex = (impex)impexViewSource.View.CurrentItem;
                    impex.contact = contact;

                    sbxContact.Text = contact.name;

                    sales_invoiceViewSource.Source =
                       pnlSalesInvoice.selected_sales_invoice;
                    btnImportInvoice_Click(sender, null);

                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void productDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            entity.Class.Impex_Products clsProductDetail = productDataGrid.SelectedItem as entity.Class.Impex_Products;
            if (impeximpex_expenseViewSource != null)
            {
                if (impeximpex_expenseViewSource.View != null)
                {

                    impeximpex_expenseViewSource.View.Filter = i =>
                    {
                        impex_expense impex_expense = (impex_expense)i;
                        if (impex_expense.id_item == clsProductDetail.id_item)
                            return true;
                        else
                            return false;
                    };

                }
            }
        }

        private void GetExpenses_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {

                if (id_incotermComboBox.SelectedItem != null && pnlSalesInvoice.selected_sales_invoice.FirstOrDefault() != null)
                {
                    impex impex = impexDataGrid.SelectedItem as impex;
                    sales_invoice sales_invoice = pnlSalesInvoice.selected_sales_invoice.FirstOrDefault() as sales_invoice;
                    impex_incoterm impex_incoterm = id_incotermComboBox.SelectedItem as impex_incoterm;
                    List<impex_incoterm_detail> impex_incoterm_detail = null;

                    if (impex.impex_type == entity.impex._impex_type.Import)
                    {
                        //Only fetch buyer expence
                        impex_incoterm_detail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == impex_incoterm.id_incoterm && i.buyer == true).ToList();
                    }

                    if (impex.impex_type == entity.impex._impex_type.Export)
                    {
                        //Only fetch seller expence
                        impex_incoterm_detail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == impex_incoterm.id_incoterm && i.seller == true).ToList();
                    }


                    foreach (entity.Class.Impex_Products product in Impex_ProductsLIST)
                    {
                        foreach (var item in impex_incoterm_detail)
                        {
                            impex_expense impex_expense = new impex_expense();
                            if (ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault() != null)
                            {
                                impex_expense.impex_incoterm_condition = ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault();
                            }
                            impex_expense.id_incoterm_condition = item.id_incoterm_condition;
                            impex_expense.id_currencyfx = sales_invoice.id_currencyfx;
                            impex_expense.id_purchase_invoice = null;
                            impex_expense.id_item = (int)product.id_item;
                            impex.impex_expense.Add(impex_expense);
                        }
                    }

                    impeximpex_expenseViewSource.View.Refresh();
                    productDataGrid.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Please select Incoterm, Type and Invoice to get expenses.", "Get Expences", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

       

    }
}
