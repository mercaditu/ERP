using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;
using entity.Brillo;

namespace Cognitivo.Purchase
{
    public partial class Import : Page
    {
        ImpexDB ImpexDB = new ImpexDB();
        CollectionViewSource impexViewSource, impeximpex_expenseViewSource, purchase_invoiceViewSource = null;
        cntrl.PanelAdv.pnlPurchaseInvoice pnlPurchaseInvoice= new cntrl.PanelAdv.pnlPurchaseInvoice();

        List<Class.Impex_CostDetail> Impex_CostDetailLIST = new List<Class.Impex_CostDetail>();
        List<Class.Impex_Products> Impex_ProductsLIST = new List<Class.Impex_Products>();

        public Import()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //purchase_invoiceViewSource
                purchase_invoiceViewSource = FindResource("purchase_invoiceViewSource") as CollectionViewSource;

                impexViewSource = FindResource("impexViewSource") as CollectionViewSource;
                ImpexDB.impex
                    .Include(x => x.impex_import)
                    .Include(x => x.impex_expense)
                    .Where(x => x.impex_type == impex._impex_type.Import && x.is_active == true && x.id_company == CurrentSession.Id_Company)
                    .Load();
                impexViewSource.Source = ImpexDB.impex.Local;
                impeximpex_expenseViewSource = FindResource("impeximpex_expenseViewSource") as CollectionViewSource;


                //incotermViewSource
                CollectionViewSource incotermViewSource = FindResource("incotermViewSource") as CollectionViewSource;
                incotermViewSource.Source = await ImpexDB.impex_incoterm.OrderBy(a => a.name).AsNoTracking().ToListAsync();

                CollectionViewSource itemsViewSource = FindResource("itemsViewSource") as CollectionViewSource;
                itemsViewSource.Source = await ImpexDB.items.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).AsNoTracking().ToListAsync();
                //CurrencyFx
                CollectionViewSource currencyfxViewSource = FindResource("currencyfxViewSource") as CollectionViewSource;
                currencyfxViewSource.Source = await ImpexDB.app_currencyfx.Include("app_currency").AsNoTracking().Where(a => a.is_active == true && a.app_currency.id_company == CurrentSession.Id_Company).ToListAsync();
                //incotermconditionViewSource
                CollectionViewSource incotermconditionViewSource = FindResource("incotermconditionViewSource") as CollectionViewSource;
                incotermconditionViewSource.Source = await ImpexDB.impex_incoterm_condition.OrderBy(a => a.name).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnCancel_Click_1(object sender)
        {
            impeximpex_expenseDataGrid.CancelEdit();
            impexViewSource.View.MoveCurrentToFirst();
            ImpexDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click_1(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                ImpexDB.impex.Remove(impex);
                toolBar_btnSave_Click_1(sender);
            }
        }

        private void toolBar_btnEdit_Click_1(object sender)
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

        private void toolBar_btnNew_Click_1(object sender)
        {
            purchase_invoiceViewSource.Source = ImpexDB.purchase_invoice.Where(a => a.id_company == CurrentSession.Id_Company && a.id_contact == 0 && a.is_issued == true).OrderByDescending(a => a.trans_date).ToList();
            impex impex = new impex();
            impex.impex_type = entity.impex._impex_type.Import;
            impex.eta = DateTime.Now;
            impex.etd = DateTime.Now;
            impex.is_active = true;
            impex.State = EntityState.Added;
            impex.status = Status.Documents_General.Pending;
            impex.IsSelected = true;
            ImpexDB.impex.Add(impex);
            impexViewSource.View.MoveCurrentToLast();
            Impex_CostDetailLIST.Clear();
        }

