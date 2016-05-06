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

namespace Cognitivo.Purchase
{
    public partial class Import : Page
    {
        ImpexDB ImpexDB = new ImpexDB();
        CollectionViewSource impexViewSource, impeximpex_expenseViewSource, purchase_invoiceViewSource = null;
        int company_ID = CurrentSession.Id_Company;
        cntrl.PanelAdv.pnlPurchaseInvoice pnlPurchaseInvoice;
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
                    .Where(x => x.impex_type == impex._impex_type.Import && x.is_active == true && x.id_company == company_ID)
                    .Load();
                impexViewSource.Source = ImpexDB.impex.Local;
                impeximpex_expenseViewSource = FindResource("impeximpex_expenseViewSource") as CollectionViewSource;


                //incotermViewSource
                CollectionViewSource incotermViewSource = FindResource("incotermViewSource") as CollectionViewSource;
                incotermViewSource.Source = await ImpexDB.impex_incoterm.OrderBy(a => a.name).AsNoTracking().ToListAsync();

                CollectionViewSource itemsViewSource = FindResource("itemsViewSource") as CollectionViewSource;
                itemsViewSource.Source = await ImpexDB.items.Where(a => a.is_active == true && a.id_company == company_ID).AsNoTracking().ToListAsync();
                //CurrencyFx
                CollectionViewSource currencyfxViewSource = FindResource("currencyfxViewSource") as CollectionViewSource;
                currencyfxViewSource.Source = await ImpexDB.app_currencyfx.Include("app_currency").AsNoTracking().Where(a => a.is_active == true && a.app_currency.id_company == company_ID).ToListAsync();
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
            purchase_invoiceViewSource.Source = ImpexDB.purchase_invoice.Where(a => a.id_company == company_ID && a.id_contact == 0 && a.is_issued == true).OrderByDescending(a => a.trans_date).ToList();
            impex impex = new impex();
            impex.impex_type = entity.impex._impex_type.Import;
            impex.eta = DateTime.Now;
            impex.etd = DateTime.Now;
            impex.is_active = true;
            // id_pur_invoiceComboBox.SelectedIndex = 0;
            impex.State = EntityState.Added;
            impex.status = Status.Documents_General.Pending;
            impex.IsSelected = true;
            ImpexDB.impex.Add(impex);
            impexViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click_1(object sender)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = ImpexDB.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    ImpexDB.SaveChanges();
                    toolBar.msgSaved();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        //Gets List of Items and Shows it.
        private void btnImportInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                if (pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() != null)
                {
                    purchase_invoice purchase_invoice = pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() as purchase_invoice;
                    getProratedCostCounted(purchase_invoice, true);
                }
            }
        }

        private void impexDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                if (impex.impex_import.Count > 0)
                {
                    purchase_invoice purchase_invoice = null;
                    getProratedCostCounted(purchase_invoice, false);
                }
                else
                {
                    List<Class.clsImpexImportDetails> clsImpexImportDetails = new List<Class.clsImpexImportDetails>();
                    impex_importDataGrid.ItemsSource = clsImpexImportDetails;
                }

                //if (impex.contact != null)
                //    contactComboBox.Text = impex.contact.name;
                //else
                //    contactComboBox.Text = "";
            }
        }

        private void getProratedCostCounted(purchase_invoice purchase_invoice, bool isNew)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            if (isNew == true)
            {
                //impex import invoice.
                if (impex.impex_import.Count > 0)
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
                impex_import impex_import = impex.impex_import.First();
                purchase_invoice = impex_import.purchase_invoice;
            }

            //Get expences
            List<impex_expense> impex_expense = impex.impex_expense.ToList();
            decimal totalExpence = 0;
            foreach (var item in impex_expense)
            {
                totalExpence += item.value;
            }

            //Insert Purchase Invoice Detail
            List<purchase_invoice_detail> purchase_invoice_detail = purchase_invoice.purchase_invoice_detail.ToList();
            List<Class.clsImpexImportDetails> clsImpexImportDetails = new List<Class.clsImpexImportDetails>();
            decimal TotalInvoiceAmount = 0;
            foreach (var item in purchase_invoice_detail)
            {
                TotalInvoiceAmount += (item.quantity * item.UnitCost_Vat);
            }
            foreach (var item in purchase_invoice_detail)
            {
                Class.clsImpexImportDetails ImpexImportDetails = new Class.clsImpexImportDetails();
                ImpexImportDetails.number = item.purchase_invoice.number;
                ImpexImportDetails.id_item = (int)item.id_item;
                ImpexImportDetails.item = ImpexDB.items.Where(a => a.id_item == item.id_item).FirstOrDefault().name;
                ImpexImportDetails.quantity = item.quantity;
                ImpexImportDetails.unit_cost = item.UnitCost_Vat;
                ImpexImportDetails.id_invoice = item.id_purchase_invoice;
                ImpexImportDetails.id_invoice_detail = item.id_purchase_invoice_detail;


                if (totalExpence > 0)
                {
                    //  ImpexImportDetails.prorated_cost = Math.Round(item.unit_cost + (ImpexImportDetails.unit_cost / TotalInvoiceAmount) * totalExpence, 2);
                    ImpexImportDetails.prorated_cost = Math.Round(item.UnitCost_Vat + (totalExpence / ImpexImportDetails.quantity), 2);
                }
                else
                {
                    ImpexImportDetails.prorated_cost = 0;
                }
                decimal SubTotal = (item.quantity * ImpexImportDetails.prorated_cost);
                ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
                clsImpexImportDetails.Add(ImpexImportDetails);
            }
            impex_importDataGrid.ItemsSource = clsImpexImportDetails;
        }

        private void GetExpences_PreviewMouseUp(object sender, EventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                if (id_incotermComboBox.SelectedItem != null && pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() != null)
                {
                    impex impex = impexDataGrid.SelectedItem as impex;
                    purchase_invoice purchase_invoice = pnlPurchaseInvoice.selected_purchase_invoice.FirstOrDefault() as purchase_invoice;
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
                    if (impex.impex_expense != null)
                        impex.impex_expense.Clear();
                    foreach (var item in impex_incoterm_detail)
                    {
                        impex_expense impex_expense = new impex_expense();
                        if ( ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault()!=null)
                        {
                            impex_expense.impex_incoterm_condition = ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault();    
                        }
                        
                        impex_expense.id_incoterm_condition = item.id_incoterm_condition;
                        impex_expense.id_currencyfx = purchase_invoice.id_currencyfx;
                        impex_expense.id_purchase_invoice = purchase_invoice.id_purchase_invoice;
                        impex.impex_expense.Add(impex_expense);
                    }
                    impeximpex_expenseViewSource.View.Refresh();
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
                    List<Class.clsImpexImportDetails> ImpexImportDetails = (List<Class.clsImpexImportDetails>)impex_importDataGrid.ItemsSource;
                    if (ImpexImportDetails.Count > 0)
                    {
                        //To make sure we have a Purchase Total
                        decimal purchaseTotal = ImpexImportDetails.Sum(i => i.sub_total);
                        if (purchaseTotal != 0)
                        {
                            foreach (Class.clsImpexImportDetails detail in ImpexImportDetails)
                            {
                                //Get total value of a Product Row
                                decimal itemTotal = detail.quantity * detail.unit_cost;

                                purchase_invoice purchase_invoice = ImpexDB.purchase_invoice.Where(x => x.id_purchase_invoice == detail.id_invoice).FirstOrDefault();
                                item_movement item_movement = ImpexDB.item_movement.Where(x => x.id_purchase_invoice_detail == detail.id_invoice_detail).FirstOrDefault();

                                foreach (impex_expense _impex_expense in impex_expenses)
                                {
                                    decimal condition_value = _impex_expense.value;
                                    if (condition_value != 0 && itemTotal != 0)
                                    {
                                        //Coeficient is used to get prorated cost of one item
                                        decimal coeficient = condition_value / itemTotal;
                                        item_movement_value item_movement_detail = new item_movement_value();
                                        item_movement_detail.unit_value = detail.unit_cost * coeficient;
                                        //Improve this in future. For now take from Purchase
                                        item_movement_detail.id_currencyfx = purchase_invoice.id_currencyfx;
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

        private void btn_Calculate(object sender, RoutedEventArgs e)
        {
            //impeximpex_expenseViewSource.
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            impex impex = impexDataGrid.SelectedItem as impex;
            decimal totalExpence = impex.impex_expense.Sum(x => x.value);
            List<Class.clsImpexImportDetails> ImpexImportDetails = (List<Class.clsImpexImportDetails>)impex_importDataGrid.ItemsSource;
            foreach (Class.clsImpexImportDetails _ImpexImportDetails in ImpexImportDetails)
            {
                if (totalExpence > 0)
                {
                    _ImpexImportDetails.prorated_cost = Math.Round(_ImpexImportDetails.unit_cost + (totalExpence / _ImpexImportDetails.quantity), 2);
                }
                else
                {
                    _ImpexImportDetails.prorated_cost = 0;
                }

                decimal SubTotal = (_ImpexImportDetails.quantity * _ImpexImportDetails.prorated_cost);
                _ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
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
                        purchase_invoiceViewSource.Source =
                            ImpexDB.purchase_invoice
                            .Where(a =>
                                   a.id_company == company_ID
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

            if (sbxContact.ContactID > 0)
            {
                contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlPurchaseInvoice._contact = contact;
                pnlPurchaseInvoice.IsImpex = true;
            }

            pnlPurchaseInvoice.PurchaseInvoice_Click += PurchaseInvoice_Click;
            crud_modal.Children.Add(pnlPurchaseInvoice);
        }

        public void PurchaseInvoice_Click(object sender)
        {
            contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

            impex impex = (impex)impexViewSource.View.CurrentItem;
            impex.contact = contact;

            sbxContact.Text = contact.name;
            if (contact != null)
            {
                purchase_invoiceViewSource.Source =
                   pnlPurchaseInvoice.selected_purchase_invoice;
                btnImportInvoice_Click(sender, null);
            }
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
        }
    }
}
