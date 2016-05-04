using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;

namespace Cognitivo.Sales
{
    /// <summary>
    /// Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : Page
    {
        ImpexDB ImpexDB = new ImpexDB();
        CollectionViewSource impexViewSource, impeximpex_expenseViewSource, sales_invoiceViewSource = null;
        cntrl.PanelAdv.pnlSalesInvoice pnlSalesInvoice;
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
            ImpexDB.impex.Add(impex);
            impexViewSource.View.MoveCurrentToLast();
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

        //private void toolBar_btnApprove_Click(object sender)
        //{
        //    if (impexDataGrid.SelectedItem != null)
        //    {
        //        impex impex = impexDataGrid.SelectedItem as impex;
        //        int id = impex.id_impex;
        //        entity.Brillo.ImpexBrillo ImpexBrillo = new entity.Brillo.ImpexBrillo();
        //        ImpexBrillo.Impex_Update((int)entity.App.Names.SalesInvoice, id);
        //        toolBar.msgDone();
        //    }
        //}
        private void toolBar_btnApprove_Click(object sender)
        {
            try
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                List<impex_expense> impex_expenses = impex.impex_expense.ToList();
                List<Class.clsImpexImportDetails> ImpexImportDetails = (List<Class.clsImpexImportDetails>)impex_ExportDataGrid.ItemsSource;
                if (ImpexImportDetails.Count > 0)
                {
                    //To make sure we have a Purchase Total
                    decimal SalesTotal = ImpexImportDetails.Sum(i => i.sub_total);
                    if (SalesTotal != 0)
                    {
                        foreach (Class.clsImpexImportDetails detail in ImpexImportDetails)
                        {
                            //Get total value of a Product Row
                            decimal itemTotal = detail.quantity * detail.unit_cost;

                            sales_invoice sales_invoice = ImpexDB.sales_invoice.Where(x => x.id_sales_invoice == detail.id_invoice).FirstOrDefault();
                            item_movement item_movement = ImpexDB.item_movement.Where(x => x.id_sales_invoice == detail.id_invoice).FirstOrDefault();

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
                                    item_movement_detail.id_currencyfx = sales_invoice.id_currencyfx;
                                    item_movement_detail.comment = _impex_expense.impex_incoterm_condition.name;
                                    item_movement.item_movement_value.Add(item_movement_detail);

                                    ImpexDB.SaveChanges();
                                }
                            }
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
                if (pnlSalesInvoice.selected_sales_invoice.FirstOrDefault() != null)
                {
                    sales_invoice sales_invoice = pnlSalesInvoice.selected_sales_invoice.FirstOrDefault() as sales_invoice;
                    getProratedCostCounted(sales_invoice, true);
                }
            }
        }

        private void impexDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                if (impex.impex_export.Count > 0)
                {
                    sales_invoice sales_invoice = null;
                    getProratedCostCounted(sales_invoice, false);
                }
                else
                {
                    List<Class.clsImpexImportDetails> clsImpexImportDetails = new List<Class.clsImpexImportDetails>();
                    impex_ExportDataGrid.ItemsSource = clsImpexImportDetails;
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
                //Impex datagrid selection change.
                impex_export impex_export = impex.impex_export.First();
                sales_invoice = impex_export.sales_invoice;
            }

            //Get expences
            List<impex_expense> impex_expense = impex.impex_expense.ToList();
            decimal totalExpence = 0;
            foreach (var item in impex_expense)
            {
                totalExpence += item.value;
            }

            //Insert sales Invoice Detail
            List<sales_invoice_detail> sales_invoice_detail = sales_invoice.sales_invoice_detail.ToList();
            List<Class.clsImpexImportDetails> clsImpexImportDetails = new List<Class.clsImpexImportDetails>();
            decimal TotalInvoiceAmount = 0;
            foreach (var item in sales_invoice_detail)
            {
                TotalInvoiceAmount += (item.quantity * item.UnitPrice_Vat);
            }

            foreach (var item in sales_invoice_detail)
            {
                Class.clsImpexImportDetails ImpexImportDetails = new Class.clsImpexImportDetails();
                ImpexImportDetails.number = item.sales_invoice.number;
                ImpexImportDetails.id_item = (int)item.id_item;
                ImpexImportDetails.item = item.item.name;
                ImpexImportDetails.quantity = item.quantity;
                ImpexImportDetails.unit_cost = item.UnitPrice_Vat;
                ImpexImportDetails.id_invoice = item.id_sales_invoice;

                if (totalExpence > 0)
                {
                    //  ImpexImportDetails.prorated_cost = Math.Round(item.unit_cost + (ImpexImportDetails.unit_cost / TotalInvoiceAmount) * totalExpence, 2);
                    ImpexImportDetails.prorated_cost = Math.Round(item.UnitPrice_Vat + (totalExpence / ImpexImportDetails.quantity), 2);
                }
                else
                {
                    ImpexImportDetails.prorated_cost = 0;
                }
                decimal SubTotal = (item.quantity * ImpexImportDetails.prorated_cost);
                ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
                clsImpexImportDetails.Add(ImpexImportDetails);
            }
            impex_ExportDataGrid.ItemsSource = clsImpexImportDetails;
        }

        private void GetExpences_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (impexDataGrid.SelectedItem != null)
            {
                impex impex = impexDataGrid.SelectedItem as impex;
                if (id_incotermComboBox.SelectedItem != null && pnlSalesInvoice.selected_sales_invoice.FirstOrDefault() != null)
                {
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

                    if (impex.impex_expense != null)
                    {
                        impex.impex_expense.Clear();
                    }

                    foreach (var item in impex_incoterm_detail)
                    {
                        impex_expense impex_expense = new impex_expense();

                        impex_expense.id_incoterm_condition = item.id_incoterm_condition;
                        impex_expense.id_currencyfx = sales_invoice.id_currencyfx;
                        impex_expense.id_purchase_invoice = null;
                        impex.impex_expense.Add(impex_expense);
                    }

                    impeximpex_expenseViewSource.View.Refresh();
                }
                else
                {
                    MessageBox.Show("Please select Incoterm, Type and Invoice to get expenses.", "Get Expences", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        sales_invoiceViewSource.Source = ImpexDB.sales_invoice.Where(a => a.id_company == company_ID && a.id_contact == contact.id_contact && a.is_issued == true).OrderByDescending(a => a.trans_date).ToList();
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
            decimal totalExpence = impex.impex_expense.Sum(x => x.value);
            List<Class.clsImpexImportDetails> ImpexImportDetails = (List<Class.clsImpexImportDetails>)impex_ExportDataGrid.ItemsSource;
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
        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlSalesInvoice = new cntrl.PanelAdv.pnlSalesInvoice();
            pnlSalesInvoice._entity = ImpexDB;
            //    pnlSalesInvoice.contactViewSource = contactViewSource;
            if (sbxContact.ContactID > 0)
            {
                contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                pnlSalesInvoice._contact = contact;
            }

            pnlSalesInvoice.SalesInvoice_Click += SalesInvoice_Click;
            crud_modal.Children.Add(pnlSalesInvoice);

        }
        public void SalesInvoice_Click(object sender)
        {
            contact contact = ImpexDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

            impex impex = (impex)impexViewSource.View.CurrentItem;
            impex.contact = contact;

            sbxContact.Text = contact.name;
            if (contact != null)
            {
                sales_invoiceViewSource.Source =
                   pnlSalesInvoice.selected_sales_invoice;
                btnImportInvoice_Click(sender, null);
            }
            crud_modal.Children.Clear();
            crud_modal.Visibility = Visibility.Collapsed;
        }

    }
}