        private void toolBar_btnSave_Click_1(object sender)
        {
            impex impex = impexDataGrid.SelectedItem as impex;

            if (ImpexDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ImpexDB.NumberOfRecords);
            }
        }

        //Gets List of Items and Shows it.
        private void btnImportInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                foreach (purchase_invoice purchase_invoice in pnlPurchaseInvoice.selected_purchase_invoice)
                {
                    if (purchase_invoice != null)
                    {
                        getProratedCostCounted(purchase_invoice, true);
                    }
                }
                productDataGrid.ItemsSource = null;
                impex_importDataGrid.ItemsSource = null;
                impex_importDataGrid.ItemsSource = Impex_CostDetailLIST;
                productDataGrid.ItemsSource = Impex_ProductsLIST;
            }
        }

        private void impexDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Impex_CostDetailLIST.Clear();

            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                foreach (impex_import impex_import in impex.impex_import)
                {
                    getProratedCostCounted(impex_import.purchase_invoice, false); 
                }
            }

            productDataGrid.ItemsSource = null;
            impex_importDataGrid.ItemsSource = null;
            impex_importDataGrid.ItemsSource = Impex_CostDetailLIST;
            productDataGrid.ItemsSource = Impex_ProductsLIST;
        }

        private void getProratedCostCounted(purchase_invoice purchase_invoice, bool isNew)
        {
            impex impex = impexDataGrid.SelectedItem as impex;

            if (isNew == true)
            {
                //impex import invoice.
                if (impex.impex_import.Where(x => x.id_purchase_invoice == purchase_invoice.id_purchase_invoice).Count() > 0)
                {
                    //Update
                    impex_import impex_import = impex.impex_import.First();
                    impex_import.id_purchase_invoice = purchase_invoice.id_purchase_invoice;
                    impex_import.purchase_invoice = purchase_invoice;
                }
                else
                {
                    //Insert
                    impex_import impex_import = new impex_import();
                    impex_import.id_purchase_invoice = purchase_invoice.id_purchase_invoice;
                    impex_import.purchase_invoice = purchase_invoice;
                    impex.impex_import.Add(impex_import);
                }
            }
            else
            {
                //Impex datagrid selection change.
                if (purchase_invoice==null)
                {
                    impex_import impex_import = impex.impex_import.First();
                    purchase_invoice = impex_import.purchase_invoice;
                }
              
            }


            //Get expences
            List<impex_expense> impex_expense = impex.impex_expense.ToList();
            decimal totalExpense = 0;

            foreach (var item in impex_expense)
            {
                totalExpense += item.value;
            }

            if (purchase_invoice != null)
            {
                //Insert Purchase Invoice Detail
                List<purchase_invoice_detail> purchase_invoice_detail = purchase_invoice.purchase_invoice_detail.ToList();

                decimal TotalInvoiceAmount = 0;

                foreach (var item in purchase_invoice_detail)
                {
                    TotalInvoiceAmount += (item.quantity * item.UnitCost_Vat);
                }

                if (Impex_ProductsLIST.Where(x => x.id_item == 0).Count() == 0)
                {
                    Class.Impex_Products ImpexImportProductDetails = new Class.Impex_Products();
                    ImpexImportProductDetails.id_item = 0;
                    ImpexImportProductDetails.item = "ALL";
                    Impex_ProductsLIST.Add(ImpexImportProductDetails);
                }

                foreach (purchase_invoice_detail _purchase_invoice_detail in purchase_invoice_detail.Where(x => x.item != null && x.item.item_product != null))
                {
                    int id_item = (int)_purchase_invoice_detail.id_item;

                    if (Impex_ProductsLIST.Where(x => x.id_item == id_item).Count() == 0)
                    {
                        Class.Impex_Products ImpexImportProductDetails = new Class.Impex_Products();
                        ImpexImportProductDetails.id_item = (int)_purchase_invoice_detail.id_item;
                        ImpexImportProductDetails.item = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().name;
                        Impex_ProductsLIST.Add(ImpexImportProductDetails);
                    }

                    Class.Impex_CostDetail ImpexImportDetails = new Class.Impex_CostDetail();
                    ImpexImportDetails.number = _purchase_invoice_detail.purchase_invoice.number;
                    ImpexImportDetails.id_item = (int)_purchase_invoice_detail.id_item;
                    ImpexImportDetails.item = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().name;
                    ImpexImportDetails.quantity = _purchase_invoice_detail.quantity;
                    ImpexImportDetails.unit_cost = _purchase_invoice_detail.unit_cost;
                    ImpexImportDetails.id_invoice = _purchase_invoice_detail.id_purchase_invoice;
                    ImpexImportDetails.id_invoice_detail = _purchase_invoice_detail.id_purchase_invoice_detail;

                    if (totalExpense > 0)
                    {
                        ImpexImportDetails.prorated_cost = Math.Round(_purchase_invoice_detail.unit_cost + (totalExpense / purchase_invoice_detail.Sum(x => x.quantity)), 2);
                    }

                    decimal SubTotal = (_purchase_invoice_detail.quantity * ImpexImportDetails.prorated_cost);
                    ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
                    Impex_CostDetailLIST.Add(ImpexImportDetails);
                }
            }
        }

        private void GetExpenses_PreviewMouseUp(object sender, EventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                if (id_incotermComboBox.SelectedItem != null && pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() != null)
                {
                    impex impex = impexDataGrid.SelectedItem as impex;
                    purchase_invoice PurchaseInvoice = pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() as purchase_invoice;
                    impex_incoterm Incoterm = id_incotermComboBox.SelectedItem as impex_incoterm;
                    List<impex_incoterm_detail> IncotermDetail = null;

                    if (impex.impex_type == entity.impex._impex_type.Import)
                    {
                        //Only fetch buyer expence
                        IncotermDetail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == Incoterm.id_incoterm && i.buyer == true).ToList();
                    }
                    if (impex.impex_type == entity.impex._impex_type.Export)
                    {
                        //Only fetch seller expence
                        IncotermDetail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == Incoterm.id_incoterm && i.seller == true).ToList();
                    }

                    foreach (Class.Impex_Products product in Impex_ProductsLIST)
                    {
                        foreach (var item in IncotermDetail)
                        {
                            impex_expense impex_expense = new impex_expense();
                            impex_expense.State = EntityState.Added;

                            if (ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault() != null)
                            {
                                impex_expense.impex_incoterm_condition = ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault();
                            }

                            impex_expense.id_incoterm_condition = item.id_incoterm_condition;
                            impex_expense.id_currencyfx = PurchaseInvoice.id_currencyfx;
                            impex_expense.id_purchase_invoice = PurchaseInvoice.id_purchase_invoice;
                            impex_expense.id_item = (int)product.id_item;
                            impex.impex_expense.Add(impex_expense);
                        }
                    }
                    impeximpex_expenseViewSource.View.Refresh();
                    productDataGrid.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Please select Incoterm, Type and Invoice to get expences.", "Get Expences", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
                    List<Class.Impex_CostDetail> ImpexImportDetails = (List<Class.Impex_CostDetail>)impex_importDataGrid.ItemsSource;
                    if (ImpexImportDetails.Count > 0)
                    {
                        //To make sure we have a Purchase Total
                        decimal purchaseTotal = ImpexImportDetails.Sum(i => i.sub_total);
                        if (purchaseTotal != 0)
                        {
                            foreach (Class.Impex_CostDetail Impex_CostDetail in ImpexImportDetails)
                            {
                                //Get total value of a Product Row
                                decimal itemTotal = Impex_CostDetail.quantity * Impex_CostDetail.unit_cost;

                                purchase_invoice purchase_invoice = ImpexDB.purchase_invoice.Where(x => x.id_purchase_invoice == Impex_CostDetail.id_invoice).FirstOrDefault();
                                item_movement item_movement = ImpexDB.item_movement.Where(x => x.id_purchase_invoice_detail == Impex_CostDetail.id_invoice_detail).FirstOrDefault();

                                foreach (impex_expense _impex_expense in impex_expenses)
                                {
                                    decimal condition_value = _impex_expense.value;

                                    if (condition_value != 0 && itemTotal != 0)
                                    {
                                        //Coeficient is used to get prorated cost of one item
                                        item_movement_value item_movement_detail = new item_movement_value();

                                        decimal Cost = Math.Round(_impex_expense.value / ImpexImportDetails.Sum(x => x.quantity), 2);

                                        //decimal Cost = Impex_CostDetail.unit_cost * coeficient;

                                        //Improve this in future. For now take from Purchase
                                        using (db db = new db())
                                        {
                                            int ID_CurrencyFX_Default = Currency.get_Default(db).app_currencyfx.Where(x => x.is_active).FirstOrDefault().id_currencyfx;
                                            decimal DefaultCurrency_Cost = Currency.convert_Values(Cost, purchase_invoice.id_currencyfx, ID_CurrencyFX_Default, null);

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

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as impex_expense != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                //DeleteDetailGridRow
                impeximpex_expenseDataGrid.CancelEdit();
                ImpexDB.impex_expense.Remove(e.Parameter as impex_expense);
                impeximpex_expenseViewSource.View.Refresh();
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            List<Class.Impex_CostDetail> Impex_CostDetail = null;
            foreach (Class.Impex_Products Impex_Products in Impex_ProductsLIST)
            {
                decimal totalExpense = 0;
                decimal totalQuantity = 0;

                if (Impex_Products.id_item == 0)
                {
                    Impex_CostDetail = impex_importDataGrid.ItemsSource.OfType<Class.Impex_CostDetail>().ToList();
                    totalExpense = impex.impex_expense.Where(x => x.id_item == 0).Sum(x => x.value);
                    totalQuantity = Impex_CostDetail.Sum(x => x.quantity);
                }
                else
                {
                    Impex_CostDetail = impex_importDataGrid.ItemsSource.OfType<Class.Impex_CostDetail>().ToList().Where(x => x.id_item == Impex_Products.id_item || x.id_item == 0).ToList();
                    totalExpense = impex.impex_expense.Where(x => x.id_item == Impex_Products.id_item).Sum(x => x.value);
                    totalQuantity = Impex_CostDetail.Sum(x => x.quantity);
                }

                foreach (Class.Impex_CostDetail _ImpexImportDetails in Impex_CostDetail)
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

        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            try
            {
                contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                if (contact != null)
                {
                    impex impex = (impex)impexViewSource.View.CurrentItem;
                    impex.contact = contact;

                    if (contact != null)
                    {
                        purchase_invoiceViewSource.Source =
                            ImpexDB.purchase_invoice
                            .Where(a =>
                                   a.id_company == CurrentSession.Id_Company
                                && a.is_impex
                                && a.id_contact == contact.id_contact
                                && a.status == Status.Documents_General.Approved)
                            .OrderByDescending(a => a.trans_date)
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlPurchaseInvoice = new cntrl.PanelAdv.pnlPurchaseInvoice();
            pnlPurchaseInvoice._entity = ImpexDB;
            impex impex = (impex)impexViewSource.View.CurrentItem;
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
                contact contact = ImpexDB.contacts.Where(x => x.id_contact == id_contact).FirstOrDefault();
                pnlPurchaseInvoice._contact = contact;
                pnlPurchaseInvoice.IsImpex = true;
            }

            pnlPurchaseInvoice.PurchaseInvoice_Click += PurchaseInvoice_Click;
            crud_modal.Children.Add(pnlPurchaseInvoice);
        }

        public void PurchaseInvoice_Click(object sender)
        {
            if (pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() != null)
            {
                if (pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault().contact != null)
                {
                    contact contact = pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault().contact;

                    impex impex = (impex)impexViewSource.View.CurrentItem;
                    impex.contact = contact;

                    sbxContact.Text = contact.name;
                    purchase_invoiceViewSource.Source =
                    pnlPurchaseInvoice.selected_purchase_invoice;
                    btnImportInvoice_Click(sender, null);

                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void productDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Class.Impex_Products clsProductDetail = productDataGrid.SelectedItem as Class.Impex_Products;
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








    }
}
