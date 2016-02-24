using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Cognitivo.Sales
{
    /// <summary>
    /// Interaction logic for Return.xaml
    /// </summary>
    public partial class Return : Page
    {
        SalesReturnDB dbContext = new SalesReturnDB();
        CollectionViewSource salesReturnViewSource, sales_invoiceViewSource, sales_returnsales_return_detailViewSource;
        cntrl.PanelAdv.pnlSalesInvoice pnlSalesInvoice;
     
        public Return()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.Properties.Settings _entity = new entity.Properties.Settings();


                salesReturnViewSource = (CollectionViewSource)FindResource("sales_returnViewSource");
                dbContext.sales_return.Where(a => a.id_company == _entity.company_ID).Load();
                salesReturnViewSource.Source = dbContext.sales_return.Local;
                sales_returnsales_return_detailViewSource = FindResource("sales_returnsales_return_detailViewSource") as CollectionViewSource;

                sales_invoiceViewSource = (CollectionViewSource)FindResource("sales_invoiceViewSource");
                sales_invoiceViewSource.Source = dbContext.sales_invoice.Where(a => a.status == Status.Documents_General.Approved && a.id_company == _entity.company_ID).ToList();

           

                CollectionViewSource currencyfxViewSource = (CollectionViewSource)FindResource("app_currencyfxViewSource");
                dbContext.app_currencyfx.Include("app_currency").Where(x => x.app_currency.id_company == _entity.company_ID).Load();
                currencyfxViewSource.Source = dbContext.app_currencyfx.Local;

              
                CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
                app_vat_groupViewSource.Source = dbContext.app_vat_group.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

                dbContext.app_contract.Where(a => a.is_active == true && a.id_company == _entity.company_ID).ToList();

                cbxContract.ItemsSource = dbContext.app_contract.Local;

                cbxReturnType.ItemsSource = Enum.GetValues(typeof(Status.ReturnTypes));

                dbContext.app_condition.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).ToList();
                cbxCondition.ItemsSource = dbContext.app_condition.Local;


                //dbContext.app_document_range.Where(d => d.is_active == true
                //                          && d.app_document.id_application == entity.App.Names.SalesReturn
                //                          && d.id_company == _entity.company_ID).Include(i => i.app_document).ToList();

                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesReturn, _entity.branch_ID, _entity.terminal_ID);

            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #region Toolbar
        private void toolBar_btnCancel_Click(object sender)
        {
            sales_return_detailDataGrid.CancelEdit();
            salesReturnViewSource.View.MoveCurrentToFirst();
            dbContext.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    dbContext.sales_return.Remove((sales_return)sales_returnDataGrid.SelectedItem);
                    salesReturnViewSource.View.MoveCurrentToFirst();
                    toolBar_btnSave_Click(sender);
                }
            }

            catch (Exception ex)
            {
                toolBar.msgError(ex);
                //throw ex;
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (sales_returnDataGrid.SelectedItem != null)
            {
                sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
                sales_return.IsSelected = true;
                sales_return.State = System.Data.Entity.EntityState.Modified;
                dbContext.Entry(sales_return).State = EntityState.Modified;
            }

            else
            {
                toolBar.msgWarning("Please Select A Record");

            }
        }
        private void toolBar_btnNew_Click(object sender)
        {
            sales_return objSalRtn = dbContext.New() ;
         
            dbContext.sales_return.Add(objSalRtn);
            salesReturnViewSource.View.MoveCurrentToLast();
        }
        private void toolBar_btnSave_Click(object sender)
        {
            dbContext.SaveChanges();
        }
        #endregion

        //private void ButtonImport_Click(object sender, RoutedEventArgs e)
        //{
        //    if (cbxSalesInvoice.SelectedItem != null)
        //    {
        //        sales_invoice sales_invoice = cbxSalesInvoice.SelectedItem as sales_invoice;
        //        if (sales_returnDataGrid.SelectedItem != null)
        //        {
        //            sales_return sales_return = sales_returnDataGrid.SelectedItem as sales_return;
        //            foreach (var item in sales_invoice.sales_invoice_detail)
        //            {
        //                sales_return_detail sales_return_detail = new sales_return_detail();
        //                sales_return_detail.sales_return = sales_return;
        //                sales_return_detail.id_location = item.id_location;
        //                sales_return_detail.app_location = item.app_location;
        //                sales_return_detail.item = item.item;
        //                sales_return_detail.item_description = item.item.description;
        //                sales_return_detail.id_item = item.id_item;
        //                sales_return_detail.unit_price = item.unit_price;
        //                sales_return_detail.unit_cost = item.unit_cost;
        //                sales_return_detail.id_sales_invoice_detail = (int)item.id_sales_invoice_detail;
        //                sales_return_detail.quantity = item.quantity;
        //                sales_return.sales_invoice = sales_invoice;
        //                sales_return.sales_return_detail.Add(sales_return_detail);
        //            }
        //            //calculate_total(sender, e);
        //            calculate_vat(sender, e);
        //            sales_returnsales_return_detailViewSource.View.Refresh();
        //        }
        //    }
        //}

        private void calculate_vat(object sender, EventArgs e)
        {
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
            //List<sales_return_vat> deletesales_return_detail_vat = entity.db.sales_return_detail_vat.Local.Where(x => x.sales_return_detail == null).ToList();
            //List<sales_return_vat> sales_return_detail_vat = entity.db.sales_return_detail_vat.Local.Where(x => x.sales_return_detail != null && x.id_sales_return_detail_vat == 0).ToList();
            //entity.db.sales_return_detail_vat.RemoveRange(deletesales_return_detail_vat);
            //sales_return_detail_vat = sales_return_detail_vat.Where(x => x.sales_return_detail.sales_return == sales_return).ToList();
            //dgvvat.ItemsSource = sales_return_detail_vat
            //                        .Join(entity.db.app_vat, ad => ad.id_vat, cfx => cfx.id_vat
            //       , (ad, cfx) => new { name = cfx.name, value = ad.unit_value, id_vat = ad.id_vat, ad.sales_return_detail })
            //       .GroupBy(a => new { a.name, a.id_vat, a.sales_return_detail })
            //       .Select(g => new
            //       {
            //           id_vat = g.Key.id_vat,
            //           name = g.Key.name,
            //           value = g.Sum(a => a.value * a.sales_return_detail.quantity)
            //       }).ToList();
            List<sales_return_detail> sales_return_detail = sales_return.sales_return_detail.ToList();
            dgvvat.ItemsSource = sales_return_detail
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
        //    //salesReturnViewSource.View.Refresh();
        //    sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
        //    if (sales_return != null)
        //    {
        //        sales_return.get_Sales_return_Total();
        //    }
        //}

        private void sales_return_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        //private void sales_returnDataGrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    calculate_total(sender, e);
        //}

        private void sales_returnDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
            if (sales_return != null)
            {
                calculate_vat(sender, e);
              
            }
            //calculate_total(sender, e);
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    salesReturnViewSource.View.Filter = i =>
                    {
                        sales_return sales_return = i as sales_return;
                        if (sales_return.contact.name.ToLower().Contains(query.ToLower()))
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
                    salesReturnViewSource.View.Filter = null;
                }
            }
            catch (Exception)
            { }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as sales_return_detail != null)
            {
                //sales_return_detail sales_return_detail = e.Parameter as sales_return_detail;
                //if (string.IsNullOrEmpty(sales_return_detail.Error))
                //{
                e.CanExecute = true;
                //}
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {

                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    sales_return sales_return = salesReturnViewSource.View.CurrentItem as sales_return;
                    //DeleteDetailGridRow
                    sales_return_detailDataGrid.CancelEdit();
                    dbContext.sales_return_detail.Remove(e.Parameter as sales_return_detail);
                    sales_returnsales_return_detailViewSource.View.Refresh();
                    //calculate_total(sender, e);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            try
            {
                if (sbxContact.ContactID > 0)
                {
                    contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;
                    sales_return.id_contact = contact.id_contact;
                    sales_return.contact = contact;
                  

                    ///Start Thread to get Data.
                    Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private async void set_ContactPref_Thread(contact objContact)
        {
            if (objContact != null)
            {
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    //Condition
                    if (objContact.app_contract != null)
                        cbxCondition.SelectedValue = objContact.app_contract.id_condition;
                    //Contract
                    if (objContact.id_contract != null)
                        cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);
                    //Currency
                    cbxCurrency.get_ActiveRateXContact(ref objContact);
                }));

                //await SalesInvoiceDB.projects.Where(a => a.is_active == true && a.id_company == company_ID && a.id_contact == objContact.id_contact).OrderBy(a => a.name).ToListAsync();
                await Dispatcher.InvokeAsync(new Action(() =>
                {
                    //cbxProject.ItemsSource = SalesInvoiceDB.projects.Local;
                }));
            }
        }

        //private void contactComboBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (contactComboBox.Data != null)
        //    {
        //        contact contact = contactComboBox.Data as contact;
        //        if (contact != null)
        //        {
        //            sales_invoiceViewSource.View.Filter = a =>
        //            {
        //                sales_invoice sales_invoice = a as sales_invoice;
        //                if (sales_invoice.id_contact == contact.id_contact)
        //                    return true;
        //                else
        //                    return false;
        //            };
        //        }
        //    }
        //}

        //private void contactComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (contactComboBox.Data != null)
        //    {
        //        contact contact = contactComboBox.Data as contact;
        //        if (contact != null)
        //        {
        //            contactComboBox.focusGrid = false;
        //            contactComboBox.Text = contact.name;
        //            sales_invoiceViewSource.View.Filter = a =>
        //            {
        //                sales_invoice sales_invoice = a as sales_invoice;
        //                if (sales_invoice.id_contact == contact.id_contact)
        //                    return true;
        //                else
        //                    return false;
        //            };
        //        }
        //    }
        //}

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                sales_return sales_return = salesReturnViewSource.View.CurrentItem as sales_return;
                item item = dbContext.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (item != null && item.id_item > 0 && sales_return != null)
                {
                    Task Thread = Task.Factory.StartNew(() => select_Item(sales_return, item));
                }
            }
        }

        private void select_Item(sales_return sales_return, item item)
        {
            if (sales_return.sales_return_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null)
            {
                sales_return_detail _sales_return_detail = new sales_return_detail();
                _sales_return_detail.sales_return = sales_return;
                _sales_return_detail.item_description = item.description;
                _sales_return_detail.item = item;
                _sales_return_detail.id_item = item.id_item;

                sales_return.sales_return_detail.Add(_sales_return_detail);
            }
            else
            {
                sales_return_detail sales_return_detail = sales_return.sales_return_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
                sales_return_detail.quantity += 1;
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                
                sales_returnsales_return_detailViewSource.View.Refresh();
                calculate_vat(null, null);
            }));
        }
        private void salesorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Hyperlink Hyperlink = (Hyperlink)sender;
            sales_invoice sales_invoice = (sales_invoice)Hyperlink.Tag;
            if (sales_invoice != null)
            {
                entity.Brillo.Logic.Document Document = new entity.Brillo.Logic.Document();
                Document.Document_PrintInvoice(null, sales_invoice);
            }


        }
        private void sales_return_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            sales_return_detail sales_return_detail = (sales_return_detail)e.NewItem;
            sales_return sales_return = (sales_return)sales_returnDataGrid.SelectedItem;

            sales_return_detail.sales_return = sales_return;
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            dbContext.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            dbContext.Anull();
        }

        private void sales_return_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //calculate_total(sender, e);
            calculate_vat(sender, e);
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            entity.Properties.Settings _entity = new entity.Properties.Settings();
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = dbContext.app_contract.Where(a => a.is_active == true
                                                                        && a.id_company == _entity.company_ID
                                                                        && a.id_condition == app_condition.id_condition).ToList();
                //Selects first Item
                cbxContract.SelectedIndex = 0;
            }
        }

        private void CreateNewCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Properties.Settings _entity = new entity.Properties.Settings();
            using (db db = new db())
            {
                CollectionViewSource conditionViewSource = (CollectionViewSource)FindResource("conditionViewSource");
                db.app_condition.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
                conditionViewSource.Source = db.app_condition.Local;
                //dbContext entity = new dbContext();
                crud_modal.Visibility = Visibility.Visible;
                cntrl.condition condition = new cntrl.condition();
                crud_modal.Children.Add(condition);
            }

        }
        private void hrefEditCondition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Properties.Settings _entity = new entity.Properties.Settings();
            using (db db = new db())
            {
                CollectionViewSource conditionViewSource = (CollectionViewSource)FindResource("conditionViewSource");
                db.app_condition.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
                conditionViewSource.Source = db.app_condition.Local;
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                if (app_condition != null)
                {
                    crud_modal.Visibility = Visibility.Visible;
                    cntrl.condition condition = new cntrl.condition();
                    crud_modal.Children.Add(condition);
                }
            }
        }

        private void hrefAddContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Properties.Settings _entity = new entity.Properties.Settings();
            using (db db = new db())
            {
                CollectionViewSource contractViewSource = (CollectionViewSource)FindResource("contractViewSource");
                db.app_contract.Where(a => a.is_active == true && a.id_company == _entity.company_ID).Load();
                contractViewSource.Source = db.app_contract.Local;

                dbContext entity = new dbContext();
                crud_modal.Visibility = Visibility.Visible;
                cntrl.contract contract = new cntrl.contract();
                contract.app_contractViewSource = contractViewSource;
                contract.MainViewSource = sales_invoiceViewSource;
                contract.curObject = sales_invoiceViewSource.View.CurrentItem;
                contract.entity = entity;
                contract.operationMode = cntrl.Class.clsCommon.Mode.Add;
                contract.isExternalCall = true;
                crud_modal.Children.Add(contract);
            }
        }

        private void hrefEditContract_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Properties.Settings _entity = new entity.Properties.Settings();
            using (db db = new db())
            {
                CollectionViewSource contractViewSource = (CollectionViewSource)FindResource("contractViewSource");
                db.app_contract.Where(a => a.is_active == true && a.id_company == _entity.company_ID).Load();
                contractViewSource.Source = db.app_contract.Local;

                app_contract app_contract = cbxContract.SelectedItem as app_contract;
                if (app_contract != null)
                {
                    dbContext entity = new dbContext();
                    crud_modal.Visibility = System.Windows.Visibility.Visible;
                    cntrl.contract contract = new cntrl.contract();
                    contract.app_contractViewSource = contractViewSource;
                    contract.MainViewSource = sales_invoiceViewSource;
                    contract.curObject = sales_invoiceViewSource.View.CurrentItem;
                    contract.entity = entity;
                    contract.app_contractobject = app_contract;
                    contract.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                    contract.isExternalCall = true;
                    crud_modal.Children.Add(contract);
                }
            }
        }

        private void btnSalesInvoice_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            pnlSalesInvoice = new cntrl.PanelAdv.pnlSalesInvoice();
            pnlSalesInvoice._entity = dbContext;
        //    pnlSalesInvoice.contactViewSource = contactViewSource;
            if (sbxContact.ContactID > 0)
            {
                contact contact = dbContext.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                 pnlSalesInvoice._contact = contact;
            }
          
            pnlSalesInvoice.SalesInvoice_Click += SalesInvoice_Click;
            crud_modal.Children.Add(pnlSalesInvoice);
        }

        public void SalesInvoice_Click(object sender)
        {
            sales_return _sales_return = (sales_return)salesReturnViewSource.View.CurrentItem;

          
            foreach (sales_invoice item in pnlSalesInvoice.selected_sales_invoice)
            {

                foreach (sales_invoice_detail _sales_invoice_detail in item.sales_invoice_detail)
                {
                    if (_sales_return.sales_return_detail.Where(x => x.id_item == _sales_invoice_detail.id_item).Count() == 0)
                    {
                        sales_return_detail sales_return_detail = new sales_return_detail();
                        _sales_return.State = EntityState.Modified;
                        _sales_return.id_condition = item.id_condition;
                        _sales_return.id_contract = item.id_contract;
                        _sales_return.id_currencyfx = item.id_currencyfx;
                        sales_return_detail.id_sales_invoice_detail = _sales_invoice_detail.id_sales_invoice_detail;
                        sales_return_detail.sales_invoice_detail = _sales_invoice_detail;
                        sales_return_detail.sales_return = _sales_return;
                        sales_return_detail.item = _sales_invoice_detail.item;
                        sales_return_detail.id_item = _sales_invoice_detail.id_item;
                        sales_return_detail.quantity = _sales_invoice_detail.quantity - dbContext.sales_return_detail
                                                                                     .Where(x => x.id_sales_invoice_detail == _sales_invoice_detail.id_sales_invoice_detail)
                                                                                     .GroupBy(x => x.id_sales_invoice_detail).Select(x => x.Sum(y => y.quantity)).FirstOrDefault();
                        sales_return_detail.unit_price = _sales_invoice_detail.unit_price;
                        _sales_return.sales_return_detail.Add(sales_return_detail);
                    }

                    salesReturnViewSource.View.Refresh();
                    _sales_return.id_sales_invoice= item.id_sales_invoice;
                   sales_returnsales_return_detailViewSource.View.Refresh();
                    dbContext.Entry(_sales_return).Entity.State = EntityState.Added;
                    crud_modal.Children.Clear();
                    crud_modal.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
        {
            sales_return sales_return = sales_returnDataGrid.SelectedItem as sales_return;
            if (sales_return != null)
            {
                entity.Brillo.Document.Start.Manual(sales_return, sales_return.app_document_range);
            }
            else
            {
                toolBar.msgWarning("Please select");
            }
        }

      

        //private void cbxSalesInvoice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (cbxSalesInvoice.Data != null)
        //    {
        //        sales_invoice sales_invoice = cbxSalesInvoice.Data as sales_invoice;
        //        if (sales_invoice != null)
        //        {
        //            ButtonImport_Click(sender, e);
        //        }
        //    }

        //}

        //private void cbxSalesInvoice_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {


        //        if (cbxSalesInvoice.Data != null)
        //        {
        //            sales_invoice sales_invoice = cbxSalesInvoice.Data as sales_invoice;
        //            if (sales_invoice != null)
        //            {
        //                ButtonImport_Click(sender, e);
        //            }
        //        }
        //    }

        //}
    }
}
